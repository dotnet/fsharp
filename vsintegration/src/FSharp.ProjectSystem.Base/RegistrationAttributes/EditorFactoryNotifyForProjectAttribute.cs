// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// This attribute adds a File Extension for a Project System so that the Project
    /// will call IVsEditorFactoryNotify methods when an item of this type is added 
    /// or renamed.
    /// </summary>
    /// <remarks>
    /// For example:
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\Projects\
    ///		{F184B08F-C81C-45F6-A57F-5ABD9991F28F}\FileExtensions\.addin]
    ///			"EditorFactoryNotify"="{FA3CD31E-987B-443A-9B81-186104E8DAC1}"
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [System.Runtime.InteropServices.ComVisibleAttribute(false)]
    public sealed class EditorFactoryNotifyForProjectAttribute : RegistrationAttribute
    {
        #region Fields
		private Guid projectType;
		private Guid factoryType;
		private string fileExtension;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new ProvideEditorFactoryNotifyForProject attribute to register a 
        /// file extension with a project. 
        /// </summary>
        /// <param name="projectType">The type of project; can be a Type, a GUID or a string representation of a GUID</param>
        /// <param name="factoryType">The type of factory; can be a Type, a GUID or a string representation of a GUID</param>
        /// <param name="fileExtension">The file extension the EditorFactoryNotify wants to handle</param>
        public EditorFactoryNotifyForProjectAttribute(object projectType, string fileExtension, object factoryType)
        {
            if (factoryType == null)
            {
                throw new ArgumentNullException("factoryType", "Factory type can not be null.");
            }
            if (projectType == null)
            {
                throw new ArgumentNullException("projectType", "Project type can not be null.");
            }

            this.fileExtension = fileExtension;

            // figure out what type of object they passed in and get the GUID from it
            if (factoryType is string)
            {
                this.factoryType = new Guid(factoryType.ToString());
            }
            else if (factoryType is Type)
            {
                this.factoryType = ((Type)factoryType).GUID;
            }
            else if (factoryType is Guid)
            {
                this.factoryType = (Guid)factoryType;
            }
            else
            {
                throw new ArgumentException( "Parameter is expected to be an instance of the type 'Type' or 'Guid'.", "factoryType");
            }

            // figure out what type of object they passed in and get the GUID from it
            if (projectType is string)
            {
                this.projectType = new Guid(projectType.ToString());
            }
			else if (projectType is Type)
			{
                this.projectType = ((Type)projectType).GUID;
			}
			else if (projectType is Guid)
            {
                this.projectType = (Guid)projectType;
            }
            else
            {
                throw new ArgumentException("Parameter is expected to be an instance of the type 'Type' or 'Guid'.", "projectType");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the Guid representing the type of the editor factory
        /// </summary>
        //public Guid FactoryType
        public object FactoryType
        {
            get { return factoryType; }
        }

        /// <summary>
        /// Get the Guid representing the project type
        /// </summary>
        public object ProjectType
        {
            get { return projectType; }
        }

        /// <summary>
        /// Get or Set the extension of the XML files that support this view
        /// </summary>
        public string FileExtension
        {
            get { return fileExtension; }
        }

        /// <summary>
        /// Extention path within the registration context
        /// </summary>
        private string ProjectFileExtensionPath
        {
            get 
            { 
                return string.Format(CultureInfo.InvariantCulture, "Projects\\{0}\\FileExtensions\\{1}", projectType.ToString("B"), fileExtension); 
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called to register this attribute with the given context.  The context
        /// contains the location where the registration information should be placed.
        /// It also contains other information such as the type being registered and path information.
        /// </summary>
        /// <param name="context">Given context to register in</param>
        public override void Register(RegistrationContext context)
        {
            if (context == null)
            {
            	throw new ArgumentNullException("context");
            }

            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "EditorFactoryNoftifyForProject: {0} Extension = {1}\n", projectType.ToString(), fileExtension));

            using (Key childKey = context.CreateKey(ProjectFileExtensionPath))
            {
                childKey.SetValue("EditorFactoryNotify", factoryType.ToString("B"));
            }
        }

        /// <summary>
        /// Unregister this file extension.
        /// </summary>
        /// <param name="context">Given context to unregister from</param>
        public override void Unregister(RegistrationContext context)
        {
            if (context != null)
            {
                context.RemoveKey(ProjectFileExtensionPath);
            }
        }
        #endregion
    }
}
