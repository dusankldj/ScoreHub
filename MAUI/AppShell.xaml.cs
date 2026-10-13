namespace MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("login", typeof(Views.LoginPage));
            Routing.RegisterRoute("register", typeof(Views.RegisterPage));
            Routing.RegisterRoute("profile", typeof(Views.ProfilePage));
            Routing.RegisterRoute("eventdetails", typeof(Views.EventDetailsPage));
            Routing.RegisterRoute("teampage", typeof(Views.TeamPage));
            Routing.RegisterRoute("playerpage", typeof(Views.PlayerPage));
            Routing.RegisterRoute("leaguepage", typeof(Views.LeaguePage));
        }

        
    }
}
