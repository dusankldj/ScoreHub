# Graph Report - project  (2026-10-31)

## Corpus Check
- 359 files · ~477,514 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 4523 nodes · 7536 edges · 249 communities (243 shown, 6 thin omitted)
- Extraction: 91% EXTRACTED · 9% INFERRED · 0% AMBIGUOUS · INFERRED: 672 edges (avg confidence: 0.71)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `8a70b7a6`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- EventBroadcastDTO
- Player
- SportsDbSyncService
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
- AuthApiService
- .RecordSearch
- PlayerFormerTeam
- Output/EventDetailsDTO.cs
- http
- TeamDTO
- SavePredictionDTO
- Log
- SportsDbTeam
- PredictionOutcome
- MainActivity
- User
- SportsDbPlayerHonour
- AuthController
- ContentPage
- ContentPage
- TeamMatches
- MatchNotificationService
- LeaderboardEntryDTO
- SportsDbStartupSyncService
- NavMenuViewModelBase
- SportsDbPlayerMilestone
- PredictionStatsDTO
- EventTimeline
- Infrastructure.Integrations.DTO
- SportsDbPlayerFormerTeam
- EventTimelineDTO
- MAUI
- ForgotPasswordViewModel
- UserProfileDTO
- PredictionStatsDTO
- ContentPage
- MAUI.ViewModel
- .CreateMauiApp
- ResourceDictionary
- ContentPage
- UserPageDTO
- SyncedEntity
- SportsDbEventLineup
- IPushNotificationService
- EventDetailsDTO
- SportsDbSport
- Squad
- EventLineup
- IPredictionApiService
- AppDelegate
- LeaderboardRowDTO
- Domain.Entities
- AddUserIsDeleted
- SportScoringDTO
- SportsDbLeague
- ScoreHub App Icon Foreground (Trophy Mark)
- ContentPage
- App
- TeamEquipment
- .GetRules
- PredictionApiService
- ContentPage
- .CreateWindow
- FriendDTO
- ControllerBase
- FriendsController
- PlayerMilestone
- SportsDbEventTimeline
- .For
- .GetTeamDetails
- UserViewModel
- PredictionsListModel
- ScoreFlashBehavior
- LeaderboardEntryDTO
- MainApplication
- graphify Knowledge Graph Tool
- ImagePreviewPage
- App
- AddFriendship
- Domain/Class1.cs
- LoadMauiAsset Method
- Colors.xaml
- PlayerStatistics
- PlayerSearchResultDTO
- ContentView
- MatchScheduleBackgroundService
- SportsDbEventStatistic
- SettingsViewModel
- .ChangePasswordAsync
- FriendApiService
- IVerificationCodeService.cs
- ContentPage
- Venue
- AddSportScoringRules
- ISportsDbClient
- DataAccess.Configurations
- AddSportScoringRuleMaxScore
- .Fill
- EditProfileViewModel
- SyncedEntityConfiguration
- League
- LoginViewModel
- Team
- RelayCommand
- UserSearch
- TeamSearchResultDTO
- .GetSports
- UserPageDTO
- SportMenuItem
- ContentView
- ScoringRule
- .ConfirmEmailChange
- EmailVerificationCode
- SearchResultsBorder
- .GetRulesAsync
- ContentView
- ResetPasswordRequestDTO
- WebAPI.DTO.Output
- Prediction
- EmailOptions
- FavoriteTeam
- Sport
- CodeSentDTO
- ResetPasswordRequestDTO
- FriendDTO
- ScoringRulesViewModel
- JwtOptions
- SportsDbVenue
- LeagueDetailsDTO
- Friendship
- ChangePasswordRequestDTO
- RegisterRequestDTO
- FavoriteTeamDTO
- UserProfileDTO
- IUserCacheService
- MyPredictionDTO
- SportScoringRule
- AddEmailVerificationCodes
- .OnLogoTapped
- LoginRequestDTO
- RefreshRequestDTO
- SportsDbTeamEquipment
- EmailTemplates
- ScoringExampleDTO
- .ShowAsync
- PasswordAttemptLimiter
- FriendshipHelper
- .AddSportsDbIntegration
- RequestThrottle
- IAccountApiService
- WebAPI/DTO/Input/AccountDTOs.cs
- RegisterRequestDTO
- PredictionsController
- PlayerStatistic
- PlayerDTO
- MAUI.Helpers
- SportsDbEventTv
- EventDetailsDTO
- EventsDTO
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
- PlayerSearchResultDTO
- TeamDTO
- MatchesList
- ITeamApiService
- EventLineupDTO
- LeagueSearchResultDTO
- ConfirmDialogPage
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
- Extensions.cs
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
- BackgroundService
- EventTvBackgroundService
- PlayerMilestones
- MyPredictionDTO
- .SendAsync
- .GetEventDetailsAsync
- MatchNotificationBackgroundService
- ContentView
- AddEventTimeline
- PreMatchLineupBackgroundService
- .GetEventDetails
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

## God Nodes (most connected - your core abstractions)
1. `ProfileViewModel` - 125 edges
2. `ScoreHubDbContext` - 102 edges
3. `ContentPage` - 98 edges
4. `ContentPage` - 96 edges
5. `EventDetailsViewModel` - 86 edges
6. `Domain.Entities` - 84 edges
7. `ContentPage` - 76 edges
8. `MAUI.DTO.Input` - 70 edges
9. `ContentPage` - 60 edges
10. `SportsDbSyncService` - 55 edges

## Surprising Connections (you probably didn't know these)
- `ScoreHubDbContext` --references--> `Channel`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/Channel.cs
- `ScoreHubDbContext` --references--> `DeviceToken`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/DeviceToken.cs
- `ScoreHubDbContext` --references--> `User`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/DeviceToken.cs
- `ScoreHubDbContext` --references--> `EmailVerificationCode`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EmailVerificationCode.cs
- `ScoreHubDbContext` --references--> `EventLineup`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventLineup.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **ScoreHub App Icon & Splash Composition** — maui_resources_appicon_appicon_background, maui_resources_appicon_appiconfg_trophy_icon, maui_resources_splash_splash_trophy_icon [INFERRED 0.85]
- **ScoreHub Logo Size Variant Set** — maui_resources_images_scorehub_160x160px_logo, maui_resources_images_scorehub_160x90px_logo, maui_resources_images_scorehub_160x90px_v2_logo, maui_resources_images_scorehub_64x36px_logo, maui_resources_images_scorehub_64x64px_logo [INFERRED 0.85]

