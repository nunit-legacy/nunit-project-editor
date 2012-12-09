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

        public PropertyView() {
            InitializeComponent();

            InitializeViewElements();
        }

        private void InitializeViewElements() {
            DialogManager = new DialogManager("NUnit Project Editor");
            MessageDisplay = new MessageDisplay("NUnit Project Editor");

            BrowseProjectBaseCommand = new ButtonElement(projectBaseBrowseButton);
            EditConfigsCommand = new ButtonElement(editConfigsButton);
            BrowseConfigBaseCommand = new ButtonElement(configBaseBrowseButton);
            AddAssemblyCommand = new ButtonElement(addAssemblyButton);
            RemoveAssemblyCommand = new ButtonElement(removeAssemblyButton);
            MoveUpAssemblyCommand = new ButtonElement(upAssemblyButton);
            MoveDownAssemblyCommand = new ButtonElement(downAssemblyButton);
            BrowseAssemblyPathCommand = new ButtonElement(assemblyPathBrowseButton);

            ProjectPath = new TextElement(projectPathLabel);
            ProjectBase = new TextElement(projectBaseTextBox);
            ProcessModel = new ComboBoxElement(processModelComboBox);
            DomainUsage = new ComboBoxElement(domainUsageComboBox);
            Runtime = new ComboBoxElement(runtimeComboBox);
            RuntimeVersion = new ComboBoxElement(runtimeVersionComboBox);
            ActiveConfigName = new TextElement(activeConfigLabel);

            ConfigList = new ComboBoxElement(configComboBox);

            ApplicationBase = new TextElement(applicationBaseTextBox);
            ConfigurationFile = new TextElement(configFileTextBox);
            BinPathType = new RadioButtonGroup("BinPathType", autoBinPathRadioButton, manualBinPathRadioButton, noBinPathRadioButton);
            PrivateBinPath = new TextElement(privateBinPathTextBox);
            AssemblyPath = new TextElement(assemblyPathTextBox);
            AssemblyList = new ListBoxElement(assemblyListBox);
        }

        #endregion

        #region IPropertyView Members

        public IDialogManager DialogManager { get; private set; }
        public IMessageDisplay MessageDisplay { get; private set; }
        public IConfigurationEditorDialog ConfigurationEditorDialog {
            get { return new ConfigurationEditorDialog(); }
        }

        public ICommand BrowseProjectBaseCommand { get; private set; }
        public ICommand EditConfigsCommand { get; private set; }
        public ICommand BrowseConfigBaseCommand { get; private set; }
        public ICommand AddAssemblyCommand { get; private set; }
        public ICommand RemoveAssemblyCommand { get; private set; }
        public ICommand MoveUpAssemblyCommand { get; private set; }
        public ICommand MoveDownAssemblyCommand { get; private set; }
        public ICommand BrowseAssemblyPathCommand { get; private set; }

        public ITextElement ProjectPath { get; private set; }
        public ITextElement ProjectBase { get; private set; }

        public ISelectionList ProcessModel { get; private set; }

        public ISelectionList DomainUsage { get; private set; }

        public ITextElement ActiveConfigName { get; private set; }

        public ISelectionList ConfigList { get; private set; }

        public ISelectionList Runtime { get; private set; }
        public IComboBox RuntimeVersion { get; private set; }
        public ITextElement ApplicationBase { get; private set; }
        public ITextElement ConfigurationFile { get; private set; }
        public ISelection BinPathType { get; private set; }

        public ITextElement PrivateBinPath { get; private set; }

        public ISelectionList AssemblyList { get; private set; }

        public ITextElement AssemblyPath { get; private set; }

        #endregion

        #region Helper Methods

        private string[] GetComboBoxOptions(ComboBox comboBox) {
            string[] options = new string[comboBox.Items.Count];

            for (int i = 0; i < comboBox.Items.Count; i++)
                options[i] = comboBox.Items[i].ToString();

            return options;
        }

        private void SetComboBoxOptions(ComboBox comboBox, string[] options) {
            comboBox.Items.Clear();

            foreach (object opt in options)
                comboBox.Items.Add(opt);

            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        #endregion
    }
}
