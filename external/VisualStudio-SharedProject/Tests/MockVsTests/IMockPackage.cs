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
namespace Microsoft.VisualStudioTools.MockVsTests {
    /// <summary>
    /// Performs initialization of a mock VS package.
    /// 
    /// Initializing a real MPF Package class inside of MockVs is not actually possible  
    /// 
    /// Despite using siting, MPF actually goes off to global service providers for various
    /// activities.  For example it uses the ActivityLog class which does not get properly
    /// sited.  
    /// 
    /// To use MockVs packages should abstract most of the code from their package into an
    /// independent service and have their package publish (and promote) their service.  Mock
    /// packages can then do the same thing.
    /// </summary>
    public interface IMockPackage : IDisposable {
        void Initialize();
    }
}
