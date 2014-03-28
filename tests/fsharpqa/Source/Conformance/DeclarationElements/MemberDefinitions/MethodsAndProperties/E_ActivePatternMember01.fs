// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression for FSHARP1.0:6168
// Active patterns should not be allowed as members - they don't work , but currently can be defined
//<Expects status="error" id="FS0827" span="(10,19-10,37)">This is not a valid name for an active pattern</Expects>
//<Expects status="error" id="FS0039" span="(21,10-21,13)">The field, constructor or member 'Foo' is not defined</Expects>

module M

type FaaBor() = 
    static member (|Foo|Bar|) (x, y) =
        match x = y with
        | true -> Foo
        | false -> Bar

    member x.doSomething y =
        match x, y with
        | Foo -> ()  // compiles!  How is 'Foo' in scope?
        | Bar -> ()

match 1,2 with
| FaaBor.Foo -> printfn "hi"  // The field, constructor or member 'Foo' is not defined
| _ -> ()

exit 1
