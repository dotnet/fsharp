//<Expects id="FS0001" status="error">'A \(liba, Version=0\.0\.0\.0,</Expects>
//<Expects id="FS0001" status="error">'B<Microsoft.FSharp.Core.int> \(liba, Version=0\ .0\.0\.0,</Expects>


let a = AMaker.makeA()
let otherA = OtherAMaker.makeOtherA()
printfn "%A %A" (a.GetType().AssemblyQualifiedName) (otherA.GetType().AssemblyQualifiedName)
printfn "%A" (a = otherA)

let b = AMaker.makeB<int>()
let otherB = OtherAMaker.makeOtherB<int>()
printfn "%A %A" (b.GetType().AssemblyQualifiedName) (otherB.GetType().AssemblyQualifiedName)
printfn "%A" (b = otherB)
