using System;
using System.Drawing;
using System.Windows.Forms;
using FaustGames.Controls.Properties;
using FbxConverter;

namespace FaustGames.Controls
{
    public class ButtonEdit : TextBox
    {
        private readonly TextStorage _storage;

        public ButtonEdit()
        {
            _storage = new TextStorage(this);
        }

        public event EventHandler ButtonClick
        {
            add { _button.Click += value; }
            remove { _button.Click -= value; }
        }
        
        protected Button Button
        {
            get { return _button ?? (_button = CreateButton()); }
        }

        protected void SetButtonLocation()
        {
            Button.Location = new Point(ClientSize.Width - _button.Width + 1, -1);
        }

        protected override void OnCreateControl()
        {
            SetButtonLocation();
            base.OnCreateControl();
        }

        protected override void OnResize(EventArgs e)
        {
            SetButtonLocation();
            base.OnResize(e);
        }

        private Button CreateButton()
        {
            var btn = new Button
            {
                Size = new Size(18, ClientSize.Height + 2),
                Cursor = Cursors.Default,
                BackColor = SystemColors.Control,
                Image = Resources.dots
            };
            Controls.Add(btn);
            return btn;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveText();
                SelectAll();
            }
            base.OnKeyDown(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            SaveText();
            base.OnLeave(e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            LoadText();
        }


        protected void LoadText()
        {
            _storage.LoadText();
        }

        protected void SaveText()
        {
            _storage.SaveText();
        }
        
        private Button _button;
    }
}
