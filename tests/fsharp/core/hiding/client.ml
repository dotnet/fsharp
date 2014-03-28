
#light

do Lib2.f1 Lib.x
do Lib2.f2 (Lib.x, Lib.x)

// Check some simple cases where a separately compiled client tries to access things whose 
// potentially-inlined-implementations might use private/internal fields, types and values
let x = Libv.MyRecord.Create(3)
printfn "x.TwiceX = %d" x.TwiceX
printfn "x.TopV = %d" x.TopV
printfn "x.X1 = %d" x.X1
printfn "x.X2 = %d" x.X2
    


printfn "useInternalValue = %A" (Lib3.useInternalValue())

printfn "rValue = %A" Lib3.rValue
printfn "useInternalField = %A" (Lib3.useInternalField(Lib3.rValue))
printfn "useInternalTag = %A" (Lib3.useInternalTag())
printfn "useInternalType = %A" (Lib3.useInternalType())

