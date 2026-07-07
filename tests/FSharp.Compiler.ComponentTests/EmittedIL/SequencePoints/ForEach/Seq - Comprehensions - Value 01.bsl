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
    IL_001c:  br.s IL_006d

  <hidden>
    IL_001e:  nop
    IL_001f:  br.s IL_0060

  <hidden>
    IL_0021:  nop
    IL_0022:  br.s IL_008e

  <hidden>
    IL_0024:  nop

  (5,9-5,12)  for
    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.1
    IL_0027:  ldc.i4.1
    IL_0028:  ldc.i4.s 10
    IL_002a:  call OperatorIntrinsics::RangeInt32
    IL_002f:  callvirt GetEnumerator
    IL_0034:  stfld a@5::enum
    IL_0039:  ldarg.0
    IL_003a:  ldc.i4.1
    IL_003b:  stfld a@5::pc
    IL_0040:  br.s IL_0060
    IL_0042:  ldarg.0
    IL_0043:  ldfld a@5::enum
    IL_0048:  callvirt get_Current
    IL_004d:  stloc.0
    IL_004e:  ldarg.0
    IL_004f:  ldc.i4.2
    IL_0050:  stfld a@5::pc
    IL_0055:  ldarg.0
    IL_0056:  stloc.1

  (6,13-6,20)  yield n
    IL_0057:  ldloc.1
    IL_0058:  ldloc.0
    IL_0059:  stfld a@5::current
    IL_005e:  ldc.i4.1
    IL_005f:  ret

  (5,15-5,17)  in
    IL_0060:  ldarg.0
    IL_0061:  ldfld a@5::enum
    IL_0066:  callvirt IEnumerator::MoveNext
    IL_006b:  brtrue.s IL_0042
    IL_006d:  ldarg.0
    IL_006e:  ldc.i4.3
    IL_006f:  stfld a@5::pc
    IL_0074:  ldarg.0
    IL_0075:  ldfld a@5::enum
    IL_007a:  call IntrinsicFunctions::Dispose
    IL_007f:  nop
    IL_0080:  ldarg.0
    IL_0081:  ldnull
    IL_0082:  stfld a@5::enum
    IL_0087:  ldarg.0
    IL_0088:  ldc.i4.3
    IL_0089:  stfld a@5::pc
    IL_008e:  ldarg.0
    IL_008f:  ldc.i4.0
    IL_0090:  stfld a@5::current
    IL_0095:  ldc.i4.0
    IL_0096:  ret

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
    IL_0014:  br.s IL_0076

  <hidden>
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  ldfld a@5::pc
    IL_001d:  switch (4 targets)
    IL_0032:  br.s IL_0040

  <hidden>
    IL_0034:  nop
    IL_0035:  br.s IL_0056

  <hidden>
    IL_0037:  nop
    IL_0038:  br.s IL_0042

  <hidden>
    IL_003a:  nop
    IL_003b:  br.s IL_0041

  <hidden>
    IL_003d:  nop
    IL_003e:  br.s IL_0056

  <hidden>
    IL_0040:  nop

  <hidden>
    IL_0041:  nop
    IL_0042:  ldarg.0
    IL_0043:  ldc.i4.3
    IL_0044:  stfld a@5::pc
    IL_0049:  ldarg.0
    IL_004a:  ldfld a@5::enum
    IL_004f:  call IntrinsicFunctions::Dispose
    IL_0054:  nop

  <hidden>
    IL_0055:  nop
    IL_0056:  ldarg.0
    IL_0057:  ldc.i4.3
    IL_0058:  stfld a@5::pc
    IL_005d:  ldarg.0
    IL_005e:  ldc.i4.0
    IL_005f:  stfld a@5::current
    IL_0064:  leave.s IL_0070
    IL_0066:  castclass Exception
    IL_006b:  stloc.1
    IL_006c:  ldloc.1
    IL_006d:  stloc.0
    IL_006e:  leave.s IL_0070

  <hidden>
    IL_0070:  nop
    IL_0071:  br IL_0000
    IL_0076:  ldloc.0
    IL_0077:  brfalse.s IL_007b

  <hidden>
    IL_0079:  ldloc.0
    IL_007a:  throw

  <hidden>
    IL_007b:  ret

a@5::get_CheckClose
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld a@5::pc
    IL_0006:  switch (4 targets)
    IL_001b:  br.s IL_0029

  <hidden>
    IL_001d:  nop
    IL_001e:  br.s IL_002e

  <hidden>
    IL_0020:  nop
    IL_0021:  br.s IL_002c

  <hidden>
    IL_0023:  nop
    IL_0024:  br.s IL_002a

  <hidden>
    IL_0026:  nop
    IL_0027:  br.s IL_002e

  <hidden>
    IL_0029:  nop
    IL_002a:  ldc.i4.1
    IL_002b:  ret
    IL_002c:  ldc.i4.1
    IL_002d:  ret
    IL_002e:  ldc.i4.0
    IL_002f:  ret
