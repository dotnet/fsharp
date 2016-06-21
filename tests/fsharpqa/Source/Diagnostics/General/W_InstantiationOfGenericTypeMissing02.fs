// #Regression #Diagnostics 
// Regression test for FSHARP1.0:3286
//<Expects status="success"></Expects>
module M

type C3<'a>() = class
                     static member Default = new C3<'a>()
                end
C3.Default |> ignore        // warning (not expected)


type C2<'a>() = class
                     static member Create () = new C2<'a>()
                end
C2.Create() |> ignore       // no warning (expected, as per #3286)
