// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.AbstractIL.Extensions.ILX.IlxSettings 

open FSharp.Compiler.AbstractIL 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AbstractIL.Internal 

type IlxCallImplementation = 
  | VirtEntriesVirtCode

//++GLOBAL MUTABLE STATE (concurrency-safe because assigned only during F# library compilation)
let ilxCompilingFSharpCoreLib = ref false

//++GLOBAL MUTABLE STATE (concurrency-safe because assigned only during F# library compilation)
let ilxFsharpCoreLibAssemRef = ref (None : ILAssemblyRef option)

/// Scope references for FSharp.Core.dll
let ilxFsharpCoreLibScopeRef () =
    if !ilxCompilingFSharpCoreLib then 
        ILScopeRef.Local 
    else 
        let assemblyRef = 
            match !ilxFsharpCoreLibAssemRef with 
            | Some o -> o
            | None -> 
                 // The exact public key token and version used here don't actually matter, or shouldn't.
                 // ilxFsharpCoreLibAssemRef is only 'None' for startup code paths such as
                 // IsSignatureDataVersionAttr, where matching is done by assembly name strings
                 // rather then versions and tokens.
                ILAssemblyRef.Create("FSharp.Core", None, 
                                     Some (PublicKeyToken(Bytes.ofInt32Array [| 0xb0; 0x3f; 0x5f; 0x7f; 0x11; 0xd5; 0x0a; 0x3a |])),
                                     false, 
                                     Some (IL.parseILVersion "0.0.0.0"), None)
        ILScopeRef.Assembly assemblyRef

let ilxNamespace () =  "Microsoft.FSharp.Core"