using MAUI.DTO.Input;

namespace MAUI.Views.Drawables
{
    // Donut grafikon ishoda prognoza (tacan rezultat / pobednik / promasaj) sa ukupnim brojem bodova u sredini.
    // Crta se direktno preko Microsoft.Maui.Graphics - bez dodatne biblioteke za grafikone.
    public class PredictionDonutDrawable : IDrawable
    {
        private readonly int _exact;
        private readonly int _close;
        private readonly int _winner;
        private readonly int _missed;
        private readonly string _centerTop;
        private readonly string _centerBottom;

        public PredictionDonutDrawable(int exact, int close, int winner, int missed, int points)
        {
            _exact = exact;
            _close = close;
            _winner = winner;
            _missed = missed;
            _centerTop = points.ToString();
            _centerBottom = points == 1 ? "point" : "points";
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var size = Math.Min(dirtyRect.Width, dirtyRect.Height);
            var thickness = size * 0.16f;
            var radius = (size - thickness) / 2f;
            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;

            canvas.StrokeSize = thickness;
            canvas.StrokeLineCap = LineCap.Butt;

            var total = _exact + _close + _winner + _missed;
            if (total == 0)
            {
                canvas.StrokeColor = Color.FromArgb("#E0E0E0");
                canvas.DrawCircle(cx, cy, radius);
            }
            else
            {
                // Luk po luk, od 12h u smeru kazaljke na satu. DrawArc: 0 stepeni = 3h, pozitivno = suprotno od
                // kazaljke, pa se krece od 90 i ide ka manjim uglovima (clockwise = true).
                var start = 90f;
                foreach (var (count, color) in new[] { (_exact, PredictionColors.Exact), (_close, PredictionColors.Close), (_winner, PredictionColors.Winner), (_missed, PredictionColors.Missed) })
                {
                    if (count == 0)
                        continue;

                    var sweep = 360f * count / total;
                    canvas.StrokeColor = color;

                    if (count == total)
                        canvas.DrawCircle(cx, cy, radius);
                    else
                        canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, start, start - sweep, true, false);

                    start -= sweep;
                }
            }

            canvas.FontColor = Color.FromArgb("#1A1A2E");
            canvas.FontSize = size * 0.22f;
            canvas.DrawString(_centerTop, cx - size / 2, cy - size * 0.17f, size, size * 0.26f,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            canvas.FontColor = PredictionColors.Neutral;
            canvas.FontSize = size * 0.10f;
            canvas.DrawString(_centerBottom, cx - size / 2, cy + size * 0.07f, size, size * 0.14f,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }
    }
}
