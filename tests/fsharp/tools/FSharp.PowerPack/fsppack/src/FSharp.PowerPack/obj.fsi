//==========================================================================
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.  The type signatures in this interface
// are an edited version of those generated automatically by running 
// "bin\fsc.exe -i" on the implementation file.
//===========================================================================

[<CompilerMessage("This module is for ML compatibility. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
module Microsoft.FSharp.Compatibility.OCaml.Obj

[<CompilerMessage("This construct is for ML compatibility. Consider using 'obj' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
type t = obj

[<CompilerMessage("This construct is for ML compatibility. Consider using 'box' and/or 'unbox' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val magic: 'T -> 'U

[<CompilerMessage("This construct is for ML compatibility. Consider using 'null' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val nullobj: obj

/// See Microsoft.FSharp.Core.Operators.unbox
[<CompilerMessage("This construct is for ML compatibility. Consider using 'unbox' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val obj: obj -> 'T

/// See Microsoft.FSharp.Core.Operators.box
[<CompilerMessage("This construct is for ML compatibility. Consider using 'box' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val repr: 'T -> obj

/// See Microsoft.FSharp.Core.LanguagePrimitives.PhysicalEquality
[<CompilerMessage("This construct is for ML compatibility. Consider using 'Microsoft.FSharp.Core.LanguagePrimitives.PhysicalEquality' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
val eq: 'T -> 'T -> bool when 'T : not struct

[<CompilerMessage("This construct is for ML compatibility. Consider using 'not(Microsoft.FSharp.Core.LanguagePrimitives.PhysicalEquality(...))' instead. This message can be disabled using '--nowarn:62' or '#nowarn \"62\"'.", 62, IsHidden=true)>]
/// Negation of Obj.eq (i.e. reference/physical inequality)
val not_eq: 'T -> 'T -> bool when 'T : not struct

