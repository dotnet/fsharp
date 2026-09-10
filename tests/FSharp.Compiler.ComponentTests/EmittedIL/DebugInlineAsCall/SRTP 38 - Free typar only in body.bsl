let inline mk< 'T, ^U when ^U : (static member op_Explicit: ^U -> int) > (y: ^U) : obj =
    let arr : 'T[] = Array.zeroCreate (int y)
    box arr

let outer<'a> () = mk<'a, byte> 3uy

[<EntryPoint>]
let main _ =
    match outer<System.DateTime> () with
    | :? (System.DateTime[]) as a when a.Length = 3 -> 0
    | o -> printfn "Unexpected %s" (o.GetType().FullName); 1
--------------------------------------------------------------------------------

Test::mk
  (3,5-3,46)  let arr : 'T[] = Array.zeroCreate (int y)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldloc.1
    IL_0003:  call LanguagePrimitives::ExplicitDynamic
    IL_0008:  call ArrayModule::ZeroCreate
    IL_000d:  stloc.0

  (4,5-4,12)  box arr
    IL_000e:  ldloc.0
    IL_000f:  box 0x1b000001
    IL_0014:  ret

Test::mk$W
  (3,5-3,46)  let arr : 'T[] = Array.zeroCreate (int y)
    IL_0000:  ldarg.1
    IL_0001:  stloc.1
    IL_0002:  ldarg.0
    IL_0003:  ldloc.1
    IL_0004:  callvirt Invoke
    IL_0009:  call ArrayModule::ZeroCreate
    IL_000e:  stloc.0

  (4,5-4,12)  box arr
    IL_000f:  ldloc.0
    IL_0010:  box 0x1b000001
    IL_0015:  ret

Test::outer
  (6,20-6,36)  mk<'a, byte> 3uy
    IL_0000:  ldc.i4.3
    IL_0001:  tail.
    IL_0003:  call Test::<mk>__debug@6
    IL_0008:  ret

Test::main
  (10,5-10,41)  match outer<System.DateTime> () with
    IL_0000:  call Test::outer
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  isinst 0x1b000003
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brfalse.s IL_001c
    IL_0010:  ldloc.1
    IL_0011:  stloc.2

  (11,40-11,52)  a.Length = 3
    IL_0012:  ldloc.2
    IL_0013:  ldlen
    IL_0014:  conv.i4
    IL_0015:  ldc.i4.3
    IL_0016:  ceq
    IL_0018:  brfalse.s IL_0025
    IL_001a:  br.s IL_0021

  <hidden>
    IL_001c:  ldloc.0
    IL_001d:  stloc.s 4
    IL_001f:  br.s IL_0028

  <hidden>
    IL_0021:  ldloc.1
    IL_0022:  stloc.3

  (11,56-11,57)  0
    IL_0023:  ldc.i4.0
    IL_0024:  ret

  <hidden>
    IL_0025:  ldloc.0
    IL_0026:  stloc.s 4

  (12,12-12,58)  printfn "Unexpected %s" (o.GetType().FullName)
    IL_0028:  ldstr "Unexpected %s"
    IL_002d:  newobj .ctor
    IL_0032:  call ExtraTopLevelOperators::PrintFormatLine
    IL_0037:  ldloc.s 4
    IL_0039:  callvirt Object::GetType
    IL_003e:  callvirt Type::get_FullName
    IL_0043:  callvirt Invoke
    IL_0048:  pop

  (12,60-12,61)  1
    IL_0049:  ldc.i4.1
    IL_004a:  ret

Test::<mk>__debug@6
  (3,5-3,46)  let arr : 'T[] = Array.zeroCreate (int y)
    IL_0000:  ldarg.0
    IL_0001:  conv.i4
    IL_0002:  call ArrayModule::ZeroCreate
    IL_0007:  stloc.0

  (4,5-4,12)  box arr
    IL_0008:  ldloc.0
    IL_0009:  box 0x1b000001
    IL_000e:  ret
