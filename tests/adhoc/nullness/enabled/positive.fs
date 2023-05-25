module EnabledPositive
open System
module Basics =     
    let x2 : String __withnull = null // expect no warning in any configuration
    let x3 : String __withnull = "a"  // expect no warning in any configuration

module NotNullConstraint =
    let f3 (x: 'T when 'T : __notnull) = 1
    let v1 = f3 1 
    let v2 = f3 "a"
    let v3 = f3 (null: obj) // Expect no warning in any configuration - warnings about 'obj' and nulls are suppressed 
    let v4 = f3 (null: String __withnull) // Expect to give a warning
    let v5 = f3 (Some 1) // Expect to give a warning

    let w1 = 1 |> f3
    let w2 = "a" |> f3
    let w3 = (null: obj) |> f3 // Expect no warning in any configuration - warnings about 'obj' and nulls are suppressed 
    let w4 = (null: String __withnull) |> f3 // Expect to give a warning
    let w5 = (Some 1) |> f3 // Expect to give a warning

module MemberBasics = 
    type C() = 
        member x.P = 1
        member x.M() = 2

    let c : C __withnull = C()
    let v1 = c.P  // Expected to give a warning
    let v2 = c.M()  // Expected to give a warning
    let f1 = c.M  // Expected to give a warning


module Basics2 = 
    let f1 () = null

    let f2 () : String __withnull = null

    let f3 () : 'T __withnull = null

    let f4 () : 'T __withnull when 'T : not struct = null

    let f5 () : 'T when 'T : not struct and 'T : null = null

    let f8 () : String = null // Expected to give a Nullness warning


type C(s: String) = 
    member __.Value = s


// This give a warning since 'T is not known to be reference type non-null
let f<'T when 'T: __notnull > (x: 'T __withnull, y: 'T __withnull) = ()

module Extractions0c =

    let x = null
    let f<'T when 'T : __notnull> (x: 'T __withnull, y: 'T __withnull) = ()
    let s : String = ""
    let result = f (x, s)   // expect no warning in any configuration

module Extractions0e =

    let x = null
    let f<'T when 'T : __notnull> (x: 'T __withnull, y: 'T __withnull) = ()
    let result = f (x, "") // expect no warning in any configuration

module Extractions1 =

    let mutable x : _ __withnull = null
    x <- ""  // expect no warning

//let f<'T when 'T : not struct> (x: 'T __withnull) = 1
module InteropBasics =
    let s0 = String.Concat("a","b")
    let s1 : String = String.Concat("a","c")
    let test1()  = String.Concat("a","d")
    let test2(s1:String, s2: String)  = String.Concat(s1,s2)
    let test3()  = String( [| 'a' |] )
    let test4()  = System.AppDomain.CurrentDomain
    let test5 : System.AppDomain  = System.AppDomain.CurrentDomain

type KonsoleWithNulls = 
    static member WriteLine(s: String __withnull) = Console.WriteLine(s)
    static member WriteLine(fmt: String __withnull, arg1: String __withnull) = Console.WriteLine(fmt, arg1)
    static member WriteLine(fmt: String __withnull, [<ParamArray>] args: obj __withnull[] __withnull) = Console.WriteLine(fmt, args)
    static member WriteLineC(s: C __withnull) = Console.WriteLine(s.Value)
    static member WriteLineC(fmt: C __withnull, arg1: C __withnull) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleWithNullsModule = 
    let WriteLine(s: String __withnull) = Console.WriteLine(s)
    let WriteLine2(fmt: String __withnull, arg1: String __withnull) = Console.WriteLine(fmt, arg1)
    let WriteLineC(s: C __withnull) = Console.WriteLine(s.Value)
    let WriteLineC2(fmt: C __withnull, arg1: C __withnull) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleWithNullsModule2 = 
    let WriteLine x = KonsoleWithNullsModule.WriteLine x
    let WriteLine2 (fmt, arg1) = KonsoleWithNullsModule.WriteLine2(fmt, arg1)
    let WriteLineC(s) = KonsoleWithNullsModule.WriteLineC(s)
    let WriteLineC2(fmt, arg1) = KonsoleWithNullsModule.WriteLineC2(fmt, arg1)

type KonsoleNoNulls = 
    static member WriteLine(s: String) = Console.WriteLine(s)
    static member WriteLine(fmt: String, arg1: String __withnull) = Console.WriteLine(fmt, arg1)
    static member WriteLine(fmt: String, [<ParamArray>] args: obj[]) = Console.WriteLine(fmt, args)
    static member WriteLineC(s: C) = Console.WriteLine(s.Value)
    static member WriteLineC(fmt: C, arg1: C) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleNoNullsModule = 
    let WriteLine(s: String) = Console.WriteLine(s)
    let WriteLine2(fmt: String, arg1: String) = Console.WriteLine(fmt, arg1)
    let WriteLineC(s: C) = Console.WriteLine(s.Value)
    let WriteLineC2(fmt: C, arg1: C) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleNoNullsModule2 = 
    let WriteLine x = KonsoleNoNullsModule.WriteLine x
    let WriteLine2 (fmt, arg1) = KonsoleNoNullsModule.WriteLine2(fmt, arg1)
    let WriteLineC(s) = KonsoleNoNullsModule.WriteLineC(s)
    let WriteLineC2(fmt, arg1) = KonsoleNoNullsModule.WriteLineC2(fmt, arg1)

