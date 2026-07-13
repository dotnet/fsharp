module Module
type Bind =
    static member inline Invoke (s: 'M) (b: 'T -> 'U) =
        ((^M or ^U) : (static member (>>=) : _*_ -> _) s, b)

type Return =
    static member Return (_: 'T list, _: Return) : 'T -> 'T list = Unchecked.defaultof<_>
    static member inline Invoke (x: 'T) =
        (^A : (static member Return : 'T -> ^A) x)

type MonadBuilder () =
    member inline _.Return x = Return.Invoke x
    member inline _.Bind (p, r) = Bind.Invoke p r
let monad' = MonadBuilder ()
--------------------------------------------------------------------------------

Program::wrap
  (6,5-6,30)  
    IL_0000:  ldc.i4.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0

  (7,5-7,11)  Retur
    IL_0007:  call Module::get_monad'
    IL_000c:  stloc.1

  (8,9-8,25)  ic member Return
    IL_000d:  ldloc.0
    IL_000e:  call get_contents
    IL_0013:  brfalse.s IL_0026

  (9,19-9,31)  inline Invok
    IL_0015:  ldsfld @_instance
    IL_001a:  stloc.2
    IL_001b:  ldloc.2
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.0
    IL_001e:  tail.
    IL_0020:  call InvokeFast
    IL_0025:  ret

  (11,13-11,28)  
    IL_0026:  ldloc.1
    IL_0027:  ldarg.0
    IL_0028:  ldloc.1
    IL_0029:  ldloc.0
    IL_002a:  newobj .ctor
    IL_002f:  callvirt MonadBuilder::Bind
    IL_0034:  ret

Program::wrap$W
  (6,5-6,30)  
    IL_0000:  ldc.i4.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0

  (7,5-7,11)  Retur
    IL_0007:  call Module::get_monad'
    IL_000c:  stloc.1

  (8,9-8,25)  ic member Return
    IL_000d:  ldloc.0
    IL_000e:  call get_contents
    IL_0013:  brfalse.s IL_0027

  (9,19-9,31)  inline Invok
    IL_0015:  ldarg.1
    IL_0016:  newobj .ctor
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.0
    IL_001f:  tail.
    IL_0021:  call InvokeFast
    IL_0026:  ret

  (11,13-11,28)  
    IL_0027:  ldarg.2
    IL_0028:  ldloc.1
    IL_0029:  ldarg.3
    IL_002a:  ldarg.0
    IL_002b:  ldarg.1
    IL_002c:  ldarg.2
    IL_002d:  ldloc.0
    IL_002e:  ldloc.1
    IL_002f:  newobj .ctor
    IL_0034:  callvirt MonadBuilder::Bind$W
    IL_0039:  ret

wrap@9-1::Invoke
  (10,9-10,51)  (^A : (static member Return : 'T -> ^A) x)
    IL_0000:  ldstr "Dynamic invocation of Return is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

wrap@9-4::Invoke
  (10,9-10,51)  (^A : (static member Return : 'T -> ^A) x)
    IL_0000:  ldstr "Dynamic invocation of Return is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

wrap@9-6::Invoke
  (10,9-10,51)  (^A : (static member Return : 'T -> ^A) x)
    IL_0000:  ldarg.0
    IL_0001:  ldfld return
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  callvirt Invoke
    IL_000e:  ret

wrap@9-9::Invoke
  (10,9-10,51)  (^A : (static member Return : 'T -> ^A) x)
    IL_0000:  ldarg.0
    IL_0001:  ldfld return
    IL_0006:  ldarg.1
    IL_0007:  tail.
    IL_0009:  callvirt Invoke
    IL_000e:  ret

wrap@13::Invoke
  (13,32-13,47)  Return.Invoke x
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.2
    IL_0008:  tail.
    IL_000a:  callvirt Invoke
    IL_000f:  ret

wrap@13-3::Invoke
  (13,32-13,47)  Return.Invoke x
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.2
    IL_0008:  tail.
    IL_000a:  callvirt Invoke
    IL_000f:  ret

wrap@13-5::Invoke
  (13,32-13,47)  Return.Invoke x
    IL_0000:  ldarg.0
    IL_0001:  ldfld return
    IL_0006:  newobj .ctor
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldarg.2
    IL_000e:  tail.
    IL_0010:  callvirt Invoke
    IL_0015:  ret

wrap@13-8::Invoke
  (13,32-13,47)  Return.Invoke x
    IL_0000:  ldarg.0
    IL_0001:  ldfld return
    IL_0006:  ldarg.0
    IL_0007:  ldfld return0
    IL_000c:  ldarg.0
    IL_000d:  ldfld op_GreaterGreaterEquals
    IL_0012:  newobj .ctor
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  ldarg.2
    IL_001a:  tail.
    IL_001c:  callvirt Invoke
    IL_0021:  ret

wrap@12-2::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0

  (12,13-12,26)  ilder () =
    IL_0002:  ldarg.0
    IL_0003:  ldfld state
    IL_0008:  ldc.i4.1
    IL_0009:  call set_contents

  (13,13-13,24)  nline _.Ret
    IL_000e:  ldsfld @_instance
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  ldarg.0
    IL_0016:  ldfld builder@
    IL_001b:  ldc.i4.1
    IL_001c:  tail.
    IL_001e:  call InvokeFast
    IL_0023:  ret

wrap@12-7::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  stloc.0

  (12,13-12,26)  ilder () =
    IL_0002:  ldarg.0
    IL_0003:  ldfld state
    IL_0008:  ldc.i4.1
    IL_0009:  call set_contents

  (13,13-13,24)  nline _.Ret
    IL_000e:  ldarg.0
    IL_000f:  ldfld return
    IL_0014:  ldarg.0
    IL_0015:  ldfld return0
    IL_001a:  ldarg.0
    IL_001b:  ldfld op_GreaterGreaterEquals
    IL_0020:  newobj .ctor
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  ldarg.0
    IL_0028:  ldfld builder@
    IL_002d:  ldc.i4.1
    IL_002e:  tail.
    IL_0030:  call InvokeFast
    IL_0035:  ret

Module::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $Module::init@
    IL_0006:  ldsfld $Module::init@
    IL_000b:  pop
    IL_000c:  ret

Module::staticInitialization@
  (15,1-15,29)  let monad' = MonadBuilder ()
    IL_0000:  newobj MonadBuilder::.ctor
    IL_0005:  stsfld Module::monad'@15
    IL_000a:  ret

Bind::Invoke
  (5,9-5,61)  ((^M or ^U) : (static member (>>=) : _*_ -> _) s, b)
    IL_0000:  ldstr "Dynamic invocation of op_GreaterGreaterEquals is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Bind::Invoke$W
  (5,9-5,61)  ((^M or ^U) : (static member (>>=) : _*_ -> _) s, b)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  tail.
    IL_0005:  call InvokeFast
    IL_000a:  ret

Return::Return
  (8,68-8,87)  Unchecked.defaultof
    IL_0000:  ldnull
    IL_0001:  ret

Return::Invoke
  (10,9-10,51)  (^A : (static member Return : 'T -> ^A) x)
    IL_0000:  ldstr "Dynamic invocation of Return is not supported"
    IL_0005:  newobj NotSupportedException::.ctor
    IL_000a:  throw

Return::Invoke$W
  (10,9-10,51)  (^A : (static member Return : 'T -> ^A) x)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  tail.
    IL_0004:  callvirt Invoke
    IL_0009:  ret

MonadBuilder::.ctor
  (12,6-12,18)  MonadBuilder
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

MonadBuilder::Return
  (13,32-13,47)  Return.Invoke x
    IL_0000:  ldarg.1
    IL_0001:  call Return::Invoke
    IL_0006:  ret

MonadBuilder::Return$W
  (13,32-13,47)  Return.Invoke x
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  call Return::Invoke$W
    IL_0007:  ret

MonadBuilder::Bind
  (14,35-14,50)  Bind.Invoke p r
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  call Bind::Invoke
    IL_0007:  ret

MonadBuilder::Bind$W
  (14,35-14,50)  Bind.Invoke p r
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.3
    IL_0003:  call Bind::Invoke$W
    IL_0008:  ret
