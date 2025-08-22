using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace app.Pkg.Model;

/// Second version of full-text-search. Features: <br/>
/// - Index per search language (EN & FR)
/// - Typically 1 combined text translation (with single weight) but can add more later
/// -- Soft-deleted records can have 0
/// - No need for 'LEAN' approach because FTS fields are in a related entity
public class HotelReview2 : HotelReviewBase
{
    /// Create from a V1
    public static HotelReview2 CreateFrom(HotelReviewBase hrBase) => new()
    {
        Id = hrBase.Id,
        Date = hrBase.Date,
        Rating = hrBase.Rating,
        Language = hrBase.Language,
        Property = hrBase.Property,
        Text = hrBase.Text,
        Title = hrBase.Title,
        Ftss = [
            new() { Id = Guid.NewGuid().ToString(), TextA = $"{hrBase.Title} {hrBase.Text}" }
        ]
    };

    /// The full text search translations. Only 1 initially.
    public List<HotelReview2Fts>? Ftss { get; set; }
}