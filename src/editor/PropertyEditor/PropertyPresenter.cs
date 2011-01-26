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
    /// The ProjectPresenter handles presentation of the doc as
    /// a set of properties, which the ProjectView is expected to
    /// display.
    /// </summary>
    public class PropertyPresenter
    {
        private IProjectModel model;
        private IProjectConfig selectedConfig;
        private IPropertyView view;

        public PropertyPresenter(IProjectModel model, IPropertyView view)
        {
            this.model = model;
            this.view = view;

            SetProcessModelOptions();
            SetDomainUsageOptions();
            SetRuntimeOptions();
            SetRuntimeVersionOptions();

            WireUpEvents(view);
        }

        public void LoadViewFromModel()
        {
            view.Visible = true;

            view.ProjectPath.Text = model.ProjectPath;
            view.ProjectBase.Text = model.BasePath == null ? model.ProjectPath : model.BasePath;
            view.ActiveConfigName.Text = model.ActiveConfigName;

            view.ProcessModel.SelectedItem = model.ProcessModel.ToString();
            view.DomainUsage.SelectedItem = model.DomainUsage.ToString();

            view.ConfigList.SelectionList = model.ConfigNames;
            if (model.ConfigNames.Length > 0)
            {
                view.ConfigList.SelectedIndex = 0;
                selectedConfig = model.Configs[0];
            }
            else
                view.ConfigList.SelectedIndex = -1;
        }

        private void WireUpEvents(IPropertyView view)
        {
            view.BrowseProjectBaseCommand.Execute += delegate
            {
                string message = "Select ApplicationBase for the model as a whole.";
                string projectBase = view.DialogManager.GetFolderPath(message, view.ProjectBase.Text);
                if (projectBase != null && projectBase != model.BasePath)
                    view.ProjectBase.Text = model.BasePath = projectBase;
            };

            view.EditConfigsCommand.Execute += delegate
            {
                using (ConfigurationEditorView editorView = new ConfigurationEditorView())
                {
                    new ConfigurationEditor(model, editorView);
                    editorView.ShowDialog();
                }

                string selectedConfig = view.ConfigList.SelectedItem;
                string[] configs = model.ConfigNames;

                view.ConfigList.SelectionList = configs;

                if (configs.Length > 0)
                {
                    view.ConfigList.SelectedIndex = 0;
                    foreach (string config in configs)
                    {
                        if (config == selectedConfig)
                            view.ConfigList.SelectedItem = config;
                    }
                }

                view.ActiveConfigName.Text = model.ActiveConfigName;
            };

            view.BrowseConfigBaseCommand.Execute += delegate
            {
                string message = string.Format(
                    "Select ApplicationBase for the {0} configuration, if different from the model as a whole.",
                    model.Configs[view.ConfigList.SelectedIndex].Name);
                string initialFolder = view.ApplicationBase.Text;
                if (initialFolder == string.Empty)
                    initialFolder = view.ProjectBase.Text;

                string appbase = view.DialogManager.GetFolderPath(message, initialFolder);
                if (appbase != null && appbase != view.ApplicationBase.Text)
                    UpdateApplicationBase(appbase);
            };

            view.AddAssemblyCommand.Execute += delegate
            {
                string assemblyPath = view.DialogManager.GetFileOpenPath(
                    "Select Assembly",
                    "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*",
                    view.AssemblyPath.Text);

                if (assemblyPath != null)
                {
                    assemblyPath = PathUtils.RelativePath(selectedConfig.EffectiveBasePath, assemblyPath);
                    selectedConfig.Assemblies.Add(assemblyPath);
                    SetAssemblyList();
                }
            };

            view.RemoveAssemblyCommand.Execute += delegate
            {
                string question = string.Format("Remove {0} from project?", view.AssemblyList.SelectedItem);
                if (view.DialogManager.AskYesNoQuestion(question))
                {
                    selectedConfig.Assemblies.Remove(view.AssemblyList.SelectedItem);
                    SetAssemblyList();
                }
            };

            view.BrowseAssemblyPathCommand.Execute += delegate
            {
                string assemblyPath = view.DialogManager.GetFileOpenPath(
                    "Select Assembly",
                    "Assemblies (*.dll,*.exe)|*.dll;*.exe|All Files (*.*)|*.*",
                    view.AssemblyPath.Text);

                if (assemblyPath != null)
                {
                    selectedConfig.Assemblies[view.AssemblyList.SelectedIndex] = assemblyPath;
                    SetAssemblyList();
                }
            };

            view.ProjectBase.Validated += delegate
            {
                string projectBase = view.ProjectBase.Text;

                if (projectBase == string.Empty)
                    view.ProjectBase.Text = projectBase = Path.GetDirectoryName(model.ProjectPath);

                if (ValidateDirectoryPath("ProjectBase", projectBase))
                    model.BasePath = projectBase;
            };

            view.ProcessModel.SelectionChanged += delegate
            {
                model.ProcessModel = (ProcessModel)Enum.Parse(typeof(ProcessModel), view.ProcessModel.SelectedItem);
                SetDomainUsageOptions();
            };

            view.DomainUsage.SelectionChanged += delegate
            {
                model.DomainUsage = (DomainUsage)Enum.Parse(typeof(DomainUsage), view.DomainUsage.SelectedItem);
            };

            view.Runtime.SelectionChanged += delegate
            {
                if (selectedConfig != null)
                {
                    RuntimeType runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), view.Runtime.SelectedItem);

                    try
                    {
                        Version version = new Version(view.RuntimeVersion.SelectedItem);
                        selectedConfig.RuntimeFramework = new RuntimeFramework(runtime, version);
                    }
                    catch (Exception ex)
                    {
                        view.DialogManager.DisplayError("Invalid RuntimeVersion: " + ex.Message);
                    }
                }
            };

            view.RuntimeVersion.SelectionChanged += delegate
            {
                if (selectedConfig != null)
                {
                    RuntimeType runtime = (RuntimeType)Enum.Parse(typeof(RuntimeType), view.Runtime.SelectedItem);

                    try
                    {
                        Version version = new Version(view.RuntimeVersion.SelectedItem);
                        selectedConfig.RuntimeFramework = new RuntimeFramework(runtime, version);
                    }
                    catch (Exception ex)
                    {
                        view.DialogManager.DisplayError("Invalid RuntimeVersion: " + ex.Message);
                    }
                }
            };

            view.ApplicationBase.Validated += delegate
            {
                if (selectedConfig != null)
                {
                    string basePath = null;

                    if (view.ApplicationBase.Text != String.Empty)
                    {
                        if (!ValidateDirectoryPath("ApplicationBase", view.ApplicationBase.Text))
                            return;

                        basePath = Path.Combine(model.BasePath, view.ApplicationBase.Text);
                        if (PathUtils.SamePath(model.BasePath, basePath))
                            basePath = null;
                    }

                    selectedConfig.BasePath = basePath;

                    // TODO: Test what happens if we set it the same as doc base
                    //if (index.RelativeBasePath == null)
                    //    view.ApplicationBase.Text = string.Empty;
                    //else
                    //    view.ApplicationBase.Text = index.RelativeBasePath;
                }
            };

            view.ConfigurationFile.Validated += delegate
            {
                if (selectedConfig != null)
                {
                    string configFile = view.ConfigurationFile.Text;

                    if (configFile == string.Empty)
                        selectedConfig.ConfigurationFile = null;
                    else if (ValidateFilePath("DefaultConfigurationFile", configFile))
                    {
                        if (configFile == Path.GetFileName(configFile))
                            selectedConfig.ConfigurationFile = view.ConfigurationFile.Text;
                        else
                            view.DialogManager.DisplayError("ConfigurationFile must be specified as a file name only - without directory path. The configuration file is always located in the application base directory.");
                    }
                }
            };

            view.ConfigList.SelectionChanged += delegate
            {
                IProjectConfig selectedConfig = view.ConfigList.SelectedIndex >= 0
                    ? model.Configs[view.ConfigList.SelectedIndex]
                    : null;

                if (selectedConfig != null)
                {
                    RuntimeFramework framework = selectedConfig.RuntimeFramework;
                    if (framework == null)
                    {
                        view.Runtime.SelectedItem = RuntimeType.Any.ToString();
                    }
                    else
                    {
                        view.Runtime.SelectedItem = framework.Runtime.ToString();
                        view.RuntimeVersion.SelectedItem = framework.ClrVersion.ToString();
                    }

                    view.ApplicationBase.Text = selectedConfig.RelativeBasePath;
                    view.ConfigurationFile.Text = selectedConfig.ConfigurationFile;
                    view.BinPathType.SelectedIndex = (int)selectedConfig.BinPathType;
                    if (selectedConfig.BinPathType == BinPathType.Manual)
                        view.PrivateBinPath.Text = selectedConfig.PrivateBinPath;
                    else
                        view.PrivateBinPath.Text = string.Empty;

                    SetAssemblyList();
                }
                else
                {
                    view.ApplicationBase.Text = null;
                    view.ConfigurationFile.Text = null;
                    view.PrivateBinPath.Text = string.Empty;
                    view.BinPathType.SelectedIndex = (int)BinPathType.Auto;

                    view.AssemblyList.SelectionList = new string[0];
                    view.AssemblyPath.Text = null;
                }
            };

            view.BinPathType.SelectionChanged += delegate
            {
                if (selectedConfig != null)
                    selectedConfig.BinPathType = (BinPathType)view.BinPathType.SelectedIndex;
                view.PrivateBinPath.Enabled = view.BinPathType.SelectedIndex == (int)BinPathType.Manual;
            };

            view.PrivateBinPath.Validated += delegate
            {
                if (selectedConfig != null)
                {
                    if (view.PrivateBinPath.Text == string.Empty)
                        selectedConfig.PrivateBinPath = null;
                    else
                    {
                        foreach (string dir in view.PrivateBinPath.Text.Split(Path.PathSeparator))
                        {
                            if (!ValidateDirectoryPath("PrivateBinPath", dir))
                                return;
                            if (Path.IsPathRooted(dir))
                            {
                                view.DialogManager.DisplayError("Path " + dir + " is an absolute path. PrivateBinPath components must all be relative paths.");
                                return;
                            }
                        }

                        selectedConfig.PrivateBinPath = view.PrivateBinPath.Text;
                    }
                }
            };

            view.AssemblyList.SelectionChanged += delegate
            {
                if (view.AssemblyList.SelectedIndex == -1)
                {
                    view.AssemblyPath.Text = null;
                    view.RemoveAssemblyCommand.Enabled = false;
                    view.BrowseAssemblyPathCommand.Enabled = false;
                }
                else if (selectedConfig != null)
                {
                    view.AssemblyPath.Text =
                        selectedConfig.Assemblies[view.AssemblyList.SelectedIndex];
                    view.RemoveAssemblyCommand.Enabled = true;
                    view.BrowseAssemblyPathCommand.Enabled = true;
                }
            };

            view.AssemblyPath.Validated += delegate
            {
                if (selectedConfig != null && ValidateFilePath("AssemblyPath", view.AssemblyPath.Text))
                {
                    selectedConfig.Assemblies[view.AssemblyList.SelectedIndex] = view.AssemblyPath.Text;
                    SetAssemblyList();
                }
            };
        }

        private void UpdateApplicationBase(string appbase)
        {
            string basePath = null;

            if (appbase != String.Empty)
            {
                basePath = Path.Combine(model.BasePath, appbase);
                if (PathUtils.SamePath(model.BasePath, basePath))
                    basePath = null;
            }

            IProjectConfig selectedConfig = model.Configs[view.ConfigList.SelectedIndex];
            view.ApplicationBase.Text = selectedConfig.BasePath = basePath;

            // TODO: Test what happens if we set it the same as doc base
            //if (index.RelativeBasePath == null)
            //    applicationBaseTextBox.Text = string.Empty;
            //else
            //    applicationBaseTextBox.Text = index.RelativeBasePath;
        }

        #region Helper Methods

        private void SetProcessModelOptions()
        {
            view.ProcessModel.SelectionList = Enum.GetNames(typeof(ProcessModel));
        }

        private void SetDomainUsageOptions()
        {
            view.DomainUsage.SelectionList = view.ProcessModel.SelectedItem == ProcessModel.Multiple.ToString()
                ? new string[] { "Default", "Single" }
                : new string[] { "Default", "Single", "Multiple" };
        }

        private void SetRuntimeOptions()
        {
            view.Runtime.SelectionList = new string[] { "Any", "Net", "Mono" };
        }

        private void SetRuntimeVersionOptions()
        {
            string[] versions = new string[RuntimeFramework.KnownClrVersions.Length];

            for (int i = 0; i < RuntimeFramework.KnownClrVersions.Length; i++)
                versions[i] = RuntimeFramework.KnownClrVersions[i].ToString(3);

            view.RuntimeVersion.SelectionList = versions;
        }

        private void SetAssemblyList()
        {
            var list = model.Configs[view.ConfigList.SelectedIndex].Assemblies;
            view.SetAssemblyList(list);

            if (list.Count == 0)
                view.AssemblyPath.Text = "";
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
                view.DialogManager.DisplayError(string.Format("Invalid directory path for {0}: {1}", property, ex.Message));
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
                view.DialogManager.DisplayError(string.Format("Invalid file path for {0}: {1}", property, ex.Message));
                return false;
            }
        }

        #endregion
    }
}
