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
using System.ComponentModel.Composition;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Represents an interactive window role.
    /// 
    /// This attribute is a MEF contract and can be used to associate a REPL provider with its commands.
    /// This is new in 1.5.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ReplRoleAttribute : Attribute {
        private readonly string _name;

        public ReplRoleAttribute(string name) {
            if (name.Contains(","))
                throw new ArgumentException("ReplRoleAttribute name cannot contain any commas. Apply multiple attributes if you want to support multiple roles.", "name");

            _name = name;
        }

        public string Name {
            get { return _name; }
        }
    }
}
