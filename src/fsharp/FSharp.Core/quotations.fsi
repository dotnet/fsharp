// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>Types and functions related to expression quotations</summary>
namespace Microsoft.FSharp.Quotations

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Reflection
open System
open System.Reflection

/// <summary>Information at the binding site of a variable</summary>
[<Sealed>]
[<CompiledName("FSharpVar")>]
type Var =
    /// <summary>The type associated with the variable</summary>
    member Type : Type

    /// <summary>The declared name of the variable</summary>
    member Name : string

    /// <summary>Indicates if the variable represents a mutable storage location</summary>
    member IsMutable: bool

    /// <summary>Creates a new variable with the given name, type and mutability</summary>
    /// <param name="name">The declared name of the variable.</param>
    /// <param name="typ">The type associated with the variable.</param>
    /// <param name="isMutable">Indicates if the variable represents a mutable storage location. Default is false.</param>
    /// <returns>The created variable.</returns>
    new : name:string * typ:Type * ?isMutable : bool -> Var

    /// <summary>Fetches or create a new variable with the given name and type from a global pool of shared variables
    /// indexed by name and type</summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="typ">The type associated with the variable.</param>
    /// <returns>The retrieved or created variable.</returns>
    static member Global : name:string * typ:Type -> Var
    
    interface System.IComparable

