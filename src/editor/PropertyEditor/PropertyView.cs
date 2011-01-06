// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NUnit.ProjectEditor
{
    public partial class PropertyView : UserControl, IPropertyView
    {
        #region Constructor

        public PropertyView()
        {
            InitializeComponent();
        }

        #endregion

        #region IPropertyView Members

        #region Properties

        public PropertyPresenter Presenter { get; set; }

        public string ProjectPath
        {
            get { return projectPathLabel.Text; }
            set { projectPathLabel.Text = value; }
        }

        public string ProjectBase
        {
            get { return projectBaseTextBox.Text; }
            set { projectBaseTextBox.Text = value; }
        }

        public string[] ProcessModelOptions
        {
            get { return GetComboBoxOptions(processModelComboBox); }
            set { SetComboBoxOptions(processModelComboBox, value); }
        }

        public string ProcessModel
        {
            get { return processModelComboBox.Text; }
            set 
            { 
                processModelComboBox.SelectedIndex = 
                    processModelComboBox.FindString(value); 
            }
        }

        public string[] DomainUsageOptions
        {
            get { return GetComboBoxOptions(domainUsageComboBox); } 
            set { SetComboBoxOptions(domainUsageComboBox, value); }
        }

        public string[] RuntimeOptions
        {
            get { return GetComboBoxOptions(this.runtimeComboBox); }
            set { SetComboBoxOptions(this.runtimeComboBox, value); }
        }

        public string[] RuntimeVersionOptions
        {
            get { return GetComboBoxOptions(this.runtimeVersionComboBox); }
            set { SetComboBoxOptions(this.runtimeVersionComboBox, value); }
        }

        public string DomainUsage
        {
            get { return domainUsageComboBox.Text; }
            set
            {
                domainUsageComboBox.SelectedIndex =
                    domainUsageComboBox.FindString(value);
            }
        }

        public string ActiveConfigName
        {
            get { return activeConfigLabel.Text; }
            set { activeConfigLabel.Text = value; }
        }

        public string[] ConfigList
        {
            set
            {
                string selectedConfig = (string)configComboBox.SelectedItem;
                configComboBox.Items.Clear();

                //if (selectedConfig == null)
                //    selectedConfig = project.ActiveConfigName;

                int selectedIndex = -1;

                foreach (string name in value)
                {
                    int index = configComboBox.Items.Add(name);
                    if (name == selectedConfig)
                        selectedIndex = index;
                }

                if (selectedIndex == -1 && configComboBox.Items.Count > 0)
                    selectedIndex = 0;

                configComboBox.SelectedIndex = selectedIndex;

                // Necessary because the previous line won't cause a change
                // if the selected index is -1.
                if (selectedIndex == -1)
                    configComboBox_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        public int SelectedConfig
        {
            get { return configComboBox.SelectedIndex; }
            set { configComboBox.SelectedIndex = value; }
        }

        public string Runtime
        {
            get { return this.runtimeComboBox.Text; }
            set { this.runtimeComboBox.Text = value; }
        }

        public string RuntimeVersion
        {
            get { return this.runtimeVersionComboBox.Text; }
            set { this.runtimeVersionComboBox.Text = value; }
        }

        public string ApplicationBase
        {
            get { return this.applicationBaseTextBox.Text; }
            set { this.applicationBaseTextBox.Text = value; }
        }

        public string ConfigurationFile
        {
            get { return this.configFileTextBox.Text; }
            set { this.configFileTextBox.Text = value; }
        }

        public BinPathType BinPathType
        {
            get
            {
                if (autoBinPathRadioButton.Checked)
                    return BinPathType.Auto;
                else if (manualBinPathRadioButton.Checked)
                    return BinPathType.Manual;
                else
                    return BinPathType.None;
            }
            set
            {
                switch (value)
                {
                    case BinPathType.Auto:
                        autoBinPathRadioButton.Checked = true;
                        break;
                    case BinPathType.Manual:
                        manualBinPathRadioButton.Checked = true;
                        break;
                    default:
                        noBinPathRadioButton.Checked = true;
                        break;
                }
            }
        }

        public string PrivateBinPath
        {
            get { return privateBinPathTextBox.Text; }
            set { privateBinPathTextBox.Text = value; }
        }

        public string[] AssemblyList
        {
            set
            {
                string selectedAssembly = (string)assemblyListBox.SelectedItem;

                assemblyListBox.Items.Clear();
                int selectedIndex = -1;

                foreach (string assembly in value)
                {
                    int index = assemblyListBox.Items.Add(Path.GetFileName(assembly));

                    if (assembly == selectedAssembly)
                        selectedIndex = index;
                }

                if (assemblyListBox.Items.Count > 0 && selectedIndex == -1)
                    selectedIndex = 0;

                if (selectedIndex == -1)
                {
                    removeAssemblyButton.Enabled = false;
                    assemblyPathBrowseButton.Enabled = false;
                }
                else
                {
                    assemblyListBox.SelectedIndex = selectedIndex;
                    removeAssemblyButton.Enabled = true;
                    assemblyPathBrowseButton.Enabled = true;
                }
            }
        }

        public int SelectedAssemblyIndex
        {
            get { return assemblyListBox.SelectedIndex; }
        }

        public string SelectedAssembly
        {
            get { return assemblyListBox.SelectedItem.ToString(); }
        }

        public string AssemblyPath
        {
            get { return this.assemblyPathTextBox.Text; }
            set { this.assemblyPathTextBox.Text = value; }
        }

        public bool PrivateBinPathEnabled
        {
            get { return privateBinPathTextBox.Enabled; }
            set { privateBinPathTextBox.Enabled = value; }
        }

        public bool AddAssemblyEnabled
        {
            get { return addAssemblyButton.Enabled; }
            set { addAssemblyButton.Enabled = value; }
        }

        public bool RemoveAssemblyEnabled
        {
            get { return removeAssemblyButton.Enabled; }
            set { removeAssemblyButton.Enabled = value; }
        }

        public bool EditAssemblyEnabled
        {
            set 
            {
                assemblyPathTextBox.Enabled = value;
                assemblyPathBrowseButton.Enabled = value; 
            }
        }

        #endregion

        #region Methods

        public void SetErrorMessage(string property, string message)
        {
            MessageBox.Show(
                property + ": " + message,
                "NUnit Project Editor",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        
        #endregion

        #endregion

        #region Event Handlers

        private void projectBaseBrowseButton_Click(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.BrowseForProjectBase();
        }

        private void editConfigsButton_Click(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.EditConfigs();
        }

        private void configBaseBrowseButton_Click(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.BrowseForConfigBase();
        }

        private void addAssemblyButton_Click(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.AddAssembly();
        }

        private void removeAssemblyButton_Click(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.RemoveAssembly();
        }

        private void assemblyPathBrowseButton_Click(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.BrowseForAssembly();
        }

        private void projectBaseTextBox_Validated(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("ProjectBase");
        }

        private void processModelComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("ProcessModel");
        }

        private void domainUsageComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("DomainUsage");
        }

        private void configComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("SelectedConfig");

            this.projectTabControl.Enabled = configComboBox.SelectedIndex >= 0;
        }

        private void runtimeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("Runtime");
        }

        private void runtimeVersionComboBox_Validated(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("RuntimeVersion");
        }

        private void applicationBaseTextBox_Validated(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("ApplicationBase");
        }

        private void configFileTextBox_Validated(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("DefaultConfigurationFile");
        }

        private void autoBinPathRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("BinPathType");
        }

        private void manualBinPathRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("BinPathType");
        }

        private void noBinPathRadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("BinPathType");
        }

        private void privateBinPathTextBox_Validated(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("PrivateBinPath");
        }

        private void assemblyListBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("SelectedAssembly");
        }

        private void assemblyPathTextBox_Validated(object sender, System.EventArgs e)
        {
            if (Presenter != null)
                Presenter.OnPropertyChange("AssemblyPath");
        }

        #endregion

        #region Helper Methods

        private string[] GetComboBoxOptions(ComboBox comboBox)
        {
            string[] options = new string[comboBox.Items.Count];

            for (int i = 0; i < comboBox.Items.Count; i++)
                options[i] = comboBox.Items[i].ToString();

            return options;
        }

        private void SetComboBoxOptions(ComboBox comboBox, string[] options)
        {
            comboBox.Items.Clear();

            foreach (object opt in options)
                comboBox.Items.Add(opt);

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        #endregion
    }
}
