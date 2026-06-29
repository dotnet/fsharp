module Module

type C<'a, 'b when 'a: (member M: unit -> 'b)> = 'a

type C = static member inline F<'a, 'b, 'c when C<'a, 'b>>(_a: 'a) = ()
--------------------------------------------------------------------------------

Program::foo
  (8,26-8,32)  
    IL_0000:  ldsfld @_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  callvirt Invoke
    IL_000d:  pop
    IL_000e:  ldsfld @_instance
    IL_0013:  ret

Program::foo$W
  (8,26-8,32)  
    IL_0000:  ldarg.0
    IL_0001:  newobj .ctor
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldarg.1
    IL_0009:  callvirt Invoke
    IL_000e:  pop
    IL_000f:  ldarg.0
    IL_0010:  newobj .ctor
    IL_0015:  ret

Program::main
  (12,13-12,22)  
    IL_0000:  newobj T::.ctor
    IL_0005:  call Program::<foo>__debug@12
    IL_000a:  pop

  (13,5-13,6)  
    IL_000b:  ldc.i4.0
    IL_000c:  ret

Program::<foo>__debug@12
  (8,26-8,32)  
    IL_0000:  ldsfld main@6::@_instance
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  callvirt Invoke
    IL_000d:  pop
    IL_000e:  ldsfld main@8-1::@_instance
    IL_0013:  ret

foo@6::Invoke
  (6,70-6,72)  ()
    IL_0000:  ldnull
    IL_0001:  ret

foo@6-2::Invoke
  (6,70-6,72)  ()
    IL_0000:  ldnull
    IL_0001:  ret

main@6::Invoke
  (6,70-6,72)  ()
    IL_0000:  ldnull
    IL_0001:  ret

T::.ctor
  (6,6-6,7)  C
    IL_0000:  ldarg.0
    IL_0001:  callvirt Object::.ctor
    IL_0006:  ldarg.0
    IL_0007:  pop
    IL_0008:  ret

T::M
  (6,27-6,29)  in
    IL_0000:  ldc.i4.s 42
    IL_0002:  ret

foo@8-1::Invoke
  (8,44-8,46)  
    IL_0000:  ldnull
    IL_0001:  ret

foo@8-3::Invoke
  (8,44-8,46)  
    IL_0000:  ldnull
    IL_0001:  ret

main@8-1::Invoke
  (8,44-8,46)  
    IL_0000:  ldnull
    IL_0001:  ret

C::F
  (6,70-6,72)  ()
    IL_0000:  ret

C::F$W
  (6,70-6,72)  ()
    IL_0000:  ret
