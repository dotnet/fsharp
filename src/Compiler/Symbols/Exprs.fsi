// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec FSharp.Compiler.Symbols

open FSharp.Compiler.CompilerImports
open FSharp.Compiler.Syntax
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree

/// Represents the definitional contents of an assembly, as seen by the F# language
type public FSharpAssemblyContents =

    internal new:
        tcGlobals: TcGlobals *
        thisCcu: CcuThunk *
        thisCcuType: ModuleOrNamespaceType option *
        tcImports: TcImports *
        mimpls: CheckedImplFile list ->
            FSharpAssemblyContents

    /// The contents of the implementation files in the assembly
    member ImplementationFiles: FSharpImplementationFileContents list

/// Represents the definitional contents of a single file or fragment in an assembly, as seen by the F# language
type public FSharpImplementationFileContents =
    internal new: cenv: SymbolEnv * mimpl: CheckedImplFile -> FSharpImplementationFileContents

    /// The qualified name acts to fully-qualify module specifications and implementations
    member QualifiedName: string

    /// Get the system path of the implementation file
    member FileName: string

    /// Get the declarations that make up this implementation file
    member Declarations: FSharpImplementationFileDeclaration list

    /// Indicates if the implementation file is a script
    member IsScript: bool

    /// Indicates if the implementation file has an explicit entry point
    member HasExplicitEntryPoint: bool

/// Represents a declaration in an implementation file, as seen by the F# language
[<RequireQualifiedAccess>]
type public FSharpImplementationFileDeclaration =

    /// Represents the declaration of a type
    | Entity of entity: FSharpEntity * declarations: FSharpImplementationFileDeclaration list

    /// Represents the declaration of a member, function or value, including the parameters and body of the member
    | MemberOrFunctionOrValue of
        value: FSharpMemberOrFunctionOrValue *
        curriedArgs: FSharpMemberOrFunctionOrValue list list *
        body: FSharpExpr

    /// Represents the declaration of a static initialization action
    | InitAction of action: FSharpExpr

/// Represents a checked and reduced expression, as seen by the F# language.  The active patterns
/// in 'FSharp.Compiler.SourceCodeServices' can be used to analyze information about the expression.
///
/// Pattern matching is reduced to decision trees and conditional tests. Some other
/// constructs may be represented in reduced form.
[<Sealed>]
type public FSharpExpr =
    /// The range of the expression
    member Range: range

    /// The type of the expression
    member Type: FSharpType

    /// The immediate sub-expressions of the expression.
    member ImmediateSubExpressions: FSharpExpr list

/// Represents a checked method in an object expression, as seen by the F# language.
[<Sealed>]
type public FSharpObjectExprOverride =
    /// The signature of the implemented abstract slot
    member Signature: FSharpAbstractSignature

    /// The generic parameters of the method
    member GenericParameters: FSharpGenericParameter list

    /// The parameters of the method
    member CurriedParameterGroups: FSharpMemberOrFunctionOrValue list list

    /// The expression that forms the body of the method
    member Body: FSharpExpr

