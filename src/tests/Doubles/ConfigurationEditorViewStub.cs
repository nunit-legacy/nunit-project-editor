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
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor.Tests
{
    public class ConfigurationEditorViewStub : IConfigurationEditorView
    {
        public ConfigurationEditorViewStub()
        {
            AddCommand = new CommandStub("Add");
            RemoveCommand = new CommandStub("Remove");
            RenameCommand = new CommandStub("Rename");
            ActiveCommand = new CommandStub("Active");

            ConfigList = new SelectionStub("ConfigList");
        }

        #region IConfigurationEditorView Members

        public event ActionDelegate SelectedConfigChanged;

        public ICommand AddCommand { get; private set; }
        public ICommand RenameCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }
        public ICommand ActiveCommand { get; private set; }

        public ISelectionList ConfigList { get; private set; }

        public string GetNewNameForRename(string oldName)
        {
            throw new NotImplementedException();
        }

        public bool GetAddConfigData(ref AddConfigData data)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void FireSelectedConfigChangedEvent()
        {
            if (SelectedConfigChanged != null)
                SelectedConfigChanged();
        }
    }
}
