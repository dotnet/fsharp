// #Regression #TypeInference 
// Regression test for FSHARP1.0:4758
// Type Inference
// Check Method Disambiguation When User Generic Variable Get Instantiated By Overload Resolution

//<Expects id="FS0064" span="(68,40-68,41)" status="warning">.+'a.+''b'</Expects>
//<Expects id="FS0064" span="(69,40-69,41)" status="warning">.+'b.+'int'</Expects>
//<Expects id="FS0064" span="(70,44-70,45)" status="warning">.+'a.+''b'</Expects>
//<Expects id="FS0064" span="(71,42-71,43)" status="warning">.+'b.+''a'</Expects>
//<Expects id="FS0064" span="(71,44-71,45)" status="warning">.+'a.+'int'</Expects>
//<Expects id="FS0064" span="(72,43-72,44)" status="warning">.+'a.+''b'</Expects>
//<Expects id="FS0064" span="(73,43-73,44)" status="warning">.+'b.+'int'</Expects>
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

module M1 =
    let gB1     (x:'a) (y:'b) = C1.M(x,y) = One      // expect: less generic 'b (warning/error) 'b = 'a
    let gB3     (x:'a) (y:'b) = C3.M(x,y)  = Three   // expect: less generic 'b (warning/error) 'b = int
    let gC1     (x:'a) (y:'b) = C1.M<'a>(x,y) = One      // expect: less generic 'b (warning/error) 'b = 'a
    let gC3     (x:'a) (y:'b) = C3.M<'b>(x,y)  = Three   // expect: less generic 'b (warning/error) 'b = int
    let gD1     (x:'a) (y:'b) = C1.M<_>(x,y) = One       // expect: less generic 'b (warning/error) 'b = 'a
    let gD3     (x:'a) (y:'b) = C3.M<_>(x,y)  = Three    // expect: less generic 'b (warning/error) 'b = int
