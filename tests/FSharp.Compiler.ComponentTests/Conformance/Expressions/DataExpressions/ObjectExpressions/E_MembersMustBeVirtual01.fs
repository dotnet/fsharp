// #Regression #Conformance #DataExpressions #ObjectConstructors 
// FSB 1683, dispatch slot checking in object expression manages to match non-virtual member

//<Expects id="FS0767" status="error" span="(11,35-11,42)">The type Foo contains the member 'MyX' but it is not a virtual or abstract method that is available to override or implement.</Expects>
//<Expects id="FS0017" status="error" span="(11,37-11,40)">The member 'MyX: unit -> int' does not have the correct type to override any given virtual method$</Expects>
//<Expects id="FS0783" status="error" span="(11,16-11,19)">At least one override did not correctly implement its corresponding abstract member$</Expects>

type Foo(x : int) =
   member v.MyX() = x;;

let foo = {new Foo(3) with member v.MyX() = 4};;
printf "%d" (foo.MyX())
