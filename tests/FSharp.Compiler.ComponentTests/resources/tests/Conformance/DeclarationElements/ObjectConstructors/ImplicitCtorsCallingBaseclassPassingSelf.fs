// #Regression #Conformance #DeclarationElements #ObjectConstructors

// verify passing self to base class constructors works, see https://github.com/Microsoft/visualfsharp/issues/669

open System

type Parent(o:obj) = class end

type Parent<'t>(t:'t) = 
    member val T = t

// this used to not work, causing NullReferenceException
module Implicit = 

    // Instantiating this type should throw InvalidOperationException.
    type Broken() as bself = 
        inherit Parent(bself)

    // this should work.
    type Ok() as self = 
        inherit Parent<unit->Ok>(fun () -> self)

module Explicit =

    // should throw InvalidOperationException.
    type Broken = 
        inherit Parent
        new() as gself = { inherit Parent(gself) }

    // this should work.
    type Ok = 
        inherit Parent<unit->Ok>
        new() as self = { inherit Parent<unit->Ok>(fun () -> self) }

    
let case1() = 
  try
    let r = Implicit.Broken()
    false
  with 
    | :? InvalidOperationException -> true 
    | _ -> false
  
let case2() = 
  try
    let r = Explicit.Broken()
    false
  with 
    | :? InvalidOperationException -> true 
    | _ -> false

let case3() = 
  let r = Implicit.Ok().T()
  true
  
let case4() =
  let r = Explicit.Ok().T()
  true

let results = [ case1(); case2(); case3(); case4()]

do if not (List.forall id results) then exit 1 else exit 0