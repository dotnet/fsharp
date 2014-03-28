// #Regression #NoMT #Import 
// Regression test for FSHARP1.0:5121
// Compiler should not ICE when dealing with multiple op_Explicit
module M
let inline impl< ^a, ^b when ^a : (static member op_Explicit : ^a -> ^b)> arg =
        (^a : (static member op_Explicit : ^a -> ^b) (arg))

open Yadda
let b = new Blah<int,string>()
let ib : Bar<int> = impl b            // OK (used to ICE!)
let is : Bar<string> = impl b
let b2 = new Blah<int,int>()
let b3 = new Blah<int list,string list>()
