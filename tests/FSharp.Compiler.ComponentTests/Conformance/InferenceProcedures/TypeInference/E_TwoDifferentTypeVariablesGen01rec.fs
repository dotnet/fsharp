// #Regression #TypeInference 
// Regression test for FSHARP1.0:4776 (see also 4758)
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

    let rec      gB4<'a,'b> (x:'a) (y:'b) = C4.M(x,y)  = Four       // expect: error
    let rec     gB14<'a,'b> (x:'a) (y:'b) = C14.M(x,y) = Four       // expect: error
    let rec      gC4<'a,'b> (x:'a) (y:'b) = C4.M<'a>(x,y)  = Four   // expect: error
    let rec     gC14<'a,'b> (x:'a) (y:'b) = C14.M<'a>(x,y)          // expect: error
    let rec      gD4<'a,'b> (x:'a) (y:'b) = C4.M<_>(x,y)  = Four    // expect: error
    let rec     gD14<'a,'b> (x:'a) (y:'b) = C14.M<_>(x,y)           // expect: error
    let rec     gD24<'a,'b> (x:'a) (y:'b) = C24.M<_>(x,y)           // expect: error

//<Expects spans="(61,52-61,53)" status="warning" id="FS0193">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(61,18-61,42)" status="error" id="FS0043">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(63,56-63,57)" status="warning" id="FS0193">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(63,18-63,42)" status="error" id="FS0043">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(65,55-65,56)" status="warning" id="FS0193">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(65,18-65,42)" status="error" id="FS0043">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(67,56-67,57)" status="warning" id="FS0193">A type parameter is missing a constraint 'when 'b :> C'</Expects>
//<Expects spans="(67,17-67,42)" status="error" id="FS0043">A type parameter is missing a constraint 'when 'b :> C'</Expects>
