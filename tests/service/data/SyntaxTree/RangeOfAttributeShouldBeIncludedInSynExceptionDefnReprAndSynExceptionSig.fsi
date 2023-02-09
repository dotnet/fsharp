
module internal FSharp.Compiler.ParseHelpers

// The error raised by the parse_error_rich function, which is called by the parser engine
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: range
