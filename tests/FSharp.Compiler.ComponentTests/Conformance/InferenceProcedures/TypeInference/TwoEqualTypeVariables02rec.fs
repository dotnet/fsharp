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

module M2Rec =
    let rec gB1     (x:'a) (y:'a) = C1.M(x,y)  = One     // expect: ok
    let rec gB2     (x:'a) (y:'a) = C2.M(x,y)  = Two     // expect: ok
    let rec gB4     (x:'a) (y:'a) = C4.M(x,y)  = Four    // expect: ok (note:  a subtype constraint has been added to 'b)
    let rec gB13    (x:'a) (y:'a) = C13.M(x,y) = One     // expect: ok
    let rec gB23    (x:'a) (y:'a) = C23.M(x,y) = Two     // expect: ok

    let rec gC1     (x:'a) (y:'a) = C1.M<'a>(x,y) = One      // expect: ok
    let rec gC2     (x:'a) (y:'a) = C2.M<'a,'a>(x,y)  = Two  // expect: ok
    let rec gC4     (x:'a) (y:'a) = C4.M<'a>(x,y)  = Four    // expect: ok
    let rec gC12    (x:'a) (y:'a) = C12.M<'a,'a>(x,y) = Two  // expect: ok
    let rec gC13    (x:'a) (y:'a) = C13.M<'a>(x,y) = One     // expect: ok
    let rec gC14    (x:'a) (y:'a) = C14.M<'a>(x,y) = One     // expect: ok
    let rec gC23    (x:'a) (y:'a) = C23.M<'a,'a>(x,y) = Two  // expect: ok
    let rec gC24    (x:'a) (y:'a) = C24.M<'a,'a>(x,y) = Two  // expect: ok
    let rec gC123   (x:'a) (y:'a) = C123.M<'a,'a>(x,y) = Two // expect: ok
    let rec gD1     (x:'a) (y:'a) = C1.M<_>(x,y) = One       // expect: ok
    let rec gD2     (x:'a) (y:'a) = C2.M<_,_>(x,y)  = Two    // expect: ok
    let rec gD4     (x:'a) (y:'a) = C4.M<_>(x,y)  = Four     // expect: ok
    let rec gD12    (x:'a) (y:'a) = C12.M<_,_>(x,y) = Two    // expect: ok
    let rec gD13    (x:'a) (y:'a) = C13.M<_>(x,y) = One      // expect: ok
    let rec gD23    (x:'a) (y:'a) = C23.M<_,_>(x,y) = Two    // expect: ok
    let rec gD24    (x:'a) (y:'a) = C24.M<_,_>(x,y) = Two    // expect: ok
    let rec gD123   (x:'a) (y:'a) = C123.M<_,_>(x,y) = Two   // expect: ok
