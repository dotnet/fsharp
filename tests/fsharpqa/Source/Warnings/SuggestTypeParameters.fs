// #Warnings
//<Expects status="Error" id="FS0039">The type parameter 'B is not defined</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>'Bar</Expects>
//<Expects>'T</Expects>

[<AbstractClass>]
type MyClass<'Bar>() =
   abstract M<'T> : 'T -> 'T
   abstract M2<'T> : 'T -> 'Bar
   abstract M3<'T> : 'T -> 'B


exit 0