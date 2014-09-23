using System;
using System.Windows.Forms;

namespace FaustGames.Controls
{
    public class OpenFileEdit : ButtonEdit
    {
        private readonly OpenFileDialog _bowseDialog = new OpenFileDialog();

        public string Filter 
        {
            get { return _bowseDialog.Filter; }
            set { _bowseDialog.Filter = value; }
        }

        protected override void OnCreateControl()
        {
            Button.Click += ButtonOnClick;
            base.OnCreateControl();
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            _bowseDialog.FileName = Text;
            if (_bowseDialog.ShowDialog(this) != DialogResult.OK) return;
            Text = _bowseDialog.FileName;
            SaveText();
        }
    }
}
