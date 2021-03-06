﻿using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class AddConfigurationPresenterTests
    {
        IPropertyModel model;
        IAddConfigurationDialog dlg;
        AddConfigurationPresenter presenter;

        [SetUp]
        public void SetUp()
        {
            var doc = new ProjectModel();
            doc.LoadXml(NUnitProjectXml.NormalProject);
            model = new PropertyModel(doc);

            dlg = Substitute.For<IAddConfigurationDialog>();

            presenter = new AddConfigurationPresenter(model, dlg);
        }

        [Test]
        public void ConfigList_LoadFromModel_SetsViewCorrectly()
        {
            Assert.That(dlg.ConfigList, Is.EqualTo(new[] {"Debug", "Release"}));
        }

        [Test]
        public void AddButton_AddNewConfig_IsAddedToList()
        {
            dlg.ConfigToCreate.Returns("New");
            dlg.OkButton.Execute += Raise.Event<CommandHandler>();

            Assert.That(model.ConfigNames, Is.EqualTo(new[] {"Debug", "Release", "New"}));
        }

        [Test]
        public void AddButton_AddExistingConfig_FailsWithErrorMessage()
        {
            dlg.ConfigToCreate.Returns("Release");
            dlg.OkButton.Execute += Raise.Event<CommandHandler>();

            dlg.MessageDisplay.Received().Error("A configuration with that name already exists");
            Assert.That(model.ConfigNames, Is.EqualTo(new[] { "Debug", "Release" }));
        }

        [Test]
        public void ConfigToCopy_WhenNotSpecified_ConfigIsEmpty()
        {
            dlg.ConfigToCreate.Returns("New");
            dlg.ConfigToCopy.Returns("<none>");

            dlg.OkButton.Execute += Raise.Event<CommandHandler>();

            Assert.That(model.ConfigNames, Is.EqualTo(new[] { "Debug", "Release", "New" }));
            Assert.That(model.Configs[2].BasePath, Is.EqualTo(null));
            Assert.That(model.Configs[2].Assemblies.Count, Is.EqualTo(0));
        }

        [Test]
        public void ConfigToCopy_WhenSpecified_ConfigIsCopied()
        {
            dlg.ConfigToCreate.Returns("New");
            dlg.ConfigToCopy.Returns("Release");

            dlg.OkButton.Execute += Raise.Event<CommandHandler>();

            Assert.That(model.ConfigNames, Is.EqualTo(new[] { "Debug", "Release", "New" }));
            Assert.That(model.Configs[2].BasePath, Is.EqualTo("bin/release"));
            Assert.That(model.Configs[2].Assemblies.Count, Is.EqualTo(2));
        }
    }
}
