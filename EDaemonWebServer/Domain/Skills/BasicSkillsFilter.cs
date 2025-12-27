using EDaemonWebServer.Utils.Enums;

namespace EDaemonWebServer.Domain.Skills
{
    public class BasicSkillsFilter
    {
        public string? Name { get; set; } = null;
        public AttributeType? BaseAttribute { get; set; } = null;
        public string? SkillGroup { get; set; } = null;
        public bool? TrainedOnly { get; set; } = null;
        public string? Description { get; set; } = null;
    }
}