## Communities (249 total, 6 thin omitted)

### Community 0 - "EventBroadcastDTO"
Cohesion: 0.14
Nodes (14): EventBroadcastDTO, ChannelName, Country, FlagImage, HasFlag, HasLogo, HasName, LogoThumbUrl (+6 more)

### Community 1 - "Player"
Cohesion: 0.06
Nodes (32): DateOnly, DateTime, ICollection, Player, BirthDate, BirthLocation, DateDied, Description (+24 more)

### Community 2 - "SportsDbSyncService"
Cohesion: 0.06
Nodes (35): StatusCode, FirstName, Func, EventTvSyncMode, All, Repair, Unsynced, CancellationToken (+27 more)

### Community 3 - "NavMenu"
Cohesion: 0.08
Nodes (22): BindableObject, BindableProperty, BindablePropertyKey, ICommand, ObservableCollection, NavMenu, BarHeight, ClearRecentsCommand (+14 more)

### Community 4 - "Infrastructure.csproj"
Cohesion: 0.06
Nodes (35): net10.0, Microsoft.EntityFrameworkCore (10.0.9), Microsoft.NET.Sdk, Domain, net10.0, Microsoft.NET.Sdk, net10.0-android, net10.0 (+27 more)

### Community 5 - "ScoreHubDbContext"
Cohesion: 0.06
Nodes (35): Jwt, ModelBuilder, Standings, TeamEquipment, ScoreHubDbContext, Channels, DeviceTokens, EmailVerificationCodes (+27 more)

### Community 6 - ".AddAppServices"
Cohesion: 0.09
Nodes (23): SportsDTO, Id, Name, HttpClient, EventDetailsApiService, IEventDetailsApiService, CancellationToken, List (+15 more)

### Community 7 - "MAUI.DTO.Input"
Cohesion: 0.12
Nodes (5): MAUI.DTO.Input, MAUI.DTO.Output, MAUI.Services, MAUI.Views.Drawables, MAUI.Services.Interfaces

### Community 8 - "IAuthService"
Cohesion: 0.18
Nodes (9): DelegatingHandler, CancellationToken, HttpResponseMessage, Task, RefreshTokenHelper, Task, AuthService, Task (+1 more)

### Community 9 - "SportsDbClient"
Cohesion: 0.17
Nodes (9): CancellationToken, DateTime, Dictionary, HttpClient, List, Task, TimeSpan, SportsDbClient (+1 more)

### Community 10 - "IAuthApiService"
Cohesion: 0.14
Nodes (11): MAUI.Messages, LoginRequestDTO, Password, Username, RefreshRequestDTO, RefreshToken, Token, PasswordResetCompletedMessage (+3 more)

### Community 11 - "EventLineupDTO"
Cohesion: 0.17
Nodes (12): EventLineupDTO, FirstName, Formation, HomeGame, ImageUrl, IsSubtitute, LastName, PlayerId (+4 more)

### Community 12 - "Match"
Cohesion: 0.07
Nodes (28): DateTime, ICollection, Match, AwayTeam, AwayTeamId, AwayTeamScore, DetailsSyncedAt, EventDate (+20 more)

### Community 13 - "ContentPage"
Cohesion: 0.03
Nodes (67): Appearences, DateAchieved, Details.Age, Details.BirthDate, Details.BirthLocation, Details.Description, Details.FacebookLink, Details.FormerTeams (+59 more)

### Community 14 - "SportsDbMatch"
Cohesion: 0.09
Nodes (22): SportsDbMatch, DateEvent, IdAwayTeam, IdEvent, IdHomeTeam, IdLeague, IdVenue, IntAwayScore (+14 more)

### Community 15 - "ContentPage"
Cohesion: 0.05
Nodes (36): ColorHex, Description, DescriptionColorHex, Details.HasLogo, Details.LogoUrl, Details.Name, Details.NoLogo, Details.Standings (+28 more)

### Community 16 - "SportsDbPlayer"
Cohesion: 0.10
Nodes (21): SportsDbPlayer, DateBorn, DateDied, IdPlayer, IdTeam, IdTeam2, StrBirthLocation, StrDescriptionEN (+13 more)

### Community 17 - "AuthApiService"
Cohesion: 0.19
Nodes (12): AuthResponseDTO, RefreshToken, Token, CancellationToken, HttpClient, HttpResponseMessage, List, ServerErrors (+4 more)

### Community 18 - ".RecordSearch"
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

### Community 23 - "SavePredictionDTO"
Cohesion: 0.50
Nodes (3): SavePredictionDTO, PredictedAwayScore, PredictedHomeScore

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
Cohesion: 0.05
Nodes (38): BaseEntity, EntityTypeBuilder, EntityTypeBuilder, UserConfiguration, DeviceToken, Token, User, UserId (+30 more)

### Community 29 - "SportsDbPlayerHonour"
Cohesion: 0.18
Nodes (10): SportsDbPlayerHonour, Id, IdHonour, IdLeague, IdPlayer, IdTeam, StrHonour, StrHonourLogo (+2 more)

### Community 30 - "AuthController"
Cohesion: 0.14
Nodes (19): ActionResult, Authorize, CancellationToken, Html, HttpPost, ILogger, IServiceScopeFactory, Subject (+11 more)

### Community 31 - "ContentPage"
Cohesion: 0.13
Nodes (20): FirstName, LastName, RegisterCommand, RegisterViewModel, Email, ErrorMessage, FirstName, IsBusy (+12 more)

### Community 32 - "ContentPage"
Cohesion: 0.03
Nodes (79): BarColor, CanCollapseFriends, CanShowMoreFriends, CloseText, CollapseFriendsCommand, Color, Detail, DonutDrawable (+71 more)

### Community 33 - "TeamMatches"
Cohesion: 0.13
Nodes (15): TeamMatches, AwayTeamLogo, AwayTeamName, AwayTeamScore, AwayTeamShortName, EventDate, EventId, HomeTeamLogo (+7 more)

### Community 34 - "MatchNotificationService"
Cohesion: 0.30
Nodes (9): AwayOnlyUserIds, HomeUserIds, CancellationToken, HashSet, IDictionary, List, Task, MatchNotificationService (+1 more)

