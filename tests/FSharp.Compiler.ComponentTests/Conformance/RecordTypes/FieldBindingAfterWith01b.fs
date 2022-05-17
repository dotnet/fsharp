// #Regression #Conformance #TypesAndModules #Records 
// Regression test for FSHARP1.0:5488
// field bindings coming after 'with' need semicolons
// Long identifier form
//<Expects status="success"></Expects>
module TestModule

type R = { a : int; b : int } 
let r = { a = 1; b = 2 }

let v1' = { r with R.a = 1
                   R.b = 3 } 
                 
let v1b' = { r with
                R.a = 1
                R.b = 3 } 
                 
let v2' = { r with R.a =
                    let x = 1
                    x + x;
                  }

