// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if SINGLE_FILE_GENERATOR

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
	/// <summary>
	/// This attribute adds a custom file generator registry entry for specific file 
    /// type. 
	/// For Example:
	///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\9.0\Generators\
    ///		[proj_fac_guid]
	/// 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	internal sealed class SingleFileGeneratorSupportRegistrationAttribute : RegistrationAttribute
	{
        private Guid _prjFacGuid;
		/// <summary>
        /// Creates a new SingleFileGeneratorSupportRegistrationAttribute attribute to register a custom
		/// code generator for the provided context. 
		/// </summary>
        /// <param name="generatorType">The type of Code generator. Type that implements IVsSingleFileGenerator</param>
        /// <param name="generatorName">The generator name</param>
        /// <param name="contextGuid">The context GUID this code generator would appear under.</param>
        public SingleFileGeneratorSupportRegistrationAttribute(Type prjFactoryType)
		{
            if (prjFactoryType == null)
                throw new ArgumentNullException("prjFactoryType");

            _prjFacGuid = prjFactoryType.GUID;
        }

		
        /// <summary>
        /// Get the Guid representing the generator type
        /// </summary>
        public Guid ProjectFactoryGuid
        {
            get { return _prjFacGuid; }
        }

        /// <summary>
        /// Property that gets the generator base key name
        /// </summary>
        private string GeneratorRegKey
        {
            get { return string.Format(CultureInfo.InvariantCulture, @"Generators\{0}", ProjectFactoryGuid.ToString("B")); }
        }
		/// <summary>
		///     Called to register this attribute with the given context.  The context
		///     contains the location where the registration inforomation should be placed.
		///     It also contains other information such as the type being registered and path information.
		/// </summary>
		public override void Register(RegistrationContext context)
		{
            using (Key childKey = context.CreateKey(GeneratorRegKey))
            {
                childKey.SetValue(string.Empty, string.Empty);
            }

        }

		/// <summary>
		/// Unregister this file extension.
		/// </summary>
		/// <param name="context"></param>
		public override void Unregister(RegistrationContext context)
		{
            context.RemoveKey(GeneratorRegKey);
		}
	}
}
#endif