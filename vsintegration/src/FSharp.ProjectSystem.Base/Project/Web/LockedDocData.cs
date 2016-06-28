// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Design.Serialization;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem.Web
{
    internal class LockedDocData : DocData
    {
        private IServiceProvider     _serviceProvider;
        private RunningDocumentTable _rdt;
        private uint                 _cookie;
        
        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///    Constructor.  Aquires edit lock on document.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        public LockedDocData(IServiceProvider serviceProvider, string fileName) : base(serviceProvider, fileName)
        {
            _serviceProvider = serviceProvider;
            _rdt = new RunningDocumentTable(serviceProvider);

            // Locate and lock the document
            _rdt.FindDocument(fileName, out _cookie);
            _rdt.LockDocument(_VSRDTFLAGS.RDT_EditLock , _cookie);
        }

        ///-------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///    Override of Dispose so we can free our lock after DocData.
        /// </summary>
        ///-------------------------------------------------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_cookie != 0 && _rdt != null)
                {
                    // prevent recursion
                    uint cookie = _cookie;
                    _cookie = 0;
                    
                    try 
                    {
                        // Unlock the document, specifying to save if this is the last lock and the buffer is dirty
                        _rdt.UnlockDocument(_VSRDTFLAGS.RDT_EditLock | _VSRDTFLAGS.RDT_Unlock_SaveIfDirty, cookie);
                    }
                    finally
                    {
                        _cookie = 0;
                        _rdt = null;
                    }
                }
                _serviceProvider = null;
            }
        }
    }
}

