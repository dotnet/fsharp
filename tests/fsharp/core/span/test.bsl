
test.fsx(303,46,303,86): typecheck error FS3235: The function or method call cannot be used at this point, because one argument that is a byref of a non-stack-local Span or IsByRefLike type is used with another argument that is a stack-local Span or IsByRefLike type. This is to ensure the address of the local value does not escape its scope.

test.fsx(306,13,306,86): typecheck error FS3230: This value can't be assigned because the target 'param1' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(309,13,309,62): typecheck error FS3230: This value can't be assigned because the target 'param1' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(348,13,348,57): typecheck error FS3230: This value can't be assigned because the target 'addr' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(351,13,351,38): typecheck error FS3230: This value can't be assigned because the target 'param1' may refer to non-stack-local memory, while the expression being assigned is assessed to potentially refer to stack-local memory. This is to help prevent pointers to stack-bound memory escaping their scope.

test.fsx(354,14,354,29): typecheck error FS3209: The address of the variable 'stackReferring3' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test.fsx(376,18,376,19): typecheck error FS3209: The address of the variable 'x' or a related expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.

test.fsx(380,18,380,33): typecheck error FS3228: The address of a value returned from the expression cannot be used at this point. This is to ensure the address of the local value does not escape its scope.
