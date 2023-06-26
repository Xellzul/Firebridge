using Firebridge.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firebridge.Service.Tests;

public class FingerprintServiceTests
{
    [Test]
    public void FingerprintIsNotEmpty()
    {
        var mock = new Mock<ILogger<FingerprintService>>(MockBehavior.Loose);
        var fingerprintService = new FingerprintService(mock.Object);

        var guid = fingerprintService.GetFingerprint();

        Assert.IsNotNull(guid);
        Assert.That(guid, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void FingerprintIsSameForMultipleCalls()
    {
        var mock = new Mock<ILogger<FingerprintService>>(MockBehavior.Loose);
        var fingerprintService = new FingerprintService(mock.Object);

        var guid = fingerprintService.GetFingerprint();
        var guid2 = fingerprintService.GetFingerprint();

        Assert.That(guid2, Is.EqualTo(guid));
    }
}