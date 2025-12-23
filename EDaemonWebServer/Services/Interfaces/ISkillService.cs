using EDaemonWebServer.Domain.Skills;

namespace EDaemonWebServer.Services.Interfaces
{
    public interface ISkillService
    {
        Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync(BasicSkillsFilter filter);
        Task<BasicSkill?> GetBasicSkillAsync(int id);
    }
}
