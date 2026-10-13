using MAUI.DTO.Input;
using System.ComponentModel;

namespace MAUI.Helpers
{
    public class ScoreFlashBehavior : Behavior<Label>
    {
        #region Fields
        private Label? _label;
        private EventsDTO? _event;
        private Color? _originalColor;
        #endregion

        #region Methods
        protected override void OnAttachedTo(Label bindable)
        {
            base.OnAttachedTo(bindable);
            _label = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            Subscribe(bindable.BindingContext);
        }

        protected override void OnDetachingFrom(Label bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            Unsubscribe();
            _label = null;
        }

        private void OnBindingContextChanged(object? sender, EventArgs e)
        {
            Unsubscribe();
            Subscribe(_label?.BindingContext);
        }

        private void Subscribe(object? context)
        {
            _event = context as EventsDTO;
            if (_event is not null)
                _event.PropertyChanged += OnEventPropertyChanged;
        }

        private void Unsubscribe()
        {
            if (_event is not null)
                _event.PropertyChanged -= OnEventPropertyChanged;
            _event = null;
        }

        private async void OnEventPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(EventsDTO.IsScoreFlashing) || _label is null || _event is null)
                return;

            if (!_event.IsScoreFlashing)
                return; 

            _originalColor ??= _label.TextColor;
            var original = _originalColor ?? Colors.Black;
            var highlight = Color.FromArgb("#FFC107");

            
            await AnimateColorAsync(_label, original, highlight, 150);
            await Task.Delay(650);
            await AnimateColorAsync(_label, highlight, original, 150);
        }

        
        private static Task AnimateColorAsync(Label label, Color from, Color to, uint lengthMs)
        {
            var tcs = new TaskCompletionSource();

            label.Animate(
                name: "ScoreFlash",
                callback: progress =>
                {
                    label.TextColor = new Color(
                        (float)(from.Red + (to.Red - from.Red) * progress),
                        (float)(from.Green + (to.Green - from.Green) * progress),
                        (float)(from.Blue + (to.Blue - from.Blue) * progress),
                        (float)(from.Alpha + (to.Alpha - from.Alpha) * progress));
                },
                start: 0,
                end: 1,
                rate: 16,
                length: lengthMs,
                easing: Easing.Linear,
                finished: (v, cancelled) => tcs.TrySetResult());

            return tcs.Task;
        }
        #endregion
    }
}
