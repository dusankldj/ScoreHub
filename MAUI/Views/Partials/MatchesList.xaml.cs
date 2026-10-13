using MAUI.DTO.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace MAUI.Views.Partials;

public partial class MatchesList : ContentView
{
    private const int DaysBack = 15;
    private const int DaysForward = 15;

    public static readonly BindableProperty MatchesProperty =
        BindableProperty.Create(nameof(Matches), typeof(ObservableCollection<MatchesDTO>), typeof(MatchesList),
            defaultValueCreator: _ => new ObservableCollection<MatchesDTO>(),
            propertyChanged: OnMatchesChanged);

    public ObservableCollection<MatchesDTO> Matches
    {
        get => (ObservableCollection<MatchesDTO>)GetValue(MatchesProperty);
        set => SetValue(MatchesProperty, value);
    }


    public static readonly BindableProperty IsBusyProperty =
        BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(MatchesList), false,
            propertyChanged: (b, o, n) => (b as MatchesList)?.UpdateShowNoMatches());

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }


    public static readonly BindableProperty IsPickerEnabledProperty =
        BindableProperty.Create(nameof(IsPickerEnabled), typeof(bool), typeof(MatchesList), true);

    public bool IsPickerEnabled
    {
        get => (bool)GetValue(IsPickerEnabledProperty);
        set => SetValue(IsPickerEnabledProperty, value);
    }


    private static readonly BindablePropertyKey ShowNoMatchesPropertyKey =
        BindableProperty.CreateReadOnly(nameof(ShowNoMatches), typeof(bool), typeof(MatchesList), false);

    public static readonly BindableProperty ShowNoMatchesProperty = ShowNoMatchesPropertyKey.BindableProperty;

    public bool ShowNoMatches => (bool)GetValue(ShowNoMatchesProperty);

    private static void OnMatchesChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not MatchesList list)
            return;

        if (oldValue is ObservableCollection<MatchesDTO> oldCollection)
            oldCollection.CollectionChanged -= list.OnMatchesCollectionChanged;

        if (newValue is ObservableCollection<MatchesDTO> newCollection)
            newCollection.CollectionChanged += list.OnMatchesCollectionChanged;

        list.UpdateShowNoMatches();
    }

    private void OnMatchesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => UpdateShowNoMatches();

    private void UpdateShowNoMatches()
        => SetValue(ShowNoMatchesPropertyKey, Matches.Count == 0 && !IsBusy);

    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(MatchesList), DateTime.Today,
            defaultBindingMode: BindingMode.TwoWay);

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public ObservableCollection<DateFilterOption> DateOptions { get; } = new();


    public static readonly BindableProperty SelectedStatusProperty =
        BindableProperty.Create(nameof(SelectedStatus), typeof(string), typeof(MatchesList), null,
            defaultBindingMode: BindingMode.TwoWay);

    public string? SelectedStatus
    {
        get => (string?)GetValue(SelectedStatusProperty);
        set => SetValue(SelectedStatusProperty, value);
    }

    public ObservableCollection<StatusFilterOption> StatusOptions { get; } = new();


    public static readonly BindableProperty EventSelectedCommandProperty =
        BindableProperty.Create(nameof(EventSelectedCommand), typeof(ICommand), typeof(MatchesList));

    public ICommand EventSelectedCommand
    {
        get => (ICommand)GetValue(EventSelectedCommandProperty);
        set => SetValue(EventSelectedCommandProperty, value);
    }


    public static readonly BindableProperty LeagueSelectedCommandProperty =
        BindableProperty.Create(nameof(LeagueSelectedCommand), typeof(ICommand), typeof(MatchesList));

    public ICommand LeagueSelectedCommand
    {
        get => (ICommand)GetValue(LeagueSelectedCommandProperty);
        set => SetValue(LeagueSelectedCommandProperty, value);
    }

    public MatchesList()
	{
		InitializeComponent();
        BuildDateOptions();
        BuildStatusOptions();
	}

    private void BuildDateOptions()
    {
        var today = DateTime.Today;
        int todayIndex = 0;

        for (int offset = -DaysBack; offset <= DaysForward; offset++)
        {
            var date = today.AddDays(offset);
            DateOptions.Add(new DateFilterOption
            {
                Date = date,
                DisplayText = offset switch
                {
                    0 => "Today",
                    -1 => "Yesterday",
                    1 => "Tomorrow",
                    _ => date.ToString("dd/MM/yyyy")
                }
            });

            if (offset == 0)
                todayIndex = DateOptions.Count - 1;
        }

        DatePicker.SelectedIndex = todayIndex;
    }

    private void BuildStatusOptions()
    {
        StatusOptions.Add(new StatusFilterOption { Value = null, DisplayText = "All" });
        StatusOptions.Add(new StatusFilterOption { Value = "Live", DisplayText = "Live" });
        StatusOptions.Add(new StatusFilterOption { Value = "Upcoming", DisplayText = "Upcoming" });
        StatusOptions.Add(new StatusFilterOption { Value = "Finished", DisplayText = "Finished" });

        StatusPicker.SelectedIndex = 0; 
    }

    private void OnDatePickerSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (DatePicker.SelectedItem is DateFilterOption option)
            SelectedDate = option.Date;
    }

    private void OnStatusPickerSelectedIndexChanged(object? sender, EventArgs e)
    {
        if (StatusPicker.SelectedItem is StatusFilterOption option)
            SelectedStatus = option.Value;
    }
}