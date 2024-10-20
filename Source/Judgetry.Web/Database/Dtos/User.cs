using System.ComponentModel.DataAnnotations;
using Judgetry.Core.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace Judgetry.Web.Database.Dtos;

public class User : IdentityUser<Guid>, IEntity<Guid>
{
    [StringLength(128)]
    public required string DisplayName { get; set; }
    
    public DateTimeOffset PenaltyResetDate { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public ICollection<ReadingEntry>? Entries { get; set; }
}
