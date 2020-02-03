
neg123.fs(19,49,19,51): typecheck error FS0001: No overloads match for method 'pass'. The available overloads are shown below.
neg123.fs(19,49,19,51): typecheck error FS0001: Possible overload: 'static member Switcher.pass : int -> unit'. Type constraint mismatch. The type 
    'unit -> unit'    
is not compatible with type
    'int'    
.
neg123.fs(19,49,19,51): typecheck error FS0001: Possible overload: 'static member Switcher.pass : unit -> unit'. Type constraint mismatch. The type 
    'unit -> unit'    
is not compatible with type
    'unit'    
.
neg123.fs(19,49,19,51): typecheck error FS0001: Possible overload: 'static member Switcher.pass : (int ->  ^r) -> unit when (Switcher or  ^r) : (static member pass :  ^r -> unit)'. Type constraint mismatch. The type 
    'unit -> unit'    
is not compatible with type
    'int -> 'a'    
.
neg123.fs(19,49,19,51): typecheck error FS0001: Possible overload: 'static member Switcher.pass : (string ->  ^r) -> unit when (Switcher or  ^r) : (static member pass :  ^r -> unit)'. Type constraint mismatch. The type 
    'unit -> unit'    
is not compatible with type
    'string -> 'a'    
.
