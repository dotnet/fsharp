// #Regression #Conformance #ApplicationExpressions 
#light

// Before check-in 11472, assert(false) had a special treatment (used to throw an AssertionFailure exception)
// Now, it is just a normal expression and it is subject to conditional compilation rules.
// Note: you must compile with: --define DEBUG

(* Define my TraceListener *)
#nowarn "70"
type MyTraceListener() = class
                          inherit System.Diagnostics.TraceListener()
                          override x.Write (message:string) = ()
                          override x.WriteLine (message:string) = ()
                          override x.Fail (message:string) = exit 0
                         end
(* Instantiate my TraceListener *)
let mtl = new MyTraceListener()

(* Remove Default TraceListener: this is responsible for bringing up the Assert/Fail dialog *)
System.Diagnostics.Trace.Listeners.Remove("Default")

(* Plug in my TraceListener *)
System.Diagnostics.Trace.Listeners.Add(mtl)

(* Finally execute the testcase. Since DEBUG is defined, the assert will fire!  *)
(* Remember that assert is really System.Diagnostics.Debug.Assert(): this will  *)
(* enumerate on all the listeners and invoke the Fail() method on them, i.e.    *)
(* invoke mtl.Fail(), which exits with 0 to signal the test passed!             *)
assert(false)

exit 1
