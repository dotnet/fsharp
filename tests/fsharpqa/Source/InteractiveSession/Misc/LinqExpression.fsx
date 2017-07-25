// #Regression #NoMT #FSI 

// Regression test for https://github.com/Microsoft/visualfsharp/issues/3376
type SomeType() = class end
type Test =
    static member ParseThis (f : System.Linq.Expressions.Expression<System.Func<SomeType>>) = ()
let foo = Test.ParseThis (fun () -> SomeType())

exit 0

// ';;' to end FSI session
;;
