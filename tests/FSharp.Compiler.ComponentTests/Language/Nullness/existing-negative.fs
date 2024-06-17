module ExistingNegative

open System
open System.Diagnostics
open System.Runtime.CompilerServices

module NullConstraintTests =
    type C<'T when 'T : null>() = class end

    let f1 (y : C< (int * int) >) = y // This gave an error in F# 4.5 and we expect it to continue to give an error

    let f2 (y : C<int list>) = y // This gave an error in F# 4.5 and we expect it to continue to give an error

    let f3 (y : C<int -> int>) = y // This gave an error in F# 4.5 and we expect it to continue to give an error

module DefaultValueTests =

    module StructExamples = 
        [<Struct>]
        type C1 =
            [<DefaultValue>]
            val mutable Whoops : String // expect an error if checknulls is on

        [<Struct>]
        type C4a =
            [<DefaultValue>]
            val mutable Whoops : int list // This gave an error in F# 4.5 and we expect it to continue to give an error

        [<Struct>]
        type C5 =
            [<DefaultValue>]
            val mutable Whoops : int * int // This gave an error in F# 4.5 and we expect it to continue to give an error

        [<Struct;NoComparison;NoEquality>]
        type C6 =
            [<DefaultValue>]
            val mutable Whoops : int -> int // This gave an error in F# 4.5 and we expect it to continue to give an error

    module ClassExamples = 

        type C4a =
            [<DefaultValue>]
            val mutable Whoops : int list // Should have given an error in F# 4.5 but didn't. Expect a corrective error if checknulls is on

        type C5 =
            [<DefaultValue>]
            val mutable Whoops : int * int // Should have given an error in F# 4.5 but didn't. Expect a corrective error if checknulls is on

        type C6 =
            [<DefaultValue>]
            val mutable Whoops : int -> int // Should have given an error in F# 4.5 but didn't. Expect a corrective error if checknulls is on

