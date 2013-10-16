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
    /// presenters for each _view of the project. It directly handles
    /// the menu commands from the top-level _view and coordinates 
    /// changes in the two different submodels.
    /// </summary>
    public class MainPresenter
    {
        private IMainView _view;
        private IProjectModel _model;

        #region Constructor

        public MainPresenter(IProjectModel model, IMainView view)
        {
            _model = model;
            _view = view;

            InitializeView();
            WireUpEvents();
        }

        private void InitializeView()
        {
            // Enable and disable menu items
            _view.NewProjectCommand.Enabled = true;
            _view.OpenProjectCommand.Enabled = true;
            _view.CloseProjectCommand.Enabled = false;
            _view.SaveProjectCommand.Enabled = false;
            _view.SaveProjectAsCommand.Enabled = false;
        }

        private void WireUpEvents()
        {
            // View events
            _view.FormClosing += view_FormClosing;

            // View command events
            _view.NewProjectCommand.Execute += NewProjectCommand_Execute;
            _view.OpenProjectCommand.Execute += OpenProjectCommand_Execute;
            _view.SaveProjectCommand.Execute += SaveProjectCommand_Execute;
            _view.SaveProjectAsCommand.Execute += SaveProjectAsCommand_Execute;
            _view.CloseProjectCommand.Execute += CloseProjectCommand_Execute;

            // Model events
            _model.Created += model_ProjectCreated;
            _model.Closed += model_ProjectClosed;
        }

        #endregion

        #region View EventHandlers

        private void view_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseProjectCommand_Execute();
        }

        private void NewProjectCommand_Execute()
        {
            _model.CreateNewProject();
        }

        private void OpenProjectCommand_Execute()
        {
            string path = _view.DialogManager.GetFileOpenPath(
                "Open Project", 
                "Test Projects (*.nunit)|*.nunit",
                null);

            if (path != null)
            {
                try
                {
                    _model.OpenProject(path);
                }
                catch (Exception ex)
                {
                    _view.MessageDisplay.Error(ex.Message);
                }
            }
        }

        private void CloseProjectCommand_Execute()
        {
            if (_model.IsValid && _model.HasUnsavedChanges &&
                _view.MessageDisplay.AskYesNoQuestion(string.Format("Do you want to save changes to {0}?", _model.Name)))
                    SaveProjectCommand_Execute();

            _model.CloseProject();
        }

        private void SaveProjectCommand_Execute()
        {
            if (IsValidWritableProjectPath(_model.ProjectPath))
            {
                _model.SaveProject();
            }
            else
            {
                this.SaveProjectAsCommand_Execute();
            }
        }

        private void SaveProjectAsCommand_Execute()
        {
            string path = _view.DialogManager.GetSaveAsPath(
                "Save As",
                "Test Projects (*.nunit)|*.nunit");

            if (path != null)
            {
                _model.SaveProject(path);
                _view.PropertyView.ProjectPath.Text = _model.ProjectPath;
            }
        }

        #endregion

        #region Model EventHandlers

        private void model_ProjectCreated()
        {
            _view.CloseProjectCommand.Enabled = true;

            bool validXml = _model.IsValid;
            _view.SaveProjectCommand.Enabled = validXml;
            _view.SaveProjectAsCommand.Enabled = validXml;
 
            // Force display of XmlView with invalid XML
            if (!validXml)
                _view.SelectedView = EditorView.XmlView;
        }

        private void model_ProjectClosed()
        {
            _view.CloseProjectCommand.Enabled = false;
            _view.SaveProjectCommand.Enabled = false;
            _view.SaveProjectAsCommand.Enabled = false;
        }

        #endregion

        #region Helper Methods

        private static bool IsValidWritableProjectPath(string path)
        {
            if (!Path.IsPathRooted(path))
                return false;

            if (!ProjectModel.IsProjectFile(path))
                return false;

            if (!File.Exists(path))
                return true;

            return (File.GetAttributes(path) & FileAttributes.ReadOnly) == 0;
        }

        #endregion
    }
}
