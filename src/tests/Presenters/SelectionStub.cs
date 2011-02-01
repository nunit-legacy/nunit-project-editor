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
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor.Tests
{
    public class SelectionStub : ISelectionList
    {
        private int selectedIndex = -1;
        private string[] selectionList;

        public SelectionStub(string name)
        {
            this.Name = name;
        }

        #region ISelectionList Members

        /// <summary>
        /// Gets or sets the currently selected item
        /// </summary>
        public string SelectedItem
        {
            get
            {
                return selectedIndex >= 0 && selectedIndex < SelectionList.Length
                    ? SelectionList[selectedIndex]
                    : null;
            }
            set
            {
                for (int index = 0; index < SelectionList.Length; index++)
                    if (value == SelectionList[index])
                    {
                        selectedIndex = index;
                        return;
                    }
            }
        }

        /// <summary>
        /// Gets or sets the contents of the selection list
        /// </summary>
        public string[] SelectionList 
        {
            get { return selectionList; }
            set
            {
                selectionList = value;
                if (selectionList.Length == 0)
                    SelectedIndex = -1;
                else
                    SelectedIndex = 0;
            }
        }

        #endregion

        #region ISelection Members

        /// <summary>
        /// Gets or sets the index of the currently selected item
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value < 0 || value >= SelectionList.Length ? -1 : value; }
        }


        /// <summary>
        /// Event raised when the selection is changed by the user
        /// </summary>
        public event ActionDelegate SelectionChanged;

        #endregion

        #region IViewElement Members

        /// <summary>
        /// Gets the name of the element in the view
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the enabled status of the element
        /// </summary>
        public bool Enabled { get; set; }

        #endregion 

        #region Public Methods

        public void RaiseSelectionChangedEvent()
        {
            if (SelectionChanged != null)
                SelectionChanged();
        }

        public bool HasSubscribers
        {
            get { return SelectionChanged != null; }
        }

        #endregion
    }
}
