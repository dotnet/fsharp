// #Libraries #Stackalloc 
#nowarn "9"
module M2 =   
// Regression test for FSHARP1.0:
// stackalloc<int64>
//<Expects status="success"></Expects>
   let mutable noerr = true
   
   let data = NativeInterop.NativePtr.stackalloc<int64> 100
   
   for i = 0 to 99 do
        NativeInterop.NativePtr.set data i (int64 (i*i))
   for i = 0 to 99 do
        if not (NativeInterop.NativePtr.get data i = (int64 (i*i))) then 
            noerr <- false

   (if noerr then 0 else 1) |> exit

