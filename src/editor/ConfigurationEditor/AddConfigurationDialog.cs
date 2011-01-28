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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace NUnit.ProjectEditor
{
	/// <summary>
    /// Displays a dialog for creation of a new configuration.
    /// The dialog collects and validates the name and the
    /// name of a configuration to be copied and then adds the
    /// new configuration to the doc.
    /// 
    /// A DialogResult of DialogResult.OK indicates that the
    /// configuration was added successfully.
    /// </summary>
	public partial class AddConfigurationDialog : System.Windows.Forms.Form
	{
		#region Instance variables

		private string[] configList;
        private string initialCopyConfig;

		#endregion

		#region Constructor

		public AddConfigurationDialog( string[] configList, string initialCopyConfig )
		{ 
			InitializeComponent();

			this.configList = configList;
            this.initialCopyConfig = initialCopyConfig;
		}

		#endregion

		#region Properties

		public string ConfigToCreate { get; private set; }

		public string ConfigToCopy { get; private set; }

		#endregion

		#region Methods

		private void ConfigurationNameDialog_Load(object sender, System.EventArgs e)
		{
			configurationComboBox.Items.Add( "<none>" );
			configurationComboBox.SelectedIndex = 0;

			foreach( string config in configList )
			{
				int index = configurationComboBox.Items.Add( config );
				if ( config == initialCopyConfig )
					configurationComboBox.SelectedIndex = index;
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
            ConfigToCreate = configurationNameTextBox.Text;
            int index = configurationComboBox.SelectedIndex;
            ConfigToCopy = index > 0
                ? (string)configurationComboBox.SelectedItem
                : null;
            
            if (ConfigToCreate == string.Empty)
			{
				MessageBox.Show( 
                    "No configuration name provided", 
                    "Configuration Name Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
				return;
			}

            foreach (string config in configList)
            {
                if (config == ConfigToCreate)
                {
                    // TODO: Need general error message display
                    MessageBox.Show(
                        "A configuration with that name already exists", 
                        "Configuration Name Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }

			DialogResult = DialogResult.OK;

			Close();
		}

		#endregion
	}
}
