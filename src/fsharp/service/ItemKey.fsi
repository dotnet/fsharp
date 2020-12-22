// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System

open FSharp.Compiler
open FSharp.Compiler.NameResolution

/// Stores a list of item key strings and their ranges in a memory mapped file.
[<Sealed>]
type internal ItemKeyStore =
    interface IDisposable

    member FindAll: Item -> Range seq

/// A builder that will build an item key store based on the written Item and its associated range.
[<Sealed>]
type internal ItemKeyStoreBuilder =

    new: unit -> ItemKeyStoreBuilder

    member Write: Range * Item -> unit

    member TryBuildAndReset: unit -> ItemKeyStore option