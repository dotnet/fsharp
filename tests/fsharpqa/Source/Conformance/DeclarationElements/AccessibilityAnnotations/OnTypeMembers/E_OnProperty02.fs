// #Regression #Conformance #DeclarationElements #Accessibility #MethodsAndProperties #MemberDefinitions 
// Regression test for FSharp1.0:4169
// Title: Accessibility modifier in front of property is ignored if either get() or set() is mentioned explicitly
// Verify accessibility annotations can not be duplicated in various places with properties

//<Expects status="error" id="FS0010" span="(15,49-15,50)">Unexpected symbol '\)' in pattern</Expects>
//<Expects status="error" id="FS0558" span="(16,36-16,50)">Multiple accessibilities given for property getter or setter</Expects>
//<Expects status="error" id="FS0558" span="(19,35-19,56)">Multiple accessibilities given for property getter or setter</Expects>
//<Expects status="error" id="FS0010" span="(20,49-20,50)">Unexpected identifier in pattern</Expects>
//<Expects status="error" id="FS0010" span="(23,36-23,42)">Unexpected keyword 'public' in member definition</Expects>

type T() =

    // Getters
    member this.test1 with private get private () = 0
    member private this.test2 with private get () = 0
    
    // Setters
    member public this.test3 with private set (x : int) = ()
    member private this.test4 with set private (x : int) = ()
    
    // Getters & Setters together
    member this.test5 with private public get () = 0
