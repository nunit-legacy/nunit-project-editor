using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class RenameConfigurationPresenterTests
    {
        IProjectModel model;
        IRenameConfigurationDialog dlg;
        RenameConfigurationPresenter presenter;

        [SetUp]
        public void Initialize()
        {
            model = Substitute.For<IProjectModel>();
            dlg = Substitute.For<IRenameConfigurationDialog>();
            presenter = new RenameConfigurationPresenter(model, dlg, "Debug");
        }

        [Test]
        public void ConfigurationNameIsInitiallySetToOriginalName()
        {
            Assert.AreEqual("Debug", dlg.ConfigurationName.Text);
        }

        [Test]
        public void ConfigurationNameIsInitiallySelected()
        {
            dlg.ConfigurationName.Received().Select(0,5);
        }

        [Test]
        public void OkButtonIsInitiallyDisabled()
        {
            Assert.False(dlg.OkButton.Enabled);
        }

        [Test]
        public void SettingNewConfigNameEnablesOK()
        {
            dlg.ConfigurationName.Text = "New";
            dlg.ConfigurationName.Changed += Raise.Event<ActionDelegate>();

            Assert.True(dlg.OkButton.Enabled);
        }

        [Test]
        public void SettingSameConfigNameDisablesOK()
        {
            dlg.ConfigurationName.Text = "Debug";
            dlg.ConfigurationName.Changed += Raise.Event<ActionDelegate>();

            Assert.False(dlg.OkButton.Enabled);
        }

        [Test]
        public void ClearingNameDisablesOK()
        {
            dlg.ConfigurationName.Text = string.Empty;
            dlg.ConfigurationName.Changed += Raise.Event<ActionDelegate>();

            Assert.False(dlg.OkButton.Enabled);
        }

        //[Test]
        //public void ClickingOKPerformsRename()
        //{
        //    dlg.ConfigurationName.Text = "New";
        //    dlg.OkButton.Execute += Raise.Event<CommandDelegate>();
        //}
    }
}
