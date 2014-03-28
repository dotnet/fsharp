module Test
#nowarn "1175"
type M3 = { x:byref<int> }             (* trap: byref record fields *)
