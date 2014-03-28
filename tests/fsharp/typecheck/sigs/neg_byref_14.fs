module Test
#nowarn "1175"
exception ByrefException of byref<int> (* trap: exception with byref type *)
