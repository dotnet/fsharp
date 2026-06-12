module Module

let f () =
    System.Console.WriteLine ""
    System.Diagnostics.Debug.Write ""
--------------------------------------------------------------------------------

Module::f
  (4,5-4,32)  System.Console.WriteLine ""
    IL_0000:  ldstr ""
    IL_0005:  call WriteLine
    IL_000a:  ret
