// #Regression #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

//<Expects id="FS0691" span="(9,33)" status="error">Named arguments must appear after all other arguments</Expects>

type Foo =
    static member DoStuff (a:int, b:int, c:int, d:int, e:int) = a + b + c + d + e

Foo.DoStuff(a=1, b=2, c=3, d=4, 5)
