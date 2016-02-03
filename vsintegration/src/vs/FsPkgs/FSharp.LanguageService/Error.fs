// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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
    let UseOfUnitializedServiceProvider = invalidOperation "ServiceProvider used before complete initialization."
    let Bug = invalidOperation "Unexpected."

/// Localizable strings.
module internal Strings = 

    let private resources = new System.Resources.ResourceManager("FSLangSvcStrings", System.Reflection.Assembly.GetExecutingAssembly())

    /// Exceptions:
    let ExceptionsHeader = resources.GetString("ExceptionsHeader", System.Globalization.CultureInfo.CurrentUICulture)
    /// (still building content cache)
    let StillBuildingContentCache = resources.GetString("StillBuildingContentCache", System.Globalization.CultureInfo.CurrentUICulture)

    let GetString(s) = resources.GetString(s, System.Globalization.CultureInfo.CurrentUICulture)

    module Errors = 
        let private Format1 id (s : string) = 
            let format = GetString(id)
            System.String.Format(format, s)

        let GotoDefinitionFailed () = GetString "GotoDefinitionFailed_Generic"
        let GotoDefinitionFailed_ProvidedType(typeName : string) = Format1 "GotoDefinitionFailed_ProvidedType" typeName
        let GotoFailed_ProvidedMember(name : string) = Format1 "GotoDefinitionFailed_ProvidedMember" name
        let GotoDefinitionFailed_NotIdentifier () = GetString "GotoDefinitionFailed_NotIdentifier"
        let GotoDefinitionFailed_NoTypecheckInfo () = GetString "GotoDefinitionFailed_NoTypecheckInfo"
        let GotoDefinitionFailed_NoSourceCode () = GetString "GotoDefinitionFailed_NotSourceCode"

/// Assert helpers
type internal Assert() = 
    /// Display a good exception for this error message and then rethrow.
    static member Exception(e:Exception) =  
        System.Diagnostics.Debug.Assert(false, "Unexpected exception seen in language service", e.ToString())
    
        
    

