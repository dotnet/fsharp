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

module M1Rec =
    let rec gB1     (x:'a) (y:'b) = C1.M(x,y) = One      // expect: less generic 'b (warning/error) 'b = 'a
    let rec gB3     (x:'a) (y:'b) = C3.M(x,y)  = Three   // expect: less generic 'b (warning/error) 'b = int
    let rec gC1     (x:'a) (y:'b) = C1.M<'a>(x,y) = One      // expect: less generic 'b (warning/error) 'b = 'a
    let rec gC3     (x:'a) (y:'b) = C3.M<'b>(x,y)  = Three   // expect: less generic 'b (warning/error) 'b = int
    let rec gD1     (x:'a) (y:'b) = C1.M<_>(x,y) = One       // expect: less generic 'b (warning/error) 'b = 'a
    let rec gD3     (x:'a) (y:'b) = C3.M<_>(x,y)  = Three    // expect: less generic 'b (warning/error) 'b = int

//<Expects id="FS0064" span="(60,44-60,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type ''b'</Expects>
//<Expects id="FS0064" span="(61,44-61,45)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(62,48-62,49)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type ''b'</Expects>
//<Expects id="FS0064" span="(63,46-63,47)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'</Expects>
//<Expects id="FS0064" span="(63,48-63,49)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'</Expects>
//<Expects id="FS0064" span="(64,47-64,48)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type ''b'</Expects>
//<Expects id="FS0064" span="(65,47-65,48)" status="warning">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type 'int'</Expects>
