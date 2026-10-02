open System.Threading.Tasks

[<EntryPoint>]
let main _ =
    let t = task { return 1 }
    if t.Result = 1 then 0 else 1
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

  (7,5-7,25)  if t.Result = 1 then
    IL_003d:  ldloc.0
    IL_003e:  callvirt get_Result
    IL_0043:  ldc.i4.1
    IL_0044:  bne.un.s IL_0048

  (7,26-7,27)  0
    IL_0046:  ldc.i4.0
    IL_0047:  ret

  (7,33-7,34)  1
    IL_0048:  ldc.i4.1
    IL_0049:  ret

t@6::MoveNext
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  ldfld t@6::ResumptionPoint
    IL_0006:  stloc.0

  (6,20-6,28)  return 1
    IL_0007:  ldc.i4.1
    IL_0008:  stloc.3
    IL_0009:  ldarg.0
    IL_000a:  ldflda t@6::Data
    IL_000f:  ldloc.3
    IL_0010:  stfld Result
    IL_0015:  ldc.i4.1
    IL_0016:  stloc.2
    IL_0017:  ldloc.2
    IL_0018:  brfalse.s IL_0037

  <hidden>
    IL_001a:  ldarg.0
    IL_001b:  ldflda t@6::Data
    IL_0020:  ldflda MethodBuilder
    IL_0025:  ldarg.0
    IL_0026:  ldflda t@6::Data
    IL_002b:  ldfld Result
    IL_0030:  call SetResult
    IL_0035:  leave.s IL_0045

  <hidden>
    IL_0037:  leave.s IL_0045
    IL_0039:  castclass Exception
    IL_003e:  stloc.s 4
    IL_0040:  ldloc.s 4
    IL_0042:  stloc.1
    IL_0043:  leave.s IL_0045

  <hidden>
    IL_0045:  ldloc.1
    IL_0046:  stloc.s 5
    IL_0048:  ldloc.s 5
    IL_004a:  brtrue.s IL_004d

  <hidden>
    IL_004c:  ret

  <hidden>
    IL_004d:  ldarg.0
    IL_004e:  ldflda t@6::Data
    IL_0053:  ldflda MethodBuilder
    IL_0058:  ldloc.s 5
    IL_005a:  call SetException
    IL_005f:  ret
