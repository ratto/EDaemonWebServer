using System;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using EDaemonWebServer.Repositories;
using EDaemonWebServer.Utils.Enums;

namespace EDaemonWebServerTests.Repositories
{
    // A minimal concrete implementation to exercise protected members for testing
    public class TestableBaseRepository : BaseRepository
    {
        public TestableBaseRepository(IConfiguration configuration) : base(configuration) { }

        public string ConnectionString => _connectionString;

        public AttributeType InvokeIntToBaseAttribute(object? raw) => IntToBaseAttribute(raw);

        public bool InvokeIntToBool(object? raw) => IntToBool(raw);
    }

    public class BaseRepositoryTests
    {
        [Fact]
        public void Constructor_Throws_When_NoDatabasePath()
        {
            // Preserve any existing environment value and restore it after the test to avoid
            // affecting other tests (tests may run in parallel in the same process).
            var prev = Environment.GetEnvironmentVariable("DATABASE_PATH");
            try
            {
                Environment.SetEnvironmentVariable("DATABASE_PATH", null);

                var mockConfig = new Mock<IConfiguration>();
                // Use SetupGet for the indexer to ensure Moq intercepts the configuration lookup
                mockConfig.SetupGet(c => c["DatabasePath"]).Returns((string?)null);

                Assert.Throws<InvalidOperationException>(() => new TestableBaseRepository(mockConfig.Object));
            }
            finally
            {
                Environment.SetEnvironmentVariable("DATABASE_PATH", prev);
            }
        }

        [Fact]
        public void Constructor_UsesEnvironmentVariable_WhenSet()
        {
            Environment.SetEnvironmentVariable("DATABASE_PATH", "mydb.sqlite");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["DatabasePath"]).Returns((string?)null);

            var repo = new TestableBaseRepository(mockConfig.Object);

            Assert.Equal("Data Source=mydb.sqlite", repo.ConnectionString);

            Environment.SetEnvironmentVariable("DATABASE_PATH", null);
        }

        [Theory]
        [InlineData(null, AttributeType.None)]
        [InlineData(1, AttributeType.Strength)]
        [InlineData(2, AttributeType.Agility)]
        [InlineData(999, AttributeType.None)]
        public void IntToBaseAttribute_Converts_Correctly(object? raw, AttributeType expected)
        {
            // Ensure the repository can be constructed (provide a DB path via env)
            Environment.SetEnvironmentVariable("DATABASE_PATH", "tmp");
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["DatabasePath"]).Returns((string?)null);
            var repo = new TestableBaseRepository(mockConfig.Object);

            var result = repo.InvokeIntToBaseAttribute(raw);

            Assert.Equal(expected, result);

            Environment.SetEnvironmentVariable("DATABASE_PATH", null);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(-1, true)]
        [InlineData("1", true)]
        [InlineData("abc", false)]
        public void IntToBool_Converts_Correctly(object? raw, bool expected)
        {
            Environment.SetEnvironmentVariable("DATABASE_PATH", "tmp");
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["DatabasePath"]).Returns((string?)null);
            var repo = new TestableBaseRepository(mockConfig.Object);

            var result = repo.InvokeIntToBool(raw);

            Assert.Equal(expected, result);

            Environment.SetEnvironmentVariable("DATABASE_PATH", null);
        }
    }
}
