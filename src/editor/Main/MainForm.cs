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
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor
{
    public partial class MainForm : Form, IMainView
    {
        #region Constructor

        public MainForm()
        {
            InitializeComponent();

            this.MessageDisplay = new MessageDisplay("Nunit Project Editor");
            this.DialogManager = new DialogManager("NUnit Project Editor");

            this.NewProjectCommand = new MenuElement(newToolStripMenuItem);
            this.OpenProjectCommand = new MenuElement(openToolStripMenuItem);
            this.CloseProjectCommand = new MenuElement(closeToolStripMenuItem);
            this.SaveProjectCommand = new MenuElement(saveToolStripMenuItem);
            this.SaveProjectAsCommand = new MenuElement(saveAsToolStripMenuItem);
        }

        #endregion

        #region IMainView Members

        #region Events

        public event ActionStartingDelegate ActiveViewChanging;
        public event ActionDelegate ActiveViewChanged;

        #endregion

        #region Properties

        public IDialogManager DialogManager { get; private set; }

        public ICommand NewProjectCommand { get; private set; }
        public ICommand OpenProjectCommand { get; private set; }
        public ICommand CloseProjectCommand { get; private set; }
        public ICommand SaveProjectCommand { get; private set; }
        public ICommand SaveProjectAsCommand { get; private set; }

        public IXmlView XmlView
        {
            get { return xmlView; }
        }

        public IPropertyView PropertyView
        {
            get { return propertyView; }
        }

        public SelectedView SelectedView
        {
            get { return (SelectedView)tabControl1.SelectedIndex; }
        }

        public IMessageDisplay MessageDisplay { get; private set; }

        #endregion

        #endregion

        #region Event Handlers

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (CloseProjectCommand.Click != null)
            //    CloseProjectCommand.Click();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog(this);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (ActiveViewChanging != null && !ActiveViewChanging())
                e.Cancel = true;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (ActiveViewChanged != null)
                ActiveViewChanged();
        }

        #endregion
    }
}
