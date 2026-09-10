module Module

let f (l: int seq) =
    for i in l do
        System.Console.WriteLine i
        System.Console.WriteLine(i + 1)
--------------------------------------------------------------------------------

Module::f
  (4,14-4,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt GetEnumerator
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0020

  (4,5-4,10)  for i
    IL_000b:  ldloc.1
    IL_000c:  callvirt get_Current
    IL_0011:  stloc.2

  (5,9-5,35)  System.Console.WriteLine i
    IL_0012:  ldloc.2
    IL_0013:  call Console::WriteLine

  (6,9-6,40)  System.Console.WriteLine(i + 1)
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  call Console::WriteLine

  (4,11-4,13)  in
    IL_0020:  ldloc.1
    IL_0021:  callvirt IEnumerator::MoveNext
    IL_0026:  brtrue.s IL_000b
    IL_0028:  leave.s IL_003c
    IL_002a:  ldloc.1
    IL_002b:  isinst IDisposable
    IL_0030:  stloc.3
    IL_0031:  ldloc.3
    IL_0032:  brfalse.s IL_003b

  <hidden>
    IL_0034:  ldloc.3
    IL_0035:  callvirt IDisposable::Dispose
    IL_003a:  endfinally

  <hidden>
    IL_003b:  endfinally

  <hidden>
    IL_003c:  ret
