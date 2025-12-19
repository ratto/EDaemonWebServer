using EDaemonWebServer.Entities.Skills;
using EDaemonWebServer.Repositories.Interfaces;

namespace EDaemonWebServer.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        public Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BasicSkill?> GetBasicSkillByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
