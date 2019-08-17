// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// This file is compiled 2(!) times in the codebase
//    - as the internal implementation of printf '%A' formatting 
//           defines: FSHARP_CORE
//    - as the internal implementation of structured formatting in FSharp.Compiler.Service.dll 
//           defines: COMPILER 
//           NOTE: this implementation is used by fsi.exe. This is very important.
//
// The one implementation file is used because we very much want to keep the implementations of
// structured formatting the same for fsi.exe and '%A' printing. However fsi.exe may have
// a richer feature set.
//
// Note no layout objects are ever transferred between the above implementations, and in 
// all 4 cases the layout types are really different types.

#if COMPILER
// fsc.exe:
// FSharp.Compiler.Service.dll:
namespace Internal.Utilities.StructuredFormat
#else
// FSharp.Core.dll:
namespace Microsoft.FSharp.Text.StructuredPrintfImpl
#endif

    open System
    open System.IO
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics

    /// Data representing structured layouts of terms.  
#if FSHARP_CORE  // FSharp.Core.dll makes things internal and hides representations
    type internal Layout
    type internal LayoutTag
    type internal TaggedText =
        abstract Tag: LayoutTag
        abstract Text: string
#else  // FSharp.Compiler.Service.dll, fsc.exe

    /// Data representing joints in structured layouts of terms.  The representation
    /// of this data type is only for the consumption of formatting engines.
    [<StructuralEquality; NoComparison>]
    type public Joint =
        | Unbreakable
        | Breakable of int
        | Broken of int
    
    [<StructuralEquality; NoComparison>]
    type public LayoutTag =
        | ActivePatternCase
        | ActivePatternResult
        | Alias
        | Class
        | Union
        | UnionCase
        | Delegate
        | Enum
        | Event
        | Field
        | Interface
        | Keyword
        | LineBreak
        | Local
        | Record
        | RecordField
        | Method
        | Member
        | ModuleBinding
        | Module
        | Namespace
        | NumericLiteral
        | Operator
        | Parameter
        | Property
        | Space
        | StringLiteral
        | Struct
        | TypeParameter
        | Text
        | Punctuation
        | UnknownType
        | UnknownEntity

    type public TaggedText =
        abstract Tag : LayoutTag
        abstract Text : string

    
    type public TaggedTextWriter =
        abstract Write: t: TaggedText -> unit
        abstract WriteLine: unit -> unit

    /// Data representing structured layouts of terms.  The representation
    /// of this data type is only for the consumption of formatting engines.
    [<NoEquality; NoComparison>]
    type public Layout =
     | ObjLeaf of bool * obj * bool
     | Leaf of bool * TaggedText * bool
     | Node of bool * Layout * bool * Layout * bool * Joint
     | Attr of string * (string * string) list * Layout
#endif

#if COMPILER
    module public TaggedTextOps =
#else
    module internal TaggedTextOps =
#endif
        val tag : LayoutTag -> string -> TaggedText
        val keywordFunctions : Set<string>
        val tagAlias : string -> TaggedText
        val tagClass : string -> TaggedText
        val tagUnionCase : string -> TaggedText
        val tagDelegate : string -> TaggedText
        val tagEnum : string -> TaggedText
        val tagEvent : string -> TaggedText
        val tagField : string -> TaggedText
        val tagInterface : string -> TaggedText
        val tagKeyword : string -> TaggedText
        val tagLineBreak : string -> TaggedText
        val tagMethod : string -> TaggedText
        val tagModuleBinding : string -> TaggedText
        val tagLocal : string -> TaggedText
        val tagRecord : string -> TaggedText
        val tagRecordField : string -> TaggedText
        val tagModule : string -> TaggedText
        val tagNamespace : string -> TaggedText
        val tagNumericLiteral : string -> TaggedText
        val tagOperator : string -> TaggedText
        val tagParameter : string -> TaggedText
        val tagProperty : string -> TaggedText
        val tagSpace : string -> TaggedText
        val tagStringLiteral : string -> TaggedText
        val tagStruct : string -> TaggedText
        val tagTypeParameter : string -> TaggedText
        val tagText : string -> TaggedText
        val tagPunctuation : string -> TaggedText

        module Literals =
            // common tagged literals
            val lineBreak : TaggedText
            val space : TaggedText
            val comma : TaggedText
            val semicolon : TaggedText
            val leftParen : TaggedText
            val rightParen : TaggedText
            val leftBracket : TaggedText
            val rightBracket : TaggedText
            val leftBrace: TaggedText
            val rightBrace : TaggedText
            val leftBraceBar: TaggedText
            val rightBraceBar : TaggedText
            val equals : TaggedText
            val arrow : TaggedText
            val questionMark : TaggedText

