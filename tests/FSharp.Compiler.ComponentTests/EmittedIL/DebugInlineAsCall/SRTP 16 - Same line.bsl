module Module

type T() = member _.M() = ()

type U = static member inline F<'a, 'b when 'a: (member M: unit -> unit)>(_a: 'a, _b: 'b) = ()
--------------------------------------------------------------------------------

Program::foo
  (6,26-6,35)  line F<'a
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.0
    IL_0009:  newobj .ctor
    IL_000e:  callvirt Invoke
    IL_0013:  pop
    IL_0014:  ldsfld @_instance
    IL_0019:  ret

Program::foo$W
  (6,26-6,35)  line F<'a
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  ldc.i4.0
    IL_000a:  newobj .ctor
    IL_000f:  callvirt Invoke
    IL_0014:  pop
    IL_0015:  ldarg.0
    IL_0016:  newobj .ctor
    IL_001b:  ret

Program::main
  (10,13-10,22)  
    IL_0000:  newobj T::.ctor
    IL_0005:  call Program::<foo>__debug@10
    IL_000a:  pop

  (11,5-11,6)  
    IL_000b:  ldc.i4.0
    IL_000c:  ret

Program::<foo>__debug@10
  (6,26-6,35)  line F<'a
    IL_0000:  ldsfld main@6::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.0
    IL_0009:  newobj .ctor
    IL_000e:  callvirt Invoke
    IL_0013:  pop
    IL_0014:  ldsfld main@6-1::@_instance
    IL_0019:  ret

foo@6::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (6,93-6,95)  ()
    IL_000e:  ldnull
    IL_000f:  ret

foo@6-2::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (6,93-6,95)  ()
    IL_000e:  ldnull
    IL_000f:  ret

main@6::Invoke
  <hidden>
    IL_0000:  ldarg.1
    IL_0001:  call get_Item1
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  call get_Item2
    IL_000d:  stloc.1

  (6,93-6,95)  ()
    IL_000e:  ldnull
    IL_000f:  ret

foo@6-1::Invoke
  (6,47-6,49)  :
    IL_0000:  ldnull
    IL_0001:  ret

foo@6-3::Invoke
  (6,47-6,49)  :
    IL_0000:  ldnull
    IL_0001:  ret

main@6-1::Invoke
  (6,47-6,49)  :
    IL_0000:  ldnull
    IL_0001:  ret

T::.ctor
  (4,6-4,7)  T
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::M
  (4,27-4,29)  ()
    IL_0000:  ret

U::F
  (6,93-6,95)  ()
    IL_0000:  ret

U::F$W
  (6,93-6,95)  ()
    IL_0000:  ret
