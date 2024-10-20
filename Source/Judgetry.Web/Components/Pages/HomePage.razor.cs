using System.Globalization;
using System.Security.Claims;
using Humanizer;
using Judgetry.Web.Database.Dtos;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Judgetry.Web.Components.Pages;

public partial class HomePage
{
    private string book = string.Empty;
    private uint pageRead = 1;
    private readonly List<Penalty> penalties = [];
    private DateTime readDate = DateTime.Today;
    private List<ReadingEntry> readings = [];

    protected override async Task OnInitializedAsync()
    {
        if (penalties.Count == 0)
        {
            List<User> users = await UnitOfWork.Context.Users.ToListAsync();

            foreach (User user in users)
            {
                decimal price = await UnitOfWork.CalculateUnreadDaysAsync(user, DateTimeOffset.UtcNow) * 1;
                var penalty = new Penalty(user.DisplayName, user.PenaltyResetDate.Humanize(culture: new CultureInfo("TR-tr")), price);
                penalties.Add(penalty);
            }
        }

        if (readings.Count == 0)
        {
            readings = await UnitOfWork.Context.Entries.Include(dto => dto.User).ToListAsync();
        }
    }

    private async Task OnAdd()
    {
        AuthenticationState state = await AuthStateProvider.GetAuthenticationStateAsync();
        ClaimsPrincipal claimsPrincipal = state.User;
        string email = claimsPrincipal.Identities.First(o => o.NameClaimType.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")).Name!;
        User user = UnitOfWork.Context.Users.First(dto => dto.Email == email);
        EntityEntry<ReadingEntry> entry = await UnitOfWork.AddReadingEntryAsync(user, book, pageRead, readDate);

        book = string.Empty;
        pageRead = 1;
        readDate = DateTime.Today;
        //UnitOfWork.AddReadingEntryAsync()
    }

    private class Penalty
    {
        public Penalty(string displayName, string resetDate, decimal price)
        {
            DisplayName = displayName;
            ResetDate = resetDate;
            Price = price;
        }

        public string DisplayName { get; }
        public string ResetDate { get; }
        public decimal Price { get; }
    }
}
