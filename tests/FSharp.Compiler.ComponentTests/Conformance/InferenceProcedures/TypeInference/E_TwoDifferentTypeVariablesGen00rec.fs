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

//<Expects spans="(60,52-60,53)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type ''b'\.</Expects>
//<Expects spans="(60,18-60,42)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'\.</Expects>
//<Expects spans="(60,18-60,42)" status="error" id="FS0043">The type ''b' does not match the type ''b0'</Expects>
//<Expects spans="(61,52-61,53)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type 'int'\.</Expects>
//<Expects spans="(61,18-61,42)" status="error" id="FS0043">The type ''b' does not match the type 'int'</Expects>
//<Expects status="error" span="(63,45-63,55)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C13\.M : x:'a \* y:'a -> One, static member C13\.M : x:'a \* y:int -> Three$</Expects>
//<Expects spans="(63,56-63,57)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type ''b'\.</Expects>
//<Expects spans="(63,18-63,42)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'\.</Expects>
//<Expects spans="(63,18-63,42)" status="error" id="FS0043">The type ''b' does not match the type ''b0'</Expects>
//<Expects spans="(64,54-64,55)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'\.</Expects>
//<Expects spans="(64,56-64,57)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type 'int'\.</Expects>
//<Expects spans="(64,18-64,42)" status="error" id="FS0043">The type ''a' does not match the type 'int'</Expects>
//<Expects status="error" span="(66,45-66,59)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C13\.M : x:'a \* y:'a -> One, static member C13\.M : x:'a \* y:int -> Three$</Expects>
//<Expects spans="(66,55-66,56)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'a has been constrained to be type ''b'\.</Expects>
//<Expects spans="(66,18-66,42)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type ''a'\.</Expects>
//<Expects spans="(66,18-66,42)" status="error" id="FS0043">The type ''b' does not match the type ''b0'</Expects>
//<Expects status="error" span="(68,45-68,58)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: static member C13\.M : x:'a \* y:'a -> One, static member C13\.M : x:'a \* y:int -> Three$</Expects>
//<Expects spans="(68,55-68,56)" status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'b has been constrained to be type 'int'\.</Expects>
//<Expects spans="(68,18-68,42)" status="error" id="FS0043">The type ''b' does not match the type 'int'</Expects>
