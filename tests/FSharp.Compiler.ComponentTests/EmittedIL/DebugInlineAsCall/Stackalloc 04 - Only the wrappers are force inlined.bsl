open System
open FSharp.NativeInterop
#nowarn 9

let inline stackalloc n = Span<char>(NativePtr.stackalloc<char> n |> NativePtr.toVoidPtr, n)
let inline fill (b: Span<char>) c = b.Fill c

[<EntryPoint>]
let main _ =
    let b = stackalloc 2
    fill b 'a'
    if String b = "aa" then 0 else 1
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

Test::fill
  (7,37-7,45)  b.Fill c
    IL_0000:  ldarga.s 0
    IL_0002:  ldarg.1
    IL_0003:  call Fill
    IL_0008:  ret

Test::main
  (11,5-11,25)  let b = stackalloc 2
    IL_0000:  ldc.i4.2
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

  (12,5-12,15)  fill b 'a'
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s 97
    IL_0018:  call Test::fill
    IL_001d:  nop

  (13,5-13,28)  if String b = "aa" then
    IL_001e:  ldloc.0
    IL_001f:  call op_Implicit
    IL_0024:  newobj String::.ctor
    IL_0029:  ldstr "aa"
    IL_002e:  call String::Equals
    IL_0033:  brfalse.s IL_0037

  (13,29-13,30)  0
    IL_0035:  ldc.i4.0
    IL_0036:  ret

  (13,36-13,37)  1
    IL_0037:  ldc.i4.1
    IL_0038:  ret
