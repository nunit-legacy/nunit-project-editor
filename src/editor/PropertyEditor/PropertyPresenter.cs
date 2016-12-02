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
using System.IO;
using System.Text;

namespace NUnit.ProjectEditor
{
    /// <summary>
    /// The ProjectPresenter handles presentation of the project as
    /// a set of properties, which the ProjectView is expected to
    /// display.
    /// </summary>
    public class PropertyPresenter
    {
        private IPropertyModel _model;
        private IPropertyView _view;
        private IProjectConfig _selectedConfig;

        public PropertyPresenter(IPropertyModel model, IPropertyView view)
        {
            _model = model;
            _view = view;

            InitializeView();
            WireUpEvents();
        }

        private void InitializeView()
        {
            _view.ProcessModel.SelectionList = new string[] { "Default", "Single", "Separate", "Multiple" };
            _view.DomainUsage.SelectionList = new string[] { "Default", "Single", "Multiple" };
            _view.Runtime.SelectionList = new string[] { "Any", "Net", "Mono" };
            _view.RuntimeVersion.SelectionList = new string[] { "1.0.3705", "1.1.4322", "2.0.50727", "4.0.21006" };
        }

        private void WireUpEvents()
        {
            _view.Selected += view_Selected;

            _view.BrowseProjectBaseCommand.Execute += BrowseProjectBaseCommand_Execute;
            _view.BrowseConfigBaseCommand.Execute += BrowseConfigBaseCommand_Execute;
            _view.EditConfigsCommand.Execute += EditConfigsCommand_Execute;

            _view.AddAssemblyCommand.Execute += AddAssemblyCommand_Execute;
            _view.RemoveAssemblyCommand.Execute += RemoveAssemblyCommand_Execute;
            _view.MoveUpAssemblyCommand.Execute += MoveUpAssemblyCommand_Execute;
            _view.MoveDownAssemblyCommand.Execute += MoveDownAssemblyCommand_Execute;
            _view.BrowseAssemblyPathCommand.Execute += BrowseForAssemblyPathCommand_Execute;

            _view.ProjectBase.Validated += ProjectBase_Validated;
            _view.ProcessModel.SelectionChanged += ProcessModel_SelectionChanged;
            _view.DomainUsage.SelectionChanged += DomainUsage_SelectionChanged;
            _view.ConfigList.SelectionChanged += ConfigList_SelectionChanged;

            _view.Runtime.SelectionChanged += RunTime_SelectionChanged;
            _view.RuntimeVersion.TextValidated += RuntimeVersion_TextValidated;
            _view.ApplicationBase.Validated += ApplicationBase_Validated;
            _view.ConfigurationFile.Validated += ConfigurationFile_Validated;
            _view.BinPathType.SelectionChanged += BinPathType_SelectionChanged;
            _view.PrivateBinPath.Validated += PrivateBinPath_Validated;
            _view.AssemblyList.SelectionChanged += AssemblyList_SelectionChanged;
            _view.AssemblyPath.Validated += AssemblyPath_Validated;

            _model.Project.Created += Project_Created;
            _model.Project.Closed += Project_Closed;
        }

        private void LoadViewFromModel()
        {
            _view.Visible = true;

            _view.ProjectPath.Text = _model.ProjectPath;
            _view.ProjectBase.Text = _model.EffectiveBasePath;
            _view.ActiveConfigName.Text = _model.ActiveConfigName;

            _view.ProcessModel.SelectedItem = _model.ProcessModel;
            _view.DomainUsage.SelectedItem = _model.DomainUsage;

            _view.ConfigList.SelectionList = _model.ConfigNames;
            if (_model.ConfigNames.Length > 0)
            {
                _view.ConfigList.SelectedIndex = 0;
                _selectedConfig = _model.Configs[0];
            }
            else
            {
                _view.ConfigList.SelectedIndex = -1;
                _selectedConfig = null;
            }

            ConfigList_SelectionChanged();
        }

        #region Command Events

        private void BrowseProjectBaseCommand_Execute()
        {
            string message = "Select ApplicationBase for the model as a whole.";
            string projectBase = _view.DialogManager.GetFolderPath(message, _view.ProjectBase.Text);
            if (projectBase != null && projectBase != _model.BasePath)
                _view.ProjectBase.Text = _model.BasePath = projectBase;
        }

        private void BrowseConfigBaseCommand_Execute()
        {
            string config = "unset";
            if (_view.ConfigList.SelectedIndex >= 0)
                config = _model.Configs[_view.ConfigList.SelectedIndex].Name;

            string message = string.Format(
                "Select ApplicationBase for the {0} configuration, if different from the model as a whole.",
                config);
            string initialFolder = _view.ApplicationBase.Text;
            if (initialFolder == string.Empty)
                initialFolder = _view.ProjectBase.Text;

            string appbase = _view.DialogManager.GetFolderPath(message, initialFolder);
            if (appbase != null && appbase != _view.ApplicationBase.Text)
                UpdateApplicationBase(appbase);
        }

