using EDaemonWebServer.Utils.Enums;
using System.ComponentModel.DataAnnotations;

namespace EDaemonWebServer.Domain.Skills
{
    public class BasicSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public AttributeType BaseAttribute { get; set; }
        public string SkillGroup { get; set; }
        public bool TrainedOnly { get; set; }
        [MaxLength(150)]
        public string Description { get; set; }
    }
}
