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
    public class SelectionStub : ViewElementStub, ISelection, ISelectionList
    {
        private int selectedIndex = -1;
        private string selectedItem;

        public SelectionStub(string name) : base(name) { }

        public int SelectedIndex 
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (selectedIndex < 0 || selectedIndex >= SelectionList.Length)
                    selectedIndex = -1;
                else
                    selectedItem = SelectionList[selectedIndex];
            }
        }

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

                throw new ArgumentException("Trying to set SelectedItem to non-existent value", "value");
            }
        }

        public string[] SelectionList { get; set; }

        public event ActionDelegate SelectionChanged;

        public void RaiseSelectionChangedEvent()
        {
            if (SelectionChanged != null)
                SelectionChanged();
        }

        public override bool HasSubscribers
        {
            get { return SelectionChanged != null; }
        }
    }
}
