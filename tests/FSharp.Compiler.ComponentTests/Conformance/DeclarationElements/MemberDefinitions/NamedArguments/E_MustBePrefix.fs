// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
// Verify warning when named arguments do not form _prefix_ of unnamed arguments.
//<Expects id="FS0035" status="error" span="(16,1)">This construct is deprecated: The unnamed arguments do not form a prefix of the arguments of the method called</Expects>

type IFoo = interface
    abstract NamedMeth1 : arg1:int * arg2:int * arg3:int * arg4:int-> unit
end

type Foo() = class
    interface IFoo with
        member this.NamedMeth1 (arg1, arg2, arg3, arg4) = printfn "%A" (arg1, arg2, arg3, arg4)
    member x.Stuff() = printfn "Foo"
end
let y = new Foo() :> IFoo

y.NamedMeth1(1, 2, arg4=4, arg2=3)  // BUG: What about arg3?

// Prints (1, 3, 2, 4)
