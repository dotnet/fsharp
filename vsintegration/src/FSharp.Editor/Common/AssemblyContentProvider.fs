// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.Linq
open System.IO

open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Editor.Options
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.DebuggerIntelliSense
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.LanguageServices.Implementation
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.ComponentModelHost

[<Export(typeof<AssemblyContentProvider>); Composition.Shared>]
type internal AssemblyContentProvider () =
    let entityCache = EntityCache()

    member x.GetAllEntitiesInProjectAndReferencedAssemblies (fileCheckResults: FSharpCheckFileResults) =
        [ yield! AssemblyContentProvider.getAssemblySignatureContent AssemblyContentType.Full fileCheckResults.PartialAssemblySignature
          // FCS sometimes returns several FSharpAssembly for single referenced assembly. 
          // For example, it returns two different ones for Swensen.Unquote; the first one 
          // contains no useful entities, the second one does. Our cache prevents to process
          // the second FSharpAssembly which results with the entities containing in it to be 
          // not discovered.
          let assembliesByFileName =
              fileCheckResults.ProjectContext.GetReferencedAssemblies()
              |> Seq.groupBy (fun asm -> asm.FileName)
              |> Seq.map (fun (fileName, asms) -> fileName, List.ofSeq asms)
              |> Seq.toList
              |> List.rev // if mscorlib.dll is the first then FSC raises exception when we try to
                          // get Content.Entities from it.

          for fileName, signatures in assembliesByFileName do
              let contentType = Public // it's always Public for now since we don't support InternalsVisibleTo attribute yet
              yield! AssemblyContentProvider.getAssemblyContent entityCache.Locking contentType fileName signatures ]
