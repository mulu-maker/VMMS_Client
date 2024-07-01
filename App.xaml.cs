namespace VMMS_Client
{
    public partial class App : Application
    {
        private string PageName = "启动";
        public App()
        {
            InitializeComponent();


            MainPage = new AppShell();
            
        }
        protected override void OnStart()
        {
            // 应用程序启动时调用
        }

        protected override void OnSleep()
        {
            // 应用程序进入后台时调用
        }

        protected override void OnResume()
        {
            // 应用程序从后台恢复时调用
        }
    }
}