#if COMPILER
    type public IEnvironment = 
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
#if COMPILER
    module public LayoutOps =
#else
    module internal LayoutOps =
#endif

        /// The empty layout
        val emptyL     : Layout

        /// Is it the empty layout?
        val isEmptyL   : layout:Layout -> bool

        /// An uninterpreted leaf, to be interpreted into a string
        /// by the layout engine. This allows leaf layouts for numbers, strings and
        /// other atoms to be customized according to culture.
        val objL       : value:obj -> Layout

        /// An string leaf 
        val wordL      : text:TaggedText -> Layout

        /// An string which requires no spaces either side.
        val sepL       : text:TaggedText -> Layout

        /// An string which is right parenthesis (no space on the left).
        val rightL     : text:TaggedText -> Layout

        /// An string which is left  parenthesis (no space on the right).
        val leftL      : text:TaggedText -> Layout

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
        val bracketL   : layout:Layout -> Layout

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
    ///   open FSharp.Compiler.Interactive.Settings;;
    ///   setPrintWidth 120;;
    /// </pre>
    /// </example>
    [<NoEquality; NoComparison>]
#if COMPILER
    type public FormatOptions =
#else
    type internal FormatOptions =
#endif
        { FloatingPointFormat: string
          AttributeProcessor: (string -> (string * string) list -> bool -> unit);
#if COMPILER  // FSharp.Core.dll: PrintIntercepts aren't used there
          PrintIntercepts: (IEnvironment -> obj -> Layout option) list;
          StringLimit: int;
#endif
          FormatProvider: System.IFormatProvider
          BindingFlags: System.Reflection.BindingFlags
          PrintWidth : int 
          PrintDepth : int 
          PrintLength : int
          PrintSize : int  
          ShowProperties : bool
          ShowIEnumerable: bool  }
        static member Default : FormatOptions

#if COMPILER
    module public Display =
#else
    module internal Display =
#endif

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
        val any_to_string: value:'T * Type -> string

        /// Output any value to a channel using the same set of formatting rules
        /// as any_to_string
        val output_any: writer:TextWriter -> value:'T * Type -> unit

#if FSHARP_CORE   // FSharp.Core.dll: Most functions aren't needed in FSharp.Core.dll, but we add one entry for printf

        val anyToStringForPrintf: options:FormatOptions -> bindingFlags:System.Reflection.BindingFlags -> value:'T * Type -> string
#else
        val asTaggedTextWriter: writer: TextWriter -> TaggedTextWriter
        val any_to_layout   : options:FormatOptions -> value:'T * Type -> Layout
        val squash_layout   : options:FormatOptions -> layout:Layout -> Layout
        val output_layout_tagged   : options:FormatOptions -> writer:TaggedTextWriter -> layout:Layout -> unit
        val output_layout   : options:FormatOptions -> writer:TextWriter -> layout:Layout -> unit
        val layout_as_string: options:FormatOptions -> value:'T * Type -> string
#endif

        /// Convert any value to a layout using the given formatting options.  The
        /// layout can then be processed using formatting display engines such as
        /// those in the LayoutOps module.  any_to_string and output_any are
        /// built using any_to_layout with default format options.
        val layout_to_string: options:FormatOptions -> layout:Layout -> string


#if COMPILER
        val fsi_any_to_layout : options:FormatOptions -> value:'T * Type -> Layout
#endif  
