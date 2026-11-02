using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI.DTO.Input;
using MAUI.Helpers;
using MAUI.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MAUI.ViewModel
{
    // Tudji profil (UserPage). Svima je vidljivo samo korisnicko ime i "Member since"; omiljeni timovi i prognoze
    // SAMO prijateljima (server to vazi i sam - 403 za ostale). Dugme zavisi od odnosa (Relationship):
    // None -> "Add friend", RequestSent -> "Request sent" (klik ponistava), RequestReceived -> Accept / Decline,
    // Friends -> X ikonica za uklanjanje prijatelja. Novi odnos uvek vraca server - klijent ga samo prikazuje.
    [QueryProperty(nameof(UserId), "userId")]
    public partial class UserViewModel : ObservableObject
    {
        #region Fields
        private const int ListLimit = 5;

        private readonly IUserApiService _userApiService;
        private readonly IFriendApiService _friendApiService;
        private readonly IPredictionApiService _predictionApiService;

        private List<FavoriteTeamDTO> _allFavoriteTeams = new();
        #endregion

        #region Constructor
        public UserViewModel(IUserApiService userApiService, IFriendApiService friendApiService, IPredictionApiService predictionApiService)
        {
            _userApiService = userApiService;
            _friendApiService = friendApiService;
            _predictionApiService = predictionApiService;
        }
        #endregion

        #region Properties
        [ObservableProperty]
        private long userId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LockedText))]
        private string username = string.Empty;

        [ObservableProperty]
        private string memberSince = string.Empty;

        // Ime i prezime ispod korisnickog imena - samo dok su prijatelji (server ga ne salje ostalima).
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasFullName))]
        private string fullName = string.Empty;

        public bool HasFullName => !string.IsNullOrWhiteSpace(FullName);

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFriend))]
        [NotifyPropertyChangedFor(nameof(IsSelf))]
        [NotifyPropertyChangedFor(nameof(IsLocked))]
        [NotifyPropertyChangedFor(nameof(ShowPrimaryButton))]
        [NotifyPropertyChangedFor(nameof(IsRequestReceived))]
        [NotifyPropertyChangedFor(nameof(ShowFriendContent))]
        [NotifyPropertyChangedFor(nameof(PrimaryButtonText))]
        [NotifyPropertyChangedFor(nameof(PrimaryButtonBackground))]
        [NotifyPropertyChangedFor(nameof(PrimaryButtonTextColor))]
        private string relationship = string.Empty;

        public bool IsFriend => Relationship == RelationshipState.Friends;

        public bool IsSelf => Relationship == RelationshipState.Self;

        // "Add friend" / "Request sent" - jedno dugme koje klikom menja stanje.
        public bool ShowPrimaryButton => Relationship is RelationshipState.None or RelationshipState.RequestSent;

        public bool IsRequestReceived => Relationship == RelationshipState.RequestReceived;

        // Zakljucano: nije prijatelj (i nije sam korisnik) - umesto sadrzaja ide katanac + objasnjenje.
        public bool IsLocked => Relationship.Length > 0 && !IsFriend && !IsSelf;

        public bool ShowFriendContent => IsFriend || IsSelf;

        public string LockedText => $"You must be friends to see {Username}'s favorite teams and predictions";

        public string PrimaryButtonText => Relationship == RelationshipState.RequestSent ? "Request sent" : "Add friend";

        // "Add friend" je teget; "Request sent" je providno sa teget ivicom da se vidi da je stanje vec poslato.
        public Color PrimaryButtonBackground => Relationship == RelationshipState.RequestSent ? Colors.White : Color.FromArgb("#1A1A2E");

        public Color PrimaryButtonTextColor => Relationship == RelationshipState.RequestSent ? Color.FromArgb("#1A1A2E") : Colors.White;

        // Omiljeni timovi prijatelja - isto kao na Profile: prvih 5, pa "Show more (N)" / "Show less".
        public ObservableCollection<FavoriteTeamDTO> FavoriteTeams { get; } = new();

        [ObservableProperty]
        private bool isFavoriteTeamsExpanded;

        public bool HasFavoriteTeams => _allFavoriteTeams.Count > 0;

        public bool HasMoreFavoriteTeams => _allFavoriteTeams.Count > ListLimit;

        public string ToggleFavoriteTeamsText => IsFavoriteTeamsExpanded ? "Show less" : $"Show more ({_allFavoriteTeams.Count})";

        public PredictionsListModel Predictions { get; } = new() { Title = "Predictions" };
        #endregion

        #region Methods
        partial void OnUserIdChanged(long value)
        {
            if (value > 0)
                _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var user = await _userApiService.GetUserAsync(UserId);
                Username = user.Username;
                MemberSince = $"Member since: {DateTime.SpecifyKind(user.MemberSince, DateTimeKind.Utc).ToLocalTime():dd/MM/yyyy}";
                Relationship = user.Relationship;
                FullName = user.FullName ?? string.Empty;

                await LoadFriendContentAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load this user.";
                System.Diagnostics.Debug.WriteLine($"UserViewModel.LoadAsync failed: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Posle promene odnosa (prihvaceno / uklonjen prijatelj) ime i prezime se pojavljuju ili nestaju - osvezava se zaglavlje.
        private async Task RefreshUserAsync()
        {
            try
            {
                var user = await _userApiService.GetUserAsync(UserId);
                Relationship = user.Relationship;
                FullName = user.FullName ?? string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshUserAsync failed: {ex}");
            }
        }

        // Omiljeni timovi + prognoze - SAMO ako je prijatelj (ili sam korisnik); inace se sve prazni.
        private async Task LoadFriendContentAsync()
        {
            if (!ShowFriendContent)
            {
                _allFavoriteTeams = new List<FavoriteTeamDTO>();
                ApplyFavoriteTeamsView();
                Predictions.ShowCommonFilter = false;
                Predictions.SelectedCommonFilter = "All";
                Predictions.SetItems(Array.Empty<MyPredictionDTO>());
                return;
            }

            try
            {
                _allFavoriteTeams = await _userApiService.GetUserFavoriteTeamsAsync(UserId);
                ApplyFavoriteTeamsView();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Loading user favorite teams failed: {ex}");
            }

            try
            {
                var theirs = await _predictionApiService.GetUserPredictionsAsync(UserId);

                // Za filter "Common" treba i MOJA lista (pregled tudjeg profila, ne mog sopstvenog).
                IEnumerable<MyPredictionDTO>? own = null;
                if (IsFriend)
                {
                    try
                    {
                        own = await _predictionApiService.GetMyPredictionsAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Loading own predictions failed: {ex}");
                    }
                }

                Predictions.ShowCommonFilter = own is not null;
                Predictions.OtherName = Username;
                Predictions.SetItems(theirs, own);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Loading user predictions failed: {ex}");
            }
        }

        private void ApplyFavoriteTeamsView()
        {
            FavoriteTeams.Clear();
            foreach (var team in IsFavoriteTeamsExpanded ? _allFavoriteTeams : _allFavoriteTeams.Take(ListLimit))
                FavoriteTeams.Add(team);

            OnPropertyChanged(nameof(HasFavoriteTeams));
            OnPropertyChanged(nameof(HasMoreFavoriteTeams));
            OnPropertyChanged(nameof(ToggleFavoriteTeamsText));
        }

        partial void OnIsFavoriteTeamsExpandedChanged(bool value) => ApplyFavoriteTeamsView();

        [RelayCommand]
        private void ToggleFavoriteTeams() => IsFavoriteTeamsExpanded = !IsFavoriteTeamsExpanded;

        [RelayCommand]
        private async Task FavoriteTeamSelected(long teamId)
        {
            await Shell.Current.GoToAsync($"teampage?teamId={teamId}");
        }

        // "Add friend" <-> "Request sent". Stanje se menja ODMAH (optimisticno), pa se uskladjuje sa odgovorom servera;
        // ako poziv ne uspe, vraca se staro.
        [RelayCommand]
        private async Task PrimaryActionAsync()
        {
            var previous = Relationship;
            var sending = previous == RelationshipState.None;

            Relationship = sending ? RelationshipState.RequestSent : RelationshipState.None;
            ErrorMessage = string.Empty;

            try
            {
                Relationship = sending
                    ? await _friendApiService.SendRequestAsync(UserId)
                    : await _friendApiService.CancelRequestAsync(UserId);

                // Ako je druga strana vec poslala zahtev, server ga je prihvatio - odmah se vidi sadrzaj.
                await RefreshUserAsync();
                await LoadFriendContentAsync();
            }
            catch (Exception ex)
            {
                Relationship = previous;
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"PrimaryActionAsync failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task AcceptAsync()
        {
            try
            {
                Relationship = await _friendApiService.AcceptRequestAsync(UserId);
                await RefreshUserAsync();
                await LoadFriendContentAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"AcceptAsync failed: {ex}");
            }
        }

        [RelayCommand]
        private async Task DeclineAsync()
        {
            try
            {
                Relationship = await _friendApiService.DeclineRequestAsync(UserId);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                System.Diagnostics.Debug.WriteLine($"DeclineAsync failed: {ex}");
            }
        }

        // X ikonica (vidljiva samo prijatelju) - uz potvrdu; posle uklanjanja sadrzaj se zakljucava.
        [RelayCommand]
        private async Task RemoveFriendAsync()
        {
            if (!await FriendDialogs.ConfirmRemoveAsync(Username))
                return;

            try
            {
                Relationship = await _friendApiService.RemoveFriendAsync(UserId);
                await RefreshUserAsync();
                await LoadFriendContentAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to remove the friend.";
                System.Diagnostics.Debug.WriteLine($"RemoveFriendAsync failed: {ex}");
            }
        }
        #endregion
    }
}
