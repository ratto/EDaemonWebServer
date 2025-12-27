using System.Text;
using EDaemonWebServer.Domain.Skills;
using EDaemonWebServer.Repositories.Interfaces;
using EDaemonWebServer.Utils.Enums;

namespace EDaemonWebServer.Repositories
{
    public class SkillRepository : BaseRepository, ISkillRepository
    {
        public SkillRepository(IConfiguration configuration) : base(configuration) { }

        public async Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync(BasicSkillsFilter filter)
        {
            var skills = new List<BasicSkill>();

            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("  Id,");
            sb.AppendLine("  Name,");
            sb.AppendLine("  BaseAttribute,");
            sb.AppendLine("  SkillGroup,");
            sb.AppendLine("  TrainedOnly,");
            sb.AppendLine("  Description");
            sb.AppendLine("FROM BASIC_SKILLS");
            sb.AppendLine("WHERE 1=1");

            if (filter.Name is not null)
            {
                sb.AppendLine("  AND Name LIKE @name");
            }

            if (filter.BaseAttribute is not null)
            {
                sb.AppendLine("  AND BaseAttribute = @baseAttribute");
            }

            if (filter.SkillGroup is not null)
            {
                sb.AppendLine("  AND SkillGroup LIKE @skillGroup");
            }

            if (filter.TrainedOnly is not null)
            {
                sb.AppendLine("  AND TrainedOnly = @trainedOnly");
            }

            if (filter.Description is not null)
            {
                sb.AppendLine("  AND Description LIKE @description");
            }

            sb.AppendLine(";");

            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = sb.ToString();

            // Add parameters for provided filters
            if (filter.Name is not null)
            {
                command.Parameters.AddWithValue("@name", $"%{filter.Name}%");
            }

            if (filter.BaseAttribute is not null)
            {
                // store enum as its underlying integer value
                command.Parameters.AddWithValue("@baseAttribute", (int)filter.BaseAttribute.Value);
            }

            if (filter.SkillGroup is not null)
            {
                command.Parameters.AddWithValue("@skillGroup", $"%{filter.SkillGroup}%");
            }

            if (filter.TrainedOnly is not null)
            {
                command.Parameters.AddWithValue("@trainedOnly", filter.TrainedOnly.Value ? 1 : 0);
            }

            if (filter.Description is not null)
            {
                command.Parameters.AddWithValue("@description", $"%{filter.Description}%");
            }

            using var reader = await command.ExecuteReaderAsync();
            var ordId = reader.GetOrdinal("Id");
            var ordName = reader.GetOrdinal("Name");
            var ordBaseAttr = reader.GetOrdinal("BaseAttribute");
            var ordSkillGroup = reader.GetOrdinal("SkillGroup");
            var ordTrained = reader.GetOrdinal("TrainedOnly");
            var ordDescription = reader.GetOrdinal("Description");

            while (await reader.ReadAsync())
            {
                var skill = new BasicSkill
                {
                    Id = reader.IsDBNull(ordId) ? 0 : reader.GetInt32(ordId),
                    Name = reader.IsDBNull(ordName) ? string.Empty : reader.GetString(ordName),
                    Description = reader.IsDBNull(ordDescription) ? string.Empty : reader.GetString(ordDescription),
                    SkillGroup = reader.IsDBNull(ordSkillGroup) ? null : reader.GetString(ordSkillGroup)
                };

                // TODO: test if IntToBaseAttribute works correctly; if so, refactor the process bellow
                // BaseAttribute: integer stored in DB
                if (!reader.IsDBNull(ordBaseAttr))
                {
                    var raw = reader.GetValue(ordBaseAttr);
                    skill.BaseAttribute = IntToBaseAttribute(raw);
                }
                else
                {
                    skill.BaseAttribute = AttributeType.None;
                }

                // TrainedOnly: 0/1 stored in DB
                if (!reader.IsDBNull(ordTrained))
                {
                    var raw = reader.GetValue(ordTrained);
                    skill.TrainedOnly = IntToBool(raw);
                }
                else
                {
                    skill.TrainedOnly = false;
                }

                skills.Add(skill);
            }

            return skills;
        }

        public async Task<BasicSkill?> GetBasicSkillByIdAsync(int id)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("  Id,");
            sb.AppendLine("  Name,");
            sb.AppendLine("  BaseAttribute,");
            sb.AppendLine("  SkillGroup,");
            sb.AppendLine("  TrainedOnly,");
            sb.AppendLine("  Description");
            sb.AppendLine("FROM BASIC_SKILLS");
            sb.AppendLine("WHERE Id = @id;");

            using var connection = GetConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = sb.ToString();
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            var ordId = reader.GetOrdinal("Id");
            var ordName = reader.GetOrdinal("Name");
            var ordBaseAttr = reader.GetOrdinal("BaseAttribute");
            var ordSkillGroup = reader.GetOrdinal("SkillGroup");
            var ordTrained = reader.GetOrdinal("TrainedOnly");
            var ordDescription = reader.GetOrdinal("Description");

            var skill = new BasicSkill
            {
                Id = reader.IsDBNull(ordId) ? 0 : reader.GetInt32(ordId),
                Name = reader.IsDBNull(ordName) ? string.Empty : reader.GetString(ordName),
                Description = reader.IsDBNull(ordDescription) ? string.Empty : reader.GetString(ordDescription),
                SkillGroup = reader.IsDBNull(ordSkillGroup) ? null : reader.GetString(ordSkillGroup)
            };

            // BaseAttribute: converter inteiro do BD para AttributeType
            if (!reader.IsDBNull(ordBaseAttr))
            {
                var raw = reader.GetValue(ordBaseAttr);
                skill.BaseAttribute = IntToBaseAttribute(raw);
            }
            else
            {
                skill.BaseAttribute = AttributeType.None;
            }

            // TrainedOnly: 0/1 -> bool
            if (!reader.IsDBNull(ordTrained))
            {
                var raw = reader.GetValue(ordTrained);
                skill.TrainedOnly = IntToBool(raw);
            }
            else
            {
                skill.TrainedOnly = false;
            }

            return skill;
        }
    }
}
