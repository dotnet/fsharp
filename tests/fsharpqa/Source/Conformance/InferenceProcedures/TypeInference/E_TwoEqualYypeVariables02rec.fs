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

module M2Rec =
    let rec gB12    (x:'a) (y:'a) = C12.M(x,y)           // expect: ambiguity error
    let rec gB14    (x:'a) (y:'a) = C14.M(x,y)           // expect: ambiguity error
    let rec gB24    (x:'a) (y:'a) = C24.M(x,y)           // expect: ambiguity error
    let rec gB123   (x:'a) (y:'a) = C123.M(x,y)          // expect: ambiguity error
    let rec gB1234  (x:'a) (y:'a) = C1234.M(x,y)         // expect: ambiguity error
    let rec gD14    (x:'a) (y:'a) = C14.M<_>(x,y) = One      // expect: ambiguity error

//<Expects status="error" span="(60,37-60,47)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C12\.M : x:'a \* y:'a -> One, static member C12\.M : x:'a \* y:'b -> Two$</Expects>
//<Expects status="error" span="(61,37-61,47)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C14\.M : x:'a \* y:'a -> One, static member C14\.M : x:'a \* y:C -> Four$</Expects>
//<Expects status="error" span="(62,37-62,47)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C24\.M : x:'a \* y:'b -> Two, static member C24\.M : x:'a \* y:C -> Four$</Expects>
//<Expects status="error" span="(63,37-63,48)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C123\.M : x:'a \* y:'a -> One, static member C123\.M : x:'a \* y:'b -> Two, static member C123\.M : x:'a \* y:int -> Three$</Expects>
//<Expects status="error" span="(64,37-64,49)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C1234\.M : x:'a \* y:'a -> One, static member C1234\.M : x:'a \* y:'b -> Two, static member C1234\.M : x:'a \* y:C -> Four, static member C1234\.M : x:'a \* y:int -> Three$</Expects>
//<Expects status="error" span="(65,37-65,50)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C14\.M : x:'a \* y:'a -> One, static member C14\.M : x:'a \* y:C -> Four$</Expects>

