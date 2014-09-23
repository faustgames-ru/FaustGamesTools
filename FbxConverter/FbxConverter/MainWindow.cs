using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FaustGames.Controls;

namespace FbxConverter
{
    public partial class MainWindow : BasicForm
    {
        readonly TextStorage _comboStorage;

        public MainWindow()
        {
            InitializeComponent();
            _comboStorage = new TextStorage(_configurations);

            Configurations.AfterSave += AfterSave;
            Configurations.Load();
            _configurations.Items.AddRange(Configurations.Instance.Values.Select(v=>(object)v).ToArray());
            _configurations.Items.Add("<New configutarion>");
            _comboStorage.LoadText();
        }

        private bool ValidateSourceFile()
        {
            var result = File.Exists(_fbxSource.Text);
            _fbxSourcePanel.BackColor = !result ? Color.Red : SystemColors.Control;
            return result;
        }

        private bool ValidateTargetFolder()
        {
            var result = Directory.Exists(_targetFolder.Text);
            _targetFolderPanel.BackColor = !result ? Color.Red : SystemColors.Control;
            return result;
        }

        private bool ValidateConfiguration()
        {
            var selected = _configurations.SelectedItem as Configuration;
            _configurationsPanel.BackColor = SystemColors.Control;
            if (selected == null)
            {
                _configurationsPanel.BackColor = Color.Red;
                return false;
            }
            var result = selected.VertexDeclarations.Length > 0;
            _propertiesPanel.BackColor = !result ? Color.Red : SystemColors.Control;
            return result;
        }

        private bool ValidateParameters()
        {
            var validSourceFile = ValidateSourceFile();
            var validTargetFolder = ValidateTargetFolder();
            var validConfiguration = ValidateConfiguration();
            return 
                validSourceFile &&
                validTargetFolder &&
                validConfiguration;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _comboStorage.SaveText();
            base.OnClosing(e);
        }

        private void AfterSave(object sender, EventArgs eventArgs)
        {
            var selected = _configurations.SelectedItem;
            _configurations.SelectedIndexChanged -= _configurations_SelectedIndexChanged;
            _configurations.SelectedItem = null;
            _configurations.SelectedItem = selected;
            _configurations.SelectedIndexChanged += _configurations_SelectedIndexChanged;
        }

        private void RefreshCombo()
        {
            _configurations.Items.Clear();
            _configurations.Items.AddRange(Configurations.Instance.Values.Select(v => (object)v).ToArray());
            _configurations.Items.Add("<New configutarion>");
        }

        private string GenerateName(string baseName)
        {
            var name = baseName;
            for (var i = 1; i < 1000; i++)
            {
                var exists = false;
                foreach (var c in Configurations.Instance.Values)
                {
                    if (c.Name == name)
                    {
                        name = string.Format("{0}{1}", baseName, i);
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    break;
            }
            return name;
        }

        private void _configurations_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var selected = _configurations.SelectedItem;
            if (selected is string)
            {
                var newSelected = new Configuration
                    {
                        Name = GenerateName("Configuration")
                    };
                Configurations.Instance.Add(newSelected);
                RefreshCombo();
                _configurations.SelectedItem = newSelected;
            }
            else
            {
                _configurationsProperties.SelectedObject = _configurations.SelectedItem;
            }
        }

        private void _remove_Click(object sender, System.EventArgs e)
        {
            var selected = _configurations.SelectedItem as Configuration;
            if (selected == null) return;
            Configurations.Instance.Remove(selected);
            RefreshCombo();
            _configurationsProperties.SelectedObject = null;
            if (_configurations.Items.Count > 1)
            {
                _configurations.SelectedItem = _configurations.Items[0];
            }
            else
            {
                _configurations.Text = "";
                _configurations.SelectedIndex = -1;
            }
        }

        private void DisableControls()
        {
            foreach (Control control in Controls)
                control.Enabled = false;
            ControlBox = false;
        }

        private void EnableControls()
        {
            foreach (Control control in Controls)
                control.Enabled = true;
            ControlBox = true;
        }


        private async void _convert_Click(object sender, EventArgs e)
        {
            if (!ValidateParameters())
                return;
            DisableControls();
            try
            {
                await
                    ConverterFbx.ConvertAsync(_fbxSource.Text, _targetFolder.Text,
                                         _configurations.SelectedItem as Configuration);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            EnableControls();
        }
    }
}
