module Module

let f (l: int[]) =
    for i in l do
        System.Console.WriteLine i
--------------------------------------------------------------------------------

Module::f
  (4,14-4,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s IL_0014

  (4,5-4,10)  for i
    IL_0006:  ldloc.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem.i4
    IL_0009:  stloc.2

  (5,9-5,35)  System.Console.WriteLine i
    IL_000a:  ldloc.2
    IL_000b:  call WriteLine

  <hidden>
    IL_0010:  ldloc.1
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.1

  (4,11-4,13)  in
    IL_0014:  ldloc.1
    IL_0015:  ldloc.0
    IL_0016:  ldlen
    IL_0017:  conv.i4
    IL_0018:  blt.s IL_0006
    IL_001a:  ret
