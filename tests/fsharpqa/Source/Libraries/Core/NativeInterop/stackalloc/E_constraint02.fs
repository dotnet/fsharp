// #Regression #Libraries #Stackalloc 
// Regression test for FSHARP1.0:5595
// stackalloc on imported types
//<Expects status="error" span="(10,9-10,43)" id="FS0001">A generic construct requires that the type 'C' is an unmanaged type$</Expects>
//<Expects status="error" span="(11,9-11,43)" id="FS0001">A generic construct requires that the type 'I' is an unmanaged type$</Expects>
//<Expects status="error" span="(12,9-12,43)" id="FS0001">A generic construct requires that the type 'D' is an unmanaged type$</Expects>

#nowarn "9"

let _ = NativeInterop.NativePtr.stackalloc<C> 1
let _ = NativeInterop.NativePtr.stackalloc<I> 1
let _ = NativeInterop.NativePtr.stackalloc<D> 1
