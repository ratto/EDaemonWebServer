using EDaemonWebServer.Utils.Enums;
using System.ComponentModel.DataAnnotations;

namespace EDaemonWebServer.Domain.Skills
{
    public class BasicSkillsFilter
    {
        public string Name { get; set; } = String.Empty;
        public AttributeType BaseAttribute { get; set; } = AttributeType.None;
        public string SkillGroup { get; set; } = String.Empty;
        public bool TrainedOnly { get; set; } = false;
        public string Description { get; set; } = String.Empty;
    }
}
