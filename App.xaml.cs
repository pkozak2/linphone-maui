using Linphone;

namespace linphone_maui
{
    public partial class App : Application
    {
        public static string ConfigFilePath { get; set; }
        public static string FactoryFilePath { get; set; }

        public LinphoneManager Manager { get; set; }

        public Core Core
        {
            get
            {
                return Manager.Core;
            }
        }

        public App()
        {
            InitializeComponent();
            var a = new PlatformHandlerService().GetPlatformHandler();
            Manager = new LinphoneManager();
            Manager.Init(ConfigFilePath, FactoryFilePath, a);

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();
            Manager.Start();
        }
    }
}
