using System;
using System.ComponentModel;
using System.Diagnostics;

namespace PDFToolbox.ViewModels
{
    /// <summary>
    /// Base class for ViewModel classes.
    /// Provides support for property change notifications 
    /// and has a DisplayName property.  This class is MustInherit.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        #region "DisplayName"

        /// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        private string DisplayNameField;
        public virtual string DisplayName
        {
            get { return DisplayNameField; }
            protected set { DisplayNameField = value; }
        }

        #endregion

        #region "Debugging Aides"

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG"), DebuggerStepThrough()]
        public void VerifyPropertyName(string PropertyName)
        {
            // Allow empty string as it indicates all properties have changed.
            if (string.IsNullOrEmpty(PropertyName))
                return;

            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[PropertyName] == null)
            {
                string msg = "Invalid property name: " + PropertyName;

                if (this.ThrowOnInvalidPropertyName)
                {
                    throw new Exception(msg);
                }
                else
                {
                    Debug.Fail(msg);
                }
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        private bool ThrowOnInvalidPropertyNameField;
        protected virtual bool ThrowOnInvalidPropertyName
        {
            get { return ThrowOnInvalidPropertyNameField; }
            set { ThrowOnInvalidPropertyNameField = value; }
        }

        #endregion

        #region "Design-time Aides"

        private static bool? _isInDesignMode = null;
        protected static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    //_isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
                }

                return _isInDesignMode.Value;
            }
        }

        #endregion

        #region "INotifyPropertyChanged Support"

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="PropertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string PropertyName)
        {
            this.VerifyPropertyName(PropertyName);

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        #endregion

        #region "IDisposable Support"

        // To detect redundant calls
        private bool DisposedValue = false;

        protected bool IsDisposed
        {
            get { return DisposedValue; }
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose(bool Disposing)
        {
            this.DisposedValue = true;
        }

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public virtual void Dispose()
        {
            OnDispose(true);

#if !DEBUG
			GC.SuppressFinalize(this);
#endif
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            try
            {
                string msg = string.Format("{0} ({1}) ({2}) Finalized", this.GetType().Name, this.DisplayName, this.GetHashCode());
                System.Diagnostics.Debug.WriteLine(msg);
            }
            catch (Exception)
            {
                // Do nothing...
            }
        }
#endif

        #endregion
    }
}
