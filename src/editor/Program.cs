﻿// ***********************************************************************
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
using System.Windows.Forms;

namespace NUnit.ProjectEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Common dialog manager
            DialogManager dialogManager = new DialogManager("NUnit Project Editor");

            // Set up main editor triad
            ProjectDocument doc = new ProjectDocument();
            MainForm view = new MainForm();
            new MainPresenter(doc, view, new DialogManager("NUnit Project Editor"));

            //// Set up property editor triad
            //ProjectModel project = new ProjectModel(doc);
            //IPropertyView propertyView = view.PropertyView;
            //new PropertyPresenter(project, propertyView, dialogManager);

            //// Set up XML editor triad
            //XmlModel xmlModel = new XmlModel();
            //IXmlView xmlView = view.XmlView;
            //new XmlPresenter(xmlModel, xmlView);

            if (args.Length > 0)
                doc.OpenProject(args[0]);

            Application.Run(view);
        }
    }
}
