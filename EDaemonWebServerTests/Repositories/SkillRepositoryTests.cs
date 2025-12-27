using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using EDaemonWebServer.Repositories;
using EDaemonWebServer.Domain.Skills;
using EDaemonWebServer.Utils.Enums;

namespace EDaemonWebServerTests.Repositories
{
    public class SkillRepositoryTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly SqliteConnection _keepAliveConnection;

        public SkillRepositoryTests()
        {
            // create a shared in-memory sqlite database name and keep one connection open
            _dbPath = $"file:memdb_{Guid.NewGuid():N}?mode=memory&cache=shared";
            Environment.SetEnvironmentVariable("DATABASE_PATH", _dbPath);

            _keepAliveConnection = new SqliteConnection($"Data Source={_dbPath}");
            _keepAliveConnection.Open();

            CreateSchemaAndSeed();
        }

        public void Dispose()
        {
            try
            {
                _keepAliveConnection?.Close();
                _keepAliveConnection?.Dispose();
                Environment.SetEnvironmentVariable("DATABASE_PATH", null);
            }
            catch { }
        }

        private void CreateSchemaAndSeed()
        {
            using var cmd = _keepAliveConnection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS BASIC_SKILLS (
                  Id INTEGER PRIMARY KEY,
                  Name TEXT NOT NULL,
                  BaseAttribute INTEGER,
                  SkillGroup TEXT,
                  TrainedOnly INTEGER,
                  Description TEXT NOT NULL
                );
            ";
            cmd.ExecuteNonQuery();

            // seed two skills using a transaction on the shared connection
            using var tr = _keepAliveConnection.BeginTransaction();
            using var insert = _keepAliveConnection.CreateCommand();
            insert.CommandText = "INSERT INTO BASIC_SKILLS (Name, BaseAttribute, SkillGroup, TrainedOnly, Description) VALUES (@n, @b, @g, @t, @d);";
            insert.Transaction = tr;

            insert.Parameters.AddWithValue("@n", "Acrobatics");
            insert.Parameters.AddWithValue("@b", (int)AttributeType.Dexterity);
            insert.Parameters.AddWithValue("@g", "Physical");
            insert.Parameters.AddWithValue("@t", 0);
            insert.Parameters.AddWithValue("@d", "Movement maneuvers");
            insert.ExecuteNonQuery();

            insert.Parameters.Clear();
            insert.Parameters.AddWithValue("@n", "Stealth");
            insert.Parameters.AddWithValue("@b", (int)AttributeType.Agility);
            insert.Parameters.AddWithValue("@g", "Physical");
            insert.Parameters.AddWithValue("@t", 1);
            insert.Parameters.AddWithValue("@d", "Hide from sight");
            insert.ExecuteNonQuery();

            tr.Commit();
        }

        private SkillRepository CreateRepository()
        {
            var mockConfig = new Mock<IConfiguration>();
            return new SkillRepository(mockConfig.Object);
        }

        [Fact]
        public async Task GetBasicSkillByIdAsync_ReturnsSkill_WhenExists()
        {
            // arrange
            var repo = CreateRepository();

            // get id of first inserted row (should be 1)
            var skill = await repo.GetBasicSkillByIdAsync(1);

            // assert
            Assert.NotNull(skill);
            Assert.Equal(1, skill!.Id);
            Assert.Equal("Acrobatics", skill.Name);
            Assert.Equal(AttributeType.Dexterity, skill.BaseAttribute);
            Assert.Equal("Physical", skill.SkillGroup);
            Assert.False(skill.TrainedOnly);
            Assert.Equal("Movement maneuvers", skill.Description);
        }

        [Fact]
        public async Task GetBasicSkillByIdAsync_ReturnsNull_WhenNotFound()
        {
            var repo = CreateRepository();
            var skill = await repo.GetBasicSkillByIdAsync(9999);
            Assert.Null(skill);
        }

        [Fact]
        public async Task GetAllBasicSkillsAsync_ReturnsAll_WhenNoFilter()
        {
            var repo = CreateRepository();
            var filter = new BasicSkillsFilter();
            var list = (await repo.GetAllBasicSkillsAsync(filter)).ToList();
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task GetAllBasicSkillsAsync_AppliesFilters()
        {
            var repo = CreateRepository();

            // Name like
            var f1 = new BasicSkillsFilter { Name = "acro" };
            var r1 = (await repo.GetAllBasicSkillsAsync(f1)).ToList();
            Assert.Single(r1);
            Assert.Equal("Acrobatics", r1[0].Name);

            // BaseAttribute
            var f2 = new BasicSkillsFilter { BaseAttribute = AttributeType.Agility };
            var r2 = (await repo.GetAllBasicSkillsAsync(f2)).ToList();
            Assert.Single(r2);
            Assert.Equal("Stealth", r2[0].Name);

            // SkillGroup like
            var f3 = new BasicSkillsFilter { SkillGroup = "Phys" };
            var r3 = (await repo.GetAllBasicSkillsAsync(f3)).ToList();
            Assert.Equal(2, r3.Count);

            // TrainedOnly
            var f4 = new BasicSkillsFilter { TrainedOnly = true };
            var r4 = (await repo.GetAllBasicSkillsAsync(f4)).ToList();
            Assert.Single(r4);
            Assert.Equal("Stealth", r4[0].Name);

            // Description like
            var f5 = new BasicSkillsFilter { Description = "hide" };
            var r5 = (await repo.GetAllBasicSkillsAsync(f5)).ToList();
            Assert.Single(r5);
            Assert.Equal("Stealth", r5[0].Name);
        }
    }
}
