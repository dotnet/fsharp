
module FSharp.BuildProperties

val internal fsProductVersion: string

val internal fsLanguageVersion: string



namespace FSharp


module ExistingPositive

module Extractions0a =
    
    val x: 'a when 'a: null
    
    val result: bool

module Extractions0b =
    
    val x: 'a when 'a: null
    
    val f: x: 'T * y: 'T -> unit
    
    val result: unit

module Extractions0d =
    
    val x: 'a when 'a: null
    
    val f: x: 'T * y: 'T -> unit when 'T: null
    
    val result: unit

module Basics =
    
    val x1: System.String
    
    val x4: System.String

module Basics2 =
    
    val f1: unit -> 'a when 'a: null
    
    val f8: unit -> System.String

type C =
    
    new: s: System.String -> C
    
    member Value: System.String

module InteropBasics =
    
    val s0: string
    
    val s1: System.String
    
    val test1: unit -> string
    
    val test2: s1: System.String * s2: System.String -> string
    
    val test3: unit -> System.String
    
    val test4: unit -> System.AppDomain
    
    val test5: System.AppDomain

val f0: line: string -> unit

module NullConstraintTests =
    
    type C<'T when 'T: null> =
        
        new: unit -> C<'T>
    
    val f3: y: C<System.String> -> C<System.String>

module DefaultValueTests =
    
    module StructExamples =
        
        [<Struct>]
        type C2 =
            
            [<DefaultValue (false)>]
            val mutable Whoops: System.String
    
    module ClassExamples =
        
        type C2 =
            
            [<DefaultValue (false)>]
            val mutable Whoops: System.String


module EnabledPositive

module Basics =
    
    val x2: System.String | null
    
    val x3: System.String | null

module NotNullConstraint =
    
    val f3: x: 'T -> int when 'T: not null
    
    val v1: int
    
    val v2: int
    
    val v3: int
    
    val v4: int
    
    val v5: int
    
    val w1: int
    
    val w2: int
    
    val w3: int
    
    val w4: int
    
    val w5: int

module MemberBasics =
    
    type C =
        
        new: unit -> C
        
        member M: unit -> int
        
        member P: int
    
    val c: C | null
    
    val v1: int
    
    val v2: int
    
    val f1: (unit -> int)

module Basics2 =
    
    val f1: unit -> 'a when 'a: null
    
    val f2: unit -> System.String | null
    
    val f3: unit -> 'T | null when 'T: not null
    
    val f4: unit -> 'T | null when 'T: not null and 'T: not struct
    
    val f5: unit -> 'T when 'T: not struct and 'T: null
    
    val f8: unit -> System.String

type C =
    
    new: s: System.String -> C
    
    member Value: System.String

val f: x: 'T | null * y: 'T | null -> unit when 'T: not null

module Extractions0c =
    
    val x: 'a when 'a: null
    
    val f: x: 'T | null * y: 'T | null -> unit when 'T: not null
    
    val s: System.String
    
    val result: unit

module Extractions0e =
    
    val x: 'a when 'a: null
    
    val f: x: 'T | null * y: 'T | null -> unit when 'T: not null
    
    val result: unit

module Extractions1 =
    
    val mutable x: string

module InteropBasics =
    
    val s0: string
    
    val s1: System.String
    
    val test1: unit -> string
    
    val test2: s1: System.String * s2: System.String -> string
    
    val test3: unit -> System.String
    
    val test4: unit -> System.AppDomain
    
    val test5: System.AppDomain

[<Class>]
type KonsoleWithNulls =
    
    static member WriteLine: s: System.String -> unit
    
    static member
      WriteLine: fmt: System.String * [<System.ParamArray>] args: obj array ->
                   unit
    
    static member WriteLine: fmt: System.String * arg1: System.String -> unit
    
    static member WriteLineC: s: C -> unit
    
    static member WriteLineC: fmt: C * arg1: C -> unit

module KonsoleWithNullsModule =
    
    val WriteLine: s: System.String | null -> unit
    
    val WriteLine2:
      fmt: System.String | null * arg1: System.String | null -> unit
    
    val WriteLineC: s: C | null -> unit
    
    val WriteLineC2: fmt: C | null * arg1: C | null -> unit

module KonsoleWithNullsModule2 =
    
    val WriteLine: x: System.String | null -> unit
    
    val WriteLine2:
      fmt: System.String | null * arg1: System.String | null -> unit
    
    val WriteLineC: s: C | null -> unit
    
    val WriteLineC2: fmt: C | null * arg1: C | null -> unit

[<Class>]
type KonsoleNoNulls =
    
    static member WriteLine: s: System.String -> unit
    
    static member
      WriteLine: fmt: System.String * [<System.ParamArray>] args: obj array ->
                   unit
    
    static member WriteLine: fmt: System.String * arg1: System.String -> unit
    
    static member WriteLineC: s: C -> unit
    
    static member WriteLineC: fmt: C * arg1: C -> unit

module KonsoleNoNullsModule =
    
    val WriteLine: s: System.String -> unit
    
    val WriteLine2: fmt: System.String * arg1: System.String -> unit
    
    val WriteLineC: s: C -> unit
    
    val WriteLineC2: fmt: C * arg1: C -> unit

module KonsoleNoNullsModule2 =
    
    val WriteLine: x: System.String -> unit
    
    val WriteLine2: fmt: System.String * arg1: System.String -> unit
    
    val WriteLineC: s: C -> unit
    
    val WriteLineC2: fmt: C * arg1: C -> unit

val f0: line: string -> unit

module NullConstraintTests =
    
    type C<'T when 'T: null> =
        
        new: unit -> C<'T>
    
    val f3: y: C<System.String> -> C<System.String>
    
    val f4: y: C<System.String | null> -> C<System.String | null>
    
    val f5: y: C<List<int> | null> -> C<List<int> | null>
    
    val f6: y: C<List<System.String> | null> -> C<List<System.String> | null>

module DefaultValueTests =
    
    module StructExamples =
        
        [<Struct>]
        type C2 =
            
            [<DefaultValue (false)>]
            val mutable Whoops: System.String
        
        [<Struct>]
        type C3 =
            
            [<DefaultValue>]
            val mutable Whoops: System.String | null
        
        [<Struct>]
        type C4b =
            
            [<DefaultValue>]
            val mutable Whoops: List<int> | null
        
        [<Struct>]
        type C7 =
            
            [<DefaultValue>]
            val mutable Whoops: (int -> int | null)
    
    module ClassExamples =
        
        type C2 =
            
            [<DefaultValue (false)>]
            val mutable Whoops: System.String
        
        type C3 =
            
            [<DefaultValue>]
            val mutable Whoops: System.String | null
        
        type C4b =
            
            [<DefaultValue>]
            val mutable Whoops: List<int> | null
        
        type C7 =
            
            [<DefaultValue>]
            val mutable Whoops: (int -> int | null)

