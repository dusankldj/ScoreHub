# Graph Report - project  (2026-10-17)

## Corpus Check
- 310 files · ~460,590 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 3932 nodes · 6459 edges · 223 communities (211 shown, 12 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 494 edges (avg confidence: 0.73)
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
- User
- .GetRecentSearches
- PlayerFormerTeam
- Output/EventDetailsDTO.cs
- http
- TeamDTO
- .SavePrediction
- Log
- SportsDbTeam
- PredictionOutcome
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
- Infrastructure.Integrations.DTO
- SportsDbPlayerFormerTeam
- EventTimelineDTO
- MAUI
- SearchResultItem
- UserProfileDTO
- PredictionStatsDTO
- ContentPage
- CountryFlagHelper
- .CreateMauiApp
- ResourceDictionary
- ContentPage
- UserPageDTO
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
- TeamEquipment
- PredictionDTO
- IPredictionApiService
- ContentPage
- .CreateWindow
- FriendDTO
- UserSearch
- FriendsController
- PlayerMilestone
- SportsDbEventTimeline
- Friendship
- .GetTeamDetails
- UserViewModel
- PredictionsListModel
- RecentSearchDTO
- BaseConfiguration
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
- WebAPI.DTO.Output
- .AddSportsDbIntegration
- UserPageDTO
- FriendApiService
- TabContentContainer
- ScoreFlashBehavior
- SportScoringRule
- AddSportScoringRules
- FavoriteTeam
- IPushNotificationService
- PlayerMilestones
- .Fill
- SportsOptions
- PlayerSearchResultDTO
- .LoadPlayerDetailsAsync
- TabContentContainer
- Sport
- .ShowAsync
- HomePage
- .HandleSportSelected
- .GetSports
- ScoringRule
- FriendDTO
- ContentView
- SportsDbPlayerMilestone
- FriendshipHelper
- PredictionDonutDrawable
- FavoriteTeamDTO
- UserProfileDTO
- ContentView
- SportsDbApiException
- .BuildTargetModel
- DataAccess.Configurations
- LeagueDetailsDTO
- MyPredictionDTO
- PredictionsController
- PlayerStatistic
- PlayerDTO
- WeatherForecast
- League
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
- ContentPage
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
- .GetLeagueDetails
- TappedEventArgs
- Migration
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
- .GetEventDetails
- SportsDbEventStatistic
- RenameEventTimelineApiId
- .BuildModel
- EventStatisticsDTO
- EventTimelineDTO
- IMatchNotificationService
- DataAccess.Migrations
- AddUserSearch
- AddMatchTvSyncedAt
- Task
- PredictionDTO
- ProfilePage
- .OnLogoTapped

## God Nodes (most connected - your core abstractions)
1. `ProfileViewModel` - 125 edges
2. `ScoreHubDbContext` - 96 edges
3. `ContentPage` - 96 edges
4. `ContentPage` - 95 edges
5. `EventDetailsViewModel` - 81 edges
6. `Domain.Entities` - 79 edges
7. `ContentPage` - 76 edges
8. `MAUI.DTO.Input` - 62 edges
9. `ContentPage` - 60 edges
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

## Communities (223 total, 12 thin omitted)

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
Nodes (34): Jwt, ModelBuilder, Standings, TeamEquipment, ScoreHubDbContext, Channels, DeviceTokens, EventLineups (+26 more)

### Community 6 - ".AddAppServices"
Cohesion: 0.08
Nodes (23): SportsDTO, Id, Name, HttpClient, EventDetailsApiService, IEventDetailsApiService, CancellationToken, List (+15 more)

### Community 7 - "MAUI.DTO.Input"
Cohesion: 0.09
Nodes (9): MAUI.DTO.Input, MAUI.ViewModel, MAUI.Views, MAUI.DTO.Output, MAUI.Services, MAUI.Services.Interfaces, MAUI.Helpers, Icons (+1 more)

### Community 8 - "IAuthService"
Cohesion: 0.17
Nodes (9): DelegatingHandler, HttpResponseMessage, CancellationToken, Task, RefreshTokenHelper, Task, AuthService, Task (+1 more)

### Community 9 - "SportsDbClient"
Cohesion: 0.17
Nodes (9): CancellationToken, DateTime, Dictionary, HttpClient, List, Task, TimeSpan, SportsDbClient (+1 more)

### Community 10 - "IAuthApiService"
Cohesion: 0.07
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

### Community 17 - "User"
Cohesion: 0.11
Nodes (14): WebAPI.Services, WebAPI.JWT, WebAPI.Services.Interfaces, EntityTypeBuilder, UserConfiguration, User, JwtOptions, Audience (+6 more)

### Community 18 - ".GetRecentSearches"
Cohesion: 0.11
Nodes (20): ActionResult, CancellationToken, HttpDelete, HttpGet, HttpPost, IActionResult, List, RecentSearchDTO (+12 more)

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
Cohesion: 0.22
Nodes (6): HttpPut, IActionResult, SavePredictionDTO, PredictedAwayScore, PredictedHomeScore, DateTime

### Community 24 - "Log"
Cohesion: 0.14
Nodes (13): Log, Exception, HttpMethod, HttpUrl, InnerException, StackTrace, UserId, HttpContext (+5 more)

### Community 25 - "SportsDbTeam"
Cohesion: 0.17
Nodes (12): List, SportsDbTeam, IdLeague, IdTeam, StrBadge, StrCountry, StrSport, StrStadium (+4 more)

### Community 26 - "PredictionOutcome"
Cohesion: 0.18
Nodes (10): HashSet, PredictionOutcome, Close, Exact, Missed, Pending, Postponed, Void (+2 more)

### Community 27 - "MainActivity"
Cohesion: 0.40
Nodes (3): Intent, MainActivity, MauiAppCompatActivity

### Community 28 - "User"
Cohesion: 0.07
Nodes (27): BaseEntity, DeviceToken, Token, UserId, DateTime, SyncedEntity, ExternalId, LastSyncedAt (+19 more)

### Community 29 - "SportsDbPlayerHonour"
Cohesion: 0.18
Nodes (10): SportsDbPlayerHonour, Id, IdHonour, IdLeague, IdPlayer, IdTeam, StrHonour, StrHonourLogo (+2 more)

### Community 30 - ".Login"
Cohesion: 0.09
Nodes (24): ActionResult, Authorize, CancellationToken, HttpPost, Task, AuthController, LoginRequestDTO, Password (+16 more)

### Community 31 - "ContentPage"
Cohesion: 0.12
Nodes (21): Email, FirstName, LastName, RegisterCommand, RepeatPassword, RegisterViewModel, Email, ErrorMessage (+13 more)

### Community 32 - "ContentPage"
Cohesion: 0.03
Nodes (79): BarColor, CanCollapseFriends, CanShowMoreFriends, CloseText, CollapseFriendsCommand, Color, Detail, DonutDrawable (+71 more)

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
Cohesion: 0.12
Nodes (16): PredictionStatsDTO, BestStreak, Close, CurrentStreak, Exact, Missed, Pending, Points (+8 more)

### Community 40 - "EventTimeline"
Cohesion: 0.10
Nodes (18): EntityTypeBuilder, EventTimelineConfiguration, EventTimeline, APIId, Assistant, AssistantId, Comment, Detail (+10 more)

### Community 41 - "Infrastructure.Integrations.DTO"
Cohesion: 0.05
Nodes (29): Infrastructure.Integrations.DTO, List, SportsDbLeague, IdLeague, StrBadge, StrCountry, StrCurrentSeason, StrLeague (+21 more)

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

### Community 46 - "UserProfileDTO"
Cohesion: 0.15
Nodes (12): DateTime, UserProfileDTO, CreatedAt, FirstName, LastName, Username, CancellationToken, Task (+4 more)

### Community 47 - "PredictionStatsDTO"
Cohesion: 0.12
Nodes (16): PredictionStatsDTO, BestStreak, Close, CurrentStreak, Exact, Missed, Pending, Points (+8 more)

### Community 48 - "ContentPage"
Cohesion: 0.03
Nodes (74): AwayLineup, AwayScore, BroadcastRows, CanPredict, DetailedStatusDisplay, Details.AwayTeamBadge, Details.AwayTeamId, Details.AwayTeamName (+66 more)

### Community 50 - ".CreateMauiApp"
Cohesion: 0.22
Nodes (7): Exception, HomeViewModel, IActivationState, MauiApp, Window, CrashApp, MauiProgram

### Community 51 - "ResourceDictionary"
Cohesion: 0.29
Nodes (9): CommonStates, Disabled, Normal, Off, On, PointerOver, ResourceDictionary, VisualState (+1 more)

### Community 52 - "ContentPage"
Cohesion: 0.05
Nodes (36): AcceptCommand, DeclineCommand, HasFullName, IsFriend, IsLocked, IsRequestReceived, LockedText, PrimaryActionCommand (+28 more)

### Community 53 - "UserPageDTO"
Cohesion: 0.14
Nodes (14): DateTime, FriendRequestDTO, RequestedAt, UserId, Username, RelationshipDTO, Relationship, RelationshipState (+6 more)

### Community 54 - "SyncedEntity"
Cohesion: 0.09
Nodes (19): DataAccess.Configurations.BaseConfiguration, EntityTypeBuilder, ChannelConfiguration, EntityTypeBuilder, EventTvConfiguration, ICollection, Channel, Broadcasts (+11 more)

### Community 55 - "SportsDbEventLineup"
Cohesion: 0.13
Nodes (14): SportsDbEventLineup, IdApiFootball, IdEvent, IdLineup, IdPlayer, IdTeam, IntSquadNumber, StrFormation (+6 more)

### Community 56 - "ISearchHistoryApiService"
Cohesion: 0.17
Nodes (12): RecordSearchRequestDTO, EntityId, Kind, CancellationToken, List, Task, ISearchHistoryApiService, CancellationToken (+4 more)

### Community 57 - "EventDetailsDTO"
Cohesion: 0.08
Nodes (26): DateTime, List, EventDetailsDTO, AwayTeamBadge, AwayTeamId, AwayTeamName, AwayTeamScore, Broadcasts (+18 more)

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
Cohesion: 0.11
Nodes (17): EntityTypeBuilder, Player, ICollection, Team, AwayMatches, Country, Equipment, FavoritedBy (+9 more)

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
Cohesion: 0.11
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

### Community 71 - "TeamEquipment"
Cohesion: 0.18
Nodes (9): EntityTypeBuilder, TeamEquipmentConfiguration, TeamEquipment, Equipment, Season, Team, TeamId, Type (+1 more)

### Community 72 - "PredictionDTO"
Cohesion: 0.25
Nodes (5): PredictionDTO, Outcome, Points, PredictedAwayScore, PredictedHomeScore

### Community 73 - "IPredictionApiService"
Cohesion: 0.16
Nodes (14): SavePredictionRequestDTO, PredictedAwayScore, PredictedHomeScore, CancellationToken, List, Task, IPredictionApiService, CancellationToken (+6 more)

### Community 74 - "ContentPage"
Cohesion: 0.04
Nodes (51): AwayTeamDisplayName, AwayTeamLogo, DateTimeText, Details.Equipment, Details.Matches, Details.NameDisplay, Details.SportName, Details.Squad (+43 more)

### Community 75 - ".CreateWindow"
Cohesion: 0.29
Nodes (4): IActivationState, Window, Shell, AppShell

### Community 76 - "FriendDTO"
Cohesion: 0.20
Nodes (10): DateTime, List, FriendDTO, FriendsSince, FriendsSinceDisplay, UserId, Username, FriendsPageDTO (+2 more)

### Community 77 - "UserSearch"
Cohesion: 0.13
Nodes (13): EntityTypeBuilder, UserSearchConfiguration, DateTime, UserSearch, EntityId, Kind, SearchedAt, User (+5 more)

### Community 78 - "FriendsController"
Cohesion: 0.27
Nodes (13): ActionResult, CancellationToken, FriendRequestDTO, FriendsPageDTO, HttpDelete, HttpGet, HttpPost, List (+5 more)

### Community 79 - "PlayerMilestone"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, PlayerMilestoneConfiguration, DateOnly, PlayerMilestone, DateAchieved, MilestoneId, MilestoneLogoUrl, MilestoneName (+4 more)

### Community 80 - "SportsDbEventTimeline"
Cohesion: 0.12
Nodes (16): SportsDbEventTimeline, IdApiFootball, IdAssist, IdEvent, IdPlayer, IdTeam, IdTimeline, IntTime (+8 more)

### Community 81 - "Friendship"
Cohesion: 0.13
Nodes (13): EntityTypeBuilder, FriendshipConfiguration, DateTime, Friendship, AcceptedAt, Addressee, AddresseeId, Requester (+5 more)

### Community 82 - ".GetTeamDetails"
Cohesion: 0.22
Nodes (12): ActionResult, Authorize, CancellationToken, HttpDelete, HttpGet, HttpPost, IActionResult, List (+4 more)

### Community 83 - "UserViewModel"
Cohesion: 0.07
Nodes (30): List, ObservableCollection, RelayCommand, Task, UserViewModel, AcceptCommand, DeclineCommand, FavoriteTeams (+22 more)

### Community 84 - "PredictionsListModel"
Cohesion: 0.08
Nodes (22): IReadOnlyList, IEnumerable, List, ObservableCollection, RelayCommand, PredictionsListModel, CommonFilterOptions, CommonLegendText (+14 more)

### Community 85 - "RecentSearchDTO"
Cohesion: 0.25
Nodes (7): RecentSearchDTO, DisplayName, EntityId, Id, ImageUrl, Kind, SportName

### Community 86 - "BaseConfiguration"
Cohesion: 0.08
Nodes (16): BaseConfiguration, EntityTypeBuilder, DeviceTokenConfiguration, EntityTypeBuilder, JwtConfiguration, EntityTypeBuilder, LogConfiguration, PredictionConfiguration (+8 more)

### Community 87 - "MainApplication"
Cohesion: 0.22
Nodes (5): Builder, FCMNotification, MauiApp, MainApplication, MauiApplication

### Community 88 - "graphify Knowledge Graph Tool"
Cohesion: 0.50
Nodes (4): GRAPH_REPORT.md Reference, graphify Knowledge Graph Tool, graphify update Command, graphify Wiki Index

### Community 89 - ".AddPushNotifications"
Cohesion: 0.17
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
Cohesion: 0.06
Nodes (34): AccentColor, CardBackground, CommonFilterOptions, CommonLegendText, FinishedPredictions, FriendDisplay, HasAnyPredictions, HasComparison (+26 more)

### Community 98 - "MatchScheduleBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchScheduleBackgroundService

### Community 99 - "WebAPI.DTO.Output"
Cohesion: 0.16
Nodes (10): ControllerBase, WebAPI.Controllers, WebAPI.DTO.Output, WebAPI.DTO.Input, WebAPI.Helpers, EventController, MatchesController, WeatherForecastController (+2 more)

### Community 100 - ".AddSportsDbIntegration"
Cohesion: 0.52
Nodes (3): IServiceCollection, IConfiguration, Extensions

### Community 101 - "UserPageDTO"
Cohesion: 0.14
Nodes (14): DateTime, FriendRequestDTO, RequestedAt, UserId, Username, UserPageDTO, FullName, Id (+6 more)

### Community 102 - "FriendApiService"
Cohesion: 0.19
Nodes (11): HttpMethod, CancellationToken, FriendsPageDTO, HttpClient, List, Task, FriendApiService, CancellationToken (+3 more)

### Community 104 - "ScoreFlashBehavior"
Cohesion: 0.23
Nodes (7): Behavior, Label, EventArgs, PropertyChangedEventArgs, Task, ScoreFlashBehavior, TaskCompletionSource

### Community 105 - "SportScoringRule"
Cohesion: 0.17
Nodes (9): EntityTypeBuilder, SportScoringRuleConfiguration, SportScoringRule, CloseMarginTolerance, ClosePoints, ExactPoints, Sport, SportId (+1 more)

### Community 106 - "AddSportScoringRules"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddSportScoringRules

### Community 107 - "FavoriteTeam"
Cohesion: 0.18
Nodes (8): EntityTypeBuilder, FavoriteTeamConfiguration, FavoriteTeam, Notifications, Team, TeamId, User, UserId

### Community 108 - "IPushNotificationService"
Cohesion: 0.25
Nodes (5): Task, IPushNotificationService, Task, PushNotificationService, PostNotifications

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

### Community 115 - "Sport"
Cohesion: 0.18
Nodes (9): EntityTypeBuilder, Venue, ICollection, Sport, Description, IconUrl, Leagues, Name (+1 more)

### Community 116 - ".ShowAsync"
Cohesion: 0.29
Nodes (4): Task, ConfirmDialog, Task, FriendDialogs

### Community 119 - ".GetSports"
Cohesion: 0.18
Nodes (9): ActionResult, CancellationToken, HttpGet, List, Task, SportsController, NavMenuSportsDTO, Id (+1 more)

### Community 120 - "ScoringRule"
Cohesion: 0.29
Nodes (5): CancellationToken, Task, ScoringRule, ScoringRuleSet, IReadOnlyDictionary

### Community 121 - "FriendDTO"
Cohesion: 0.22
Nodes (9): DateTime, List, FriendDTO, FriendsSince, UserId, Username, FriendsPageDTO, Items (+1 more)

### Community 122 - "ContentView"
Cohesion: 0.22
Nodes (8): ChannelName, FlagImage, HasFlag, HasLogo, HasName, LogoThumbUrl, NoFlag, ContentView

### Community 123 - "SportsDbPlayerMilestone"
Cohesion: 0.22
Nodes (8): SportsDbPlayerMilestone, DateMilestone, Id, IdMilestone, IdPlayer, IdTeam, StrMilestone, StrMilestoneLogo

### Community 124 - "FriendshipHelper"
Cohesion: 0.52
Nodes (3): CancellationToken, Task, FriendshipHelper

### Community 125 - "PredictionDonutDrawable"
Cohesion: 0.29
Nodes (5): MAUI.Views.Drawables, ICanvas, IDrawable, PredictionDonutDrawable, RectF

### Community 126 - "FavoriteTeamDTO"
Cohesion: 0.29
Nodes (6): FavoriteTeamDTO, LogoUrl, ShortName, SportName, TeamId, TeamName

### Community 127 - "UserProfileDTO"
Cohesion: 0.29
Nodes (6): DateTime, UserProfileDTO, CreatedAt, FirstName, LastName, Username

### Community 128 - "ContentView"
Cohesion: 0.40
Nodes (4): IsLoading, ContentView, Root, ContentView

### Community 129 - "SportsDbApiException"
Cohesion: 0.40
Nodes (4): Exception, SportsDbApiException, HttpMethod, Url

### Community 130 - ".BuildTargetModel"
Cohesion: 0.50
Nodes (3): DateOnly, DateTime, ModelBuilder

### Community 136 - "DataAccess.Configurations"
Cohesion: 0.14
Nodes (10): DataAccess.Configurations, Domain.Entities.SyncedEntity, LeagueConfiguration, EntityTypeBuilder, MatchConfiguration, PlayerConfiguration, SportConfiguration, TeamConfiguration (+2 more)

### Community 141 - "LeagueDetailsDTO"
Cohesion: 0.11
Nodes (18): List, LeagueDetailsDTO, Country, FlagImage, HasFlag, HasLogo, LogoUrl, Name (+10 more)

### Community 148 - "MyPredictionDTO"
Cohesion: 0.05
Nodes (38): DateTime, MyPredictionDTO, AccentColor, AwayTeam, AwayTeamScore, CardBackground, Comparison, ComparisonAccent (+30 more)

### Community 185 - "PredictionsController"
Cohesion: 0.25
Nodes (13): IReadOnlyCollection, RankedUser, ActionResult, CancellationToken, HttpGet, LeaderboardDTO, List, MyPredictionDTO (+5 more)

### Community 225 - "PlayerStatistic"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, PlayerStatisticConfiguration, PlayerStatistic, League, LeagueId, Player, PlayerId, Season (+4 more)

### Community 228 - "PlayerDTO"
Cohesion: 0.06
Nodes (34): DateTime, List, PlayerDTO, Age, BirthDate, BirthLocation, Description, FacebookLink (+26 more)

### Community 229 - "WeatherForecast"
Cohesion: 0.15
Nodes (10): WebAPI, WeatherForecast, HttpGet, IEnumerable, DateOnly, WeatherForecast, Date, Summary (+2 more)

### Community 268 - "League"
Cohesion: 0.07
Nodes (23): EntityTypeBuilder, EntityTypeBuilder, EntityTypeBuilder, Team, ICollection, League, Country, CurrentSeason (+15 more)

### Community 272 - "SportMenuItem"
Cohesion: 0.14
Nodes (12): IconFontFamily, FontAwesome, MaterialDesignIcons, SportIcon, FontFamilyName, Dictionary, SportIconMap, SportMenuItem (+4 more)

### Community 279 - "SportsDbEventTv"
Cohesion: 0.22
Nodes (8): SportsDbEventTv, Id, IdChannel, IdEvent, StrChannel, StrCountry, StrLogo, StrTimeStamp

### Community 305 - "EventDetailsDTO"
Cohesion: 0.07
Nodes (27): DateTime, List, EventDetailsDTO, AwayTeamBadge, AwayTeamId, AwayTeamName, AwayTeamScore, Broadcasts (+19 more)

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
Nodes (54): List, EventDetailsViewModel, AllBroadcastRows, AwayLineup, BroadcastRows, CanPredict, DetailedStatusDisplay, Details (+46 more)

### Community 333 - "PlayerHonour"
Cohesion: 0.12
Nodes (14): EntityTypeBuilder, PlayerHonourConfiguration, PlayerHonour, HonourId, HonourLogoUrl, HonourName, HonourTrophyUrl, League (+6 more)

### Community 335 - "FavoriteTeamDTO"
Cohesion: 0.13
Nodes (20): FavoriteTeamDTO, HasShortName, LogoUrl, ShortName, SportName, TeamDisplayName, TeamId, TeamName (+12 more)

### Community 348 - "HomeViewModel"
Cohesion: 0.12
Nodes (14): date, CancellationToken, CancellationTokenSource, DateTime, List, ObservableCollection, RelayCommand, Task (+6 more)

### Community 349 - "PlayerDTO"
Cohesion: 0.08
Nodes (24): DateTime, List, PlayerDTO, BirthDate, BirthLocation, Description, FacebookLink, FirstName (+16 more)

### Community 398 - "TeamMatches"
Cohesion: 0.08
Nodes (26): TeamMatches, AwayTeamDisplayName, AwayTeamLogo, AwayTeamName, AwayTeamScore, AwayTeamShortName, DateTimeText, EventDate (+18 more)

### Community 399 - "TeamViewModel"
Cohesion: 0.08
Nodes (22): Task, TeamViewModel, Details, ErrorMessage, FavoriteButtonIcon, FavoriteButtonIconFontFamily, FavoriteButtonText, HasAnyTabData (+14 more)

### Community 401 - "LeagueSearchResultDTO"
Cohesion: 0.22
Nodes (8): LeagueSearchResultDTO, Id, ImageUrl, Name, TeamSearchResultDTO, Id, ImageUrl, Name

### Community 402 - "ContentView"
Cohesion: 0.09
Nodes (22): AwayTeamLogoUrl, EventId, Events, HasLeagueLogo, HomeTeamLogoUrl, LeagueId, LeagueLogoUrl, LeagueName (+14 more)

### Community 403 - "TeamPage"
Cohesion: 0.25
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
Cohesion: 0.15
Nodes (14): TeamSearchResultDTO, Id, ImageUrl, Name, CancellationToken, List, Task, ITeamApiService (+6 more)

### Community 408 - "EventLineupDTO"
Cohesion: 0.13
Nodes (15): EventLineupDTO, FirstName, Formation, HasImage, HomeGame, ImageUrl, IsSubtitute, LastName (+7 more)

### Community 409 - "ILeagueApiService"
Cohesion: 0.13
Nodes (15): LeagueSearchResultDTO, Id, ImageUrl, Name, CancellationToken, List, Task, ILeagueApiService (+7 more)

### Community 410 - "ContentPage"
Cohesion: 0.09
Nodes (19): ContentPage, Task, ImagePreviewHelper, ContentPage, MessageLabel, NoButton, TitleLabel, EventArgs (+11 more)

### Community 411 - "ContentPage"
Cohesion: 0.12
Nodes (16): IsAnyMenuOpen, IsInitialLoading, SelectedDate, SelectedStatus, EventSelectedCommand, IsInitialLoading, LeagueSelectedCommand, SelectedDate (+8 more)

### Community 412 - "UsersController"
Cohesion: 0.18
Nodes (15): ActionResult, CancellationToken, FavoriteTeamDTO, HttpDelete, HttpGet, HttpPost, IActionResult, List (+7 more)

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
Cohesion: 0.12
Nodes (13): Domain.Predictions, Infrastructure.Sync, Infrastructure.Integrations, Infrastructure.Integrations.SportsDB, DataAccess, Infrastructure, Infrastructure.Integrations.Exceptions, Domain.Entities (+5 more)

### Community 439 - "EventDetailsPage"
Cohesion: 0.33
Nodes (3): EventArgs, Task, EventDetailsPage

### Community 444 - "LeagueViewModel"
Cohesion: 0.14
Nodes (13): ObservableCollection, LeagueViewModel, Details, ErrorMessage, HasStandings, HasTeams, IsBusy, LeagueId (+5 more)

### Community 445 - "IDeviceTokenApiService"
Cohesion: 0.17
Nodes (10): HttpRequestMessage, DeviceTokenDTO, Token, CancellationToken, HttpClient, Task, DeviceTokenApiService, CancellationToken (+2 more)

### Community 447 - "LeagueStandingDTO"
Cohesion: 0.12
Nodes (16): LeagueStandingDTO, Description, Form, GoalDifference, GoalsAgainst, GoalsFor, MatchesDraw, MatchesLost (+8 more)

### Community 450 - "LeagueDetailsDTO"
Cohesion: 0.17
Nodes (12): List, LeagueDetailsDTO, Country, LogoUrl, Name, Standings, Teams, LeagueTeamDTO (+4 more)

### Community 460 - ".GetLeagueDetails"
Cohesion: 0.29
Nodes (8): ActionResult, CancellationToken, HttpGet, LeagueDetailsDTO, LeagueSearchResultDTO, List, Task, LeagueController

### Community 479 - "Migration"
Cohesion: 0.18
Nodes (7): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddEventTv, Migration

### Community 482 - "ProfileViewModel"
Cohesion: 0.03
Nodes (68): IDrawable, ProfileViewModel, AcceptRequestCommand, CanCollapseFriends, CanShowMoreFriends, CloseText, DeclineRequestCommand, DonutDrawable (+60 more)

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

### Community 522 - ".GetEventDetails"
Cohesion: 0.33
Nodes (5): ActionResult, CancellationToken, EventDetailsDTO, HttpGet, Task

### Community 523 - "SportsDbEventStatistic"
Cohesion: 0.22
Nodes (8): SportsDbEventStatistic, IdApiFootball, IdEvent, IdStatistic, IntAway, IntHome, StrEvent, StrStat

### Community 525 - "RenameEventTimelineApiId"
Cohesion: 0.22
Nodes (5): MigrationBuilder, DateOnly, DateTime, ModelBuilder, RenameEventTimelineApiId

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

### Community 532 - "DataAccess.Migrations"
Cohesion: 0.28
Nodes (5): DataAccess.Migrations, DateOnly, DateTime, MigrationBuilder, Baseline_PreEventTv

### Community 533 - "AddUserSearch"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddUserSearch

### Community 534 - "AddMatchTvSyncedAt"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddMatchTvSyncedAt

### Community 537 - "PredictionDTO"
Cohesion: 0.33
Nodes (5): PredictionDTO, Outcome, Points, PredictedAwayScore, PredictedHomeScore

## Knowledge Gaps
- **1908 isolated node(s):** `Class1`, `net10.0`, `Microsoft.EntityFrameworkCore (10.0.9)`, `Microsoft.EntityFrameworkCore.SqlServer (10.0.9)`, `Microsoft.NET.Sdk` (+1903 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 2329 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **12 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ScoreHubDbContext` connect `ScoreHubDbContext` to `BackgroundService`, `PreMatchLineupBackgroundService`, `League`, `Match`, `User`, `.GetRecentSearches`, `PlayerFormerTeam`, `Log`, `User`, `UsersController`, `.Login`, `PlayerSyncBackgroundService`, `MatchNotificationService`, `SportsDbStartupSyncService`, `EventTimeline`, `.GetPlayerDetails`, `EventStatistic`, `SyncedEntity`, `MatchStatisticsTrackerService`, `PredictionsController`, `EventLineup`, `Team`, `SportsDbSyncService`, `Prediction`, `PlayerFormerTeamBackgroundService`, `.GetLeagueDetails`, `PlayerHonour`, `UserSearch`, `PlayerMilestone`, `FriendsController`, `Friendship`, `.GetTeamDetails`, `.AddPushNotifications`, `PlayerStatistic`, `MatchScheduleBackgroundService`, `WebAPI.DTO.Output`, `.AddSportsDbIntegration`, `EventTimelineBackgroundService`, `SportScoringRule`, `FavoriteTeam`, `Sport`, `.GetSports`, `ScoringRule`, `EventTvBackgroundService`, `FriendshipHelper`?**
  _High betweenness centrality (0.099) - this node is a cross-community bridge._
- **Why does `MAUI.DTO.Input` connect `MAUI.DTO.Input` to `EventBroadcastDTO`, `.AddAppServices`, `IAuthApiService`, `LeagueDetailsDTO`, `MyPredictionDTO`, `TeamDTO`, `ILeagueApiService`, `MatchesDTO`, `MAUI.Views.Partials`, `UserProfileDTO`, `UserPageDTO`, `IDeviceTokenApiService`, `LeaderboardRowDTO`, `PredictionDTO`, `FriendDTO`, `FavoriteTeamDTO`, `RecentSearchDTO`, `PlayerMilestones`, `PredictionDonutDrawable`?**
  _High betweenness centrality (0.071) - this node is a cross-community bridge._
- **Why does `ProfileViewModel` connect `ProfileViewModel` to `ContentPage`, `LoginViewModel`, `NavMenuViewModelBase`, `FriendApiService`, `MAUI.DTO.Input`, `PredictionStatsDTO`, `IPredictionApiService`, `ProfilePage`, `FriendDTO`, `UserProfileDTO`, `FavoriteTeamDTO`, `.Fill`, `PredictionsListModel`, `UserPageDTO`, `ITeamApiService`, `Task`, `LeaderboardRowDTO`?**
  _High betweenness centrality (0.059) - this node is a cross-community bridge._
- **Are the 31 inferred relationships involving `ProfileViewModel` (e.g. with `AcceptRequestCommand` and `CollapseFriendsCommand`) actually correct?**
  _`ProfileViewModel` has 31 INFERRED edges - model-reasoned connections that need verification._
- **Are the 12 inferred relationships involving `ContentPage` (e.g. with `EventDetailsViewModel` and `ErrorMessage`) actually correct?**
  _`ContentPage` has 12 INFERRED edges - model-reasoned connections that need verification._
- **Are the 14 inferred relationships involving `ContentPage` (e.g. with `ProfileViewModel` and `CollapseFriendsCommand`) actually correct?**
  _`ContentPage` has 14 INFERRED edges - model-reasoned connections that need verification._
- **Are the 18 inferred relationships involving `EventDetailsViewModel` (e.g. with `Details` and `ErrorMessage`) actually correct?**
  _`EventDetailsViewModel` has 18 INFERRED edges - model-reasoned connections that need verification._