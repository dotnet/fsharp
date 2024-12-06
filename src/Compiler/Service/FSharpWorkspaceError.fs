// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis.Workspace

open System

/// Everything that can go wrong with the workspace
type FSharpWorkSpaceError = ProjectNotFoundForFile of file: Uri

[<AutoOpen>]
module Utils =

    module Option =

        let toResult e =
            function
            | Some x -> Ok x
            | None -> Error e

        let swapAsync o =
            async {
                match o with
                | Some asyncValue ->
                    let! value = asyncValue
                    return Some value
                | None -> return None
            }

    module Result =

        let swapAsync r =
            async {
                match r with
                | Ok asyncValue ->
                    let! value = asyncValue
                    return Ok value
                | Error e -> return Error e
            }
