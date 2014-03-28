// Regression test for FSharp1.0:3597
// Title: Dev10: we should support the AssemblyKeyFile attribute (and also --keyfile should work)

open System.Reflection

[<assembly:AssemblyKeyFileAttribute("foo.snk")>]
do ()

exit 0