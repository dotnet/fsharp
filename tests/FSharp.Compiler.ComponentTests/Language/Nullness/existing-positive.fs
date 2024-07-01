module ExistingPositive
open System
open System.Diagnostics
open System.Runtime.CompilerServices

module Extractions0a =

    let x = null
    let result = (x = "") //The type 'string' does not support 'null'.

module Extractions0b =

    let x = null
    let f (x: 'T, y: 'T) = ()
    let result = f (x, "")   //The type 'string' does not support 'null'.

module Extractions0d =

    let x = null
    let f<'T when 'T : null> (x: 'T, y: 'T) = ()
    let result = f (x, "")  //The type 'string' does not support 'null'.

module Basics =     
    let x1 : String = null // expect a warning when checknulls is on
    let x4 : String = ""

module Basics2 = 
    let f1 () = null
    let f8 () : String = null  // expect a warning when checknulls is on

type C(s: String) = 
    member __.Value = s

module InteropBasics =
    let s0 = String.Concat("a","b")
    let s1 : String = String.Concat("a","c")
    let test1()  = String.Concat("a","d")
    let test2(s1:String, s2: String)  = String.Concat(s1,s2)
    let test3()  = String( [| 'a' |] )
    let test4()  = System.AppDomain.CurrentDomain
    let test5 : System.AppDomain  = System.AppDomain.CurrentDomain

System.Console.WriteLine("a")
System.Console.WriteLine("a", (null: obj[])) // expect a warning when checknulls is on

let f0 line = 
    let add (s:String) = ()
    match line with 
    | null | "" -> () //The type 'string' does not support 'null'.
    | _ -> add line

module NullConstraintTests =
    type C<'T when 'T : null>() = class end

    let f3 (y : C<String>) = y // expect a warning when checknulls is on

module DefaultValueTests =

    module StructExamples = 

        [<Struct>]
        type C2 =
            [<DefaultValue(false)>]
            val mutable Whoops : String // expect no warning or error under any configuration

    module ClassExamples = 
        type C2 =
            [<DefaultValue(false)>]
            val mutable Whoops : String // expect no warning or error under any configuration
