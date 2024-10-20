using Judgetry.Web.Database.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Judgetry.Web.Database;

public class UnitOfWork
{
    public UnitOfWork(JudgetryDbContext context)
    {
        Context = context;
    }

    public JudgetryDbContext Context { get; }

    public async Task<EntityEntry<ReadingEntry>> AddReadingEntryAsync(User user, string book, uint pageRead, DateTimeOffset date)
    {
        var entry = new ReadingEntry
        {
            UserId = user.Id,
            Book = book,
            PageRead = pageRead,
            ReadDate = date
        };
        EntityEntry<ReadingEntry> entity = await Context.Entries.AddAsync(entry);
        await Context.SaveChangesAsync();

        return entity;
    }

    public async Task<int> ResetPenaltyAsync(User user, DateTimeOffset date)
    {
        user.PenaltyResetDate = date;
        Context.Users.Update(user);

        return await Context.SaveChangesAsync();
    }

    public int CalculateUnreadDays(User user, DateTimeOffset date)
    {
        IQueryable<DateTimeOffset> dateTimeOffsets = Context.Entries
                                                            .Where(dto => dto.UserId == user.Id)
                                                            .Where(
                                                                 dto => dto.ReadDate.Date >= user.PenaltyResetDate.Date
                                                                     && dto.ReadDate.Date <= date.Date.Date)
                                                            .Select(dto => dto.ReadDate)
                                                            .Distinct();

        string queryString = dateTimeOffsets.ToQueryString();

        List<DateTimeOffset> offsets = dateTimeOffsets
           .ToList();

        int readDayCount = offsets.Count();

        int totalDays = (date.Date - user.PenaltyResetDate.Date).Days + 1;
        int unreadDays = totalDays - readDayCount;

        return unreadDays;
    }
}
