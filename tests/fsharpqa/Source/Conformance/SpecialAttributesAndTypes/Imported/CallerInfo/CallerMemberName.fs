namespace Test

open System.Runtime.CompilerServices
open CSharpLib

type MyTy() =   
    static member GetCallerMemberName([<CallerMemberName>] ?memberName : string) =
        memberName
    
    static member GetCallerMemberNameProperty
        with get () = MyTy.GetCallerMemberName()
        
    member val GetCallerMemberNameProperty1 = MyTy.GetCallerMemberName() with get, set

module Program =
    [<EntryPoint>]
    let main (_:string[]) =
        if MyTy.GetCallerMemberName() <> Some("main") then
            failwith "Unexpected F# CallerMemberName"
        
        if MyTy.GetCallerMemberName("foo") <> Some("foo") then
            failwith "Unexpected F# CallerMemberName"
                
        if MyTy.GetCallerMemberNameProperty <> Some("GetCallerMemberNameProperty") then
            failwith "Unexpected F# CallerMemberName"
            
        if MyTy().GetCallerMemberNameProperty1 <> Some("GetCallerMemberNameProperty1") then
            failwith "Unexpected F# CallerMemberName"
                
        if CallerInfoTest.MemberName() <> "main" then
            failwith "Unexpected C# CallerMemberName"
            
        if CallerInfoTest.MemberName("foo") <> "foo" then
            failwith "Unexpected C# CallerMemberName"
            
        match CallerInfoTest.AllInfo(21) with
        | (_, _, "main") -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

        let getCallerLineNumber = CallerInfoTest.MemberName

        if () |> CallerInfoTest.MemberName <> "main" then
            failwith "Unexpected C# CallerMemberName"

        if getCallerLineNumber () <> "main" then
            failwith "Unexpected C# CallerMemberName"

        0