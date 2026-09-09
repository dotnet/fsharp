open System.Threading.Tasks

[<EntryPoint>]
let main _ =
    let t = task {
        let! x = Task.FromResult(1)
        let! y = Task.FromResult(2)
        return x + y
    }
    if t.Result = 3 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,13-6,17)  task
    IL_0000:  ldloca.s 1
    IL_0002:  initobj t@6
    IL_0008:  ldloca.s 1
    IL_000a:  stloc.2
    IL_000b:  ldloc.2
    IL_000c:  ldflda t@6::Data
    IL_0011:  call Create
    IL_0016:  stfld MethodBuilder
    IL_001b:  ldloc.2
    IL_001c:  ldflda t@6::Data
    IL_0021:  ldflda MethodBuilder
    IL_0026:  ldloc.2
    IL_0027:  call Start
    IL_002c:  ldloc.2
    IL_002d:  ldflda t@6::Data
    IL_0032:  ldflda MethodBuilder
    IL_0037:  call get_Task
    IL_003c:  stloc.0

  (11,5-11,25)  if t.Result = 3 then
    IL_003d:  ldloc.0
    IL_003e:  callvirt get_Result
    IL_0043:  ldc.i4.3
    IL_0044:  bne.un.s IL_0048

  (11,26-11,27)  0
    IL_0046:  ldc.i4.0
    IL_0047:  ret

  (11,33-11,34)  1
    IL_0048:  ldc.i4.1
    IL_0049:  ret

