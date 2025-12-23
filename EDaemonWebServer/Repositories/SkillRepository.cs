using EDaemonWebServer.Domain.Skills;
using EDaemonWebServer.Repositories.Interfaces;

namespace EDaemonWebServer.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        public Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync(BasicSkillsFilter filter)
        {
            throw new NotImplementedException();
        }

        public Task<BasicSkill?> GetBasicSkillByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
