namespace app.Pkg.Model;

/// Second version of full-text-search. Features: <br/>
/// - Index per search language (EN & FR)
/// - Typically 1 combined text translation (with single weight) but can add more later
/// -- Soft-deleted records can have 0
/// - No need for 'LEAN' approach because FTS fields are in a related entity
public class HotelReview2 : HotelReviewBase, IHasFtses
{
    /// Create from a V1
    public static HotelReview2 CreateFrom(HotelReviewBase hrBase)
    {
        var result = new HotelReview2
        {
            Id = hrBase.Id,
            Date = hrBase.Date,
            Rating = hrBase.Rating,
            Language = hrBase.Language,
            Property = hrBase.Property,
            Text = hrBase.Text,
            Title = hrBase.Title
        };

        result.Ftses =
        [
            new() { Id = Guid.NewGuid().ToString(), TextA = result.CalcTextA() }
        ];
        
        return result;
    }

    public List<Fts>? Ftses { get; set; }
    
    public string CalcTextA() => $"{Title} {Text}";
}