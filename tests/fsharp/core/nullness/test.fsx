module M

open System
open System.Runtime.CompilerServices

//let f<'T when 'T : not struct> (x: 'T?) = 1

module Basics = 
    let x1 : string = null // Expected to give a nullability warning
    let x2 : string? = null // Should not give a nullability warning
    let x3 : string? = "a" // Should not give a nullability warning
    let x4 : string = "" // Should not give a nullability warning

    let x5 = nonNull<string> "" // Should not give a nullability warning
    let x6 = nonNull< string? > "" // QUESTION: should this give a nullability warning due to (T?)?  Should there be a non-null constraint?
    let x7 = nonNull ""
    let x8 = nonNull<string[]> Array.empty
    let x9 = nonNull [| "" |]
    let x10 = nonNullV (Nullable(3))
    let x11 = nonNullV (Nullable<int>())
    let x12 = nullV<int>
    let x13 = nullV<int64>
    let x14 = withNullV 6L
    let x15 : string? = withNull x4
    let x16 : Nullable<int> = withNullV 3
    
    let y0 = isNull null // QUESTION: gives a nullability warning due to 'obj' being the default. This seems ok
    let y1 = isNull (null: obj?) // Should not give a nullability warning
    let y2 = isNull "" // Should not give a nullability warning
    let y9 = isNull<string> "" // Should not give a nullability warning
    let y10 = isNull< string? > "" // QUESTION: should this give a nullability warning due to (T?)?  Should there be a non-null constraint?

module NotNullConstraint =
    let f3 (x: 'T when 'T : not null) = 1
    f3 1 // Should not give an error
    f3 "a" // Should not give an error
    f3 (null: obj?) // Expect to give a warning
    f3 (null: string?) // Expect to give a warning
#if NEGATIVE
    f3 (Some 1) // Expect to give an error
#endif

module Basics2 = 
    let f1 () = null
    // val f : unit -> 'a when 'a : null

    let f2 () : string? = null
    // val f : unit -> string?

    let f3 () : 'T? = null

    let f4 () : 'T? when 'T : not struct = null

    let f5 () : 'T when 'T : not struct and 'T : null = null

#if NEGATIVE
    let f6 () : 'T? when 'T : not struct and 'T : null = null // Expected to give an error about inconistent constraints
#endif

    // Note yet allowed 
    //let f7 () : 'T? when 'T : struct = null
    let f7b () : Nullable<'T> = nullV // BUG: Incorrectly gives a warning about System.ValueType with /test:AssumeNullOnImport

    let f8 () : string = null // Expected to give a nullability warning

type C(s: string) = 
    member __.Value = s

module InteropBasics =
    let s0 = String.Concat("","") // Expected to infer string? with /test:AssumeNullOnImport
    let s1 : string = String.Concat("","") // Expected to gives a warning with /test:AssumeNullOnImport
    let test1()  = String.Concat("","")
    let test2(s1:string, s2: string)  = String.Concat(s1,s2)
    let test3()  = String( [| 'a' |] )
    let test4()  = System.AppDomain.CurrentDomain
    let test5 : System.AppDomain  = System.AppDomain.CurrentDomain // Expected to gives a warning with /test:AssumeNullOnImport

type KonsoleWithNulls = 
    static member WriteLine(s: string?) = Console.WriteLine(s)
    static member WriteLine(fmt: string?, arg1: string?) = Console.WriteLine(fmt, arg1)
    static member WriteLine(fmt: string?, [<ParamArray>] args: obj?[]?) = Console.WriteLine(fmt, args)
    static member WriteLineC(s: C?) = Console.WriteLine(s.Value)
    static member WriteLineC(fmt: C?, arg1: C?) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleWithNullsModule = 
    let WriteLine(s: string?) = Console.WriteLine(s)
    let WriteLine2(fmt: string?, arg1: string?) = Console.WriteLine(fmt, arg1)
    let WriteLineC(s: C?) = Console.WriteLine(s.Value)
    let WriteLineC2(fmt: C?, arg1: C?) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleWithNullsModule2 = 
    let WriteLine x = KonsoleWithNullsModule.WriteLine x
    let WriteLine2 (fmt, arg1) = KonsoleWithNullsModule.WriteLine2(fmt, arg1)
    let WriteLineC(s) = KonsoleWithNullsModule.WriteLineC(s)
    let WriteLineC2(fmt, arg1) = KonsoleWithNullsModule.WriteLineC2(fmt, arg1)

