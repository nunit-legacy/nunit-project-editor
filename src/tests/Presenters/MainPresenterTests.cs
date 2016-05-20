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
using NSubstitute;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    [TestFixture]
    public class MainPresenterTests
    {
        private static readonly string GOOD_PROJECT = Path.Combine(TestContext.CurrentContext.TestDirectory, "NUnitTests.nunit");
        private static readonly string BAD_PROJECT = Path.Combine(TestContext.CurrentContext.TestDirectory, "BadProject.nunit");
        private static readonly string NONEXISTENT_PROJECT = "NonExistent.nunit";

        private IMainView view;
        private IProjectModel model;

        [SetUp]
        public void Initialize()
        {
            view = Substitute.For<IMainView>();
            model = new ProjectModel();
            new MainPresenter(model, view);
        }

        [Test]
        public void CloseProject_OnLoad_IsDisabled()
        {
            Assert.False(view.CloseProjectCommand.Enabled);
        }

        [Test]
        public void CloseProject_AfterCreatingNewProject_IsEnabled()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.True(view.CloseProjectCommand.Enabled);
        }

        [Test]
        public void CloseProject_AfterOpeningGoodProject_IsEnabled()
        {
            view.DialogManager.GetFileOpenPath("", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.True(view.CloseProjectCommand.Enabled);
        }

        [Test]
        public void NewProject_OnLoad_IsEnabled()
        {
            Assert.True(view.NewProjectCommand.Enabled);
        }

        [Test]
        public void NewProject_WhenClicked_CreatesNewProject()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.IsNotNull(model.RootNode);
            Assert.That(model.Name, Does.Match("Project\\d"));
        }

        [Test]
        public void OpenProject_OnLoad_IsEnabled()
        {
            Assert.True(view.OpenProjectCommand.Enabled);
        }

        [Test]
        public void OpenProject_WhenClickedAndProjectIsValid_OpensProject()
        {
            view.DialogManager.GetFileOpenPath("Open", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.NotNull(model.XmlText);
            Assert.NotNull(model.RootNode);
            Assert.AreEqual("NUnitTests", model.Name);
        }

        [Test]
        public void OpenProject_WhenClickedAndProjectXmlIsNotValid_OpensProject()
        {
            view.DialogManager.GetFileOpenPath("Open", "", "").ReturnsForAnyArgs(BAD_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.NotNull(model.XmlText);
            Assert.Null(model.RootNode);
            Assert.AreEqual("BadProject", model.Name);

            Assert.That(view.SelectedView, Is.EqualTo(EditorView.XmlView));
        }

        [Test]
        public void OpenProject_WhenClickedAndProjectDoesNotExist_DisplaysError()
        {
            view.DialogManager.GetFileOpenPath("Open", "", "").ReturnsForAnyArgs(NONEXISTENT_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandHandler>();

            view.MessageDisplay.Received().Error(Arg.Is((string x) => x.StartsWith("Could not find file")));

            Assert.Null(model.XmlText);
            Assert.Null(model.RootNode);
        }

        [Test]
        public void SaveProject_OnLoad_IsDisabled()
        {
            Assert.False(view.SaveProjectCommand.Enabled);
        }

        [Test]
        public void SaveProject_AfterCreatingNewProject_IsEnabled()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.True(view.SaveProjectCommand.Enabled);
        }

        [Test]
        public void SaveProject_AfterOpeningGoodProject_IsEnabled()
        {
            view.DialogManager.GetFileOpenPath("", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.True(view.SaveProjectCommand.Enabled);
        }

        [Test]
        public void SaveProjectAs_OnLoad_IsDisabled()
        {
            Assert.False(view.SaveProjectAsCommand.Enabled);
        }

        [Test]
        public void SaveProjectAs_AfterCreatingNewProject_IsEnabled()
        {
            view.NewProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.True(view.SaveProjectAsCommand.Enabled);
        }

        [Test]
        public void SaveProjectAs_AfterOpeningGoodProject_IsEnabled()
        {
            view.DialogManager.GetFileOpenPath("", "", "").ReturnsForAnyArgs(GOOD_PROJECT);
            view.OpenProjectCommand.Execute += Raise.Event<CommandHandler>();

            Assert.True(view.SaveProjectAsCommand.Enabled);
        }
    }
}