/// <summary>Quoted expressions annotated with System.Type values. </summary>
[<CompiledName("FSharpExpr")>]
[<Class>]
type Expr =
    
    /// <summary>Substitutes through the given expression using the given functions
    /// to map variables to new values. The functions must give consistent results
    /// at each application. Variable renaming may occur on the target expression
    /// if variable capture occurs.</summary>
    /// <param name="substitution">The function to map variables into expressions.</param>
    /// <returns>The expression with the given substitutions.</returns>
    member Substitute : substitution:(Var -> Expr option) -> Expr 

    /// <summary>Gets the free expression variables of an expression as a list.</summary>
    /// <returns>A sequence of the free variables in the expression.</returns>
    member GetFreeVars : unit -> seq<Var>

    /// <summary>Returns type of an expression.</summary>
    member Type : Type

    /// <summary>Returns the custom attributes of an expression.</summary>
    member CustomAttributes : Expr list

    override Equals : obj:obj -> bool 
    
    /// <summary>Builds an expression that represents getting the address of a value.</summary>
    /// <param name="target">The target expression.</param>
    /// <returns>The resulting expression.</returns>
    static member AddressOf : target:Expr -> Expr
    
    /// <summary>Builds an expression that represents setting the value held at a particular address.</summary>
    /// <param name="target">The target expression.</param>
    /// <param name="value">The value to set at the address.</param>
    /// <returns>The resulting expression.</returns>
    static member AddressSet : target:Expr * value:Expr -> Expr
    
    /// <summary>Builds an expression that represents the application of a first class function value to a single argument.</summary>
    /// <param name="functionExpr">The function to apply.</param>
    /// <param name="argument">The argument to the function.</param>
    /// <returns>The resulting expression.</returns>
    static member Application: functionExpr:Expr * argument:Expr -> Expr
    
    /// <summary>Builds an expression that represents the application of a first class function value to multiple arguments</summary>
    /// <param name="functionExpr">The function to apply.</param>
    /// <param name="arguments">The list of lists of arguments to the function.</param>
    /// <returns>The resulting expression.</returns>
    static member Applications: functionExpr:Expr * arguments:list<list<Expr>> -> Expr
    
    /// <summary>Builds an expression that represents a call to an static method or module-bound function</summary>
    /// <param name="methodInfo">The MethodInfo describing the method to call.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    /// <returns>The resulting expression.</returns>
    static member Call : methodInfo:MethodInfo * arguments:list<Expr> -> Expr

    /// <summary>Builds an expression that represents a call to an instance method associated with an object</summary>
    /// <param name="obj">The input object.</param>
    /// <param name="methodInfo">The description of the method to call.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    /// <returns>The resulting expression.</returns>
    static member Call : obj:Expr * methodInfo:MethodInfo * arguments:list<Expr> -> Expr

    /// <summary>Builds an expression that represents a call to an static method or module-bound function</summary>
    /// <param name="methodInfo">The MethodInfo describing the method to call.</param>
    /// <param name="methodInfoWithWitnesses">The additional MethodInfo describing the method to call, accepting witnesses.</param>
    /// <param name="witnesses">The list of witnesses to the method.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    /// <returns>The resulting expression.</returns>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    static member CallWithWitnesses: methodInfo: MethodInfo * methodInfoWithWitnesses: MethodInfo * witnesses: Expr list * arguments: Expr list -> Expr

    /// <summary>Builds an expression that represents a call to an instance method associated with an object</summary>
    /// <param name="obj">The input object.</param>
    /// <param name="methodInfo">The description of the method to call.</param>
    /// <param name="methodInfoWithWitnesses">The additional MethodInfo describing the method to call, accepting witnesses.</param>
    /// <param name="witnesses">The list of witnesses to the method.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    /// <returns>The resulting expression.</returns>
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    static member CallWithWitnesses: obj:Expr * methodInfo:MethodInfo * methodInfoWithWitnesses: MethodInfo * witnesses: Expr list * arguments:Expr list -> Expr

    /// <summary>Builds an expression that represents the coercion of an expression to a type</summary>
    /// <param name="source">The expression to coerce.</param>
    /// <param name="target">The target type.</param>
    /// <returns>The resulting expression.</returns>
    static member Coerce : source:Expr * target:Type -> Expr 

    /// <summary>Builds 'if ... then ... else' expressions.</summary>
    /// <param name="guard">The condition expression.</param>
    /// <param name="thenExpr">The <c>then</c> sub-expression.</param>
    /// <param name="elseExpr">The <c>else</c> sub-expression.</param>
    /// <returns>The resulting expression.</returns>
    static member IfThenElse : guard:Expr * thenExpr:Expr * elseExpr:Expr -> Expr 

    /// <summary>Builds a 'for i = ... to ... do ...' expression that represent loops over integer ranges</summary>
    /// <param name="loopVariable">The sub-expression declaring the loop variable.</param>
    /// <param name="start">The sub-expression setting the initial value of the loop variable.</param>
    /// <param name="endExpr">The sub-expression declaring the final value of the loop variable.</param>
    /// <param name="body">The sub-expression representing the body of the loop.</param>
    /// <returns>The resulting expression.</returns>
    static member ForIntegerRangeLoop: loopVariable:Var * start:Expr * endExpr:Expr * body:Expr -> Expr 

    /// <summary>Builds an expression that represents the access of a static field</summary>
    /// <param name="fieldInfo">The description of the field to access.</param>
    /// <returns>The resulting expression.</returns>
    static member FieldGet: fieldInfo:FieldInfo -> Expr 

    /// <summary>Builds an expression that represents the access of a field of an object</summary>
    /// <param name="obj">The input object.</param>
    /// <param name="fieldInfo">The description of the field to access.</param>
    /// <returns>The resulting expression.</returns>
    static member FieldGet: obj:Expr * fieldInfo:FieldInfo -> Expr 

    /// <summary>Builds an expression that represents writing to a static field </summary>
    /// <param name="fieldInfo">The description of the field to write to.</param>
    /// <param name="value">The value to the set to the field.</param>
    /// <returns>The resulting expression.</returns>
    static member FieldSet: fieldInfo:FieldInfo * value:Expr -> Expr 

    /// <summary>Builds an expression that represents writing to a field of an object</summary>
    /// <param name="obj">The input object.</param>
    /// <param name="fieldInfo">The description of the field to write to.</param>
    /// <param name="value">The value to set to the field.</param>
    /// <returns>The resulting expression.</returns>
    static member FieldSet: obj:Expr * fieldInfo:FieldInfo * value:Expr -> Expr 

    /// <summary>Builds an expression that represents the construction of an F# function value</summary>
    /// <param name="parameter">The parameter to the function.</param>
    /// <param name="body">The body of the function.</param>
    /// <returns>The resulting expression.</returns>
    static member Lambda : parameter:Var * body:Expr -> Expr

    /// <summary>Builds expressions associated with 'let' constructs</summary>
    /// <param name="letVariable">The variable in the let expression.</param>
    /// <param name="letExpr">The expression bound to the variable.</param>
    /// <param name="body">The sub-expression where the binding is in scope.</param>
    /// <returns>The resulting expression.</returns>
    static member Let : letVariable:Var * letExpr:Expr * body:Expr -> Expr 

    /// <summary>Builds recursive expressions associated with 'let rec' constructs</summary>
    /// <param name="bindings">The list of bindings for the let expression.</param>
    /// <param name="body">The sub-expression where the bindings are in scope.</param>
    /// <returns>The resulting expression.</returns>
    static member LetRecursive : bindings:(Var * Expr) list * body:Expr -> Expr 

    /// <summary>Builds an expression that represents the invocation of an object constructor</summary>
    /// <param name="constructorInfo">The description of the constructor.</param>
    /// <param name="arguments">The list of arguments to the constructor.</param>
    /// <returns>The resulting expression.</returns>
    static member NewObject: constructorInfo:ConstructorInfo * arguments:Expr list -> Expr 


    /// <summary>Builds an expression that represents the invocation of a default object constructor</summary>
    /// <param name="expressionType">The type on which the constructor is invoked.</param>
    /// <returns>The resulting expression.</returns>
    static member DefaultValue: expressionType:Type -> Expr 


    /// <summary>Builds an expression that represents the creation of an F# tuple value</summary>
    /// <param name="elements">The list of elements of the tuple.</param>
    /// <returns>The resulting expression.</returns>
    static member NewTuple: elements:Expr list -> Expr 

    /// <summary>Builds an expression that represents the creation of an F# tuple value</summary>
    /// <param name="asm">Runtime assembly containing System.ValueTuple definitions.</param>
    /// <param name="elements">The list of elements of the tuple.</param>
    /// <returns>The resulting expression.</returns>
    static member NewStructTuple: asm:Assembly * elements:Expr list -> Expr 

    /// <summary>Builds record-construction expressions </summary>
    /// <param name="recordType">The type of record.</param>
    /// <param name="elements">The list of elements of the record.</param>
    /// <returns>The resulting expression.</returns>
    static member NewRecord: recordType:Type * elements:Expr list -> Expr 

    /// <summary>Builds an expression that represents the creation of an array value initialized with the given elements</summary>
    /// <param name="elementType">The type for the elements of the array.</param>
    /// <param name="elements">The list of elements of the array.</param>
    /// <returns>The resulting expression.</returns>
    static member NewArray: elementType:Type * elements:Expr list -> Expr 

    /// <summary>Builds an expression that represents the creation of a delegate value for the given type</summary>
    /// <param name="delegateType">The type of delegate.</param>
    /// <param name="parameters">The parameters for the delegate.</param>
    /// <param name="body">The body of the function.</param>
    /// <returns>The resulting expression.</returns>
    static member NewDelegate: delegateType:Type * parameters:Var list * body:Expr -> Expr 

    /// <summary>Builds an expression that represents the creation of a union case value</summary>
    /// <param name="unionCase">The description of the union case.</param>
    /// <param name="arguments">The list of arguments for the case.</param>
    /// <returns>The resulting expression.</returns>
    static member NewUnionCase: unionCase:UnionCaseInfo * arguments:Expr list -> Expr 

    /// <summary>Builds an expression that represents reading a property of an object</summary>
    /// <param name="obj">The input object.</param>
    /// <param name="property">The description of the property.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    /// <returns>The resulting expression.</returns>
    static member PropertyGet: obj:Expr * property:PropertyInfo  * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents reading a static property </summary>
    /// <param name="property">The description of the property.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    /// <returns>The resulting expression.</returns>
    static member PropertyGet: property:PropertyInfo * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents writing to a property of an object</summary>
    /// <param name="obj">The input object.</param>
    /// <param name="property">The description of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    /// <returns>The resulting expression.</returns>
    static member PropertySet: obj:Expr * property:PropertyInfo * value:Expr * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents writing to a static property </summary>
    /// <param name="property">The description of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    /// <returns>The resulting expression.</returns>
    static member PropertySet: property:PropertyInfo * value:Expr * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents a nested typed or raw quotation literal</summary>
    /// <param name="inner">The expression being quoted.</param>
    /// <returns>The resulting expression.</returns>
    [<Obsolete("Please use Expr.QuoteTyped or Expr.QuoteRaw to distinguish between typed and raw quotation literals")>]
    static member Quote: inner:Expr -> Expr 

    /// <summary>Builds an expression that represents a nested raw quotation literal</summary>
    /// <param name="inner">The expression being quoted.</param>
    /// <returns>The resulting expression.</returns>
    static member QuoteRaw: inner:Expr -> Expr 

    /// <summary>Builds an expression that represents a nested typed quotation literal</summary>
    /// <param name="inner">The expression being quoted.</param>
    /// <returns>The resulting expression.</returns>
    static member QuoteTyped: inner:Expr -> Expr 

    /// <summary>Builds an expression that represents the sequential execution of one expression followed by another</summary>
    /// <param name="first">The first expression.</param>
    /// <param name="second">The second expression.</param>
    /// <returns>The resulting expression.</returns>
    static member Sequential: first:Expr * second:Expr -> Expr 

    /// <summary>Builds an expression that represents a try/with construct for exception filtering and catching.</summary>
    /// <param name="body">The body of the try expression.</param>
    /// <param name="filterVar"></param>
    /// <param name="filterBody"></param>
    /// <param name="catchVar">The variable to bind to a caught exception.</param>
    /// <param name="catchBody">The expression evaluated when an exception is caught.</param>
    /// <returns>The resulting expression.</returns>
    static member TryWith: body:Expr * filterVar:Var * filterBody:Expr * catchVar:Var * catchBody:Expr -> Expr 

    /// <summary>Builds an expression that represents a try/finally construct </summary>
    /// <param name="body">The body of the try expression.</param>
    /// <param name="compensation">The final part of the expression to be evaluated.</param>
    /// <returns>The resulting expression.</returns>
    static member TryFinally: body:Expr * compensation:Expr -> Expr 

    /// <summary>Builds an expression that represents getting a field of a tuple</summary>
    /// <param name="tuple">The input tuple.</param>
    /// <param name="index">The index of the tuple element to get.</param>
    /// <returns>The resulting expression.</returns>
    static member TupleGet: tuple:Expr * index:int -> Expr 


    /// <summary>Builds an expression that represents a type test.</summary>
    /// <param name="source">The expression to test.</param>
    /// <param name="target">The target type.</param>
    /// <returns>The resulting expression.</returns>
    static member TypeTest: source:Expr * target:Type -> Expr 

    /// <summary>Builds an expression that represents a test of a value is of a particular union case</summary>
    /// <param name="source">The expression to test.</param>
    /// <param name="unionCase">The description of the union case.</param>
    /// <returns>The resulting expression.</returns>
    static member UnionCaseTest: source:Expr * unionCase:UnionCaseInfo -> Expr 

    /// <summary>Builds an expression that represents a constant value of a particular type</summary>
    /// <param name="value">The untyped object.</param>
    /// <param name="expressionType">The type of the object.</param>
    /// <returns>The resulting expression.</returns>
    static member Value : value:obj * expressionType:Type -> Expr
    
    /// <summary>Builds an expression that represents a constant value </summary>
    /// <param name="value">The typed value.</param>
    /// <returns>The resulting expression.</returns>
    static member Value : value:'T -> Expr

    /// <summary>Builds an expression that represents a constant value, arising from a variable of the given name </summary>
    /// <param name="value">The typed value.</param>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The resulting expression.</returns>
    static member ValueWithName : value:'T * name: string -> Expr

    /// <summary>Builds an expression that represents a constant value of a particular type, arising from a variable of the given name </summary>
    /// <param name="value">The untyped object.</param>
    /// <param name="expressionType">The type of the object.</param>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The resulting expression.</returns>
    static member ValueWithName : value:obj * expressionType:Type * name: string -> Expr
    
    /// <summary>Builds an expression that represents a value and its associated reflected definition as a quotation</summary>
    /// <param name="value">The value being quoted.</param>
    /// <param name="definition">The definition of the value being quoted.</param>
    /// <returns>The resulting expression.</returns>
    static member WithValue: value: 'T * definition: Expr<'T> -> Expr<'T>


    /// <summary>Builds an expression that represents a value and its associated reflected definition as a quotation</summary>
    /// <param name="value">The untyped object.</param>
    /// <param name="expressionType">The type of the object.</param>
    /// <param name="definition">The definition of the value being quoted.</param>
    /// <returns>The resulting expression.</returns>
    static member WithValue: value: obj * expressionType:Type * definition: Expr -> Expr

    /// <summary>Builds an expression that represents a variable</summary>
    /// <param name="variable">The input variable.</param>
    /// <returns>The resulting expression.</returns>
    static member Var : variable:Var -> Expr
    
    /// <summary>Builds an expression that represents setting a mutable variable</summary>
    /// <param name="variable">The input variable.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The resulting expression.</returns>
    static member VarSet : variable:Var * value:Expr -> Expr
    
    /// <summary>Builds an expression that represents a while loop</summary>
    /// <param name="guard">The predicate to control the loop iteration.</param>
    /// <param name="body">The body of the while loop.</param>
    /// <returns>The resulting expression.</returns>
    static member WhileLoop : guard:Expr * body:Expr -> Expr

    //----------------    


    /// <summary>Returns a new typed expression given an underlying runtime-typed expression.
    /// A type annotation is usually required to use this function, and 
    /// using an incorrect type annotation may result in a later runtime exception.</summary>
    /// <param name="source">The expression to cast.</param>
    /// <returns>The resulting typed expression.</returns>
    static member Cast : source:Expr -> Expr<'T> 

    /// <summary>Try and find a stored reflection definition for the given method. Stored reflection
    /// definitions are added to an F# assembly through the use of the [&lt;ReflectedDefinition&gt;] attribute.</summary>
    /// <param name="methodBase">The description of the method to find.</param>
    /// <returns>The reflection definition or None if a match could not be found.</returns>
    static member TryGetReflectedDefinition : methodBase:MethodBase -> Expr option
    
    /// <summary>This function is called automatically when quotation syntax (&lt;@ @&gt;) and other sources of
    /// quotations are used. </summary>
    /// <param name="qualifyingType">A type in the assembly where the quotation occurs.</param>
    /// <param name="spliceTypes">The spliced types, to replace references to type variables.</param>
    /// <param name="spliceExprs">The spliced expressions to replace references to spliced expressions.</param>
    /// <param name="bytes">The serialized form of the quoted expression.</param>
    /// <returns>The resulting expression.</returns>
    static member Deserialize : qualifyingType:System.Type * spliceTypes:list<System.Type> * spliceExprs:list<Expr> * bytes:byte[] -> Expr
    
    /// <summary>This function is called automatically when quotation syntax (&lt;@ @&gt;) and other sources of
    /// quotations are used. </summary>
    /// <param name="qualifyingType">A type in the assembly where the quotation occurs.</param>
    /// <param name="referencedTypes">The type definitions referenced.</param>
    /// <param name="spliceTypes">The spliced types, to replace references to type variables.</param>
    /// <param name="spliceExprs">The spliced expressions to replace references to spliced expressions.</param>
    /// <param name="bytes">The serialized form of the quoted expression.</param>
    /// <returns>The resulting expression.</returns>
    static member Deserialize40 : qualifyingType:Type * referencedTypes:Type[] * spliceTypes:Type[] * spliceExprs:Expr[] * bytes:byte[] -> Expr
    
    /// <summary>Permits interactive environments such as F# Interactive
    /// to explicitly register new pickled resources that represent persisted 
    /// top level definitions.</summary>
    /// <param name="assembly">The assembly associated with the resource.</param>
    /// <param name="resource">The unique name for the resources being added.</param>
    /// <param name="serializedValue">The serialized resource to register with the environment.</param>
    static member RegisterReflectedDefinitions: assembly:Assembly * resource:string * serializedValue:byte[] -> unit

    /// <summary>Permits interactive environments such as F# Interactive
    /// to explicitly register new pickled resources that represent persisted 
    /// top level definitions.</summary>
    /// <param name="assembly">The assembly associated with the resource.</param>
    /// <param name="resource">The unique name for the resources being added.</param>
    /// <param name="referencedTypes">The type definitions referenced.</param>
    /// <param name="serializedValue">The serialized resource to register with the environment.</param>
    static member RegisterReflectedDefinitions: assembly:Assembly * resource:string * serializedValue:byte[] * referencedTypes:Type[] -> unit

    /// <summary>Fetches or creates a new variable with the given name and type from a global pool of shared variables
    /// indexed by name and type. The type is given by the explicit or inferred type parameter</summary>
    /// <param name="name">The variable name.</param>
    /// <returns>The created of fetched typed global variable.</returns>
    static member GlobalVar<'T> : name:string -> Expr<'T>

    /// <summary>Format the expression as a string</summary>
    /// <param name="full">Indicates if method, property, constructor and type objects should be printed in detail. If false, these are abbreviated to their name.</param>
    /// <returns>The formatted string.</returns>
    member ToString : full: bool -> string


