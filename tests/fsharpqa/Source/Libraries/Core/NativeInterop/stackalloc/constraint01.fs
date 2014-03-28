// #Regression #Libraries #Stackalloc 
// Regression test for FSHARP1.0:5595
// stackalloc on imported types (C# struct and enum) -> OK
//<Expects status="success"></Expects>

let _ = NativeInterop.NativePtr.stackalloc<E> 1
let _ = NativeInterop.NativePtr.stackalloc<S> 1

exit 0