### Community 35 - "LeaderboardEntryDTO"
Cohesion: 0.15
Nodes (13): List, LeaderboardDTO, Entries, Me, RankedUsers, LeaderboardEntryDTO, Exact, IsMe (+5 more)

### Community 36 - "SportsDbStartupSyncService"
Cohesion: 0.30
Nodes (7): CancellationToken, DateTime, ILogger, IServiceScopeFactory, Task, TimeSpan, SportsDbStartupSyncService

### Community 37 - "NavMenuViewModelBase"
Cohesion: 0.05
Nodes (44): RecentSearchDTO, DisplayName, EntityId, Id, ImageUrl, Kind, SportName, RecordSearchRequestDTO (+36 more)

### Community 38 - "SportsDbPlayerMilestone"
Cohesion: 0.22
Nodes (8): SportsDbPlayerMilestone, DateMilestone, Id, IdMilestone, IdPlayer, IdTeam, StrMilestone, StrMilestoneLogo

### Community 39 - "PredictionStatsDTO"
Cohesion: 0.12
Nodes (16): PredictionStatsDTO, BestStreak, Close, CurrentStreak, Exact, Missed, Pending, Points (+8 more)

### Community 40 - "EventTimeline"
Cohesion: 0.10
Nodes (18): EntityTypeBuilder, EventTimelineConfiguration, EventTimeline, APIId, Assistant, AssistantId, Comment, Detail (+10 more)

### Community 41 - "Infrastructure.Integrations.DTO"
Cohesion: 0.10
Nodes (13): Infrastructure.Integrations.DTO, SportsDbLiveScore, IdEvent, StrProgress, StrStatus, SportsDbPlayerStatistic, Id, IdLeague (+5 more)

### Community 42 - "SportsDbPlayerFormerTeam"
Cohesion: 0.18
Nodes (10): SportsDbPlayerFormerTeam, Id, IdFormerTeam, IdPlayer, IntAppearances, IntGoals, StrBadge, StrDeparted (+2 more)

### Community 43 - "EventTimelineDTO"
Cohesion: 0.06
Nodes (32): FlexJustify, EventTimelineDTO, AssistDisplay, AssistIconText, AssistName, AssistOpen, CardColor, Comment (+24 more)

### Community 44 - "MAUI"
Cohesion: 0.29
Nodes (3): MAUI, Program, Program

### Community 45 - "ForgotPasswordViewModel"
Cohesion: 0.05
Nodes (46): IsEmailStep, IsPasswordStep, ResetPasswordCommand, SendCodeCommand, StartOverCommand, ForgotPasswordViewModel, CanResend, Code (+38 more)

### Community 46 - "UserProfileDTO"
Cohesion: 0.15
Nodes (11): DateTime, UserProfileDTO, CreatedAt, Email, FirstName, LastName, Username, CancellationToken (+3 more)

### Community 47 - "PredictionStatsDTO"
Cohesion: 0.12
Nodes (16): PredictionStatsDTO, BestStreak, Close, CurrentStreak, Exact, Missed, Pending, Points (+8 more)

### Community 48 - "ContentPage"
Cohesion: 0.03
Nodes (77): AwayLineup, AwayScore, BroadcastRows, CanPredict, DetailedStatusDisplay, Details.AwayTeamBadge, Details.AwayTeamId, Details.AwayTeamName (+69 more)

### Community 49 - "MAUI.ViewModel"
Cohesion: 0.08
Nodes (15): ContentPage, MAUI.ViewModel, MAUI.Views, IConfiguration, MauiExtensions, PlayerPageViewModel, ChangePasswordPage, EditProfilePage (+7 more)

### Community 50 - ".CreateMauiApp"
Cohesion: 0.22
Nodes (7): Exception, HomeViewModel, IActivationState, MauiApp, Window, CrashApp, MauiProgram

### Community 51 - "ResourceDictionary"
Cohesion: 0.29
Nodes (9): CommonStates, Disabled, Normal, Off, On, PointerOver, ResourceDictionary, VisualState (+1 more)

### Community 52 - "ContentPage"
Cohesion: 0.05
Nodes (35): AcceptCommand, DeclineCommand, HasFullName, IsFriend, IsLocked, IsRequestReceived, LockedText, PrimaryActionCommand (+27 more)

### Community 53 - "UserPageDTO"
Cohesion: 0.14
Nodes (14): DateTime, FriendRequestDTO, RequestedAt, UserId, Username, RelationshipDTO, Relationship, RelationshipState (+6 more)

### Community 54 - "SyncedEntity"
Cohesion: 0.09
Nodes (19): DataAccess.Configurations.BaseConfiguration, EntityTypeBuilder, ChannelConfiguration, EntityTypeBuilder, EventTvConfiguration, ICollection, Channel, Broadcasts (+11 more)

### Community 55 - "SportsDbEventLineup"
Cohesion: 0.14
Nodes (14): SportsDbEventLineup, IdApiFootball, IdEvent, IdLineup, IdPlayer, IdTeam, IntSquadNumber, StrFormation (+6 more)

### Community 56 - "IPushNotificationService"
Cohesion: 0.19
Nodes (7): Task, IPushNotificationService, Task, PushNotificationService, RelayCommand, Task, PostNotifications

### Community 57 - "EventDetailsDTO"
Cohesion: 0.07
Nodes (28): DateTime, List, EventDetailsDTO, AwayTeamBadge, AwayTeamId, AwayTeamName, AwayTeamScore, Broadcasts (+20 more)

### Community 58 - "SportsDbSport"
Cohesion: 0.25
Nodes (8): List, SportsDbSport, IdSport, StrSport, StrSportDescription, StrSportThumb, SportsDbSportResponse, Sports

### Community 59 - "Squad"
Cohesion: 0.25
Nodes (8): DateTime, Squad, BirthDate, FirstName, ImageUrl, LastName, PlayerId, Position

### Community 60 - "EventLineup"
Cohesion: 0.12
Nodes (15): EntityTypeBuilder, EventLineupConfiguration, EventLineup, ApiId, Event, EventId, Formation, HomeGame (+7 more)

### Community 61 - "IPredictionApiService"
Cohesion: 0.19
Nodes (9): PredictionDTO, Outcome, Points, PredictedAwayScore, PredictedHomeScore, CancellationToken, List, Task (+1 more)

