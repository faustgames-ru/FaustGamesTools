using System.Configuration;

namespace FaustGames.Controls
{
    public static class SettingsService
    {
        public static ApplicationSettingsBase Settings = Properties.ControlsSettings.Default;
    }
}
