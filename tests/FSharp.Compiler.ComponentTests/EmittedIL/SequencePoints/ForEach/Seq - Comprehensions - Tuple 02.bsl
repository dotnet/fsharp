module Module

let f (l: (int * int) list) =
    seq {
        for i, i1 in l do
            yield i
    }
--------------------------------------------------------------------------------

Module::f
  (4,5-7,6)  seq { for i, i1 in l do yield i }
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  newobj .ctor
    IL_0009:  ret

f@5::GenerateNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld pc
    IL_0006:  ldc.i4.1
    IL_0007:  sub
    IL_0008:  switch (3 targets)
    IL_0019:  br.s IL_0024

  <hidden>
    IL_001b:  nop
    IL_001c:  br.s IL_0078

  <hidden>
    IL_001e:  nop
    IL_001f:  br.s IL_006b

  <hidden>
    IL_0021:  nop
    IL_0022:  br.s IL_0099

  <hidden>
    IL_0024:  nop
    IL_0025:  br.s IL_0027

  (5,9-5,12)  for
    IL_0027:  ldarg.0
    IL_0028:  ldarg.0
    IL_0029:  ldfld l
    IL_002e:  callvirt GetEnumerator
    IL_0033:  stfld enum
    IL_0038:  ldarg.0
    IL_0039:  ldc.i4.1
    IL_003a:  stfld pc
    IL_003f:  br.s IL_006b
    IL_0041:  ldarg.0
    IL_0042:  ldfld enum
    IL_0047:  callvirt get_Current
    IL_004c:  stloc.0
    IL_004d:  ldloc.0
    IL_004e:  call get_Item2
    IL_0053:  stloc.1
    IL_0054:  ldloc.0
    IL_0055:  call get_Item1
    IL_005a:  stloc.2

  (6,13-6,20)  yield i
    IL_005b:  ldarg.0
    IL_005c:  ldc.i4.2
    IL_005d:  stfld pc
    IL_0062:  ldarg.0
    IL_0063:  ldloc.2
    IL_0064:  stfld current
    IL_0069:  ldc.i4.1
    IL_006a:  ret

  (5,19-5,21)  in
    IL_006b:  ldarg.0
    IL_006c:  ldfld enum
    IL_0071:  callvirt MoveNext
    IL_0076:  brtrue.s IL_0041
    IL_0078:  ldarg.0
    IL_0079:  ldc.i4.3
    IL_007a:  stfld pc
    IL_007f:  ldarg.0
    IL_0080:  ldfld enum
    IL_0085:  call Dispose
    IL_008a:  nop
    IL_008b:  ldarg.0
    IL_008c:  ldnull
    IL_008d:  stfld enum
    IL_0092:  ldarg.0
    IL_0093:  ldc.i4.3
    IL_0094:  stfld pc
    IL_0099:  ldarg.0
    IL_009a:  ldc.i4.0
    IL_009b:  stfld current
    IL_00a0:  ldc.i4.0
    IL_00a1:  ret

f@5::Close
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld pc
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
    IL_0018:  ldfld pc
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
    IL_0046:  stfld pc
    IL_004b:  ldarg.0
    IL_004c:  ldfld enum
    IL_0051:  call Dispose
    IL_0056:  nop

  <hidden>
    IL_0057:  nop
    IL_0058:  ldarg.0
    IL_0059:  ldc.i4.3
    IL_005a:  stfld pc
    IL_005f:  ldarg.0
    IL_0060:  ldc.i4.0
    IL_0061:  stfld current
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

f@5::get_CheckClose
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld pc
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
