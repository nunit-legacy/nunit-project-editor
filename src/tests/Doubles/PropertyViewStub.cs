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
using System.Reflection;
using NUnit.ProjectEditor.ViewElements;

namespace NUnit.ProjectEditor.Tests
{
    public class PropertyViewStub : IPropertyView
    {
        public PropertyViewStub()
        {
            this.DialogManager = new DialogManagerStub();

            this.BrowseProjectBaseCommand = new CommandStub("BrowseProjectBase");
            this.EditConfigsCommand = new CommandStub("EditConfigs");
            this.BrowseConfigBaseCommand = new CommandStub("BrowseConfigBase");
            this.AddAssemblyCommand = new CommandStub("AddAssembly");
            this.RemoveAssemblyCommand = new CommandStub("RemoveAssembly");
            this.BrowseAssemblyPathCommand = new CommandStub("BrowseAssemblyPath");

            this.ProjectPath = new ValidatedStub("ProjectPath");
            this.ProjectBase = new ValidatedStub("ProjectBase");
            this.ProcessModel = new SelectionStub("ProcessModel");
            this.DomainUsage = new SelectionStub("DomainUsage");

            this.Runtime = new SelectionStub("Runtime");
            this.RuntimeVersion = new SelectionStub("RuntimeVersion");
            this.ActiveConfigName = new ValidatedStub("ActiveConfigName");

            this.ConfigList = new SelectionStub("ConfigList");

            this.BinPathType = new SelectionStub("BinPathType");
            this.ApplicationBase = new ValidatedStub("ApplicationBase");
            ConfigurationFile = new ValidatedStub("ConfigurationFile");
            this.PrivateBinPath = new ValidatedStub("PrivateBinPath");
            this.AssemblyPath = new ValidatedStub("AssemblyPath");
            this.AssemblyList = new SelectionStub("AssemblyList");
        }

        public DialogManagerStub DialogManager { get; private set; }

        public CommandStub BrowseProjectBaseCommand { get; private set; }
        public CommandStub EditConfigsCommand { get; private set; }
        public CommandStub BrowseConfigBaseCommand { get; private set; }
        public CommandStub AddAssemblyCommand { get; private set; }
        public CommandStub RemoveAssemblyCommand { get; private set; }
        public CommandStub BrowseAssemblyPathCommand { get; private set; }

        public ValidatedStub ProjectPath { get; private set; }
        public ValidatedStub ProjectBase { get; private set; }
        public SelectionStub ProcessModel { get; private set; }
        public SelectionStub DomainUsage { get; private set; }

        public SelectionStub Runtime { get; private set; }
        public SelectionStub RuntimeVersion { get; private set; }
        public ValidatedStub ActiveConfigName { get; private set; }

        public SelectionStub ConfigList { get; private set; }

        public SelectionStub BinPathType { get; private set; }
        public ValidatedStub ApplicationBase { get; private set; }
        public ValidatedStub ConfigurationFile { get; private set; }
        public ValidatedStub PrivateBinPath { get; private set; }
        public ValidatedStub AssemblyPath { get; private set; }
        public SelectionStub AssemblyList { get; private set; }
        
        #region IPropertyView Members

        IDialogManager IPropertyView.DialogManager
        {
            get { return DialogManager; }
        }

        ICommand IPropertyView.BrowseProjectBaseCommand
        {
            get { return BrowseProjectBaseCommand; }
        }

        ICommand IPropertyView.EditConfigsCommand 
        {
            get { return EditConfigsCommand; }
        }

        ICommand IPropertyView.BrowseConfigBaseCommand 
        {
            get { return BrowseConfigBaseCommand; }
        }

        ICommand IPropertyView.AddAssemblyCommand 
        {
            get { return AddAssemblyCommand; }
        }

        ICommand IPropertyView.RemoveAssemblyCommand 
        {
            get { return RemoveAssemblyCommand; }
        }

        ICommand IPropertyView.BrowseAssemblyPathCommand
        {
            get { return BrowseAssemblyPathCommand; }
        }

        ITextElement IPropertyView.ProjectPath
        {
            get { return ProjectPath; }
        }

        IValidatedElement IPropertyView.ProjectBase 
        {
            get { return ProjectBase; }
        }

        ISelectionList IPropertyView.ProcessModel
        {
            get { return ProcessModel; }
        }

        ISelectionList IPropertyView.DomainUsage
        {
            get { return DomainUsage; }
        }

        ITextElement IPropertyView.ActiveConfigName
        {
            get { return ActiveConfigName; }
        }

        ISelectionList IPropertyView.ConfigList
        {
            get { return ConfigList; }
        }

        ISelectionList IPropertyView.Runtime
        {
            get { return Runtime; }
        }

        ISelectionList IPropertyView.RuntimeVersion
        {
            get { return RuntimeVersion; }
        }

        IValidatedElement IPropertyView.ApplicationBase 
        { 
            get { return ApplicationBase; }
        }

        IValidatedElement IPropertyView.ConfigurationFile
        {
            get { return ConfigurationFile; }
        }

        ISelection IPropertyView.BinPathType
        {
            get { return BinPathType; }
        }

        IValidatedElement IPropertyView.PrivateBinPath
        {
            get { return PrivateBinPath; }
        }

        ISelectionList IPropertyView.AssemblyList
        {
            get { return this.AssemblyList; }
        }

        IValidatedElement IPropertyView.AssemblyPath
        {
            get { return AssemblyPath; }
        }

        public bool Visible { get; set; }

        public void SetAssemblyList(IEnumerable<string> list)
        {
            throw new NotImplementedException();
        }

        public void SetErrorMessage(string property, string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
