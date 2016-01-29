// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// This file is compiled 4(!) times in the codebase
//    - as the internal implementation of printf '%A' formatting 
//           defines: RUNTIME
//    - as the internal implementation of structured formatting in the FSharp.Compiler-proto.dll 
//           defines: COMPILER + BUILDING_WITH_LKG
//    - as the internal implementation of structured formatting in FSharp.Compiler.dll 
//           defines: COMPILER 
//           NOTE: this implementation is used by fsi.exe. This is very important.
//    - as the public implementation of structured formatting in the FSharp.PowerPack.dll  
//           defines: <none> 
//
// The one implementation file is used because we very much want to keep the implementations of
// structured formatting the same for fsi.exe and '%A' printing. However fsi.exe may have
// a richer feature set.
//
// Note no layout objects are ever transferred between the above implementations, and in 
// all 4 cases the layout types are really different types.

#if COMPILER
// FSharp.Compiler-proto.dll:
// FSharp.Compiler.dll:
namespace Internal.Utilities.StructuredFormat
#else
#if RUNTIME 
// FSharp.Core.dll:
namespace Microsoft.FSharp.Text.StructuredPrintfImpl
#else
// Powerpack: 
namespace Microsoft.FSharp.Text.StructuredFormat
#endif
#endif

    open System
    open System.IO
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics

    /// Data representing structured layouts of terms.  
#if RUNTIME  // FSharp.Core.dll makes things internal and hides representations
    type internal Layout
#else  // FSharp.Compiler.dll, FSharp.Compiler-proto.dll, FSharp.PowerPack.dll
    // FSharp.PowerPack.dll: reveals representations
    // FSharp.Compiler-proto.dll, FSharp.Compiler.dll: the F# compiler likes to see these representations

    /// Data representing joints in structured layouts of terms.  The representation
    /// of this data type is only for the consumption of formatting engines.
    [<StructuralEquality; NoComparison>]
#if COMPILER
    type internal Joint =
#else
    type Joint =
#endif
        | Unbreakable
        | Breakable of int
        | Broken of int

    /// Data representing structured layouts of terms.  The representation
    /// of this data type is only for the consumption of formatting engines.
    [<NoEquality; NoComparison>]
#if COMPILER
    type internal Layout =
#else
    type Layout =
#endif
     | Leaf of bool * obj * bool
     | Node of bool * Layout * bool * Layout * bool * Joint
     | Attr of string * (string * string) list * Layout
#endif


#if RUNTIME   // FSharp.Core.dll doesn't use PrintIntercepts
#else  // FSharp.Compiler.dll, FSharp.Compiler-proto.dll, FSharp.PowerPack.dll
#if COMPILER
    type internal IEnvironment = 
#else
    type IEnvironment = 
#endif
        /// Return to the layout-generation 
        /// environment to layout any otherwise uninterpreted object
        abstract GetLayout : obj -> Layout
        /// The maximum number of elements for which to generate layout for 
        /// list-like structures, or columns in table-like 
        /// structures.  -1 if no maximum.
        abstract MaxColumns : int
        /// The maximum number of rows for which to generate layout for table-like 
        /// structures.  -1 if no maximum.
        abstract MaxRows : int
#endif
      
    /// A layout is a sequence of strings which have been joined together.
    /// The strings are classified as words, separators and left and right parenthesis.
    /// This classification determines where spaces are inserted.
    /// A joint is either unbreakable, breakable or broken.
    /// If a joint is broken the RHS layout occurs on the next line with optional indentation.
    /// A layout can be squashed to for given width which forces breaks as required.
    module
#if RUNTIME   // FSharp.Core.dll
      internal 
#else
#if COMPILER
      internal
