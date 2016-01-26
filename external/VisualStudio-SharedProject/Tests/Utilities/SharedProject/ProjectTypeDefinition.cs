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


namespace TestUtilities.SharedProject {
    /// <summary>
    /// Defines a project type definition, an instance of this gets exported:
    /// 
    /// [Export]
    /// [ProjectExtension(".njsproj")]                            // required
    /// [ProjectTypeGuid("577B58BB-F149-4B31-B005-FC17C8F4CE7C")] // required
    /// [CodeExtension(".js")]                                    // required
    /// [SampleCode("console.log('hi');")]                        // optional
    /// internal static ProjectTypeDefinition ProjectTypeDefinition = new ProjectTypeDefinition();
    /// </summary>
    public sealed class ProjectTypeDefinition {
    }
}
