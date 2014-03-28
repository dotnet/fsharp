module Test
#nowarn "1175"
type M4 = Nested of byref<int>         (* trap: byref union constr fields *)
