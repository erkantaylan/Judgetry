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

    public async Task<EntityEntry<ReadingEntry>> AddReadingEntryAsync(User user, string book, uint pageRead, DateTimeOffset readDate)
    {
        var entry = new ReadingEntry
        {
            UserId = user.Id,
            Book = book,
            PageRead = pageRead,
            ReadDate = readDate.Date.ToUniversalTime()
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

    public async Task<int> CalculateUnreadDaysAsync(User user, DateTimeOffset date)
    {
        IQueryable<DateTimeOffset> dateTimeOffsets = Context.Entries
                                                            .Where(dto => dto.UserId == user.Id)
                                                            .Where(
                                                                 dto => dto.ReadDate.Date >= user.PenaltyResetDate.Date
                                                                     && dto.ReadDate.Date <= date.Date)
                                                            .Select(dto => dto.ReadDate)
                                                            .Distinct();

        List<DateTimeOffset> offsets = await dateTimeOffsets.ToListAsync();

        int readDayCount = offsets.Count;

        int totalDays = (date.Date - user.PenaltyResetDate.Date).Days + 1;
        int unreadDays = totalDays - readDayCount;

        return unreadDays;
    }
}
