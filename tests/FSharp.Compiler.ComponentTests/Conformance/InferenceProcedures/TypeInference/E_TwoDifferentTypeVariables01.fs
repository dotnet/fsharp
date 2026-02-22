// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution
//<Expects status="error" span="(65,33-65,43)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C13\.M : x:'a \* y:'a -> One, static member C13\.M : x:'a \* y:int -> Three$</Expects>
//<Expects status="error" span="(66,33-66,43)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C24\.M : x:'a \* y:'b -> Two, static member C24\.M : x:'a \* y:C -> Four$</Expects>
//<Expects status="error" span="(67,33-67,47)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C13\.M : x:'a \* y:'a -> One, static member C13\.M : x:'a \* y:int -> Three$</Expects>
//<Expects status="error" span="(68,33-68,46)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C13\.M : x:'a \* y:'a -> One, static member C13\.M : x:'a \* y:int -> Three$</Expects>

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

module M1 =
// Errors    
    let gB13    (x:'a) (y:'b) = C13.M(x,y)           // expect: ambiguity error (and note: both would instantiate 'a or 'b)
    let gB24    (x:'a) (y:'b) = C24.M(x,y) = Four    // expect: ambiguity error 
    let gC13    (x:'a) (y:'b) = C13.M<'a>(x,y)       // expect: ambiguity error (and note: both would instantiate 'a or 'b)
    let gD13    (x:'a) (y:'b) = C13.M<_>(x,y)        // expect: ambiguity error (and note: both would instantiate 'a or 'b)
