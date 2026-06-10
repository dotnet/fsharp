module Module

let a =
    seq {
        for n in 1..10 do
            yield n
    }
--------------------------------------------------------------------------------

Module::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  ret

Module::staticInitialization@
  (4,5-7,6)  seq { for n in 1..10 do yield n }
    IL_0000:  ldnull
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  newobj a@5::.ctor
    IL_0008:  stsfld Module::a@3
    IL_000d:  ret

a@5::GenerateNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld a@5::pc
    IL_0006:  ldc.i4.1
    IL_0007:  sub
    IL_0008:  switch (3 targets)
    IL_0019:  br.s IL_0024

  <hidden>
    IL_001b:  nop
    IL_001c:  br.s IL_006f

  <hidden>
    IL_001e:  nop
    IL_001f:  br.s IL_0062

  <hidden>
    IL_0021:  nop
    IL_0022:  br.s IL_0090

  <hidden>
    IL_0024:  nop
    IL_0025:  br.s IL_0027

  (5,9-5,12)  for
    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.1
    IL_0029:  ldc.i4.1
    IL_002a:  ldc.i4.s 10
    IL_002c:  call OperatorIntrinsics::RangeInt32
    IL_0031:  callvirt GetEnumerator
    IL_0036:  stfld a@5::enum
    IL_003b:  ldarg.0
    IL_003c:  ldc.i4.1
    IL_003d:  stfld a@5::pc
    IL_0042:  br.s IL_0062
    IL_0044:  ldarg.0
    IL_0045:  ldfld a@5::enum
    IL_004a:  callvirt get_Current
    IL_004f:  stloc.0
    IL_0050:  ldarg.0
    IL_0051:  ldc.i4.2
    IL_0052:  stfld a@5::pc
    IL_0057:  ldarg.0
    IL_0058:  stloc.1

  (6,13-6,20)  yield n
    IL_0059:  ldloc.1
    IL_005a:  ldloc.0
    IL_005b:  stfld a@5::current
    IL_0060:  ldc.i4.1
    IL_0061:  ret

  (5,15-5,17)  in
    IL_0062:  ldarg.0
    IL_0063:  ldfld a@5::enum
    IL_0068:  callvirt IEnumerator::MoveNext
    IL_006d:  brtrue.s IL_0044
    IL_006f:  ldarg.0
    IL_0070:  ldc.i4.3
    IL_0071:  stfld a@5::pc
    IL_0076:  ldarg.0
    IL_0077:  ldfld a@5::enum
    IL_007c:  call IntrinsicFunctions::Dispose
    IL_0081:  nop
    IL_0082:  ldarg.0
    IL_0083:  ldnull
    IL_0084:  stfld a@5::enum
    IL_0089:  ldarg.0
    IL_008a:  ldc.i4.3
    IL_008b:  stfld a@5::pc
    IL_0090:  ldarg.0
    IL_0091:  ldc.i4.0
    IL_0092:  stfld a@5::current
    IL_0097:  ldc.i4.0
    IL_0098:  ret

a@5::Close
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld a@5::pc
    IL_0006:  ldc.i4.3
    IL_0007:  sub
    IL_0008:  switch (1 targets)
    IL_0011:  br.s IL_0016

  <hidden>
    IL_0013:  nop
    IL_0014:  br.s IL_0078

  <hidden>
    IL_0016:  nop

  <hidden>
    IL_0017:  ldarg.0
    IL_0018:  ldfld a@5::pc
    IL_001d:  switch (4 targets)
    IL_0032:  br.s IL_0040

  <hidden>
    IL_0034:  nop
    IL_0035:  br.s IL_0058

  <hidden>
    IL_0037:  nop
    IL_0038:  br.s IL_0044

  <hidden>
    IL_003a:  nop
    IL_003b:  br.s IL_0043

  <hidden>
    IL_003d:  nop
    IL_003e:  br.s IL_0058

  <hidden>
    IL_0040:  nop
    IL_0041:  br.s IL_0043

  <hidden>
    IL_0043:  nop
    IL_0044:  ldarg.0
    IL_0045:  ldc.i4.3
    IL_0046:  stfld a@5::pc
    IL_004b:  ldarg.0
    IL_004c:  ldfld a@5::enum
    IL_0051:  call IntrinsicFunctions::Dispose
    IL_0056:  nop

  <hidden>
    IL_0057:  nop
    IL_0058:  ldarg.0
    IL_0059:  ldc.i4.3
    IL_005a:  stfld a@5::pc
    IL_005f:  ldarg.0
    IL_0060:  ldc.i4.0
    IL_0061:  stfld a@5::current
    IL_0066:  leave.s IL_0072
    IL_0068:  castclass Exception
    IL_006d:  stloc.1
    IL_006e:  ldloc.1
    IL_006f:  stloc.0
    IL_0070:  leave.s IL_0072

  <hidden>
    IL_0072:  nop
    IL_0073:  br IL_0000

  <hidden>
    IL_0078:  ldloc.0
    IL_0079:  brfalse.s IL_007d

  <hidden>
    IL_007b:  ldloc.0
    IL_007c:  throw

  <hidden>
    IL_007d:  ret

  <hidden>

a@5::get_CheckClose
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld a@5::pc
    IL_0006:  switch (4 targets)
    IL_001b:  br.s IL_0029

  <hidden>
    IL_001d:  nop
    IL_001e:  br.s IL_0030

  <hidden>
    IL_0020:  nop
    IL_0021:  br.s IL_002e

  <hidden>
    IL_0023:  nop
    IL_0024:  br.s IL_002c

  <hidden>
    IL_0026:  nop
    IL_0027:  br.s IL_0030

  <hidden>
    IL_0029:  nop
    IL_002a:  br.s IL_002c

  <hidden>
    IL_002c:  ldc.i4.1
    IL_002d:  ret
    IL_002e:  ldc.i4.1
    IL_002f:  ret
    IL_0030:  ldc.i4.0
    IL_0031:  ret
