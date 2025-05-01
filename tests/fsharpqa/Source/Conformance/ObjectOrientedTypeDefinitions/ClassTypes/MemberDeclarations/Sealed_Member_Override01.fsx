// #Regression #Conformance #ObjectOrientedTypes #Classes #Inheritance 
// Regression test DevDiv:370485 ([Portable] Cannot override sealed method)
//<Expects status="success"></Expects>
// This is the repro case for the bug. There should be no error.

type T1() =
    inherit CSLib.B1()
    override x.M(o : obj) = 12
    //override x.M(i : int) = 2     // ERROR {expected}

// Same as above, just use a hierarchy 3 levels deep
type T2() =
    inherit CSLib2.B2()
    override x.M(o : obj) = 12
    //override x.M(i : int) = 2     // ERROR {expected}

type T3() =
    inherit CSLib4.B1()
    override x.M(o : obj) = 12
    override x.M(o : char) = 12
    //override x.M(o : float) = 12
    //override x.M(i : int) = 2     // ERROR {expected}

type T4() =
    inherit CSLib5.B1()
    override x.M<'a>(o : 'a) = 12
    override x.N<'a>(o : 'a) = 12
    override x.M(c : char, o : int) = 12
    override x.N(c : char, o : int) = 12
    //override x.M(i : int) = 2     // ERROR {expected}

let t1 = T1()
let t2 = T2()

// The non-overridable defined in B1
if t1.M(1) <> 2 || t2.M(1) <> 2 then exit 1

// The overridden defined here
if t1.M(t1) <> 12 || t2.M(t2) <> 12 then exit 1

exit 0