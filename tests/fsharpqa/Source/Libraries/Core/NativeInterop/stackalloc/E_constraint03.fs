// #Regression #Libraries #Stackalloc 
// Regression test for FSHARP1.0:5595
// stackalloc on managed types (record and class)
//<Expects status="error" span="(14,16-14,50)" id="FS0001">A generic construct requires that the type 'R' is an unmanaged type$</Expects>
//<Expects status="error" span="(15,17-15,51)" id="FS0001">A generic construct requires that the type 'C' is an unmanaged type$</Expects>
#nowarn "9"

    type R = { A : int }
    
    type C() = class
                        member __.M = 10
                        member __.N(x) = x + 1
                     end
    let data = NativeInterop.NativePtr.stackalloc<R> 10
    let data2 = NativeInterop.NativePtr.stackalloc<C> 10
