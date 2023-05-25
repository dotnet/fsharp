open System

module Basics2 = 
    let f6 () : 'T __withnull when 'T : not struct and 'T : null = null // Expected to give an error about inconistent constraints

module NullConstraintTests =
    type C<'T when 'T : null>() = class end

    let f1 (y : C< (int * int) >) = y // This gave an error in previous F# and we expect it to continue to give an error

    let f2 (y : C<FSharp.Collections.List<int>>) = y // This gave an error in previous F# and we expect it to continue to give an error

    let f7 (y : C<FSharp.Collections.List<String>> __withnull) = y // We expect this to give an error

    module StructExamples = 

        [<Struct>]
        type C1 =
            [<DefaultValue>]
            val mutable Whoops : String // expect a warning

        [<Struct>]
        type C4a =
            [<DefaultValue>]
            val mutable Whoops : int FSharp.Collections.List // expect a hard error like in previous F#

        [<Struct>]
        type C5 =
            [<DefaultValue>]
            val mutable Whoops : int * int // expect an error like previous F#

        [<Struct>]
        type C6 =
            [<DefaultValue>]
            val mutable Whoops : int -> int // expect an error like previous F#

    module ClassExamples = 
        type C1 =
            [<DefaultValue>]
            val mutable Whoops : String // expect an error if checknulls is on

        type C4a =
            [<DefaultValue>]
            val mutable Whoops : int FSharp.Collections.List // expect an error if checknulls is on

        type C5 =
            [<DefaultValue>]
            val mutable Whoops : int * int // expect an error like previous F#

        type C6 =
            [<DefaultValue>]
            val mutable Whoops : int -> int // expect an error like previous F#
