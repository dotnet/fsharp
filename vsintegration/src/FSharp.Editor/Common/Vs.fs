// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop

/// Helper methods for interoperating with COM
module internal Com =        
    let Succeeded hr = 
        // REVIEW: Not the correct check for succeeded
        hr = VSConstants.S_OK

[<AutoOpen>]
module internal ServiceProviderExtensions =
    type internal System.IServiceProvider with 
        member sp.GetService<'S,'T>() = sp.GetService(typeof<'S>) :?> 'T

        member sp.TextManager = sp.GetService<SVsTextManager, IVsTextManager>()
        member sp.RunningDocumentTable = sp.GetService<SVsRunningDocumentTable, IVsRunningDocumentTable>()
        member sp.XmlService = sp.GetService<SVsXMLMemberIndexService, IVsXMLMemberIndexService>()
        member sp.DTE = sp.GetService<SDTE, EnvDTE.DTE>()
