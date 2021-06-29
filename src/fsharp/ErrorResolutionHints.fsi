// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions to format error message details
module internal FSharp.Compiler.ErrorResolutionHints

open Internal.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library
open System
open System.Collections
open System.Collections.Generic

/// We report a candidate if its edit distance is <= the threshold.
/// The threshold is set to about a quarter of the number of characters.
val IsInEditDistanceProximity: idText:string -> suggestion:string -> bool

/// Demangles a suggestion
val DemangleOperator: nm:string -> string

type SuggestionBufferEnumerator =

    interface IDisposable
    interface IEnumerator
    interface IEnumerator<string>
    new: tail:int * data: KeyValuePair<float,string> [] -> SuggestionBufferEnumerator
  
type SuggestionBuffer =

    interface IEnumerable
    interface IEnumerable<string>

    new: idText:string -> SuggestionBuffer

    member Add: suggestion:string -> unit

    member Disabled: bool

    member IsEmpty: bool