#endif
#endif
         LayoutOps =

        /// The empty layout
        val emptyL     : Layout
        /// Is it the empty layout?
        val isEmptyL   : layout:Layout -> bool
        
        /// An uninterpreted leaf, to be interpreted into a string
        /// by the layout engine. This allows leaf layouts for numbers, strings and
        /// other atoms to be customized according to culture.
        val objL       : value:obj -> Layout

        /// An string leaf 
        val wordL      : text:string -> Layout
        /// An string which requires no spaces either side.
        val sepL       : text:string -> Layout
        /// An string which is right parenthesis (no space on the left).
        val rightL     : text:string -> Layout
        /// An string which is left  parenthesis (no space on the right).
        val leftL      : text:string -> Layout

        /// Join, unbreakable. 
        val ( ^^ )     : layout1:Layout -> layout2:Layout -> Layout   
        /// Join, possible break with indent=0
        val ( ++ )     : layout1:Layout -> layout2:Layout -> Layout   
        /// Join, possible break with indent=1
        val ( -- )     : layout1:Layout -> layout2:Layout -> Layout   
        /// Join, possible break with indent=2 
        val ( --- )    : layout1:Layout -> layout2:Layout -> Layout   
        /// Join broken with ident=0
        val ( @@ )     : layout1:Layout -> layout2:Layout -> Layout   
        /// Join broken with ident=1 
        val ( @@- )    : layout1:Layout -> layout2:Layout -> Layout   
        /// Join broken with ident=2 
        val ( @@-- )   : layout1:Layout -> layout2:Layout -> Layout   

        /// Join layouts into a comma separated list.
        val commaListL : layouts:Layout list -> Layout
          
        /// Join layouts into a space separated list.    
        val spaceListL : layouts:Layout list -> Layout
          
        /// Join layouts into a semi-colon separated list.
        val semiListL  : layouts:Layout list -> Layout

        /// Join layouts into a list separated using the given Layout.
        val sepListL   : layout1:Layout -> layouts:Layout list -> Layout

        /// Wrap round brackets around Layout.
        val bracketL   : Layout:Layout -> Layout
        /// Wrap square brackets around layout.    
        val squareBracketL   : layout:Layout -> Layout
        /// Wrap braces around layout.        
        val braceL     : layout:Layout -> Layout
        /// Form tuple of layouts.            
        val tupleL     : layouts:Layout list -> Layout
        /// Layout two vertically.
        val aboveL     : layout1:Layout -> layout2:Layout -> Layout
        /// Layout list vertically.    
        val aboveListL : layouts:Layout list -> Layout

        /// Layout like an F# option.
        val optionL    : selector:('T -> Layout) -> value:'T option -> Layout
        /// Layout like an F# list.    
        val listL      : selector:('T -> Layout) -> value:'T list   -> Layout

        /// See tagL
        val tagAttrL : text:string -> maps:(string * string) list -> layout:Layout -> Layout

        /// For limiting layout of list-like sequences (lists,arrays,etc).
        /// unfold a list of items using (project and z) making layout list via itemL.
        /// If reach maxLength (before exhausting) then truncate.
        val unfoldL : selector:('T -> Layout) -> folder:('State -> ('T * 'State) option) -> state:'State -> count:int -> Layout list

    /// A record of options to control structural formatting.
    /// For F# Interactive properties matching those of this value can be accessed via the 'fsi'
    /// value.
    /// 
    /// Floating Point format given in the same format accepted by System.Double.ToString,
    /// e.g. f6 or g15.
    ///
    /// If ShowProperties is set the printing process will evaluate properties of the values being
    /// displayed.  This may cause additional computation.  
    ///
    /// The ShowIEnumerable is set the printing process will force the evaluation of IEnumerable objects
    /// to a small, finite depth, as determined by the printing parameters.
    /// This may lead to additional computation being performed during printing.
    ///
    /// <example>
    /// From F# Interactive the default settings can be adjusted using, for example, 
    /// <pre>
    ///   open Microsoft.FSharp.Compiler.Interactive.Settings;;
    ///   setPrintWidth 120;;
    /// </pre>
    /// </example>
    [<NoEquality; NoComparison>]
    type
#if RUNTIME   // FSharp.Core.dll
      internal 
#else
#if COMPILER
      internal
#endif
#endif
         FormatOptions = 
        { FloatingPointFormat: string
          AttributeProcessor: (string -> (string * string) list -> bool -> unit);
#if RUNTIME  // FSharp.Core.dll: PrintIntercepts aren't used there
#else
#if COMPILER    // FSharp.Compiler.dll: This is the PrintIntercepts extensibility point currently revealed by fsi.exe's AddPrinter
          PrintIntercepts: (IEnvironment -> obj -> Layout option) list;
          StringLimit: int;
#endif
#endif
          FormatProvider: System.IFormatProvider
#if FX_RESHAPED_REFLECTION
          ShowNonPublic : bool
#else
          BindingFlags: System.Reflection.BindingFlags
#endif
          PrintWidth : int 
          PrintDepth : int 
          PrintLength : int
          PrintSize : int  
          ShowProperties : bool
          ShowIEnumerable: bool  }
        static member Default : FormatOptions

    module
#if RUNTIME   // FSharp.Core.dll
      internal 
#else
#if COMPILER
      internal
#endif
#endif
         Display = 


        /// Convert any value to a string using a standard formatter
        /// Data is typically formatted in a structured format, e.g.
        /// lists are formatted using the "[1;2]" notation.
        /// The details of the format are not specified and may change
        /// from version to version and according to the flags given
        /// to the F# compiler.  The format is intended to be human-readable,
        /// not machine readable.  If alternative generic formats are required
        /// you should develop your own formatter, using the code in the
        /// implementation of this file as a starting point.
        ///
        /// Data from other .NET languages is formatted using a virtual
        /// call to Object.ToString() on the boxed version of the input.
        val any_to_string: value:'T -> string

        /// Output any value to a channel using the same set of formatting rules
        /// as any_to_string
        val output_any: writer:TextWriter -> value:'T -> unit

#if RUNTIME   // FSharp.Core.dll: Most functions aren't needed in FSharp.Core.dll, but we add one entry for printf

#if FX_RESHAPED_REFLECTION
        val anyToStringForPrintf: options:FormatOptions -> showNonPublicMembers : bool -> value:'T -> string
#else
        val anyToStringForPrintf: options:FormatOptions -> bindingFlags:System.Reflection.BindingFlags -> value:'T -> string
#endif
#else
        val any_to_layout   : options:FormatOptions -> value:'T -> Layout
        val squash_layout   : options:FormatOptions -> layout:Layout -> Layout
        val output_layout   : options:FormatOptions -> writer:TextWriter -> layout:Layout -> unit
        val layout_as_string: options:FormatOptions -> value:'T -> string
#endif

        /// Convert any value to a layout using the given formatting options.  The
        /// layout can then be processed using formatting display engines such as
        /// those in the LayoutOps module.  any_to_string and output_any are
        /// built using any_to_layout with default format options.
        val layout_to_string: options:FormatOptions -> layout:Layout -> string


#if COMPILER
        val fsi_any_to_layout : options:FormatOptions -> value:'T -> Layout
#endif  
