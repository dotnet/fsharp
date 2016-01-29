// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
	/// <summary>
	/// This attribute adds a solution persistence property name and related Guid
    /// type. 
	/// For Example:
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0Exp\SolutionPersistence\MyProperty]
    ///			"Default"="{AAAA53CC-3D4F-40a2-BD4D-4F3419755476}"
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class SolutionPersistenceRegistrationAttribute : RegistrationAttribute
	{
        /// <summary>
        /// Property name
        /// </summary>
        private string _propName;

		/// <summary>
        /// Creates a new SolutionPersistenceRegistrationAttribute attribute to register a solution persistence attribute
		/// for the provided context. 
		/// </summary>
        /// <param name="propName">Name of the property</param>
        public SolutionPersistenceRegistrationAttribute(string propName)
		{
            _propName = propName;
        }

		/// <summary>
		/// Get the property name
		/// </summary>
		public string PropName
		{
            get { return _propName; }
		}

        /// <summary>
        /// Property that gets the SolutionPersistence base key name
        /// </summary>
        private string SolutionPersistenceRegKey
        {
            get { return "SolutionPersistence"; }
        }

		/// <summary>
		///     Called to register this attribute with the given context.  The context
		///     contains the location where the registration inforomation should be placed.
		///     It also contains other information such as the type being registered and path information.
		/// </summary>
		public override void Register(RegistrationContext context)
		{
            context.Log.WriteLine(string.Format(CultureInfo.InvariantCulture, "ProvideSolutionProps: ({0} = {1})", context.ComponentType.GUID.ToString("B"), PropName));
            Key childKey = null;
            
            try
            {
                childKey = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", SolutionPersistenceRegKey, PropName));
                childKey.SetValue(string.Empty, context.ComponentType.GUID.ToString("B"));
            }
            finally
            {
                if (childKey != null) childKey.Close();
            }
        }

		/// <summary>
		/// Unregister this property.
		/// </summary>
		/// <param name="context"></param>
		public override void Unregister(RegistrationContext context)
		{
            context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", SolutionPersistenceRegKey, PropName));
		}
	}
}
