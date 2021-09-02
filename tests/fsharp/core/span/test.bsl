
test.fsx(32,22,32,24): typecheck info FS3370: The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.

test.fsx(37,34,37,36): typecheck info FS3370: The use of ':=' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change 'cell := expr' to 'cell.Value <- expr'.

test.fsx(37,37,37,38): typecheck info FS3370: The use of '!' from the F# library is deprecated. See https://aka.ms/fsharp-refcell-ops. For example, please change '!cell' to 'cell.Value'.

test.fsx(302,46,302,86): typecheck error FS3233: The function or method call cannot be used at this point, because one argument that is a byref of a non-stack-local Span or IsByRefLike type is used with another argument that is a stack-local Span or IsByRefLike type. This is to ensure the address of the local value does not escape its scope.

test.fsx(305,13,305,86): typecheck error FS3229: This value can't be assigned because the target 'param1' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(308,13,308,62): typecheck error FS3229: This value can't be assigned because the target 'param1' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(347,13,347,57): typecheck error FS3229: This value can't be assigned because the target 'addr' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(350,13,350,38): typecheck error FS3229: This value can't be assigned because the target 'param1' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(353,14,353,29): typecheck error FS3209: The address of the variable 'stackReferring3' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test.fsx(375,18,375,19): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test.fsx(379,18,379,33): typecheck error FS3228: The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.