### Community 62 - "AppDelegate"
Cohesion: 0.22
Nodes (5): MauiApp, AppDelegate, MauiApp, AppDelegate, MauiUIApplicationDelegate

### Community 63 - "LeaderboardRowDTO"
Cohesion: 0.10
Nodes (20): FontAttributes, FormDotDTO, Color, Letter, LeaderboardRowDTO, BarColor, Detail, Exact (+12 more)

### Community 64 - "Domain.Entities"
Cohesion: 0.17
Nodes (6): Infrastructure.Sync, DataAccess, Domain.Entities, WebAPI.Middleware, Infrastructure.Push, Class1

### Community 65 - "AddUserIsDeleted"
Cohesion: 0.22
Nodes (5): MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddUserIsDeleted

### Community 66 - "SportScoringDTO"
Cohesion: 0.09
Nodes (21): List, SportScoringDTO, CardBorder, CardBorderWidth, CloseDescription, CloseLine, ClosePoints, ExactLine (+13 more)

### Community 67 - "SportsDbLeague"
Cohesion: 0.20
Nodes (10): List, SportsDbLeague, IdLeague, StrBadge, StrCountry, StrCurrentSeason, StrLeague, StrSport (+2 more)

### Community 68 - "ScoreHub App Icon Foreground (Trophy Mark)"
Cohesion: 0.25
Nodes (8): ScoreHub App Icon Background, ScoreHub App Icon Foreground (Trophy Mark), ScoreHub Logo 160x160, ScoreHub Logo 160x90, ScoreHub Logo 160x90 v2 (Blank Variant), ScoreHub Logo 64x36, ScoreHub Logo 64x64, ScoreHub Splash Screen

### Community 69 - "ContentPage"
Cohesion: 0.11
Nodes (18): CardBorder, CardBorderWidth, CloseLine, ExactLine, ExampleHeader, Examples, HasClose, HasMaxScore (+10 more)

### Community 70 - "App"
Cohesion: 0.29
Nodes (4): MAUI.WinUI, MauiWinUIApplication, MauiApp, App

### Community 71 - "TeamEquipment"
Cohesion: 0.18
Nodes (9): EntityTypeBuilder, TeamEquipmentConfiguration, TeamEquipment, Equipment, Season, Team, TeamId, Type (+1 more)

### Community 72 - ".GetRules"
Cohesion: 0.09
Nodes (21): ActionResult, CancellationToken, HttpGet, List, SportScoringDTO, Task, List, ScoringExampleDTO (+13 more)

### Community 73 - "PredictionApiService"
Cohesion: 0.18
Nodes (10): SavePredictionRequestDTO, PredictedAwayScore, PredictedHomeScore, CancellationToken, HttpClient, LeaderboardDTO, List, PredictionStatsDTO (+2 more)

### Community 74 - "ContentPage"
Cohesion: 0.04
Nodes (51): AwayTeamDisplayName, AwayTeamLogo, DateTimeText, Details.Equipment, Details.Matches, Details.NameDisplay, Details.SportName, Details.Squad (+43 more)

### Community 75 - ".CreateWindow"
Cohesion: 0.29
Nodes (4): IActivationState, Window, Shell, AppShell

### Community 76 - "FriendDTO"
Cohesion: 0.20
Nodes (10): DateTime, List, FriendDTO, FriendsSince, FriendsSinceDisplay, UserId, Username, FriendsPageDTO (+2 more)

### Community 77 - "ControllerBase"
Cohesion: 0.12
Nodes (13): ControllerBase, WebAPI, WeatherForecast, ScoringRulesController, HttpGet, IEnumerable, WeatherForecastController, DateOnly (+5 more)

### Community 78 - "FriendsController"
Cohesion: 0.27
Nodes (13): ActionResult, CancellationToken, FriendRequestDTO, FriendsPageDTO, HttpDelete, HttpGet, HttpPost, List (+5 more)

### Community 79 - "PlayerMilestone"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, PlayerMilestoneConfiguration, DateOnly, PlayerMilestone, DateAchieved, MilestoneId, MilestoneLogoUrl, MilestoneName (+4 more)

### Community 80 - "SportsDbEventTimeline"
Cohesion: 0.12
Nodes (16): SportsDbEventTimeline, IdApiFootball, IdAssist, IdEvent, IdPlayer, IdTeam, IdTimeline, IntTime (+8 more)

### Community 81 - ".For"
Cohesion: 0.33
Nodes (4): Away, Home, Results, ScoringExamples

### Community 82 - ".GetTeamDetails"
Cohesion: 0.22
Nodes (12): ActionResult, Authorize, CancellationToken, HttpDelete, HttpGet, HttpPost, IActionResult, List (+4 more)

### Community 83 - "UserViewModel"
Cohesion: 0.07
Nodes (30): List, ObservableCollection, RelayCommand, Task, UserViewModel, AcceptCommand, FavoriteTeams, FavoriteTeamSelectedCommand (+22 more)

### Community 84 - "PredictionsListModel"
Cohesion: 0.08
Nodes (22): IReadOnlyList, IEnumerable, List, ObservableCollection, RelayCommand, PredictionsListModel, CommonFilterOptions, CommonLegendText (+14 more)

### Community 85 - "ScoreFlashBehavior"
Cohesion: 0.23
Nodes (7): Behavior, Label, EventArgs, PropertyChangedEventArgs, Task, ScoreFlashBehavior, TaskCompletionSource

### Community 86 - "LeaderboardEntryDTO"
Cohesion: 0.15
Nodes (13): List, LeaderboardDTO, Entries, Me, RankedUsers, LeaderboardEntryDTO, Exact, IsMe (+5 more)

### Community 87 - "MainApplication"
Cohesion: 0.22
Nodes (5): Builder, FCMNotification, MauiApp, MainApplication, MauiApplication

### Community 88 - "graphify Knowledge Graph Tool"
Cohesion: 0.50
Nodes (4): GRAPH_REPORT.md Reference, graphify Knowledge Graph Tool, graphify update Command, graphify Wiki Index

### Community 89 - "ImagePreviewPage"
Cohesion: 0.29
Nodes (5): ContentPage, PreviewImage, EventArgs, ImagePreviewPage, Image

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

### Community 99 - "SportsDbEventStatistic"
Cohesion: 0.22
Nodes (8): SportsDbEventStatistic, IdApiFootball, IdEvent, IdStatistic, IntAway, IntHome, StrEvent, StrStat

