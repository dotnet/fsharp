//<Expects id="FS0001" status="error" span="(16,19)">This expression was expected to have type</Expects>
//<Expects>'A (liba, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'</Expects>
//<Expects>but here has type</Expects>
//<Expects>'A (libb, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'</Expects>


//<Expects id="FS0001" status="error" span="(21,19)">This expression was expected to have type</Expects>
//<Expects>'B<Microsoft.FSharp.Core.int> (liba, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'</Expects>
//<Expects>but here has type</Expects>
//<Expects>'B<Microsoft.FSharp.Core.int> (libb, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null)'</Expects>


let a = AMaker.makeA()
let otherA = OtherAMaker.makeOtherA()
printfn "%A %A" (a.GetType().AssemblyQualifiedName) (otherA.GetType().AssemblyQualifiedName)
printfn "%A" (a = otherA)

let b = AMaker.makeB<int>()
let otherB = OtherAMaker.makeOtherB<int>()
printfn "%A %A" (b.GetType().AssemblyQualifiedName) (otherB.GetType().AssemblyQualifiedName)
printfn "%A" (b = otherB)
 