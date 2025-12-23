using EDaemonWebServer.Domain.Skills;
using EDaemonWebServer.Repositories.Interfaces;
using EDaemonWebServer.Services.Interfaces;

namespace EDaemonWebServer.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public Task<IEnumerable<BasicSkill>> GetAllBasicSkillsAsync(BasicSkillsFilter filter)
        {
            return _skillRepository.GetAllBasicSkillsAsync(filter);
        }

        public Task<BasicSkill?> GetBasicSkillAsync(int id)
        {
            return _skillRepository.GetBasicSkillByIdAsync(id);
        }
    }
}
