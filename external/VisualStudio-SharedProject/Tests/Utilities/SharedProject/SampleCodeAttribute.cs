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

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Registers the sample code used for a project.  See ProjectTypeDefinition
    /// for how this is used.  This attribute is optional.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class SampleCodeAttribute : Attribute {
        public readonly string _sampleCode;

        public SampleCodeAttribute(string sampleCode) {
            _sampleCode = sampleCode;
        }

        public string SampleCode {
            get {
                return _sampleCode;
            }
        }
    }
}
