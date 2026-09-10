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
    IL_0000:  call TaskBuilderModule::get_task
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  ldloc.1
    IL_0009:  newobj t@9::.ctor
    IL_000e:  callvirt TaskBuilderBase::Delay
    IL_0013:  callvirt TaskBuilder::Run
    IL_0018:  stloc.0

  (11,5-11,25)  if t.Result = 3 then
    IL_0019:  ldloc.0
    IL_001a:  callvirt get_Result
    IL_001f:  ldc.i4.3
    IL_0020:  bne.un.s IL_0024

  (11,26-11,27)  0
    IL_0022:  ldc.i4.0
    IL_0023:  ret

  (11,33-11,34)  1
    IL_0024:  ldc.i4.1
    IL_0025:  ret

t@8-1::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0

  (8,9-8,36)  let! y = Task.FromResult(2)
    IL_0002:  ldarg.0
    IL_0003:  ldfld t@8-1::builder@
    IL_0008:  ldc.i4.2
    IL_0009:  call Task::FromResult
    IL_000e:  ldarg.0
    IL_000f:  ldfld t@8-1::builder@
    IL_0014:  ldloc.0
    IL_0015:  newobj t@9-2::.ctor
    IL_001a:  tail.
    IL_001c:  call HighPriority::TaskBuilderBase.Bind
    IL_0021:  ret

t@9-2::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0

  (9,9-9,21)  return x + y
    IL_0002:  ldarg.0
    IL_0003:  ldfld t@9-2::builder@
    IL_0008:  ldarg.0
    IL_0009:  ldfld t@9-2::x
    IL_000e:  ldloc.0
    IL_000f:  add
    IL_0010:  tail.
    IL_0012:  callvirt TaskBuilderBase::Return
    IL_0017:  ret

t@9::Invoke
  (7,9-7,36)  let! x = Task.FromResult(1)
    IL_0000:  ldarg.0
    IL_0001:  ldfld t@9::builder@
    IL_0006:  ldc.i4.1
    IL_0007:  call Task::FromResult
    IL_000c:  ldarg.0
    IL_000d:  ldfld t@9::builder@
    IL_0012:  newobj t@8-1::.ctor
    IL_0017:  tail.
    IL_0019:  call HighPriority::TaskBuilderBase.Bind
    IL_001e:  ret