### Community 100 - "SettingsViewModel"
Cohesion: 0.18
Nodes (11): RelayCommand, Task, SettingsViewModel, DeleteAccountCommand, ErrorMessage, IsBusy, ContentPage, ErrorMessage (+3 more)

### Community 101 - ".ChangePasswordAsync"
Cohesion: 0.25
Nodes (5): ChangePasswordRequestDTO, CurrentPassword, NewPassword, RelayCommand, Task

### Community 102 - "FriendApiService"
Cohesion: 0.19
Nodes (11): HttpMethod, CancellationToken, FriendsPageDTO, HttpClient, List, Task, FriendApiService, CancellationToken (+3 more)

### Community 103 - "IVerificationCodeService.cs"
Cohesion: 0.18
Nodes (12): IssueResult, IssueStatus, LimitReached, SendFailed, Sent, TooSoon, VerifyResult, VerifyStatus (+4 more)

### Community 104 - "ContentPage"
Cohesion: 0.11
Nodes (23): ChangePasswordCommand, RepeatNewPassword, ChangePasswordViewModel, ChangePasswordCommand, CurrentPassword, ErrorMessage, HidePasswords, IsBusy (+15 more)

### Community 105 - "Venue"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, Sport, ICollection, Venue, Country, ImageUrl, Location, Matches (+4 more)

### Community 106 - "AddSportScoringRules"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddSportScoringRules

### Community 107 - "ISportsDbClient"
Cohesion: 0.26
Nodes (4): CancellationToken, List, Task, ISportsDbClient

### Community 108 - "DataAccess.Configurations"
Cohesion: 0.06
Nodes (21): BaseConfiguration, DataAccess.Configurations, Domain.Entities.SyncedEntity, EntityTypeBuilder, DeviceTokenConfiguration, EntityTypeBuilder, EmailVerificationCodeConfiguration, JwtConfiguration (+13 more)

### Community 109 - "AddSportScoringRuleMaxScore"
Cohesion: 0.22
Nodes (5): MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddSportScoringRuleMaxScore

### Community 110 - ".Fill"
Cohesion: 0.11
Nodes (9): ICanvas, IDrawable, CancellationToken, CancellationTokenSource, IEnumerable, List, ObservableCollection, PredictionDonutDrawable (+1 more)

### Community 111 - "EditProfileViewModel"
Cohesion: 0.07
Nodes (39): CancelCodeCommand, IsFormStep, NeedsPassword, SaveCommand, Regex, EditProfileViewModel, CancelCodeCommand, CanResend (+31 more)

### Community 112 - "SyncedEntityConfiguration"
Cohesion: 0.17
Nodes (10): Domain.Entities.BaseEntity, EntityTypeBuilder, BaseConfiguration, SyncedEntityConfiguration, DateTime, BaseEntity, CreatedAt, Id (+2 more)

### Community 113 - "League"
Cohesion: 0.17
Nodes (10): EntityTypeBuilder, Team, ICollection, League, Country, CurrentSeason, LogoUrl, Name (+2 more)

### Community 114 - "LoginViewModel"
Cohesion: 0.14
Nodes (16): InfoMessage, LoginCommand, LoginViewModel, ErrorMessage, InfoMessage, IsBusy, LoginCommand, Password (+8 more)

### Community 115 - "Team"
Cohesion: 0.11
Nodes (17): EntityTypeBuilder, Player, ICollection, Team, AwayMatches, Country, Equipment, FavoritedBy (+9 more)

### Community 116 - "RelayCommand"
Cohesion: 0.23
Nodes (4): CancellationToken, CancellationTokenSource, RelayCommand, Task

### Community 117 - "UserSearch"
Cohesion: 0.13
Nodes (13): EntityTypeBuilder, UserSearchConfiguration, DateTime, UserSearch, EntityId, Kind, SearchedAt, User (+5 more)

### Community 118 - "TeamSearchResultDTO"
Cohesion: 0.40
Nodes (5): TeamSearchResultDTO, Id, ImageUrl, Name, SportName

### Community 119 - ".GetSports"
Cohesion: 0.18
Nodes (9): ActionResult, CancellationToken, HttpGet, List, Task, SportsController, NavMenuSportsDTO, Id (+1 more)

### Community 120 - "UserPageDTO"
Cohesion: 0.14
Nodes (14): DateTime, FriendRequestDTO, RequestedAt, UserId, Username, UserPageDTO, FullName, Id (+6 more)

### Community 121 - "SportMenuItem"
Cohesion: 0.20
Nodes (6): SportMenuItem, IconFontFamily, IconGlyph, Id, Name, SelectionChangedEventArgs

### Community 122 - "ContentView"
Cohesion: 0.22
Nodes (8): ChannelName, FlagImage, HasFlag, HasLogo, HasName, LogoThumbUrl, NoFlag, ContentView

### Community 123 - "ScoringRule"
Cohesion: 0.24
Nodes (7): CancellationToken, Task, ScoringRuleExtensions, ScoringRule, CloseDescription, ScoringRuleSet, IReadOnlyDictionary

### Community 124 - ".ConfirmEmailChange"
Cohesion: 0.11
Nodes (23): ActionResult, CancellationToken, CodeSentDTO, EmailDTO, HttpDelete, HttpPost, HttpPut, IActionResult (+15 more)

### Community 125 - "EmailVerificationCode"
Cohesion: 0.11
Nodes (23): DateTime, EmailVerificationCode, CodeHash, ConsumedAt, ExpiresAt, FailedAttempts, Purpose, TargetEmail (+15 more)

### Community 126 - "SearchResultsBorder"
Cohesion: 0.40
Nodes (5): AccountMenuBorder, OverflowMenuBorder, SearchResultsBorder, IsSearchResultsVisible, Border

### Community 127 - ".GetRulesAsync"
Cohesion: 0.20
Nodes (9): CancellationToken, List, Task, IScoringRulesApiService, CancellationToken, HttpClient, List, Task (+1 more)

### Community 128 - "ContentView"
Cohesion: 0.40
Nodes (4): IsLoading, ContentView, Root, ContentView

### Community 129 - "ResetPasswordRequestDTO"
Cohesion: 0.20
Nodes (9): ForgotPasswordRequestDTO, Email, ResetPasswordRequestDTO, Email, NewPassword, ResetToken, VerifyResetCodeRequestDTO, Code (+1 more)

