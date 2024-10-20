using Judgetry.Web.Database.Dtos;

namespace Judgetry.Web.Database;

public class JudgetrySeeds : IDbSeeder<JudgetryDbContext>
{
    public async Task SeedAsync(JudgetryDbContext context)
    {
        return;
        
        var guid1 = Guid.NewGuid();

        DateTimeOffset.UtcNow.Deconstruct(out DateOnly date, out _, out _);

        var dateTime = date.ToDateTime(new TimeOnly());
        var entries = new List<ReadingEntry>
        {
            new()
            {
                Id = 1,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-9).ToUniversalTime(),
                UserId = guid1
            },
            new()
            {
                Id = 2,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-8).ToUniversalTime(),
                UserId = guid1
            },
            new()
            {
                Id = 3,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-7).ToUniversalTime(),
                UserId = guid1
            },
            new()
            {
                Id = 4,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-6).ToUniversalTime(),
                UserId = guid1
            },
            new()
            {
                Id = 5,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-6).ToUniversalTime(),
                UserId = guid1
            },
            new()
            {
                Id = 6,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-6).ToUniversalTime(),
                UserId = guid1
            },
            new()
            {
                Id = 7,
                Book = "Quran",
                PageRead = 8,
                ReadDate = dateTime.AddDays(-6).ToUniversalTime(),
                UserId = guid1
            }
        };

        var user1 = new User
        {
            DisplayName = "Ahmed",
            Id = guid1,
            PenaltyResetDate = dateTime.AddDays(-10).ToUniversalTime(),
            Entries = new List<ReadingEntry>(),
            Email = "ahmed@gmail.com",
            UserName = "ahmed",
            EmailConfirmed = true
        };

        context.Users.Add(user1);
        context.Entries.AddRange(entries);
        await context.SaveChangesAsync();
    }
}
