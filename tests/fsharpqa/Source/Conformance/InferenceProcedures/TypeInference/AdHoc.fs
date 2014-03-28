// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution
// Notice that the [<OverloadID("...")>] attribute is no longer needed
// These different return types are used to determine which overload got chosen
namespace N
type One = | One
type Two = | Two
type Three = | Three
type Four = | Four

// An unsealed type
type C() = 
    member x.P = 1
    
type C1 =
    static member M<'a>(x:'a,y:'a) = One

type C2 =
    static member M<'a,'b>(x:'a,y:'b) = Two

type C3 =    
    static member M<'a>(x:'a,y:int) = Three

type C4 =    
    static member M<'a>(x:'a,y:C) = Four

type C12 =
    static member M<'a>(x:'a,y:'a) = One
    static member M<'a,'b>(x:'a,y:'b) = Two

type C23 =
    static member M<'a,'b>(x:'a,y:'b) = Two
    static member M<'a>(x:'a,y:int) = Three

type C13 =
    static member M<'a>(x:'a,y:'a) = One
    static member M<'a>(x:'a,y:int) = Three

type C14 =
    static member M<'a>(x:'a,y:'a) = One
    static member M<'a>(x:'a,y:C) = Four

type C24 =
    static member M<'a,'b>(x:'a,y:'b) = Two
    static member M<'a>(x:'a,y:C) = Four

type C123 =
    static member M<'a>(x:'a,y:'a) = One
    static member M<'a,'b>(x:'a,y:'b) = Two
    static member M<'a>(x:'a,y:int) = Three

type C1234 =
    static member M<'a>(x:'a,y:'a) = One
    static member M<'a,'b>(x:'a,y:'b) = Two
    static member M<'a>(x:'a,y:int) = Three
    static member M<'a>(x:'a,y:C) = Four

module Adhoc =
    let gB12a1            (x:'a) (y:'b) = C12.M(x,y) 
    let gB12a2<'a,'b>     (x:'a) (y:'b) = C12.M(x,y) 
    let rec gB12a3<'a,'b>     (x:'a) (y:'b) = C12.M(x,y) 
    let gB12b            (x:'a) (y:'b) = C12.M<'a,'b>(x,y) 
    let gB12c<'a,'b>     (x:'a) (y:'b) = C12.M<'a,'b>(x,y) 
    let rec gB12d        (x:'a) (y:'b) = C12.M<'a,'b>(x,y) 
    let rec gB12e<'a,'b> (x:'a) (y:'b) = C12.M<'a,'b>(x,y) 
