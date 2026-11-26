type A = static member Name = "A"
type B = static member Name = "B"

let inline name<'T when 'T: (static member Name: string)> () = 'T.Name

[<EntryPoint>]
let main _ =
    if name<A> () = "A" && name<B> () = "B" then 0 else 1
--------------------------------------------------------------------------------

Test::name
  (5,64-5,71)  'T.Name
    IL_0000:  ldstr "Dynamic invocation of get_Name is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Test::name$W
  (5,64-5,71)  'T.Name
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  tail.
    IL_0004:  callvirt Invoke
    IL_0009:  ret

Test::main
  (9,5-9,49)  if name<A> () = "A" && name<B> () = "B" then
    IL_0000:  nop

  (9,8-9,24)  name<A> () = "A"
    IL_0001:  call Test::<name>__debug@9
    IL_0006:  ldstr "A"
    IL_000b:  call String::Equals
    IL_0010:  brfalse.s IL_0024

  (9,28-9,44)  name<B> () = "B"
    IL_0012:  call Test::<name>__debug@9-1
    IL_0017:  ldstr "B"
    IL_001c:  call String::Equals

  <hidden>
    IL_0021:  nop
    IL_0022:  br.s IL_0026

  <hidden>
    IL_0024:  ldc.i4.0

  <hidden>
    IL_0025:  nop
    IL_0026:  brfalse.s IL_002a

  (9,50-9,51)  0
    IL_0028:  ldc.i4.0
    IL_0029:  ret

  (9,57-9,58)  1
    IL_002a:  ldc.i4.1
    IL_002b:  ret

Test::<name>__debug@9
  (5,64-5,71)  'T.Name
    IL_0000:  call A::get_Name
    IL_0005:  ret

Test::<name>__debug@9-1
  (5,64-5,71)  'T.Name
    IL_0000:  call B::get_Name
    IL_0005:  ret

A::get_Name
  (2,31-2,34)  "A"
    IL_0000:  ldstr "A"
    IL_0005:  ret

B::get_Name
  (3,31-3,34)  "B"
    IL_0000:  ldstr "B"
    IL_0005:  ret
