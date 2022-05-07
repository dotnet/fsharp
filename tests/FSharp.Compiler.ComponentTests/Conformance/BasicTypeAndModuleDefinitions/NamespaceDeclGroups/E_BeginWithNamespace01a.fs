// #Regression #Conformance #TypesAndModules #Namespaces 
// Regression test for FSHARP1.0:5354
// Namespace fragment warning should not underlines the entire file in blue
//<Expects status="error" span="(6,1,7,1)" id="FS0222">Files in libraries or multiple-file applications must begin with a namespace or module declaration, e\.g\. 'namespace SomeNamespace\.SubNamespace' or 'module SomeNamespace\.SomeModule'\. Only the last source file of an application may omit such a declaration\.$</Expects>

open System

let ran = System.Random()
let diceInterval (a,b) = a + (b-a) * ran.NextDouble()

let inline sqr x = x*x
let inline productR a b f = let mutable result = 1.0 in  for i = a to b do result <- result * f i done; result
let inline sumR     a b f = let mutable result = 0.0 in  for i = a to b do result <- result + f i done; result

let mutable npts = 500
//let integrate a b f = sumR 1 npts (fun i -> let x = diceInterval (a,b) in f x) * (b-a) / float npts

let integrate a b f =
    let dx = (b-a) / float npts
    sumR 0 npts (fun i -> let x = a + float i * dx in
                          let k = if i=0 || i=npts then 1.0 else 2.0
                          k * f x) * dx / 2.0

let pi     = Math.PI
let twoPi  = Math.PI * 2.0
let fourPi = Math.PI * 4.0

let f x =
    let y = 123
    let y = 123
    let y = 123
    let y = 123
    let y = 123
    12
