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
using System.Reflection;
using NUnit.Framework;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor.Tests
{
    public class PropertyPresenterTests
    {
        private IProjectModel model;
        private PropertyViewStub view;
        private IDialogManager dialogManager;
        private PropertyPresenter presenter;

        [SetUp]
        public void Initialize()
        {
            ProjectDocument doc = new ProjectDocument();
            doc.CreateNewProject();
            model = new ProjectModel(doc);
            model.Configs.Add("Debug");
            model.Configs.Add("Release");
            model.ActiveConfigName = "Release";
            view = new PropertyViewStub();
            dialogManager = new DialogManagerStub();

            presenter = new PropertyPresenter(model, view, dialogManager);
            presenter.LoadViewFromModel();
        }

        [Test]
        public void AllViewElementsAreInitializedCorrectly()
        {
            foreach (PropertyInfo prop in typeof(PropertyViewStub).GetProperties())
            {
                if (typeof(ViewElementStub).IsAssignableFrom(prop.PropertyType))
                {
                    ViewElementStub element = prop.GetValue(view, new object[0]) as ViewElementStub;
                    if (element == null)
                        Assert.Fail("{0} was not initialized", prop.Name);
                    //VerifyHasSubscribers(element);
                }
            }
        }

        [Test]
        public void PresenterSubscribesToRequiredViewEvents()
        {
            VerifyHasSubscribers(view.BrowseProjectBaseCommand);
            VerifyHasSubscribers(view.EditConfigsCommand);
            VerifyHasSubscribers(view.BrowseConfigBaseCommand);
            VerifyHasSubscribers(view.AddAssemblyCommand);
            VerifyHasSubscribers(view.RemoveAssemblyCommand);
            VerifyHasSubscribers(view.BrowseAssemblyPathCommand);

            VerifyHasSubscribers(view.ProjectBase);
            VerifyHasSubscribers(view.ProcessModel);
            VerifyHasSubscribers(view.DomainUsage);
            VerifyHasSubscribers(view.ApplicationBase);
            VerifyHasSubscribers(view.ConfigurationFile);
            VerifyHasSubscribers(view.Runtime);
            VerifyHasSubscribers(view.RuntimeVersion);
        }

        [Test]
        public void ProcessModelOptionsAreInitializedCorrectly()
        {
            Assert.That(view.ProcessModel.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single", "Separate", "Multiple" }));
        }

        [Test]
        public void RuntimesAreInitializedCorrectly()
        {
            Assert.That(view.Runtime.SelectionList, Is.EqualTo(
                new string[] { "Any", "Net", "Mono" }));
        }

        [Test]
        public void RuntimeVersionsAreInitializedCorrectly()
        {
            Assert.That(view.RuntimeVersion.SelectionList, Is.EqualTo(
                new string[] { "1.0.3705", "1.1.4322", "2.0.50727", "4.0.21006" }));
        }

        [Test]
        public void ActiveConfigIsSetCorrectly()
        {
            Assert.That(view.ActiveConfigName.Text, Is.EqualTo("Release"));
        }

        [Test]
        public void SelectedConfigIsSetCorrectly()
        {
            Assert.That(view.ConfigList.SelectedIndex, Is.EqualTo(0));
            Assert.That(view.ConfigList.SelectedItem, Is.EqualTo("Debug"));
        }

        [Test]
        public void ConfigListIsSetCorrectly()
        {
            Assert.That(view.ConfigList.SelectionList, Is.EqualTo(
                new string[] { "Debug", "Release" }));
        }

        [Test]
        public void WhenProjectModelIsChangedDomainUsageOptionsChanged()
        {
            view.ProcessModel.SelectedItem = "Single";
            view.ProcessModel.RaiseSelectionChangedEvent();
            Assert.That(view.DomainUsage.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single", "Multiple" }));

            view.ProcessModel.SelectedItem = "Multiple";
            view.ProcessModel.RaiseSelectionChangedEvent();
            Assert.That(view.DomainUsage.SelectionList, Is.EqualTo(
                new string[] { "Default", "Single" }));
        }

        [Test]
        public void ChangingProcessModelUpdatesProject()
        {
            view.ProcessModel.SelectedItem = "Multiple";
            view.ProcessModel.RaiseSelectionChangedEvent();
            Assert.That(model.ProcessModel, Is.EqualTo(ProcessModel.Multiple));
        }

        [Test]
        public void ChangingDomainUsageUpdatesProject()
        {
            view.DomainUsage.SelectedItem = "Multiple";
            view.DomainUsage.RaiseSelectionChangedEvent();
            Assert.That(model.DomainUsage, Is.EqualTo(DomainUsage.Multiple));
        }

        [Test]
        public void ChangingProjectBaseUpdatesProject()
        {
            view.ProjectBase.Text = "test.nunit";
            view.ProjectBase.FireValidatedEvent();
            Assert.That(model.BasePath, Is.EqualTo("test.nunit"));
        }

        [Test]
        public void ChangingRuntimeUpdatesProject()
        {
            model.Configs[0].RuntimeFramework = new RuntimeFramework(RuntimeType.Net, new Version("2.0.50727"));

            view.ConfigList.SelectedIndex = 0;
            view.Runtime.SelectedItem = "Mono";
            view.RuntimeVersion.SelectedItem = "1.1.4322";
            view.Runtime.RaiseSelectionChangedEvent();
            RuntimeFramework framework = model.Configs[0].RuntimeFramework;
            Assert.That(framework.Runtime, Is.EqualTo(RuntimeType.Mono));
            Assert.That(framework.ClrVersion, Is.EqualTo(new Version("1.1.4322")));
        }

        [Test]
        public void ChangingRuntimeVersionUpdatesProject()
        {
            view.Runtime.SelectedItem = "Mono";
            view.RuntimeVersion.SelectedItem = "1.1.4322";
            view.RuntimeVersion.RaiseSelectionChangedEvent();
            RuntimeFramework framework = model.Configs[0].RuntimeFramework;
            Assert.That(framework.Runtime, Is.EqualTo(RuntimeType.Mono));
            Assert.That(framework.ClrVersion, Is.EqualTo(new Version(1, 1, 4322)));
        }

        public void ChangingSelectedConfigUpdatesRuntime()
        {
        }

        private void VerifyHasSubscribers(ViewElementStub stub)
        {
            if (!stub.HasSubscribers)
                Assert.Fail("{0} has no subscribers", stub.Name);
        }
    }
}
