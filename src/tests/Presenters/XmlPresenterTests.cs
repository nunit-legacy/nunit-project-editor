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
using System.Xml;
using NUnit.Framework;
using NSubstitute;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class XmlPresenterTests
    {
        private IProjectModel model;
        private IXmlView view;

        private static readonly string initialText = "<NUnitProject />";
        private static readonly string changedText = "<NUnitProject processModel=\"Separate\" />";

        [SetUp]
        public void Initialize()
        {
            model = new ProjectModel();
            model.LoadXml(initialText);
            view = Substitute.For<IXmlView>();
            new XmlPresenter(model, view);
        }

        [Test]
        public void ViewSelected_WhenNoProjectIsOpen_RemainsHidden()
        {
            view.Selected += Raise.Event<ActionHandler>();
            Assert.False(view.Visible);
        }

        [Test]
        public void ViewSelected_WhenProjectIsOpen_BecomesVisible()
        {
            model.CreateNewProject();
            view.Selected += Raise.Event<ActionHandler>();
            Assert.True(view.Visible);
        }

        [Test]
        public void XmlText_OnLoad_IsInitializedCorrectly()
        {
            view.Selected += Raise.Event<ActionHandler>();
            Assert.AreEqual(initialText, view.Xml.Text);
        }

        [Test]
        public void XmlText_WhenChanged_ModelIsUpdated()
        {
            view.Xml.Text = changedText;
            view.Xml.Validated += Raise.Event<ActionHandler>();
            Assert.AreEqual(changedText, model.XmlText);
        }

        [Test]
        public void BadXmlSetsException()
        {
            view.Xml.Text = "<NUnitProject>"; // Missing slash
            view.Xml.Validated += Raise.Event<ActionHandler>();
            
            Assert.AreEqual("<NUnitProject>", model.XmlText);
            Assert.NotNull(model.Exception);
            Assert.IsInstanceOf<XmlException>(model.Exception);

            var ex = model.Exception as XmlException;
            view.Received().DisplayError(ex.Message, ex.LineNumber, ex.LinePosition);
        }
    }
}
