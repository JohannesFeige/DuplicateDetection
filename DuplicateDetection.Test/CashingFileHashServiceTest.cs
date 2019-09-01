using DuplicateDetection.Abstractions;
using NSubstitute;
using Shouldly;
using Xunit;

namespace DuplicateDetection.Test
{
    public class CashingFileHashServiceTest
    {
        private readonly IFileHashService service;
        private readonly IFileHashService fileHashService = Substitute.For<IFileHashService>();

        public CashingFileHashServiceTest()
        {
            service = new CashingFileHashService(fileHashService);
        }

        [Fact]
        public void ShouldCalculateHashViaInnerService()
        {
            var filePath = "foo.txt";
            var hash = new byte[] { 0x00 };
            fileHashService.CalculateHash(filePath).Returns(hash);

            var calculatedHash = service.CalculateHash(filePath);
            calculatedHash.ShouldBe(hash);
        }

        [Fact]
        public void ShouldCalculateHashOnlyOnce()
        {
            var filePath = "foo.txt";
            var hash = new byte[] { 0x00 };
            fileHashService.CalculateHash(filePath).Returns(hash);

            service.CalculateHash(filePath);
            fileHashService.ClearReceivedCalls();

            var calculatedHash = service.CalculateHash(filePath);
            calculatedHash.ShouldBe(hash);
            fileHashService.DidNotReceiveWithAnyArgs().CalculateHash(filePath);
        }
    }
}