type KonsoleNoNulls = 
    static member WriteLine(s: string) = Console.WriteLine(s)
    static member WriteLine(fmt: string, arg1: string?) = Console.WriteLine(fmt, arg1)
    static member WriteLine(fmt: string, [<ParamArray>] args: obj[]) = Console.WriteLine(fmt, args)
    static member WriteLineC(s: C) = Console.WriteLine(s.Value)
    static member WriteLineC(fmt: C, arg1: C) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleNoNullsModule = 
    let WriteLine(s: string) = Console.WriteLine(s)
    let WriteLine2(fmt: string, arg1: string) = Console.WriteLine(fmt, arg1)
    let WriteLineC(s: C) = Console.WriteLine(s.Value)
    let WriteLineC2(fmt: C, arg1: C) = Console.WriteLine(fmt.Value, arg1.Value)

module KonsoleNoNullsModule2 = 
    let WriteLine x = KonsoleNoNullsModule.WriteLine x
    let WriteLine2 (fmt, arg1) = KonsoleNoNullsModule.WriteLine2(fmt, arg1)
    let WriteLineC(s) = KonsoleNoNullsModule.WriteLineC(s)
    let WriteLineC2(fmt, arg1) = KonsoleNoNullsModule.WriteLineC2(fmt, arg1)

System.Console.WriteLine("a")
System.Console.WriteLine("a", (null: obj[])) // Expected to give a nullability warning

KonsoleWithNulls.WriteLine("Hello world")
KonsoleWithNulls.WriteLine(null) // WRONG: gives an incorrect nullability warning for string? and string?
KonsoleWithNulls.WriteLine("Hello","world")
KonsoleWithNulls.WriteLine("Hello","world","there") // // WRONG: gives an incorrect nullability warning for string? and string?

KonsoleNoNulls.WriteLine("Hello world")
KonsoleNoNulls.WriteLine(null)  // Expected to give a nullability warning
KonsoleNoNulls.WriteLine("Hello","world")
//KonsoleNoNulls.WriteLine("Hello",null) // CHECK ME 
KonsoleNoNulls.WriteLine(null, "World")   // Expected to give a nullability warning

KonsoleWithNullsModule.WriteLine("Hello world")
KonsoleWithNullsModule.WriteLine(null) 
KonsoleWithNullsModule.WriteLine2("Hello","world") 
KonsoleWithNullsModule.WriteLine2("Hello",null)
KonsoleWithNullsModule.WriteLine2(null,"world")

KonsoleWithNullsModule2.WriteLine("Hello world")
KonsoleWithNullsModule2.WriteLine(null) 
KonsoleWithNullsModule2.WriteLine2("Hello","world")
KonsoleWithNullsModule2.WriteLine2("Hello",null)
KonsoleWithNullsModule2.WriteLine2(null,"world")

KonsoleNoNullsModule.WriteLine("Hello world")
KonsoleNoNullsModule.WriteLine(null)  // Expected to give a nullability warning
KonsoleNoNullsModule.WriteLine2("Hello","world")
KonsoleNoNullsModule.WriteLine2("Hello",null) // Expected to give a nullability warning
KonsoleNoNullsModule.WriteLine2(null,"world") // Expected to give a nullability warning

//-------------------------------------

// Param array cases

KonsoleNoNulls.WriteLine("Hello","world","there")
KonsoleWithNulls.WriteLine("Hello","world",null)  // Expected to give a nullability warning 
KonsoleNoNulls.WriteLine("Hello","world",null)  // Expected to give a nullability warning
System.Console.WriteLine("a", (null: obj[]?)) 
System.Console.WriteLine("a", (null: obj?[]?))

//-------
// random stuff

let f0 line = 
    let add (s:string) = ()
    match line with 
    | null | "" -> ()
    | _ -> add line // Exected to give a nullness warning

let f0b line = 
    let add (s:string) = ()
    match line with 
    | null  -> ()
    | _ -> add (nonNull<string> line) // Exected to give a nullness warning

let add (s:string) = ()
let f0c line = 
    add (nonNull<string> "") // WRONG: should not give a nullness warning

let f1 (x: (string __hacknull)) = x;;

//let f2 (x: string or null) = x;;
let f3 (x: string?) = x
let f5 x = (x: int)
//let f4 x = (x: string nullable)

//let f6<'T when 'T : not null> (x: 'T) = x
//let f6<'T when 'T : not null> (x: 'T) = x

//let f2 (x: string | null) = x;;

//let f2 (_x : string | null) = x;;

//let f2 (x: (string | null)) = x;;

//let f3 x = (x: (string | null))

