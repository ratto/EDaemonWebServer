using EDaemonWebServer.Domain.Skills;

namespace EDaemonWebServer.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync(BasicSkillsFilter filter);
        Task<BasicSkill?> GetBasicSkillByIdAsync(int id);
    }
}
