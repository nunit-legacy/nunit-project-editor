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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor
{
    public partial class PropertyView : UserControl, IPropertyView
    {
        #region Constructor

        public PropertyView()
        {
            InitializeComponent();

            InitializeViewElements();
        }

        private void InitializeViewElements()
        {
            BrowseProjectBaseCommand = new ButtonElement(projectBaseBrowseButton);
            EditConfigsCommand = new ButtonElement(editConfigsButton);
            BrowseConfigBaseCommand = new ButtonElement(configBaseBrowseButton);
            AddAssemblyCommand = new ButtonElement(addAssemblyButton);
            RemoveAssemblyCommand = new ButtonElement(removeAssemblyButton);
            BrowseAssemblyPathCommand = new ButtonElement(assemblyPathBrowseButton);

            ProjectPath = new ControlElement(projectPathLabel);
            ProjectBase = new ValidatedElement(projectBaseTextBox);
            ProcessModel = new ListControlWrapper(processModelComboBox);
            DomainUsage = new ListControlWrapper(domainUsageComboBox);
            Runtime = new ListControlWrapper(runtimeComboBox);
            RuntimeVersion = new ListControlWrapper(runtimeVersionComboBox);
            ActiveConfigName = new ControlElement(activeConfigLabel);

            ConfigList = new ListControlWrapper(configComboBox);

            ApplicationBase = new ValidatedElement(applicationBaseTextBox);
            ConfigurationFile = new ValidatedElement(configFileTextBox);
            BinPathType = new RadioButtonGroup("BinPathType", autoBinPathRadioButton, manualBinPathRadioButton, noBinPathRadioButton);
            PrivateBinPath = new ValidatedElement(privateBinPathTextBox);
            AssemblyPath = new ValidatedElement(assemblyPathTextBox);
            AssemblyList = new ListControlWrapper(assemblyListBox);
        }

        #endregion
        
        #region IPropertyView Members

        #region Properties

        public ICommand BrowseProjectBaseCommand { get; private set; }
        public ICommand EditConfigsCommand { get; private set; }
        public ICommand BrowseConfigBaseCommand { get; private set; }
        public ICommand AddAssemblyCommand { get; private set; }
        public ICommand RemoveAssemblyCommand { get; private set; }
        public ICommand BrowseAssemblyPathCommand { get; private set; }

        public ITextElement ProjectPath{ get; private set; }
        public IValidatedElement ProjectBase { get; private set; }

        public ISelectionList ProcessModel { get; private set; }

        public ISelectionList DomainUsage { get; private set; }

        public ITextElement ActiveConfigName { get; private set; }

        public ISelectionList ConfigList { get; private set; }

        public ISelectionList Runtime { get; private set; }
        public ISelectionList RuntimeVersion { get; private set; }
        public IValidatedElement ApplicationBase { get; private set; }
        public IValidatedElement ConfigurationFile { get; private set; }
        public ISelection BinPathType { get; private set; }

        public IValidatedElement PrivateBinPath { get; private set;  }

        public ISelectionList AssemblyList { get; private set; }

        public IValidatedElement AssemblyPath { get; private set; }

        #endregion

        #region Methods

        public void SetAssemblyList(IEnumerable<string> list)
        {
            string selectedAssembly = (string)assemblyListBox.SelectedItem;

            assemblyListBox.Items.Clear();
            int selectedIndex = -1;

            foreach (string assembly in list)
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
