open System
open FSharp.NativeInterop
#nowarn 9

let inline stackalloc n = Span<char>(NativePtr.stackalloc<char> n |> NativePtr.toVoidPtr, n)

[<EntryPoint>]
let main _ =
    let b = stackalloc 3
    b[0] <- 'a'
    b[1] <- 'b'
    b[2] <- 'c'
    if String b = "abc" then 0 else 1
--------------------------------------------------------------------------------

Test::stackalloc
  (6,27-6,93)  Span<char>(NativePtr.stackalloc<char> n |> NativePtr.toVoidPtr, n)
    IL_0000:  nop

  (6,38-6,66)  NativePtr.stackalloc<char> n
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  sizeof Char
    IL_000a:  mul
    IL_000b:  localloc
    IL_000d:  stloc.0

  (6,70-6,89)  NativePtr.toVoidPtr
    IL_000e:  ldloc.0
    IL_000f:  stloc.2
    IL_0010:  ldloc.2
    IL_0011:  ldarg.0
    IL_0012:  newobj .ctor
    IL_0017:  ret

Test::main
  (10,5-10,25)  let b = stackalloc 3
    IL_0000:  ldc.i4.3
    IL_0001:  stloc.1
    IL_0002:  ldloc.1
    IL_0003:  stloc.2
    IL_0004:  ldloc.2
    IL_0005:  sizeof Char
    IL_000b:  mul
    IL_000c:  localloc
    IL_000e:  ldloc.1
    IL_000f:  newobj .ctor
    IL_0014:  stloc.0

  (11,5-11,9)  b[0]
    IL_0015:  ldloca.s 0
    IL_0017:  ldc.i4.0
    IL_0018:  call get_Item
    IL_001d:  stloc.3
    IL_001e:  ldloc.3
    IL_001f:  ldc.i4.s 97
    IL_0021:  stobj Char

  (12,5-12,9)  b[1]
    IL_0026:  ldloca.s 0
    IL_0028:  ldc.i4.1
    IL_0029:  call get_Item
    IL_002e:  stloc.s 4
    IL_0030:  ldloc.s 4
    IL_0032:  ldc.i4.s 98
    IL_0034:  stobj Char

  (13,5-13,9)  b[2]
    IL_0039:  ldloca.s 0
    IL_003b:  ldc.i4.2
    IL_003c:  call get_Item
    IL_0041:  stloc.s 5
    IL_0043:  ldloc.s 5
    IL_0045:  ldc.i4.s 99
    IL_0047:  stobj Char

  (14,5-14,29)  if String b = "abc" then
    IL_004c:  ldloc.0
    IL_004d:  call op_Implicit
    IL_0052:  newobj String::.ctor
    IL_0057:  ldstr "abc"
    IL_005c:  call String::Equals
    IL_0061:  brfalse.s IL_0065

  (14,30-14,31)  0
    IL_0063:  ldc.i4.0
    IL_0064:  ret

  (14,37-14,38)  1
    IL_0065:  ldc.i4.1
    IL_0066:  ret
