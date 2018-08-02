
test3.fsx(39,18,39,28): typecheck error FS3237: Cannot call the extension member as it requires a value to be mutable or a byref due to the extending type being used as a byref.

test3.fsx(40,9,40,11): typecheck error FS0001: Type mismatch. Expecting a
    'byref<DateTime>'    
but given a
    'inref<DateTime>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

test3.fsx(44,9,44,20): typecheck error FS3237: Cannot call the extension member as it requires a value to be mutable or a byref due to the extending type being used as a byref.
