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
    /// This attribute allows the Web Site Project to nest one file type (related) under another file type (primary) in the solution explorer
    /// </summary>
    /// <remarks>
    /// As an example the following Attribute definition 
    /// [WebSiteProjectRelatedFiles("aspx","py")]
    /// 
    /// would add the following registry key:
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\(*version*)\Projects\
    ///		{E24C65DC-7377-472B-9ABA-BC803B73C61A}\RelatedFiles\.aspx\.py
    ///			"Default"=""
    
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [System.Runtime.InteropServices.ComVisibleAttribute(false)]
    public sealed class WebSiteProjectRelatedFilesAttribute : RegistrationAttribute
    {
        private const string webSiteProjectGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";

        //private Type packageType;
        private string primaryFileExtension;
        private string relatedFileExtension;
        
        /// <summary>
        /// Creates a new WebSiteProjectAttribute attribute to register a 
        /// language with the web site project 
        /// </summary>
        /// <param name="primaryFileExtension">The primary file extension which will nest files.</param>
        /// <param name="relatedFileExtension">The related file extion which willl nest under the primary file extension</param>
        public WebSiteProjectRelatedFilesAttribute(string primaryFileExtension, string relatedFileExtension)
        {
            if (string.IsNullOrEmpty(primaryFileExtension))
            {
                throw new ArgumentNullException("primaryFileExtension", "primaryFileExtension can not be null.");
            }
            if (primaryFileExtension.Contains("."))
            {
                throw new ArgumentNullException("primaryFileExtension", "primaryFileExtension must not contain '.'");
            }
            if (string.IsNullOrEmpty(relatedFileExtension))
            {
                throw new ArgumentNullException("relatedFileExtension", "relatedFileExtension can not be null.");
            }
            if (relatedFileExtension.Contains("."))
            {
                throw new ArgumentNullException("relatedFileExtension", "relatedFileExtension must not contain '.'");
            }

            this.primaryFileExtension = primaryFileExtension;
            this.relatedFileExtension = relatedFileExtension;

        }

        /// <summary>
        /// Gets the primary file extension which will nest files
        /// </summary>
        public string PrimaryFileExtension
        {
            get { return primaryFileExtension; }
        }

        /// <summary>
        /// Gets the related file extion which willl nest under the primary file extension
        /// </summary>
        public object RelatedFileExtension
        {
            get { return relatedFileExtension; }
        }

        /// <summary>
        /// Returns the Web Site Project RelatedFiles Path
        /// </summary>
        private string RelatedFilePath
        {
            get
            {
                string relatedFiles = string.Format(CultureInfo.InvariantCulture, @"Projects\{0}\RelatedFiles", webSiteProjectGuid);
                return string.Format(CultureInfo.InvariantCulture, "{0}\\.{1}\\.{2}", relatedFiles, primaryFileExtension, relatedFileExtension);
            }
        }

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
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "WebSiteProjectRelatedFiles: Primary File Ext = {0} Related File Ext = {1}\n", primaryFileExtension, relatedFileExtension));

            //Register Related File
            context.CreateKey(RelatedFilePath);
        }

        /// <summary>
        /// Unregister this related file extension
        /// </summary>
        /// <param name="context">Given context to unregister from</param>
        public override void Unregister(RegistrationContext context)
        {
            if (context != null)
            {
                //UnRegister related file extextion
                context.RemoveKey(RelatedFilePath);
            }

        }
    }
}
#endif