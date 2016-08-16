// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Result =

        /// <summary><c>map f inp</c> evaluates to <c>match inp with Error e -> Error e | Ok x -> Ok (f x)</c>.</summary>
        /// <param name="mapping">A function to apply to the OK result value.</param>
        /// <param name="result">The input result.</param>
        /// <returns>A result of the input value after applying the mapping function, or Error if the input is Error.</returns>
        [<CompiledName("Map")>]
        val map : mapping:('T -> 'U) -> result:Result<'T, 'TError> -> Result<'U, 'TError>

        /// <summary><c>map f inp</c> evaluates to <c>match inp with Error x -> Error (f x) | Ok v -> Ok v</c>.</summary>
        /// <param name="mapping">A function to apply to the OK result value.</param>
        /// <param name="result">The input result.</param>
        /// <returns>A result of the input value after applying the mapping function, or Error if the input is Error.</returns>
        [<CompiledName("MapError")>]
        val mapError: mapping:('TError -> 'U) -> result:Result<'T, 'TError> -> Result<'T, 'U>

        /// <summary><c>bind f inp</c> evaluates to <c>match inp with Error e -> Error e | Ok x -> f x</c></summary>
        /// <param name="binder">A function that takes the value of type T from a result and transforms it into
        /// a result containing a value of type U.</param>
        /// <param name="result">The input result.</param>
        /// <returns>A result of the output type of the binder.</returns>
        [<CompiledName("Bind")>]
        val bind: binder:('T -> Result<'U, 'TError>) -> result:Result<'T, 'TError> -> Result<'U, 'TError>
