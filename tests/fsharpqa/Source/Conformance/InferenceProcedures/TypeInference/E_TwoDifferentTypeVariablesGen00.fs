// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution

//<Expects id="FS0001" span="(104,48-104,49)" status="error">This expression was expected to have type.    ''a'    .but here has type.    ''b'</Expects>
//<Expects id="FS0001" span="(105,48-105,49)" status="error">This expression was expected to have type.    'int'    .but here has type.    ''b'</Expects>
//<Expects id="FS0001" span="(106,48-106,49)" status="error">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects id="FS0193" span="(106,48-106,49)" status="error">Type constraint mismatch. The type.+''b'.+is not compatible with type</Expects>
//<Expects id="FS0041" span="(107,41-107,51)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>



//<Expects id="FS0041" span="(108,41-108,51)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>



//<Expects id="FS0001" span="(109,52-109,53)" status="error">This expression was expected to have type.    ''a'    .but here has type.    ''b'</Expects>
//<Expects id="FS0001" span="(110,50-110,51)" status="error">This expression was expected to have type.    ''b'    .but here has type.    ''a'</Expects>
//<Expects id="FS0001" span="(110,52-110,53)" status="error">This expression was expected to have type.    'int'    .but here has type.    ''b'</Expects>
//<Expects id="FS0001" span="(111,52-111,53)" status="error">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects id="FS0193" span="(111,52-111,53)" status="error">Type constraint mismatch. The type.+''b'.+is not compatible with type</Expects>
//<Expects id="FS0041" span="(112,41-112,55)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>




//<Expects id="FS0041" span="(113,41-113,55)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>




//<Expects id="FS0001" span="(114,51-114,52)" status="error">This expression was expected to have type.    ''a'    .but here has type.    ''b'</Expects>
//<Expects id="FS0001" span="(115,51-115,52)" status="error">This expression was expected to have type.    'int'    .but here has type.    ''b'</Expects>
//<Expects id="FS0001" span="(116,51-116,52)" status="error">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects id="FS0193" span="(116,51-116,52)" status="error">Type constraint mismatch. The type.+''b'.+is not compatible with type</Expects>
//<Expects id="FS0041" span="(117,41-117,54)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>




//<Expects id="FS0041" span="(118,41-118,54)" status="error">No overloads match for method 'M'\. The available overloads are shown below \(or in the Error List window\)\.</Expects>




//<Expects id="FS0001" span="(119,52-119,53)" status="error">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects id="FS0193" span="(119,52-119,53)" status="error">Type constraint mismatch. The type.+''b'.+is not compatible with type</Expects>

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

module M0 =
    let gB1     <'a,'b> (x:'a) (y:'b) = C1.M(x,y) = One      // expect: type error
    let gB3     <'a,'b> (x:'a) (y:'b) = C3.M(x,y)  = Three   // expect: type error
    let gB4     <'a,'b> (x:'a) (y:'b) = C4.M(x,y)  = Four    // expect: type error
    let gB13    <'a,'b> (x:'a) (y:'b) = C13.M(x,y)           // expect: ambiguity error (and note: both would instantiate 'a or 'b)
    let gB14    <'a,'b> (x:'a) (y:'b) = C14.M(x,y) = Four    // expect: ambiguity error 
    let gC1     <'a,'b> (x:'a) (y:'b) = C1.M<'a>(x,y) = One      // expect: type error
    let gC3     <'a,'b> (x:'a) (y:'b) = C3.M<'b>(x,y)  = Three   // expect: type error
    let gC4     <'a,'b> (x:'a) (y:'b) = C4.M<'a>(x,y)  = Four    // expect: type error
    let gC13    <'a,'b> (x:'a) (y:'b) = C13.M<'a>(x,y)           // expect: ambiguity error 
    let gC14    <'a,'b> (x:'a) (y:'b) = C14.M<'a>(x,y)           // expect: ambiguity error 
    let gD1     <'a,'b> (x:'a) (y:'b) = C1.M<_>(x,y) = One       // expect: type error
    let gD3     <'a,'b> (x:'a) (y:'b) = C3.M<_>(x,y)  = Three    // expect: type error
    let gD4     <'a,'b> (x:'a) (y:'b) = C4.M<_>(x,y)  = Four     // expect: type error
    let gD13    <'a,'b> (x:'a) (y:'b) = C13.M<_>(x,y)            // expect: ambiguity error (and note: both would instantiate 'a or 'b)
    let gD14    <'a,'b> (x:'a) (y:'b) = C14.M<_>(x,y)            // expect: type error
    let gD24    <'a,'b> (x:'a) (y:'b) = C24.M<_>(x,y)            // expect: type error

