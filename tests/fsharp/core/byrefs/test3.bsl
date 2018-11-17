
test3.fsx(39,18,39,28): typecheck error FS3237: Cannot call the byref extension method 'Test2. The first parameter requires the value to be mutable or a non-readonly byref type.

test3.fsx(40,9,40,11): typecheck error FS0001: Type mismatch. Expecting a
    'byref<DateTime>'    
but given a
    'inref<DateTime>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

test3.fsx(44,9,44,20): typecheck error FS3237: Cannot call the byref extension method 'Change. The first parameter requires the value to be mutable or a non-readonly byref type.

test3.fsx(49,19,49,30): typecheck error FS3237: Cannot call the byref extension method 'Test2. The first parameter requires the value to be mutable or a non-readonly byref type.

test3.fsx(55,9,55,21): typecheck error FS3237: Cannot call the byref extension method 'Change. The first parameter requires the value to be mutable or a non-readonly byref type.

test3.fsx(59,17,59,29): typecheck error FS3239: Cannot partially apply the extension method 'NotChange' because the first parameter is a byref type.

test3.fsx(60,17,60,24): typecheck error FS3239: Cannot partially apply the extension method 'Test' because the first parameter is a byref type.

test3.fsx(61,17,61,26): typecheck error FS3239: Cannot partially apply the extension method 'Change' because the first parameter is a byref type.

test3.fsx(62,17,62,25): typecheck error FS3239: Cannot partially apply the extension method 'Test2' because the first parameter is a byref type.
