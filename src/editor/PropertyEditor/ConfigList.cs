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
using System.Xml;

namespace NUnit.ProjectEditor
{
	/// <summary>
	/// Summary description for ConfigList.
	/// </summary>
	public class ConfigList : IEnumerable<IProjectConfig>
	{
        private XmlNode _projectNode;

        public ConfigList(XmlNode projectNode)
        {
            _projectNode = projectNode;
        }

		#region Properties

        public int Count
        {
            get { return ConfigNodes.Count; }
        }

		public IProjectConfig this[int index]
		{
            get { return new ProjectConfig(ConfigNodes[index]); }
		}

        public IProjectConfig this[string name]
        {
            get
            {
                int index = IndexOf(name);
                return index >= 0 ? this[index] : null;
            }
        }

        private XmlNodeList ConfigNodes
        {
            get { return _projectNode.SelectNodes("Config"); }
        }

        private XmlNode SettingsNode
        {
            get { return _projectNode.SelectSingleNode("Settings"); }
        }

		#endregion

		#region Methods

        private int IndexOf(string name)
        {
            for (int index = 0; index < ConfigNodes.Count; index++)
            {
                if (XmlHelper.GetAttribute(ConfigNodes[index], "name") == name)
                    return index;
            }

            return -1;
        }

		#endregion

        #region IEnumerable<IProjectConfig> Members

        public IEnumerator<IProjectConfig> GetEnumerator()
        {
            foreach (XmlNode node in ConfigNodes)
                yield return new ProjectConfig(node);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
