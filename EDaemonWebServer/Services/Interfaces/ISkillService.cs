using EDaemonWebServer.Entities.Skills;

namespace EDaemonWebServer.Services.Interfaces
{
    public interface ISkillService
    {
        Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync();
        Task<BasicSkill?> GetBasicSkillAsync(int id);
    }
}
