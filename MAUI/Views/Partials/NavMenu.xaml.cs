using MAUI.Helpers;
using MAUI.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MAUI.Views.Partials;

public partial class NavMenu : ContentView
{
    private bool _isSportsMenuOpen = false;

    public static readonly BindableProperty IsLoggedInProperty =
        BindableProperty.Create(nameof(IsLoggedIn), typeof(bool), typeof(NavMenu), false,
            propertyChanged: OnIsLoggedInChanged);

    public static readonly BindableProperty SportsProperty =
     BindableProperty.Create(nameof(Sports), typeof(ObservableCollection<SportMenuItem>), typeof(NavMenu),
         defaultValueCreator: _ => new ObservableCollection<SportMenuItem>());


    public static readonly BindableProperty OverflowSportsProperty =
    BindableProperty.Create(nameof(OverflowSports), typeof(ObservableCollection<SportMenuItem>), typeof(NavMenu),
        defaultValueCreator: _ => new ObservableCollection<SportMenuItem>());

    public static readonly BindableProperty LogoutCommandProperty =
    BindableProperty.Create(nameof(LogoutCommand), typeof(ICommand), typeof(NavMenu));

    public static readonly BindableProperty SportSelectedCommandProperty =
    BindableProperty.Create(nameof(SportSelectedCommand), typeof(ICommand), typeof(NavMenu));

    public static readonly BindableProperty SearchTextProperty =
    BindableProperty.Create(nameof(SearchText), typeof(string), typeof(NavMenu), string.Empty);

    public static readonly BindableProperty SearchResultsProperty =
    BindableProperty.Create(nameof(SearchResults), typeof(ObservableCollection<SearchResultItem>), typeof(NavMenu),
        defaultValueCreator: _ => new ObservableCollection<SearchResultItem>());

    public static readonly BindableProperty IsSearchResultsVisibleProperty =
    BindableProperty.Create(nameof(IsSearchResultsVisible), typeof(bool), typeof(NavMenu), false,
        propertyChanged: OnIsSearchResultsVisibleChanged);

    public static readonly BindableProperty HasNoSearchResultsProperty =
    BindableProperty.Create(nameof(HasNoSearchResults), typeof(bool), typeof(NavMenu), false);

    public bool HasNoSearchResults
    {
        get => (bool)GetValue(HasNoSearchResultsProperty);
        set => SetValue(HasNoSearchResultsProperty, value);
    }

    public static readonly BindableProperty SearchResultSelectedCommandProperty =
    BindableProperty.Create(nameof(SearchResultSelectedCommand), typeof(ICommand), typeof(NavMenu));

