module Module

let f (l: (int * int) list) =
    seq {
        for n in l do
            yield n
    }
--------------------------------------------------------------------------------

Module::f
  (4,5-7,6)  seq { for n in l do yield n }
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldc.i4.0
    IL_0003:  ldnull
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
    IL_001c:  br.s IL_006c

  <hidden>
    IL_001e:  nop
    IL_001f:  br.s IL_005f

  <hidden>
    IL_0021:  nop
    IL_0022:  br.s IL_008d

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
    IL_003f:  br.s IL_005f
    IL_0041:  ldarg.0
    IL_0042:  ldfld enum
    IL_0047:  callvirt get_Current
    IL_004c:  stloc.0
    IL_004d:  ldarg.0
    IL_004e:  ldc.i4.2
    IL_004f:  stfld pc
    IL_0054:  ldarg.0
    IL_0055:  stloc.1

  (6,13-6,20)  yield n
    IL_0056:  ldloc.1
    IL_0057:  ldloc.0
    IL_0058:  stfld current
    IL_005d:  ldc.i4.1
    IL_005e:  ret

  (5,15-5,17)  in
    IL_005f:  ldarg.0
    IL_0060:  ldfld enum
    IL_0065:  callvirt MoveNext
    IL_006a:  brtrue.s IL_0041
    IL_006c:  ldarg.0
    IL_006d:  ldc.i4.3
    IL_006e:  stfld pc
    IL_0073:  ldarg.0
    IL_0074:  ldfld enum
    IL_0079:  call Dispose
    IL_007e:  nop
    IL_007f:  ldarg.0
    IL_0080:  ldnull
    IL_0081:  stfld enum
    IL_0086:  ldarg.0
    IL_0087:  ldc.i4.3
    IL_0088:  stfld pc
    IL_008d:  ldarg.0
    IL_008e:  ldnull
    IL_008f:  stfld current
    IL_0094:  ldc.i4.0
    IL_0095:  ret

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
    IL_0060:  ldnull
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
