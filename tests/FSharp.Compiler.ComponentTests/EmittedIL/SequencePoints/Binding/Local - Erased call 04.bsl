module Module

let i =
    ()
    System.Diagnostics.Debug.Write ""
    System.Diagnostics.Debug.Write ""
--------------------------------------------------------------------------------

Module::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld init@
    IL_0006:  ldsfld init@
    IL_000b:  pop
    IL_000c:  ret

Module::staticInitialization@
  (4,5-4,7)  ()
    IL_0000:  ldnull
    IL_0001:  stsfld i@3
    IL_0006:  ret
