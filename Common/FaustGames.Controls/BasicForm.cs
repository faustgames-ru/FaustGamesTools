using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FaustGames.Controls
{
    public class BasicForm : Form
    {
        protected override void OnLoad(System.EventArgs e)
        {
            LoadState();
            base.OnLoad(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveState();
            base.OnClosing(e);
        }

        private ApplicationSettingsBase Settings
        {
            get { return SettingsService.Settings; }
        }

        protected string PropertyName
        {
            get { return Name; }
        }

        protected SettingsProperty SettingsProperty
        {
            get
            {
                var p = Settings.Properties[PropertyName];
                if (p == null)
                {
                    p = new SettingsProperty(PropertyName)
                    {
                        DefaultValue = "",
                        IsReadOnly = false,
                        PropertyType = typeof(string),
                        Provider = Settings.Providers["LocalFileSettingsProvider"]
                    };
                    p.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
                    Settings.Properties.Add(p);
                    Settings.Reload();
                }
                return p;
            }
        }

        protected void SaveState()
        {
            var state = CreateFormState();
            Settings[SettingsProperty.Name] = JsonConvert.SerializeObject(state);
            Settings.Save();
        }

        protected void LoadState()
        {
            Settings.Reload();
            var value = (string)Settings[SettingsProperty.Name];
            if (string.IsNullOrEmpty(value)) return;
            var state = JsonConvert.DeserializeObject<FormState>(value);
            if (state == null) return;
            SetFormState(state);
        }

        private void SetFormState(FormState state)
        {
            Location = new Point(state.PositionX, state.PositionY);
            Size = new Size(state.SizeX, state.SizeY);
            WindowState = (FormWindowState)state.WindowState;
        }

        FormState CreateFormState()
        {
            if (WindowState == FormWindowState.Normal)
            {
                return new FormState
                    {
                        PositionX = Location.X,
                        PositionY = Location.Y,
                        SizeX = Size.Width,
                        SizeY = Size.Height,
                        WindowState = (int) WindowState
                    };
            }
            return new FormState
            {
                PositionX = RestoreBounds.X,
                PositionY = RestoreBounds.Y,
                SizeX = RestoreBounds.Width,
                SizeY = RestoreBounds.Height,
                WindowState = (int)WindowState
            };
        }
    }

    public class FormState
    {
        [JsonProperty("positionX")]
        public int PositionX { get; set; }
        [JsonProperty("positionY")]
        public int PositionY { get; set; }
        [JsonProperty("sizeX")]
        public int SizeX { get; set; }
        [JsonProperty("sizeY")]
        public int SizeY { get; set; }
        [JsonProperty("windowState")]
        public int WindowState { get; set; }
    }
}
