// #Regression #Conformance #DataExpressions #ObjectConstructors 

// FSharp1.0:6424 - Object expressions fail to check override of final method (from method impl)

type Class1() =
    abstract CompareTo : string -> int
    default this.CompareTo(other : string) = 6
    interface System.IComparable<string> with
        member this.CompareTo o = this.CompareTo o

type Class2() =
    inherit Class1()
    override this.CompareTo(other : string) = 2

let x1 = { new Class1() with member __.CompareTo other = 7 }
if x1.CompareTo("") <> 7 then exit 1

let x3 = { new Class2() with member __.CompareTo other = 17 }
if x3.CompareTo("") <> 17 then exit 1

exit 0