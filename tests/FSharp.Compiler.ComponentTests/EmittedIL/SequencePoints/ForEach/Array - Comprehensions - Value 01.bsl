module Module

let a =
    [|
        for n in 1..10 do
            yield n
    |]
--------------------------------------------------------------------------------

Module::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  ret

Module::staticInitialization@
  (3,1-7,7)  let a = [| for n in 1..10 do yield n |]
    IL_0000:  ldc.i4.s 10
    IL_0002:  conv.i8
    IL_0003:  conv.ovf.i.un
    IL_0004:  newarr Int32
    IL_0009:  stloc.0
    IL_000a:  ldc.i4.0
    IL_000b:  conv.i8
    IL_000c:  stloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  stloc.2
    IL_000f:  br.s IL_0029
    IL_0011:  ldloc.2
    IL_0012:  stloc.3

  (5,9-5,12)  for
    IL_0013:  ldloc.0
    IL_0014:  ldloc.1
    IL_0015:  conv.i
    IL_0016:  stloc.s 4
    IL_0018:  stloc.s 5

  (6,13-6,20)  yield n
    IL_001a:  ldloc.s 5
    IL_001c:  ldloc.s 4
    IL_001e:  ldloc.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.2
    IL_0021:  ldc.i4.1
    IL_0022:  add
    IL_0023:  stloc.2
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  conv.i8
    IL_0027:  add
    IL_0028:  stloc.1

  (5,15-5,17)  in
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.s 10
    IL_002c:  conv.i8
    IL_002d:  blt.un.s IL_0011
    IL_002f:  ldloc.0
    IL_0030:  stsfld Module::a@3
    IL_0035:  ret
