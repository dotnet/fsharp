// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
open System

/// Error messages 
module internal Error = 
    let private invalidOperation s = new InvalidOperationException(s)
    let private argument s = new ArgumentException(s)
    let private argumentNull s = new ArgumentNullException(s)
    let private argumentOutOfRange s = new ArgumentOutOfRangeException(s)
    let private notImplemented s = new NotImplementedException(s)
    let ArgumentNull s = argumentNull s
    let ArgumentOutOfRange s = argumentOutOfRange s
    let NotImplemented s = notImplemented s
    let NoHierarchy = invalidOperation "No hierarchy was available."
    let NoHostObject = invalidOperation "No host object was found."
    let ProviderFactoryNotFilledIn = invalidOperation "ProviderFactory not filled in."
    let UnhandledMatchCase a = invalidOperation (sprintf "Unhandled match case '%A'" a)
    let NoMsBuildType = invalidOperation "No MSBuild Engine type loaded."
    let UseOfUnhookedLanguageServiceState = invalidOperation "Invalid call because language service state has been unhooked."
    let UseOfUninitializedLanguageServiceState = invalidOperation "Invalid call because language service state has not been initialized."
    let UseOfUninitializedServiceProvider = invalidOperation "ServiceProvider used before complete initialization."
    let Bug = invalidOperation "Unexpected."

/// Assert helpers
type internal Assert() = 
    /// Display a good exception for this error message and then rethrow.
    static member Exception(e:Exception) =  
        System.Diagnostics.Debug.Assert(false, "Unexpected exception seen in language service", e.ToString())
