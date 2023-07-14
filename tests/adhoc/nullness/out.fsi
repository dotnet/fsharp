
module FSharp.BuildProperties

val internal fsProductVersion: string

val internal fsLanguageVersion: string



namespace FSharp


module ExistingPositive

module Extractions0a =
    
    val x: 'a __withnull
    
    val result: bool

module Extractions0b =
    
    val x: 'a __withnull
    
    val f: x: 'T * y: 'T -> unit
    
    val result: unit

module Extractions0d =
    
    val x: 'a __withnull
    
    val f: x: 'T * y: 'T -> unit when 'T: null
    
    val result: unit

module Basics =
    
    val x1: System.String
    
    val x4: System.String

module Basics2 =
    
    val f1: unit -> 'a __withnull
    
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
    
    val x2: System.String __withnull
    
    val x3: System.String __withnull

module NotNullConstraint =
    
    val f3: x: 'T -> int when 'T: __notnull
    
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
    
    val c: C __withnull
    
    val v1: int
    
    val v2: int
    
    val f1: (unit -> int)

module Basics2 =
    
    val f1: unit -> 'a __withnull
    
    val f2: unit -> System.String __withnull
    
    val f3: unit -> 'T __withnull when 'T: __notnull
    
    val f4: unit -> 'T __withnull when 'T: __notnull and 'T: not struct
    
    val f5: unit -> 'T when 'T: not struct and 'T: null
    
    val f8: unit -> System.String

type C =
    
    new: s: System.String -> C
    
    member Value: System.String

val f: x: 'T __withnull * y: 'T __withnull -> unit when 'T: __notnull

module Extractions0c =
    
    val x: 'a __withnull
    
    val f: x: 'T __withnull * y: 'T __withnull -> unit when 'T: __notnull
    
    val s: System.String
    
    val result: unit

module Extractions0e =
    
    val x: 'a __withnull
    
    val f: x: 'T __withnull * y: 'T __withnull -> unit when 'T: __notnull
    
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
    
    val WriteLine: s: System.String __withnull -> unit
    
    val WriteLine2:
      fmt: System.String __withnull * arg1: System.String __withnull -> unit
    
    val WriteLineC: s: C __withnull -> unit
    
    val WriteLineC2: fmt: C __withnull * arg1: C __withnull -> unit

module KonsoleWithNullsModule2 =
    
    val WriteLine: x: System.String __withnull -> unit
    
    val WriteLine2:
      fmt: System.String __withnull * arg1: System.String __withnull -> unit
    
    val WriteLineC: s: C __withnull -> unit
    
    val WriteLineC2: fmt: C __withnull * arg1: C __withnull -> unit

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
    
    val f4: y: C<System.String __withnull> -> C<System.String __withnull>
    
    val f5: y: C<List<int> __withnull> -> C<List<int> __withnull>
    
    val f6:
      y: C<List<System.String> __withnull> -> C<List<System.String> __withnull>

module DefaultValueTests =
    
    module StructExamples =
        
        [<Struct>]
        type C2 =
            
            [<DefaultValue (false)>]
            val mutable Whoops: System.String
        
        [<Struct>]
        type C3 =
            
            [<DefaultValue>]
            val mutable Whoops: System.String __withnull
        
        [<Struct>]
        type C4b =
            
            [<DefaultValue>]
            val mutable Whoops: List<int> __withnull
        
        [<Struct>]
        type C7 =
            
            [<DefaultValue>]
            val mutable Whoops: (int -> int __withnull)
    
    module ClassExamples =
        
        type C2 =
            
            [<DefaultValue (false)>]
            val mutable Whoops: System.String
        
        type C3 =
            
            [<DefaultValue>]
            val mutable Whoops: System.String __withnull
        
        type C4b =
            
            [<DefaultValue>]
            val mutable Whoops: List<int> __withnull
        
        type C7 =
            
            [<DefaultValue>]
            val mutable Whoops: (int -> int __withnull)