System.Console.WriteLine("a")
System.Console.WriteLine("a", (null: obj[])) // Expected to give a Nullness warning

KonsoleWithNulls.WriteLine("Hello world")
KonsoleWithNulls.WriteLine(null) // WRONG: gives an incorrect Nullness warning for String __withnull and String __withnull
KonsoleWithNulls.WriteLine("Hello","world")
KonsoleWithNulls.WriteLine("Hello","world","there") // // WRONG: gives an incorrect Nullness warning for String __withnull and String __withnull

KonsoleNoNulls.WriteLine("Hello world")
try 
   KonsoleNoNulls.WriteLine(null)  // Expected to give a Nullness warning
with :? System.ArgumentNullException -> ()
KonsoleNoNulls.WriteLine("Hello","world")
//KonsoleNoNulls.WriteLine("Hello",null) // CHECK ME 
try 
    KonsoleNoNulls.WriteLine(null, "World")   // Expected to give a Nullness warning
with :? System.ArgumentNullException -> ()

KonsoleWithNullsModule.WriteLine("Hello world")
try 
    KonsoleWithNullsModule.WriteLine(null) 
with :? System.ArgumentNullException -> ()
KonsoleWithNullsModule.WriteLine2("Hello","world") 
KonsoleWithNullsModule.WriteLine2("Hello",null)
try
    KonsoleWithNullsModule.WriteLine2(null,"world")
with :? System.ArgumentNullException -> ()

KonsoleWithNullsModule2.WriteLine("Hello world")
try 
    KonsoleWithNullsModule2.WriteLine(null) 
with :? System.ArgumentNullException -> ()
KonsoleWithNullsModule2.WriteLine2("Hello","world")
KonsoleWithNullsModule2.WriteLine2("Hello",null)
try 
    KonsoleWithNullsModule2.WriteLine2(null,"world")
with :? System.ArgumentNullException -> ()

KonsoleNoNullsModule.WriteLine("Hello world")
try 
    KonsoleNoNullsModule.WriteLine(null)  // Expected to give a Nullness warning
with :? System.ArgumentNullException -> ()
KonsoleNoNullsModule.WriteLine2("Hello","world")
KonsoleNoNullsModule.WriteLine2("Hello",null) // Expected to give a Nullness warning
try 
    KonsoleNoNullsModule.WriteLine2(null,"world") // Expected to give a Nullness warning
with :? System.ArgumentNullException -> ()

//-------------------------------------

// Param array cases

KonsoleNoNulls.WriteLine("Hello","world","there")
KonsoleWithNulls.WriteLine("Hello","world",null)  // Expected to give a Nullness warning 
KonsoleNoNulls.WriteLine("Hello","world",null)  // Expected to give a Nullness warning
System.Console.WriteLine("a", (null: obj[] __withnull)) 
System.Console.WriteLine("a", (null: obj __withnull[] __withnull))

//-------
// random stuff

let f0 line = 
    let add (s:String) = ()
    match line with 
    | null | "" -> ()
    | _ -> add line // Exected to give a nullness warning

module NullConstraintTests =
    type C<'T when 'T : null>() = class end

    let f3 (y : C<String>) = y // Expect a Nullness warning

    let f4 (y : C<String __withnull>) = y // No warning expected 

    let f5 (y : C<FSharp.Collections.List<int> __withnull>) = y // No warning expected

    let f6 (y : C<FSharp.Collections.List<String> __withnull>) = y // No warning expected, lexing/parsing should succeed 

module DefaultValueTests =


    module StructExamples = 
        [<Struct>]
        type C2 =
            [<DefaultValue(false)>]
            val mutable Whoops : String // expect no warning

        [<Struct>]
        type C3 =
            [<DefaultValue>]
            val mutable Whoops : String __withnull // expect no warning

        [<Struct>]
        type C4b =
            [<DefaultValue>]
            val mutable Whoops : FSharp.Collections.List<int> __withnull // expect no warning

        [<Struct>]
        type C7 =
            [<DefaultValue>]
            val mutable Whoops : (int -> int) __withnull // expect no warning

    module ClassExamples = 

        type C2 =
            [<DefaultValue(false)>]
            val mutable Whoops : String // expect no warning

        type C3 =
            [<DefaultValue>]
            val mutable Whoops : String __withnull // expect no warning

        type C4b =
            [<DefaultValue>]
            val mutable Whoops : int FSharp.Collections.List __withnull // expect no warning

        type C7 =
            [<DefaultValue>]
            val mutable Whoops : (int -> int) __withnull // expect no warning
