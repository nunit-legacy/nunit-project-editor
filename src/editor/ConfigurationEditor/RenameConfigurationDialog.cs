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
    /// Displays a dialog for entry of a new name for an
    /// existing configuration. This dialog collects and
    /// validates the name. The caller is responsible for
    /// actually renaming the cofiguration.
    /// </summary>
	public partial class RenameConfigurationDialog : System.Windows.Forms.Form
	{
		#region Instance Variables

		/// <summary>
		/// The new name to give the configuration
		/// </summary>
		private string configurationName;

		/// <summary>
		/// The original name of the configuration
		/// </summary>
		private string originalName;

        /// <summary>
        /// An array of existing config names used for validation
        /// </summary>
        private string[] configList;

		#endregion

		#region Constructor

		public RenameConfigurationDialog( string configurationName, string[] configList )
		{
			InitializeComponent();
			this.configurationName = configurationName;
			this.originalName = configurationName;
            this.configList = configList;
        }

		#endregion

		#region Properties & Methods

		public string ConfigurationName
		{
			get{ return configurationName; }
			set{ configurationName = value; }
		}
		
		private void ConfigurationNameDialog_Load(object sender, System.EventArgs e)
		{
			if ( configurationName != null )
			{
				configurationNameTextBox.Text = configurationName;
				configurationNameTextBox.SelectAll();
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			configurationName = configurationNameTextBox.Text;
		
            foreach(string existingName in configList)
            {
                if (existingName == configurationName)
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

		private void configurationNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			okButton.Enabled = 
				configurationNameTextBox.TextLength > 0 &&
				configurationNameTextBox.Text != originalName;
		}

		#endregion
	}
}
