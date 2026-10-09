// This is a generated file; the original input is 'Messages.txt'
namespace Messages

open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
// (namespaces below for specific case of using the tool to compile FSharp.Core itself)
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators

#nowarn "1182" // Generated boilerplate may include helper functions not referenced when the resource file has no entries
#nowarn "3262" // The call to Option.ofObj below is applied in multiple compilation modes for GetString, sometimes the value is typed as a non-nullable string

type internal SR =
    private new: unit -> SR
    // BEGIN BOILERPLATE

    static member GetTextOpt: key:string -> string option

    /// If set to true, then all error messages will just return the filled 'holes' delimited by ',,,'s - this is for language-neutral testing (e.g. localization-invariant baselines).
    static member SwallowResourceText: bool with get, set
    // END BOILERPLATE
    /// No arguments
    /// (Originally from Messages.txt:1)
    static member zero: unit -> string
    /// Integer %d
    /// (Originally from Messages.txt:2)
    static member integer: a0: System.Int32 -> string
    /// Floating %f
    /// (Originally from Messages.txt:3)
    static member floating: a0: System.Double -> string
    /// Text %s
    /// (Originally from Messages.txt:4)
    static member text: a0: System.String -> string
    /// Hex %x
    /// (Originally from Messages.txt:5)
    static member hex: a0: System.UInt32 -> string
    /// Mixed %d / %f / %s
    /// (Originally from Messages.txt:6)
    static member mixed: a0: System.Int32 * a1: System.Double * a2: System.String -> string
    /// Numbered without arguments
    /// (Originally from Messages.txt:7)
    static member numberedZero: unit -> int * string
    /// Numbered %d / %f / %s
    /// (Originally from Messages.txt:8)
    static member numberedMixed: a0: System.Int32 * a1: System.Double * a2: System.String -> int * string
    /// Escaped {literal}, 100%% and %s\n\t\"quoted\"
    /// (Originally from Messages.txt:9)
    static member escaped: a0: System.String -> string
    static member RunStartupValidation: unit -> unit
