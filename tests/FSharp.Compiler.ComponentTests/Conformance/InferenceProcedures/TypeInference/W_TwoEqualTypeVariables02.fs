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


module M2 =
    let gB3     (x:'a) (y:'a) = C3.M(x,y)  = Three   // expect: less generic 'b (warning/error) 'a = int
    let gC3     (x:'a) (y:'a) = C3.M<'a>(x,y)  = Three   // expect: less generic 'a (warning/error) 'a = int
    let gD3     (x:'a) (y:'a) = C3.M<_>(x,y)  = Three    // expect: less generic 'a (warning/error) 'a = int

//<Expects id="FS0064" span="(61,40-61,41)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(62,44-62,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(63,43-63,44)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
