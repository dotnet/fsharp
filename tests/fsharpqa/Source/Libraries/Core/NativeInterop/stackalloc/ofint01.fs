// #Libraries #Stackalloc 
#nowarn "9"
// Regression test for FSHARP1.0:
// stackalloc<int>
//<Expects status="success"></Expects>
module M1 = 

   let mutable noerr = true

   let data = NativeInterop.NativePtr.stackalloc<int> 100
   
   for i = 0 to 99 do
        NativeInterop.NativePtr.set data i (i*i)
        
   for i = 0 to 99 do
        if not (NativeInterop.NativePtr.get data i = (i*i)) then 
            noerr <- false

   for i = 0 to 99 do
        let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
        datai <- 1-i
    
   for i = 0 to 99 do
        let datai = NativeInterop.NativePtr.toByRef (NativeInterop.NativePtr.add data i)
        if not (datai = 1-i) then 
            noerr <- false

   (if noerr then 0 else 1) |> exit

