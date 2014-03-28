// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3286
//<Expects id="FS1125" span="(10,1-10,11)"   status="warning">The instantiation of the generic type 'C1' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'C1<_>'</Expects>
//<Expects id="FS1125" span="(15,1-15,14)" status="warning">The instantiation of the generic type 'C2' is missing and can't be inferred from the arguments or return type of this member\. Consider providing a type instantiation when accessing this type, e\.g\. 'C2<_>'</Expects>
module M

type C1<'a>() = 
         static member SizeOfA = sizeof<'a> // warning on use C.SizeOfA

C1.SizeOfA |> ignore

type C2<'a> =
         static member GetSizeOfA () = sizeof<'a>  // warning on use C2.GetSizeOfA

C2.GetSizeOfA() |> ignore

exit 0
