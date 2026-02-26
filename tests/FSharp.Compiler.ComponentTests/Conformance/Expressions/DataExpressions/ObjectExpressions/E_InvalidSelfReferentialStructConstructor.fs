// #Regression #Conformance #DataExpressions #ObjectConstructors 
// FSB 6350, Compiler crash on invalid code: Crash by invalid type definition

//<Expects status="error" span="(19,39)" id="FS0658">Structs may only bind a 'this' parameter at member declarations$</Expects>
//<Expects status="error" span="(20,9)" id="FS0696">This is not a valid object construction expression\. Explicit object constructors must either call an alternate constructor or initialize all fields of the object and specify a call to a super class constructor\.$</Expects>
//<Expects status="error" span="(20,14)" id="FS0039">The type 'byref<_,_>' does not define the field, constructor or member 'dt'</Expects>

module mod6350

open System

// bad
type X = struct

    val mutable dt : DateTime
    member public x.Dt = x.dt
    //member public x.Foo() = x.dt <- new DateTime(1,1,1)

    new (d : int, m : int, y: int) as this =
        this.dt <- new DateTime(y,m,d)

end
