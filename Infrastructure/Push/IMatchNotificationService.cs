using Domain.Entities;

namespace Infrastructure.Push
{
    public interface IMatchNotificationService
    {
        Task SendMatchReminderAsync(Match match, CancellationToken ct = default);

        Task SendGoalNotificationAsync(Match match, bool isHomeGoal, CancellationToken ct = default);

        Task SendFinalScoreNotificationAsync(Match match, CancellationToken ct = default);
    }
}
