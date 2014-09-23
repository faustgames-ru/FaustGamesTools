using System;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace FaustGames.Controls
{
    public class OpenFolderEdit : ButtonEdit
    {
        private readonly VistaFolderBrowserDialog _bowseDialog = new VistaFolderBrowserDialog();

        protected override void OnCreateControl()
        {
            Button.Click += ButtonOnClick;
            base.OnCreateControl();
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            _bowseDialog.SelectedPath = Text;
            if (_bowseDialog.ShowDialog(this) != DialogResult.OK) return;
            Text = _bowseDialog.SelectedPath;
            SaveText();
        }
    }
}
