module Module

let f () =
    System.Diagnostics.Debug.Write ""
    System.Console.WriteLine ""
--------------------------------------------------------------------------------

Module::f
  (5,5-5,32)  System.Console.WriteLine ""
    IL_0000:  ldstr ""
    IL_0005:  call Console::WriteLine
    IL_000a:  ret
