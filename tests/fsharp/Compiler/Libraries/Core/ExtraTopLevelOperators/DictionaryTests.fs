// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities

module ``Dictionary Tests`` =

    [<Test>]
    let ``Assigning to dictionary should compile``() =
        // Regression test for FSHARP1.0:5365

        CompilerAssert.Pass
            """
module N.M

    open System
    open System.Collections.Generic

    type ICloneable<'a> =
        abstract Clone : unit -> 'a

    type DictionaryFeature<'key> (dict: IDictionary<'key, int>) =
        member this.Add key value =
            dict.[key] <- value
            """

    [<Test>]
    let ``Assigning to dictionary with type constraint should compile``() =
        // Regression test for FSHARP1.0:5365
        // Used to give error: value must be local and mutable in order to mutate the contents of a value type, e.g. 'let mutable x = ...'

        CompilerAssert.Pass
            """
module N.M

    open System
    open System.Collections.Generic

    type ICloneable<'a> =
        abstract Clone : unit -> 'a

    type DictionaryFeature<'key, 'dict when 'dict :> IDictionary<'key, int> and 'dict :> ICloneable<'dict>> (dict: 'dict) =
        member this.Add key value =
            dict.[key] <- value  
            """
