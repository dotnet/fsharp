// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
// FSB 1430, Compiler spits out duplicate errors when given bogus syntax for named arguments
//<Expects id="FS0039" span="(8,47-8,51)" status="error">The value or constructor 'arg1' is not defined</Expects>
//<Expects id="FS0039" span="(8,54-8,58)" status="error">The value or constructor 'arg2' is not defined</Expects>
//<Expects id="FS0003" span="(12,1-12,8)" status="error">This value is not a function and cannot be applied</Expects>

type Foo() = class
    member this.Stuff : arg1:int * arg2:int = arg1 + arg2
end

let x = new Foo()
x.Stuff(arg2=2, arg1=1)
