
neg106.fs(8,59,8,61): typecheck error FS3230: A value defined in a module must be mutable in order to take its address, e.g. 'let mutable x = ...'

neg106.fs(13,18,13,72): typecheck error FS0041: No overloads match for method 'CompareExchange'.

Known types of arguments: inref<int> * int * int

Available overloads:
 - System.Threading.Interlocked.CompareExchange(location1: byref<float32>, value: float32, comparand: float32) : float32 // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<float>, value: float, comparand: float) : float // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<int64>, value: int64, comparand: int64) : int64 // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<int>, value: int, comparand: int) : int // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<nativeint>, value: nativeint, comparand: nativeint) : nativeint // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<obj>, value: obj, comparand: obj) : obj // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange<'T when 'T: not struct>(location1: byref<'T>, value: 'T, comparand: 'T) : 'T // Argument 'location1' doesn't match

neg106.fs(17,59,17,61): typecheck error FS3236: Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.

neg106.fs(17,14,17,68): typecheck error FS0041: No overloads match for method 'CompareExchange'.

Known types of arguments: inref<int> * int * int

Available overloads:
 - System.Threading.Interlocked.CompareExchange(location1: byref<float32>, value: float32, comparand: float32) : float32 // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<float>, value: float, comparand: float) : float // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<int64>, value: int64, comparand: int64) : int64 // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<int>, value: int, comparand: int) : int // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<nativeint>, value: nativeint, comparand: nativeint) : nativeint // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange(location1: byref<obj>, value: obj, comparand: obj) : obj // Argument 'location1' doesn't match
 - System.Threading.Interlocked.CompareExchange<'T when 'T: not struct>(location1: byref<'T>, value: 'T, comparand: 'T) : 'T // Argument 'location1' doesn't match

neg106.fs(23,35,23,39): typecheck error FS0001: Type mismatch. Expecting a
    'byref<int>'    
but given a
    'inref<int>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(31,22,31,26): typecheck error FS0001: Type mismatch. Expecting a
    'byref<int>'    
but given a
    'inref<int>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(40,18,40,32): typecheck error FS0041: No overloads match for method 'M'.

Known types of arguments: string * inref<int>

Available overloads:
 - static member C.M: a: int * x: byref<int> -> unit // Argument 'a' doesn't match
 - static member C.M: a: string * x: byref<int> -> unit // Argument 'x' doesn't match

neg106.fs(41,19,41,31): typecheck error FS0041: No overloads match for method 'M'.

Known types of arguments: int * inref<int>

Available overloads:
 - static member C.M: a: int * x: byref<int> -> unit // Argument 'x' doesn't match
 - static member C.M: a: string * x: byref<int> -> unit // Argument 'a' doesn't match

neg106.fs(49,22,49,26): typecheck error FS0001: Type mismatch. Expecting a
    'byref<int>'    
but given a
    'inref<int>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(60,9,60,25): typecheck error FS3224: The byref pointer is readonly, so this write is not permitted.

neg106.fs(65,42,65,48): typecheck error FS3224: The byref pointer is readonly, so this write is not permitted.

neg106.fs(74,9,74,17): typecheck error FS0257: Invalid mutation of a constant expression. Consider copying the expression to a mutable local, e.g. 'let mutable x = ...'.

neg106.fs(83,34,83,40): typecheck error FS3224: The byref pointer is readonly, so this write is not permitted.

neg106.fs(86,32,86,40): typecheck error FS0257: Invalid mutation of a constant expression. Consider copying the expression to a mutable local, e.g. 'let mutable x = ...'.

neg106.fs(90,36,90,38): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(94,35,94,39): typecheck error FS0001: Type mismatch. Expecting a
    'byref<int>'    
but given a
    'inref<int>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(98,36,98,38): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

neg106.fs(102,37,102,40): typecheck error FS0072: Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.

neg106.fs(102,36,102,40): typecheck error FS3236: Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.

neg106.fs(102,36,102,40): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'a>'    
but given a
    'inref<'a>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

neg106.fs(107,38,107,40): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(112,39,112,42): typecheck error FS0072: Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.

neg106.fs(112,38,112,42): typecheck error FS3236: Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.

neg106.fs(112,38,112,42): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'a>'    
but given a
    'inref<'a>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(117,38,117,40): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

neg106.fs(122,39,122,42): typecheck error FS0072: Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.

neg106.fs(122,38,122,42): typecheck error FS3236: Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.

neg106.fs(122,38,122,42): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'a>'    
but given a
    'inref<'a>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

neg106.fs(127,38,127,40): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(132,39,132,42): typecheck error FS0072: Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.

neg106.fs(132,38,132,42): typecheck error FS3236: Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.

neg106.fs(132,38,132,42): typecheck error FS0001: Type mismatch. Expecting a
    'byref<'a>'    
but given a
    'inref<'a>'    
The type 'ByRefKinds.InOut' does not match the type 'ByRefKinds.In'

neg106.fs(137,38,137,40): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'T>'    
but given a
    'inref<'T>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

neg106.fs(142,39,142,42): typecheck error FS0072: Lookup on object of indeterminate type based on information prior to this program point. A type annotation may be needed prior to this program point to constrain the type of the object. This may allow the lookup to be resolved.

neg106.fs(142,38,142,42): typecheck error FS3236: Cannot take the address of the value returned from the expression. Assign the returned value to a let-bound value before taking the address.

neg106.fs(142,38,142,42): typecheck error FS0001: Type mismatch. Expecting a
    'outref<'a>'    
but given a
    'inref<'a>'    
The type 'ByRefKinds.Out' does not match the type 'ByRefKinds.In'

neg106.fs(146,34,146,47): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(147,34,147,44): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(148,34,148,44): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(149,34,149,44): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(146,34,146,47): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(147,34,147,44): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(148,34,148,44): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly

neg106.fs(149,34,149,44): typecheck error FS1204: This construct is for use in the FSharp.Core library and should not be used directly
