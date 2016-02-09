// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.ComponentModel.Design;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// This attribute adds a commandline option to devenv for a specfic package 
    /// type. 
    /// For Example:
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\AppCommandLine\MyAppCommand
    ///     "Arguments"="*"
    ///     "DemandLoad"=dword:1
    ///     "Package"="{5C48C732-5C7F-40f0-87A7-05C4F15BC8C3}"
    ///     "HelpString"="#200"
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideAppCommandLineAttribute : RegistrationAttribute
    {
        #region fields
        private string _name = null;
        private string _args = null;
        private int _demandLoad = 0;
        private Guid _pkgGuid = Guid.Empty;
        private string _helpString = null; 
        #endregion

        #region ctors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of new command line option</param>
        /// <param name="packageType">package type</param>
        public ProvideAppCommandLineAttribute(string name, Type packageType)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name is null");

            if (packageType == null)
                throw new ArgumentNullException("Package Type is null.");

            _name = name;
            _pkgGuid = packageType.GUID;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Name of the command line
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Default arguments for the command line
        /// </summary>
        public string Arguments
        {
            get { return _args; }
            set { _args = value; }
        }

        /// <summary>
        /// Should the package be demand loaded.
        /// </summary>
        public int DemandLoad
        {
            get { return _demandLoad; }
            set { _demandLoad = value; }
        }

        /// <summary>
        /// Guid of the package providing the command line
        /// </summary>
        public string PackageGuid
        {
            get { return _pkgGuid.ToString("B"); }
            set { _pkgGuid = new Guid(value.ToString()); }
        }

        /// <summary>
        /// Help string to show for the command. Can be a resource id
        /// </summary>
        public string HelpString
        {
            get { return _helpString; }
            set { _helpString = value; }
        }

        #endregion

        #region overridden methods
        /// <summary>
        /// Called to register this attribute with the given context.  The context
        /// contains the location where the registration information should be placed.
        /// it also contains such as the type being registered, and path information.
        /// </summary>
        public override void Register(RegistrationContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "AppCommandLineKey: {0} \n", AppCommandLineRegKeyName));

            using (Key childKey = context.CreateKey(AppCommandLineRegKeyName))
            {
                childKey.SetValue("Arguments", Arguments);
                childKey.SetValue("DemandLoad", DemandLoad);
                childKey.SetValue("Package", PackageGuid);
                childKey.SetValue("HelpString", HelpString);
            }
        }

        /// <summary>
        /// Unregister this App command line
        /// </summary>
        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(AppCommandLineRegKeyName);
        } 
        #endregion

        #region helpers
        /// <summary>
        /// The reg key name of this AppCommandLine.
        /// </summary>
        private string AppCommandLineRegKeyName
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, @"AppCommandLine\{0}", Name);
            }
        } 
        #endregion
    }
}

