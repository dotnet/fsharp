// #Regression #Conformance #DataExpressions #ObjectConstructors 

// FSharp1.0:6424 - Object expressions fail to check override of final method (from method impl)

//<Expects id="FS0360" status="error" span="(9,23-9,32)">The method 'Class1\.CompareTo\(other: string\) : int' is sealed and cannot be overridden</Expects>

let z = 
    { new Class1() with
        override this.CompareTo(other : string) = 12
    }
let q = z.CompareTo("hello")