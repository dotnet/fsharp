// #Libraries #Stackalloc 
#nowarn "9"
module M4 = 
// Regression test for FSHARP1.0:
// stackalloc<System.DateTime> 0
//<Expects status="success"></Expects>

   // check stackalloc 0 -- ok
   let data = NativeInterop.NativePtr.stackalloc<System.DateTime> 0

   // The returned pointer is undefined
   // No allocation should happen
   let q = NativeInterop.NativePtr.toNativeInt data

   
   exit 0
