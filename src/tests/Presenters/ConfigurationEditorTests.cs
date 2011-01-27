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
using NUnit.ProjectEditor.ViewElements;
using NSubstitute;

namespace NUnit.ProjectEditor.Tests
{
    public class ConfigurationEditorTests
    {
        private IConfigurationEditorView view;

        private ProjectModel model;
        private ConfigurationEditor editor;

        [SetUp]
        public void Initialize()
        {
            ProjectDocument doc = new ProjectDocument();
            doc.LoadXml(NUnitProjectXml.NormalProject);
            model = new ProjectModel(doc);

            view = Substitute.For<IConfigurationEditorView>();

            editor = new ConfigurationEditor(model, view);
        }

        [Test]
        public void PresenterSubscribesToRequiredEvents()
        {
            view.AddCommand.Received().Execute += editor.AddConfig;
            view.RemoveCommand.Received().Execute += editor.RemoveConfig;
            view.RenameCommand.Received().Execute += editor.RenameConfig;
            view.ActiveCommand.Received().Execute += editor.MakeActive;
            view.ConfigList.Received().SelectionChanged += editor.SelectedConfigChanged;
        }

        [Test]
        public void ConfigListIsCorrectlyInitialized()
        {
            Assert.That(view.ConfigList.SelectionList, Is.EqualTo( new[] { "Debug (active)", "Release" }));
        }

        [Test]
        public void ButtonsAreCorrectlyInitialized()
        {
            Assert.True(view.AddCommand.Enabled, "Add button should be enabled");
            Assert.True(view.RemoveCommand.Enabled, "Remove button should be enabled");
            Assert.True(view.RenameCommand.Enabled, "Rename button should be enabled");
            Assert.False(view.ActiveCommand.Enabled, "Active button should be disabled");
        }

        [Test]
        public void ClickingAddAddsNewConfig()
        {
            view.GetAddConfigData().Returns( new AddConfigData("New", "Release") );

            view.AddCommand.Execute += Raise.Event<CommandDelegate>();

            Assert.That(model.Configs.Count, Is.EqualTo(3));
            Assert.That(model.ConfigNames, Is.EqualTo(new[] { "Debug", "Release", "New" }));
        }

        [Test]
        public void ClickingRemoveRemovesConfig()
        {
            view.RemoveCommand.Execute += Raise.Event<CommandDelegate>();
            Assert.That(model.Configs.Count, Is.EqualTo(1));
            Assert.That(model.Configs[0].Name, Is.EqualTo("Release"));
        }

        private void RaiseExecute(ICommand command)
        {
            command.Execute += Raise.Event<CommandDelegate>();
        }

        [Test]
        public void ClickingRenamePerformsRename()
        {
            view.ConfigList.SelectedItem.Returns("Debug (active)");
            view.GetNewNameForRename("Debug").Returns("NewName");
            RaiseExecute(view.RenameCommand);
            Assert.That(model.Configs[0].Name, Is.EqualTo("NewName"));
        }

        [Test]
        public void ClickingActiveMakesConfigActive()
        {
            view.ConfigList.SelectedItem = "Release";
            RaiseExecute(view.ActiveCommand);
            Assert.That(model.ActiveConfigName, Is.EqualTo("Release"));
        }
    }
}
