namespace TestLibrary

open Lib

type Helper() =

    static member GetMessage () = Lib.message ()

    static member SayHi () = Lib.sayHi ()
