open System.Threading.Tasks

[<EntryPoint>]
let main _ =
    let t = task { return 1 }
    if t.Result = 1 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,13-6,17)  task
    IL_0000:  call TaskBuilderModule::get_task
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  ldloc.1
    IL_0008:  ldloc.1
    IL_0009:  newobj t@6::.ctor
    IL_000e:  callvirt TaskBuilderBase::Delay
    IL_0013:  callvirt TaskBuilder::Run
    IL_0018:  stloc.0

  (7,5-7,25)  if t.Result = 1 then
    IL_0019:  ldloc.0
    IL_001a:  callvirt get_Result
    IL_001f:  ldc.i4.1
    IL_0020:  bne.un.s IL_0024

  (7,26-7,27)  0
    IL_0022:  ldc.i4.0
    IL_0023:  ret

  (7,33-7,34)  1
    IL_0024:  ldc.i4.1
    IL_0025:  ret

t@6::Invoke
  (6,20-6,28)  return 1
    IL_0000:  ldarg.0
    IL_0001:  ldfld t@6::builder@
    IL_0006:  ldc.i4.1
    IL_0007:  tail.
    IL_0009:  callvirt TaskBuilderBase::Return
    IL_000e:  ret
