// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Math

// Deliberately left empty
//
//  FSharp.Core previously exposed the namespace Microsoft.FSharp.Math even though there were no types in it.
//  This retains that.
//  Existing programs could, and did contain the line:
//  open FSharp.Math
//

namespace Microsoft.FSharp.Core

/// <summary>An abbreviation for <see cref="T:System.Numerics.BigInteger"/>. </summary>
///
/// <category>Basic Types</category>
type bigint = System.Numerics.BigInteger

/// <summary>Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI' </summary>
///
/// <category>Language Primitives</category>
[<AutoOpen>]
module NumericLiterals =

    /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
    module NumericLiteralI =
        open System.Numerics

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromZero: value: unit -> 'T

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromOne: value: unit -> 'T

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromInt32: value: int32 -> 'T

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromInt64: value: int64 -> 'T

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromString: text: string -> 'T

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromInt64Dynamic: value: int64 -> obj

        /// Provides a default implementations of F# numeric literal syntax  for literals of the form 'dddI'
        val FromStringDynamic: text: string -> obj
