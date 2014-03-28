// #Libraries #Stackalloc 
#nowarn "9"
module M6 =
// Regression test for FSHARP1.0:
// stackalloc<enum> 10
//<Expects status="success"></Expects>

    type E = | A = 1
             | B = 2
    
    let mutable noerr = true
       
    let data = NativeInterop.NativePtr.stackalloc<E> 10
    
    for i = 0 to 9 do
        NativeInterop.NativePtr.set data i (if (i % 2)=0 then E.A else E.B)
    
    for i = 0 to 9 do
         if not (NativeInterop.NativePtr.get data i = (if (i % 2)=0 then E.A else E.B)) then 
             noerr <- false

    (if noerr then 0 else 1) |> exit
