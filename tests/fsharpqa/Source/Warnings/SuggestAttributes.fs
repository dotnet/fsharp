// #Warnings
//<Expects status="Error" id="FS0039">The type 'AbstractClas' is not defined.</Expects>
//<Expects>Maybe you want one of the following:</Expects>
//<Expects>'AbstractClass</Expects>
//<Expects>'AbstractClassAttribute</Expects>

[<AbstractClas>]
type MyClass<'Bar>() =
   abstract M<'T> : 'T -> 'T
   abstract M2<'T> : 'T -> 'Bar


exit 0