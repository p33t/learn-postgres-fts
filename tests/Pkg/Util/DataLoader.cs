using System.Text.Json;
using System.Text.Json.Nodes;
using tests.Infra;
using Xunit.Abstractions;

namespace tests.Pkg.Util;

public class DataLoader(ITestOutputHelper testOutputHelper) : TestsBase
{
    /// Loading data from JSON file downloaded from:<br/>
    /// https://raw.githubusercontent.com/Macrometacorp/datasets/master/hotel-reviews/hotels.json
    [Fact]
    public async Task LoadHotelReviewData()
    {
        var reviews = await LoadArrayAsync("Downloads", "hotels-macrometa.json");
        Assert.Equal(100, reviews.Count());
    }

    private async Task<List<JsonNode?>> LoadArrayAsync(params string[] relativePathElems)
    {
        var relativePath = Path.Combine(relativePathElems);
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string filePath = Path.Combine(homeDir, relativePath);
        Assert.True(File.Exists(filePath));
        await using var stream = File.OpenRead(filePath);
        var arr = await JsonSerializer.DeserializeAsync<JsonArray>(stream);
        Assert.NotNull(arr);
        testOutputHelper.WriteLine($"Loaded {arr.Count.ToString()} recs from {relativePath}");
        return arr.Take(100).ToList();
    }
}