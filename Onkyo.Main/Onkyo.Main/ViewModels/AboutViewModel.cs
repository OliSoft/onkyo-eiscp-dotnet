using Eiscp.Core.Commands;
using Eiscp.Core.Model;
using System.Diagnostics;
using System.Windows.Input;

using Xamarin.Forms;

namespace Onkyo.Main.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private readonly PWRCommand pwr = new PWRCommand();
        private readonly MVLCommand mvl = new MVLCommand();
        private readonly AMTCommand amt = new AMTCommand();

        public AboutViewModel() : this(null) { }

        public AboutViewModel(string name = null)
        {
            MessagingCenter.Instance.Subscribe<BaseCommand, string>(this, "UPDATE_AVR_COMMANDS", (cmd, resp) =>
            {
                if (cmd is PWRCommand pwrCmd && pwrCmd.IsCommand(resp))
                {
                    _powerOn = pwrCmd.Response.ToBoolean();
                    OnPropertyChanged(nameof(PowerOn));
                    var pow = PowerOn ? "on" : "off";
                    Debug.WriteLine("Power is " + pow);
                }
                else
                if (cmd is MVLCommand mvlCmd && mvlCmd.IsCommand(resp))
                {
                    if (mvlCmd.Response.FromHex() != -1)
                    {
                        _volume = mvlCmd.Response.FromHex();
                        OnPropertyChanged(nameof(Volume));
                        Debug.WriteLine("Volume is at " + _volume);
                    }
                }
                else
                if (cmd is AMTCommand amtCmd && amtCmd.IsCommand(resp))
                {
                    _isAudioMuted = amtCmd.Response.ToBoolean();
                    OnPropertyChanged(nameof(IsAudioMuted));
                    var mute = (IsAudioMuted == true) ? "muted" : "not muted";
                    Debug.WriteLine("Audio is " + mute);
                }
                else
                if (cmd is SLICommand sliCmd && sliCmd.IsCommand(resp))
                {
                    var index = sliCmd.Response.ToInt();
                    if (Title != sliCmd.Response.Description)
                        Title = sliCmd.Response.Description;
                    Debug.WriteLine("Seleded input is " + index);
                }
            });

            //Title = name ?? "AVR";

            OpenWebCommand = new Command(() => Onkyo.Core.Service.UpdateService.Get());
            PowerCommand = new Command(() => Onkyo.Core.Service.UpdateService.Send(PowerOn ? pwr.Off : pwr.On));
        }


        private int _volume;
        public int Volume
        {
            get => _volume;
            set
            {
                if(SetProperty(ref _volume, value))
                {
                    Onkyo.Core.Service.UpdateService.Send(mvl.SetVolume(_volume));
                }
            }
        }

        private bool _isAudioMuted;
        public bool IsAudioMuted
        {
            get => _isAudioMuted;
            set
            {
                if (SetProperty(ref _isAudioMuted, value))
                {
                    Onkyo.Core.Service.UpdateService.Send(_isAudioMuted ? amt.On : amt.Off);
                }
            }
        }

        private bool _powerOn;
        public bool PowerOn
        {
            get => _powerOn;
            set => SetProperty(ref _powerOn, value);
        }

        public ICommand OpenWebCommand { get; }
        public ICommand PowerCommand { get; }
    }
}