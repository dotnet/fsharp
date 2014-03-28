module Microsoft.FSharp.Compatibility.Lazy

open System

[<CompilerMessage("This construct is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type 'T t = Microsoft.FSharp.Control.Lazy<'T>

/// See Lazy.Force
val force: Microsoft.FSharp.Control.Lazy<'T> -> 'T

/// See Lazy.Force.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'v.Force()' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val force_val: Microsoft.FSharp.Control.Lazy<'T> -> 'T

/// Build a lazy (delayed) value from the given computation
[<CompilerMessage("This construct is for ML compatibility. Consider using 'lazy' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val lazy_from_fun: (unit -> 'T) -> Microsoft.FSharp.Control.Lazy<'T>

/// Build a lazy (delayed) value from the given pre-computed value.
[<CompilerMessage("This construct is for ML compatibility. Consider using 'Lazy.CreateFromValue' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val lazy_from_val: 'T -> Microsoft.FSharp.Control.Lazy<'T>

/// Check if a lazy (delayed) value has already been computed
[<CompilerMessage("This construct is for ML compatibility. Consider using 'Lazy.IsForced' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val lazy_is_val: Microsoft.FSharp.Control.Lazy<'T> -> bool

/// Build a lazy (delayed) value from the given computation
[<CompilerMessage("This construct is for ML compatibility. Consider using Lazy.Create instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val create : (unit -> 'T) -> Microsoft.FSharp.Control.Lazy<'T>
