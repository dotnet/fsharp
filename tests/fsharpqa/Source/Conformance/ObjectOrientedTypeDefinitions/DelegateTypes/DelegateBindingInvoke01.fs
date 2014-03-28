// #Regression #Conformance #Delegates
// Regression for SP1 bug 40222 in Devdiv2(dev11).
// Verify the runtime doesn't throw InvalidCastException when invoking delegate binding.

open Ninject.Planning.Bindings
 
// No explicit coercion fun -> Func: runtime exception "Invalid cast..."
let DelegateBindingInvoke = 
    try
        let h = new Binding( Condition = fun x -> true );
 
        let k = new Binding();
        k.Condition <- fun x -> false
 
        // Dummy code to invoke the 2 Conditions... and see the runtime behavior
        let t1 = k.Condition.Invoke(new Request())
        let t2 = h.Condition.Invoke(new Request())
        exit 0
    with
        | :? System.InvalidCastException as e -> printfn "InvalidCastException catched!" 
                                                 exit 1
