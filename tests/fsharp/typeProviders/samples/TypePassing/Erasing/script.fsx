#r "type_passing_tp/bin/Debug/type_passing_tp.dll"

open Test

type MyRecord = 
    { Id: string }
    member x.TestInstanceMember(y:string) = y
    static member TestStaticMember(y:string) = y

type Test = TypePassing.TypePassingTP<MyRecord>

type Test2 = TypePassing.TypePassingTP<System.Int32>

type Test3 = TypePassing.TypePassingTP<int32>
