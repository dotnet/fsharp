#r "helloWorldProvider.dll"

open FSharp.HelloWorld
open System.Collections.Generic

// Emit error if we try to test against an erased type
let testtype1 (x:obj) = x :? HelloWorldType

// Emit warning if we try to test against a type that contains an erased type
let testtype2 (x:obj) = x :? List<FSharp.HelloWorld.HelloWorldType>

// Emit error if we try to cast to an erased type
let casttype1 (x:obj) = x :?> HelloWorldType

// Emit warning if we try to cast to a type that contains an erased type
let casttype2 (x:obj) = x :?> List<FSharp.HelloWorld.HelloWorldType>

// Emit error if we try to cast to an erased type
let casttype3 (x:obj) = x :?> HelloWorldSubType

// Emit error if we try to cast to an erased type
let casttype4 (x:obj) = x :?> HelloWorldException

// Emit error if we try to cast to an erased type
let casttype5 (x:obj) = x :?> HelloWorldSubException

// Emit error if we try to match against an erased type
let matchtype1 (x:obj) =
  match x with
    :? HelloWorldType -> "yes"
  | _ -> "no"

// Emit warning if we try to match against a type that contains an erased type
let matchtype2 (x:obj) =
  match x with
    :? List<HelloWorldType> -> "yes"
  | _ -> "no"


type C = 
    class
       inherit HelloWorldSubType
    end

type C2() = 
    class
       inherit HelloWorldSubType()
    end

let okObjExpr1 = new HelloWorldSubType() // this is allowed

let badObjExpr2 = { new HelloWorldSubType() with override x.ToString() = "" }

