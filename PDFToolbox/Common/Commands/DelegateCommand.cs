using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PDFToolbox.Common.Commands
{
    /// <summary>
    /// A command whose sole purpose is to relay its functionality
    /// to other objects by invoking delegates. The default return
    /// value for the CanExecute method is True.
    /// </summary>
    public class DelegateCommand : ICommand
    {

        #region "Fields"

        protected readonly Action<object> oExecute;
        protected readonly Predicate<object> oCanExecute;

        #endregion

        #region "Constructors"

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="oExecute">The execution logic.</param>
        public DelegateCommand(Action<object> oExecute)
            : this(oExecute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="oExecute">The execution logic.</param>
        /// <param name="oCanExecute">The execution status logic.</param>
        public DelegateCommand(Action<object> oExecute, Predicate<object> oCanExecute)
        {
            if (oExecute == null)
                throw new ArgumentNullException("oExecute");

            this.oExecute = oExecute;
            this.oCanExecute = oCanExecute;
        }

        #endregion

        #region "ICommand Members"

        [DebuggerStepThrough()]
        public bool CanExecute(object oParameter)
        {
            return oCanExecute == null ? true : oCanExecute.Invoke(oParameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }
        public virtual void OnCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void Execute(object oParameter)
        {
            oExecute.Invoke(oParameter);
        }

        #endregion

    }
}
