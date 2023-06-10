// #Regression #Conformance #DeclarationElements #Accessibility #MethodsAndProperties #MemberDefinitions 
// Regression test for FSharp1.0:4169
// Title: Accessibility modifier in front of property is ignored if either get() or set() is mentioned explicitly
// Verify 'private' is honored everywhere expected and is not accessible

//<Expects status="error" id="FS0491">The member or object constructor 'Foo' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test1' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test2' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test5' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test6' is not accessible</Expects>
//<Expects status="error" id="FS0807">Property 'test8' is not readable</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test3' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test4' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test5' is not accessible</Expects>
//<Expects status="error" id="FS0491">The member or object constructor 'test6' is not accessible</Expects>
//<Expects status="error" id="FS0810">Property 'test7' cannot be set</Expects>

type T() =
    member private this.Foo = 0

    // Getters
    member this.test1 with private get () = 0
    member private this.test2 with get () = 0
    
    // Setters
    member this.test3 with private set (x : int) = ()
    member private this.test4 with set (x : int) = ()
    
    // Getters & Setters together
    member this.test5 with private get () = 0
                       and private set (x : int) = ()
    member private this.test6 with get () = 0
                              and set (x : int) = ()
                              
    // Different accessibility on getter and setter
    member this.test7 with public get () = 0
                       and private set (x : int) = ()
    member this.test8 with private get () = 0
                       and public set (x : int) = ()
                        
let a = T()
a.Foo + a.test1 + a.test2 + a.test5  + a.test6 + a.test7 + a.test8 |> ignore
a.test3 <- 0
a.test4 <- 0
a.test5 <- 0
a.test6 <- 0
a.test7 <- 0
a.test8 <- 0
