using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.ProjectEditor
{
    public interface ISelectableView : IView
    {

        /// <summary>
        /// Event that is fired whenever the _view is selected
        /// </summary>
        event ActionHandler Selected;

        /// <summary>
        /// Event that is fired in order to check whether
        /// the _view is in a state to be deselected
        /// </summary>
        event ActionStartingHandler Deselecting;

        /// <summary>
        /// Notify the _view that it has been selected
        /// </summary>
        void NotifySelected();

        /// <summary>
        /// Ask the _view if it can be deseleced
        /// </summary>
        /// <returns></returns>
        bool CanDeselect();
    }
}
