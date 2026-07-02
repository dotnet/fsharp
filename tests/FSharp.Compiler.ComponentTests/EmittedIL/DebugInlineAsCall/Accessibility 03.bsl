module App
let r = Lib.T().G(1)
--------------------------------------------------------------------------------

App::.cctor
  <hidden>
    IL_0000:  ldc.i4.0
    IL_0001:  stsfld $App::init@
    IL_0006:  ldsfld $App::init@
    IL_000b:  pop
    IL_000c:  ret

App::staticInitialization@
  (3,1-3,21)  let r = Lib.T().G(1)
    IL_0000:  newobj T::.ctor
    IL_0005:  ldc.i4.1
    IL_0006:  callvirt T::G
    IL_000b:  stsfld App::r@3
    IL_0010:  ret