        private void EditConfigsCommand_Execute()
        {
            var editorView = _view.ConfigurationEditorDialog;
            new ConfigurationEditor(_model, editorView);
            editorView.ShowDialog();

            string selectedConfig = _view.ConfigList.SelectedItem;
            string[] configs = _model.ConfigNames;

            _view.ConfigList.SelectionList = configs;

            if (configs.Length > 0)
            {
                _view.ConfigList.SelectedIndex = 0;
                foreach (string config in configs)
                {
                    if (config == selectedConfig)
                        _view.ConfigList.SelectedItem = config;
                }
            }

            _view.ActiveConfigName.Text = _model.ActiveConfigName;
        }

        private void AddAssemblyCommand_Execute()
        {
            string assemblyPath = _view.DialogManager.GetFileOpenPath(
                "Select Assembly",
                "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*",
                _view.AssemblyPath.Text);

            if (assemblyPath != null)
            {
                var basePath = _model.EffectiveBasePath;
                if (_selectedConfig.BasePath != null)
                    basePath = Path.Combine(_model.EffectiveBasePath, _selectedConfig.BasePath);
                assemblyPath = PathUtils.RelativePath(basePath, assemblyPath);
                _selectedConfig.Assemblies.Add(assemblyPath);
                SetAssemblyList();
            }
        }

        private void RemoveAssemblyCommand_Execute()
        {
            string question = string.Format("Remove {0} from project?", _view.AssemblyList.SelectedItem);
            if (_view.MessageDisplay.AskYesNoQuestion(question))
            {
                _selectedConfig.Assemblies.Remove(_view.AssemblyList.SelectedItem);
                SetAssemblyList();
            }
        }

        private void MoveUpAssemblyCommand_Execute()
        {
            var newIndex = _view.AssemblyList.SelectedIndex - 1;
            
            if (IsIndexInRange(newIndex))
            {
                SwapAssembly(newIndex);
            }
        }
        
        private void MoveDownAssemblyCommand_Execute()
        {
            var newIndex = _view.AssemblyList.SelectedIndex + 1;
            if (IsIndexInRange(newIndex))
            {
                SwapAssembly(newIndex);
            }
        }

        private void SwapAssembly(int newIndex) {
            var selectedItem = _view.AssemblyList.SelectedItem;
            var selectedIndex = _view.AssemblyList.SelectedIndex;

            var assemblyList = new List<string>(_view.AssemblyList.SelectionList);
            assemblyList.RemoveAt(_view.AssemblyList.SelectedIndex);
            assemblyList.Insert(newIndex, selectedItem);
            _view.AssemblyList.SelectionList = assemblyList.ToArray();

            _view.AssemblyList.SelectedIndex = newIndex;
            _selectedConfig.Assemblies[_view.AssemblyList.SelectedIndex] = selectedItem;
            AssemblyList_SelectionChanged();
        }

        private bool IsIndexInRange(int currentIndex) {
            return currentIndex > -1 && currentIndex < _view.AssemblyList.SelectionList.Length;
        }

        private void BrowseForAssemblyPathCommand_Execute()
        {
            string assemblyPath = _view.DialogManager.GetFileOpenPath(
                "Select Assembly",
                "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*",
                _view.AssemblyPath.Text);

            if (assemblyPath != null)
            {
                _selectedConfig.Assemblies[_view.AssemblyList.SelectedIndex] = assemblyPath;
                SetAssemblyList();
            }
        }

        #endregion

        #region View Change Events

        private void view_Selected()
        {
            if (_model.Project.RootNode != null)
                LoadViewFromModel();
        }

        private void ProjectBase_Validated()
        {
            string projectBase = _view.ProjectBase.Text;

            if (projectBase == string.Empty)
                _view.ProjectBase.Text = projectBase = Path.GetDirectoryName(_model.ProjectPath);

            if (ValidateDirectoryPath("ProjectBase", projectBase))
                _model.BasePath = projectBase;
        }

        private void ProcessModel_SelectionChanged()
        {
            _model.ProcessModel = _view.ProcessModel.SelectedItem;
            _view.DomainUsage.SelectionList = _view.ProcessModel.SelectedItem == "Multiple"
                ? new string[] { "Default", "Single" }
                : new string[] { "Default", "Single", "Multiple" };
        }

        private void DomainUsage_SelectionChanged()
        {
            _model.DomainUsage = _view.DomainUsage.SelectedItem;
        }

