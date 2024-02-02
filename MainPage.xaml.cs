using Linphone;

namespace linphone_maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        private Core Core
        {
            get
            {
                return ((App)App.Current).Core;
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        bool opusStateBool = true;
        private void onOpusToggleClicked(object sender, EventArgs e)
        {
            opusStateBool = !opusStateBool;
            var codec = Core.AudioPayloadTypes.First(w => w.MimeType == "opus");
            codec.Enable(opusStateBool);
            opusStateLabel.Text = codec.Enabled().ToString();
        }
        private void onOpusStateClicked(object sender, EventArgs e)
        {
            var codec = Core.AudioPayloadTypes.First(w => w.MimeType == "opus");
            opusStateLabel.Text = codec.Enabled().ToString();
        }
    }

}
