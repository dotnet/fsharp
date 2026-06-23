module Module

let a =
    [
        for n in 1..10 do
            yield n
    ]
--------------------------------------------------------------------------------

Module::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  ret

Module::staticInitialization@
  (3,1-7,6)  let a = [ for n in 1..10 do yield n ]
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  br.s IL_001f
    IL_0007:  ldloc.2
    IL_0008:  stloc.3

  (5,9-5,12)  for
    IL_0009:  ldloca.s 0
    IL_000b:  stloc.s 4

  (6,13-6,20)  yield n
    IL_000d:  ldloc.s 4
    IL_000f:  ldloc.3
    IL_0010:  call Add
    IL_0015:  nop
    IL_0016:  ldloc.2
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.2
    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  conv.i8
    IL_001d:  add
    IL_001e:  stloc.1

  (5,15-5,17)  in
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.s 10
    IL_0022:  conv.i8
    IL_0023:  blt.un.s IL_0007
    IL_0025:  ldloca.s 0
    IL_0027:  call Close
    IL_002c:  stsfld Module::a@3
    IL_0031:  ret
