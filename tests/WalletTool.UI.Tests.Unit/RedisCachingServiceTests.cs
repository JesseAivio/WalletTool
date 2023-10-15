using System.Text.Json;
using FluentAssertions;
using NSubstitute;
using StackExchange.Redis;
using WalletTool.UI.Services;

namespace WalletTool.UI.Tests.Unit;

public class RedisCachingServiceTests : IDisposable, IAsyncDisposable
{
    private readonly RedisCachingService _sut;
    private readonly IConnectionMultiplexer _redis = Substitute.For<IConnectionMultiplexer>();
    private readonly IDatabase _db = Substitute.For<IDatabase>();

    public RedisCachingServiceTests()
    {
        _redis.GetDatabase().Returns(_db);
        _sut = new RedisCachingService(_redis);
    }
    
    [Fact]
    public async Task GetAsync_ShouldReturnData_WhenKeyExists()
    {
        // Arrange
        string testKey = "testKey";
        string serializedData = "{\"Name\":\"test\"}";
        _db.StringGetAsync(testKey, default).ReturnsForAnyArgs(new RedisValue(serializedData));

        // Act
        var result = await _sut.GetAsync<TestClass>(testKey);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("test");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefault_WhenKeyDoesNotExist()
    {
        // Arrange
        string testKey = "testKey";
        _db.StringGetAsync(testKey, default).ReturnsForAnyArgs(RedisValue.Null);

        // Act
        var result = await _sut.GetAsync<TestClass>(testKey);

        // Assert
        result.Should().BeNull();
    }
// TODO: Fix this unit test for RedisCachingService.SetAsync.
    /*[Fact]
    public async Task SetAsync_ShouldSerializeData_AndStoreInCache()
    {
        
        // Arrange
        string testKey = "testKey";
        var data = new TestClass() { Name = "test" };
        TimeSpan duration = TimeSpan.FromMinutes(10);

        // Act
        await _sut.SetAsync(testKey, data, duration);

        // Assert
        await _db.Received().StringSetAsync(
            Arg.Is<RedisKey>(key => key == testKey),
            Arg.Is<RedisValue>(value => value.ToString().Contains("\"Name\":\"test\"")),
            Arg.Is<TimeSpan?>(ts => ts == duration),
            Arg.Any<When>(), 
            Arg.Any<CommandFlags>());
    }*/

    [Fact]
    public async Task InvalidateAsync_ShouldDeleteKeyFromCache()
    {
        // Arrange
        string testKey = "testKey";

        // Act
        await _sut.InvalidateAsync(testKey);

        // Assert
        await _db.Received().KeyDeleteAsync(testKey, Arg.Any<CommandFlags>());
    }

    private class TestClass
    {
        public string Name { get; set; }
    }

    public void Dispose()
    {
        _db.ClearReceivedCalls();
        _redis.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _redis.DisposeAsync();
    }
}