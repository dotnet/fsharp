// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution

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
    let gB23    (x:'a) (y:int) = C23.M(x,y)           // expect: ambiguity error
    let gB123   (x:'a) (y:int) = C123.M(x,y)          // expect: ambiguous on 2,3

//<Expects status="error" span="(60,34-60,44)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C23\.M : x:'a \* y:'b -> Two, static member C23\.M : x:'a \* y:int -> Three$</Expects>
//<Expects status="error" span="(61,34-61,45)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C123\.M : x:'a \* y:'a -> One, static member C123\.M : x:'a \* y:'b -> Two, static member C123\.M : x:'a \* y:int -> Three$</Expects>

