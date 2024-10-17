// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
// FSB 1430, Compiler spits out duplicate errors when given bogus syntax for named arguments




type Foo() = class
    member this.Stuff : arg1:int * arg2:int = arg1 + arg2
end

let x = new Foo()
x.Stuff(arg2=2, arg1=1)
