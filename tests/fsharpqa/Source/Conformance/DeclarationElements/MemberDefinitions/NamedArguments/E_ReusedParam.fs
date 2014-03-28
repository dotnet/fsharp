// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
//<Expects status="error" span="(7,1)" id="FS0364">A named argument has been assigned more than one value</Expects>

type Foo = 
    static member DoStuff (param1:int, param2:int) = param1 + param2
    
Foo.DoStuff(param1=1, param1=2, param2=3)