### Community 130 - "WebAPI.DTO.Output"
Cohesion: 0.16
Nodes (8): WebAPI.Controllers, Domain.Predictions, WebAPI.DTO.Output, WebAPI.DTO.Input, WebAPI.Helpers, Infrastructure.Email, TimeSpan, MatchStateCalculator

### Community 131 - "Prediction"
Cohesion: 0.14
Nodes (11): EntityTypeBuilder, PredictionConfiguration, Prediction, Hit, Match, MatchId, Points, PredictedAwayScore (+3 more)

### Community 132 - "EmailOptions"
Cohesion: 0.12
Nodes (15): EmailOptions, Address, AppPassword, DisplayName, Host, IsConfigured, Port, Username (+7 more)

### Community 133 - "FavoriteTeam"
Cohesion: 0.18
Nodes (8): EntityTypeBuilder, FavoriteTeamConfiguration, FavoriteTeam, Notifications, Team, TeamId, User, UserId

### Community 134 - "Sport"
Cohesion: 0.18
Nodes (9): EntityTypeBuilder, Venue, ICollection, Sport, Description, IconUrl, Leagues, Name (+1 more)

### Community 135 - "CodeSentDTO"
Cohesion: 0.09
Nodes (26): CodeSentDTO, ExpiresInSeconds, ResendAfterSeconds, SentTo, EmailDTO, Email, ResetTokenDTO, ResetToken (+18 more)

### Community 136 - "ResetPasswordRequestDTO"
Cohesion: 0.20
Nodes (9): ForgotPasswordRequestDTO, Email, ResetPasswordRequestDTO, Email, NewPassword, ResetToken, VerifyResetCodeDTO, Code (+1 more)

### Community 137 - "FriendDTO"
Cohesion: 0.22
Nodes (9): DateTime, List, FriendDTO, FriendsSince, UserId, Username, FriendsPageDTO, Items (+1 more)

### Community 138 - "ScoringRulesViewModel"
Cohesion: 0.22
Nodes (7): ObservableCollection, Task, ScoringRulesViewModel, ErrorMessage, IsBusy, SportId, Sports

### Community 139 - "JwtOptions"
Cohesion: 0.22
Nodes (7): JwtOptions, Audience, Issuer, RefreshTokenExpirationHours, SecretKey, TokenExpirationMinutes, JwtService

### Community 140 - "SportsDbVenue"
Cohesion: 0.25
Nodes (7): SportsDbVenue, IdVenue, StrCountry, StrLocation, StrSport, StrThumb, StrVenue

### Community 141 - "LeagueDetailsDTO"
Cohesion: 0.11
Nodes (18): List, LeagueDetailsDTO, Country, FlagImage, HasFlag, HasLogo, LogoUrl, Name (+10 more)

### Community 142 - "Friendship"
Cohesion: 0.13
Nodes (13): EntityTypeBuilder, FriendshipConfiguration, DateTime, Friendship, AcceptedAt, Addressee, AddresseeId, Requester (+5 more)

### Community 143 - "ChangePasswordRequestDTO"
Cohesion: 0.50
Nodes (3): ChangePasswordRequestDTO, CurrentPassword, NewPassword

### Community 144 - "RegisterRequestDTO"
Cohesion: 0.20
Nodes (8): RegisterRequestDTO, Email, FirstName, LastName, Password, Username, RelayCommand, Task

### Community 145 - "FavoriteTeamDTO"
Cohesion: 0.29
Nodes (6): FavoriteTeamDTO, LogoUrl, ShortName, SportName, TeamId, TeamName

### Community 146 - "UserProfileDTO"
Cohesion: 0.25
Nodes (7): DateTime, UserProfileDTO, CreatedAt, Email, FirstName, LastName, Username

### Community 147 - "IUserCacheService"
Cohesion: 0.16
Nodes (6): IUserCacheService, CancellationToken, CancellationTokenSource, CodeSentDTO, RelayCommand, Task

### Community 148 - "MyPredictionDTO"
Cohesion: 0.05
Nodes (38): DateTime, MyPredictionDTO, AccentColor, AwayTeam, AwayTeamScore, CardBackground, Comparison, ComparisonAccent (+30 more)

### Community 149 - "SportScoringRule"
Cohesion: 0.15
Nodes (10): EntityTypeBuilder, SportScoringRuleConfiguration, SportScoringRule, CloseMarginTolerance, ClosePoints, ExactPoints, MaxScore, Sport (+2 more)

### Community 150 - "AddEmailVerificationCodes"
Cohesion: 0.20
Nodes (6): DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, AddEmailVerificationCodes

### Community 152 - "LoginRequestDTO"
Cohesion: 0.50
Nodes (3): LoginRequestDTO, Password, Username

### Community 153 - "RefreshRequestDTO"
Cohesion: 0.50
Nodes (3): RefreshRequestDTO, RefreshToken, Token

### Community 154 - "SportsDbTeamEquipment"
Cohesion: 0.25
Nodes (7): SportsDbTeamEquipment, IdEquipment, IdTeam, StrEquipment, StrSeason, StrType, StrUsername

### Community 155 - "EmailTemplates"
Cohesion: 0.53
Nodes (5): DateTime, Html, Subject, Text, EmailTemplates

### Community 156 - "ScoringExampleDTO"
Cohesion: 0.25
Nodes (7): ScoringExampleDTO, Outcome, OutcomeColor, OutcomeDisplay, Points, Result, ResultDisplay

### Community 157 - ".ShowAsync"
Cohesion: 0.29
Nodes (4): Task, ConfirmDialog, Task, FriendDialogs

### Community 158 - "PasswordAttemptLimiter"
Cohesion: 0.22
Nodes (6): Failures, ConcurrentDictionary, DateTime, TimeSpan, PasswordAttemptLimiter, WindowStart

### Community 159 - "FriendshipHelper"
Cohesion: 0.52
Nodes (3): CancellationToken, Task, FriendshipHelper

### Community 160 - ".AddSportsDbIntegration"
Cohesion: 0.13
Nodes (14): ILoggerFactory, List, SportsOptions, ApiKey, BaseUrl, LeagueExternalIds, SportsIds, FirebaseOptions (+6 more)

### Community 161 - "RequestThrottle"
Cohesion: 0.33
Nodes (5): ConcurrentDictionary, DateTime, List, TimeSpan, RequestThrottle

