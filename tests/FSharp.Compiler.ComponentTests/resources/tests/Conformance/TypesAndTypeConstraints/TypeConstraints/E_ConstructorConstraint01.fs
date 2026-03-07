// #Conformance #TypeConstraints #Regression
// Originally written for Dev11:40389
// Now, it is just illegal to use '.ctor' or '.cctor' as member names

//<Expects status="error" span="(8,50)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>
//<Expects status="error" span="(9,25)" id="FS3066">Invalid member name\. Members may not have name '\.ctor' or '\.cctor'$</Expects>

let inline ctor< ^R, ^T when ^R : (static member ``.ctor`` : ^T -> ^R)> (arg:^T) =
   (^R : (static member ``.ctor`` : ^T -> ^R) arg)

let rnd = ctor<System.Random, _> 10
rnd.Next(10) |> ignore

exit 1