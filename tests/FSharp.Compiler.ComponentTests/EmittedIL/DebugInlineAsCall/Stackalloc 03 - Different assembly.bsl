open System
open MyLib

[<EntryPoint>]
let main _ =
    let b = stackalloc 2
    b[0] <- 'a'
    b[1] <- 'b'
    if String b = "ab" then 0 else 1
--------------------------------------------------------------------------------

Test::main
  (7,5-7,25)  let b = stackalloc 2
    IL_0000:  ldc.i4.2
    IL_0001:  stloc.1
    IL_0002:  ldloc.1
    IL_0003:  stloc.2
    IL_0004:  ldloc.2
    IL_0005:  stloc.3
    IL_0006:  ldloc.3
    IL_0007:  sizeof Char
    IL_000d:  mul
    IL_000e:  localloc
    IL_0010:  ldloc.1
    IL_0011:  newobj .ctor
    IL_0016:  stloc.0

  (8,5-8,9)  b[0]
    IL_0017:  ldloca.s 0
    IL_0019:  ldc.i4.0
    IL_001a:  call get_Item
    IL_001f:  stloc.s 4
    IL_0021:  ldloc.s 4
    IL_0023:  ldc.i4.s 97
    IL_0025:  stobj Char

  (9,5-9,9)  b[1]
    IL_002a:  ldloca.s 0
    IL_002c:  ldc.i4.1
    IL_002d:  call get_Item
    IL_0032:  stloc.s 5
    IL_0034:  ldloc.s 5
    IL_0036:  ldc.i4.s 98
    IL_0038:  stobj Char

  (10,5-10,28)  if String b = "ab" then
    IL_003d:  ldloc.0
    IL_003e:  call op_Implicit
    IL_0043:  newobj String::.ctor
    IL_0048:  ldstr "ab"
    IL_004d:  call String::Equals
    IL_0052:  brfalse.s IL_0056

  (10,29-10,30)  0
    IL_0054:  ldc.i4.0
    IL_0055:  ret

  (10,36-10,37)  1
    IL_0056:  ldc.i4.1
    IL_0057:  ret
