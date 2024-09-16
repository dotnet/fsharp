
neg63.fs(6,8,6,14): typecheck error FS3155: A quotation may not involve an assignment to or taking the address of a captured local variable

neg63.fs(9,5,9,13): typecheck error FS3301: The function or method has an invalid return type 'Quotations.Expr<byref<int>>'. This is not permitted by the rules of Common IL.

neg63.fs(11,9,11,10): typecheck error FS0421: The address of the variable 'x' cannot be used at this point

neg63.fs(11,9,11,10): typecheck error FS3155: A quotation may not involve an assignment to or taking the address of a captured local variable

neg63.fs(11,5,11,13): typecheck error FS0412: A type instantiation involves a byref type. This is not permitted by the rules of Common IL.

neg63.fs(14,8,14,9): typecheck error FS3155: A quotation may not involve an assignment to or taking the address of a captured local variable

neg63.fs(18,6,18,7): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

neg63.fs(26,6,26,10): typecheck error FS3209: The address of the variable 'addr' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

neg63.fs(30,24,30,25): typecheck error FS3232: Struct members cannot return the address of fields of the struct by reference
