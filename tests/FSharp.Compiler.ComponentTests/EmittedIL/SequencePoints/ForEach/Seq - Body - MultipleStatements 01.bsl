Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt GetEnumerator
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0020

  (5,5-5,10)  for i
    IL_000b:  ldloc.1
    IL_000c:  callvirt get_Current
    IL_0011:  stloc.2

  (6,9-6,35)  System.Console.WriteLine i
    IL_0012:  ldloc.2
    IL_0013:  call WriteLine

  (7,9-7,40)  System.Console.WriteLine(i + 1)
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.1
    IL_001a:  add
    IL_001b:  call WriteLine

  (5,11-5,13)  in
    IL_0020:  ldloc.1
    IL_0021:  callvirt MoveNext
    IL_0026:  brtrue.s IL_000b
    IL_0028:  leave.s IL_003c
    IL_002a:  ldloc.1
    IL_002b:  isinst IDisposable
    IL_0030:  stloc.3

  <hidden>
    IL_0031:  ldloc.3
    IL_0032:  brfalse.s IL_003b

  <hidden>
    IL_0034:  ldloc.3
    IL_0035:  callvirt Dispose
    IL_003a:  endfinally

  <hidden>
    IL_003b:  endfinally

  <hidden>
    IL_003c:  ret
