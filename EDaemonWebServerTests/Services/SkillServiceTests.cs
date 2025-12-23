using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDaemonWebServer.Domain.Skills;
using EDaemonWebServer.Repositories.Interfaces;
using EDaemonWebServer.Services;
using Moq;
using Xunit;

namespace EDaemonWebServerTests.Services
{
    // London-style unit test: mock the ISkillRepository and test SkillService in isolation
    public class SkillServiceTests
    {
        [Fact]
        public async Task GetAllBasicSkillsAsync_ForwardsRepositoryData()
        {
            // Arrange (London style): set up the mock repository to return a known dataset
            var mockRepo = new Mock<ISkillRepository>(MockBehavior.Strict);

            var expected = new List<BasicSkill>
            {
                new BasicSkill { Id = 1, Name = "Acrobatics" },
                new BasicSkill { Id = 2, Name = "Stealth" }
            };

            mockRepo
                .Setup(r => r.GetAllBasicSkillsAsync(It.IsAny<BasicSkillsFilter>()))
                .ReturnsAsync(expected)
                .Verifiable();

            var service = new SkillService(mockRepo.Object);

            // Act
            var result = await service.GetAllBasicSkillsAsync(new BasicSkillsFilter());

            // Assert: service should return the same sequence from repository
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(expected.Count, list.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i].Id, list[i].Id);
                Assert.Equal(expected[i].Name, list[i].Name);
            }

            mockRepo.Verify();
        }

        [Fact]
        public async Task GetBasicSkillAsync_ForwardsRepositoryData()
        {
            // Arrange: set up repository to return a known skill for id 1
            var mockRepo = new Mock<ISkillRepository>(MockBehavior.Strict);

            var expected = new BasicSkill { Id = 1, Name = "Acrobatics" };

            mockRepo
                .Setup(r => r.GetBasicSkillByIdAsync(It.Is<int>(i => i == expected.Id)))
                .ReturnsAsync(expected)
                .Verifiable();

            var service = new SkillService(mockRepo.Object);

            // Act
            var result = await service.GetBasicSkillAsync(expected.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Id, result!.Id);
            Assert.Equal(expected.Name, result.Name);

            mockRepo.Verify();
        }

        [Fact]
        public async Task GetBasicSkillAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange: repository returns null for unknown id
            var mockRepo = new Mock<ISkillRepository>(MockBehavior.Strict);

            const int unknownId = 99;

            mockRepo
                .Setup(r => r.GetBasicSkillByIdAsync(It.Is<int>(i => i == unknownId)))
                .ReturnsAsync((BasicSkill?)null)
                .Verifiable();

            var service = new SkillService(mockRepo.Object);

            // Act
            var result = await service.GetBasicSkillAsync(unknownId);

            // Assert: service should return null when repository has no item
            Assert.Null(result);

            mockRepo.Verify();
        }
    }
}
