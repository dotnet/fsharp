open System
open FSharp.NativeInterop
#nowarn 9

let inline alloc n : nativeptr<char> = NativePtr.stackalloc<char> n
let inline stackalloc n = Span<char>(alloc n |> NativePtr.toVoidPtr, n)

[<EntryPoint>]
let main _ =
    let b = stackalloc 2
    b[0] <- 'a'
    b[1] <- 'b'
    if String b = "ab" then 0 else 1
--------------------------------------------------------------------------------

Test::alloc
  (6,40-6,68)  NativePtr.stackalloc<char> n
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  sizeof Char
    IL_0009:  mul
    IL_000a:  localloc
    IL_000c:  ret

Test::stackalloc
  (7,27-7,72)  Span<char>(alloc n |> NativePtr.toVoidPtr, n)
    IL_0000:  nop

  (7,38-7,45)  alloc n
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  stloc.2
    IL_0005:  ldloc.2
    IL_0006:  sizeof Char
    IL_000c:  mul
    IL_000d:  localloc
    IL_000f:  stloc.0

  (7,49-7,68)  NativePtr.toVoidPtr
    IL_0010:  ldloc.0
    IL_0011:  stloc.3
    IL_0012:  ldloc.3
    IL_0013:  ldarg.0
    IL_0014:  newobj .ctor
    IL_0019:  ret

Test::main
  (11,5-11,25)  let b = stackalloc 2
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

  (12,5-12,9)  b[0]
    IL_0017:  ldloca.s 0
    IL_0019:  ldc.i4.0
    IL_001a:  call get_Item
    IL_001f:  stloc.s 4
    IL_0021:  ldloc.s 4
    IL_0023:  ldc.i4.s 97
    IL_0025:  stobj Char

  (13,5-13,9)  b[1]
    IL_002a:  ldloca.s 0
    IL_002c:  ldc.i4.1
    IL_002d:  call get_Item
    IL_0032:  stloc.s 5
    IL_0034:  ldloc.s 5
    IL_0036:  ldc.i4.s 98
    IL_0038:  stobj Char

  (14,5-14,28)  if String b = "ab" then
    IL_003d:  ldloc.0
    IL_003e:  call op_Implicit
    IL_0043:  newobj String::.ctor
    IL_0048:  ldstr "ab"
    IL_004d:  call String::Equals
    IL_0052:  brfalse.s IL_0056

  (14,29-14,30)  0
    IL_0054:  ldc.i4.0
    IL_0055:  ret

  (14,36-14,37)  1
    IL_0056:  ldc.i4.1
    IL_0057:  ret
