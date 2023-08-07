using Firbridge.Agent;

namespace Firebridge.Agent.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async void Test1()
    {
        var msIn = new MemoryStream();
        var msOut = new MemoryStream();
        var msErr = new MemoryStream();

        Console.SetIn(new StreamReader(msIn));
        Console.SetOut(new StreamWriter(msOut));
        Console.SetError(new StreamWriter(msErr));

        var task = Program.Main(Array.Empty<string>());



        Assert.Pass();
    }
}