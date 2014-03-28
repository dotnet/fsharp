// #Regression #Libraries #Operators 
// Regression for FSHARP1.0:5995
// Calling "string" on an enum results in the integer value rather than the ToString name (possibly other static optimization issues?)

module M

// int32
type Foo =
  |  A  =  1
  |  B  =  2

let a = Foo.A
let r = a :> System.IFormattable 
if (string a) <> (string r) then exit 1

// uint32
type Foo2 =
    | A = 3u
    | B = 4u
    
let a2 = Foo2.A
let r2 = a :> System.IFormattable
if (string a2) <> (string r2) then exit 1

// char : see FSHARP1.0:6228
//type Foo3 =
//    | A = 'a'
//    | B = 'b'
    
//let a3 = Foo3.A
//let r3 = a :> System.IFormattable
//if (string a3) <> (string r3) then exit 1

// int16
type Foo4 =
    | A = 1s
    | B = 2s
    
let a4 = Foo4.A
let r4 = a :> System.IFormattable
if (string a4) <> (string r4) then exit 1

// uint16
type Foo5 =
    | A = 1us
    | B = 2us
    
let a5 = Foo5.A
let r5 = a :> System.IFormattable
if (string a5) <> (string r5) then exit 1

// sbyte
type Foo6 =
    | A = 1y
    | B = 2y
    
let a6 = Foo6.A
let r6 = a :> System.IFormattable
if (string a6) <> (string r6) then exit 1

// byte
type Foo7 =
    | A = 1uy
    | B = 2uy
    
let a7 = Foo7.A
let r7 = a :> System.IFormattable
if (string a7) <> (string r7) then exit 1
