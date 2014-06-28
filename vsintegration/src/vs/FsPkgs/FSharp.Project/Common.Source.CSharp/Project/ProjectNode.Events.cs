// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.Shell.Interop;

    public partial class ProjectNode
    {
        #region fields
        private EventHandler<ProjectPropertyChangedArgs> projectPropertiesListeners;
        #endregion

        #region events
        internal event EventHandler<ProjectPropertyChangedArgs> OnProjectPropertyChanged
        {
            add { projectPropertiesListeners += value; }
            remove { projectPropertiesListeners -= value; }
        }
        #endregion

        #region methods
        internal void RaiseProjectPropertyChanged(string propertyName, string oldValue, string newValue)
        {
            if (null != projectPropertiesListeners)
            {
                projectPropertiesListeners(this, new ProjectPropertyChangedArgs(propertyName, oldValue, newValue));
            }
        }
        #endregion
    }

}
