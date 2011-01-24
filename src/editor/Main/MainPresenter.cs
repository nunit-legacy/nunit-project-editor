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
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NUnit.ProjectEditor
{
    /// <summary>
    /// MainPresenter is the top-level presenter with subordinate 
    /// presenters for each view of the doc. It directly handles
    /// the menu commands from the top-level view and coordinates 
    /// changes in the two different submodels.
    /// </summary>
    public class MainPresenter
    {
        private IMainView view;
        private IProjectDocument doc;
        private IDialogManager dialogManager;

        private PropertyPresenter propertyPresenter;
        private XmlPresenter xmlPresenter;

        #region Constructor

        public MainPresenter(IProjectDocument doc, IMainView view, IDialogManager dialogHandler)
        {
            this.doc = doc;
            this.view = view;
            this.dialogManager = dialogHandler;

            // Set up handlers for view events
            view.NewProjectCommand.Execute += CreateNewProject;
            view.OpenProjectCommand.Execute += OpenProject;
            view.SaveProjectCommand.Execute += SaveProject;
            view.SaveProjectAsCommand.Execute += SaveProjectAs;
            view.CloseProjectCommand.Execute += CloseProject;
            view.ActiveViewChanging += this.ValidateActiveViewChange;
            view.ActiveViewChanged += this.ActiveViewChanged;

            // Set up handlers for doc events
            doc.ProjectCreated += new ActionDelegate(OnProjectCreated);
            doc.ProjectClosed += new ActionDelegate(OnProjectClosed);
        }

        public bool ValidateActiveViewChange()
        {
            doc.SynchronizeModel();
            if (doc.IsValid)
                return true;

            view.SaveProjectCommand.Enabled = false;
            view.SaveProjectAsCommand.Enabled = false;
            return false;
        }

        public void ActiveViewChanged()
        {
            switch (view.SelectedView)
            {
                case SelectedView.PropertyView:
                    propertyPresenter.LoadViewFromModel();
                    break;

                case SelectedView.XmlView:
                    xmlPresenter.LoadViewFromModel();
                    break;
            }
        }

        #endregion

        #region Command Event Handlers

        private void CreateNewProject()
        {
            doc.CreateNewProject();
            view.CloseProjectCommand.Enabled = true;
            view.SaveProjectCommand.Enabled = true;
            view.SaveProjectAsCommand.Enabled = true;
        }

        private void OpenProject()
        {
            string path = dialogManager.GetFileOpenPath(
                "Open Project", 
                "Test Projects (*.nunit)|*.nunit",
                null);

            if (path != null)
            {
                doc.OpenProject(path);
                view.CloseProjectCommand.Enabled = true;
                view.SaveProjectCommand.Enabled = doc.IsValid;
                view.SaveProjectAsCommand.Enabled = doc.IsValid;
            }
        }

        private void CloseProject()
        {
            if (doc.HasUnsavedChanges &&
                dialogManager.AskYesNoQuestion(string.Format("Do you want to save changes to {0}?", doc.Name)))
                    SaveProject();

            doc.CloseProject();
            view.CloseProjectCommand.Enabled = false;
            view.SaveProjectCommand.Enabled = false;
            view.SaveProjectAsCommand.Enabled = false;
        }

        private void SaveProject()
        {
            if (IsValidWritableProjectPath(doc.ProjectPath))
            {
                doc.SaveProject();
            }
            else
            {
                this.SaveProjectAs();
            }
        }

        private void SaveProjectAs()
        {
            string path = dialogManager.GetSaveAsPath(
                "Save As",
                "Test Projects (*.nunit)|*.nunit");

            if (path != null)
            {
                doc.SaveProject(path);
                view.PropertyView.ProjectPath.Text = doc.ProjectPath;
            }
        }

        #endregion

        #region Model Event Handlers

        void OnProjectCreated()
        {
            // Set up property editor triad
            ProjectModel project = new ProjectModel(this.doc);
            IPropertyView propertyView = view.PropertyView;
            propertyPresenter = new PropertyPresenter(project, propertyView, dialogManager);
            propertyView.Visible = true;

            // Set up XML editor triad
            //XmlModel xmlModel = new XmlModel();
            //xmlModel.XmlText = doc.ToXml();
            IXmlView xmlView = view.XmlView;
            xmlPresenter = new XmlPresenter((IXmlModel)doc, xmlView);
            xmlView.Visible = true;

            propertyPresenter.LoadViewFromModel();
            xmlPresenter.LoadViewFromModel();
        }

        void OnProjectClosed()
        {

            view.XmlView.Xml.Text = null;

            view.XmlView.Visible = false;
            view.PropertyView.Visible = false;
        }

        #endregion

        #region Helper Methods

        private static bool IsValidWritableProjectPath(string path)
        {
            if (!Path.IsPathRooted(path))
                return false;

            if (!ProjectDocument.IsProjectFile(path))
                return false;

            if (!File.Exists(path))
                return true;

            return (File.GetAttributes(path) & FileAttributes.ReadOnly) == 0;
        }

        #endregion
    }
}
