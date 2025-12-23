using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EDaemonWebServer.Controllers;
using EDaemonWebServer.Domain.Skills;
using EDaemonWebServer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EDaemonWebServerTests.Controllers
{
    // London-style unit tests: mock ISkillService and test SkillController in isolation
    public class SkillControllerTests
    {
        [Fact]
        public async Task GetAllBasicSkills_ReturnsOkWithSkills()
        {
            // Arrange
            var mockService = new Mock<ISkillService>(MockBehavior.Strict);
            var filter = new BasicSkillsFilter { Name = "acro" };

            var expected = new List<BasicSkill>
            {
                new BasicSkill { Id = 1, Name = "Acrobatics" },
                new BasicSkill { Id = 2, Name = "Stealth" }
            };

            mockService
                .Setup(s => s.GetAllBasicSkillsAsync(It.Is<BasicSkillsFilter>(f => f == filter)))
                .ReturnsAsync(expected)
                .Verifiable();

            var controller = new SkillController(mockService.Object);

            // Act
            var actionResult = await controller.GetAllBasicSkills(filter);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var actual = Assert.IsAssignableFrom<IEnumerable<BasicSkill>>(ok.Value);
            var list = actual.ToList();
            Assert.Equal(expected.Count, list.Count);
            Assert.Equal(expected[0].Id, list[0].Id);
            Assert.Equal(expected[1].Name, list[1].Name);

            mockService.Verify();
        }

        [Fact]
        public async Task GetBasicSkill_ReturnsOk_WhenFound()
        {
            // Arrange
            var mockService = new Mock<ISkillService>(MockBehavior.Strict);
            var expected = new BasicSkill { Id = 5, Name = "Sense Motive" };

            mockService
                .Setup(s => s.GetBasicSkillAsync(It.Is<int>(i => i == expected.Id)))
                .ReturnsAsync(expected)
                .Verifiable();

            var controller = new SkillController(mockService.Object);

            // Act
            var actionResult = await controller.GetBasicSkill(expected.Id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            var actual = Assert.IsType<BasicSkill>(ok.Value);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);

            mockService.Verify();
        }

        [Fact]
        public async Task GetBasicSkill_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<ISkillService>(MockBehavior.Strict);
            const int missingId = 99;

            mockService
                .Setup(s => s.GetBasicSkillAsync(It.Is<int>(i => i == missingId)))
                .ReturnsAsync((BasicSkill?)null)
                .Verifiable();

            var controller = new SkillController(mockService.Object);

            // Act
            var actionResult = await controller.GetBasicSkill(missingId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);

            mockService.Verify();
        }
    }
}
