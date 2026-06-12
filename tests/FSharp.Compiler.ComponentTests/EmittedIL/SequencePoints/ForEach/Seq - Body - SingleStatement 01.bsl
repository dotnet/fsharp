Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  callvirt GetEnumerator
    IL_0008:  stloc.1
    IL_0009:  br.s IL_0018

  (5,5-5,10)  for i
    IL_000b:  ldloc.1
    IL_000c:  callvirt get_Current
    IL_0011:  stloc.2

  (6,9-6,35)  System.Console.WriteLine i
    IL_0012:  ldloc.2
    IL_0013:  call WriteLine

  (5,11-5,13)  in
    IL_0018:  ldloc.1
    IL_0019:  callvirt MoveNext
    IL_001e:  brtrue.s IL_000b
    IL_0020:  leave.s IL_0034
    IL_0022:  ldloc.1
    IL_0023:  isinst IDisposable
    IL_0028:  stloc.3

  <hidden>
    IL_0029:  ldloc.3
    IL_002a:  brfalse.s IL_0033

  <hidden>
    IL_002c:  ldloc.3
    IL_002d:  callvirt Dispose
    IL_0032:  endfinally

  <hidden>
    IL_0033:  endfinally

  <hidden>
    IL_0034:  ret
