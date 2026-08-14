open MyLib

[<EntryPoint>]
let main _ =
    if outer 42 = 43 then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (6,5-6,26)  if outer 42 = 43 then
    IL_0000:  ldc.i4.s 42
    IL_0002:  call MyLib::outer
    IL_0007:  ldc.i4.s 43
    IL_0009:  bne.un.s IL_000d

  (6,27-6,28)  0
    IL_000b:  ldc.i4.0
    IL_000c:  ret

  (6,34-6,35)  1
    IL_000d:  ldc.i4.1
    IL_000e:  ret