### Community 163 - "IAccountApiService"
Cohesion: 0.38
Nodes (3): CancellationToken, Task, IAccountApiService

### Community 164 - "WebAPI/DTO/Input/AccountDTOs.cs"
Cohesion: 0.25
Nodes (7): ConfirmEmailCodeDTO, Code, RequestEmailChangeDTO, CurrentPassword, NewEmail, UpdateUsernameRequestDTO, Username

### Community 166 - "RegisterRequestDTO"
Cohesion: 0.29
Nodes (6): RegisterRequestDTO, Email, FirstName, LastName, Password, Username

### Community 185 - "PredictionsController"
Cohesion: 0.18
Nodes (17): NotFound, IReadOnlyCollection, RankedUser, ActionResult, CancellationToken, HttpGet, HttpPut, IActionResult (+9 more)

### Community 225 - "PlayerStatistic"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, PlayerStatisticConfiguration, PlayerStatistic, League, LeagueId, Player, PlayerId, Season (+4 more)

### Community 228 - "PlayerDTO"
Cohesion: 0.06
Nodes (34): DateTime, List, PlayerDTO, Age, BirthDate, BirthLocation, Description, FacebookLink (+26 more)

### Community 272 - "MAUI.Helpers"
Cohesion: 0.09
Nodes (11): MAUI.Helpers, Dictionary, CountryFlagHelper, Icons, IconFontFamily, FontAwesome, MaterialDesignIcons, SportIcon (+3 more)

### Community 279 - "SportsDbEventTv"
Cohesion: 0.22
Nodes (8): SportsDbEventTv, Id, IdChannel, IdEvent, StrChannel, StrCountry, StrLogo, StrTimeStamp

### Community 305 - "EventDetailsDTO"
Cohesion: 0.07
Nodes (29): DateTime, List, EventDetailsDTO, AwayTeamBadge, AwayTeamId, AwayTeamName, AwayTeamScore, Broadcasts (+21 more)

### Community 306 - "EventsDTO"
Cohesion: 0.06
Nodes (36): ActionResult, CancellationToken, HttpGet, List, MatchesDTO, Task, MatchesController, DateTime (+28 more)

### Community 311 - "MatchStatisticsTrackerService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchStatisticsTrackerService

### Community 321 - "PlayerViewModel"
Cohesion: 0.06
Nodes (33): List, RelayCommand, Task, PlayerStatisticsSeasonGroup, Items, Season, PlayerViewModel, Details (+25 more)

### Community 322 - "Standings"
Cohesion: 0.09
Nodes (20): EntityTypeBuilder, StandingsConfiguration, Standings, Description, Form, GoalDifference, GoalsAgainst, GoalsFor (+12 more)

### Community 328 - "PlayerFormerTeamBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PlayerFormerTeamBackgroundService

### Community 329 - "EventDetailsViewModel"
Cohesion: 0.03
Nodes (55): List, EventDetailsViewModel, AllBroadcastRows, AwayLineup, BroadcastRows, CanPredict, DetailedStatusDisplay, Details (+47 more)

### Community 333 - "PlayerHonour"
Cohesion: 0.12
Nodes (14): EntityTypeBuilder, PlayerHonourConfiguration, PlayerHonour, HonourId, HonourLogoUrl, HonourName, HonourTrophyUrl, League (+6 more)

### Community 335 - "FavoriteTeamDTO"
Cohesion: 0.11
Nodes (21): FavoriteTeamDTO, HasShortName, LogoUrl, ShortName, SportName, TeamDisplayName, TeamId, TeamName (+13 more)

### Community 348 - "HomeViewModel"
Cohesion: 0.11
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
Cohesion: 0.18
Nodes (10): LeagueSearchResultDTO, Id, ImageUrl, Name, SportName, TeamSearchResultDTO, Id, ImageUrl (+2 more)

### Community 402 - "ContentView"
Cohesion: 0.09
Nodes (22): AwayTeamLogoUrl, EventId, Events, HasLeagueLogo, HomeTeamLogoUrl, LeagueId, LeagueLogoUrl, LeagueName (+14 more)

### Community 403 - "TeamPage"
Cohesion: 0.25
Nodes (4): EventArgs, PropertyChangedEventArgs, Task, TeamPage

### Community 404 - "PlayerSearchResultDTO"
Cohesion: 0.15
Nodes (15): PlayerSearchResultDTO, FirstName, Id, ImageUrl, LastName, SportName, CancellationToken, List (+7 more)

### Community 405 - "TeamDTO"
Cohesion: 0.11
Nodes (19): List, TeamDTO, Country, Equipment, FlagImage, HasFlag, ImageUrl, Matches (+11 more)

### Community 406 - "MatchesList"
Cohesion: 0.07
Nodes (25): DateTime, DateFilterOption, Date, DisplayText, BindableObject, BindableProperty, BindablePropertyKey, DateTime (+17 more)

### Community 407 - "ITeamApiService"
Cohesion: 0.19
Nodes (10): CancellationToken, List, Task, ITeamApiService, CancellationToken, HttpClient, List, Task (+2 more)

### Community 408 - "EventLineupDTO"
Cohesion: 0.13
Nodes (15): EventLineupDTO, FirstName, Formation, HasImage, HomeGame, ImageUrl, IsSubtitute, LastName (+7 more)

### Community 409 - "LeagueSearchResultDTO"
Cohesion: 0.12
Nodes (16): LeagueSearchResultDTO, Id, ImageUrl, Name, SportName, CancellationToken, List, Task (+8 more)

### Community 410 - "ConfirmDialogPage"
Cohesion: 0.18
Nodes (11): ContentPage, MessageLabel, NoButton, TitleLabel, EventArgs, Task, ConfirmDialogPage, Result (+3 more)

### Community 411 - "ContentPage"
Cohesion: 0.11
Nodes (18): IsAnyMenuOpen, IsInitialLoading, SelectedDate, SelectedStatus, EventSelectedCommand, IsInitialLoading, LeagueSelectedCommand, SelectedDate (+10 more)

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
Nodes (26): ., DisplayName, IconFontFamily, IconGlyph, IsRecent, KindLabel, Name, BarGrid (+18 more)

### Community 420 - "MatchesDTO"
Cohesion: 0.18
Nodes (12): ObservableCollection, LeaguesDTO, Events, HasLeagueLogo, LeagueId, LeagueLogoUrl, LeagueName, NoLeagueLogo (+4 more)

