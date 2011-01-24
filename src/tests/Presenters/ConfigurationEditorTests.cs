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
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests
{
    public class ConfigurationEditorTests
    {
        private ConfigurationEditorViewStub view;
        private ProjectModel model;
        private ConfigurationEditor editor;

        [SetUp]
        public void Initialize()
        {
            ProjectDocument doc = new ProjectDocument();
            doc.LoadXml(NUnitProjectXml.NormalProject);
            view = new ConfigurationEditorViewStub();
            model = new ProjectModel(doc);
            editor = new ConfigurationEditor(model, view);
        }

        [Test]
        public void PresenterSubscribesToRequiredEvents()
        {
            VerifyHasSubscribers(view.AddCommand);
            VerifyHasSubscribers(view.RemoveCommand);
            VerifyHasSubscribers(view.RenameCommand);
            VerifyHasSubscribers(view.ActiveCommand);
            VerifyHasSubscribers(view.ConfigList);
        }

        [Test]
        public void ConfigListIsCorrectlyInitialized()
        {
            Assert.AreEqual(
                new string[] { "Debug (active)", "Release" },
                view.ConfigList.SelectionList);
            Assert.AreEqual(0, view.ConfigList.SelectedIndex);
            Assert.AreEqual("Debug (active)", view.ConfigList.SelectedItem);
        }

        [Test]
        public void ButtonsAreCorrectlyInitialized()
        {
            Assert.True(view.AddCommand.Enabled, "Add button should be enabled");
            Assert.True(view.RemoveCommand.Enabled, "Remove button should be enabled");
            Assert.True(view.RenameCommand.Enabled, "Rename button should be enabled");
            Assert.False(view.ActiveCommand.Enabled, "Active button should be disabled");
        }

        private void VerifyHasSubscribers(IViewElement element)
        {
            ViewElementStub stub = (ViewElementStub)element;
            if (!stub.HasSubscribers)
                Assert.Fail("{0} has no subscribers", stub.Name);
        }
    }
}
