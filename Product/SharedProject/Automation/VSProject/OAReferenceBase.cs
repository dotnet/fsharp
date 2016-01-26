/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Runtime.InteropServices;
using VSLangProj;
using VSLangProj80;

namespace Microsoft.VisualStudioTools.Project.Automation {
    /// <summary>
    /// Represents the automation equivalent of ReferenceNode
    /// </summary>
    /// <typeparam name="RefType"></typeparam>
    [ComVisible(true)]
    public abstract class OAReferenceBase : Reference3 {
        #region fields
        private ReferenceNode referenceNode;
        #endregion

        #region ctors
        internal OAReferenceBase(ReferenceNode referenceNode) {
            this.referenceNode = referenceNode;
        }
        #endregion

        #region properties
        internal ReferenceNode BaseReferenceNode {
            get { return referenceNode; }
        }
        #endregion

        #region Reference Members
        public virtual int BuildNumber {
            get { return 0; }
        }

        public virtual References Collection {
            get {
                return BaseReferenceNode.Parent.Object as References;
            }
        }

        public virtual EnvDTE.Project ContainingProject {
            get {
                return BaseReferenceNode.ProjectMgr.GetAutomationObject() as EnvDTE.Project;
            }
        }

        public virtual bool CopyLocal {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public virtual string Culture {
            get { throw new NotImplementedException(); }
        }

        public virtual EnvDTE.DTE DTE {
            get {
                return BaseReferenceNode.ProjectMgr.Site.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            }
        }

        public virtual string Description {
            get {
                return this.Name;
            }
        }

        public virtual string ExtenderCATID {
            get { throw new NotImplementedException(); }
        }

        public virtual object ExtenderNames {
            get { throw new NotImplementedException(); }
        }

        public virtual string Identity {
            get { throw new NotImplementedException(); }
        }

        public virtual int MajorVersion {
            get { return 0; }
        }

        public virtual int MinorVersion {
            get { return 0; }
        }

        public virtual string Name {
            get { throw new NotImplementedException(); }
        }

        public virtual string Path {
            get {
                return BaseReferenceNode.Url;
            }
        }

        public virtual string PublicKeyToken {
            get { throw new NotImplementedException(); }
        }

        public virtual void Remove() {
            BaseReferenceNode.Remove(false);
        }

        public virtual int RevisionNumber {
            get { return 0; }
        }

        public virtual EnvDTE.Project SourceProject {
            get { return null; }
        }

        public virtual bool StrongName {
            get { return false; }
        }

        public virtual prjReferenceType Type {
            get { throw new NotImplementedException(); }
        }

        public virtual string Version {
            get { return new Version().ToString(); }
        }

        public virtual object get_Extender(string ExtenderName) {
            throw new NotImplementedException();
        }
        #endregion

        public string Aliases {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool AutoReferenced {
            get { throw new NotImplementedException(); }
        }

        public virtual bool Isolated {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public virtual uint RefType {
            get {
                // Default to native reference to help prevent callers from
                // making incorrect assumptions
                return (uint)__PROJECTREFERENCETYPE.PROJREFTYPE_NATIVE;
            }
        }

        public virtual bool Resolved {
            get { throw new NotImplementedException(); }
        }

        public string RuntimeVersion {
            get { throw new NotImplementedException(); }
        }

        public virtual bool SpecificVersion {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public virtual string SubType {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }
    }
}
