module Module

let f (x: int) =
    x + x

let g () =
    match f 5 with
    | 10 -> 0
    | _ -> 1
--------------------------------------------------------------------------------

Module::f
  (4,5-4,10)  x + x
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  add
    IL_0003:  ret

Module::g
  (7,5-7,19)  match f 5 with
    IL_0000:  ldc.i4.5
    IL_0001:  call Module::f
    IL_0006:  ldc.i4.s 10
    IL_0008:  sub
    IL_0009:  switch (1 targets)
    IL_0012:  br.s IL_0016

  (8,13-8,14)  0
    IL_0014:  ldc.i4.0
    IL_0015:  ret

  (9,12-9,13)  1
    IL_0016:  ldc.i4.1
    IL_0017:  ret