        private void ConfigList_SelectionChanged()
        {
            IProjectConfig selectedConfig = _view.ConfigList.SelectedIndex >= 0
                ? _model.Configs[_view.ConfigList.SelectedIndex]
                : null;

            if (selectedConfig != null)
            {
                RuntimeFramework framework = selectedConfig.RuntimeFramework;
                _view.Runtime.SelectedItem = framework.Runtime.ToString();
                _view.RuntimeVersion.Text = framework.Version == new Version()
                    ? string.Empty
                    : framework.Version.ToString();

                if (selectedConfig.BasePath == null)
                    _view.ApplicationBase.Text = _model.EffectiveBasePath;
                else
                    _view.ApplicationBase.Text = PathUtils.RelativePath(
                        Path.Combine(_model.EffectiveBasePath, selectedConfig.BasePath),
                        selectedConfig.BasePath);
                _view.ConfigurationFile.Text = selectedConfig.ConfigurationFile;
                _view.BinPathType.SelectedIndex = (int)selectedConfig.BinPathType;
                if (selectedConfig.BinPathType == BinPathType.Manual)
                    _view.PrivateBinPath.Text = selectedConfig.PrivateBinPath;
                else
                    _view.PrivateBinPath.Text = string.Empty;

                SetAssemblyList();
            }
            else
            {
                _view.Runtime.SelectedItem = "Any";
                _view.RuntimeVersion.Text = string.Empty;

                _view.ApplicationBase.Text = null;
                _view.ConfigurationFile.Text = string.Empty;
                _view.PrivateBinPath.Text = string.Empty;
                _view.BinPathType.SelectedIndex = (int)BinPathType.Auto;

                _view.AssemblyList.SelectionList = new string[0];
                _view.AssemblyPath.Text = string.Empty;
            }
        }

        #region Changes Pertaining to Selected Config

        private void RunTime_SelectionChanged()
        {
            try
            {
                if (_selectedConfig != null)
                    _selectedConfig.RuntimeFramework = new RuntimeFramework(
                        (RuntimeType)Enum.Parse(typeof(RuntimeType), _view.Runtime.SelectedItem),
                        _selectedConfig.RuntimeFramework.Version);
            }
            catch(Exception ex)
            {
                // Note: Should not be called with an invalid value,
                // but we catch and report the error in any case
                _view.MessageDisplay.Error("Invalid Runtime: " + ex.Message);
            }
        }

        private void RuntimeVersion_TextValidated()
        {
            if (_selectedConfig != null)
            {
                try
                {
                    Version version = string.IsNullOrEmpty(_view.RuntimeVersion.Text)
                        ? new Version()
                        : new Version(_view.RuntimeVersion.Text);
                    _selectedConfig.RuntimeFramework = new RuntimeFramework(
                        _selectedConfig.RuntimeFramework.Runtime,
                        version);
                }
                catch (Exception ex)
                {
                    // User entered an bad value for the version
                    _view.MessageDisplay.Error("Invalid RuntimeVersion: " + ex.Message);
                }
            }
        }

        private void ApplicationBase_Validated()
        {
            if (_selectedConfig != null)
            {
                string basePath = null;

                if (_view.ApplicationBase.Text != String.Empty)
                {
                    if (!ValidateDirectoryPath("ApplicationBase", _view.ApplicationBase.Text))
                        return;

                    basePath = Path.Combine(_model.BasePath, _view.ApplicationBase.Text);
                    if (PathUtils.SamePath(_model.BasePath, basePath))
                        basePath = null;
                }

                _selectedConfig.BasePath = basePath;

                // TODO: Test what happens if we set it the same as project base
                //if (index.RelativeBasePath == null)
                //    _view.ApplicationBase.Text = string.Empty;
                //else
                //    _view.ApplicationBase.Text = index.RelativeBasePath;
            }
        }

        private void ConfigurationFile_Validated()
        {
            if (_selectedConfig != null)
            {
                string configFile = _view.ConfigurationFile.Text;

                if (configFile == string.Empty)
                    _selectedConfig.ConfigurationFile = null;
                else if (ValidateFilePath("DefaultConfigurationFile", configFile))
                {
                    if (configFile == Path.GetFileName(configFile))
                        _selectedConfig.ConfigurationFile = _view.ConfigurationFile.Text;
                    else
                        _view.MessageDisplay.Error("ConfigurationFile must be specified as a file name only - without directory path. The configuration file is always located in the application base directory.");
                }
            }
        }

        private void BinPathType_SelectionChanged()
        {
            if (_selectedConfig != null)
                _selectedConfig.BinPathType = (BinPathType)_view.BinPathType.SelectedIndex;
            _view.PrivateBinPath.Enabled = _view.BinPathType.SelectedIndex == (int)BinPathType.Manual;
        }

