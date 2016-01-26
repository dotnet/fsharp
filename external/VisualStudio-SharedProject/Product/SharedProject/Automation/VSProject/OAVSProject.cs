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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using EnvDTE;
using VSLangProj;

namespace Microsoft.VisualStudioTools.Project.Automation
{
    /// <summary>
    /// Represents an automation friendly version of a language-specific project.
    /// </summary>
    [ComVisible(true), CLSCompliant(false)]
    public class OAVSProject : VSProject
    {
        #region fields
        private ProjectNode project;
        private OAVSProjectEvents events;
        #endregion

        #region ctors
        internal OAVSProject(ProjectNode project)
        {
            this.project = project;
        }
        #endregion

        #region VSProject Members

        public virtual ProjectItem AddWebReference(string bstrUrl)
        {
            throw new NotImplementedException();
        }

        public virtual BuildManager BuildManager
        {
            get
            {
                throw new NotImplementedException();
                //return new OABuildManager(this.project);
            }
        }

        public virtual void CopyProject(string bstrDestFolder, string bstrDestUNCPath, prjCopyProjectOption copyProjectOption, string bstrUsername, string bstrPassword)
        {
            throw new NotImplementedException();
        }

        public virtual ProjectItem CreateWebReferencesFolder()
        {
            throw new NotImplementedException();
        }

        public virtual DTE DTE
        {
            get
            {
                return (EnvDTE.DTE)this.project.Site.GetService(typeof(EnvDTE.DTE));
            }
        }

        public virtual VSProjectEvents Events
        {
            get
            {
                if (events == null)
                    events = new OAVSProjectEvents(this);
                return events;
            }
        }

        public virtual void Exec(prjExecCommand command, int bSuppressUI, object varIn, out object pVarOut)
        {
            throw new NotImplementedException(); ;
        }

        public virtual void GenerateKeyPairFiles(string strPublicPrivateFile, string strPublicOnlyFile)
        {
            throw new NotImplementedException(); ;
        }

        public virtual string GetUniqueFilename(object pDispatch, string bstrRoot, string bstrDesiredExt)
        {
            throw new NotImplementedException(); ;
        }

        public virtual Imports Imports
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual EnvDTE.Project Project
        {
            get
            {
                return this.project.GetAutomationObject() as EnvDTE.Project;
            }
        }

        public virtual References References
        {
            get
            {
                ReferenceContainerNode references = project.GetReferenceContainer() as ReferenceContainerNode;
                if (null == references)
                {
                    return new OAReferences(null, project);
                }
                return references.Object as References;
            }
        }

        public virtual void Refresh()
        {
        }

        public virtual string TemplatePath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ProjectItem WebReferencesFolder
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool WorkOffline
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides access to language-specific project events
    /// </summary>
    [ComVisible(true), CLSCompliant(false)]
    public class OAVSProjectEvents : VSProjectEvents
    {
        #region fields
        private OAVSProject vsProject;
        #endregion

        #region ctors
        public OAVSProjectEvents(OAVSProject vsProject)
        {
            this.vsProject = vsProject;
        }
        #endregion

        #region VSProjectEvents Members

        public virtual BuildManagerEvents BuildManagerEvents
        {
            get
            {
                return vsProject.BuildManager as BuildManagerEvents;
            }
        }

        public virtual ImportsEvents ImportsEvents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ReferencesEvents ReferencesEvents
        {
            get
            {
                return vsProject.References as ReferencesEvents;
            }
        }

        #endregion
    }

}
