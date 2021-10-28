// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1423
//
// bad error message: The member or object constructor 'Random' takes 1 arguments but is here supplied with 1.
//
// Now we should diplay: 
//    The member or object constructor 'Random' takes 0 type argument(s) but is here given 1. The required signature is 'static member Variable.Random: y: Variable<'a> -> Variable<'a>'.	E:\dd\fsharp_1\src\qa\md\src\fsh\Compiler\fsharp\Source\Conformance\DeclarationElements\ObjectConstructors\E-ImplicitExplicitCTors.fs
//
//<Expects id="FS0502" status="error" span="(22,1)">The member or object constructor 'Random' takes 0 type argument\(s\) but is here given 1\. The required signature is 'static member Variable\.Random: y: Variable<'a> -> Variable<'a>'</Expects>

#light

type Variable() =
    member x.Name with set(v:string) = ()

type Variable<'a>() =
    inherit Variable()
    static member Random(y:Variable<'a>) = new Variable<'a>()
    

let x : Variable<int> = failwith ""
Variable.Random<float> (x, Name = "m_")
