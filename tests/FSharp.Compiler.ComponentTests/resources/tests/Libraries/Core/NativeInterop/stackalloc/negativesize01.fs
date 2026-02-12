// #Libraries #Stackalloc 
#nowarn "9"
module M5 =
// Regression test for FSHARP1.0:
// stackalloc<System.DateTime> -1
//<Expects status="success"></Expects>
     
   // This rightly causes a stack overflow exception (is interpreted as a unsigned int)
   let w = try
             let _ = NativeInterop.NativePtr.stackalloc<System.DateTime> -1
             1
           with
           | Failure(e) -> 1
