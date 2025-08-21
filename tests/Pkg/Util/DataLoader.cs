using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using app.Pkg;
using app.Pkg.Model;
using Microsoft.Extensions.DependencyInjection;
using tests.Infra;
using Xunit.Abstractions;

namespace tests.Pkg.Util;

public class DataLoader(TestFixture fixture, ITestOutputHelper testOutputHelper) : TestsBase
{
    /// Loading data from JSON file downloaded from:<br/>
    /// https://raw.githubusercontent.com/Macrometacorp/datasets/master/hotel-reviews/hotels.json
    /// And then transformed to fix issues:<br/>
    /// - Replacing \u00ef\u00bf\u00bd with \u00e9 in json file had lots of fixes<br/>
    /// - Inconsistent encoding fixed by replacing regexp: &lt;U\+([[:alnum:]]{4})> with \\u$1

    [Fact]
    public async Task LoadHotelReviewData()
    {
        var reviews = await LoadArrayAsync("Downloads", "hotels-macrometa-2.json");
        Assert.Equal(10_000, reviews.Count);

        await fixture.ResetAsync();
        await fixture.WithScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<AppDb>();
            for (var index = 0; index < reviews.Count; index++)
            {
                var review = reviews[(Index)index]!.AsObject();

                T? ReviewProp<T>(string propName)
                {
                    var node = review[propName];
                    if (node == null)
                    {
                        return default;
                    }

                    return node.GetValue<T>();
                }

                var hr = new HotelReview { Id = $"hotels-macrometa-{index}" };
                hr.Property = ReviewProp<string>("Property_Name")!;
                hr.Rating = ReviewProp<int>("Review Rating");
                hr.Text = ReviewProp<string>("Review Text")!;
                hr.Title = ReviewProp<string>("Review Title")!;
                var dateStr = ReviewProp<string>("Date Of Review");
                hr.Date = DateOnly.ParseExact(dateStr ?? "1/1/0001", "M/d/yyyy", CultureInfo.InvariantCulture);
                db.HotelReview.Add(hr);

                var reviewLocation = ReviewProp<string>("Location Of The Reviewer");
                hr.Language = LocationToLanguage(reviewLocation, hr.Text);

                // testOutputHelper.WriteLine(JsonSerializer.Serialize(hr));

                // if (index >= 100)
                // {
                //     break;
                // }
            }

            await db.SaveChangesAsync();
        });
    }

    private static LanguageEnum LocationToLanguage(string? loc, string reviewText) => loc switch
    {
        var s when s != null && s.EndsWith("France") => LanguageEnum.French,
        var s when s != null
                   && s.EndsWith("Greece")
                   && reviewText.Contains('έ')
            => LanguageEnum.Greek,
        _ => LanguageEnum.English
    };

    private async Task<JsonArray> LoadArrayAsync(params string[] relativePathElems)
    {
        var relativePath = Path.Combine(relativePathElems);
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string filePath = Path.Combine(homeDir, relativePath);
        Assert.True(File.Exists(filePath));
        await using var stream = File.OpenRead(filePath);
        var arr = await JsonSerializer.DeserializeAsync<JsonArray>(stream);
        Assert.NotNull(arr);
        testOutputHelper.WriteLine($"Loaded {arr.Count.ToString()} recs from {relativePath}");

        return arr;
    }
}