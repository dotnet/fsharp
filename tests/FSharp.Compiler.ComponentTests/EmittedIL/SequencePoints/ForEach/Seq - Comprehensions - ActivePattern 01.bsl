module Module

let (|Id|) (x: int) = x

let f (l: int list) =
    seq {
        for Id i in l do
            yield i
    }
--------------------------------------------------------------------------------

Module::|Id|
  (3,23-3,24)  x
    IL_0000:  ldarg.0
    IL_0001:  ret

Module::f
  (6,5-9,6)  seq { for Id i in l do yield i }
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  newobj f@7::.ctor
    IL_0009:  ret

f@7::GenerateNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld f@7::pc
    IL_0006:  ldc.i4.1
    IL_0007:  sub
    IL_0008:  switch (3 targets)
    IL_0019:  br.s IL_0024

  <hidden>
    IL_001b:  nop
    IL_001c:  br.s IL_0071

  <hidden>
    IL_001e:  nop
    IL_001f:  br.s IL_0064

  <hidden>
    IL_0021:  nop
    IL_0022:  br.s IL_0092

  <hidden>
    IL_0024:  nop

  (7,9-7,12)  for
    IL_0025:  ldarg.0
    IL_0026:  ldarg.0
    IL_0027:  ldfld f@7::l
    IL_002c:  callvirt GetEnumerator
    IL_0031:  stfld f@7::enum
    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.1
    IL_0038:  stfld f@7::pc
    IL_003d:  br.s IL_0064
    IL_003f:  ldarg.0
    IL_0040:  ldfld f@7::enum
    IL_0045:  callvirt get_Current
    IL_004a:  stloc.0
    IL_004b:  ldloc.0
    IL_004c:  call Module::|Id|
    IL_0051:  stloc.1
    IL_0052:  ldloc.1
    IL_0053:  stloc.2

  (8,13-8,20)  yield i
    IL_0054:  ldarg.0
    IL_0055:  ldc.i4.2
    IL_0056:  stfld f@7::pc
    IL_005b:  ldarg.0
    IL_005c:  ldloc.2
    IL_005d:  stfld f@7::current
    IL_0062:  ldc.i4.1
    IL_0063:  ret

  (7,18-7,20)  in
    IL_0064:  ldarg.0
    IL_0065:  ldfld f@7::enum
    IL_006a:  callvirt IEnumerator::MoveNext
    IL_006f:  brtrue.s IL_003f
    IL_0071:  ldarg.0
    IL_0072:  ldc.i4.3
    IL_0073:  stfld f@7::pc
    IL_0078:  ldarg.0
    IL_0079:  ldfld f@7::enum
    IL_007e:  call IntrinsicFunctions::Dispose
    IL_0083:  nop
    IL_0084:  ldarg.0
    IL_0085:  ldnull
    IL_0086:  stfld f@7::enum
    IL_008b:  ldarg.0
    IL_008c:  ldc.i4.3
    IL_008d:  stfld f@7::pc
    IL_0092:  ldarg.0
    IL_0093:  ldc.i4.0
    IL_0094:  stfld f@7::current
    IL_0099:  ldc.i4.0
    IL_009a:  ret

f@7::Close
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld f@7::pc
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
    IL_0018:  ldfld f@7::pc
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
    IL_0044:  stfld f@7::pc
    IL_0049:  ldarg.0
    IL_004a:  ldfld f@7::enum
    IL_004f:  call IntrinsicFunctions::Dispose
    IL_0054:  nop

  <hidden>
    IL_0055:  nop
    IL_0056:  ldarg.0
    IL_0057:  ldc.i4.3
    IL_0058:  stfld f@7::pc
    IL_005d:  ldarg.0
    IL_005e:  ldc.i4.0
    IL_005f:  stfld f@7::current
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

f@7::get_CheckClose
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld f@7::pc
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
