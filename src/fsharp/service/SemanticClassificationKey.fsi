// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System

open FSharp.Compiler.Text

/// Provides a read only view to iterate over the semantic classification contents.
[<Sealed>]
type FSharpSemanticClassificationView =

    /// Iterate through the stored FSharpSemanticClassificationItem entries from the store and apply the passed function on each entry.
    member ForEach: (FSharpSemanticClassificationItem -> unit) -> unit

/// Stores a list of semantic classification key strings and their ranges in a memory mapped file.
/// Provides a view to iterate over the contents of the file.
[<Sealed>]
type internal SemanticClassificationKeyStore =
    interface IDisposable

    /// Get a read only view on the semantic classification key store
    member GetView: unit -> FSharpSemanticClassificationView


/// A builder that will build an semantic classification key store based on the written Item and its associated range.
[<Sealed>]
type internal SemanticClassificationKeyStoreBuilder =

    new: unit -> SemanticClassificationKeyStoreBuilder

    member WriteAll: FSharpSemanticClassificationItem[] -> unit

    member TryBuildAndReset: unit -> SemanticClassificationKeyStore option
