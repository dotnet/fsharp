// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer.UnitTests

open System
open System.IO

type TemporaryDirectory() =

    let directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())
    do Directory.CreateDirectory(directory) |> ignore

    member __.Directory = directory

    interface IDisposable with
        member __.Dispose() =
            try
                Directory.Delete(directory, true)
            with
            | _ -> ()
