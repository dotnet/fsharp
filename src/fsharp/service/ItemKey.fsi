// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System
open FSharp.Compiler.Range
open FSharp.Compiler.NameResolution

[<Sealed>]
type internal ItemKeyStore =
    interface IDisposable

    member FindAll: Item -> range seq

[<Sealed>]
type internal ItemKeyStoreBuilder =

    new: unit -> ItemKeyStoreBuilder

    member Write: range * Item -> unit

    member TryBuildAndReset: unit -> ItemKeyStore option