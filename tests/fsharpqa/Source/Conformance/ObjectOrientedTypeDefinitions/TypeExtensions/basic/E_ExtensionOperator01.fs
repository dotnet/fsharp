// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
// Regression for FSHARP1.0:3592
// Can't use extension methods to define operators
//<Expects status="warning" id="FS1215" span="(18,24-18,25)">Extension members cannot provide operator overloads\.  Consider defining the operator as part of the type definition instead\.</Expects>
//<Expects status="warning" id="FS1215" span="(22,24-22,25)">Extension members cannot provide operator overloads\.  Consider defining the operator as part of the type definition instead\.</Expects>
//<Expects status="error" span="(27,14-27,16)" id="FS0001">The type 'Exception' does not support the operator '\+'$</Expects>
//<Expects status="error" span="(27,12-27,13)" id="FS0043">The type 'Exception' does not support the operator '\+'$</Expects>
//<Expects status="error" span="(31,15-31,17)" id="FS0001">The type 'MyType' does not support the operator '\+'$</Expects>
//<Expects status="error" span="(31,13-31,14)" id="FS0043">The type 'MyType' does not support the operator '\+'$</Expects>

open System

type MyType() =
    member this.X = 1

module TestExtensions =
    type MyType with
        static member (+) (e1: MyType, e2: MyType) =
            new MyType()

    type System.Exception with
        static member (+) (e1: Exception, e2: Exception) =
            new Exception(e1.Message + " " + e2.Message)
            
let e1 = Exception()
let e2 = Exception()
let r = e1 + e2

let e3 = MyType()
let e4 = MyType()
let r2 = e3 + e4

exit 1
