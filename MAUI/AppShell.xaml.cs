namespace MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("login", typeof(Views.LoginPage));
            Routing.RegisterRoute("register", typeof(Views.RegisterPage));
            Routing.RegisterRoute("forgotpassword", typeof(Views.ForgotPasswordPage));
            Routing.RegisterRoute("profile", typeof(Views.ProfilePage));
            Routing.RegisterRoute("eventdetails", typeof(Views.EventDetailsPage));
            Routing.RegisterRoute("teampage", typeof(Views.TeamPage));
            Routing.RegisterRoute("playerpage", typeof(Views.PlayerPage));
            Routing.RegisterRoute("leaguepage", typeof(Views.LeaguePage));
            Routing.RegisterRoute("userpage", typeof(Views.UserPage));
            Routing.RegisterRoute("scoringrules", typeof(Views.ScoringRulesPage));
            Routing.RegisterRoute("settings", typeof(Views.SettingsPage));
            Routing.RegisterRoute("changepassword", typeof(Views.ChangePasswordPage));
            Routing.RegisterRoute("editprofile", typeof(Views.EditProfilePage));
        }

        
    }
}
