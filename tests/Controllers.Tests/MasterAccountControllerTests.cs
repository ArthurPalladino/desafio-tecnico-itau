using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Controllers.Tests
{
    public class MasterAccountControllerTests
    {
        private readonly Mock<IMasterAccountService> _masterAccountServiceMock;
        private readonly Mock<IRecommendationBasketService> _basketServiceMock;
        private readonly MasterAccountController _controller;

        public MasterAccountControllerTests()
        {
            _masterAccountServiceMock = new Mock<IMasterAccountService>();
            _basketServiceMock = new Mock<IRecommendationBasketService>();
            _controller = new MasterAccountController(_basketServiceMock.Object, _masterAccountServiceMock.Object);
        }

        [Fact]
        public async Task GetMasterCustody_ShouldReturnOk_WhenCustodyExists()
        {
            var response = new MasterCustodyResponse(
                new MasterAccountDto(1, "MST-000001", "MASTER"),
                new List<MasterAssetDto>(),
                1000m
            );

            _masterAccountServiceMock.Setup(x => x.GetMasterCustodyAsync()).ReturnsAsync(response);

            var result = await _controller.GetMasterCustody();

            result.Should().BeOfType<ActionResult<MasterCustodyResponse>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetMasterCustody_ShouldThrow_WhenMasterAccountNotFound()
        {
            _masterAccountServiceMock.Setup(x => x.GetMasterCustodyAsync()).ThrowsAsync(new CustomException("A conta master informada é inválida."));

            Func<Task> act = async () => await _controller.GetMasterCustody();

            await act.Should().ThrowAsync<CustomException>();
        }
    }
}
