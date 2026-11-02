# Graph Report - project  (2026-10-14)

## Corpus Check
- 305 files · ~456,636 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 3844 nodes · 6337 edges · 220 communities (209 shown, 11 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 489 edges (avg confidence: 0.73)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `8a70b7a6`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- EventBroadcastDTO
- Player
- ISportsDbSyncService
- NavMenu
- Infrastructure.csproj
- ScoreHubDbContext
- .AddAppServices
- MAUI.DTO.Input
- IAuthService
- SportsDbClient
- IAuthApiService
- EventLineupDTO
- Match
- ContentPage
- SportsDbMatch
- ContentPage
- SportsDbPlayer
- Extensions.cs
- UserSearch
- PlayerFormerTeam
- Output/EventDetailsDTO.cs
- http
- TeamDTO
- .SavePrediction
- Log
- SportsDbTeam
- .LoadPredictionsAsync
- MainActivity
- User
- SportsDbPlayerHonour
- .Login
- ContentPage
- ContentPage
- TeamMatches
- MatchNotificationService
- LeaderboardDTO
- SportsDbStartupSyncService
- NavMenuViewModelBase
- SyncedEntityConfiguration
- PredictionStatsDTO
- EventTimeline
- SportsDbLeague
- SportsDbPlayerFormerTeam
- EventTimelineDTO
- MAUI
- SearchResultItem
- ContentView
- PredictionStatsDTO
- ContentPage
- MAUI.ViewModel
- .CreateMauiApp
- ResourceDictionary
- ContentPage
- IPushNotificationService
- SyncedEntity
- SportsDbEventLineup
- ISearchHistoryApiService
- EventDetailsDTO
- SportsDbSport
- Squad
- EventLineup
- Team
- AppDelegate
- LeaderboardRowDTO
- LeaderboardEntryDTO
- LoginViewModel
- SportsDbSyncService
- ISportsDbClient
- ScoreHub App Icon Foreground (Trophy Mark)
- Prediction
- App
- Team
- PredictionDTO
- IPredictionApiService
- ContentPage
- .CreateWindow
- FriendDTO
- BaseEntity
- FriendsController
- User
- Infrastructure.Integrations.DTO
- Friendship
- .GetTeamDetails
- UserViewModel
- PredictionsListModel
- RecentSearchDTO
- League
- MainApplication
- graphify Knowledge Graph Tool
- .AddPushNotifications
- App
- AddFriendship
- Domain/Class1.cs
- LoadMauiAsset Method
- Colors.xaml
- PlayerStatistics
- PlayerSearchResultDTO
- ContentView
- MatchScheduleBackgroundService
- IFriendApiService
- UserRole
- PredictionDonutDrawable
- FriendApiService
- TabContentContainer
- JwtOptions
- .BuildTargetModel
- .HandleSportSelected
- FavoriteTeam
- RegisterRequestDTO
- PlayerMilestones
- .Fill
- SportsOptions
- PlayerSearchResultDTO
- .LoadPlayerDetailsAsync
- TabContentContainer
- .AddSportsDbIntegration
- .ShowAsync
- HomePage
- LoginRequestDTO
- RefreshRequestDTO
- AuthResponseDTO
- NavMenuSportsDTO
- DataAccess.Configurations
- LeagueDetailsDTO
- MyPredictionDTO
- SavePredictionRequestDTO
- PredictionsController
- PlayerStatistic
- PlayerDTO
- WeatherForecast
- Venue
- SportMenuItem
- SportsDbEventTv
- EventDetailsDTO
- EventsDTO
- SportsDbPlayerStatistic
- MatchStatisticsTrackerService
- PlayerViewModel
- Standings
- PlayerFormerTeamBackgroundService
- EventDetailsViewModel
- .GetSports
- PlayerHonour
- FavoriteTeamDTO
- HomeViewModel
- PlayerDTO
- TeamMatches
- TeamViewModel
- LeagueSearchResultDTO
- ContentView
- TeamPage
- IPlayerApiService
- TeamDTO
- MatchesList
- ITeamApiService
- EventLineupDTO
- ILeagueApiService
- ScoreFlashBehavior
- ContentPage
- UsersController
- NavMenuControl
- PlayerFormerTeams
- Squad
- PlayerSyncBackgroundService
- DatePicker
- ContentView
- MatchesDTO
- EventsDTO
- PlayerHonours
- PlayerStatistics
- .IsLoggedInAsync
- .GetMatchesAsync
- .GetPlayerDetails
- SportsDbStanding
- PlayerPage
- MAUI.Views.Partials
- EventStatistic
- PlayerFormerTeams
- InvertedBoolConverter
- LeagueStandingDTO
- PlayerHonours
- Domain.Entities
- EventDetailsPage
- LeagueViewModel
- IDeviceTokenApiService
- LeagueStandingDTO
- LeagueDetailsDTO
- SportsDbTeamEquipment
- .GetLeagueDetails
- TappedEventArgs
- DataAccess.Migrations
- ProfileViewModel
- SearchEntry
- EventTimelineBackgroundService
- EventTvBackgroundService
- PlayerMilestones
- MyPredictionDTO
- .SendAsync
- .GetEventDetailsAsync
- BackgroundService
- ContentView
- AddEventTimeline
- PreMatchLineupBackgroundService
- ContentPage
- .GetEventDetails
- SportsDbEventStatistic
- RenameEventTimelineApiId
- SportsDbVenue
- .BuildModel
- EventStatisticsDTO
- EventTimelineDTO
- IMatchNotificationService
- Migration
- AddUserSearch
- AddMatchTvSyncedAt
- Task
- PredictionDTO
- ContentView
- ProfilePage
- .OnLogoTapped

## God Nodes (most connected - your core abstractions)
1. `ProfileViewModel` - 124 edges
2. `ContentPage` - 95 edges
3. `ContentPage` - 94 edges
4. `ScoreHubDbContext` - 93 edges
5. `EventDetailsViewModel` - 80 edges
6. `Domain.Entities` - 77 edges
7. `ContentPage` - 76 edges
8. `MAUI.DTO.Input` - 62 edges
9. `ContentPage` - 56 edges
10. `SportsDbSyncService` - 55 edges

## Surprising Connections (you probably didn't know these)
- `ScoreHubDbContext` --references--> `Channel`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/Channel.cs
- `ScoreHubDbContext` --references--> `DeviceToken`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/DeviceToken.cs
- `ScoreHubDbContext` --references--> `User`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/DeviceToken.cs
- `ScoreHubDbContext` --references--> `EventLineup`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventLineup.cs
- `ScoreHubDbContext` --references--> `Player`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventLineup.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **ScoreHub App Icon & Splash Composition** — maui_resources_appicon_appicon_background, maui_resources_appicon_appiconfg_trophy_icon, maui_resources_splash_splash_trophy_icon [INFERRED 0.85]
- **ScoreHub Logo Size Variant Set** — maui_resources_images_scorehub_160x160px_logo, maui_resources_images_scorehub_160x90px_logo, maui_resources_images_scorehub_160x90px_v2_logo, maui_resources_images_scorehub_64x36px_logo, maui_resources_images_scorehub_64x64px_logo [INFERRED 0.85]

## Communities (220 total, 11 thin omitted)

### Community 0 - "EventBroadcastDTO"
Cohesion: 0.14
Nodes (14): EventBroadcastDTO, ChannelName, Country, FlagImage, HasFlag, HasLogo, HasName, LogoThumbUrl (+6 more)

### Community 1 - "Player"
Cohesion: 0.06
Nodes (32): DateOnly, DateTime, ICollection, Player, BirthDate, BirthLocation, DateDied, Description (+24 more)

### Community 2 - "ISportsDbSyncService"
Cohesion: 0.12
Nodes (17): StatusCode, EventTvSyncMode, All, Repair, Unsynced, CancellationToken, Task, ISportsDbSyncService (+9 more)

### Community 3 - "NavMenu"
Cohesion: 0.09
Nodes (21): BindableObject, BindableProperty, BindablePropertyKey, ICommand, ObservableCollection, NavMenu, ClearRecentsCommand, HasNoSearchResults (+13 more)

### Community 4 - "Infrastructure.csproj"
Cohesion: 0.06
Nodes (34): net10.0, Microsoft.EntityFrameworkCore (10.0.9), Microsoft.NET.Sdk, Domain, net10.0, Microsoft.NET.Sdk, net10.0-android, net10.0 (+26 more)

### Community 5 - "ScoreHubDbContext"
Cohesion: 0.06
Nodes (37): ControllerBase, Jwt, ModelBuilder, Standings, TeamEquipment, ScoreHubDbContext, Channels, DeviceTokens (+29 more)

### Community 6 - ".AddAppServices"
Cohesion: 0.09
Nodes (23): SportsDTO, Id, Name, HttpClient, EventDetailsApiService, IEventDetailsApiService, CancellationToken, List (+15 more)

### Community 7 - "MAUI.DTO.Input"
Cohesion: 0.14
Nodes (4): MAUI.DTO.Input, MAUI.DTO.Output, MAUI.Services, MAUI.Services.Interfaces

### Community 8 - "IAuthService"
Cohesion: 0.17
Nodes (9): DelegatingHandler, HttpResponseMessage, CancellationToken, Task, RefreshTokenHelper, Task, AuthService, Task (+1 more)

### Community 9 - "SportsDbClient"
Cohesion: 0.17
Nodes (9): CancellationToken, DateTime, Dictionary, HttpClient, List, Task, TimeSpan, SportsDbClient (+1 more)

### Community 10 - "IAuthApiService"
Cohesion: 0.08
Nodes (26): AuthResponseDTO, RefreshToken, Token, LoginRequestDTO, Password, Username, RefreshRequestDTO, RefreshToken (+18 more)

### Community 11 - "EventLineupDTO"
Cohesion: 0.17
Nodes (12): EventLineupDTO, FirstName, Formation, HomeGame, ImageUrl, IsSubtitute, LastName, PlayerId (+4 more)

### Community 12 - "Match"
Cohesion: 0.07
Nodes (28): DateTime, ICollection, Match, AwayTeam, AwayTeamId, AwayTeamScore, DetailsSyncedAt, EventDate (+20 more)

### Community 13 - "ContentPage"
Cohesion: 0.03
Nodes (65): Appearences, DateAchieved, Details.Age, Details.BirthDate, Details.BirthLocation, Details.Description, Details.FacebookLink, Details.FormerTeams (+57 more)

### Community 14 - "SportsDbMatch"
Cohesion: 0.09
Nodes (22): SportsDbMatch, DateEvent, IdAwayTeam, IdEvent, IdHomeTeam, IdLeague, IdVenue, IntAwayScore (+14 more)

### Community 15 - "ContentPage"
Cohesion: 0.05
Nodes (37): ColorHex, Description, DescriptionColorHex, Details.HasLogo, Details.LogoUrl, Details.Name, Details.NoLogo, Details.Standings (+29 more)

### Community 16 - "SportsDbPlayer"
Cohesion: 0.10
Nodes (21): SportsDbPlayer, DateBorn, DateDied, IdPlayer, IdTeam, IdTeam2, StrBirthLocation, StrDescriptionEN (+13 more)

### Community 17 - "Extensions.cs"
Cohesion: 0.14
Nodes (12): WebAPI.Services, Infrastructure.Integrations, Infrastructure.Integrations.SportsDB, Infrastructure, Infrastructure.Integrations.Exceptions, WebAPI.JWT, WebAPI.Services.Interfaces, Exception (+4 more)

### Community 18 - "UserSearch"
Cohesion: 0.07
Nodes (32): EntityTypeBuilder, DateTime, UserSearch, EntityId, Kind, SearchedAt, User, UserId (+24 more)

### Community 19 - "PlayerFormerTeam"
Cohesion: 0.13
Nodes (13): EntityTypeBuilder, PlayerFormerTeamConfiguration, PlayerFormerTeam, Appearances, BadgeUrl, Departed, FormerTeam, FormerTeamId (+5 more)

### Community 20 - "Output/EventDetailsDTO.cs"
Cohesion: 0.22
Nodes (8): EventBroadcastDTO, ChannelName, Country, LogoUrl, EventStatisticsDTO, AwayScore, HomeScore, Stat

### Community 21 - "http"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 22 - "TeamDTO"
Cohesion: 0.13
Nodes (15): List, TeamDTO, Country, Equipment, ImageUrl, Matches, Name, ShortName (+7 more)

### Community 23 - ".SavePrediction"
Cohesion: 0.17
Nodes (8): HttpPut, IActionResult, SavePredictionDTO, PredictedAwayScore, PredictedHomeScore, DateTime, TimeSpan, MatchStateCalculator

### Community 24 - "Log"
Cohesion: 0.14
Nodes (13): Log, Exception, HttpMethod, HttpUrl, InnerException, StackTrace, UserId, HttpContext (+5 more)

### Community 25 - "SportsDbTeam"
Cohesion: 0.17
Nodes (12): List, SportsDbTeam, IdLeague, IdTeam, StrBadge, StrCountry, StrSport, StrStadium (+4 more)

### Community 26 - ".LoadPredictionsAsync"
Cohesion: 0.17
Nodes (10): HashSet, PredictionOutcome, Exact, Missed, Pending, Postponed, Void, Winner (+2 more)

### Community 27 - "MainActivity"
Cohesion: 0.40
Nodes (3): Intent, MainActivity, MauiAppCompatActivity

### Community 28 - "User"
Cohesion: 0.12
Nodes (15): ICollection, User, DeviceTokens, Email, FavoriteTeams, FirstName, LastName, Password (+7 more)

### Community 29 - "SportsDbPlayerHonour"
Cohesion: 0.18
Nodes (10): SportsDbPlayerHonour, Id, IdHonour, IdLeague, IdPlayer, IdTeam, StrHonour, StrHonourLogo (+2 more)

### Community 30 - ".Login"
Cohesion: 0.25
Nodes (10): ActionResult, Authorize, CancellationToken, HttpPost, Task, AuthController, List, ErrorResponseDTO (+2 more)

### Community 31 - "ContentPage"
Cohesion: 0.12
Nodes (21): Email, FirstName, LastName, RegisterCommand, RepeatPassword, RegisterViewModel, Email, ErrorMessage (+13 more)

### Community 32 - "ContentPage"
Cohesion: 0.03
Nodes (78): BarColor, CanCollapseFriends, CanShowMoreFriends, CollapseFriendsCommand, Color, Detail, DonutDrawable, ExactText (+70 more)

### Community 33 - "TeamMatches"
Cohesion: 0.13
Nodes (15): TeamMatches, AwayTeamLogo, AwayTeamName, AwayTeamScore, AwayTeamShortName, EventDate, EventId, HomeTeamLogo (+7 more)

### Community 34 - "MatchNotificationService"
Cohesion: 0.30
Nodes (9): AwayOnlyUserIds, HomeUserIds, CancellationToken, HashSet, IDictionary, List, Task, MatchNotificationService (+1 more)

### Community 35 - "LeaderboardDTO"
Cohesion: 0.17
Nodes (12): List, LeaderboardDTO, Entries, Me, RankedUsers, LeaderboardEntryDTO, Exact, IsMe (+4 more)

### Community 36 - "SportsDbStartupSyncService"
Cohesion: 0.30
Nodes (7): CancellationToken, DateTime, ILogger, IServiceScopeFactory, Task, TimeSpan, SportsDbStartupSyncService

### Community 37 - "NavMenuViewModelBase"
Cohesion: 0.21
Nodes (8): CancellationToken, CancellationTokenSource, ObservableCollection, RelayCommand, Task, TimeSpan, NavMenuViewModelBase, HasNoSearchResults

### Community 38 - "SyncedEntityConfiguration"
Cohesion: 0.17
Nodes (10): Domain.Entities.BaseEntity, EntityTypeBuilder, BaseConfiguration, SyncedEntityConfiguration, DateTime, BaseEntity, CreatedAt, Id (+2 more)

### Community 39 - "PredictionStatsDTO"
Cohesion: 0.13
Nodes (15): PredictionStatsDTO, BestStreak, CurrentStreak, Exact, Missed, Pending, Points, Rank (+7 more)

### Community 40 - "EventTimeline"
Cohesion: 0.10
Nodes (18): EntityTypeBuilder, EventTimelineConfiguration, EventTimeline, APIId, Assistant, AssistantId, Comment, Detail (+10 more)

### Community 41 - "SportsDbLeague"
Cohesion: 0.20
Nodes (10): List, SportsDbLeague, IdLeague, StrBadge, StrCountry, StrCurrentSeason, StrLeague, StrSport (+2 more)

### Community 42 - "SportsDbPlayerFormerTeam"
Cohesion: 0.18
Nodes (10): SportsDbPlayerFormerTeam, Id, IdFormerTeam, IdPlayer, IntAppearances, IntGoals, StrBadge, StrDeparted (+2 more)

### Community 43 - "EventTimelineDTO"
Cohesion: 0.06
Nodes (32): FlexJustify, EventTimelineDTO, AssistDisplay, AssistIconText, AssistName, AssistOpen, CardColor, Comment (+24 more)

### Community 44 - "MAUI"
Cohesion: 0.29
Nodes (3): MAUI, Program, Program

### Community 45 - "SearchResultItem"
Cohesion: 0.12
Nodes (16): SearchResultItem, DisplayName, HasImage, IconGlyph, Id, ImageUrl, IsRecent, Kind (+8 more)

### Community 46 - "ContentView"
Cohesion: 0.22
Nodes (8): ChannelName, FlagImage, HasFlag, HasLogo, HasName, LogoThumbUrl, NoFlag, ContentView

### Community 47 - "PredictionStatsDTO"
Cohesion: 0.13
Nodes (15): PredictionStatsDTO, BestStreak, CurrentStreak, Exact, Missed, Pending, Points, Rank (+7 more)

### Community 48 - "ContentPage"
Cohesion: 0.03
Nodes (73): AwayLineup, AwayScore, BroadcastRows, CanPredict, DetailedStatusDisplay, Details.AwayTeamBadge, Details.AwayTeamId, Details.AwayTeamName (+65 more)

### Community 49 - "MAUI.ViewModel"
Cohesion: 0.09
Nodes (8): MAUI.ViewModel, MAUI.Views, MAUI.Views.Drawables, MAUI.Helpers, Dictionary, CountryFlagHelper, Icons, PlayerPageViewModel

### Community 50 - ".CreateMauiApp"
Cohesion: 0.22
Nodes (7): Exception, HomeViewModel, IActivationState, MauiApp, Window, CrashApp, MauiProgram

### Community 51 - "ResourceDictionary"
Cohesion: 0.29
Nodes (9): CommonStates, Disabled, Normal, Off, On, PointerOver, ResourceDictionary, VisualState (+1 more)

### Community 52 - "ContentPage"
Cohesion: 0.06
Nodes (33): AcceptCommand, DeclineCommand, IsFriend, IsLocked, IsRequestReceived, LockedText, PrimaryActionCommand, PrimaryButtonBackground (+25 more)

### Community 53 - "IPushNotificationService"
Cohesion: 0.25
Nodes (5): Task, IPushNotificationService, Task, PushNotificationService, PostNotifications

### Community 54 - "SyncedEntity"
Cohesion: 0.09
Nodes (19): DataAccess.Configurations.BaseConfiguration, EntityTypeBuilder, ChannelConfiguration, EntityTypeBuilder, EventTvConfiguration, ICollection, Channel, Broadcasts (+11 more)

### Community 55 - "SportsDbEventLineup"
Cohesion: 0.14
Nodes (14): SportsDbEventLineup, IdApiFootball, IdEvent, IdLineup, IdPlayer, IdTeam, IntSquadNumber, StrFormation (+6 more)

### Community 56 - "ISearchHistoryApiService"
Cohesion: 0.17
Nodes (12): RecordSearchRequestDTO, EntityId, Kind, CancellationToken, List, Task, ISearchHistoryApiService, CancellationToken (+4 more)

### Community 57 - "EventDetailsDTO"
Cohesion: 0.08
Nodes (25): DateTime, List, EventDetailsDTO, AwayTeamBadge, AwayTeamId, AwayTeamName, AwayTeamScore, Broadcasts (+17 more)

### Community 58 - "SportsDbSport"
Cohesion: 0.25
Nodes (8): List, SportsDbSport, IdSport, StrSport, StrSportDescription, StrSportThumb, SportsDbSportResponse, Sports

### Community 59 - "Squad"
Cohesion: 0.25
Nodes (8): DateTime, Squad, BirthDate, FirstName, ImageUrl, LastName, PlayerId, Position

### Community 60 - "EventLineup"
Cohesion: 0.12
Nodes (15): EntityTypeBuilder, EventLineupConfiguration, EventLineup, ApiId, Event, EventId, Formation, HomeGame (+7 more)

### Community 61 - "Team"
Cohesion: 0.06
Nodes (29): EntityTypeBuilder, EntityTypeBuilder, PlayerMilestoneConfiguration, Player, DateOnly, PlayerMilestone, DateAchieved, MilestoneId (+21 more)

### Community 62 - "AppDelegate"
Cohesion: 0.22
Nodes (5): MauiApp, AppDelegate, MauiApp, AppDelegate, MauiUIApplicationDelegate

### Community 63 - "LeaderboardRowDTO"
Cohesion: 0.11
Nodes (19): FontAttributes, FormDotDTO, Color, Letter, LeaderboardRowDTO, BarColor, Detail, Exact (+11 more)

### Community 64 - "LeaderboardEntryDTO"
Cohesion: 0.17
Nodes (12): List, LeaderboardDTO, Entries, Me, RankedUsers, LeaderboardEntryDTO, Exact, IsMe (+4 more)

### Community 65 - "LoginViewModel"
Cohesion: 0.13
Nodes (17): LoginCommand, IConfiguration, MauiExtensions, LoginViewModel, ErrorMessage, IsBusy, LoginCommand, Password (+9 more)

### Community 66 - "SportsDbSyncService"
Cohesion: 0.13
Nodes (18): FirstName, Func, CancellationToken, DateTime, Dictionary, IEnumerable, ILogger, League (+10 more)

### Community 67 - "ISportsDbClient"
Cohesion: 0.26
Nodes (4): CancellationToken, List, Task, ISportsDbClient

### Community 68 - "ScoreHub App Icon Foreground (Trophy Mark)"
Cohesion: 0.25
Nodes (8): ScoreHub App Icon Background, ScoreHub App Icon Foreground (Trophy Mark), ScoreHub Logo 160x160, ScoreHub Logo 160x90, ScoreHub Logo 160x90 v2 (Blank Variant), ScoreHub Logo 64x36, ScoreHub Logo 64x64, ScoreHub Splash Screen

### Community 69 - "Prediction"
Cohesion: 0.17
Nodes (10): EntityTypeBuilder, Prediction, Hit, Match, MatchId, Points, PredictedAwayScore, PredictedHomeScore (+2 more)

### Community 70 - "App"
Cohesion: 0.29
Nodes (4): MAUI.WinUI, MauiWinUIApplication, MauiApp, App

### Community 71 - "Team"
Cohesion: 0.14
Nodes (11): EntityTypeBuilder, EntityTypeBuilder, TeamEquipmentConfiguration, Team, TeamEquipment, Equipment, Season, Team (+3 more)

### Community 72 - "PredictionDTO"
Cohesion: 0.25
Nodes (5): PredictionDTO, Outcome, Points, PredictedAwayScore, PredictedHomeScore

### Community 73 - "IPredictionApiService"
Cohesion: 0.20
Nodes (11): CancellationToken, List, Task, IPredictionApiService, CancellationToken, HttpClient, LeaderboardDTO, List (+3 more)

### Community 74 - "ContentPage"
Cohesion: 0.04
Nodes (49): AwayTeamDisplayName, AwayTeamLogo, DateTimeText, Details.Equipment, Details.Matches, Details.NameDisplay, Details.SportName, Details.Squad (+41 more)

### Community 75 - ".CreateWindow"
Cohesion: 0.29
Nodes (4): IActivationState, Window, Shell, AppShell

### Community 76 - "FriendDTO"
Cohesion: 0.18
Nodes (10): DateTime, List, FriendDTO, FriendsSince, FriendsSinceDisplay, UserId, Username, FriendsPageDTO (+2 more)

### Community 77 - "BaseEntity"
Cohesion: 0.17
Nodes (11): BaseEntity, Jwt, IsRevoked, RefreshJwtToken, Token, User, UserId, DateTime (+3 more)

### Community 78 - "FriendsController"
Cohesion: 0.15
Nodes (22): ActionResult, CancellationToken, FriendRequestDTO, FriendsPageDTO, HttpDelete, HttpGet, HttpPost, List (+14 more)

### Community 79 - "User"
Cohesion: 0.29
Nodes (5): EntityTypeBuilder, DeviceToken, Token, User, UserId

### Community 80 - "Infrastructure.Integrations.DTO"
Cohesion: 0.05
Nodes (29): Infrastructure.Integrations.DTO, SportsDbEventTimeline, IdApiFootball, IdAssist, IdEvent, IdPlayer, IdTeam, IdTimeline (+21 more)

### Community 81 - "Friendship"
Cohesion: 0.15
Nodes (14): DateTime, Friendship, AcceptedAt, Addressee, AddresseeId, Requester, RequesterId, Status (+6 more)

### Community 82 - ".GetTeamDetails"
Cohesion: 0.22
Nodes (12): ActionResult, Authorize, CancellationToken, HttpDelete, HttpGet, HttpPost, IActionResult, List (+4 more)

### Community 83 - "UserViewModel"
Cohesion: 0.07
Nodes (26): ObservableCollection, UserViewModel, AcceptCommand, FavoriteTeams, FavoriteTeamSelectedCommand, HasFavoriteTeams, HasMoreFavoriteTeams, IsFavoriteTeamsExpanded (+18 more)

### Community 84 - "PredictionsListModel"
Cohesion: 0.10
Nodes (18): IReadOnlyList, IEnumerable, List, ObservableCollection, RelayCommand, PredictionsListModel, FinishedPredictions, HasAnyPredictions (+10 more)

### Community 85 - "RecentSearchDTO"
Cohesion: 0.25
Nodes (7): RecentSearchDTO, DisplayName, EntityId, Id, ImageUrl, Kind, SportName

### Community 86 - "League"
Cohesion: 0.06
Nodes (26): Domain.Entities.SyncedEntity, EntityTypeBuilder, LeagueConfiguration, EntityTypeBuilder, MatchConfiguration, PlayerConfiguration, TeamConfiguration, EntityTypeBuilder (+18 more)

### Community 87 - "MainApplication"
Cohesion: 0.22
Nodes (5): Builder, FCMNotification, MauiApp, MainApplication, MauiApplication

### Community 88 - "graphify Knowledge Graph Tool"
Cohesion: 0.50
Nodes (4): GRAPH_REPORT.md Reference, graphify Knowledge Graph Tool, graphify update Command, graphify Wiki Index

### Community 89 - ".AddPushNotifications"
Cohesion: 0.18
Nodes (9): ILoggerFactory, FirebaseMessaging, ILogger, FcmPushNotificationService, FirebaseOptions, ServiceAccountKeyPath, IPushNotificationService, IOptions (+1 more)

### Community 90 - "App"
Cohesion: 0.33
Nodes (3): FCMNotificationTappedEventArgs, Application, App

### Community 91 - "AddFriendship"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddFriendship

### Community 93 - "LoadMauiAsset Method"
Cohesion: 0.67
Nodes (3): LoadMauiAsset Method, MauiAsset Build Action, FileSystem.OpenAppPackageFileAsync

### Community 95 - "PlayerStatistics"
Cohesion: 0.33
Nodes (6): PlayerStatistics, League, Season, Statistic, Team, Value

### Community 96 - "PlayerSearchResultDTO"
Cohesion: 0.33
Nodes (6): PlayerSearchResultDTO, FirstName, Id, ImageUrl, LastName, SportName

### Community 97 - "ContentView"
Cohesion: 0.08
Nodes (24): AccentColor, CardBackground, FinishedPredictions, HasAnyPredictions, HasFinishedPredictions, HasLivePredictions, HasMorePredictions, HasNoPredictionsForFilter (+16 more)

### Community 98 - "MatchScheduleBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchScheduleBackgroundService

### Community 99 - "IFriendApiService"
Cohesion: 0.19
Nodes (7): CancellationToken, List, Task, IFriendApiService, List, RelayCommand, Task

### Community 100 - "UserRole"
Cohesion: 0.29
Nodes (5): EntityTypeBuilder, ICollection, UserRole, Name, Users

### Community 101 - "PredictionDonutDrawable"
Cohesion: 0.33
Nodes (4): ICanvas, IDrawable, PredictionDonutDrawable, RectF

### Community 102 - "FriendApiService"
Cohesion: 0.31
Nodes (7): HttpMethod, CancellationToken, FriendsPageDTO, HttpClient, List, Task, FriendApiService

### Community 104 - "JwtOptions"
Cohesion: 0.20
Nodes (7): JwtOptions, Audience, Issuer, RefreshTokenExpirationHours, SecretKey, TokenExpirationMinutes, JwtService

### Community 105 - ".BuildTargetModel"
Cohesion: 0.50
Nodes (3): DateOnly, DateTime, ModelBuilder

### Community 107 - "FavoriteTeam"
Cohesion: 0.22
Nodes (7): EntityTypeBuilder, FavoriteTeam, Notifications, Team, TeamId, User, UserId

### Community 108 - "RegisterRequestDTO"
Cohesion: 0.29
Nodes (6): RegisterRequestDTO, Email, FirstName, LastName, Password, Username

### Community 109 - "PlayerMilestones"
Cohesion: 0.22
Nodes (8): DateOnly, PlayerMilestones, DateAchieved, HasImage, HasTeam, MilestoneName, MilestoneUrl, Team

### Community 110 - ".Fill"
Cohesion: 0.15
Nodes (5): CancellationToken, CancellationTokenSource, IEnumerable, List, ObservableCollection

### Community 111 - "SportsOptions"
Cohesion: 0.29
Nodes (6): List, SportsOptions, ApiKey, BaseUrl, LeagueExternalIds, SportsIds

### Community 112 - "PlayerSearchResultDTO"
Cohesion: 0.33
Nodes (6): PlayerSearchResultDTO, FirstName, Id, ImageUrl, LastName, SportName

### Community 115 - ".AddSportsDbIntegration"
Cohesion: 0.52
Nodes (3): IServiceCollection, IConfiguration, Extensions

### Community 116 - ".ShowAsync"
Cohesion: 0.29
Nodes (4): Task, ConfirmDialog, Task, FriendDialogs

### Community 118 - "LoginRequestDTO"
Cohesion: 0.50
Nodes (3): LoginRequestDTO, Password, Username

### Community 119 - "RefreshRequestDTO"
Cohesion: 0.50
Nodes (3): RefreshRequestDTO, RefreshToken, Token

### Community 120 - "AuthResponseDTO"
Cohesion: 0.50
Nodes (3): AuthResponseDTO, RefreshToken, Token

### Community 121 - "NavMenuSportsDTO"
Cohesion: 0.50
Nodes (3): NavMenuSportsDTO, Id, Name

### Community 136 - "DataAccess.Configurations"
Cohesion: 0.09
Nodes (15): BaseConfiguration, DataAccess.Configurations, EntityTypeBuilder, DeviceTokenConfiguration, FavoriteTeamConfiguration, EntityTypeBuilder, FriendshipConfiguration, EntityTypeBuilder (+7 more)

### Community 141 - "LeagueDetailsDTO"
Cohesion: 0.11
Nodes (18): List, LeagueDetailsDTO, Country, FlagImage, HasFlag, HasLogo, LogoUrl, Name (+10 more)

### Community 148 - "MyPredictionDTO"
Cohesion: 0.07
Nodes (27): DateTime, MyPredictionDTO, AccentColor, AwayTeam, AwayTeamScore, CardBackground, EventDate, EventId (+19 more)

### Community 183 - "SavePredictionRequestDTO"
Cohesion: 0.50
Nodes (3): SavePredictionRequestDTO, PredictedAwayScore, PredictedHomeScore

### Community 185 - "PredictionsController"
Cohesion: 0.27
Nodes (12): IReadOnlyCollection, RankedUser, ActionResult, CancellationToken, HttpGet, LeaderboardDTO, List, PredictionDTO (+4 more)

### Community 225 - "PlayerStatistic"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, PlayerStatisticConfiguration, PlayerStatistic, League, LeagueId, Player, PlayerId, Season (+4 more)

### Community 228 - "PlayerDTO"
Cohesion: 0.06
Nodes (34): DateTime, List, PlayerDTO, Age, BirthDate, BirthLocation, Description, FacebookLink (+26 more)

### Community 229 - "WeatherForecast"
Cohesion: 0.13
Nodes (11): WebAPI, WeatherForecast, HttpGet, IEnumerable, WeatherForecastController, DateOnly, WeatherForecast, Date (+3 more)

### Community 268 - "Venue"
Cohesion: 0.12
Nodes (13): EntityTypeBuilder, SportConfiguration, Sport, ICollection, Venue, Country, ImageUrl, Location (+5 more)

### Community 272 - "SportMenuItem"
Cohesion: 0.14
Nodes (12): IconFontFamily, FontAwesome, MaterialDesignIcons, SportIcon, FontFamilyName, Dictionary, SportIconMap, SportMenuItem (+4 more)

### Community 279 - "SportsDbEventTv"
Cohesion: 0.22
Nodes (8): SportsDbEventTv, Id, IdChannel, IdEvent, StrChannel, StrCountry, StrLogo, StrTimeStamp

### Community 305 - "EventDetailsDTO"
Cohesion: 0.07
Nodes (26): DateTime, List, EventDetailsDTO, AwayTeamBadge, AwayTeamId, AwayTeamName, AwayTeamScore, Broadcasts (+18 more)

### Community 306 - "EventsDTO"
Cohesion: 0.06
Nodes (35): ActionResult, CancellationToken, HttpGet, List, MatchesDTO, Task, DateTime, MatchesFiltersDTO (+27 more)

### Community 307 - "SportsDbPlayerStatistic"
Cohesion: 0.22
Nodes (8): SportsDbPlayerStatistic, Id, IdLeague, IdPlayer, IdTeam, StrSeason, StrStatistic, StrValue

### Community 311 - "MatchStatisticsTrackerService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchStatisticsTrackerService

### Community 321 - "PlayerViewModel"
Cohesion: 0.06
Nodes (31): List, PlayerStatisticsSeasonGroup, Items, Season, PlayerViewModel, Details, ErrorMessage, HasAnyTabData (+23 more)

### Community 322 - "Standings"
Cohesion: 0.09
Nodes (20): EntityTypeBuilder, StandingsConfiguration, Standings, Description, Form, GoalDifference, GoalsAgainst, GoalsFor (+12 more)

### Community 328 - "PlayerFormerTeamBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PlayerFormerTeamBackgroundService

### Community 329 - "EventDetailsViewModel"
Cohesion: 0.03
Nodes (53): List, EventDetailsViewModel, AllBroadcastRows, AwayLineup, BroadcastRows, CanPredict, DetailedStatusDisplay, Details (+45 more)

### Community 330 - ".GetSports"
Cohesion: 0.33
Nodes (5): ActionResult, CancellationToken, HttpGet, List, Task

### Community 333 - "PlayerHonour"
Cohesion: 0.12
Nodes (14): EntityTypeBuilder, PlayerHonourConfiguration, PlayerHonour, HonourId, HonourLogoUrl, HonourName, HonourTrophyUrl, League (+6 more)

### Community 335 - "FavoriteTeamDTO"
Cohesion: 0.05
Nodes (45): FavoriteTeamDTO, HasShortName, LogoUrl, ShortName, SportName, TeamDisplayName, TeamId, TeamName (+37 more)

### Community 348 - "HomeViewModel"
Cohesion: 0.12
Nodes (14): date, CancellationToken, CancellationTokenSource, DateTime, List, ObservableCollection, RelayCommand, Task (+6 more)

### Community 349 - "PlayerDTO"
Cohesion: 0.08
Nodes (24): DateTime, List, PlayerDTO, BirthDate, BirthLocation, Description, FacebookLink, FirstName (+16 more)

### Community 398 - "TeamMatches"
Cohesion: 0.09
Nodes (22): TeamMatches, AwayTeamDisplayName, AwayTeamLogo, AwayTeamName, AwayTeamScore, AwayTeamShortName, DateTimeText, EventDate (+14 more)

### Community 399 - "TeamViewModel"
Cohesion: 0.07
Nodes (23): RelayCommand, Task, TeamViewModel, Details, ErrorMessage, FavoriteButtonIcon, FavoriteButtonIconFontFamily, FavoriteButtonText (+15 more)

### Community 401 - "LeagueSearchResultDTO"
Cohesion: 0.22
Nodes (8): LeagueSearchResultDTO, Id, ImageUrl, Name, TeamSearchResultDTO, Id, ImageUrl, Name

### Community 402 - "ContentView"
Cohesion: 0.09
Nodes (22): AwayTeamLogoUrl, EventId, Events, HasLeagueLogo, HomeTeamLogoUrl, LeagueId, LeagueLogoUrl, LeagueName (+14 more)

### Community 403 - "TeamPage"
Cohesion: 0.29
Nodes (4): EventArgs, PropertyChangedEventArgs, Task, TeamPage

### Community 404 - "IPlayerApiService"
Cohesion: 0.23
Nodes (9): CancellationToken, List, Task, IPlayerApiService, CancellationToken, HttpClient, List, Task (+1 more)

### Community 405 - "TeamDTO"
Cohesion: 0.11
Nodes (19): List, TeamDTO, Country, Equipment, FlagImage, HasFlag, ImageUrl, Matches (+11 more)

### Community 406 - "MatchesList"
Cohesion: 0.07
Nodes (25): DateTime, DateFilterOption, Date, DisplayText, BindableObject, BindableProperty, BindablePropertyKey, DateTime (+17 more)

### Community 407 - "ITeamApiService"
Cohesion: 0.17
Nodes (13): TeamSearchResultDTO, Id, ImageUrl, Name, CancellationToken, List, Task, ITeamApiService (+5 more)

### Community 408 - "EventLineupDTO"
Cohesion: 0.13
Nodes (15): EventLineupDTO, FirstName, Formation, HasImage, HomeGame, ImageUrl, IsSubtitute, LastName (+7 more)

### Community 409 - "ILeagueApiService"
Cohesion: 0.13
Nodes (15): LeagueSearchResultDTO, Id, ImageUrl, Name, CancellationToken, List, Task, ILeagueApiService (+7 more)

### Community 410 - "ScoreFlashBehavior"
Cohesion: 0.10
Nodes (18): Behavior, Label, EventArgs, PropertyChangedEventArgs, Task, ScoreFlashBehavior, ContentPage, MessageLabel (+10 more)

### Community 411 - "ContentPage"
Cohesion: 0.12
Nodes (16): IsAnyMenuOpen, IsInitialLoading, SelectedDate, SelectedStatus, EventSelectedCommand, IsInitialLoading, LeagueSelectedCommand, SelectedDate (+8 more)

### Community 412 - "UsersController"
Cohesion: 0.06
Nodes (40): ActionResult, CancellationToken, FavoriteTeamDTO, HttpDelete, HttpGet, HttpPost, IActionResult, List (+32 more)

### Community 413 - "NavMenuControl"
Cohesion: 0.12
Nodes (17): ClearRecentSearchesCommand, IsLoggedIn, IsSearchFocused, RemoveRecentSearchCommand, SearchResultSelectedCommand, SportSelectedCommand, SportSelectedCommand, NavMenuControl (+9 more)

### Community 414 - "PlayerFormerTeams"
Cohesion: 0.17
Nodes (12): PlayerFormerTeams, Appearences, Departed, Goals, HasAppearances, HasDeparted, HasGoals, Joined (+4 more)

### Community 415 - "Squad"
Cohesion: 0.18
Nodes (11): DateTime, Squad, BirthDate, FirstName, HasImage, ImageUrl, LastName, NoImage (+3 more)

### Community 416 - "PlayerSyncBackgroundService"
Cohesion: 0.38
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PlayerSyncBackgroundService

### Community 418 - "DatePicker"
Cohesion: 0.27
Nodes (8): DateOptions, DisplayText, IsPickerEnabled, StatusOptions, DatePicker, StatusPicker, EventArgs, Picker

### Community 419 - "ContentView"
Cohesion: 0.08
Nodes (27): ., DisplayName, IconFontFamily, IconGlyph, IsRecent, KindLabel, Name, AccountMenuBorder (+19 more)

### Community 420 - "MatchesDTO"
Cohesion: 0.18
Nodes (12): ObservableCollection, LeaguesDTO, Events, HasLeagueLogo, LeagueId, LeagueLogoUrl, LeagueName, NoLeagueLogo (+4 more)

### Community 421 - "EventsDTO"
Cohesion: 0.18
Nodes (11): DateTime, EventsDTO, AwayTeam, AwayTeamLogoUrl, EventDate, EventId, HomeTeam, HomeTeamLogoUrl (+3 more)

### Community 422 - "PlayerHonours"
Cohesion: 0.20
Nodes (10): PlayerHonours, HasLeague, HasTeam, HasTrophyImage, HonourName, HonourTrophyUrl, League, Season (+2 more)

### Community 423 - "PlayerStatistics"
Cohesion: 0.18
Nodes (11): PlayerStatistics, HasLeague, HasTeam, League, Season, StatDisplay, Statistic, SubText (+3 more)

### Community 424 - ".IsLoggedInAsync"
Cohesion: 0.31
Nodes (3): PredictionDTO, RelayCommand, Task

### Community 425 - ".GetMatchesAsync"
Cohesion: 0.17
Nodes (11): CancellationToken, DateTime, List, Task, IMatchesAPIService, CancellationToken, DateTime, HttpClient (+3 more)

### Community 426 - ".GetPlayerDetails"
Cohesion: 0.29
Nodes (8): ActionResult, CancellationToken, HttpGet, List, PlayerDTO, PlayerSearchResultDTO, Task, PlayerController

### Community 427 - "SportsDbStanding"
Cohesion: 0.11
Nodes (18): SportsDbStanding, IdLeague, IdStanding, IdTeam, IntDraw, IntGoalDifference, IntGoalsAgainst, IntGoalsFor (+10 more)

### Community 428 - "PlayerPage"
Cohesion: 0.47
Nodes (3): EventArgs, Task, PlayerPage

### Community 429 - "MAUI.Views.Partials"
Cohesion: 0.12
Nodes (12): ContentView, MAUI.Views.Partials, BroadcastCell, BindableProperty, LoadingOverlay, IsLoading, TappedEventArgs, PredictionsSection (+4 more)

### Community 430 - "EventStatistic"
Cohesion: 0.17
Nodes (10): EntityTypeBuilder, EventStatisticsConfiguration, EventStatistic, ApiId, AwayScore, Event, EventId, EventName (+2 more)

### Community 432 - "PlayerFormerTeams"
Cohesion: 0.25
Nodes (8): PlayerFormerTeams, Appearences, Departed, Goals, Joined, MoveType, Team, TeamLogoUrl

### Community 433 - "InvertedBoolConverter"
Cohesion: 0.38
Nodes (4): CultureInfo, IValueConverter, InvertedBoolConverter, Type

### Community 434 - "LeagueStandingDTO"
Cohesion: 0.10
Nodes (21): FormattedString, LeagueStandingDTO, Description, DescriptionColorHex, Form, FormDisplay, GoalDifference, GoalsAgainst (+13 more)

### Community 435 - "PlayerHonours"
Cohesion: 0.29
Nodes (6): PlayerHonours, HonourName, HonourTrophyUrl, League, Season, Team

### Community 438 - "Domain.Entities"
Cohesion: 0.13
Nodes (11): WebAPI.Controllers, Domain.Predictions, Infrastructure.Sync, WebAPI.DTO.Output, WebAPI.DTO.Input, DataAccess, Domain.Entities, WebAPI.Middleware (+3 more)

### Community 439 - "EventDetailsPage"
Cohesion: 0.33
Nodes (3): EventArgs, Task, EventDetailsPage

### Community 444 - "LeagueViewModel"
Cohesion: 0.14
Nodes (13): ObservableCollection, LeagueViewModel, Details, ErrorMessage, HasStandings, HasTeams, IsBusy, LeagueId (+5 more)

### Community 445 - "IDeviceTokenApiService"
Cohesion: 0.18
Nodes (10): HttpRequestMessage, DeviceTokenDTO, Token, CancellationToken, HttpClient, Task, DeviceTokenApiService, CancellationToken (+2 more)

### Community 447 - "LeagueStandingDTO"
Cohesion: 0.12
Nodes (16): LeagueStandingDTO, Description, Form, GoalDifference, GoalsAgainst, GoalsFor, MatchesDraw, MatchesLost (+8 more)

### Community 450 - "LeagueDetailsDTO"
Cohesion: 0.17
Nodes (12): List, LeagueDetailsDTO, Country, LogoUrl, Name, Standings, Teams, LeagueTeamDTO (+4 more)

### Community 453 - "SportsDbTeamEquipment"
Cohesion: 0.25
Nodes (7): SportsDbTeamEquipment, IdEquipment, IdTeam, StrEquipment, StrSeason, StrType, StrUsername

### Community 460 - ".GetLeagueDetails"
Cohesion: 0.29
Nodes (8): ActionResult, CancellationToken, HttpGet, LeagueDetailsDTO, LeagueSearchResultDTO, List, Task, LeagueController

### Community 479 - "DataAccess.Migrations"
Cohesion: 0.32
Nodes (4): DataAccess.Migrations, DateTime, MigrationBuilder, AddEventTv

### Community 482 - "ProfileViewModel"
Cohesion: 0.03
Nodes (67): IDrawable, ProfileViewModel, AcceptRequestCommand, CanCollapseFriends, CanShowMoreFriends, DeclineRequestCommand, ExactText, FavoriteTeams (+59 more)

### Community 486 - "SearchEntry"
Cohesion: 0.40
Nodes (4): FocusEventArgs, SearchEntry, SearchText, Entry

### Community 504 - "EventTimelineBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, EventTimelineBackgroundService

### Community 507 - "EventTvBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, EventTvBackgroundService

### Community 508 - "PlayerMilestones"
Cohesion: 0.33
Nodes (6): DateOnly, PlayerMilestones, DateAchieved, MilestoneName, MilestoneUrl, Team

### Community 509 - "MyPredictionDTO"
Cohesion: 0.14
Nodes (13): DateTime, MyPredictionDTO, AwayTeam, AwayTeamScore, EventDate, EventId, HomeTeam, HomeTeamScore (+5 more)

### Community 510 - ".SendAsync"
Cohesion: 0.20
Nodes (8): CancellationToken, IDictionary, IEnumerable, Task, CancellationToken, IDictionary, IEnumerable, Task

### Community 513 - ".GetEventDetailsAsync"
Cohesion: 0.33
Nodes (4): CancellationToken, Task, CancellationToken, Task

### Community 515 - "BackgroundService"
Cohesion: 0.33
Nodes (7): BackgroundService, CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchNotificationBackgroundService

### Community 517 - "ContentView"
Cohesion: 0.08
Nodes (24): AssistDisplay, AssistIconText, AssistOpen, CardColor, CommentDisplay, FlexJustify, GoalPlayerDisplay, HasComment (+16 more)

### Community 518 - "AddEventTimeline"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddEventTimeline

### Community 519 - "PreMatchLineupBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PreMatchLineupBackgroundService

### Community 520 - "ContentPage"
Cohesion: 0.18
Nodes (8): ContentPage, Task, ImagePreviewHelper, ContentPage, PreviewImage, EventArgs, ImagePreviewPage, Image

### Community 522 - ".GetEventDetails"
Cohesion: 0.33
Nodes (5): ActionResult, CancellationToken, EventDetailsDTO, HttpGet, Task

### Community 523 - "SportsDbEventStatistic"
Cohesion: 0.22
Nodes (8): SportsDbEventStatistic, IdApiFootball, IdEvent, IdStatistic, IntAway, IntHome, StrEvent, StrStat

### Community 525 - "RenameEventTimelineApiId"
Cohesion: 0.22
Nodes (5): MigrationBuilder, DateOnly, DateTime, ModelBuilder, RenameEventTimelineApiId

### Community 526 - "SportsDbVenue"
Cohesion: 0.25
Nodes (7): SportsDbVenue, IdVenue, StrCountry, StrLocation, StrSport, StrThumb, StrVenue

### Community 527 - ".BuildModel"
Cohesion: 0.29
Nodes (5): DateOnly, DateTime, ModelBuilder, ScoreHubDbContextModelSnapshot, ModelSnapshot

### Community 528 - "EventStatisticsDTO"
Cohesion: 0.50
Nodes (4): EventStatisticsDTO, AwayScore, HomeScore, Stat

### Community 529 - "EventTimelineDTO"
Cohesion: 0.25
Nodes (8): EventTimelineDTO, AssistName, Comment, Detail, Home, Minute, PlayerName, Type

### Community 530 - "IMatchNotificationService"
Cohesion: 0.60
Nodes (3): CancellationToken, Task, IMatchNotificationService

### Community 532 - "Migration"
Cohesion: 0.17
Nodes (8): DateOnly, DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, Baseline_PreEventTv, Migration

### Community 533 - "AddUserSearch"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddUserSearch

### Community 534 - "AddMatchTvSyncedAt"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddMatchTvSyncedAt

### Community 537 - "PredictionDTO"
Cohesion: 0.33
Nodes (5): PredictionDTO, Outcome, Points, PredictedAwayScore, PredictedHomeScore

### Community 551 - "ContentView"
Cohesion: 0.40
Nodes (4): IsLoading, ContentView, Root, ContentView

## Knowledge Gaps
- **1856 isolated node(s):** `Class1`, `net10.0`, `Microsoft.EntityFrameworkCore (10.0.9)`, `Microsoft.EntityFrameworkCore.SqlServer (10.0.9)`, `Microsoft.NET.Sdk` (+1851 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 2268 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **11 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ScoreHubDbContext` connect `ScoreHubDbContext` to `BackgroundService`, `PreMatchLineupBackgroundService`, `Venue`, `Match`, `UserSearch`, `PlayerFormerTeam`, `Log`, `UsersController`, `.Login`, `PlayerSyncBackgroundService`, `MatchNotificationService`, `SportsDbStartupSyncService`, `EventTimeline`, `.GetPlayerDetails`, `EventStatistic`, `SyncedEntity`, `Domain.Entities`, `MatchStatisticsTrackerService`, `PredictionsController`, `EventLineup`, `Team`, `SportsDbSyncService`, `Prediction`, `Team`, `PlayerFormerTeamBackgroundService`, `.GetLeagueDetails`, `PlayerHonour`, `FriendsController`, `User`, `Friendship`, `.GetTeamDetails`, `League`, `.AddPushNotifications`, `PlayerStatistic`, `MatchScheduleBackgroundService`, `UserRole`, `FavoriteTeam`, `.AddSportsDbIntegration`, `EventTimelineBackgroundService`, `EventTvBackgroundService`?**
  _High betweenness centrality (0.094) - this node is a cross-community bridge._
- **Why does `ProfileViewModel` connect `ProfileViewModel` to `ContentPage`, `LoginViewModel`, `IFriendApiService`, `NavMenuViewModelBase`, `PredictionStatsDTO`, `IPredictionApiService`, `ProfilePage`, `FriendDTO`, `.Fill`, `FavoriteTeamDTO`, `MAUI.ViewModel`, `PredictionsListModel`, `ITeamApiService`, `Task`, `LeaderboardRowDTO`?**
  _High betweenness centrality (0.064) - this node is a cross-community bridge._
- **Why does `MAUI.DTO.Input` connect `MAUI.DTO.Input` to `EventBroadcastDTO`, `MatchesDTO`, `PredictionDonutDrawable`, `.AddAppServices`, `PredictionDTO`, `IAuthApiService`, `FriendDTO`, `LeagueDetailsDTO`, `PlayerMilestones`, `FavoriteTeamDTO`, `MAUI.Views.Partials`, `MAUI.ViewModel`, `MyPredictionDTO`, `RecentSearchDTO`, `TeamDTO`, `ILeagueApiService`, `IDeviceTokenApiService`, `LeaderboardRowDTO`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Are the 31 inferred relationships involving `ProfileViewModel` (e.g. with `AcceptRequestCommand` and `CollapseFriendsCommand`) actually correct?**
  _`ProfileViewModel` has 31 INFERRED edges - model-reasoned connections that need verification._
- **Are the 12 inferred relationships involving `ContentPage` (e.g. with `EventDetailsViewModel` and `ErrorMessage`) actually correct?**
  _`ContentPage` has 12 INFERRED edges - model-reasoned connections that need verification._
- **Are the 14 inferred relationships involving `ContentPage` (e.g. with `ProfileViewModel` and `CollapseFriendsCommand`) actually correct?**
  _`ContentPage` has 14 INFERRED edges - model-reasoned connections that need verification._
- **Are the 18 inferred relationships involving `EventDetailsViewModel` (e.g. with `Details` and `ErrorMessage`) actually correct?**
  _`EventDetailsViewModel` has 18 INFERRED edges - model-reasoned connections that need verification._