### Community 421 - "EventsDTO"
Cohesion: 0.20
Nodes (10): DateTime, EventsDTO, AwayTeam, AwayTeamLogoUrl, EventDate, EventId, HomeTeam, HomeTeamLogoUrl (+2 more)

### Community 422 - "PlayerHonours"
Cohesion: 0.11
Nodes (18): DateOnly, PlayerHonours, HasLeague, HasTeam, HasTrophyImage, HonourName, HonourTrophyUrl, League (+10 more)

### Community 423 - "PlayerStatistics"
Cohesion: 0.18
Nodes (11): PlayerStatistics, HasLeague, HasTeam, League, Season, StatDisplay, Statistic, SubText (+3 more)

### Community 424 - ".IsLoggedInAsync"
Cohesion: 0.29
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

### Community 438 - "Extensions.cs"
Cohesion: 0.13
Nodes (12): WebAPI.Services, Infrastructure.Integrations, Infrastructure.Integrations.SportsDB, Infrastructure, Infrastructure.Integrations.Exceptions, WebAPI.JWT, WebAPI.Services.Interfaces, Exception (+4 more)

### Community 439 - "EventDetailsPage"
Cohesion: 0.24
Nodes (5): Task, ImagePreviewHelper, EventArgs, Task, EventDetailsPage

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

### Community 504 - "BackgroundService"
Cohesion: 0.33
Nodes (7): BackgroundService, CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, EventTimelineBackgroundService

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
Cohesion: 0.14
Nodes (12): CancellationToken, FirebaseMessaging, IDictionary, IEnumerable, ILogger, Task, FcmPushNotificationService, CancellationToken (+4 more)

### Community 513 - ".GetEventDetailsAsync"
Cohesion: 0.33
Nodes (4): CancellationToken, Task, CancellationToken, Task

### Community 515 - "MatchNotificationBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchNotificationBackgroundService

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
Cohesion: 0.29
Nodes (6): ActionResult, CancellationToken, EventDetailsDTO, HttpGet, Task, EventController

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
Cohesion: 0.18
Nodes (8): DataAccess.Migrations, DateOnly, DateTime, MigrationBuilder, DateOnly, DateTime, ModelBuilder, Baseline_PreEventTv

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
- **2119 isolated node(s):** `Class1`, `net10.0`, `Microsoft.EntityFrameworkCore (10.0.9)`, `Microsoft.EntityFrameworkCore.SqlServer (10.0.9)`, `Microsoft.NET.Sdk` (+2114 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 2620 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **6 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ScoreHubDbContext` connect `ScoreHubDbContext` to `SportsDbSyncService`, `Prediction`, `MatchNotificationBackgroundService`, `FavoriteTeam`, `Sport`, `PreMatchLineupBackgroundService`, `.GetEventDetails`, `Match`, `Friendship`, `.RecordSearch`, `PlayerFormerTeam`, `SportScoringRule`, `Log`, `User`, `UsersController`, `AuthController`, `FriendshipHelper`, `PlayerSyncBackgroundService`, `.AddSportsDbIntegration`, `MatchNotificationService`, `SportsDbStartupSyncService`, `EventTimeline`, `.GetPlayerDetails`, `EventStatistic`, `EventsDTO`, `SyncedEntity`, `MatchStatisticsTrackerService`, `PredictionsController`, `EventLineup`, `Domain.Entities`, `PlayerFormerTeamBackgroundService`, `.GetLeagueDetails`, `PlayerHonour`, `FriendsController`, `PlayerMilestone`, `ControllerBase`, `.GetTeamDetails`, `.GetSports`, `PlayerStatistic`, `MatchScheduleBackgroundService`, `Venue`, `League`, `Team`, `UserSearch`, `EventTvBackgroundService`, `BackgroundService`, `ScoringRule`, `.ConfirmEmailChange`, `EmailVerificationCode`, `.SendAsync`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **Why does `MAUI.DTO.Input` connect `MAUI.DTO.Input` to `EventBroadcastDTO`, `.AddAppServices`, `CodeSentDTO`, `LeagueDetailsDTO`, `MAUI.Helpers`, `AuthApiService`, `MyPredictionDTO`, `TeamDTO`, `LeagueSearchResultDTO`, `ScoringExampleDTO`, `MatchesDTO`, `NavMenuViewModelBase`, `PlayerHonours`, `MAUI.Views.Partials`, `UserProfileDTO`, `MAUI.ViewModel`, `UserPageDTO`, `IPredictionApiService`, `IDeviceTokenApiService`, `LeaderboardRowDTO`, `FriendDTO`, `FavoriteTeamDTO`?**
  _High betweenness centrality (0.069) - this node is a cross-community bridge._
- **Why does `ProfileViewModel` connect `ProfileViewModel` to `ContentPage`, `NavMenuViewModelBase`, `FriendApiService`, `MAUI.DTO.Input`, `PredictionStatsDTO`, `ProfilePage`, `FriendDTO`, `.Fill`, `FavoriteTeamDTO`, `MAUI.ViewModel`, `IUserCacheService`, `PredictionsListModel`, `UserPageDTO`, `ITeamApiService`, `Task`, `IPredictionApiService`, `LeaderboardRowDTO`?**
  _High betweenness centrality (0.056) - this node is a cross-community bridge._
- **Are the 31 inferred relationships involving `ProfileViewModel` (e.g. with `AcceptRequestCommand` and `CollapseFriendsCommand`) actually correct?**
  _`ProfileViewModel` has 31 INFERRED edges - model-reasoned connections that need verification._
- **Are the 13 inferred relationships involving `ContentPage` (e.g. with `EventDetailsViewModel` and `ErrorMessage`) actually correct?**
  _`ContentPage` has 13 INFERRED edges - model-reasoned connections that need verification._
- **Are the 14 inferred relationships involving `ContentPage` (e.g. with `ProfileViewModel` and `CollapseFriendsCommand`) actually correct?**
  _`ContentPage` has 14 INFERRED edges - model-reasoned connections that need verification._
- **Are the 19 inferred relationships involving `EventDetailsViewModel` (e.g. with `Details` and `ErrorMessage`) actually correct?**
  _`EventDetailsViewModel` has 19 INFERRED edges - model-reasoned connections that need verification._