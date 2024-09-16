// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light



type Foo =
    static member DoStuff (a:int, b:int, c:int, d:int, e:int) = a + b + c + d + e

Foo.DoStuff(a=1, b=2, c=3, d=4, 5)
