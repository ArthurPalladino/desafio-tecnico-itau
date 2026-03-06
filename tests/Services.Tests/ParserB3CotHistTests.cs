using Xunit;
using Moq;
using FluentAssertions;
using Repositories.Interfaces;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Text;

namespace Tests.Services.Tests
{
    public class ParserB3CotHistTests
    {
        private readonly Mock<ITickerRepository> _tickerRepoMock;
        private readonly ParserB3CotHist _service;

        public ParserB3CotHistTests()
        {
            _tickerRepoMock = new Mock<ITickerRepository>();
            _service = new ParserB3CotHist(_tickerRepoMock.Object);
        }

        [Fact]
        public async Task ParseAndSyncDatabaseAsync_ShouldThrowException_WhenDirectoryNotFound()
        {
            _tickerRepoMock.Setup(x => x.GetDistinctDatesAsync()).ReturnsAsync(new List<DateTime>());

            Func<Task> act = async () => await _service.ParseAndSyncDatabaseAsync();

            await act.Should().ThrowAsync<CustomException>().WithMessage("Arquivo COTAHIST nao encontrado para a data.");
        }

        [Fact]
        public async Task ParseAndSyncDatabaseAsync_ShouldNotProcess_WhenDateAlreadyInDatabase()
        {
            var existingDate = new DateTime(2026, 03, 05);
            _tickerRepoMock.Setup(x => x.GetDistinctDatesAsync()).ReturnsAsync(new List<DateTime> { existingDate });

            var task = _service.ParseAndSyncDatabaseAsync();

            _tickerRepoMock.Verify(x => x.GetTickersByDateDictAsync(It.IsAny<DateTime>()), Times.Never);
        }
    }
}