/// A collection of active patterns to analyze expressions
module public FSharpExprPatterns =

    /// Matches expressions which are uses of values
    [<return: Struct>]
    val (|Value|_|): FSharpExpr -> FSharpMemberOrFunctionOrValue voption

    /// Matches expressions which are the application of function values
    [<return: Struct>]
    val (|Application|_|): FSharpExpr -> (FSharpExpr * FSharpType list * FSharpExpr list) voption

    /// Matches expressions which are type abstractions
    [<return: Struct>]
    val (|TypeLambda|_|): FSharpExpr -> (FSharpGenericParameter list * FSharpExpr) voption

    /// Matches expressions with a decision expression, each branch of which ends in DecisionTreeSuccess passing control and values to one of the targets.
    [<return: Struct>]
    val (|DecisionTree|_|): FSharpExpr -> (FSharpExpr * (FSharpMemberOrFunctionOrValue list * FSharpExpr) list) voption

    /// Special expressions at the end of a conditional decision structure in the decision expression node of a DecisionTree .
    /// The given expressions are passed as values to the decision tree target.
    [<return: Struct>]
    val (|DecisionTreeSuccess|_|): FSharpExpr -> (int * FSharpExpr list) voption

    /// Matches expressions which are lambda abstractions
    [<return: Struct>]
    val (|Lambda|_|): FSharpExpr -> (FSharpMemberOrFunctionOrValue * FSharpExpr) voption

    /// Matches expressions which are conditionals
    [<return: Struct>]
    val (|IfThenElse|_|): FSharpExpr -> (FSharpExpr * FSharpExpr * FSharpExpr) voption

    /// Matches expressions which are let definitions
    [<return: Struct>]
    val (|Let|_|):
        FSharpExpr -> ((FSharpMemberOrFunctionOrValue * FSharpExpr * DebugPointAtBinding) * FSharpExpr) voption

    /// Matches expressions which are calls to members or module-defined functions. When calling curried functions and members the
    /// arguments are collapsed to a single collection of arguments, as done in the compiled version of these.
    [<return: Struct>]
    val (|Call|_|):
        FSharpExpr ->
            (FSharpExpr option * FSharpMemberOrFunctionOrValue * FSharpType list * FSharpType list * FSharpExpr list) voption

    /// Like Call but also indicates witness arguments
    [<return: Struct>]
    val (|CallWithWitnesses|_|):
        FSharpExpr ->
            (FSharpExpr option *
            FSharpMemberOrFunctionOrValue *
            FSharpType list *
            FSharpType list *
            FSharpExpr list *
            FSharpExpr list) voption

    /// Matches expressions which are calls to object constructors
    [<return: Struct>]
    val (|NewObject|_|): FSharpExpr -> (FSharpMemberOrFunctionOrValue * FSharpType list * FSharpExpr list) voption

    /// Matches expressions which are uses of the 'this' value
    [<return: Struct>]
    val (|ThisValue|_|): FSharpExpr -> FSharpType voption

    /// Matches expressions which are uses of the 'base' value
    [<return: Struct>]
    val (|BaseValue|_|): FSharpExpr -> FSharpType voption

    /// Matches expressions which are quotation literals
    [<return: Struct>]
    val (|Quote|_|): FSharpExpr -> FSharpExpr voption

    /// Matches expressions which are let-rec definitions
    [<return: Struct>]
    val (|LetRec|_|):
        FSharpExpr -> ((FSharpMemberOrFunctionOrValue * FSharpExpr * DebugPointAtBinding) list * FSharpExpr) voption

    /// Matches record expressions
    [<return: Struct>]
    val (|NewRecord|_|): FSharpExpr -> (FSharpType * FSharpExpr list) voption

    /// Matches anonymous record expressions
    [<return: Struct>]
    val (|NewAnonRecord|_|): FSharpExpr -> (FSharpType * FSharpExpr list) voption

    /// Matches expressions getting a field from an anonymous record. The integer represents the
    /// index into the sorted fields of the anonymous record.
    [<return: Struct>]
    val (|AnonRecordGet|_|): FSharpExpr -> (FSharpExpr * FSharpType * int) voption

    /// Matches expressions which get a field from a record or class
    [<return: Struct>]
    val (|FSharpFieldGet|_|): FSharpExpr -> (FSharpExpr option * FSharpType * FSharpField) voption

    /// Matches expressions which set a field in a record or class
    [<return: Struct>]
    val (|FSharpFieldSet|_|): FSharpExpr -> (FSharpExpr option * FSharpType * FSharpField * FSharpExpr) voption

    /// Matches expressions which create an object corresponding to a union case
    [<return: Struct>]
    val (|NewUnionCase|_|): FSharpExpr -> (FSharpType * FSharpUnionCase * FSharpExpr list) voption

    /// Matches expressions which get a field from a union case
    [<return: Struct>]
    val (|UnionCaseGet|_|): FSharpExpr -> (FSharpExpr * FSharpType * FSharpUnionCase * FSharpField) voption

    /// Matches expressions which set a field from a union case (only used in FSharp.Core itself)
    [<return: Struct>]
    val (|UnionCaseSet|_|): FSharpExpr -> (FSharpExpr * FSharpType * FSharpUnionCase * FSharpField * FSharpExpr) voption

    /// Matches expressions which gets the tag for a union case
    [<return: Struct>]
    val (|UnionCaseTag|_|): FSharpExpr -> (FSharpExpr * FSharpType) voption

    /// Matches expressions which test if an expression corresponds to a particular union case
    [<return: Struct>]
    val (|UnionCaseTest|_|): FSharpExpr -> (FSharpExpr * FSharpType * FSharpUnionCase) voption

    /// Matches tuple expressions
    [<return: Struct>]
    val (|NewTuple|_|): FSharpExpr -> (FSharpType * FSharpExpr list) voption

    /// Matches expressions which get a value from a tuple
    [<return: Struct>]
    val (|TupleGet|_|): FSharpExpr -> (FSharpType * int * FSharpExpr) voption

    /// Matches expressions which coerce the type of a value
    [<return: Struct>]
    val (|Coerce|_|): FSharpExpr -> (FSharpType * FSharpExpr) voption

    /// Matches array expressions
    [<return: Struct>]
    val (|NewArray|_|): FSharpExpr -> (FSharpType * FSharpExpr list) voption

    /// Matches expressions which test the runtime type of a value
    [<return: Struct>]
    val (|TypeTest|_|): FSharpExpr -> (FSharpType * FSharpExpr) voption

    /// Matches expressions which set the contents of an address
    [<return: Struct>]
    val (|AddressSet|_|): FSharpExpr -> (FSharpExpr * FSharpExpr) voption

    /// Matches expressions which set the contents of a mutable variable
    [<return: Struct>]
    val (|ValueSet|_|): FSharpExpr -> (FSharpMemberOrFunctionOrValue * FSharpExpr) voption

    /// Matches default-value expressions, including null expressions
    [<return: Struct>]
    val (|DefaultValue|_|): FSharpExpr -> FSharpType voption

    /// Matches constant expressions, including signed and unsigned integers, strings, characters, booleans, arrays
    /// of bytes and arrays of unit16.
    [<return: Struct>]
    val (|Const|_|): FSharpExpr -> (obj * FSharpType) voption

    /// Matches expressions which take the address of a location
    [<return: Struct>]
    val (|AddressOf|_|): FSharpExpr -> FSharpExpr voption

    /// Matches sequential expressions
    [<return: Struct>]
    val (|Sequential|_|): FSharpExpr -> (FSharpExpr * FSharpExpr) voption

    /// Matches debug points at leaf expressions in control flow
    [<return: Struct>]
    val (|DebugPoint|_|): FSharpExpr -> (DebugPointAtLeafExpr * FSharpExpr) voption

    /// Matches fast-integer loops (up or down)
    [<return: Struct>]
    val (|FastIntegerForLoop|_|):
        FSharpExpr -> (FSharpExpr * FSharpExpr * FSharpExpr * bool * DebugPointAtFor * DebugPointAtInOrTo) voption

    /// Matches while loops
    [<return: Struct>]
    val (|WhileLoop|_|): FSharpExpr -> (FSharpExpr * FSharpExpr * DebugPointAtWhile) voption

    /// Matches try/finally expressions
    [<return: Struct>]
    val (|TryFinally|_|): FSharpExpr -> (FSharpExpr * FSharpExpr * DebugPointAtTry * DebugPointAtFinally) voption

    /// Matches try/with expressions
    [<return: Struct>]
    val (|TryWith|_|):
        FSharpExpr ->
            (FSharpExpr *
            FSharpMemberOrFunctionOrValue *
            FSharpExpr *
            FSharpMemberOrFunctionOrValue *
            FSharpExpr *
            DebugPointAtTry *
            DebugPointAtWith) voption

    /// Matches expressions which create an instance of a delegate type
    [<return: Struct>]
    val (|NewDelegate|_|): FSharpExpr -> (FSharpType * FSharpExpr) voption

    /// Matches expressions which are IL assembly code
    [<return: Struct>]
    val (|ILAsm|_|): FSharpExpr -> (string * FSharpType list * FSharpExpr list) voption

    /// Matches expressions which fetch a field from a .NET type
    [<return: Struct>]
    val (|ILFieldGet|_|): FSharpExpr -> (FSharpExpr option * FSharpType * string) voption

    /// Matches expressions which set a field in a .NET type
    [<return: Struct>]
    val (|ILFieldSet|_|): FSharpExpr -> (FSharpExpr option * FSharpType * string * FSharpExpr) voption

    /// Matches object expressions, returning the base type, the base call, the overrides and the interface implementations
    [<return: Struct>]
    val (|ObjectExpr|_|):
        FSharpExpr ->
            (FSharpType * FSharpExpr * FSharpObjectExprOverride list * (FSharpType * FSharpObjectExprOverride list) list) voption

    /// Matches expressions for an unresolved call to a trait
    [<return: Struct>]
    val (|TraitCall|_|):
        FSharpExpr ->
            (FSharpType list * string * SynMemberFlags * FSharpType list * FSharpType list * FSharpExpr list) voption

    /// Indicates a witness argument index from the witness arguments supplied to the enclosing method
    [<return: Struct>]
    val (|WitnessArg|_|): FSharpExpr -> int voption
