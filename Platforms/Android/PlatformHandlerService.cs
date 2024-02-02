namespace linphone_maui
{
    public partial class PlatformHandlerService
    {
        public partial IntPtr GetPlatformHandler()
        {
            return MainApplication.Handler;
        }
    }
}
