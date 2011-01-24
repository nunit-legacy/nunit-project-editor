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
    public class ProjectModel : IProjectModel
    {
        private IProjectDocument doc;

        public ProjectModel(IProjectDocument doc)
        {
            this.doc = doc;
        }

        #region IProjectModel Members

        public IProjectDocument Document
        {
            get { return doc; }
        }

        public string ProjectPath
        {
            get { return doc.ProjectPath; }
            set { doc.ProjectPath = value; }
        }

        public string BasePath
        {
            get { return GetSettingsAttribute("appbase"); }
            set { SetSettingsAttribute("appbase", value); }
        }

        public string EffectiveBasePath
        {
            get { return this.BasePath ?? Path.GetDirectoryName(this.ProjectPath); }
        }

        public bool AutoConfig
        {
            get { return GetSettingsAttribute("autoconfig", false); }
            set { SetSettingsAttribute("autoconfig", value.ToString()); }
        }

        public string ActiveConfigName
        {
            get
            {
                string activeConfigName = GetSettingsAttribute("activeconfig");

                // In case the previous active config was removed
                if (!Configs.Contains(activeConfigName))
                    activeConfigName = null;

                // In case no active config is set or it was removed
                if (activeConfigName == null && Configs.Count > 0)
                    activeConfigName = Configs[0].Name;

                return activeConfigName;
            }
            set
            {
                int index = -1;

                if (value != null)
                {
                    for (int i = 0; i < Configs.Count; i++)
                    {
                        if (Configs[i].Name == value)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                if (index >= 0)
                    SetSettingsAttribute("activeconfig", value);
                else
                    RemoveSettingsAttribute("activeconfig");
            }
        }

        public ProcessModel ProcessModel
        {
            get { return GetSettingsAttribute("processModel", ProcessModel.Default); }
            set { SetSettingsAttribute("processModel", value); }
        }

        public DomainUsage DomainUsage
        {
            get { return GetSettingsAttribute("domainUsage", DomainUsage.Default); }
            set { SetSettingsAttribute("domainUsage", value); }
        }

        public ConfigList Configs
        {
            get { return new ConfigList(this); }
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

        #endregion

        #region Helper Properties and Methods

        /// <summary>
        /// The Settings node in the xml doc
        /// </summary>
        private XmlNode SettingsNode
        {
            get { return doc.RootNode.SelectSingleNode("Settings"); }
        }

        private string GetSettingsAttribute(string name)
        {
            if (SettingsNode == null)
                return null;

            return XmlHelper.GetAttribute(SettingsNode, name);
        }

        private bool GetSettingsAttribute(string name, bool defaultValue)
        {
            string val = GetSettingsAttribute(name);
            return val == null
                ? defaultValue
                : bool.Parse(val);
        }

        private T GetSettingsAttribute<T>(string name, T defaultValue)
        {
            if (SettingsNode == null)
                return defaultValue;

            return XmlHelper.GetAttributeAsEnum(SettingsNode, name, defaultValue);
        }

        private void SetSettingsAttribute(string name, object value)
        {
            if (SettingsNode == null)
            {
                XmlNode settingsNode = XmlHelper.InsertElement(doc.RootNode, "Settings", 0);
            }

            XmlHelper.SetAttribute(SettingsNode, name, value);
        }

        private void RemoveSettingsAttribute(string name)
        {
            if (SettingsNode != null)
                XmlHelper.RemoveAttribute(SettingsNode, name);
        }

        #endregion
    }
}
