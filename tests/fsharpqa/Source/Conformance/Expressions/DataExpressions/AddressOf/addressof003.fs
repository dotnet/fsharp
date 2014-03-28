// #Regression #Conformance #DataExpressions #NoMono 
// AddressOf Operator
// Verify we can use the && operator to invoke Native methods (p/Invoke)
// In this case we call into a C method
//<Expects status="warning" span="(12,11-12,14)" id="FS0051">The use of native pointers may result in unverifiable \.NET IL code$</Expects>

[<System.Runtime.InteropServices.DllImportAttribute("addressof003dll.dll", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)>]
let M (x:nativeptr<int>) = 0                 // as per specs, the body of the let-binding is ignored...

let mutable m = 10

let x = M(&&m)

exit (if (x=11) && (m=11) then 0 else 1)
