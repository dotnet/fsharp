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

module M3 =
    let gB1     (x:'a) (y:int) = C1.M(x,y)   = One    // expect: less generic 'a = int
    let gC1     (x:'a) (y:int) = C1.M<'a>(x,y) = One       // expect: less generic int (warning/error) int = 'a
    let gC3     (x:'a) (y:int) = C3.M<int>(x,y)  = Three   // expect: less generic int (warning/error) int = int
    let gD1     (x:'a) (y:int) = C1.M<_>(x,y) = One        // expect: less generic int (warning/error) int = 'a

//<Expects id="FS0064" span="(60,41-60,42)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(61,45-61,46)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(62,44-62,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(63,44-63,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
