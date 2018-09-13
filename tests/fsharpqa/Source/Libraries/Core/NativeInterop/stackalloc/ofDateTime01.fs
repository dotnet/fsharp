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
            
   let later = now.AddDays 1.
   for i = 0 to 99 do
        let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
        datai <- later
   for i = 0 to 99 do
        let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
        if not (datai = later) then 
            noerr <- false

   (if noerr then 0 else 1) |> exit
