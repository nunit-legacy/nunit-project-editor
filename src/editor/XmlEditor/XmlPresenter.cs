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

namespace NUnit.ProjectEditor
{
    public class XmlPresenter
    {
        private IProjectModel _model;
        private IXmlView _view;

        public XmlPresenter(IProjectModel model, IXmlView view)
        {
            _model = model;
            _view = view;

            _view.Deselecting += delegate
            {
            };

            _view.Selected += delegate
            {
                LoadViewFromModel();
            };

            _view.Xml.Validated += delegate
            {
                UpdateModelFromView();

                if (!_model.IsValid)
                {
                    var ex = model.Exception as XmlException;
                    if (ex != null)
                        view.DisplayError(ex.Message, ex.LineNumber, ex.LinePosition);
                    else
                        view.DisplayError(model.Exception.Message);
                }
            };

            _model.Created += delegate
            {
                _view.Visible = true;
                LoadViewFromModel();
            };

            _model.Closed += delegate
            {
                _view.Xml.Text = null;
                _view.Visible = false;
            };
        }

        public void LoadViewFromModel()
        {
            _view.Xml.Text = _model.XmlText;

            if (_model.Exception != null)
            {
                var ex = _model.Exception as XmlException;
                if (ex != null)
                    _view.DisplayError(ex.Message, ex.LineNumber, ex.LinePosition);
                else
                    _view.DisplayError(_model.Exception.Message);
            }
            else
                _view.RemoveError();
        }

        private int GetOffset(int lineNumber, int charPosition)
        {
            int offset = 0;

            for (int lineCount = 1; lineCount < lineNumber; lineCount++ )
            {
                int next = _model.XmlText.IndexOf(Environment.NewLine, offset);
                if (next < 0) break;

                offset = next + Environment.NewLine.Length;
            }

            return offset - lineNumber + charPosition;
        }

        public void UpdateModelFromView()
        {
            _model.XmlText = _view.Xml.Text;
        }
    }
}
