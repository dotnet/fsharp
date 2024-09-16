// #Regression #Conformance #DataExpressions #ObjectConstructors 
#light

// Verify the static type of object expressions

// Verify static type is of type System.Collections.Generic.IComparer<int>

// See FSHARP1.0:4112 (it is by design that oe3 is IDisposable)
// See also FSHARP1.0:2954 (corresponding spec bug)

let oe1 =
    { 
        new System.Collections.Generic.IComparer<int> with
            member x.Compare(a,b) = compare (a % 7) (b % 7) 
    }

let _ = oe1 : System.Collections.Generic.IComparer<int>
  
// Verify static type is of type System.Object
let oe2 =
    { 
        new System.Object() with
            member x.ToString () = "Hello, base.ToString() = " + base.ToString() 
    }
let _ = oe2 : System.Object

// Verify static type is of type System.Object
let oe3 =
    { 
        new System.Object() with
            member x.Finalize() = printfn "Finalize"
        interface System.IDisposable with
            member x.Dispose() = printfn "Dispose"
    }
let _ = oe3 : System.IDisposable

exit 0
