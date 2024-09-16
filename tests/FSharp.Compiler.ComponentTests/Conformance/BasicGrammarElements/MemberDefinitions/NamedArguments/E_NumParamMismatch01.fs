// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
// FSB 1433, Count of supplied parameters incorrect in error message if named parameters are used.




type IFoo = interface
    abstract NamedMeth1 : arg1:int * arg2:int * arg3:int * arg4:int-> float
end

type Foo() = class
    interface IFoo with
        member public this.NamedMeth1 (arg1, arg2, arg3, arg4) = 2.718
    member x.Stuff() = printfn "Foo"
end

let y = new Foo() :> IFoo
y.NamedMeth1(1, arg4=1, arg2=2)             // Too few
y.NamedMeth1(1, 2, arg4=1, arg2=2, arg1=0)   // Too many

