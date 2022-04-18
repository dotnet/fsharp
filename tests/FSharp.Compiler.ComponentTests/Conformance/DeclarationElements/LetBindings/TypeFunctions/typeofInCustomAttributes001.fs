// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:1490
// can't use typeof in attributes
//<Expects status="success"></Expects>

[<System.Diagnostics.DebuggerTypeProxy(typeof<TestTypeOnTypeView>)>]
type TestTypeOnType() = 
    class
       member x.P = 1
    end
    
and TestTypeOnTypeView() = 
    class
       member x.P = 1
    end
