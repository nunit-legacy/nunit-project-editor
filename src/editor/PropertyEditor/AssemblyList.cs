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
	/// Represents a list of assemblies. It stores paths 
	/// that are added and fires an event whenevever it
	/// changes. All paths should be added as absolute paths.
	/// </summary>
	public class AssemblyList : IList<string>
	{
        private XmlNode configNode;

        public AssemblyList(XmlNode configNode)
        {
            this.configNode = configNode;
        }

        #region IList<string> Members

        public int IndexOf(string item)
        {
            int index = -1;
            foreach(string s in this)
            {
                ++index;
                if (s == item)
                    return index;
            }

            return -1;
        }

        public void Insert(int index, string assemblyPath)
        {
            XmlHelper.AddAttribute(
                XmlHelper.InsertElement(configNode, "assembly", index),
                "path",
                assemblyPath);
        }

        public void RemoveAt(int index)
        {
            configNode.RemoveChild(AssemblyNodes[index]);
        }

        public string this[int index]
        {
            get { return XmlHelper.GetAttribute(AssemblyNodes[index], "path"); }
            set { XmlHelper.SetAttribute(AssemblyNodes[index], "path", value); }
        }

        #endregion

        #region ICollection<string> Members

        public void Add(string assemblyPath)
        {
            XmlHelper.AddAttribute(
                XmlHelper.AddElement(configNode, "assembly"),
                "path",
                assemblyPath);
        }
        public void Clear()
        {
            configNode.RemoveAll();
        }

        public bool Contains(string item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            foreach(string item in this)
                array[arrayIndex++] = item;
        }

        public int Count
        {
            get { return AssemblyNodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(string assemblyPath)
        {
            int index = IndexOf(assemblyPath);

            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        #endregion

        #region IEnumerable<string> Members

        public IEnumerator<string> GetEnumerator()
        {
            var list = new List<string>();
            foreach (XmlNode node in AssemblyNodes)
                list.Add(XmlHelper.GetAttribute(node, "path"));
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (System.Collections.IEnumerator)this.GetEnumerator();
        }

        #endregion

        #region private Properties

        private XmlNodeList AssemblyNodes
        {
            get { return configNode.SelectNodes("assembly"); }
        }

        private XmlNode GetAssemblyNodes(int index)
        {
            return AssemblyNodes[index];
        }

        #endregion
    }
}