    // Dvosmerno - NavMenu javlja ViewModel-u da li search polje ima fokus (fokus na prazno polje
    // otvara "Recent searches").
    public static readonly BindableProperty IsSearchFocusedProperty =
    BindableProperty.Create(nameof(IsSearchFocused), typeof(bool), typeof(NavMenu), false, defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty IsShowingRecentsProperty =
    BindableProperty.Create(nameof(IsShowingRecents), typeof(bool), typeof(NavMenu), false);

    public static readonly BindableProperty RemoveRecentCommandProperty =
    BindableProperty.Create(nameof(RemoveRecentCommand), typeof(ICommand), typeof(NavMenu));

    public static readonly BindableProperty ClearRecentsCommandProperty =
    BindableProperty.Create(nameof(ClearRecentsCommand), typeof(ICommand), typeof(NavMenu));

    public bool IsSearchFocused
    {
        get => (bool)GetValue(IsSearchFocusedProperty);
        set => SetValue(IsSearchFocusedProperty, value);
    }

    public bool IsShowingRecents
    {
        get => (bool)GetValue(IsShowingRecentsProperty);
        set => SetValue(IsShowingRecentsProperty, value);
    }

    public ICommand RemoveRecentCommand
    {
        get => (ICommand)GetValue(RemoveRecentCommandProperty);
        set => SetValue(RemoveRecentCommandProperty, value);
    }

    public ICommand ClearRecentsCommand
    {
        get => (ICommand)GetValue(ClearRecentsCommandProperty);
        set => SetValue(ClearRecentsCommandProperty, value);
    }


    // Visina SAME trake (bez padajucih menija) - HomePage po njoj postavlja gornju marginu sadrzaja, jer se NavMenu
    // sa otvorenim menijem uvecava (da meni upadne u njegove granice i primi dodir), a sadrzaj ispod ne sme da se pomera.
    private static readonly BindablePropertyKey BarHeightPropertyKey =
        BindableProperty.CreateReadOnly(nameof(BarHeight), typeof(double), typeof(NavMenu), 0d);

    public static readonly BindableProperty BarHeightProperty = BarHeightPropertyKey.BindableProperty;

    public double BarHeight => (double)GetValue(BarHeightProperty);

    private static readonly BindablePropertyKey IsAnyMenuOpenPropertyKey =
        BindableProperty.CreateReadOnly(nameof(IsAnyMenuOpen), typeof(bool), typeof(NavMenu), false);

    public static readonly BindableProperty IsAnyMenuOpenProperty = IsAnyMenuOpenPropertyKey.BindableProperty;

    public bool IsAnyMenuOpen => (bool)GetValue(IsAnyMenuOpenProperty);

    public ICommand LogoutCommand
    {
        get => (ICommand)GetValue(LogoutCommandProperty);
        set => SetValue(LogoutCommandProperty, value);
    }

    public ICommand SportSelectedCommand
    {
        get => (ICommand)GetValue(SportSelectedCommandProperty);
        set => SetValue(SportSelectedCommandProperty, value);
    }

    public string SearchText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public ObservableCollection<SearchResultItem> SearchResults
    {
        get => (ObservableCollection<SearchResultItem>)GetValue(SearchResultsProperty);
        set => SetValue(SearchResultsProperty, value);
    }

    public bool IsSearchResultsVisible
    {
        get => (bool)GetValue(IsSearchResultsVisibleProperty);
        set => SetValue(IsSearchResultsVisibleProperty, value);
    }

    public ICommand SearchResultSelectedCommand
    {
        get => (ICommand)GetValue(SearchResultSelectedCommandProperty);
        set => SetValue(SearchResultSelectedCommandProperty, value);
    }

    public ObservableCollection<SportMenuItem> Sports
    {
        get => (ObservableCollection<SportMenuItem>)GetValue(SportsProperty);
        set => SetValue(SportsProperty, value);
    }

    public ObservableCollection<SportMenuItem> OverflowSports
    {
        get => (ObservableCollection<SportMenuItem>)GetValue(OverflowSportsProperty);
        set => SetValue(OverflowSportsProperty, value);
    }

    public bool IsLoggedIn
    {
        get => (bool)GetValue(IsLoggedInProperty);
        set => SetValue(IsLoggedInProperty, value);
    }
    public NavMenu()
	{
		InitializeComponent();
        BarGrid.SizeChanged += (_, _) => SetValue(BarHeightPropertyKey, BarGrid.Height);
        UpdateAccountIconVisibility();
    }

    private static void OnIsLoggedInChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NavMenu menu)
        {
            menu.UpdateAccountIconVisibility();
        }
    }

    private static void OnIsSearchResultsVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NavMenu menu)
        {
            menu.UpdateIsAnyMenuOpen();
            menu.UpdateSearchIcon();
        }
    }

    // Lupa -> X dok je padajuca lista pretrage (rezultati ili "Recent searches") otvorena.
    private void UpdateSearchIcon()
    {
        SearchIconLabel.Text = IsSearchResultsVisible ? Icons.XIcon : Icons.SearchIcon;
        SearchIconLabel.TextColor = IsSearchResultsVisible ? Color.FromArgb("#1A1A2E") : Color.FromArgb("#8A8A8A");
    }

    // X zatvara listu: skida fokus sa polja i brise ukucani tekst - ViewModel (preko dvosmernih SearchText /
    // IsSearchFocused) tada sam sakriva rezultate i "Recent searches". Kad je lista zatvorena, tap na lupu fokusira polje.
    private void OnSearchIconTapped(object? sender, TappedEventArgs e)
    {
        if (IsSearchResultsVisible)
        {
            IsSearchFocused = false;
            SearchText = string.Empty;
            SearchEntry.Unfocus();
        }
        else
        {
            SearchEntry.Focus();
        }
    }

    private void UpdateAccountIconVisibility()
    {
        
    }

    
    private async void OnLogoTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//home");

        if (Shell.Current.CurrentPage is Views.HomePage { BindingContext: ViewModel.HomeViewModel vm })
        {
            vm.SelectedSportId = null;
            vm.SelectedStatus = null;
            vm.SelectedDate = DateTime.Today;
        }
    }

    private async void OnSportsMenuTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Label label)
            return;

        await Task.WhenAll(
        label.FadeTo(0, 120),
        label.RotateTo(_isSportsMenuOpen ? 0 : 90, 120));

        _isSportsMenuOpen = !_isSportsMenuOpen;
        label.Text = _isSportsMenuOpen ? Icons.XIcon : Icons.HamburgerIcon;
        label.Rotation = _isSportsMenuOpen ? -90 : 0;

        await Task.WhenAll(
        label.FadeTo(1, 120),
        label.RotateTo(0, 120));

        OverflowMenuBorder.IsVisible = _isSportsMenuOpen;
        UpdateIsAnyMenuOpen();
    }

    private async void OnAccountMenuTapped(object? sender, TappedEventArgs e)
    {
        if (!IsLoggedIn)
        {
            await Shell.Current.GoToAsync("login");
            return;
        }

        AccountMenuBorder.IsVisible = !AccountMenuBorder.IsVisible;
        UpdateIsAnyMenuOpen();
    }

    private async void OnSettingsTapped(object? sender, TappedEventArgs e)
    {
        AccountMenuBorder.IsVisible = false;
        UpdateIsAnyMenuOpen();
        await Shell.Current.GoToAsync("settings");
    }

    private async void OnProfileInfoTapped(object? sender, TappedEventArgs e)
    {
        AccountMenuBorder.IsVisible = false;
        UpdateIsAnyMenuOpen();
        await Shell.Current.GoToAsync("profile");
    }

    private void UpdateIsAnyMenuOpen()
        => SetValue(IsAnyMenuOpenPropertyKey, OverflowMenuBorder.IsVisible || AccountMenuBorder.IsVisible || IsSearchResultsVisible);

    private void OnLogoutTapped(object? sender, TappedEventArgs e)
    {
        AccountMenuBorder.IsVisible = false;
        UpdateIsAnyMenuOpen();

        if (LogoutCommand is null)
        {
            System.Diagnostics.Debug.WriteLine("OnLogoutTapped: LogoutCommand is not bound (null) - nothing to execute.");
            return;
        }

        System.Diagnostics.Debug.WriteLine($"OnLogoutTapped: executing LogoutCommand, CanExecute={LogoutCommand.CanExecute(null)}");
        LogoutCommand.Execute(null);
    }

    private void OnSportTapped(object? sender, TappedEventArgs e)
    {
        if (e.Parameter is not SportMenuItem sport)
            return;

        HandleSportSelected(sport);
    }

    private void OnOverflowSportSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not CollectionView collectionView)
            return;

        if (e.CurrentSelection.FirstOrDefault() is SportMenuItem sport)
            HandleSportSelected(sport);


        collectionView.SelectedItem = null;
    }

    // Izbor sporta zatvara padajuci meni sportova i vraca X nazad u hamburger ikonicu.
    private void CloseSportsMenu()
    {
        if (!_isSportsMenuOpen)
            return;

        _isSportsMenuOpen = false;
        OverflowMenuBorder.IsVisible = false;
        SportsMenuLabel.Text = Icons.HamburgerIcon;
        SportsMenuLabel.Rotation = 0;
        UpdateIsAnyMenuOpen();
    }

    private void HandleSportSelected(SportMenuItem sport)
    {
        CloseSportsMenu();

        System.Diagnostics.Debug.WriteLine($"Tapped sport: Id={sport.Id}, Name={sport.Name}");

        if (SportSelectedCommand is null)
            return;

        if (SportSelectedCommand.CanExecute(sport.Id))
            SportSelectedCommand.Execute(sport.Id);
    }

    private void OnSearchEntryFocused(object? sender, FocusEventArgs e)
        => IsSearchFocused = true;

    private void OnSearchEntryUnfocused(object? sender, FocusEventArgs e)
        => IsSearchFocused = false;

    // Tap na ceo red (umesto CollectionView selekcije) - X ikonica unutar reda ima SOPSTVENI tap
    // (OnRemoveRecentTapped) i on ima prednost, isti obrazac kao slika/red igraca na EventDetailsPage.
    private void OnSearchResultTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Grid { BindingContext: SearchResultItem result })
            HandleSearchResultSelected(result);
    }

    private void OnRemoveRecentTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Label { BindingContext: SearchResultItem item })
            return;

        if (RemoveRecentCommand?.CanExecute(item) == true)
            RemoveRecentCommand.Execute(item);
    }

    private void OnClearRecentsTapped(object? sender, TappedEventArgs e)
    {
        if (ClearRecentsCommand?.CanExecute(null) == true)
            ClearRecentsCommand.Execute(null);
    }

    private void HandleSearchResultSelected(SearchResultItem result)
    {
        System.Diagnostics.Debug.WriteLine($"Tapped search result: Kind={result.Kind}, Id={result.Id}, Name={result.DisplayName}");

        if (SearchResultSelectedCommand is null)
            return;

        if (SearchResultSelectedCommand.CanExecute(result))
            SearchResultSelectedCommand.Execute(result);
    }
}