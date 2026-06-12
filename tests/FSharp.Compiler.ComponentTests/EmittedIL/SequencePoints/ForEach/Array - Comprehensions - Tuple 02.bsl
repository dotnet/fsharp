Module::f
  (5,5-8,7)  [| for i, i1 in l do yield i |]
    IL_0000:  nop

  (6,9-6,12)  for
    IL_0001:  ldarg.0
    IL_0002:  callvirt GetEnumerator
    IL_0007:  stloc.1
    IL_0008:  br.s IL_002b
    IL_000a:  ldloc.1
    IL_000b:  callvirt get_Current
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  call get_Item2
    IL_0017:  stloc.s 4
    IL_0019:  ldloc.3
    IL_001a:  call get_Item1
    IL_001f:  stloc.s 5

  (7,13-7,20)  yield i
    IL_0021:  ldloca.s 0
    IL_0023:  ldloc.s 5
    IL_0025:  call Add
    IL_002a:  nop

  (6,19-6,21)  in
    IL_002b:  ldloc.1
    IL_002c:  callvirt MoveNext
    IL_0031:  brtrue.s IL_000a
    IL_0033:  ldnull
    IL_0034:  stloc.2
    IL_0035:  leave.s IL_004c
    IL_0037:  ldloc.1
    IL_0038:  isinst IDisposable
    IL_003d:  stloc.s 6

  <hidden>
    IL_003f:  ldloc.s 6
    IL_0041:  brfalse.s IL_004b

  <hidden>
    IL_0043:  ldloc.s 6
    IL_0045:  callvirt Dispose
    IL_004a:  endfinally

  <hidden>
    IL_004b:  endfinally

  <hidden>
    IL_004c:  ldloc.2
    IL_004d:  pop
    IL_004e:  ldloca.s 0
    IL_0050:  call Close
    IL_0055:  ret