        private void PrivateBinPath_Validated()
        {
            if (_selectedConfig != null)
            {
                if (_view.PrivateBinPath.Text == string.Empty)
                    _selectedConfig.PrivateBinPath = null;
                else
                {
                    foreach (string dir in _view.PrivateBinPath.Text.Split(Path.PathSeparator))
                    {
                        if (!ValidateDirectoryPath("PrivateBinPath", dir))
                            return;
                        if (Path.IsPathRooted(dir))
                        {
                            _view.MessageDisplay.Error("Path " + dir + " is an absolute path. PrivateBinPath components must all be relative paths.");
                            return;
                        }
                    }

                    _selectedConfig.PrivateBinPath = _view.PrivateBinPath.Text;
                }
            }
        }

        private void AssemblyList_SelectionChanged()
        {
            if (_view.AssemblyList.SelectedIndex == -1)
            {
                _view.AssemblyPath.Text = null;
                _view.AddAssemblyCommand.Enabled = true;
                _view.RemoveAssemblyCommand.Enabled = false;
                _view.MoveUpAssemblyCommand.Enabled = false;
                _view.MoveDownAssemblyCommand.Enabled = false;
                _view.BrowseAssemblyPathCommand.Enabled = false;
            }
            else if (_selectedConfig != null)
            {
                _view.AssemblyPath.Text =
                    _selectedConfig.Assemblies[_view.AssemblyList.SelectedIndex];
                _view.AddAssemblyCommand.Enabled = true;
                _view.RemoveAssemblyCommand.Enabled = true;
                _view.MoveUpAssemblyCommand.Enabled = _view.AssemblyList.SelectedIndex > 0;
                _view.MoveDownAssemblyCommand.Enabled = _view.AssemblyList.SelectedIndex < _view.AssemblyList.SelectionList.Length-1;
                _view.BrowseAssemblyPathCommand.Enabled = true;
            }
        }

        private void AssemblyPath_Validated()
        {
            if (_selectedConfig != null && ValidateFilePath("AssemblyPath", _view.AssemblyPath.Text))
            {
                _selectedConfig.Assemblies[_view.AssemblyList.SelectedIndex] = _view.AssemblyPath.Text;
                SetAssemblyList();
            }
        }

        #endregion

        #endregion

        #region Model Change Events

        private void Project_Created()
        {
            _view.Visible = true;
            if (_model.Project.RootNode != null)
                LoadViewFromModel();
        }

        private void Project_Closed()
        {
            _view.Visible = false;
        }

        #endregion

        #region Helper Methods

        private void UpdateApplicationBase(string appbase)
        {
            string basePath = null;

            if (appbase != String.Empty)
            {
                basePath = Path.Combine(_model.BasePath, appbase);
                if (PathUtils.SamePath(_model.BasePath, basePath))
                    basePath = null;
            }

            IProjectConfig selectedConfig = _model.Configs[_view.ConfigList.SelectedIndex];
            _view.ApplicationBase.Text = selectedConfig.BasePath = basePath;

            // TODO: Test what happens if we set it the same as project base
            //if (index.RelativeBasePath == null)
            //    applicationBaseTextBox.Text = string.Empty;
            //else
            //    applicationBaseTextBox.Text = index.RelativeBasePath;
        }

        private void SetAssemblyList()
        {
            IProjectConfig config = _model.Configs[_view.ConfigList.SelectedIndex];
            var list = new string[config.Assemblies.Count];
            
            for (int i = 0; i < list.Length; i++)
                list[i] = config.Assemblies[i];

            _view.AssemblyList.SelectionList = list;
            if (list.Length > 0)
            {
                _view.AssemblyList.SelectedIndex = 0;
                _view.AssemblyPath.Text = list[0];
            }
            else
            {
                _view.AssemblyList.SelectedIndex = -1;
                _view.AssemblyPath.Text = string.Empty;
            }
        }

        private bool ValidateDirectoryPath(string property, string path)
        {
            try
            {
                new DirectoryInfo(path);
                return true;
            }
            catch (Exception ex)
            {
                _view.MessageDisplay.Error(string.Format("Invalid directory path for {0}: {1}", property, ex.Message));
                return false;
            }
        }

        private bool ValidateFilePath(string property, string path)
        {
            try
            {
                new FileInfo(path);
                return true;
            }
            catch (Exception ex)
            {
                _view.MessageDisplay.Error(string.Format("Invalid file path for {0}: {1}", property, ex.Message));
                return false;
            }
        }

        public string[] ConfigNames
        {
            get
            {
                ConfigList configs = _model.Configs;

                string[] configList = new string[configs.Count];
                for (int i = 0; i < configs.Count; i++)
                    configList[i] = configs[i].Name;

                return configList;
            }
        }
        
        #endregion
    }
}
