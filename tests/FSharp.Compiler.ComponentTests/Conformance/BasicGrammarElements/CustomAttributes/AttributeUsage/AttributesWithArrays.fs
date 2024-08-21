// #Regression #Conformance #DeclarationElements #Attributes 
// Regression for FSB 6162, Attribute constructors don't always accept arrays as arguments

open System

type ArrayAttribute(a:int array) = inherit Attribute()
type AnyAttribute(a:obj) = inherit Attribute()

(* works *)
[<Any 42>]
type T=class end

(* also works *)
[<Array [| 42 |]>]
type T2=class end

(* doesn't work, but should *)
[<Any [| 42 |]>]
type T3 = class end
