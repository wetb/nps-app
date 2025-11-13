using NPSApplication.Domain.Common;
using NPSApplication.Domain.Enums;

namespace NPSApplication.Domain.Entities;

public class Vote : BaseEntity
{
    public int UserId { get; set; }
    public int Score { get; set; }  // 0-10
    public VoteCategory Category { get; set; }
    public DateTime VotedAt { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    
    public void CalculateCategory()
    {
        Category = Score switch
        {
            >= 9 => VoteCategory.Promoter,
            >= 7 => VoteCategory.Neutral,
            _ => VoteCategory.Detractor
        };
    }
}
