// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution
module M
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

 
module M0Rec =
    let rec      gB1<'a,'b> (x:'a) (y:'b) = C1.M(x,y) = One      // expect: type error
    let rec      gB3<'a,'b> (x:'a) (y:'b) = C3.M(x,y)  = Three   // expect: type error
    let rec     gB13<'a,'b> (x:'a) (y:'b) = C13.M(x,y)           // expect: ambiguity error (and note: both would instantiate 'a or 'b)
    let rec      gC1<'a,'b> (x:'a) (y:'b) = C1.M<'a>(x,y) = One      // expect: error
    let rec      gC3<'a,'b> (x:'a) (y:'b) = C3.M<'b>(x,y)  = Three   // expect: error
    let rec     gC13<'a,'b> (x:'a) (y:'b) = C13.M<'a>(x,y)           // expect: ambiguity error 
    let rec      gD1<'a,'b> (x:'a) (y:'b) = C1.M<_>(x,y) = One       // expect: type error
    let rec     gD13<'a,'b> (x:'a) (y:'b) = C13.M<_>(x,y)            // expect: ambiguity error (and note: both would instantiate 'a or 'b)
    let rec      gD3<'a,'b> (x:'a) (y:'b) = C3.M<_>(x,y)  = Three    // expect: error
