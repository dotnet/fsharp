module Module

let inline internal fInternal () = ()
let inline f () = fInternal ()
--------------------------------------------------------------------------------

Module::fInternal
  (4,36-4,38)  ()
    IL_0000:  ret

Module::f
  (5,19-5,31)  fInternal ()
    IL_0000:  tail.
    IL_0002:  call Module::<fInternal>__debug@5
    IL_0007:  ret

Module::<fInternal>__debug@5
  (4,36-4,38)  ()
    IL_0000:  ret
