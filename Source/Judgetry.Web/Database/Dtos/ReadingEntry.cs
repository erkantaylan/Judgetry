using System.ComponentModel.DataAnnotations;
using Judgetry.Core.Database.Models;

namespace Judgetry.Web.Database.Dtos;

public class ReadingEntry : EntityBase<int>
{
    public required Guid UserId { get; set; }
    public required DateTimeOffset ReadDate { get; set; }
    [StringLength(256)]
    public required string Book { get; set; }
    public required uint PageRead { get; set; }
    public User? User { get; set; }
}
