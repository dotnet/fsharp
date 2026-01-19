// #Regression #Conformance #Quotations 
//Regression for FSHARP1.0:6030
//Quoting a class member ? results in error about 'trait members' but quoting it as op_Dynamic is ok
//<Expects status="error" id="FS0458" span="(19,17-19,18)">Quotations cannot contain expressions that make member constraint calls, or uses of operators that implicitly resolve to a member constraint call</Expects>
//<Expects status="error" id="FS0458" span="(22,13-22,30)">Quotations cannot contain expressions that make member constraint calls, or uses of operators that implicitly resolve to a member constraint call</Expects>

module T

type Foo =
     val s : string
     new(s) = { s = s }
     static member (?) (foo : Foo, name : string) = foo.s + name
     static member (?<-) (foo : Foo, name : string, v : string) = ()

let foo = Foo("hello, ")

// Desugared form is ok, but ? desugars to a method with constraints which aren't allowed in quotes
let q1 = <@ Foo.op_Dynamic(foo, "uhh") @>
let q2 = <@ foo ? uhh @>

let q3 = <@ Foo.op_DynamicAssignment(foo, "uhh", "hm") @>
let q4 = <@ foo ? uhh <- "hm" @>

// Let bound functions handle this ok
let (?) o s =
    printfn "%s" s

// No error here because it binds to the let bound version
let q8 = <@ foo ? uhh @>

exit 0
