// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices

[<Export(typeof<AssemblyContentProvider>); Composition.Shared>]
type internal AssemblyContentProvider () =
    let entityCache = EntityCache()

    member x.GetAllEntitiesInProjectAndReferencedAssemblies (fileCheckResults: FSharpCheckFileResults) =
        [ yield! AssemblyContent.GetAssemblySignatureContent AssemblyContentType.Full fileCheckResults.PartialAssemblySignature
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
              let contentType = AssemblyContentType.Public // it's always Public for now since we don't support InternalsVisibleTo attribute yet
              yield! AssemblyContent.GetAssemblyContent entityCache.Locking contentType fileName signatures ]
