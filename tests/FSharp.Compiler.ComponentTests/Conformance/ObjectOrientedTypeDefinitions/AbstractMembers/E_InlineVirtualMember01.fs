// #Regression #Conformance #ObjectOrientedTypes #MethodsAndProperties #MemberDefinitions 
// Dev11:175889, used to not be an error to define a class like this
//<Expects status="error" id="FS3151" span="(8,5-8,28)">This member, function or value declaration may not be declared 'inline'</Expects>
//<Expects status="error" id="FS3151" span="(12,5-12,35)">This member, function or value declaration may not be declared 'inline'</Expects>

[<AbstractClass>]
type Bad10 = 
    abstract inline X : int

[<AbstractClass>]
type Bad11 = 
    abstract inline X : int -> int
