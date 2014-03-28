// #Libraries #Stackalloc 
#nowarn "9"
module M3 = 
// Regression test for FSHARP1.0:
// stackalloc<System.DateTime> 
//<Expects status="success"></Expects>

   let mutable noerr = true

   let data = NativeInterop.NativePtr.stackalloc<System.DateTime> 100
   let now = System.DateTime.Now
   for i = 0 to 99 do
        NativeInterop.NativePtr.set data i now
   for i = 0 to 99 do
        if not (NativeInterop.NativePtr.get data i = now) then 
            noerr <- false
            
   (if noerr then 0 else 1) |> exit
