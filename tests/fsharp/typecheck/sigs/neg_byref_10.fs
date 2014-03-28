module Test
type M2 = class
    val x : byref<int>                 (* trap: byref fields *)
end
