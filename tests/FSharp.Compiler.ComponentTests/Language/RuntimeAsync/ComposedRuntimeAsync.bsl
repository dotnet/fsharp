module M
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core.CompilerServices

let helper x = x * 2

let outer (n: int) : Task<int> =
    let inner y = y + helper n
    let baseline = inner 10
    StateMachineHelpers.__runtimeAsyncReturn (
        let mutable total = baseline
        for i in 1 .. n do
            let d = AsyncHelpers.Await(Task.FromResult i)
            total <- total + d
        AsyncHelpers.Await(Task.Delay 1)
        if total > 0 then total else -1)
--------------------------------------------------------------------------------

M::helper
  (7,16-7,21)  x * 2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  mul
    IL_0003:  ret

M::outer
  <hidden>
    IL_0000:  ldarg.0
    IL_0001:  newobj inner@10::.ctor
    IL_0006:  stloc.0

  (11,5-11,28)  let baseline = inner 10
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s 10
    IL_000a:  callvirt Invoke
    IL_000f:  stloc.1

  (12,5-18,41)  StateMachineHelpers.__runtimeAsyncReturn ( let mutable total = baseline for i in 1 .. n do let d = AsyncHelpers.Await(Task.FromResult i) total <- total + d AsyncHelpers.Await(Task.Delay 1) if total > 0 then total else -1)
    IL_0010:  ldarg.0
    IL_0011:  ldloc.1
    IL_0012:  newobj outer@12::.ctor
    IL_0017:  ldnull
    IL_0018:  tail.
    IL_001a:  callvirt Invoke
    IL_001f:  ret

inner@10::Invoke
  (10,19-10,31)  y + helper n
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  ldfld inner@10::n
    IL_0007:  call M::helper
    IL_000c:  add
    IL_000d:  ret

outer@12::Invoke
  (13,9-13,37)  let mutable total = baseline
    IL_0000:  ldarg.0
    IL_0001:  ldfld outer@12::baseline
    IL_0006:  stloc.0

  (14,9-14,12)  for
    IL_0007:  ldc.i4.1
    IL_0008:  stloc.2
    IL_0009:  ldarg.0
    IL_000a:  ldfld outer@12::n
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  ldloc.2
    IL_0012:  blt.s IL_002e

  (15,13-15,58)  let d = AsyncHelpers.Await(Task.FromResult i)
    IL_0014:  ldloc.2
    IL_0015:  call Task::FromResult
    IL_001a:  call AsyncHelpers::Await
    IL_001f:  stloc.3

  (16,13-16,31)  total <- total + d
    IL_0020:  ldloc.0
    IL_0021:  ldloc.3
    IL_0022:  add
    IL_0023:  stloc.0

  <hidden>
    IL_0024:  ldloc.2
    IL_0025:  ldc.i4.1
    IL_0026:  add
    IL_0027:  stloc.2

  (14,15-14,17)  in
    IL_0028:  ldloc.2
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  add
    IL_002c:  bne.un.s IL_0014

  (17,9-17,41)  AsyncHelpers.Await(Task.Delay 1)
    IL_002e:  ldc.i4.1
    IL_002f:  call Task::Delay
    IL_0034:  call AsyncHelpers::Await

  (18,9-18,26)  if total > 0 then
    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.0
    IL_003b:  ble.s IL_003f

  (18,27-18,32)  total
    IL_003d:  ldloc.0
    IL_003e:  ret

  (18,38-18,40)  -1
    IL_003f:  ldc.i4.m1
    IL_0040:  ret
