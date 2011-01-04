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

namespace NUnit.ProjectEditor
{
    /// <summary>
    /// IXmlView is the interface implemented by the XmlView
    /// and consumed by the XmlPresenter.
    /// </summary>
    public interface IXmlView
    {
        /// <summary>
        /// Event signaled when the xml text has changed
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        /// Gets or sets the visibility of the view
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the XML text
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Sets an exception arising in validating
        /// the XmlText.
        /// </summary>
        Exception Exception { set; }
    }
}
