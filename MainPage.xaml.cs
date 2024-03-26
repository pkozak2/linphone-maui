﻿using Android.Media.TV;
using Google.Android.Material.Color.Utilities;
using Linphone;
using System.Diagnostics;

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

        private Dictionary<string, TransportType> Transports;

        private void OnRegistration(Core lc, ProxyConfig config, RegistrationState state, string message)
        {
            Debug.WriteLine("Registration state changed: " + state);

            registration_status.Text = "Registration state changed: " + state;

            if (state == RegistrationState.Ok)
            {
                register.IsEnabled = false;
                stack_registrar.IsVisible = false;
                unregister.IsVisible = true;
            }
            if (state != RegistrationState.Ok)
            {
                register.IsEnabled = true;
                stack_registrar.IsVisible = true;
                unregister.IsVisible = false;
            }
        }

        private void OnCall(Core lc, Call lcall, CallState state, string message)
        {
            Debug.WriteLine("Call state changed: " + state);

            call_status.Text = "Call state changed: " + state;

            if (lc.CallsNb > 0)
            {
                video.IsEnabled = state == CallState.StreamsRunning;

                if (state == CallState.IncomingReceived)
                {
                    call.Text = "Answer Call (" + lcall.RemoteAddressAsString + ")";
                    video_call.Text = "Answer Call with Video";
                }
                else
                {
                    call.Text = "Terminate Call";
                    video_call.Text = "Terminate Call";
                }
                if (lcall.CurrentParams.VideoEnabled)
                {
                    video.Text = "Stop Video";
                }
                else
                {
                    video.Text = "Start Video";
                }
            }
            else
            {
                video.IsEnabled = false;
                call.Text = "Start Call";
                call_stats.Text = "";
                video_call.Text = "Start Video Call";
            }
            camera.IsEnabled = video.IsEnabled;
        }

        private void OnStats(Core lc, Call call, CallStats stats)
        {
            Debug.WriteLine("Call stats: " + stats.DownloadBandwidth + " kbits/s / " + stats.UploadBandwidth + " kbits/s");

            call_stats.Text = "Call stats: " + stats.DownloadBandwidth + " kbits/s / " + stats.UploadBandwidth + " kbits/s";
        }

        private void OnLogCollectionUpload(Core lc, CoreLogCollectionUploadState state, string info)
        {
            Debug.WriteLine("Logs upload state changed: " + state + ", url is " + info);

            logsUrl.Text = info;
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                //Device.OpenUri(new Uri(((Label)s).Text));
            };
            logsUrl.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public MainPage()
        {
            InitializeComponent();
            welcome.Text = "Linphone Xamarin version: " + Core.Version;

            Permissions.RequestAsync<Permissions.Microphone>();
            Permissions.RequestAsync<Permissions.Phone>();
            Permissions.RequestAsync<Permissions.Camera>();

            Core.Listener.OnRegistrationStateChanged += OnRegistration;
            Core.Listener.OnCallStateChanged += OnCall;
            Core.Listener.OnCallStatsUpdated += OnStats;
            Core.Listener.OnLogCollectionUploadStateChanged += OnLogCollectionUpload;

            videoStateLabel.Text = Core.VideoCaptureEnabled.ToString();

            Transports = new Dictionary<string, TransportType>
            {
                { "UDP", TransportType.Udp }, { "TCP", TransportType.Tcp }, { "TLS", TransportType.Tls },
            };
            foreach (string protocol in Transports.Keys)
            {
                transport.Items.Add(protocol);
            }
            transport.SelectedIndex = 2;

            if (Core.DefaultProxyConfig?.State == RegistrationState.Ok)
            {
                register.IsEnabled = false;
                stack_registrar.IsVisible = false;
                unregister.IsVisible = false;
                registration_status.Text = "Registration state: " + Core.DefaultProxyConfig.State;
            }
        }
        private void OnRegisterClicked(object sender, EventArgs e)
        {
            Core.VerifyServerCertificates(false);
            var aCreator = AccountCreator.Create(Core);
            aCreator.Username = username.Text;
            aCreator.Password = password.Text;
            aCreator.Domain = domain.Text;
            aCreator.DisplayName = username.Text;
            aCreator.Transport = Transports.Values.ElementAt(transport.SelectedIndex);

            var proxy = aCreator.CreateProxyConfig();
            proxy.ServerAddr = port.Text != "5060" ? $"{domain.Text}:{port.Text}" : domain.Text;
            proxy.Route = port.Text != "5060" ? $"{domain.Text}:{port.Text}" : domain.Text;
            proxy.RegisterEnabled = true;
            aCreator.ProxyConfig = proxy;

            var account = aCreator.CreateAccountInCore();

            //var authInfo = Factory.Instance.CreateAuthInfo(username.Text, null, password.Text, null, null, domain.Text);
            //Core.AddAuthInfo(authInfo);

            //var proxyConfig = Core.CreateProxyConfig();
            //var identity = Factory.Instance.CreateAddress("sip:sample@domain.tld");
            //identity.Username = username.Text;
            //identity.Domain = domain.Text;
            //identity.Transport = Transports.Values.ElementAt(transport.SelectedIndex);
            //proxyConfig.Edit();
            //proxyConfig.IdentityAddress = identity;
            //proxyConfig.ServerAddr = domain.Text;
            //proxyConfig.Route = domain.Text;
            //proxyConfig.RegisterEnabled = true;
            //proxyConfig.Done();
            //Core.AddProxyConfig(proxyConfig);
            //Core.DefaultProxyConfig = proxyConfig;

            //Core.RefreshRegisters();

            var a = Core.AccountList.ToList();
        }

        public void OnChatMessageSent(ChatRoom room, EventLog log)
        {
            Debug.WriteLine("Chat message sent !");
        }

        public void OnMessageReceived(ChatRoom room, EventLog log)
        {
            Debug.WriteLine("Chat message received !");
        }

        public void OnMessageStateChanged(ChatMessage msg, ChatMessageState state)
        {
            Debug.WriteLine("Chat message state changed: " + state);
        }

        public void OnMessageClicked(object sender, EventArgs e)
        {
            ProxyConfig proxyConfig = Core.DefaultProxyConfig;
            if (proxyConfig != null)
            {
                Address remoteAddr = Core.InterpretUrl(address.Text);
                if (remoteAddr != null)
                {
                    ChatRoom room = Core.GetChatRoom(remoteAddr, proxyConfig.IdentityAddress);

                    ChatMessage message = room.CreateMessage(chatMessage.Text);
                    message.Listener.OnMsgStateChanged = OnMessageStateChanged;

                    message.Send();
                }
            }

        }

        private void OnCallClicked(object sender, EventArgs e)
        {
            if (Core.CallsNb == 0)
            {
                var addr = Core.InterpretUrl(address.Text);
                Core.InviteAddress(addr);
            }
            else
            {
                Call call = Core.CurrentCall;
                if (call.State == CallState.IncomingReceived)
                {
                    call.Accept();
                }
                else
                {
                    Core.TerminateAllCalls();
                }
            }
        }

        private void OnVideoCallClicked(object sender, EventArgs e)
        {
            if (Core.CallsNb == 0)
            {
                var addr = Core.InterpretUrl(address.Text);
                CallParams CallParams = Core.CreateCallParams(null);
                CallParams.VideoEnabled = true;
                Core.InviteAddressWithParams(addr, CallParams);
            }
            else
            {
                Call call = Core.CurrentCall;
                if (call.State == CallState.IncomingReceived)
                {
                    CallParams CallParams = Core.CreateCallParams(call);
                    CallParams.VideoEnabled = true;
                    call.AcceptWithParams(CallParams);
                }
                else
                {
                    Core.TerminateAllCalls();
                }
            }
        }

        private void OnVideoClicked(object sender, EventArgs e)
        {
            if (Core.CallsNb > 0)
            {
                Call call = Core.CurrentCall;
                if (call.State == CallState.StreamsRunning)
                {
                    Core.VideoAdaptiveJittcompEnabled = true;
                    CallParams param = Core.CreateCallParams(call);
                    param.VideoEnabled = !call.CurrentParams.VideoEnabled;
                    param.VideoDirection = MediaDirection.SendRecv;
                    call.Update(param);
                }
            }
        }

        private void OnCameraClicked(object sender, EventArgs e)
        {
            if (Core.CallsNb > 0)
            {
                Call call = Core.CurrentCall;
                if (call.State == CallState.StreamsRunning)
                {
                    try
                    {
                        string currentDevice = Core.VideoDevice;
                        IEnumerable<string> devices = Core.VideoDevicesList;
                        int index = 0;
                        foreach (string d in devices)
                        {
                            if (d == currentDevice)
                            {
                                break;
                            }
                            index++;
                        }

                        String newDevice;
                        if (index == 1)
                        {
                            newDevice = devices.ElementAt(0);
                        }
                        else if (devices.Count() > 1)
                        {
                            newDevice = devices.ElementAt(1);
                        }
                        else
                        {
                            newDevice = devices.ElementAt(index);
                        }
                        Core.VideoDevice = newDevice;

                        call.Update(call.Params);
                    }
                    catch (ArithmeticException)
                    {
                        Debug.WriteLine("Cannot swtich camera : no camera");
                    }
                }
            }
        }

        private void onUploadLogsCliked(object sender, EventArgs e)
        {
            Core.UploadLogCollection();
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

        private void unregister_Clicked(object sender, EventArgs e)
        {
            Core.ClearAllAuthInfo();
            Core.ClearAccounts();
        }

        private void videoToggle_Clicked(object sender, EventArgs e)
        {
            Core.VideoCaptureEnabled = !Core.VideoCaptureEnabled;
            videoStateLabel.Text = Core.VideoCaptureEnabled.ToString();

        }

        private void videoState_Clicked(object sender, EventArgs e)
        {
            videoStateLabel.Text = Core.VideoCaptureEnabled.ToString();
        }
    }

}
