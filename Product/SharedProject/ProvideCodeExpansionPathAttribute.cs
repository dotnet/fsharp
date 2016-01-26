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
using System.Globalization;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudioTools {

    /// <summary>
    /// This attribute registers an additional path for code snippets to live
    /// in for a particular language.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    // Disable the "IdentifiersShouldNotHaveIncorrectSuffix" warning.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711")]
    sealed class ProvideCodeExpansionPathAttribute : RegistrationAttribute {
        private readonly string _languageStringId;
        private readonly string _description;
        private readonly string _paths;

        /// <summary>
        /// Creates a new RegisterSnippetsAttribute.
        /// </summary>
        public ProvideCodeExpansionPathAttribute(string languageStringId, string description,
                                          string paths) {
            _languageStringId = languageStringId;
            _description = description;
            _paths = paths;
        }

        /// <summary>
        /// Returns the string to use for the language name.
        /// </summary>
        public string LanguageStringId {
            get { return _languageStringId; }
        }

        /// <summary>
        /// Returns the paths to look for snippets.
        /// </summary>
        public string Paths {
            get { return _paths; }
        }

        /// <summary>
        /// The reg key name of the project.
        /// </summary>
        private string LanguageName() {
            return string.Format(CultureInfo.InvariantCulture, "Languages\\CodeExpansions\\{0}", LanguageStringId);
        }

        /// <summary>
        /// Called to register this attribute with the given context.
        /// </summary>
        /// <param name="context">
        /// Contains the location where the registration information should be placed.
        /// It also contains other informations as the type being registered and path information.
        /// </param>
        public override void Register(RegistrationContext context) {
            using (Key childKey = context.CreateKey(LanguageName())) {
                string snippetPaths = context.ComponentPath;
                snippetPaths = System.IO.Path.Combine(snippetPaths, Paths);
                snippetPaths = context.EscapePath(System.IO.Path.GetFullPath(snippetPaths));

                using (Key pathsSubKey = childKey.CreateSubkey("Paths")) {
                    pathsSubKey.SetValue(_description, snippetPaths);
                }
            }
        }

        /// <summary>
        /// Called to unregister this attribute with the given context.
        /// </summary>
        /// <param name="context">
        /// Contains the location where the registration information should be placed.
        /// It also contains other informations as the type being registered and path information.
        /// </param>
        public override void Unregister(RegistrationContext context) {
        }
    }
}

