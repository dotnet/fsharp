// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Interactive

/// <summary>An event loop used by the currently executing F# Interactive session to execute code
/// in the context of a GUI or another event-based system.</summary>
type IEventLoop =

    /// <summary>Run the event loop.</summary>
    ///
    /// <returns>True if the event loop was restarted; false otherwise.</returns>
    abstract Run: unit -> bool

    /// <summary>Request that the given operation be run synchronously on the event loop.</summary>
    ///
    /// <returns>The result of the operation.</returns>
    abstract Invoke: (unit -> 'T) -> 'T

    /// <summary>Schedule a restart for the event loop.</summary>
    abstract ScheduleRestart: unit -> unit

/// Operations supported by the currently executing F# Interactive session.
[<Sealed>]
type InteractiveSession =

    /// <summary>Get or set the floating point format used in the output of the interactive session.</summary>
    member FloatingPointFormat: string with get, set

    /// <summary>Get or set the format provider used in the output of the interactive session.</summary>
    member FormatProvider: System.IFormatProvider with get, set

    /// <summary>Get or set the print width of the interactive session.</summary>
    member PrintWidth: int with get, set

    /// <summary>Get or set the print depth of the interactive session.</summary>
    member PrintDepth: int with get, set

    /// <summary>Get or set the total print length of the interactive session.</summary>
    member PrintLength: int with get, set

    /// <summary>Get or set the total print size of the interactive session.</summary>
    member PrintSize: int with get, set

    /// <summary>When set to 'false', disables the display of properties of evaluated objects in the output of the interactive session.</summary>
    member ShowProperties: bool with get, set

    /// <summary>When set to 'false', disables the display of sequences in the output of the interactive session.</summary>
    member ShowIEnumerable: bool with get, set

    /// <summary>When set to 'false', disables the display of declaration values in the output of the interactive session.</summary>
    member ShowDeclarationValues: bool with get, set

    /// <summary>Register a printer that controls the output of the interactive session.</summary>
    member AddPrinter: ('T -> string) -> unit

    /// <summary>Register a print transformer that controls the output of the interactive session.</summary>
    member AddPrintTransformer: ('T -> obj) -> unit

    member internal AddedPrinters: Choice<(System.Type * (obj -> string)), (System.Type * (obj -> obj))> list

    /// <summary>The command line arguments after ignoring the arguments relevant to the interactive
    /// environment and replacing the first argument with the name of the last script file,
    /// if any. Thus 'fsi.exe test1.fs test2.fs -- hello goodbye' will give arguments
    /// 'test2.fs', 'hello', 'goodbye'.  This value will normally be different to those
    /// returned by System.Environment.GetCommandLineArgs.</summary>
    member CommandLineArgs: string[] with get, set

    /// <summary>Gets or sets a the current event loop being used to process interactions.</summary>
    member EventLoop: IEventLoop with get, set

    /// <summary>Sets the current event loop being used to process interactions.</summary>
    member internal SetEventLoop: (unit -> bool) * ((unit -> obj) -> obj) * (unit -> unit) -> unit


module Settings =

    /// <summary>The settings associated with the interactive session.</summary>
    val fsi: InteractiveSession

// For legacy compatibility with old naming
namespace Microsoft.FSharp.Compiler.Interactive

type IEventLoop = FSharp.Compiler.Interactive.IEventLoop

type InteractiveSession = FSharp.Compiler.Interactive.InteractiveSession

module Settings =

    val fsi: InteractiveSession
