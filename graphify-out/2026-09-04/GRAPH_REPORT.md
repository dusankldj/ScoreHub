# Graph Report - project  (2026-09-03)

## Corpus Check
- Corpus is ~30,126 words - fits in a single context window. You may not need a graph.

## Summary
- 1236 nodes · 2186 edges · 95 communities (90 shown, 5 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 180 edges (avg confidence: 0.79)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- Infrastructure Sync
- Domain Entities
- Infrastructure Sync
- MAUI Views Partials
- MAUI MAUI
- DataAccess DataAccess
- MAUI DTO Input
- MAUI Services
- MAUI Services Interfaces
- Infrastructure Infrastructure
- MAUI Services
- DataAccess Configurations
- Domain Entities
- DataAccess Migrations
- Infrastructure Integrations DTO
- MAUI Views
- Infrastructure Integrations DTO
- Domain Entities
- Domain Entities
- Domain Entities
- Domain Entities
- WebAPI Properties
- Infrastructure Sync
- Domain Entities
- Domain Entities
- Infrastructure Integrations DTO
- WebAPI Infrastructure
- DataAccess Configurations
- Domain Entities
- Infrastructure Integrations DTO
- WebAPI Controllers
- MAUI Views
- MAUI Views
- Domain Entities
- WebAPI DTO Input
- Domain Entities
- Infrastructure SportsDbStartupSyncService
- MAUI Services
- WebAPI Controllers
- Domain Entities
- Domain Entities
- Infrastructure Integrations DTO
- Infrastructure Integrations DTO
- Infrastructure PlayerFormerTeamBackgroundService
- MAUI Platforms iOS
- Domain Entities
- Infrastructure Integrations DTO
- Infrastructure PlayerSyncBackgroundService
- MAUI DTO Output
- MAUI Helpers
- MAUI MauiProgram
- MAUI Resources Styles
- WebAPI NavMenuSportsDTO
- WebAPI WebAPI
- Domain Entities
- Infrastructure Integrations DTO
- Infrastructure Integrations DTO
- Infrastructure Integrations DTO
- Infrastructure Integrations DTO
- Infrastructure MatchScheduleBackgroundService
- Infrastructure MatchStatisticsTrackerService
- Infrastructure PreMatchLineupBackgroundService
- MAUI AppDelegate
- Domain Entities BaseEntity
- DataAccess BaseConfiguration
- Domain Entities
- Infrastructure Integrations DTO
- Infrastructure Integrations DTO
- MAUI Resources Images
- MAUI Views
- MAUI App.xaml
- Domain UserRole
- WebAPI Services Interfaces
- Domain Entities
- WebAPI Extensions
- MAUI MAUI
- MAUI LoginRequestDTO
- MAUI MauiExtensions
- WebAPI JWT
- Infrastructure Integrations DTO
- WebAPI AuthResponseDTO
- DataAccess MatchConfiguration
- Infrastructure SportsDbApiException
- Infrastructure SportOptions
- Domain SyncedEntity
- DataAccess TeamEquipmentConfiguration
- DataAccess SportConfiguration
- MAUI MainApplication
- CLAUDE.md CLAUDE
- MAUI App
- MAUI RegisterPage.xaml
- WebAPI RefreshRequestDTO
- Domain Class1
- MAUI AboutAssets
- MAUI Colors

## God Nodes (most connected - your core abstractions)
1. `ScoreHubDbContext` - 59 edges
2. `Domain.Entities` - 50 edges
3. `Player` - 36 edges
4. `SportsDbSyncService` - 35 edges
5. `Match` - 33 edges
6. `SportsDbMatch` - 32 edges
7. `ISportsDbSyncService` - 27 edges
8. `SportsDbClient` - 25 edges
9. `SportsDbPlayer` - 24 edges
10. `Team` - 22 edges

## Surprising Connections (you probably didn't know these)
- `TeamEquipmentConfiguration` --references--> `TeamEquipment`  [EXTRACTED]
  DataAccess/Configurations/TeamEquipmentConfiguration.cs → Domain/Entities/TeamEquipment.cs
- `ScoreHubDbContext` --references--> `EventLineup`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventLineup.cs
- `ScoreHubDbContext` --references--> `Player`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventLineup.cs
- `ScoreHubDbContext` --references--> `Team`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventLineup.cs
- `ScoreHubDbContext` --references--> `EventStatistic`  [EXTRACTED]
  DataAccess/ScoreHubDbContext.cs → Domain/Entities/EventStatistic.cs

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **ScoreHub App Icon & Splash Composition** — maui_resources_appicon_appicon_background, maui_resources_appicon_appiconfg_trophy_icon, maui_resources_splash_splash_trophy_icon [INFERRED 0.85]
- **ScoreHub Logo Size Variant Set** — maui_resources_images_scorehub_160x160px_logo, maui_resources_images_scorehub_160x90px_logo, maui_resources_images_scorehub_160x90px_v2_logo, maui_resources_images_scorehub_64x36px_logo, maui_resources_images_scorehub_64x64px_logo [INFERRED 0.85]

## Communities (95 total, 5 thin omitted)

### Community 0 - "Infrastructure Sync"
Cohesion: 0.12
Nodes (17): FirstName, Func, CancellationToken, List, Task, ISportsDbClient, CancellationToken, DateTime (+9 more)

### Community 1 - "Domain Entities"
Cohesion: 0.04
Nodes (42): EntityTypeBuilder, PlayerStatisticConfiguration, DateOnly, DateTime, ICollection, Player, BirthDate, BirthLocation (+34 more)

### Community 2 - "Infrastructure Sync"
Cohesion: 0.17
Nodes (10): StatusCode, IActionResult, CancellationToken, Task, ISportsDbSyncService, CancellationToken, HttpPost, ILogger (+2 more)

### Community 3 - "MAUI Views Partials"
Cohesion: 0.07
Nodes (28): BindableObject, BindableProperty, ., IconFontFamily, IconGlyph, Name, ICommand, AccountMenuBorder (+20 more)

### Community 4 - "MAUI MAUI"
Cohesion: 0.07
Nodes (31): net10.0, Microsoft.EntityFrameworkCore (10.0.9), Microsoft.NET.Sdk, Domain, net10.0, Microsoft.NET.Sdk, net10.0-android, net10.0 (+23 more)

### Community 5 - "DataAccess DataAccess"
Cohesion: 0.08
Nodes (25): Jwt, ModelBuilder, TeamEquipment, ScoreHubDbContext, EventLineups, EventStatistics, FavoriteTeams, Jwt (+17 more)

### Community 6 - "MAUI DTO Input"
Cohesion: 0.11
Nodes (20): SportsDTO, Id, Name, CancellationToken, List, Task, ISportCacheService, CancellationToken (+12 more)

### Community 7 - "MAUI Services"
Cohesion: 0.17
Nodes (8): MAUI.DTO.Input, MAUI.Views.Partials, MAUI.ViewModel, MAUI.DTO.Output, MAUI.Services, MAUI.Services.Interfaces, MAUI.Helpers, Icons

### Community 8 - "MAUI Services Interfaces"
Cohesion: 0.17
Nodes (10): DelegatingHandler, HttpRequestMessage, HttpResponseMessage, CancellationToken, Task, RefreshTokenHelper, Task, AuthService (+2 more)

### Community 9 - "Infrastructure Infrastructure"
Cohesion: 0.25
Nodes (6): CancellationToken, Dictionary, HttpClient, List, Task, SportsDbClient

### Community 10 - "MAUI Services"
Cohesion: 0.17
Nodes (13): AuthResponseDTO, RefreshToken, Token, RefreshRequestDTO, RefreshToken, Token, CancellationToken, HttpClient (+5 more)

### Community 11 - "DataAccess Configurations"
Cohesion: 0.11
Nodes (12): BaseConfiguration, DataAccess.Configurations, FavoriteTeamConfiguration, EntityTypeBuilder, JwtConfiguration, EntityTypeBuilder, LogConfiguration, EntityTypeBuilder (+4 more)

### Community 12 - "Domain Entities"
Cohesion: 0.09
Nodes (21): DateTime, ICollection, Match, AwayTeam, AwayTeamId, AwayTeamScore, DetailsSyncedAt, EventDate (+13 more)

### Community 13 - "DataAccess Migrations"
Cohesion: 0.10
Nodes (14): DataAccess.Migrations, DateOnly, DateTime, DateOnly, DateTime, ModelBuilder, InitialCreate, DateOnly (+6 more)

### Community 14 - "Infrastructure Integrations DTO"
Cohesion: 0.10
Nodes (21): SportsDbMatch, DateEvent, IdAwayTeam, IdEvent, IdHomeTeam, IdLeague, IdVenue, IntAwayScore (+13 more)

### Community 15 - "MAUI Views"
Cohesion: 0.14
Nodes (19): Email, FirstName, LastName, RegisterCommand, RepeatPassword, RegisterViewModel, Email, ErrorMessage (+11 more)

### Community 16 - "Infrastructure Integrations DTO"
Cohesion: 0.10
Nodes (20): SportsDbPlayer, DateBorn, DateDied, IdPlayer, IdTeam, StrBirthLocation, StrDescriptionEN, StrFacebook (+12 more)

### Community 17 - "Domain Entities"
Cohesion: 0.12
Nodes (15): EntityTypeBuilder, EventLineupConfiguration, EventLineup, ApiId, Event, EventId, Formation, HomeGame (+7 more)

### Community 18 - "Domain Entities"
Cohesion: 0.12
Nodes (14): EntityTypeBuilder, PlayerHonourConfiguration, PlayerHonour, HonourId, HonourLogoUrl, HonourName, HonourTrophyUrl, League (+6 more)

### Community 19 - "Domain Entities"
Cohesion: 0.13
Nodes (13): EntityTypeBuilder, PlayerFormerTeamConfiguration, PlayerFormerTeam, Appearances, BadgeUrl, Departed, FormerTeam, FormerTeamId (+5 more)

### Community 20 - "Domain Entities"
Cohesion: 0.12
Nodes (15): ICollection, Team, AwayMatches, Country, Equipment, FavoritedBy, HomeMatches, League (+7 more)

### Community 21 - "WebAPI Properties"
Cohesion: 0.13
Nodes (15): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, launchBrowser, applicationUrl, commandName (+7 more)

### Community 22 - "Infrastructure Sync"
Cohesion: 0.27
Nodes (5): Infrastructure.Sync, DataAccess, Domain.Entities, WebAPI.Middleware, Class1

### Community 23 - "Domain Entities"
Cohesion: 0.14
Nodes (12): EntityTypeBuilder, PlayerMilestoneConfiguration, DateOnly, PlayerMilestone, DateAchieved, MilestoneId, MilestoneLogoUrl, MilestoneName (+4 more)

### Community 24 - "Domain Entities"
Cohesion: 0.14
Nodes (13): Log, Exception, HttpMethod, HttpUrl, InnerException, StackTrace, UserId, HttpContext (+5 more)

### Community 25 - "Infrastructure Integrations DTO"
Cohesion: 0.14
Nodes (12): List, SportsDbTeam, IdLeague, IdTeam, StrBadge, StrCountry, StrSport, StrStadium (+4 more)

### Community 26 - "WebAPI Infrastructure"
Cohesion: 0.20
Nodes (7): WebAPI.Services, Infrastructure.Integrations, Infrastructure.Integrations.SportsDB, Infrastructure, WebAPI.JWT, WebAPI.Services.Interfaces, Class1

### Community 27 - "DataAccess Configurations"
Cohesion: 0.14
Nodes (8): EntityTypeBuilder, LeagueConfiguration, EntityTypeBuilder, PlayerConfiguration, TeamConfiguration, VenueConfiguration, Player, SyncedEntityConfiguration

### Community 28 - "Domain Entities"
Cohesion: 0.14
Nodes (13): ICollection, User, Email, FavoriteTeams, FirstName, LastName, Password, Predictions (+5 more)

### Community 29 - "Infrastructure Integrations DTO"
Cohesion: 0.14
Nodes (14): SportsDbEventLineup, IdApiFootball, IdEvent, IdLineup, IdPlayer, IdTeam, IntSquadNumber, StrFormation (+6 more)

### Community 30 - "WebAPI Controllers"
Cohesion: 0.33
Nodes (9): Authorize, ActionResult, CancellationToken, HttpPost, Task, AuthController, List, ErrorResponseDTO (+1 more)

### Community 31 - "MAUI Views"
Cohesion: 0.21
Nodes (12): LoginCommand, LoginViewModel, ErrorMessage, IsBusy, LoginCommand, Password, Username, ContentPage (+4 more)

### Community 32 - "MAUI Views"
Cohesion: 0.18
Nodes (7): ContentPage, MAUI.Views, HomePage, TappedEventArgs, LoginPage, ContentPage, ProfilePage

### Community 33 - "Domain Entities"
Cohesion: 0.17
Nodes (10): EntityTypeBuilder, EventStatisticsConfiguration, EventStatistic, ApiId, AwayScore, Event, EventId, EventName (+2 more)

### Community 34 - "WebAPI DTO Input"
Cohesion: 0.17
Nodes (10): WebAPI.DTO.Input, LoginRequestDTO, Password, Username, RegisterRequestDTO, Email, FirstName, LastName (+2 more)

### Community 35 - "Domain Entities"
Cohesion: 0.17
Nodes (10): EntityTypeBuilder, Team, ICollection, League, Country, CurrentSeason, LogoUrl, Name (+2 more)

### Community 36 - "Infrastructure SportsDbStartupSyncService"
Cohesion: 0.30
Nodes (7): CancellationToken, DateTime, ILogger, IServiceScopeFactory, Task, TimeSpan, SportsDbStartupSyncService

### Community 37 - "MAUI Services"
Cohesion: 0.23
Nodes (10): SportMenuItem, IconFontFamily, IconGlyph, Id, Name, ObservableCollection, RelayCommand, Task (+2 more)

### Community 38 - "WebAPI Controllers"
Cohesion: 0.20
Nodes (7): ControllerBase, WebAPI.Controllers, WeatherForecast, SportsController, HttpGet, IEnumerable, WeatherForecastController

### Community 39 - "Domain Entities"
Cohesion: 0.18
Nodes (9): EntityTypeBuilder, Venue, ICollection, Sport, Description, IconUrl, Leagues, Name (+1 more)

### Community 40 - "Domain Entities"
Cohesion: 0.18
Nodes (10): ICollection, Venue, Country, ImageUrl, Location, Matches, Name, Sport (+2 more)

### Community 41 - "Infrastructure Integrations DTO"
Cohesion: 0.20
Nodes (10): List, SportsDbLeague, IdLeague, StrBadge, StrCountry, StrCurrentSeason, StrLeague, StrSport (+2 more)

### Community 42 - "Infrastructure Integrations DTO"
Cohesion: 0.18
Nodes (10): SportsDbPlayerFormerTeam, Id, IdFormerTeam, IdPlayer, IntAppearances, IntGoals, StrBadge, StrDeparted (+2 more)

### Community 43 - "Infrastructure PlayerFormerTeamBackgroundService"
Cohesion: 0.33
Nodes (7): BackgroundService, CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PlayerFormerTeamBackgroundService

### Community 44 - "MAUI Platforms iOS"
Cohesion: 0.20
Nodes (5): MAUI, MainActivity, Program, Program, MauiAppCompatActivity

### Community 45 - "Domain Entities"
Cohesion: 0.20
Nodes (9): Prediction, Hit, Match, MatchId, Points, PredictedAwayScore, PredictedHomeScore, User (+1 more)

### Community 46 - "Infrastructure Integrations DTO"
Cohesion: 0.20
Nodes (10): SportsDbPlayerHonour, Id, IdHonour, IdLeague, IdPlayer, IdTeam, StrHonour, StrHonourLogo (+2 more)

### Community 47 - "Infrastructure PlayerSyncBackgroundService"
Cohesion: 0.38
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PlayerSyncBackgroundService

### Community 48 - "MAUI DTO Output"
Cohesion: 0.20
Nodes (8): RegisterRequestDTO, Email, FirstName, LastName, Password, Username, RelayCommand, Task

### Community 49 - "MAUI Helpers"
Cohesion: 0.24
Nodes (7): IconFontFamily, FontAwesome, MaterialDesignIcons, SportIcon, FontFamilyName, Dictionary, SportIconMap

### Community 50 - "MAUI MauiProgram"
Cohesion: 0.22
Nodes (7): Exception, HomeViewModel, IActivationState, MauiApp, Window, CrashApp, MauiProgram

### Community 51 - "MAUI Resources Styles"
Cohesion: 0.29
Nodes (9): CommonStates, Disabled, Normal, Off, On, PointerOver, ResourceDictionary, VisualState (+1 more)

### Community 52 - "WebAPI NavMenuSportsDTO"
Cohesion: 0.20
Nodes (8): ActionResult, CancellationToken, HttpGet, List, Task, NavMenuSportsDTO, Id, Name

### Community 53 - "WebAPI WebAPI"
Cohesion: 0.22
Nodes (7): WebAPI, DateOnly, WeatherForecast, Date, Summary, TemperatureC, TemperatureF

### Community 54 - "Domain Entities"
Cohesion: 0.22
Nodes (8): TeamEquipment, Equipment, Season, Team, TeamId, Type, Username, SyncedEntity

### Community 55 - "Infrastructure Integrations DTO"
Cohesion: 0.22
Nodes (8): SportsDbEventStatistic, IdApiFootball, IdEvent, IdStatistic, IntAway, IntHome, StrEvent, StrStat

### Community 56 - "Infrastructure Integrations DTO"
Cohesion: 0.22
Nodes (8): SportsDbPlayerMilestone, DateMilestone, Id, IdMilestone, IdPlayer, IdTeam, StrMilestone, StrMilestoneLogo

### Community 57 - "Infrastructure Integrations DTO"
Cohesion: 0.22
Nodes (8): SportsDbPlayerStatistic, Id, IdLeague, IdPlayer, IdTeam, StrSeason, StrStatistic, StrValue

### Community 58 - "Infrastructure Integrations DTO"
Cohesion: 0.25
Nodes (8): List, SportsDbSport, IdSport, StrSport, StrSportDescription, StrSportThumb, SportsDbSportResponse, Sports

### Community 59 - "Infrastructure MatchScheduleBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchScheduleBackgroundService

### Community 60 - "Infrastructure MatchStatisticsTrackerService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, MatchStatisticsTrackerService

### Community 61 - "Infrastructure PreMatchLineupBackgroundService"
Cohesion: 0.39
Nodes (6): CancellationToken, ILogger, IServiceScopeFactory, Task, TimeSpan, PreMatchLineupBackgroundService

### Community 62 - "MAUI AppDelegate"
Cohesion: 0.22
Nodes (5): MauiApp, AppDelegate, MauiApp, AppDelegate, MauiUIApplicationDelegate

### Community 63 - "Domain Entities BaseEntity"
Cohesion: 0.25
Nodes (6): Domain.Entities.BaseEntity, DateTime, BaseEntity, CreatedAt, Id, UpdatedAt

### Community 64 - "DataAccess BaseConfiguration"
Cohesion: 0.43
Nodes (4): EntityTypeBuilder, BaseConfiguration, SyncedEntityConfiguration, IEntityTypeConfiguration

### Community 65 - "Domain Entities"
Cohesion: 0.25
Nodes (6): EntityTypeBuilder, FavoriteTeam, Notifications, Team, TeamId, UserId

### Community 66 - "Infrastructure Integrations DTO"
Cohesion: 0.25
Nodes (7): SportsDbTeamEquipment, IdEquipment, IdTeam, StrEquipment, StrSeason, StrType, StrUsername

### Community 67 - "Infrastructure Integrations DTO"
Cohesion: 0.25
Nodes (7): SportsDbVenue, IdVenue, StrCountry, StrLocation, StrSport, StrThumb, StrVenue

### Community 68 - "MAUI Resources Images"
Cohesion: 0.25
Nodes (8): ScoreHub App Icon Background, ScoreHub App Icon Foreground (Trophy Mark), ScoreHub Logo 160x160, ScoreHub Logo 160x90, ScoreHub Logo 160x90 v2 (Blank Variant), ScoreHub Logo 64x36, ScoreHub Logo 64x64, ScoreHub Splash Screen

### Community 69 - "MAUI Views"
Cohesion: 0.29
Nodes (6): IsLoggedIn, LogoutCommand, ContentPage, ErrorMessage, OverflowSports, Sports

### Community 70 - "MAUI App.xaml"
Cohesion: 0.29
Nodes (4): MAUI.WinUI, MauiWinUIApplication, MauiApp, App

### Community 71 - "Domain UserRole"
Cohesion: 0.29
Nodes (5): EntityTypeBuilder, ICollection, UserRole, Name, Users

### Community 72 - "WebAPI Services Interfaces"
Cohesion: 0.43
Nodes (3): User, IJwtService, JwtService

### Community 73 - "Domain Entities"
Cohesion: 0.29
Nodes (6): Jwt, IsRevoked, RefreshJwtToken, Token, User, UserId

### Community 74 - "WebAPI Extensions"
Cohesion: 0.52
Nodes (3): IServiceCollection, IConfiguration, Extensions

### Community 75 - "MAUI MAUI"
Cohesion: 0.29
Nodes (4): IActivationState, Window, Shell, AppShell

### Community 76 - "MAUI LoginRequestDTO"
Cohesion: 0.29
Nodes (5): LoginRequestDTO, Password, Username, RelayCommand, Task

### Community 77 - "MAUI MauiExtensions"
Cohesion: 0.38
Nodes (4): IConfiguration, MauiExtensions, HomeViewModel, MauiAppBuilder

### Community 78 - "WebAPI JWT"
Cohesion: 0.29
Nodes (6): JwtOptions, Audience, Issuer, RefreshTokenExpirationHours, SecretKey, TokenExpirationMinutes

### Community 80 - "WebAPI AuthResponseDTO"
Cohesion: 0.33
Nodes (4): WebAPI.DTO.Output, AuthResponseDTO, RefreshToken, Token

### Community 81 - "DataAccess MatchConfiguration"
Cohesion: 0.33
Nodes (3): Domain.Entities.SyncedEntity, EntityTypeBuilder, MatchConfiguration

### Community 82 - "Infrastructure SportsDbApiException"
Cohesion: 0.33
Nodes (5): Infrastructure.Integrations.Exceptions, Exception, SportsDbApiException, HttpMethod, Url

### Community 83 - "Infrastructure SportOptions"
Cohesion: 0.33
Nodes (6): List, SportsOptions, ApiKey, BaseUrl, LeagueExternalIds, SportsIds

### Community 84 - "Domain SyncedEntity"
Cohesion: 0.40
Nodes (5): BaseEntity, DateTime, SyncedEntity, ExternalId, LastSyncedAt

### Community 85 - "DataAccess TeamEquipmentConfiguration"
Cohesion: 0.40
Nodes (3): DataAccess.Configurations.BaseConfiguration, EntityTypeBuilder, TeamEquipmentConfiguration

### Community 86 - "DataAccess SportConfiguration"
Cohesion: 0.40
Nodes (3): EntityTypeBuilder, SportConfiguration, Sport

### Community 87 - "MAUI MainApplication"
Cohesion: 0.40
Nodes (3): MauiApp, MainApplication, MauiApplication

### Community 88 - "CLAUDE.md CLAUDE"
Cohesion: 0.50
Nodes (4): GRAPH_REPORT.md Reference, graphify Knowledge Graph Tool, graphify update Command, graphify Wiki Index

### Community 91 - "WebAPI RefreshRequestDTO"
Cohesion: 0.50
Nodes (3): RefreshRequestDTO, RefreshToken, Token

### Community 93 - "MAUI AboutAssets"
Cohesion: 0.67
Nodes (3): LoadMauiAsset Method, MauiAsset Build Action, FileSystem.OpenAppPackageFileAsync

## Knowledge Gaps
- **460 isolated node(s):** `Class1`, `net10.0`, `Microsoft.EntityFrameworkCore (10.0.9)`, `Microsoft.EntityFrameworkCore.SqlServer (10.0.9)`, `Microsoft.NET.Sdk` (+455 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 616 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **5 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ScoreHubDbContext` connect `DataAccess DataAccess` to `Infrastructure Sync`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Infrastructure Sync`, `Domain Entities`, `Domain Entities`, `DataAccess Configurations`, `WebAPI Controllers`, `Domain Entities`, `Domain Entities`, `Infrastructure SportsDbStartupSyncService`, `WebAPI Controllers`, `Domain Entities`, `Infrastructure PlayerFormerTeamBackgroundService`, `Domain Entities`, `Infrastructure PlayerSyncBackgroundService`, `Infrastructure MatchScheduleBackgroundService`, `Infrastructure MatchStatisticsTrackerService`, `Infrastructure PreMatchLineupBackgroundService`, `Domain Entities`, `Domain UserRole`, `WebAPI Services Interfaces`, `WebAPI Extensions`, `DataAccess SportConfiguration`?**
  _High betweenness centrality (0.183) - this node is a cross-community bridge._
- **Why does `SportsDbSyncService` connect `Infrastructure Sync` to `Infrastructure Sync`, `DataAccess DataAccess`, `WebAPI Extensions`, `Infrastructure SportOptions`, `Infrastructure Sync`?**
  _High betweenness centrality (0.125) - this node is a cross-community bridge._
- **Why does `Domain.Entities` connect `Infrastructure Sync` to `Domain Entities`, `DataAccess Configurations`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `WebAPI Infrastructure`, `DataAccess Configurations`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain Entities`, `Domain UserRole`, `Domain Entities`, `DataAccess MatchConfiguration`, `DataAccess TeamEquipmentConfiguration`, `DataAccess SportConfiguration`?**
  _High betweenness centrality (0.062) - this node is a cross-community bridge._
- **What connects `Class1`, `net10.0`, `Microsoft.EntityFrameworkCore (10.0.9)` to the rest of the system?**
  _460 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Infrastructure Sync` be split into smaller, more focused modules?**
  _Cohesion score 0.12215758857747223 - nodes in this community are weakly interconnected._
- **Should `Domain Entities` be split into smaller, more focused modules?**
  _Cohesion score 0.044444444444444446 - nodes in this community are weakly interconnected._
- **Should `MAUI Views Partials` be split into smaller, more focused modules?**
  _Cohesion score 0.07017543859649122 - nodes in this community are weakly interconnected._