/// <summary>Type-carrying quoted expressions. Expressions are generated either
/// by quotations in source text or programatically</summary>
and [<CompiledName("FSharpExpr`1")>]
    [<Class>]
    Expr<'T> =
        inherit Expr
        /// <summary>Gets the raw expression associated with this type-carrying expression</summary>
        member Raw : Expr


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
/// <summary>Contains a set of primitive F# active patterns to analyze F# expression objects</summary>
module Patterns =
    
    /// <summary>An active pattern to recognize expressions that represent getting the address of a value</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>Expr option</returns>
    [<CompiledName("AddressOfPattern")>]
    val (|AddressOf|_|)       : input:Expr -> Expr option

    /// <summary>An active pattern to recognize expressions that represent setting the value held at an address </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr) option</returns>
    [<CompiledName("AddressSetPattern")>]
    val (|AddressSet|_|)      : input:Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent applications of first class function values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr) option</returns>
    [<CompiledName("ApplicationPattern")>]
    val (|Application|_|)     : input:Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent calls to static and instance methods, and functions defined in modules</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr option * MethodInfo * Expr list) option</returns>
    [<CompiledName("CallPattern")>]
    val (|Call|_|)            : input:Expr -> (Expr option * MethodInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent calls to static and instance methods, and functions defined in modules, including witness arguments</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr option * MethodInfo * MethodInfo * Expr list) option</returns>
    [<CompiledName("CallWithWitnessesPattern")>]
    [<Experimental("Experimental library feature, requires '--langversion:preview'")>]
    val (|CallWithWitnesses|_|) : input:Expr -> (Expr option * MethodInfo * MethodInfo * Expr list * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent coercions from one type to another</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Type) option</returns>
    [<CompiledName("CoercePattern")>]
    val (|Coerce|_|)          : input:Expr -> (Expr * Type) option

    /// <summary>An active pattern to recognize expressions that represent getting a static or instance field </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr option * FieldInfo) option</returns>
    [<CompiledName("FieldGetPattern")>]
    val (|FieldGet|_|)        : input:Expr -> (Expr option * FieldInfo) option

    /// <summary>An active pattern to recognize expressions that represent setting a static or instance field </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr option * FieldInfo * Expr) option</returns>
    [<CompiledName("FieldSetPattern")>]
    val (|FieldSet|_|)        : input:Expr -> (Expr option * FieldInfo * Expr) option

    /// <summary>An active pattern to recognize expressions that represent loops over integer ranges</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Var * Expr * Expr * Expr) option</returns>
    [<CompiledName("ForIntegerRangeLoopPattern")>]
    val (|ForIntegerRangeLoop|_|) : input:Expr -> (Var * Expr * Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent while loops </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr) option</returns>
    [<CompiledName("WhileLoopPattern")>]
    val (|WhileLoop|_|)       : input:Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent conditionals</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr * Expr) option</returns>
    [<CompiledName("IfThenElsePattern")>]
    val (|IfThenElse|_|)      : input:Expr -> (Expr * Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent first class function values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Var * Expr) option</returns>
    [<CompiledName("LambdaPattern")>]
    val (|Lambda|_|)          : input:Expr -> (Var * Expr) option

    /// <summary>An active pattern to recognize expressions that represent let bindings</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Var * Expr * Expr) option</returns>
    [<CompiledName("LetPattern")>]
    val (|Let|_|)             : input:Expr -> (Var * Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent recursive let bindings of one or more variables</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>((Var * Expr) list * Expr) option</returns>
    [<CompiledName("LetRecursivePattern")>]
    val (|LetRecursive|_|)          : input:Expr -> ((Var * Expr) list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent the construction of arrays </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Type * Expr list) option</returns>
    [<CompiledName("NewArrayPattern")>]
    val (|NewArray|_|)        : input:Expr -> (Type * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent invocations of a default constructor of a struct</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>Type option</returns>
    [<CompiledName("DefaultValuePattern")>]
    val (|DefaultValue|_|)    : input:Expr -> Type option

    /// <summary>An active pattern to recognize expressions that represent construction of delegate values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Type * Var list * Expr) option</returns>
    [<CompiledName("NewDelegatePattern")>]
    val (|NewDelegate|_|)     : input:Expr -> (Type * Var list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent invocation of object constructors</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(ConstructorInfo * Expr list) option</returns>
    [<CompiledName("NewObjectPattern")>]
    val (|NewObject|_|)       : input:Expr -> (ConstructorInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of record values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Type * Expr list) option</returns>
    [<CompiledName("NewRecordPattern")>]
    val (|NewRecord|_|)       : input:Expr -> (Type * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of particular union case values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(UnionCaseInfo * Expr list) option</returns>
    [<CompiledName("NewUnionCasePattern")>]
    val (|NewUnionCase|_|)    : input:Expr -> (UnionCaseInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of tuple values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr list) option</returns>
    [<CompiledName("NewTuplePattern")>]
    val (|NewTuple|_|)        : input:Expr -> (Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of struct tuple values</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr list) option</returns>
    [<CompiledName("NewStructTuplePattern")>]
    val (|NewStructTuple|_|)        : input:Expr -> (Expr list) option

    /// <summary>An active pattern to recognize expressions that represent the read of a static or instance property, or a non-function value declared in a module</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr option * PropertyInfo * Expr list) option</returns>
    [<CompiledName("PropertyGetPattern")>]
    val (|PropertyGet|_|)         : input:Expr -> (Expr option * PropertyInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent setting a static or instance property, or a non-function value declared in a module</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr option * PropertyInfo * Expr list * Expr) option</returns>
    [<CompiledName("PropertySetPattern")>]
    val (|PropertySet|_|)         : input:Expr -> (Expr option * PropertyInfo * Expr list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent a nested quotation literal</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>Expr option</returns>
    [<CompiledName("QuotePattern")>]
    [<Obsolete("Please use QuoteTyped or QuoteRaw to distinguish between typed and raw quotation literals")>]
    val (|Quote|_|)           : input:Expr -> Expr option 

    /// <summary>An active pattern to recognize expressions that represent a nested raw quotation literal</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>Expr option</returns>
    [<CompiledName("QuoteRawPattern")>]
    val (|QuoteRaw|_|)           : input:Expr -> Expr option 

    /// <summary>An active pattern to recognize expressions that represent a nested typed quotation literal</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>Expr option</returns>
    [<CompiledName("QuoteTypedPattern")>]
    val (|QuoteTyped|_|)           : input:Expr -> Expr option 

    /// <summary>An active pattern to recognize expressions that represent sequential execution of one expression followed by another</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr) option</returns>
    [<CompiledName("SequentialPattern")>]
    val (|Sequential|_|)      : input:Expr -> (Expr * Expr) option 

    /// <summary>An active pattern to recognize expressions that represent a try/with construct for exception filtering and catching </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Var * Expr * Var * Expr) option</returns>
    [<CompiledName("TryWithPattern")>]
    val (|TryWith|_|)        : input:Expr -> (Expr * Var * Expr * Var * Expr) option 

    /// <summary>An active pattern to recognize expressions that represent a try/finally construct </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The body and handler parts of the try/finally expression</returns>
    [<CompiledName("TryFinallyPattern")>]
    val (|TryFinally|_|)      : input:Expr -> (Expr * Expr) option 

    /// <summary>An active pattern to recognize expressions that represent getting a tuple field</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The expression and tuple field being accessed</returns>
    [<CompiledName("TupleGetPattern")>]
    val (|TupleGet|_|)        : input:Expr -> (Expr * int) option 

    /// <summary>An active pattern to recognize expressions that represent a dynamic type test</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The expression and type being tested</returns>
    [<CompiledName("TypeTestPattern")>]
    val (|TypeTest|_|)        : input:Expr -> (Expr * Type) option 

    /// <summary>An active pattern to recognize expressions that represent a test if a value is of a particular union case</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The expression and union case being tested</returns>
    [<CompiledName("UnionCaseTestPattern")>]
    val (|UnionCaseTest|_|)   : input:Expr -> (Expr * UnionCaseInfo) option 

    /// <summary>An active pattern to recognize expressions that represent a constant value. This also matches expressions matched by ValueWithName.</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The boxed value and its static type</returns>
    [<CompiledName("ValuePattern")>]
    val (|Value|_|)           : input:Expr -> (obj * Type) option

    /// <summary>An active pattern to recognize expressions that represent a constant value</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The boxed value, its static type and its name</returns>
    [<CompiledName("ValueWithNamePattern")>]
    val (|ValueWithName|_|)  : input:Expr -> (obj * Type * string) option

    /// <summary>An active pattern to recognize expressions that are a value with an associated definition</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>The boxed value, its static type and its definition</returns>
    [<CompiledName("WithValuePattern")>]
    val (|WithValue|_|)  : input:Expr -> (obj * Type * Expr) option

    /// <summary>An active pattern to recognize expressions that represent a variable</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>Var option</returns>
    [<CompiledName("VarPattern")>]
    val (|Var|_|)             : input:Expr -> Var option

    /// <summary>An active pattern to recognize expressions that represent setting a mutable variable</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Var * Expr) option</returns>
    [<CompiledName("VarSetPattern")>]
    val (|VarSet|_|)          : input:Expr -> (Var * Expr) option
    
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
/// <summary>Contains a set of derived F# active patterns to analyze F# expression objects</summary>
module DerivedPatterns =    

    /// <summary>An active pattern to recognize expressions that represent a (possibly curried or tupled) first class function value</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Var list list * Expr) option</returns>
    [<CompiledName("LambdasPattern")>]
    val (|Lambdas|_|)       : input:Expr -> (Var list list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent the application of a (possibly curried or tupled) first class function value</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr list list) option</returns>
    [<CompiledName("ApplicationsPattern")>]
    val (|Applications|_|)  : input:Expr -> (Expr * Expr list list) option

    /// <summary>An active pattern to recognize expressions of the form <c>a &amp;&amp; b</c> </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr) option</returns>
    [<CompiledName("AndAlsoPattern")>]
    val (|AndAlso|_|)       : input:Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions of the form <c>a || b</c> </summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>(Expr * Expr) option</returns>
    [<CompiledName("OrElsePattern")>]
    val (|OrElse|_|)        : input:Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize <c>()</c> constant expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>unit option</returns>
    [<CompiledName("UnitPattern")>]
    val (|Unit|_|)          : input:Expr -> unit option 

    /// <summary>An active pattern to recognize constant boolean expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>bool option</returns>
    [<CompiledName("BoolPattern")>]
    val (|Bool|_|)          : input:Expr -> bool option 

    /// <summary>An active pattern to recognize constant string expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>string option</returns>
    [<CompiledName("StringPattern")>]
    val (|String|_|)        : input:Expr -> string option 

    /// <summary>An active pattern to recognize constant 32-bit floating point number expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>float32 option</returns>
    [<CompiledName("SinglePattern")>]
    val (|Single|_|)        : input:Expr -> float32 option 

    /// <summary>An active pattern to recognize constant 64-bit floating point number expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>float option</returns>
    [<CompiledName("DoublePattern")>]
    val (|Double|_|)        : input:Expr -> float option 

    /// <summary>An active pattern to recognize constant unicode character expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>char  option</returns>
    [<CompiledName("CharPattern")>]
    val (|Char|_|)          : input:Expr -> char  option 

    /// <summary>An active pattern to recognize constant signed byte expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>sbyte option</returns>
    [<CompiledName("SBytePattern")>]
    val (|SByte|_|)         : input:Expr -> sbyte option 

    /// <summary>An active pattern to recognize constant byte expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>byte option</returns>
    [<CompiledName("BytePattern")>]
    val (|Byte|_|)          : input:Expr -> byte option 

    /// <summary>An active pattern to recognize constant int16 expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>int16 option</returns>
    [<CompiledName("Int16Pattern")>]
    val (|Int16|_|)         : input:Expr -> int16 option 

    /// <summary>An active pattern to recognize constant unsigned int16 expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>uint16 option</returns>
    [<CompiledName("UInt16Pattern")>]
    val (|UInt16|_|)        : input:Expr -> uint16 option 

    /// <summary>An active pattern to recognize constant int32 expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>int32 option</returns>
    [<CompiledName("Int32Pattern")>]
    val (|Int32|_|)         : input:Expr -> int32 option 

    /// <summary>An active pattern to recognize constant unsigned int32 expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>uint32 option</returns>
    [<CompiledName("UInt32Pattern")>]
    val (|UInt32|_|)        : input:Expr -> uint32 option 

    /// <summary>An active pattern to recognize constant int64 expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>int64 option</returns>
    [<CompiledName("Int64Pattern")>]
    val (|Int64|_|)         : input:Expr -> int64 option 

    /// <summary>An active pattern to recognize constant unsigned int64 expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>uint64 option</returns>
    [<CompiledName("UInt64Pattern")>]
    val (|UInt64|_|)        : input:Expr -> uint64 option 

    /// <summary>An active pattern to recognize constant decimal expressions</summary>
    /// <param name="input">The input expression to match against.</param>
    /// <returns>decimal option</returns>
    [<CompiledName("DecimalPattern")>]
    val (|Decimal|_|)        : input:Expr -> decimal option 

    /// <summary>A parameterized active pattern to recognize calls to a specified function or method.
    /// The returned elements are the optional target object (present if the target is an 
    /// instance method), the generic type instantiation (non-empty if the target is a generic
    /// instantiation), and the arguments to the function or method.</summary>
    /// <param name="templateParameter">The input template expression to specify the method to call.</param>
    /// <returns>The optional target object (present if the target is an 
    /// instance method), the generic type instantiation (non-empty if the target is a generic
    /// instantiation), and the arguments to the function or method.</returns>
    [<CompiledName("SpecificCallPattern")>]
    val (|SpecificCall|_|)  : templateParameter:Expr -> (Expr -> (Expr option * list<Type> * list<Expr>) option)

    /// <summary>An active pattern to recognize methods that have an associated ReflectedDefinition</summary>
    /// <param name="methodBase">The description of the method.</param>
    /// <returns>The expression of the method definition if found, or None.</returns>
    [<CompiledName("MethodWithReflectedDefinitionPattern")>]
    val (|MethodWithReflectedDefinition|_|) : methodBase:MethodBase -> Expr option
    
    /// <summary>An active pattern to recognize property getters or values in modules that have an associated ReflectedDefinition</summary>
    /// <param name="propertyInfo">The description of the property.</param>
    /// <returns>The expression of the method definition if found, or None.</returns>
    [<CompiledName("PropertyGetterWithReflectedDefinitionPattern")>]
    val (|PropertyGetterWithReflectedDefinition|_|) : propertyInfo:PropertyInfo -> Expr option

    /// <summary>An active pattern to recognize property setters that have an associated ReflectedDefinition</summary>
    /// <param name="propertyInfo">The description of the property.</param>
    /// <returns>The expression of the method definition if found, or None.</returns>
    [<CompiledName("PropertySetterWithReflectedDefinitionPattern")>]
    val (|PropertySetterWithReflectedDefinition|_|) : propertyInfo:PropertyInfo -> Expr option

/// <summary>Active patterns for traversing, visiting, rebuilding and transforming expressions in a generic way</summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ExprShape =

    /// <summary>An active pattern that performs a complete decomposition viewing the expression tree as a binding structure</summary>
    /// <param name="input">The input expression.</param>
    /// <returns>The decomposed Var, Lambda, or ConstApp.</returns>
    [<CompiledName("ShapePattern")>]
    val (|ShapeVar|ShapeLambda|ShapeCombination|) : 
            input:Expr -> Choice<Var,                // Var
                                 (Var * Expr),       // Lambda
                                 (obj * list<Expr>)> // ConstApp

    /// <summary>Re-build combination expressions. The first parameter should be an object
    /// returned by the <c>ShapeCombination</c> case of the active pattern in this module.</summary>
    /// <param name="shape">The input shape.</param>
    /// <param name="arguments">The list of arguments.</param>
    /// <returns>The rebuilt expression.</returns>
    val RebuildShapeCombination  : shape:obj * arguments:list<Expr> -> Expr
