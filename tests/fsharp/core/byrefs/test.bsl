
test.fsx(31,34,31,40): typecheck error FS3224: The byref pointer is readonly, so this write is not permitted.

test.fsx(34,32,34,40): typecheck error FS0257: Invalid mutation of a constant expression. Consider copying the expression to a mutable local, e.g. 'let mutable x = ...'.

test.fsx(38,36,38,38): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

test.fsx(42,36,42,38): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

test.fsx(47,38,47,40): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

test.fsx(52,38,52,40): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

test.fsx(57,38,57,40): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

test.fsx(62,38,62,40): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

test.fsx(66,34,66,47): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

test.fsx(66,34,66,47): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly
