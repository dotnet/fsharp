// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Pervasives: Additional bindings available at the top level
namespace Microsoft.FSharp.Core

/// <summary>A set of extra operators and functions. This module is automatically opened in all F# code.</summary>
///
/// <category>Basic Operators</category>
[<AutoOpen>]
module ExtraTopLevelOperators = 

    open System.IO
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Quotations

    /// <summary>Print to the console.</summary>
    ///
    /// <param name="text">The string to print.</param>
    ///
    /// <example id="print-1">Using interpolated strings:
    /// <code lang="fsharp">
    /// print $"Write three = {1+2}"
    /// </code>
    /// After evaluation the text <c>"Write three = 3"</c> is written to the console.
    /// </example>
    /// <example id="print-2">Using pipeline operator:
    /// <code lang="fsharp">
    /// "1"
    /// |> String.replicate 4
    /// |> print
    /// </code>
    /// After evaluation the text <c>"1111"</c> is written to the console.
    /// </example>
    [<CompiledName("Print")>]
    val print: text: string -> unit

    /// <summary>Print to the console and add a newline.</summary>
    ///
    /// <param name="text">The string to print.</param>
    ///
    /// <example id="println-1">Using interpolated strings:
    /// <code lang="fsharp">
    /// println $"Write three = {1+2}"
    /// println $"Write four = {2+2}"
    /// </code>
    /// After evaluation the two lines are written to the console.
    /// </example>
    /// <example id="println-2">Using pipeline operator:
    /// <code lang="fsharp">
    /// "hello " + "world"
    /// |> println
    /// </code>
    /// After evaluation the text <c>"hello world"</c> is written to the console.
    /// </example>
    [<CompiledName("PrintLine")>]
    val println: text: string -> unit

    /// <summary>Print to <c>stdout</c> using the given format.</summary>
    ///
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.printf</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormat``1'/>) for examples.</example>
    [<CompiledName("PrintFormat")>]
    val printf: format: Printf.TextWriterFormat<'T> -> 'T

    /// <summary>Print to <c>stdout</c> using the given format, and add a newline.</summary>
    ///
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.printfn</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatLine``1'/>) for examples.</example>
    [<CompiledName("PrintFormatLine")>]
    val printfn: format: Printf.TextWriterFormat<'T> -> 'T

    /// <summary>Print to <c>stderr</c> using the given format.</summary>
    ///
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.eprintf</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatToError``1'/>) for examples.</example>
    [<CompiledName("PrintFormatToError")>]
    val eprintf: format: Printf.TextWriterFormat<'T> -> 'T

    /// <summary>Print to <c>stderr</c> using the given format, and add a newline.</summary>
    ///
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.eprintfn</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatLineToError``1'/>) for examples.</example>
    [<CompiledName("PrintFormatLineToError")>]
    val eprintfn: format: Printf.TextWriterFormat<'T> -> 'T

    /// <summary>Print to a string using the given format.</summary>
    ///
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.sprintf</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThen``1'/>) for examples.</example>
    [<CompiledName("PrintFormatToString")>]
    val sprintf: format: Printf.StringFormat<'T> -> 'T

    /// <summary>Print to a string buffer and raise an exception with the given
    /// result. Helper printers must return strings.</summary>
    ///
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.failwithf</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatToStringThenFail``2'/>) for examples.</example>
    [<CompiledName("PrintFormatToStringThenFail")>]
    val failwithf: format: Printf.StringFormat<'T,'Result> -> 'T

    /// <summary>Print to a file using the given format.</summary>
    ///
    /// <param name="textWriter">The file TextWriter.</param>
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.fprintf</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatToTextWriter``1'/>) for examples.</example>
    [<CompiledName("PrintFormatToTextWriter")>]
    val fprintf: textWriter: TextWriter -> format:Printf.TextWriterFormat<'T> -> 'T

    /// <summary>Print to a file using the given format, and add a newline.</summary>
    ///
    /// <param name="textWriter">The file TextWriter.</param>
    /// <param name="format">The formatter.</param>
    ///
    /// <returns>The formatted result.</returns>
    /// 
    /// <example>See <c>Printf.fprintfn</c> (link: <see cref='M:Microsoft.FSharp.Core.PrintfModule.PrintFormatLineToTextWriter``1'/>) for examples.</example>
    [<CompiledName("PrintFormatLineToTextWriter")>]
    val fprintfn : textWriter: TextWriter -> format:Printf.TextWriterFormat<'T> -> 'T

    /// <summary>Builds a set from a sequence of objects. The objects are indexed using generic comparison.</summary>
    ///
    /// <param name="elements">The input sequence of elements.</param>
    ///
    /// <returns>The created set.</returns>
    /// 
    /// <example id="set-1">
    /// <code lang="fsharp">
    /// let values = set [ 1; 2; 3; 5; 7; 11 ]
    /// </code>
    /// Evaluates to a set containing the given numbers.
    /// </example>
    [<CompiledName("CreateSet")>]
    val set: elements: seq<'T> -> Set<'T>

    /// <summary>Builds an asynchronous workflow using computation expression syntax.</summary>
    /// 
    /// <example id="async-1">
    /// <code lang="fsharp">
    /// let sleepExample() =
    ///     async {
    ///         printfn "sleeping"
    ///         do! Async.Sleep 10
    ///         printfn "waking up"
    ///         return 6
    ///      }
    ///
    /// sleepExample() |> Async.RunSynchronously
    /// </code>
    /// </example>
    [<CompiledName("DefaultAsyncBuilder")>]
    val async: Microsoft.FSharp.Control.AsyncBuilder  

    /// <summary>Converts the argument to 32-bit float.</summary>
    ///
    /// <remarks>This is a direct conversion for all 
    /// primitive numeric types. For strings, the input is converted using <c>Single.Parse()</c>  with InvariantCulture settings. Otherwise the operation requires and invokes a <c>ToSingle</c> method on the input type.</remarks>
    /// 
    /// <example id="single-1">
    /// <code lang="fsharp">
    /// single 45
    /// </code>
    /// Evaluates to <c>45.0f</c>.
    /// </example>
    [<CompiledName("ToSingle")>]
    val inline single: value: ^T -> single when ^T : (static member op_Explicit : ^T -> single) and default ^T : int

    /// <summary>Converts the argument to 64-bit float.</summary>
    ///
    /// <remarks>This is a direct conversion for all 
    /// primitive numeric types. For strings, the input is converted using <c>Double.Parse()</c>  with InvariantCulture settings. Otherwise the operation requires and invokes a <c>ToDouble</c> method on the input type.</remarks>
    /// 
    /// <example id="double-1">
    /// <code lang="fsharp">
    /// double 45
    /// </code>
    /// Evaluates to <c>45.0</c>.
    /// </example>
    /// 
    /// <example id="double-2">
    /// <code lang="fsharp">
    /// double 12.3f
    /// </code>
    /// Evaluates to <c>12.30000019</c>.
    /// </example>
    [<CompiledName("ToDouble")>]
    val inline double: value: ^T -> double when ^T : (static member op_Explicit : ^T -> double) and default ^T : int

    /// <summary>Converts the argument to byte.</summary>
    /// <remarks>This is a direct conversion for all 
    /// primitive numeric types. For strings, the input is converted using <c>Byte.Parse()</c> on strings and otherwise requires a <c>ToByte</c> method on the input type.</remarks>
    /// 
    /// <example id="uint8-1">
    /// <code lang="fsharp">
    /// uint8 12
    /// </code>
    /// Evaluates to <c>12uy</c>.
    /// </example>
    [<CompiledName("ToByte")>]
    val inline uint8: value: ^T -> uint8 when ^T : (static member op_Explicit : ^T -> uint8) and default ^T : int        
    
    /// <summary>Converts the argument to signed byte.</summary>
    /// <remarks>This is a direct conversion for all 
    /// primitive numeric types. For strings, the input is converted using <c>SByte.Parse()</c>  with InvariantCulture settings.
    /// Otherwise the operation requires and invokes a <c>ToSByte</c> method on the input type.</remarks>
    /// 
    /// <example id="int8-1">
    /// <code lang="fsharp">
    /// int8 -12
    /// </code>
    /// Evaluates to <c>-12y</c>.
    /// </example>
    /// 
    /// <example id="int8-2">
    /// <code lang="fsharp">
    /// int8 "3"
    /// </code>
    /// Evaluates to <c>3y</c>.
    /// </example>
    [<CompiledName("ToSByte")>]
    val inline int8: value: ^T -> int8 when ^T : (static member op_Explicit : ^T -> int8) and default ^T : int

    module Checked = 

        /// <summary>Converts the argument to byte.</summary>
        /// <remarks>This is a direct, checked conversion for all 
        /// primitive numeric types. For strings, the input is converted using <c>Byte.Parse()</c> on strings and otherwise requires a <c>ToByte</c> method on the input type.</remarks>
        /// 
        /// <example id="uint8-1">
        /// <code lang="fsharp">
        /// Checked.uint8 12
        /// </code>
        /// Evaluates to <c>-12y</c>.
        /// </example>
        /// 
        /// <example id="uint8-2">
        /// <code lang="fsharp">
        /// Checked.uint8 -12
        /// </code>
        /// Throws <c>System.OverflowException</c>.
        /// </example>
        [<CompiledName("ToByte")>]
        val inline uint8: value: ^T -> byte when ^T : (static member op_Explicit : ^T -> uint8) and default ^T : int        
    
        /// <summary>Converts the argument to signed byte.</summary>
        /// <remarks>This is a direct, checked conversion for all 
        /// primitive numeric types. For strings, the input is converted using <c>SByte.Parse()</c>  with InvariantCulture settings.
        /// Otherwise the operation requires and invokes a <c>ToSByte</c> method on the input type.</remarks>
        /// 
        /// <example id="int8-1">
        /// <code lang="fsharp">
        /// Checked.int8 -12
        /// </code>
        /// Evaluates to <c>-12y</c>.
        /// </example>
        /// 
        /// <example id="int8-2">
        /// <code lang="fsharp">
        /// Checked.int8 "129"
        /// </code>
        /// Throws <c>System.OverflowException</c>.
        /// </example>
        [<CompiledName("ToSByte")>]
        val inline int8: value: ^T -> sbyte when ^T : (static member op_Explicit : ^T -> int8) and default ^T : int

    /// <summary>Builds a read-only lookup table from a sequence of key/value pairs. The key objects are indexed using generic hashing and equality.</summary>
    /// 
    /// <example id="dict-1">
    /// <code lang="fsharp">
    /// let table = dict [ (1, 100); (2, 200) ]
    ///
    /// table[1]
    /// </code>
    /// Evaluates to <c>100</c>.
    /// </example>
    /// 
    /// <example id="dict-2">
    /// <code lang="fsharp">
    /// let table = dict [ (1, 100); (2, 200) ]
    ///
    /// table[3]
    /// </code>
    /// Throws <c>System.Collections.Generic.KeyNotFoundException</c>.
    /// </example>
    [<CompiledName("CreateDictionary")>]
    val dict: keyValuePairs: seq<'Key * 'Value> -> System.Collections.Generic.IDictionary<'Key,'Value> when 'Key : equality

    /// <summary>Builds a read-only lookup table from a sequence of key/value pairs. The key objects are indexed using generic hashing and equality.</summary>
    /// 
    /// <example id="readonlydict-1">
    /// <code lang="fsharp">
    /// let table = readOnlyDict [ (1, 100); (2, 200) ]
    ///
    /// table[1]
    /// </code>
    /// Evaluates to <c>100</c>.
    /// </example>
    /// 
    /// <example id="readonlydict-2">
    /// <code lang="fsharp">
    /// let table = readOnlyDict [ (1, 100); (2, 200) ]
    ///
    /// table[3]
    /// </code>
    /// Throws <c>System.Collections.Generic.KeyNotFoundException</c>.
    /// </example>
    [<CompiledName("CreateReadOnlyDictionary")>]
    val readOnlyDict: keyValuePairs: seq<'Key * 'Value> -> System.Collections.Generic.IReadOnlyDictionary<'Key,'Value> when 'Key : equality

    /// <summary>Builds a 2D array from a sequence of sequences of elements.</summary>
    /// 
    /// <example id="array2d-1">
    /// <code lang="fsharp">
    /// array2D [ [ 1.0; 2.0 ]; [ 3.0; 4.0 ] ]
    /// </code>
    /// Evaluates to a 2x2 zero-based array with contents <c>[[1.0; 2.0]; [3.0; 4.0]]</c>
    /// </example>
    [<CompiledName("CreateArray2D")>]
    val array2D: rows: seq<#seq<'T>> -> 'T[,]

    /// <summary>Special prefix operator for splicing typed expressions into quotation holes.</summary>
    /// 
    /// <example id="splice-1">
    /// <code lang="fsharp">
    /// let f v = &lt;@ %v + %v @>
    ///
    /// f &lt;@ 5 + 5 @>;;
    /// </code>
    /// Evaluates to a quotation equivalent to <c>&lt;@ (5 + 5) + (5 + 5) @> </c>
    /// </example>
    [<CompiledName("SpliceExpression")>]
    val (~%): expression: Expr<'T> -> 'T

    /// <summary>Special prefix operator for splicing untyped expressions into quotation holes.</summary>
    /// 
    /// <example id="rawsplice-1">
    /// <code lang="fsharp">
    /// let f v = &lt;@@ (%%v: int) + (%%v: int) @@>
    ///
    /// f &lt;@@ 5 + 5 @@>;;
    /// </code>
    /// Evaluates to an untyped quotation equivalent to <c>&lt;@@ (5 + 5) + (5 + 5) @@> </c>
    /// </example>
    [<CompiledName("SpliceUntypedExpression")>]
    val (~%%): expression: Expr -> 'T

    /// <summary>An active pattern to force the execution of values of type <c>Lazy&lt;_&gt;</c>.</summary>
    /// 
    /// <example id="lazy-1">
    /// <code lang="fsharp">
    /// let f (Lazy v) = v + v
    ///
    /// let v = lazy (printf "eval!"; 5+5)
    ///
    /// f v
    /// f v
    /// </code>
    /// Evaluates to <c>10</c>. The text <c>eval!</c> is printed once on the first invocation of <c>f</c>.
    /// </example>
    [<CompiledName("LazyPattern")>]
    val (|Lazy|): input: Lazy<'T> -> 'T

    /// <summary>Builds a query using query syntax and operators.</summary>
    /// 
    /// <example id="query-1">
    /// <code lang="fsharp">
    /// let findEvensAndSortAndDouble(xs: System.Linq.IQueryable&lt;int>) =
    ///     query {
    ///         for x in xs do
    ///         where (x % 2 = 0)
    ///         sortBy x
    ///         select (x+x)
    ///      }
    ///
    /// let data = [1; 2; 6; 7; 3; 6; 2; 1]
    ///
    /// findEvensAndSortAndDouble (data.AsQueryable()) |> Seq.toList
    /// </code>
    /// Evaluates to <c>[4; 4; 12; 12]</c>.
    /// </example>
    val query: Microsoft.FSharp.Linq.QueryBuilder

namespace Microsoft.FSharp.Core.CompilerServices

    open System
    open System.Reflection
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Quotations

    /// <summary>Represents the product of two measure expressions when returned as a generic argument of a provided type.</summary>
    ///
    /// <namespacedoc><summary>
    ///   Library functionality for supporting type providers and code generated by the F# compiler. See
    ///   also <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/tutorials/type-providers/">F# Type Providers</a> in the F# Language Guide.
    /// </summary></namespacedoc>
    type MeasureProduct<'Measure1, 'Measure2> 

    /// <summary>Represents the inverse of a measure expressions when returned as a generic argument of a provided type.</summary>
    type MeasureInverse<'Measure> 

    /// <summary>Represents the '1' measure expression when returned as a generic argument of a provided type.</summary>
    type MeasureOne

    /// <summary>Place on a class that implements ITypeProvider to extend the compiler</summary>
    [<AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false)>]
    type TypeProviderAttribute =
        inherit System.Attribute

        /// <summary>Creates an instance of the attribute</summary>
        /// <returns>TypeProviderAttribute</returns>
        new : unit -> TypeProviderAttribute

    /// <summary>Additional type attribute flags related to provided types</summary>
    type TypeProviderTypeAttributes =
        | SuppressRelocate = 0x80000000
        | IsErased = 0x40000000

    /// <summary>Place this attribute on a runtime assembly to indicate that there is a corresponding design-time 
    /// assembly that contains a type provider. Runtime and design-time assembly may be the same. </summary>
    [<AttributeUsageAttribute(AttributeTargets.Assembly, AllowMultiple = false)>]
    type TypeProviderAssemblyAttribute = 
        inherit System.Attribute

        /// <summary>Creates an instance of the attribute</summary>
        /// <returns>TypeProviderAssemblyAttribute</returns>
        new : unit -> TypeProviderAssemblyAttribute

        /// <summary>Creates an instance of the attribute</summary>
        /// <returns>TypeProviderAssemblyAttribute</returns>
        /// <param name="assemblyName">The name of the design-time assembly for this type provider.</param>
        new : assemblyName : string -> TypeProviderAssemblyAttribute

        /// <summary>Gets the assembly name.</summary>
        member AssemblyName : string

    /// <summary>A type provider may provide an instance of this attribute to indicate the documentation to show for 
    /// a provided type or member.</summary>
    [<AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)>]
    type TypeProviderXmlDocAttribute = 
        inherit System.Attribute

        /// <summary>Creates an instance of the attribute</summary>
        /// <returns>TypeProviderXmlDocAttribute</returns>
        new : commentText : string -> TypeProviderXmlDocAttribute

        /// <summary>Gets the comment text.</summary>
        member CommentText : string

    /// <summary>A type provider may provide an instance of this attribute to indicate the definition location for a provided type or member.</summary>
    [<AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = false)>]
    type TypeProviderDefinitionLocationAttribute = 
        inherit System.Attribute
        new : unit -> TypeProviderDefinitionLocationAttribute

        /// <summary>Gets or sets the file path for the definition location.</summary>
        member FilePath : string with get, set

        /// <summary>Gets or sets the line for the location.</summary>
        member Line : int with get, set

        /// <summary>Gets or sets the column for the location.</summary>
        member Column : int with get, set

    /// <summary>Indicates that a code editor should hide all System.Object methods from the intellisense menus for instances of a provided type</summary>
    [<AttributeUsageAttribute(AttributeTargets.Class ||| AttributeTargets.Interface ||| AttributeTargets.Struct ||| AttributeTargets.Delegate, AllowMultiple = false)>]
    type TypeProviderEditorHideMethodsAttribute = 
        inherit System.Attribute

        /// <summary>Creates an instance of the attribute</summary>
        /// <returns>TypeProviderEditorHideMethodsAttribute</returns>
        new : unit -> TypeProviderEditorHideMethodsAttribute

    /// If the class that implements ITypeProvider has a constructor that accepts TypeProviderConfig
    /// then it will be constructed with an instance of TypeProviderConfig.
    type TypeProviderConfig =
        new :  systemRuntimeContainsType : (string -> bool) -> TypeProviderConfig

        /// Get the full path to use to resolve relative paths in any file name arguments given to the type provider instance.
        member ResolutionFolder : string with get,set

        /// Get the full path to referenced assembly that caused this type provider instance to be created.
        member RuntimeAssembly : string with get,set

        /// Get the referenced assemblies for the type provider instance.
        member ReferencedAssemblies : string[] with get,set

        /// Get the full path to use for temporary files for the type provider instance.
        member TemporaryFolder : string with get,set

        /// Indicates if the type provider host responds to invalidation events for type provider instances. 
        member IsInvalidationSupported : bool with get,set

        /// Indicates if the type provider instance is used in an environment which executes provided code such as F# Interactive.
        member IsHostedExecution : bool with get,set

        /// version of referenced system runtime assembly
        member SystemRuntimeAssemblyVersion : System.Version with get,set

        /// Checks if given type exists in target system runtime library
        member SystemRuntimeContainsType : string -> bool

    /// <summary>
    /// Represents a namespace provided by a type provider component.
    /// </summary>
    type IProvidedNamespace =
        /// Namespace name the provider injects types into.
        abstract NamespaceName : string

        /// The sub-namespaces in this namespace. An optional member to prevent generation of namespaces until an outer namespace is explored.
        abstract GetNestedNamespaces : unit -> IProvidedNamespace[] 

        /// <summary>
        /// The top-level types
        /// </summary>
        /// <returns></returns>
        abstract GetTypes : unit -> Type[] 

        /// <summary>
        /// Compilers call this method to query a type provider for a type <c>name</c>.
        /// </summary>
        /// <remarks>Resolver should return a type called <c>name</c> in namespace <c>NamespaceName</c> or <c>null</c> if the type is unknown.
        /// </remarks>
        /// <returns></returns>
        abstract ResolveTypeName : typeName: string -> Type

    /// <summary>
    /// Represents an instantiation of a type provider component.
    /// </summary>
    type ITypeProvider =
        inherit System.IDisposable

        /// <summary>
        /// Gets the namespaces provided by the type provider.
        /// </summary>
        abstract GetNamespaces : unit -> IProvidedNamespace[] 

        /// <summary>
        /// Get the static parameters for a provided type. 
        /// </summary>
        ///
        /// <param name="typeWithoutArguments">A type returned by GetTypes or ResolveTypeName</param>
        ///
        /// <returns></returns>
        abstract GetStaticParameters : typeWithoutArguments:Type -> ParameterInfo[]

        /// <summary>
        /// Apply static arguments to a provided type that accepts static arguments. 
        /// </summary>
        ///
        /// <remarks>The provider must return a type with the given mangled name.</remarks>
        ///
        /// <param name="typeWithoutArguments">the provided type definition which has static parameters</param>
        /// <param name="typePathWithArguments">the full path of the type, including encoded representations of static parameters</param>
        /// <param name="staticArguments">the static parameters, indexed by name</param>
        ///
        /// <returns></returns>
        abstract ApplyStaticArguments : typeWithoutArguments:Type * typePathWithArguments:string[] * staticArguments:obj[] -> Type 

        /// <summary>
        /// Called by the compiler to ask for an Expression tree to replace the given MethodBase with.
        /// </summary>
        ///
        /// <param name="syntheticMethodBase">MethodBase that was given to the compiler by a type returned by a GetType(s) call.</param>
        /// <param name="parameters">Expressions that represent the parameters to this call.</param>
        ///
        /// <returns>An expression that the compiler will use in place of the given method base.</returns>
        abstract GetInvokerExpression : syntheticMethodBase:MethodBase * parameters:Expr[] -> Expr

        /// <summary>
        /// Triggered when an assumption changes that invalidates the resolutions so far reported by the provider
        /// </summary>
        [<CLIEvent>]
        abstract Invalidate : IEvent<System.EventHandler, System.EventArgs>

        /// <summary>
        /// Get the physical contents of the given logical provided assembly.
        /// </summary>
        abstract GetGeneratedAssemblyContents : assembly:Assembly -> byte[]

    /// Represents additional, optional information for a type provider component
    type ITypeProvider2 =

        /// <summary>
        /// Get the static parameters for a provided method. 
        /// </summary>
        ///
        /// <param name="methodWithoutArguments">A method returned by GetMethod on a provided type</param>
        ///
        /// <returns>The static parameters of the provided method, if any</returns>
        abstract GetStaticParametersForMethod : methodWithoutArguments:MethodBase -> ParameterInfo[] 

        /// <summary>
        /// Apply static arguments to a provided method that accepts static arguments. 
        /// </summary>
        /// <remarks>The provider must return a provided method with the given mangled name.</remarks>
        /// <param name="methodWithoutArguments">the provided method definition which has static parameters</param>
        /// <param name="methodNameWithArguments">the full name of the method that must be returned, including encoded representations of static parameters</param>
        /// <param name="staticArguments">the values of the static parameters, indexed by name</param>
        ///
        /// <returns>The provided method definition corresponding to the given static parameter values</returns>
        abstract ApplyStaticArgumentsForMethod : methodWithoutArguments:MethodBase * methodNameWithArguments:string * staticArguments:obj[] -> MethodBase
