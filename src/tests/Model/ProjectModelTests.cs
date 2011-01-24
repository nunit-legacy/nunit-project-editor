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

using System.IO;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests
{
    [TestFixture]
    public class ProjectModelTests
    {
        static readonly string xmlfile = "MyProject.nunit";

        private ProjectDocument doc;
        private ProjectModel project;
        private bool gotChangeNotice;

        [SetUp]
        public void SetUp()
        {
            doc = new ProjectDocument(xmlfile);
            project = new ProjectModel(doc);

            doc.ProjectChanged += OnProjectChange;
            gotChangeNotice = false;
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(xmlfile))
                File.Delete(xmlfile);
        }

        private void OnProjectChange()
        {
            gotChangeNotice = true;
        }

        [Test]
        public void LoadEmptyProject()
        {
            doc.LoadXml(NUnitProjectXml.EmptyProject);
            Assert.AreEqual(Path.GetFullPath(xmlfile), project.ProjectPath);
            Assert.AreEqual(0, project.Configs.Count);
        }

        [Test]
        public void LoadEmptyConfigs()
        {
            doc.LoadXml(NUnitProjectXml.EmptyConfigs);
            Assert.AreEqual(2, project.Configs.Count);
            Assert.AreEqual("Debug", project.Configs[0].Name);
            Assert.AreEqual("Release", project.Configs[1].Name);
        }

        [Test]
        public void LoadNormalProject()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            Assert.AreEqual(2, project.Configs.Count);

            IProjectConfig config1 = project.Configs[0];
            Assert.AreEqual(2, config1.Assemblies.Count);
            Assert.AreEqual(
                "assembly1.dll",
                config1.Assemblies[0]);
            Assert.AreEqual(
                "assembly2.dll",
                config1.Assemblies[1]);

            IProjectConfig config2 = project.Configs[1];
            Assert.AreEqual(2, config2.Assemblies.Count);
            Assert.AreEqual(
                "assembly1.dll",
                config2.Assemblies[0]);
            Assert.AreEqual(
                "assembly2.dll",
                config2.Assemblies[1]);
        }

        [Test]
        public void LoadProjectWithManualBinPath()
        {
            doc.LoadXml(NUnitProjectXml.ManualBinPathProject);
            Assert.AreEqual(1, project.Configs.Count);
            IProjectConfig config1 = project.Configs["Debug"];
            Assert.AreEqual("bin_path_value", config1.PrivateBinPath);
        }

        [Test]
        public void LoadProjectWithComplexSettings()
        {
            doc.LoadXml(NUnitProjectXml.ComplexSettingsProject);
            Assert.AreEqual("bin", project.BasePath);
            Assert.AreEqual(ProcessModel.Separate, project.ProcessModel);
            Assert.AreEqual(DomainUsage.Multiple, project.DomainUsage);

            Assert.AreEqual(2, project.Configs.Count);

            IProjectConfig config1 = project.Configs[0];
            Assert.AreEqual(
                "debug",
                config1.BasePath);
            Assert.AreEqual("v2.0", config1.RuntimeFramework.ToString());
            Assert.AreEqual("2.0", config1.RuntimeFramework.ClrVersion.ToString(2));
            Assert.AreEqual(2, config1.Assemblies.Count);
            Assert.AreEqual(
                "assembly1.dll",
                config1.Assemblies[0]);
            Assert.AreEqual(
                "assembly2.dll",
                config1.Assemblies[1]);

            IProjectConfig config2 = project.Configs[1];
            Assert.AreEqual(2, config2.Assemblies.Count);
            Assert.AreEqual(
                "release",
                config2.BasePath);
            Assert.AreEqual("v4.0", config2.RuntimeFramework.ToString());
            Assert.AreEqual("4.0", config2.RuntimeFramework.ClrVersion.ToString(2));
            Assert.AreEqual(
                "assembly1.dll",
                config2.Assemblies[0]);
            Assert.AreEqual(
                "assembly2.dll",
                config2.Assemblies[1]);
        }

        [Test]
        public void RenameConfigMakesProjectDirty()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.Configs[0].Name = "New";
            Assert.IsTrue(doc.HasUnsavedChanges);
        }

        [Test]
        public void RenameConfigFiresChangedEvent()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.Configs[0].Name = "New";
            Assert.IsTrue(gotChangeNotice);
        }

        [Test]
        public void RenamingActiveConfigChangesActiveConfigName()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.Configs[0].Name = "New";
            Assert.AreEqual("New", project.ActiveConfigName);
        }

        [Test]
        public void RemoveConfigMakesProjectDirty()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.Configs.Remove("Debug");
            Assert.IsTrue(doc.HasUnsavedChanges);
        }

        [Test]
        public void RemoveConfigFiresChangedEvent()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.Configs.Remove("Debug");
            Assert.IsTrue(gotChangeNotice);
        }

        [Test]
        public void RemovingActiveConfigChangesActiveConfigName()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.ActiveConfigName = "Debug";
            project.Configs.Remove("Debug");
            Assert.AreEqual("Release", project.ActiveConfigName);
        }

        [Test]
        public void SettingActiveConfigMakesProjectDirty()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.ActiveConfigName = "Release";
            Assert.IsTrue(doc.HasUnsavedChanges);
        }

        [Test]
        public void SettingActiveConfigFiresChangedEvent()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.ActiveConfigName = "Release";
            Assert.IsTrue(gotChangeNotice);
        }

        [Test]
        public void CanSetActiveConfig()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.ActiveConfigName = "Release";
            Assert.AreEqual("Release", project.ActiveConfigName);
        }

        [Test]
        public void CanAddAssemblies()
        {
            doc.LoadXml(NUnitProjectXml.EmptyConfigs);
            project.Configs["Debug"].Assemblies.Add(Path.GetFullPath(@"bin\debug\assembly1.dll"));
            project.Configs["Debug"].Assemblies.Add(Path.GetFullPath(@"bin\debug\assembly2.dll"));
            project.Configs["Release"].Assemblies.Add(Path.GetFullPath(@"bin\debug\assembly3.dll"));

            Assert.AreEqual(2, project.Configs.Count);
            Assert.AreEqual(2, project.Configs["Debug"].Assemblies.Count);
            Assert.AreEqual(1, project.Configs["Release"].Assemblies.Count);
        }

        [Test]
        public void AddingAssemblyFiresChangedEvent()
        {
            doc.LoadXml(NUnitProjectXml.EmptyConfigs);
            project.Configs["Debug"].Assemblies.Add("assembly1.dll");
            Assert.IsTrue(gotChangeNotice);
        }

        [Test]
        public void RemoveAssemblyFiresChangedEvent()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            project.Configs["Debug"].Assemblies.Remove("assembly1.dll");
            Assert.IsTrue(gotChangeNotice);
        }

        [Test]
        public void CanSaveAndReloadProject()
        {
            doc.LoadXml(NUnitProjectXml.NormalProject);
            doc.Save(xmlfile);
            Assert.IsTrue(File.Exists(xmlfile));

            ProjectDocument doc2 = new ProjectDocument(xmlfile);
            doc2.Load();
            ProjectModel project2 = new ProjectModel(doc2);

            Assert.AreEqual(2, project2.Configs.Count);

            Assert.AreEqual(2, project2.Configs[0].Assemblies.Count);
            Assert.AreEqual("assembly1.dll", project2.Configs[0].Assemblies[0]);
            Assert.AreEqual("assembly2.dll", project2.Configs[0].Assemblies[1]);

            Assert.AreEqual(2, project2.Configs[1].Assemblies.Count);
            Assert.AreEqual("assembly1.dll", project2.Configs[1].Assemblies[0]);
            Assert.AreEqual("assembly2.dll", project2.Configs[1].Assemblies[1]);
        }
    }
}
