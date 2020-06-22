// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices


module ``Unit generic abstract Type`` =

    [<Fact>]
    let ``Unit can not be used as return type of abstract method paramete on return type``() =    
        CompilerAssert.TypeCheckSingleError
            """
type EDF<'S> =
    abstract member Apply : int -> 'S
type SomeEDF () =
    interface EDF<unit> with
        member this.Apply d = 
            // [ERROR] The member 'Apply' does not have the correct type to override the corresponding abstract method.
            ()
            """
            FSharpErrorSeverity.Error
            17
            (6, 21, 6, 26)
            "The member 'Apply : int -> unit' is specialized with 'unit' but 'unit' can't be used as return type of an abstract method parameterized on return type."
            
