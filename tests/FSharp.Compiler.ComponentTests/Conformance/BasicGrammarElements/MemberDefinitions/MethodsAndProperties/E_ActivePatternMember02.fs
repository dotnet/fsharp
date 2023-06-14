// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// Regression for FSHARP1.0:6168
// Active patterns should not be allowed as members - they don't work , but currently can be defined
//<Expects status="error" id="FS0827" span="(6,12-6,27)">This is not a valid name for an active pattern</Expects>
type FooBar2() = 
    member x.(|Foo|Bar|) y =

        match x = y with
        | true -> Foo
        | false -> Bar

    // compiler error on "Foo"    
    member x.doSomething y =
        let r = x.``|Foo|Bar|``  y   // compiles!
        match r with
        | Foo -> () // The type 'Choice<unit,unit>' is not compatible with the type 'FooBar2'
        | Bar -> ()
