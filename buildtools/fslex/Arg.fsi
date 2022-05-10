// (c) Microsoft Corporation 2005-2009. 

/// A simple command-line argument processor.
#if INTERNALIZED_FSLEXYACC_RUNTIME
namespace Internal.Utilities
#else
namespace Microsoft.FSharp.Text
#endif

/// The spec value describes the action of the argument,
/// and whether it expects a following parameter.
[<Sealed>]
type ArgType = 
    static member Clear  : bool ref         -> ArgType
    static member Float  : (float -> unit)  -> ArgType
    static member Int    : (int -> unit)    -> ArgType
    static member Rest   : (string -> unit) -> ArgType
    static member Set    : bool ref         -> ArgType
    static member String : (string -> unit) -> ArgType
    static member Unit   : (unit -> unit)   -> ArgType

type ArgInfo = 
  new : name:string * action:ArgType * help:string -> ArgInfo
  /// Return the name of the argument
  member Name : string
  /// Return the argument type and action of the argument
  member ArgType : ArgType
  /// Return the usage help associated with the argument
  member HelpText : string

[<Sealed>]
type ArgParser = 
    #if FX_NO_COMMAND_LINE_ARGS
    #else

    /// Parse some of the arguments given by 'argv', starting at the given position
    [<System.Obsolete("This method should not be used directly as it will be removed in a future revision of this library")>]
    static member ParsePartial: cursor: int ref * argv: string[] * arguments:seq<ArgInfo> * ?otherArgs: (string -> unit) * ?usageText:string -> unit

    /// Parse the arguments given by System.Environment.GetEnvironmentVariables()
    /// according to the argument processing specifications "specs".
    /// Args begin with "-". Non-arguments are passed to "f" in
    /// order.  "use" is printed as part of the usage line if an error occurs.

    static member Parse: arguments:seq<ArgInfo> * ?otherArgs: (string -> unit) * ?usageText:string -> unit
    #endif

    /// Prints the help for each argument.
    static member Usage : arguments:seq<ArgInfo> * ?usage:string -> unit

