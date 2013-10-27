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
using System.Xml;

namespace NUnit.ProjectEditor
{
    public class PropertyModel : IPropertyModel
    {
        private IProjectModel project;

        public PropertyModel(IProjectModel project)
        {
            this.project = project;
        }

        #region IPropertyModel Members

        public IProjectModel Project
        {
            get { return project; }
        }

        public string ProjectPath
        {
            get { return project.ProjectPath; }
            set { project.ProjectPath = value; }
        }

        /// <summary>
        /// BasePath is the base as it appears in the document
        /// and may be null if there is no setting.
        /// </summary>
        public string BasePath
        {
            get { return StandardizeSeparators(project.GetSettingsAttribute("appbase")); }
            set { project.SetSettingsAttribute("appbase", StandardizeSeparators(value)); }
        }

        /// <summary>
        /// EffectiveBasePath uses the BasePath if present and otherwise
        /// defaults to the directory part of the ProjectPath.
        /// </summary>
        public string EffectiveBasePath
        {
            get 
            { 
                return this.BasePath == null
                    ? Path.GetDirectoryName(this.ProjectPath)
                    : Path.Combine(
                        Path.GetDirectoryName(this.ProjectPath),
                        this.BasePath); 
            }
        }

        public string ActiveConfigName
        {
            get { return project.GetSettingsAttribute("activeconfig"); }
            set { project.SetSettingsAttribute("activeconfig", value); }
        }

        public string ProcessModel
        {
            get { return project.GetSettingsAttribute("processModel") ?? "Default"; }
            set { project.SetSettingsAttribute("processModel", value.ToString()); }
        }

        public string DomainUsage
        {
            get { return project.GetSettingsAttribute("domainUsage") ?? "Default"; }
            set { project.SetSettingsAttribute("domainUsage", value.ToString()); }
        }

        public ConfigList Configs
        {
            get { return new ConfigList(Project.RootNode); }
        }

        public string[] ConfigNames
        {
            get
            {
                string[] configList = new string[Configs.Count];
                for (int i = 0; i < Configs.Count; i++)
                    configList[i] = Configs[i].Name;

                return configList;
            }
        }

        public IProjectConfig AddConfig(string name)
        {
            XmlNode configNode = XmlHelper.AddElement(project.RootNode, "Config");
            XmlHelper.AddAttribute(configNode, "name", name);

            return new ProjectConfig(configNode);
        }

        public void RemoveConfigAt(int index)
        {
            bool itWasActive = ActiveConfigName == Configs[index].Name;

            project.RootNode.RemoveChild(project.ConfigNodes[index]);
            
            if (itWasActive)
                project.RemoveSettingsAttribute("activeconfig");
        }

        public void RemoveConfig(string name)
        {
            int index = IndexOf(name);
            if (index >= 0)
            {
                RemoveConfigAt(index);
            }
        }

        #endregion

        #region Helper Properties and Methods

        private int IndexOf(string name)
        {
            for (int index = 0; index < project.ConfigNodes.Count; index++)
            {
                if (XmlHelper.GetAttribute(project.ConfigNodes[index], "name") == name)
                    return index;
            }

            return -1;
        }

        private string StandardizeSeparators(string path)
        {
            return path != null
                ? path.Replace('\\', '/')
                : null;
        }

        #endregion
    }
}
