// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution
namespace N
// These different return types are used to determine which overload got chosen
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

module M3 =
    let gB12    (x:'a) (y:int) = C12.M(x,y)  = Two    // expect: ok
    let gB13    (x:'a) (y:int) = C13.M(x,y)  = Three  // expect: ok
    let gB2     (x:'a) (y:int) = C2.M(x,y)   = Two    // expect: ok
    let gB3     (x:'a) (y:int) = C3.M(x,y)   = Three  // expect: ok

    let gC12    (x:'a) (y:int) = C12.M<'a,int>(x,y) = Two  // expect: ok
    let gC23    (x:'a) (y:int) = C23.M<'a,int>(x,y) = Two  // expect: ok
    let gC13    (x:'a) (y:int) = C13.M<'a>(x,y) = Three    // expect: ok
    let gC2     (x:'a) (y:int) = C2.M<'a,int>(x,y)  = Two  // expect: ok
    let gC123   (x:'a) (y:int) = C123.M<'a,int>(x,y) = Two // expect: ok
    let gD12    (x:'a) (y:int) = C12.M<_,_>(x,y) = Two     // expect: ok
    let gD23    (x:'a) (y:int) = C23.M<_,_>(x,y) = Two     // expect: ok
    let gD13    (x:'a) (y:int) = C13.M<_>(x,y) = Three     // expect: ok
    let gD2     (x:'a) (y:int) = C2.M<_,_>(x,y)  = Two     // expect: ok
    let gD3     (x:'a) (y:int) = C3.M<_>(x,y)  = Three     // expect: ok
    let gD123   (x:'a) (y:int) = C123.M<_,_>(x,y) = Two    // expect: ok
