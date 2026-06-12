Module::f
  (5,14-5,15)  l
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.2
    IL_0004:  ldloc.0
    IL_0005:  callvirt get_Length
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldloc.2
    IL_000f:  blt.s IL_0029

  (5,5-5,10)  for c
    IL_0011:  ldloc.0
    IL_0012:  ldloc.2
    IL_0013:  callvirt get_Chars
    IL_0018:  stloc.3

  (6,9-6,35)  System.Console.WriteLine c
    IL_0019:  ldloc.3
    IL_001a:  call WriteLine

  <hidden>
    IL_001f:  ldloc.2
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stloc.2

  (5,11-5,13)  in
    IL_0023:  ldloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  add
    IL_0027:  bne.un.s IL_0011
    IL_0029:  ret
