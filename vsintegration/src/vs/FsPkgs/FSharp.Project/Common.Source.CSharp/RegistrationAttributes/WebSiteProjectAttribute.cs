// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


#if UNUSED

using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// This attribute adds a ProjectSubType to the exisiting list defined of ProjectSubTypes
    /// for the Web Site Project
    /// </summary>
    /// <remarks>
    /// For example:
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\(*version*)\Projects\
    ///		{E24C65DC-7377-472B-9ABA-BC803B73C61A}\ProjectSubType(VsTemplate)\IronPython
    ///			"Default"="Iron Python"
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0Exp\NewProjectTemplates\TemplateDirs\{39c9c826-8ef8-4079-8c95-428f5b1c323f}\IronPython]
    ///     @="Iron Python"
    ///     "NewProjectDialogExOnly"=dword:00000001
    ///     "SortPriority"=dword:0000012c
    ///     "TemplatesDir"="D:\\Program Files\\Microsoft Visual Studio 8\\Web\\.\\WebProjects\\IronPython"
    ///     "DeveloperActivity"="IronPython"

    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [System.Runtime.InteropServices.ComVisibleAttribute(false)]
    public sealed class WebSiteProjectAttribute : RegistrationAttribute
    {
        #region Constants
        private const string webSiteProjectGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        private const string websitePackageGuid = "{39c9c826-8ef8-4079-8c95-428f5b1c323f}";
        #endregion

        #region Fields
        private Type packageType;
        private string languageID;
        private string languageName;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new WebSiteProjectAttribute attribute to register a 
        /// language with the web site project 
        /// </summary>
        /// <param name="languageID">Language ID which is being referenced from the vstemplate</param>
        /// <param name="languageName">Language Name which shows up in the add new Web Site dialog under the list of languages</param>
        public WebSiteProjectAttribute(string languageID, string languageName)
        {
            if (languageID == null)
            {
                throw new ArgumentNullException("languageID", "languageID can not be null.");
            }
            if (languageName == null)
            {
                throw new ArgumentNullException("languageName", "languageName can not be null.");
            }

            this.languageID = languageID;
            this.languageName = languageName;

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Language ID which is being referenced from the vstemplate
        /// </summary>
        public string LanguageID
        {
            get { return languageID; }
        }

        /// <summary>
        /// Gets the Language Name which shows up in the add new Web Site dialog under the list of languages
        /// </summary>
        public object LanguageName
        {
            get { return languageName; }
        }

        /// <summary>
        /// ProjectSubTypePath for Web Site Project
        /// </summary>
        private string ProjectSubTypePath
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"Projects\{0}\ProjectSubType(VsTemplate)", webSiteProjectGuid);
            }
        }

        private string ProjectTemplatesDir
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"NewProjectTemplates\TemplateDirs\{0}", websitePackageGuid);
            }
        }
        /// <summary>
        /// Gets the Location of devenv.exe based on the RegistryRoot for the current package type
        /// </summary>
        private string getVSInstallDir(RegistrationContext context)
        {
            DefaultRegistryRootAttribute regRootAttr = (DefaultRegistryRootAttribute)TypeDescriptor.GetAttributes(context.ComponentType)[typeof(DefaultRegistryRootAttribute)];
            if (regRootAttr == null)
            {
                throw new NotSupportedException("could not find DefaultRegitryRootAttribute on " + context.ComponentType.ToString());
            }

            Win32.RegistryKey key = Win32.Registry.LocalMachine.OpenSubKey(regRootAttr.Root);
            //We are using HKCU in the case that the HKLM Experimental hive doesn't exist
            if (key == null || key.GetValue("InstallDir") == null)
            {
                key = Win32.Registry.CurrentUser.OpenSubKey(regRootAttr.Root + @"\Configuration");
            }
            string vsInstallDir = (string)key.GetValue("InstallDir");
            key.Close();
            return vsInstallDir;
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
            packageType = context.ComponentType;
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "WebSiteProject: LanguageID = {0} Language Name = {1}\n", languageID, languageName));

            //Register ProjectSubType(VsTemplates)
            using (Key childKey = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", ProjectSubTypePath, languageID)))
            {
                childKey.SetValue("", languageName);
            }

            //Register NewProjectTemplates
            using (Key childKey = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", ProjectTemplatesDir, languageID)))
            {
                childKey.SetValue("", languageName);
                childKey.SetValue("NewProjectDialogExOnly", 1);
                childKey.SetValue("SortPriority", 300);
                string templateDir = context.RootFolder.TrimEnd('\\') + string.Format(CultureInfo.InvariantCulture, "\\Web\\.\\WebProjects\\{0}", languageID);
                childKey.SetValue("TemplatesDir", context.EscapePath(templateDir));
                childKey.SetValue("DeveloperActivity", languageID);
            }
        }

        /// <summary>
        /// Unregister this languageID
        /// </summary>
        /// <param name="context">Given context to unregister from</param>
        public override void Unregister(RegistrationContext context)
        {
            if (context != null)
            {
                //UnRegister ProjectSubType(VsTemplates)
                context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", ProjectSubTypePath, languageID));

                //Register NewProjectTemplates
                context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", ProjectTemplatesDir, languageID));
            }
        }
        #endregion
    }
}

#endif