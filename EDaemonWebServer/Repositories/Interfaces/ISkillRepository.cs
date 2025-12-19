using EDaemonWebServer.Entities.Skills;

namespace EDaemonWebServer.Repositories.Interfaces
{
    public interface ISkillRepository
    {
        Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync();
        Task<BasicSkill?> GetBasicSkillByIdAsync(int id);
    }
}
