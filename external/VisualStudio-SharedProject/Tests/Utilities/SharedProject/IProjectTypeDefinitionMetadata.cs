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

using System.ComponentModel;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Metadata interface for getting information about declared project kinds.
    /// MEF requires that this be public.
    /// </summary>
    public interface IProjectTypeDefinitionMetadata {
        string ProjectExtension { get; }
        string ProjectTypeGuid { get; }
        string CodeExtension { get; }

        [DefaultValue("")]
        string SampleCode { get; }
    }
}
