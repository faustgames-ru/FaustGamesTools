using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FaustGames.Controls;

namespace FbxConverter
{
    public class TextStorage
    {
        private readonly Control _control;

        public TextStorage(Control control)
        {
            _control = control;
        }

        private ApplicationSettingsBase Settings
        {
            get { return SettingsService.Settings; }
        }

        protected string PropertyName
        {
            get
            {
                if (_control.TopLevelControl != null)
                    return _control.TopLevelControl.Name + "." + _control.Name;
                return _control.Name;
            }
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

        public void LoadText()
        {
            Settings.Reload();
            _control.Text = (string)Settings[SettingsProperty.Name];
        }

        public void SaveText()
        {
            Settings[SettingsProperty.Name] = _control.Text;
            Settings.Save();
        }

    }
}
