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
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor
{
	/// <summary>
	/// ConfigurationEditor form is designed for adding, deleting
	/// and renaming configurations from a doc.
	/// </summary>
	public partial class ConfigurationEditorView : System.Windows.Forms.Form, IConfigurationEditorView
    {
		#region Constructor

		public ConfigurationEditorView()
		{
			InitializeComponent();

            AddCommand = new ButtonElement(addButton);
            RemoveCommand = new ButtonElement(removeButton);
            RenameCommand = new ButtonElement(renameButton);
            ActiveCommand = new ButtonElement(activeButton);

            ConfigList = new ListControlWrapper(configListBox);
		}

		#endregion

        #region IConfigurationEditorView Members

        #region Properties

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand ActiveCommand { get; private set; }

        public ISelectionList ConfigList { get; private set; }

        public IViewElement NewName { get; private set; }

        public IAddConfigurationDialog AddConfigurationDialog
        {
            get { return new AddConfigurationDialog(); }
        }

        #endregion

        #region Methods

        public string GetNewNameForRename(string oldName)
        {
            string[] configList = new string[configListBox.Items.Count];
            for (int i = 0; i < configListBox.Items.Count; i++)
            {
                string config = configListBox.Items[i].ToString();
                if (config.EndsWith(" (active)"))
                    config = config.Substring(0, config.Length - 9);
                configList[i] = config;
            }

            using (RenameConfigurationDialog dlg =
                       new RenameConfigurationDialog(oldName, configList))
            {
                return dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK
                    ? dlg.ConfigurationName : null;
            }
        }

        #endregion

        #endregion
    }
}
