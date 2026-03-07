// #inline #FSharpQA #Conformance #TypeConstraints  
//<Expects status="error" id="FS3151" span="(7,5-7,28)">This member, function or value declaration may not be declared 'inline'</Expects>
//<Expects status="error" id="FS3151" span="(12,5-12,35)">This member, function or value declaration may not be declared 'inline'</Expects>

[<AbstractClass>]
type Bad10 = 
    abstract inline X : int


[<AbstractClass>]
type Bad11 = 
    abstract inline X : int -> int