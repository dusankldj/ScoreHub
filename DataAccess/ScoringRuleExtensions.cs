using Domain.Predictions;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public static class ScoringRuleExtensions
    {
        // Pravila bodovanja svih sportova (tabela SportScoringRules je mala - jedan red po sportu), citaju se po zahtevu.
        public static async Task<ScoringRuleSet> LoadScoringRulesAsync(this ScoreHubDbContext context, CancellationToken ct = default)
        {
            var rules = await context.SportScoringRules
                .AsNoTracking()
                .ToDictionaryAsync(
                    r => r.SportId,
                    r => new ScoringRule(r.ExactPoints, r.ClosePoints, r.WinnerPoints, r.CloseMarginTolerance, r.MaxScore),
                    ct);

            return new ScoringRuleSet(rules);
        }
    }
}
