using System;
using NSubstitute;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class AddConfigurationPresenterTests
    {
        IProjectModel model;
        IAddConfigurationDialog dlg;
        AddConfigurationPresenter presenter;

        [SetUp]
        public void SetUp()
        {
            var doc = new ProjectDocument();
            doc.LoadXml(NUnitProjectXml.EmptyConfigs);
            model = new ProjectModel(doc);

            dlg = Substitute.For<IAddConfigurationDialog>();

            presenter = new AddConfigurationPresenter(model, dlg);
        }

        [Test]
        public void ConfigListIsInitializedCorrectly()
        {
            Assert.That(dlg.ConfigList, Is.EqualTo(new[] {"Debug", "Release"}));
        }

        [Test]
        public void CanAddNewConfig()
        {
            dlg.ConfigToCreate.Returns("New");
            dlg.OkButton.Execute += Raise.Event<CommandDelegate>();

            Assert.That(model.ConfigNames, Is.EqualTo(new[] {"Debug", "Release", "New"}));
        }

        [Test]
        public void NewConfigCannotMatchNameOfExistingConfig()
        {
            dlg.ConfigToCreate.Returns("Release");
            dlg.OkButton.Execute += Raise.Event<CommandDelegate>();

            dlg.MessageDisplay.Received().Error("A configuration with that name already exists");
        }
    }
}