t@6::MoveNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld t@6::ResumptionPoint
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  sub
    IL_000a:  switch (2 targets)
    IL_0017:  br.s IL_001f

  <hidden>
    IL_0019:  nop
    IL_001a:  br.s IL_0020

  <hidden>
    IL_001c:  nop
    IL_001d:  br.s IL_0020

  <hidden>
    IL_001f:  nop
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.1
    IL_0022:  sub
    IL_0023:  switch (2 targets)
    IL_0030:  br.s IL_003b

  <hidden>
    IL_0032:  nop
    IL_0033:  br.s IL_0064

  <hidden>
    IL_0035:  nop
    IL_0036:  br IL_00c5

  <hidden>
    IL_003b:  nop

  (7,9-7,36)  let! x = Task.FromResult(1)
    IL_003c:  ldc.i4.1
    IL_003d:  call Task::FromResult
    IL_0042:  stloc.3
    IL_0043:  ldarg.0
    IL_0044:  ldloc.3
    IL_0045:  callvirt GetAwaiter
    IL_004a:  stfld t@6::awaiter0
    IL_004f:  ldc.i4.1
    IL_0050:  stloc.s 4
    IL_0052:  ldarg.0
    IL_0053:  ldflda t@6::awaiter0
    IL_0058:  call get_IsCompleted
    IL_005d:  brfalse.s IL_0061
    IL_005f:  br.s IL_007a

  <hidden>
    IL_0061:  ldc.i4.0
    IL_0062:  brfalse.s IL_0068

  <hidden>
    IL_0064:  ldc.i4.1

  <hidden>
    IL_0065:  nop
    IL_0066:  br.s IL_0071

  <hidden>
    IL_0068:  ldarg.0
    IL_0069:  ldc.i4.1
    IL_006a:  stfld t@6::ResumptionPoint
    IL_006f:  ldc.i4.0

  <hidden>
    IL_0070:  nop
    IL_0071:  stloc.s 5
    IL_0073:  ldloc.s 5
    IL_0075:  stloc.s 4

  <hidden>
    IL_0077:  nop
    IL_0078:  br.s IL_007b

  <hidden>
    IL_007a:  nop
    IL_007b:  ldloc.s 4
    IL_007d:  brfalse IL_014b

  <hidden>
    IL_0082:  ldarg.0
    IL_0083:  ldflda t@6::awaiter0
    IL_0088:  call GetResult
    IL_008d:  stloc.s 6
    IL_008f:  ldloc.s 6
    IL_0091:  stloc.s 7
    IL_0093:  ldarg.0
    IL_0094:  ldloc.s 7
    IL_0096:  stfld t@6::x

  (8,9-8,36)  let! y = Task.FromResult(2)
    IL_009b:  ldc.i4.2
    IL_009c:  call Task::FromResult
    IL_00a1:  stloc.s 8
    IL_00a3:  ldarg.0
    IL_00a4:  ldloc.s 8
    IL_00a6:  callvirt GetAwaiter
    IL_00ab:  stfld t@6::awaiter
    IL_00b0:  ldc.i4.1
    IL_00b1:  stloc.s 9
    IL_00b3:  ldarg.0
    IL_00b4:  ldflda t@6::awaiter
    IL_00b9:  call get_IsCompleted
    IL_00be:  brfalse.s IL_00c2
    IL_00c0:  br.s IL_00db

  <hidden>
    IL_00c2:  ldc.i4.0
    IL_00c3:  brfalse.s IL_00c9

  <hidden>
    IL_00c5:  ldc.i4.1

  <hidden>
    IL_00c6:  nop
    IL_00c7:  br.s IL_00d2

  <hidden>
    IL_00c9:  ldarg.0
    IL_00ca:  ldc.i4.2
    IL_00cb:  stfld t@6::ResumptionPoint
    IL_00d0:  ldc.i4.0

  <hidden>
    IL_00d1:  nop
    IL_00d2:  stloc.s 10
    IL_00d4:  ldloc.s 10
    IL_00d6:  stloc.s 9

  <hidden>
    IL_00d8:  nop
    IL_00d9:  br.s IL_00dc

  <hidden>
    IL_00db:  nop
    IL_00dc:  ldloc.s 9
    IL_00de:  brfalse.s IL_0111

  <hidden>
    IL_00e0:  ldarg.0
    IL_00e1:  ldflda t@6::awaiter
    IL_00e6:  call GetResult
    IL_00eb:  stloc.s 11
    IL_00ed:  ldloc.s 11
    IL_00ef:  stloc.s 12
    IL_00f1:  ldloc.s 12
    IL_00f3:  stloc.s 13

  (9,9-9,21)  return x + y
    IL_00f5:  ldarg.0
    IL_00f6:  ldfld t@6::x
    IL_00fb:  ldloc.s 13
    IL_00fd:  add
    IL_00fe:  stloc.s 14
    IL_0100:  ldarg.0
    IL_0101:  ldflda t@6::Data
    IL_0106:  ldloc.s 14
    IL_0108:  stfld Result
    IL_010d:  ldc.i4.1

  <hidden>
    IL_010e:  nop
    IL_010f:  br.s IL_012a

  <hidden>
    IL_0111:  ldarg.0
    IL_0112:  ldflda t@6::Data
    IL_0117:  ldflda MethodBuilder
    IL_011c:  ldarg.0
    IL_011d:  ldflda t@6::awaiter
    IL_0122:  ldarg.0
    IL_0123:  call AwaitUnsafeOnCompleted
    IL_0128:  ldc.i4.0

  <hidden>
    IL_0129:  nop
    IL_012a:  brfalse.s IL_0138

  <hidden>
    IL_012c:  ldarg.0
    IL_012d:  ldloc.s 15
    IL_012f:  stfld t@6::awaiter
    IL_0134:  ldc.i4.1

  <hidden>
    IL_0135:  nop
    IL_0136:  br.s IL_013a

  <hidden>
    IL_0138:  ldc.i4.0

  <hidden>
    IL_0139:  nop
    IL_013a:  brfalse.s IL_0147

  <hidden>
    IL_013c:  ldarg.0
    IL_013d:  ldc.i4.0
    IL_013e:  stfld t@6::x
    IL_0143:  ldc.i4.1

  <hidden>
    IL_0144:  nop
    IL_0145:  br.s IL_0164

  <hidden>
    IL_0147:  ldc.i4.0

  <hidden>
    IL_0148:  nop
    IL_0149:  br.s IL_0164

  <hidden>
    IL_014b:  ldarg.0
    IL_014c:  ldflda t@6::Data
    IL_0151:  ldflda MethodBuilder
    IL_0156:  ldarg.0
    IL_0157:  ldflda t@6::awaiter0
    IL_015c:  ldarg.0
    IL_015d:  call AwaitUnsafeOnCompleted
    IL_0162:  ldc.i4.0

  <hidden>
    IL_0163:  nop
    IL_0164:  brfalse.s IL_0172

  <hidden>
    IL_0166:  ldarg.0
    IL_0167:  ldloc.s 16
    IL_0169:  stfld t@6::awaiter0
    IL_016e:  ldc.i4.1

  <hidden>
    IL_016f:  nop
    IL_0170:  br.s IL_0174

  <hidden>
    IL_0172:  ldc.i4.0

  <hidden>
    IL_0173:  nop
    IL_0174:  stloc.2
    IL_0175:  ldloc.2
    IL_0176:  brfalse.s IL_0195

  <hidden>
    IL_0178:  ldarg.0
    IL_0179:  ldflda t@6::Data
    IL_017e:  ldflda MethodBuilder
    IL_0183:  ldarg.0
    IL_0184:  ldflda t@6::Data
    IL_0189:  ldfld Result
    IL_018e:  call SetResult
    IL_0193:  leave.s IL_01a3

  <hidden>
    IL_0195:  leave.s IL_01a3
    IL_0197:  castclass Exception
    IL_019c:  stloc.s 17
    IL_019e:  ldloc.s 17
    IL_01a0:  stloc.1
    IL_01a1:  leave.s IL_01a3

  <hidden>
    IL_01a3:  ldloc.1
    IL_01a4:  stloc.s 18
    IL_01a6:  ldloc.s 18
    IL_01a8:  brtrue.s IL_01ab

  <hidden>
    IL_01aa:  ret

  <hidden>
    IL_01ab:  ldarg.0
    IL_01ac:  ldflda t@6::Data
    IL_01b1:  ldflda MethodBuilder
    IL_01b6:  ldloc.s 18
    IL_01b8:  call SetException
    IL_01bd:  ret
