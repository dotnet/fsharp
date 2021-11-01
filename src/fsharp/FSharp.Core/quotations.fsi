// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// <summary>Types and functions related to expression quotations</summary>
namespace Microsoft.FSharp.Quotations

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Reflection
open System
open System.Reflection

/// <summary>Information at the binding site of a variable</summary>
///
/// <namespacedoc><summary>
///   Library functionality for F# quotations.
///    See also <a href="https: //docs.microsoft.com/dotnet/fsharp/language-reference/code-quotations">F# Code Quotations</a> in the F# Language Guide.
/// </summary></namespacedoc>
[<Sealed>]
[<CompiledName("FSharpVar")>]
type Var =
    /// <summary>The type associated with the variable</summary>
    /// 
    /// <example id="type-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// match &lt;@ fun v -> v @&gt; with
    /// | Lambda(v, body) -> v.Type
    /// | _ -> failwith "unreachable"
    /// </code>
    /// Evaluates to <c>typeof&lt;int&gt;</c>
    /// </example>
    member Type: Type

    /// <summary>The declared name of the variable</summary>
    /// 
    /// <example id="name-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// match &lt;@ fun v -> v @&gt; with
    /// | Lambda(v, body) -> v.Name
    /// | _ -> failwith "unreachable"
    /// </code>
    /// Evaluates to <c>"v"</c>
    /// </example>
    member Name: string

    /// <summary>Indicates if the variable represents a mutable storage location</summary>
    /// 
    /// <example id="ismutable-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// match &lt;@ fun v -> v @&gt; with
    /// | Lambda(v, body) -> v.IsMutable
    /// | _ -> failwith "unreachable"
    /// </code>
    /// Evaluates to <c>false</c>.
    /// </example>
    member IsMutable: bool

    /// <summary>Creates a new variable with the given name, type and mutability</summary>
    ///
    /// <param name="name">The declared name of the variable.</param>
    /// <param name="typ">The type associated with the variable.</param>
    /// <param name="isMutable">Indicates if the variable represents a mutable storage location. Default is false.</param>
    ///
    /// <returns>The created variable.</returns>
    /// 
    /// <example id="ctor-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let valueVar = Var("value"), typeof&lt;int&gt;)
    /// </code>
    /// Evaluates to a new quotation variable with the given name and type<c></c>.
    /// </example>
    new: name: string * typ: Type * ?isMutable: bool -> Var

    /// <summary>Fetches or create a new variable with the given name and type from a global pool of shared variables
    /// indexed by name and type</summary>
    ///
    /// <param name="name">The name of the variable.</param>
    /// <param name="typ">The type associated with the variable.</param>
    ///
    /// <returns>The retrieved or created variable.</returns>
    /// 
    /// <example id="global-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let valueVar1 = Var.Global("value", typeof&lt;int&gt;)
    /// let valueVar2 = Var.Global("value", typeof&lt;int&gt;)
    /// </code>
    /// Evaluates both to <c>valueVar1</c> and <c>valueVar2</c> to the same variable from a global pool of shared variables.
    /// </example>
    static member Global: name: string * typ: Type -> Var
    
    interface System.IComparable

/// <summary>Quoted expressions annotated with System.Type values. </summary>
[<CompiledName("FSharpExpr")>]
[<Class>]
type Expr =
    
    /// <summary>Substitutes through the given expression using the given functions
    /// to map variables to new values. The functions must give consistent results
    /// at each application. Variable renaming may occur on the target expression
    /// if variable capture occurs.</summary>
    ///
    /// <param name="substitution">The function to map variables into expressions.</param>
    ///
    /// <returns>The expression with the given substitutions.</returns>
    /// 
    /// <example id="substitute-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let sampleQuotation =  &lt;@ fun v -> v * v @&gt;
    ///
    /// let v, body =
    ///     match sampleQuotation with
    ///     | Lambda(v, body) -> (v, body)
    ///     | _ -> failwith "unreachable"
    ///
    /// body.Substitute(function v2 when v = v2 -> Some &lt;@ 1 + 1 @&gt; | _ -> None)
    /// </code>
    /// Evaluates to <c>&lt;@ (1 + 1) * (1 + 1)&gt;</c>.
    /// </example>
    member Substitute: substitution: (Var -> Expr option) -> Expr 

    /// <summary>Gets the free expression variables of an expression as a list.</summary>
    /// <returns>A sequence of the free variables in the expression.</returns>
    /// 
    /// <example id="getfreevars-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let sampleQuotation =  &lt;@ fun v -> v * v @&gt;
    ///
    /// let v, body =
    ///     match sampleQuotation with
    ///     | Lambda(v, body) -> (v, body)
    ///     | _ -> failwith "unreachable"
    ///
    /// body.GetFreeVars()
    /// </code>
    /// Evaluates to a set containing the single variable for <c>v</c>
    /// </example>
    member GetFreeVars: unit -> seq<Var>

    /// <summary>Returns type of an expression.</summary>
    /// 
    /// <example id="type-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let sampleQuotation =  &lt;@ 1 + 1 @&gt;
    ///
    /// sampleQuotation.Type
    /// </code>
    /// Evaluates to <c>typeof&lt;int&gt;</c>.
    /// </example>
    member Type: Type

    /// <summary>Returns the custom attributes of an expression. For quotations deriving from quotation literals this may include the source location of the literal.</summary>
    /// 
    /// <example id="customattributes-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let sampleQuotation =  &lt;@ 1 + 1 @&gt;
    ///
    /// sampleQuotation.CustomAttributes
    /// </code>
    /// Evaluates to a list of expressions containing one custom attribute for the source location of the quotation literal.
    /// </example>
    member CustomAttributes: Expr list

    override Equals: obj: obj -> bool 
    
    /// <summary>Builds an expression that represents getting the address of a value.</summary>
    ///
    /// <param name="target">The target expression.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="addressof-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let array = [| 1; 2; 3 |]
    /// 
    /// Expr.AddressOf(&lt;@ array.[1] @&gt;)
    /// </code>
    /// Evaluates to <c>AddressOf (Call (None, GetArray, [PropertyGet (None, array, []), Value (1)]))</c>.
    /// </example>
    static member AddressOf: target: Expr -> Expr
    
    /// <summary>Builds an expression that represents setting the value held at a particular address.</summary>
    ///
    /// <param name="target">The target expression.</param>
    /// <param name="value">The value to set at the address.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="addresset-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let array = [| 1; 2; 3 |]
    /// 
    /// let addrExpr = Expr.AddressOf(&lt;@ array.[1] @&gt;)
    /// 
    /// Expr.AddressSet(addrExpr, &lt;@ 4 @&gt;)
    /// </code>
    /// Evaluates to <c>AddressSet (AddressOf (Call (None, GetArray, [PropertyGet (None, array, []), Value (1)])), Value (4))</c>.
    /// </example>
    static member AddressSet: target: Expr * value: Expr -> Expr
    
    /// <summary>Builds an expression that represents the application of a first class function value to a single argument.</summary>
    ///
    /// <param name="functionExpr">The function to apply.</param>
    /// <param name="argument">The argument to the function.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="application-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let funcExpr = &lt;@ (fun x -> x + 1) @&gt;
    /// let argExpr = &lt;@ 3 @&gt;
    ///
    /// Expr.Application(funcExpr, argExpr)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ (fun x -> x + 1) 3 @&gt;</c>.
    /// </example>
    static member Application: functionExpr: Expr * argument: Expr -> Expr
    
    /// <summary>Builds an expression that represents the application of a first class function value to multiple arguments</summary>
    ///
    /// <param name="functionExpr">The function to apply.</param>
    /// <param name="arguments">The list of lists of arguments to the function.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="applications-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let funcExpr = &lt;@ (fun (x, y) z -> x + y + z) @&gt;
    /// let curriedArgExprs = [[ &lt;@ 1 @&gt;; &lt;@ 2 @&gt; ]; [ &lt;@ 3 @&gt; ]]
    ///
    /// Expr.Applications(funcExpr, curriedArgExprs)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ (fun (x, y) z -> x + y + z) (1,2) 3 @&gt;</c>.
    /// </example>
    static member Applications: functionExpr: Expr * arguments: list<list<Expr>> -> Expr
    
    /// <summary>Builds an expression that represents a call to an static method or module-bound function</summary>
    ///
    /// <param name="methodInfo">The MethodInfo describing the method to call.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="call-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let methInfo =
    ///     match &lt;@ Console.WriteLine("1") @&gt; with
    ///     | Call(_, mi, _) -> mi
    ///     | _ -> failwith "call expected"
    ///
    /// let argExpr = &lt;@ "Hello World" @&gt;
    ///
    /// Expr.Call(methInfo, [argExpr])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ Console.WriteLine("Hello World") @&gt;</c>.
    /// </example>
    static member Call: methodInfo: MethodInfo * arguments: list<Expr> -> Expr

    /// <summary>Builds an expression that represents a call to an instance method associated with an object</summary>
    ///
    /// <param name="obj">The input object.</param>
    /// <param name="methodInfo">The description of the method to call.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="call-2">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let objExpr, methInfo =
    ///     match &lt;@ Console.Out.WriteLine("1") @&gt; with
    ///     | Call(Some obj, mi, _) -> obj, mi
    ///     | _ -> failwith "call expected"
    ///
    /// let argExpr = &lt;@ "Hello World" @&gt;
    ///
    /// Expr.Call(objExpr, methInfo, [argExpr])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ Console.Out.WriteLine("Hello World") @&gt;</c>.
    /// </example>
    static member Call: obj: Expr * methodInfo: MethodInfo * arguments: list<Expr> -> Expr

    /// <summary>Builds an expression that represents a call to an static method or module-bound function, potentially passing additional witness arguments</summary>
    ///
    /// <param name="methodInfo">The MethodInfo describing the method to call.</param>
    /// <param name="methodInfoWithWitnesses">The additional MethodInfo describing the method to call, accepting witnesses.</param>
    /// <param name="witnesses">The list of witnesses to the method.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="callwithwitnesses-1">In this example, we show how to use a witness to cosntruct an `op_Addition` call for a type that doesn't support addition directly:
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// // Get the entrypoint for inline addition that takes an explicit witness
    /// let addMethInfoG, addMethInfoGW =
    ///     match &lt;@ 1+1 @&gt; with
    ///     | CallWithWitnesses(None, mi, miW, _, _) ->
    ///         mi.GetGenericMethodDefinition(), miW.GetGenericMethodDefinition()
    ///     | _ ->
    ///         failwith "call expected"
    /// 
    /// // Make a non-standard witness for addition for a type C
    ///
    /// type C(value: int) =
    ///     member x.Value = value
    ///
    /// let witnessExpr = &lt;@ (fun (x: C) (y: C) -> C(x.Value + y.Value)) @&gt;
    /// let argExpr1 = &lt;@ C(4) @&gt;
    /// let argExpr2 = &lt;@ C(5) @&gt;
    ///
    /// // Instantiate the generic method at the right type
    ///
    /// let addMethInfo = addMethInfoG.MakeGenericMethod(typeof&lt;C&gt;, typeof&lt;C&gt;, typeof&lt;C&gt;)
    /// let addMethInfoW = addMethInfoGW.MakeGenericMethod(typeof&lt;C&gt;, typeof&lt;C&gt;, typeof&lt;C&gt;)
    ///
    /// Expr.CallWithWitnesses(addMethInfo, addMethInfoW, [witnessExpr], [argExpr1; argExpr2])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ Call (None, op_Addition, [NewObject (C, Value (4)), NewObject (C, Value (5))]) @&gt;</c>.
    /// </example>
    static member CallWithWitnesses: methodInfo: MethodInfo * methodInfoWithWitnesses: MethodInfo * witnesses: Expr list * arguments: Expr list -> Expr

    /// <summary>Builds an expression that represents a call to an instance method associated with an object, potentially passing additional witness arguments</summary>
    ///
    /// <param name="obj">The input object.</param>
    /// <param name="methodInfo">The description of the method to call.</param>
    /// <param name="methodInfoWithWitnesses">The additional MethodInfo describing the method to call, accepting witnesses.</param>
    /// <param name="witnesses">The list of witnesses to the method.</param>
    /// <param name="arguments">The list of arguments to the method.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="callwithwitnesses-2">See examples for Call and CallWithWitnesses</example>
    static member CallWithWitnesses: obj: Expr * methodInfo: MethodInfo * methodInfoWithWitnesses: MethodInfo * witnesses: Expr list * arguments: Expr list -> Expr

    /// <summary>Builds an expression that represents the coercion of an expression to a type</summary>
    ///
    /// <param name="source">The expression to coerce.</param>
    /// <param name="target">The target type.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="coerce-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let expr = &lt;@ box "3" @&gt;
    ///
    /// Expr.Coerce(expr, typeof&lt;string&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ (fun x -> x + 1) 3 @&gt;</c>.
    /// </example>
    static member Coerce: source: Expr * target: Type -> Expr 

    /// <summary>Builds 'if ... then ... else' expressions.</summary>
    ///
    /// <param name="guard">The condition expression.</param>
    /// <param name="thenExpr">The <c>then</c> sub-expression.</param>
    /// <param name="elseExpr">The <c>else</c> sub-expression.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="coerce-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let guardExpr = &lt;@ 1 > 3 @&gt;
    /// let thenExpr = &lt;@ 6 @&gt;
    /// let elseExpr = &lt;@ 7 @&gt;
    ///
    /// Expr.IfThenElse(guardExpr, thenExpr, elseExpr)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ if 1 > 3 then 6 else 7 @&gt;</c>.
    /// </example>
    static member IfThenElse: guard: Expr * thenExpr: Expr * elseExpr: Expr -> Expr 

    /// <summary>Builds a 'for i = ... to ... do ...' expression that represent loops over integer ranges</summary>
    ///
    /// <param name="loopVariable">The sub-expression declaring the loop variable.</param>
    /// <param name="start">The sub-expression setting the initial value of the loop variable.</param>
    /// <param name="endExpr">The sub-expression declaring the final value of the loop variable.</param>
    /// <param name="body">The sub-expression representing the body of the loop.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="coerce-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let loopVariable = Var("x", typeof&lt;int&gt;)
    /// let startExpr = &lt;@ 6 @&gt;
    /// let endExpr = &lt;@ 7 @&gt;
    /// let body = &lt;@ System.Console.WriteLine("hello") @&gt;
    ///
    /// Expr.ForIntegerRangeLoop(loopVariable, startExpr, endExpr, body)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ if 1 > 3 then 6 else 7 @&gt;</c>.
    /// </example>
    static member ForIntegerRangeLoop: loopVariable: Var * start: Expr * endExpr: Expr * body: Expr -> Expr 

    /// <summary>Builds an expression that represents the access of a static field</summary>
    ///
    /// <param name="fieldInfo">The description of the field to access.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="fieldget-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let fieldInfo = typeof&lt;System.DayOfWeek&gt;.GetField("Monday")
    ///
    /// Expr.FieldGet(fieldInfo)
    /// </code>
    /// Evaluates to <c>FieldGet (None, Monday)</c>. Note that for technical reasons the quotation <c>&lt;@ System.DayOfWeek.Monday @&gt;</c> evaluates to a different quotation containing a constant enum value <c>Value (Monday)</c>.
    /// </example>
    static member FieldGet: fieldInfo: FieldInfo -> Expr 

    /// <summary>Builds an expression that represents the access of a field of an object</summary>
    ///
    /// <param name="obj">The input object.</param>
    /// <param name="fieldInfo">The description of the field to access.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="fieldget-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let fieldInfo = typeof&lt;int ref&gt;.GetField("contents@")
    /// let refValue = ref 3
    /// let refExpr = &lt;@ refValue @&gt;
    ///
    /// Expr.FieldGet(refExpr, fieldInfo)
    /// </code>
    /// Evaluates to <c>FieldGet (Some (PropertyGet (None, refValue, [])), contents@)</c>.
    /// Note that for technical reasons the quotation <c>&lt;@ refValue.contents @&gt;</c> evaluates to a different quotation
    /// accessing the contents field via a property.
    /// </example>
    static member FieldGet: obj: Expr * fieldInfo: FieldInfo -> Expr 

    /// <summary>Builds an expression that represents writing to a static field </summary>
    ///
    /// <param name="fieldInfo">The description of the field to write to.</param>
    /// <param name="value">The value to the set to the field.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <remarks>Settable public static fields are rare in F# and .NET libraries, so examples of using this method are uncommon.</remarks>
    static member FieldSet: fieldInfo: FieldInfo * value: Expr -> Expr 

    /// <summary>Builds an expression that represents writing to a field of an object</summary>
    ///
    /// <param name="obj">The input object.</param>
    /// <param name="fieldInfo">The description of the field to write to.</param>
    /// <param name="value">The value to set to the field.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="fieldset-1">Create an expression setting a reference cell via the public backing field:
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let fieldInfo = typeof&lt;int ref&gt;.GetField("contents@")
    /// let refValue = ref 3
    /// let refExpr = &lt;@ refValue @&gt;
    /// let valueExpr = &lt;@ 6 @&gt;
    ///
    /// Expr.FieldSet(refExpr, fieldInfo, valueExpr)
    /// </code>
    /// Evaluates to <c>FieldSet (Some (PropertyGet (None, refValue, [])), contents@, Value (6))</c>.
    /// Note that for technical reasons the quotation <c>&lt;@ refValue.contents &lt;- 6 @&gt;</c> evaluates to a slightly different quotation
    /// accessing the contents field via a property.
    /// </example>
    static member FieldSet: obj: Expr * fieldInfo: FieldInfo * value: Expr -> Expr 

    /// <summary>Builds an expression that represents the construction of an F# function value</summary>
    ///
    /// <param name="parameter">The parameter to the function.</param>
    /// <param name="body">The body of the function.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="lambda-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let vVar = Var("v", typeof&lt;int&gt;)
    /// let vExpr = Expr.Var(vVar)
    ///
    /// Expr.Lambda(vVar, vExpr)
    /// </code>
    /// Evaluates to <c>Lambda (v, v)</c>.
    /// </example>
    static member Lambda: parameter: Var * body: Expr -> Expr

    /// <summary>Builds expressions associated with 'let' constructs</summary>
    ///
    /// <param name="letVariable">The variable in the let expression.</param>
    /// <param name="letExpr">The expression bound to the variable.</param>
    /// <param name="body">The sub-expression where the binding is in scope.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="let-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let vVar = Var("v", typeof&lt;int&gt;)
    /// let rhsExpr = &lt;@ 6 @&gt;
    /// let vExpr = Expr.Var(vVar)
    ///
    /// Expr.Let(vVar, rhsExpr, vExpr)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ let v = 6 in v @&gt;</c>.
    /// </example>
    static member Let: letVariable: Var * letExpr: Expr * body: Expr -> Expr 

    /// <summary>Builds recursive expressions associated with 'let rec' constructs</summary>
    ///
    /// <param name="bindings">The list of bindings for the let expression.</param>
    /// <param name="body">The sub-expression where the bindings are in scope.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// 
    /// <example id="letrecursive-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let fVar = Var("f", typeof&lt;int -&gt; int&gt;)
    /// let gVar = Var("v", typeof&lt;int -&gt; int&gt;)
    /// let fExpr = Expr.Var(fVar)
    /// let gExpr = Expr.Var(gVar)
    /// let fImplExpr = &lt;@ fun x -> (%%gExpr : int -> int) (x - 1) + 1 @&gt;
    /// let gImplExpr = &lt;@ fun x -> if x > 0 then (%%fExpr : int -> int) (x - 1) else 0 @&gt;
    /// let bodyExpr = &lt;@ (%%gExpr : int -> int) 10 @&gt;
    ///
    /// Expr.LetRecursive([(fVar, fImplExpr); (gVar, gImplExpr)], bodyExpr)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ let rec f x = g (x-1) + 1 and g x = if x > 0 then f (x - 1) else 0 in g 10 @&gt;</c>.
    /// </example>
    static member LetRecursive: bindings: (Var * Expr) list * body: Expr -> Expr 

    /// <summary>Builds an expression that represents the invocation of an object constructor</summary>
    ///
    /// <param name="constructorInfo">The description of the constructor.</param>
    /// <param name="arguments">The list of arguments to the constructor.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="newobject-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let ctorInfo =
    ///     match &lt;@ new System.DateTime(100L) @&gt; with
    ///     | NewObject(ci, _) -> ci
    ///     | _ -> failwith "call expected"
    ///
    /// let argExpr = &lt;@ 100000L @&gt;
    ///
    /// Expr.NewObject(ctorInfo, [argExpr])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ NewObject (DateTime, Value (100000L)) @&gt;</c>.
    /// </example>
    static member NewObject: constructorInfo: ConstructorInfo * arguments: Expr list -> Expr 

    /// <summary>Builds an expression that represents the invocation of a default object constructor</summary>
    ///
    /// <param name="expressionType">The type on which the constructor is invoked.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="defaultvalue-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.DefaultValue(typeof&lt;int&gt;)
    /// </code>
    /// Evaluates to the quotation <c>DefaultValue (Int32)</c>.
    /// </example>
    static member DefaultValue: expressionType: Type -> Expr 

    /// <summary>Builds an expression that represents the creation of an F# tuple value</summary>
    ///
    /// <param name="elements">The list of elements of the tuple.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="defaultvalue-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.NewTuple([ &lt;@ 1 @&gt;; &lt;@ "a" @&gt; ])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ (1, "a") @&gt;</c>.
    /// </example>
    static member NewTuple: elements: Expr list -> Expr 

    /// <summary>Builds an expression that represents the creation of an F# tuple value</summary>
    ///
    /// <param name="asm">Runtime assembly containing System.ValueTuple definitions.</param>
    /// <param name="elements">The list of elements of the tuple.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="newstructtuple-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.NewStructTuple(typeof&lt;struct (int * int)&gt;.Assembly, [ &lt;@ 1 @&gt;; &lt;@ "a" @&gt; ])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ struct (1, "a") @&gt;</c>.
    /// </example>
    static member NewStructTuple: asm: Assembly * elements: Expr list -> Expr 

    /// <summary>Builds record-construction expressions </summary>
    ///
    /// <param name="recordType">The type of record.</param>
    /// <param name="elements">The list of elements of the record.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="newrecord-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// type R = { Y: int; X: string }
    ///
    /// Expr.NewRecord(typeof&lt;R&gt;, [ &lt;@ 1 @&gt;; &lt;@ "a" @&gt; ])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ { Y = 1; X = "a" } @&gt;</c>.
    /// </example>
    static member NewRecord: recordType: Type * elements: Expr list -> Expr 

    /// <summary>Builds an expression that represents the creation of an array value initialized with the given elements</summary>
    ///
    /// <param name="elementType">The type for the elements of the array.</param>
    /// <param name="elements">The list of elements of the array.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="newarray-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.NewArray(typeof&lt;int&gt;, [ &lt;@ 1 @&gt;; &lt;@ 2 @&gt; ])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ [| 1; 2 |] @&gt;</c>.
    /// </example>
    static member NewArray: elementType: Type * elements: Expr list -> Expr 

    /// <summary>Builds an expression that represents the creation of a delegate value for the given type</summary>
    ///
    /// <param name="delegateType">The type of delegate.</param>
    /// <param name="parameters">The parameters for the delegate.</param>
    /// <param name="body">The body of the function.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="newdelegate-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    ///
    /// let vVar = Var("v", typeof&lt;int&gt;)
    /// let vExpr = Expr.Var(vVar)
    ///
    /// Expr.NewDelegate(typeof&lt;Func&lt;int,int&gt;&gt;, [vVar], vExpr)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ new System.Func&lt;int, int&gt;(fun v -> v) @&gt;</c>.
    /// </example>
    static member NewDelegate: delegateType: Type * parameters: Var list * body: Expr -> Expr 

    /// <summary>Builds an expression that represents the creation of a union case value</summary>
    ///
    /// <param name="unionCase">The description of the union case.</param>
    /// <param name="arguments">The list of arguments for the case.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="newunioncase-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Reflection
    ///
    /// let ucCons = FSharpType.GetUnionCases(typeof&lt;int list&gt;)[1]
    ///
    /// Expr.NewUnionCase(ucCons, [ &lt;@ 10 @&gt;; &lt;@ [11] @&gt; ])
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ 10 :: [11] @&gt;</c>.
    /// </example>
    static member NewUnionCase: unionCase: UnionCaseInfo * arguments: Expr list -> Expr 

    /// <summary>Builds an expression that represents reading a property of an object</summary>
    ///
    /// <param name="obj">The input object.</param>
    /// <param name="property">The description of the property.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="propertyget-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let propInfo =
    ///     match &lt;@ "a".Length @&gt; with
    ///     | PropertyGet(Some _, pi, _) -> pi
    ///     | _ -> failwith "property get expected"
    ///
    /// let objExpr = &lt;@ "bb" @&gt;
    ///
    /// Expr.PropertyGet(objExpr, propInfo)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ "bb".Length @&gt;</c>.
    /// </example>
    static member PropertyGet: obj: Expr * property: PropertyInfo  * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents reading a static property </summary>
    ///
    /// <param name="property">The description of the property.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="propertyget-2">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let propInfo =
    ///     match &lt;@ Console.Out @&gt; with
    ///     | PropertyGet(None, pi, _) -> pi
    ///     | _ -> failwith "property get expected"
    ///
    /// Expr.PropertyGet(propInfo)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ Console.Out @&gt;</c>.
    /// </example>
    static member PropertyGet: property: PropertyInfo * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents writing to a property of an object</summary>
    ///
    /// <param name="obj">The input object.</param>
    /// <param name="property">The description of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="propertyset-1">
    /// <code lang="fsharp">
    /// open System
    /// open System.Collections.Generic
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let propInfo =
    ///     match &lt;@ (new List&lt;int&gt;()).Capacity @&gt; with
    ///     | PropertyGet(Some _, pi, _) -> pi
    ///     | _ -> failwith "property get expected"
    ///
    /// let objExpr = &lt;@ (new List&lt;int&gt;()) @&gt;
    ///
    /// Expr.PropertySet(objExpr, propInfo, &lt;@ 6 @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ (new List&lt;int&gt;()).Capacity &lt;- 6 @&gt;</c>.
    /// </example>
    static member PropertySet: obj: Expr * property: PropertyInfo * value: Expr * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents writing to a static property </summary>
    ///
    /// <param name="property">The description of the property.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="indexerArgs">List of indices for the property if it is an indexed property.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="propertyset-2">
    /// <code lang="fsharp">
    /// open System
    /// open System.Collections.Generic
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// let propInfo =
    ///     match &lt;@ Console.BackgroundColor &lt;- ConsoleColor.Red  @&gt; with
    ///     | PropertySet(None, pi, _, _) -> pi
    ///     | _ -> failwith "property get expected"
    ///
    /// Expr.PropertySet(propInfo, &lt;@ ConsoleColor.Blue @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ Console.BackgroundColor &lt;- ConsoleColor.Blue @&gt;</c>.
    /// </example>
    static member PropertySet: property: PropertyInfo * value: Expr * ?indexerArgs: Expr list -> Expr 

    /// <summary>Builds an expression that represents a nested typed or raw quotation literal</summary>
    ///
    /// <param name="inner">The expression being quoted.</param>
    ///
    /// <returns>The resulting expression.</returns>
    [<Obsolete("Please use Expr.QuoteTyped or Expr.QuoteRaw to distinguish between typed and raw quotation literals")>]
    static member Quote: inner: Expr -> Expr 

    /// <summary>Builds an expression that represents a nested raw quotation literal</summary>
    ///
    /// <param name="inner">The expression being quoted.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="quoteraw-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.QuoteRaw(&lt;@ 1 @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ &lt;@ 1 @&gt; @&gt;</c>.
    /// </example>
    static member QuoteRaw: inner: Expr -> Expr 

    /// <summary>Builds an expression that represents a nested typed quotation literal</summary>
    ///
    /// <param name="inner">The expression being quoted.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="quotetyped-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.QuoteTyped(&lt;@ 1 @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ &lt;@ 1 @&gt; @&gt;</c>.
    /// </example>
    static member QuoteTyped: inner: Expr -> Expr 

    /// <summary>Builds an expression that represents the sequential execution of one expression followed by another</summary>
    ///
    /// <param name="first">The first expression.</param>
    /// <param name="second">The second expression.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="sequential-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    ///
    /// Expr.Sequential(&lt;@ Console.WriteLine("a") @&gt;, &lt;@ Console.WriteLine("b") @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ Console.WriteLine("a"); Console.WriteLine("b") @&gt;</c>.
    /// </example>
    static member Sequential: first: Expr * second: Expr -> Expr 

    /// <summary>Builds an expression that represents a try/with construct for exception filtering and catching.</summary>
    ///
    /// <param name="body">The body of the try expression.</param>
    /// <param name="filterVar"></param>
    /// <param name="filterBody"></param>
    /// <param name="catchVar">The variable to bind to a caught exception.</param>
    /// <param name="catchBody">The expression evaluated when an exception is caught.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="trywith-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    ///
    /// let exnVar = Var("exn", typeof&lt;exn&gt;)
    ///
    /// Expr.TryWith(&lt;@ 1+1 @&gt;, exnVar, &lt;@ 1 @&gt;, exnVar, &lt;@ 2+2 @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ try 1+1 with exn -> 2+2 @&gt;</c>.
    /// </example>
    static member TryWith: body: Expr * filterVar: Var * filterBody: Expr * catchVar: Var * catchBody: Expr -> Expr 

    /// <summary>Builds an expression that represents a try/finally construct </summary>
    ///
    /// <param name="body">The body of the try expression.</param>
    /// <param name="compensation">The final part of the expression to be evaluated.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="tryfinally-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    ///
    /// Expr.TryFinally(&lt;@ 1+1 @&gt;, &lt;@ Console.WriteLine("finally") @&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ try 1+1 finally Console.WriteLine("finally") @&gt;</c>.
    /// </example>
    static member TryFinally: body: Expr * compensation: Expr -> Expr 

    /// <summary>Builds an expression that represents getting a field of a tuple</summary>
    ///
    /// <param name="tuple">The input tuple.</param>
    /// <param name="index">The index of the tuple element to get.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="tupleget-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let tupExpr = &lt;@ (1, 2, 3) @&gt;
    ///
    /// Expr.TupleGet(tupExpr, 1)
    /// </code>
    /// Evaluates to quotation that displays as <c>TupleGet (NewTuple (Value (1), Value (2), Value (3)), 1)</c>.
    /// </example>
    static member TupleGet: tuple: Expr * index: int -> Expr 

    /// <summary>Builds an expression that represents a type test.</summary>
    ///
    /// <param name="source">The expression to test.</param>
    /// <param name="target">The target type.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="typetest-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let obj = box 1
    ///
    /// Expr.TypeTest( &lt;@ obj @&gt;, typeof&lt;int&gt;)
    /// </code>
    /// Evaluates to quotation that displays as <c>TypeTest (Int32, PropertyGet (None, obj, []))</c>.
    /// </example>
    static member TypeTest: source: Expr * target: Type -> Expr 

    /// <summary>Builds an expression that represents a test of a value is of a particular union case</summary>
    ///
    /// <param name="source">The expression to test.</param>
    /// <param name="unionCase">The description of the union case.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="unioncasetest-1">
    /// <code lang="fsharp">
    /// open System
    /// open FSharp.Quotations
    /// open FSharp.Reflection
    ///
    /// let ucCons = FSharpType.GetUnionCases(typeof&lt;int list&gt;)[1]
    ///
    /// Expr.UnionCaseTest(&lt;@ [11] @&gt;, ucCons)
    /// </code>
    /// Evaluates to a quotation that displays as <c>UnionCaseTest (NewUnionCase (Cons, Value (11), NewUnionCase (Empty)), Cons)</c>.
    /// </example>
    static member UnionCaseTest: source: Expr * unionCase: UnionCaseInfo -> Expr 

    /// <summary>Builds an expression that represents a constant value of a particular type</summary>
    ///
    /// <param name="value">The untyped object.</param>
    /// <param name="expressionType">The type of the object.</param>
    ///
    /// <returns>The resulting expression.</returns>
    ///
    /// <example id="value-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.Value(box 1, typeof&lt;int&gt;)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ 1 @&gt;</c>.
    /// </example>
    static member Value: value: obj * expressionType: Type -> Expr
    
    /// <summary>Builds an expression that represents a constant value </summary>
    ///
    /// <param name="value">The typed value.</param>
    ///
    /// <example id="value-2">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.Value(1)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ 1 @&gt;</c>.
    /// </example>
    static member Value: value: 'T -> Expr

    /// <summary>Builds an expression that represents a constant value, arising from a variable of the given name </summary>
    ///
    /// <param name="value">The typed value.</param>
    /// <param name="name">The name of the variable.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="valuewithname-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.ValueWithName(1, "name")
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ 1 @&gt;</c> and associated information that the name of the value is <c>"name"</c>.
    /// </example>
    static member ValueWithName: value: 'T * name: string -> Expr

    /// <summary>Builds an expression that represents a constant value of a particular type, arising from a variable of the given name </summary>
    ///
    /// <param name="value">The untyped object.</param>
    /// <param name="expressionType">The type of the object.</param>
    /// <param name="name">The name of the variable.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="valuewithname-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.ValueWithName(box 1, typeof&lt;int&gt;, "name")
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ 1 @&gt;</c> and associated information that the name of the value is <c>"name"</c>.
    /// </example>
    static member ValueWithName: value: obj * expressionType: Type * name: string -> Expr
    
    /// <summary>Builds an expression that represents a value and its associated reflected definition as a quotation</summary>
    ///
    /// <param name="value">The value being quoted.</param>
    /// <param name="definition">The definition of the value being quoted.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="withvalue-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.WithValue(1, &lt;@ 2 - 1 @&gt;)
    /// </code>
    /// Evaluates to a quotation that displays as <c>WithValue (1, Call (None, op_Subtraction, [Value (2), Value (1)]))</c>.
    /// </example>
    static member WithValue: value: 'T * definition: Expr<'T> -> Expr<'T>

    /// <summary>Builds an expression that represents a value and its associated reflected definition as a quotation</summary>
    ///
    /// <param name="value">The untyped object.</param>
    /// <param name="expressionType">The type of the object.</param>
    /// <param name="definition">The definition of the value being quoted.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="withvalue-2">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// Expr.WithValue(box 1, typeof&lt;int&gt;, &lt;@ 2 - 1 @&gt;)
    /// </code>
    /// Evaluates to a quotation that displays as <c>WithValue (1, Call (None, op_Subtraction, [Value (2), Value (1)]))</c>.
    /// </example>
    static member WithValue: value: obj * expressionType: Type * definition: Expr -> Expr

    /// <summary>Builds an expression that represents a variable</summary>
    ///
    /// <param name="variable">The input variable.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="var-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let vVar = Var("v", typeof&lt;int&gt;)
    ///
    /// Expr.Var(vVar)
    /// </code>
    /// Evaluates to a quotation displayed as <c>v</c>.
    /// </example>
    static member Var: variable: Var -> Expr
    
    /// <summary>Builds an expression that represents setting a mutable variable</summary>
    ///
    /// <param name="variable">The input variable.</param>
    /// <param name="value">The value to set.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="varset-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let vVar = Var("v", typeof&lt;int&gt;, isMutable=true)
    ///
    /// Expr.VarSet(vVar, &lt;@ 5 @&gt;)
    /// </code>
    /// Evaluates to a quotation displayed as <c>VarSet (v, Value (5))</c>.
    /// </example>
    static member VarSet: variable: Var * value: Expr -> Expr
    
    /// <summary>Builds an expression that represents a while loop</summary>
    ///
    /// <param name="guard">The predicate to control the loop iteration.</param>
    /// <param name="body">The body of the while loop.</param>
    ///
    /// <returns>The resulting expression.</returns>
    /// 
    /// <example id="varset-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let guardExpr = &lt;@ true @&gt;
    /// let bodyExpr = &lt;@ () @&gt;
    ///
    /// Expr.WhileLoop(guardExpr, bodyExpr)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ while true do () @&gt;</c>.
    /// </example>
    static member WhileLoop: guard: Expr * body: Expr -> Expr

    /// <summary>Returns a new typed expression given an underlying runtime-typed expression.
    /// A type annotation is usually required to use this function, and 
    /// using an incorrect type annotation may result in a later runtime exception.</summary>
    ///
    /// <param name="source">The expression to cast.</param>
    ///
    /// <returns>The resulting typed expression.</returns>
    /// 
    /// <example id="cast-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let rawExpr = &lt;@ 1 @&gt;
    ///
    /// Expr.Cast&lt;int&gt;(rawExpr)
    /// </code>
    /// Evaluates with type <c>Expr&lt;int&gt;</c>.
    /// </example>
    static member Cast: source: Expr -> Expr<'T> 

    /// <summary>Try and find a stored reflection definition for the given method. Stored reflection
    /// definitions are added to an F# assembly through the use of the [&lt;ReflectedDefinition&gt;] attribute.</summary>
    ///
    /// <param name="methodBase">The description of the method to find.</param>
    ///
    /// <returns>The reflection definition or None if a match could not be found.</returns>
    /// 
    /// <example id="trygetreflecteddefinition-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// [&lt;ReflectedDefinition&gt;]
    /// let f x = x + 1
    ///
    /// let methInfo =
    ///     match &lt;@ f 1 @&gt; with
    ///     | Call(_, mi, _) -> mi
    ///     | _ -> failwith "call expected"
    ///
    /// Expr.TryGetReflectedDefinition(methInfo)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ fun x -> x + 1 @&gt;</c>, which is the implementation of the
    /// method <c>f</c>.
    /// </example>
    /// 
    /// <example id="trygetreflecteddefinition-2">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    ///
    /// [&lt;ReflectedDefinition&gt;]
    /// module Methods =
    ///    let f x = (x, x)
    ///
    /// let methInfoGeneric =
    ///     match &lt;@ Methods.f (1, 2) @&gt; with
    ///     | Call(_, mi, _) -> mi.GetGenericMethodDefinition()
    ///     | _ -> failwith "call expected"
    ///
    /// let methInfoAtString = methInfoGeneric.MakeGenericMethod(typeof&lt;string&gt;)
    ///
    /// Expr.TryGetReflectedDefinition(methInfoAtString)
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ fun (x: string) -> (x, x) @&gt;</c>, which is the implementation of the
    /// generic method <c>f</c> instanatiated at type <c>string</c>.
    /// </example>
    static member TryGetReflectedDefinition: methodBase: MethodBase -> Expr option
    
    /// <summary>This function is called automatically when quotation syntax (&lt;@ @&gt;) and other sources of
    /// quotations are used. </summary>
    ///
    /// <param name="qualifyingType">A type in the assembly where the quotation occurs.</param>
    /// <param name="spliceTypes">The spliced types, to replace references to type variables.</param>
    /// <param name="spliceExprs">The spliced expressions to replace references to spliced expressions.</param>
    /// <param name="bytes">The serialized form of the quoted expression.</param>
    ///
    /// <returns>The resulting expression.</returns>
    static member Deserialize: qualifyingType: System.Type * spliceTypes: list<System.Type> * spliceExprs: list<Expr> * bytes: byte[] -> Expr
    
    /// <summary>This function is called automatically when quotation syntax (&lt;@ @&gt;) and other sources of
    /// quotations are used. </summary>
    ///
    /// <param name="qualifyingType">A type in the assembly where the quotation occurs.</param>
    /// <param name="referencedTypes">The type definitions referenced.</param>
    /// <param name="spliceTypes">The spliced types, to replace references to type variables.</param>
    /// <param name="spliceExprs">The spliced expressions to replace references to spliced expressions.</param>
    /// <param name="bytes">The serialized form of the quoted expression.</param>
    ///
    /// <returns>The resulting expression.</returns>
    static member Deserialize40: qualifyingType: Type * referencedTypes: Type[] * spliceTypes: Type[] * spliceExprs: Expr[] * bytes: byte[] -> Expr
    
    /// <summary>Permits interactive environments such as F# Interactive
    /// to explicitly register new pickled resources that represent persisted 
    /// top level definitions.</summary>
    ///
    /// <param name="assembly">The assembly associated with the resource.</param>
    /// <param name="resource">The unique name for the resources being added.</param>
    /// <param name="serializedValue">The serialized resource to register with the environment.</param>
    /// 
    static member RegisterReflectedDefinitions: assembly: Assembly * resource: string * serializedValue: byte[] -> unit

    /// <summary>Permits interactive environments such as F# Interactive
    /// to explicitly register new pickled resources that represent persisted 
    /// top level definitions.</summary>
    ///
    /// <param name="assembly">The assembly associated with the resource.</param>
    /// <param name="resource">The unique name for the resources being added.</param>
    /// <param name="referencedTypes">The type definitions referenced.</param>
    /// <param name="serializedValue">The serialized resource to register with the environment.</param>
    /// 
    static member RegisterReflectedDefinitions: assembly: Assembly * resource: string * serializedValue: byte[] * referencedTypes: Type[] -> unit

    /// <summary>Fetches or creates a new variable with the given name and type from a global pool of shared variables
    /// indexed by name and type. The type is given by the explicit or inferred type parameter</summary>
    ///
    /// <param name="name">The variable name.</param>
    ///
    /// <returns>The created of fetched typed global variable.</returns>
    /// 
    /// <example id="globalvar-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let expr1 = Expr.GlobalVar&lt;int&gt;("x")
    /// let expr2 = Expr.GlobalVar&lt;int&gt;("x")
    /// </code>
    /// Evaluates <c>expr1</c> and <c>expr2</c> to identical quotations.
    /// </example>
    static member GlobalVar<'T> : name: string -> Expr<'T>

    /// <summary>Format the expression as a string</summary>
    ///
    /// <param name="full">Indicates if method, property, constructor and type objects should be printed in detail. If false, these are abbreviated to their name.</param>
    ///
    /// <returns>The formatted string.</returns>
    /// 
    /// <example id="globalvar-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    ///
    /// let expr1 = &lt;@ 1 + 1 @&gt;
    ///
    /// expr1.ToString(true)
    /// </code>
    /// Evaluates <c>"Call (None, Int32 op_Addition[Int32,Int32,Int32](Int32, Int32),[Value (1), Value (1)])"</c>.
    /// </example>
    member ToString: full: bool -> string

/// <summary>Type-carrying quoted expressions. Expressions are generated either
/// by quotations in source text or programatically</summary>
and [<CompiledName("FSharpExpr`1")>]
    [<Class>]
    Expr<'T> =
        inherit Expr
        /// <summary>Gets the raw expression associated with this type-carrying expression</summary>
        /// 
        /// <example id="raw-1">
        /// <code lang="fsharp">
        /// open FSharp.Quotations
        ///
        /// let expr1 = &lt;@ 1 + 1 @&gt;
        ///
        /// expr1.Raw
        /// </code>
        /// Evaluates to the same quotation as <c>&lt;@ expr1 @&gt;</c> except with the weaker type <c>Expr</c> instead of <c>Expr&lt;int&gt;</c>.
        /// </example>
        member Raw: Expr

/// <summary>Contains a set of primitive F# active patterns to analyze F# expression objects</summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Patterns =
    
    /// <summary>An active pattern to recognize expressions that represent getting the address of a value</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the sub-expression of the input AddressOf expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("AddressOfPattern")>]
    val (|AddressOf|_|): input: Expr -> Expr option

    /// <summary>An active pattern to recognize expressions that represent setting the value held at an address </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the target and value expressions of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("AddressSetPattern")>]
    val (|AddressSet|_|): input: Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent applications of first class function values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the function and argument of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ApplicationPattern")>]
    val (|Application|_|): input: Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent calls to static and instance methods, and functions defined in modules</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the object, method and argument sub-expressions of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("CallPattern")>]
    val (|Call|_|): input: Expr -> (Expr option * MethodInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent calls to static and instance methods, and functions defined in modules, including witness arguments</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the object, method, witness-argument and argument sub-expressions of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("CallWithWitnessesPattern")>]
    val (|CallWithWitnesses|_|): input: Expr -> (Expr option * MethodInfo * MethodInfo * Expr list * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent coercions from one type to another</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the source expression and target type of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("CoercePattern")>]
    val (|Coerce|_|): input: Expr -> (Expr * Type) option

    /// <summary>An active pattern to recognize expressions that represent getting a static or instance field </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the object and field of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("FieldGetPattern")>]
    val (|FieldGet|_|): input: Expr -> (Expr option * FieldInfo) option

    /// <summary>An active pattern to recognize expressions that represent setting a static or instance field </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the object, field and value of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("FieldSetPattern")>]
    val (|FieldSet|_|): input: Expr -> (Expr option * FieldInfo * Expr) option

    /// <summary>An active pattern to recognize expressions that represent loops over integer ranges</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the value, start, finish and body of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ForIntegerRangeLoopPattern")>]
    val (|ForIntegerRangeLoop|_|): input: Expr -> (Var * Expr * Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent while loops </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the guard and body of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("WhileLoopPattern")>]
    val (|WhileLoop|_|): input: Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent conditionals</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the condition, then-branch and else-branch of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("IfThenElsePattern")>]
    val (|IfThenElse|_|): input: Expr -> (Expr * Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent first class function values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the variable and body of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("LambdaPattern")>]
    val (|Lambda|_|): input: Expr -> (Var * Expr) option

    /// <summary>An active pattern to recognize expressions that represent let bindings</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the variable, binding expression and body of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("LetPattern")>]
    val (|Let|_|) : input: Expr -> (Var * Expr * Expr) option

    /// <summary>An active pattern to recognize expressions that represent recursive let bindings of one or more variables</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the bindings and body of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("LetRecursivePattern")>]
    val (|LetRecursive|_|): input: Expr -> ((Var * Expr) list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent the construction of arrays </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the element type and values of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewArrayPattern")>]
    val (|NewArray|_|): input: Expr -> (Type * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent invocations of a default constructor of a struct</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the relevant type of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("DefaultValuePattern")>]
    val (|DefaultValue|_|): input: Expr -> Type option

    /// <summary>An active pattern to recognize expressions that represent construction of delegate values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the delegate type, argument parameters and body of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewDelegatePattern")>]
    val (|NewDelegate|_|): input: Expr -> (Type * Var list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent invocation of object constructors</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constructor and arguments of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewObjectPattern")>]
    val (|NewObject|_|): input: Expr -> (ConstructorInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of record values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the record type and field values of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewRecordPattern")>]
    val (|NewRecord|_|): input: Expr -> (Type * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of particular union case values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the union case and field values of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewUnionCasePattern")>]
    val (|NewUnionCase|_|): input: Expr -> (UnionCaseInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of tuple values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the element expressions of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewTuplePattern")>]
    val (|NewTuple|_|): input: Expr -> (Expr list) option

    /// <summary>An active pattern to recognize expressions that represent construction of struct tuple values</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the element expressions of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("NewStructTuplePattern")>]
    val (|NewStructTuple|_|): input: Expr -> (Expr list) option

    /// <summary>An active pattern to recognize expressions that represent the read of a static or instance property, or a non-function value declared in a module</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the object, property and indexer arguments of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PropertyGetPattern")>]
    val (|PropertyGet|_|): input: Expr -> (Expr option * PropertyInfo * Expr list) option

    /// <summary>An active pattern to recognize expressions that represent setting a static or instance property, or a non-function value declared in a module</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the object, property, indexer arguments and setter value of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("PropertySetPattern")>]
    val (|PropertySet|_|): input: Expr -> (Expr option * PropertyInfo * Expr list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent a nested quotation literal</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the nested quotation expression of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("QuotePattern")>]
    [<Obsolete("Please use QuoteTyped or QuoteRaw to distinguish between typed and raw quotation literals")>]
    val (|Quote|_|): input: Expr -> Expr option 

    /// <summary>An active pattern to recognize expressions that represent a nested raw quotation literal</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the nested quotation expression of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("QuoteRawPattern")>]
    val (|QuoteRaw|_|): input: Expr -> Expr option 

    /// <summary>An active pattern to recognize expressions that represent a nested typed quotation literal</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the nested quotation expression of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("QuoteTypedPattern")>]
    val (|QuoteTyped|_|): input: Expr -> Expr option 

    /// <summary>An active pattern to recognize expressions that represent sequential execution of one expression followed by another</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the two sub-expressions of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("SequentialPattern")>]
    val (|Sequential|_|): input: Expr -> (Expr * Expr) option 

    /// <summary>An active pattern to recognize expressions that represent a try/with construct for exception filtering and catching </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the body, exception variable, filter expression and catch expression of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("TryWithPattern")>]
    val (|TryWith|_|): input: Expr -> (Expr * Var * Expr * Var * Expr) option 

    /// <summary>An active pattern to recognize expressions that represent a try/finally construct </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the body and handler parts of the try/finally expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("TryFinallyPattern")>]
    val (|TryFinally|_|): input: Expr -> (Expr * Expr) option 

    /// <summary>An active pattern to recognize expressions that represent getting a tuple field</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the expression and tuple field being accessed</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("TupleGetPattern")>]
    val (|TupleGet|_|): input: Expr -> (Expr * int) option 

    /// <summary>An active pattern to recognize expressions that represent a dynamic type test</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the expression and type being tested</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("TypeTestPattern")>]
    val (|TypeTest|_|): input: Expr -> (Expr * Type) option 

    /// <summary>An active pattern to recognize expressions that represent a test if a value is of a particular union case</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the expression and union case being tested</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("UnionCaseTestPattern")>]
    val (|UnionCaseTest|_|): input: Expr -> (Expr * UnionCaseInfo) option 

    /// <summary>An active pattern to recognize expressions that represent a constant value. This also matches expressions matched by ValueWithName.</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the boxed value and its static type</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ValuePattern")>]
    val (|Value|_|): input: Expr -> (obj * Type) option

    /// <summary>An active pattern to recognize expressions that represent a constant value</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the boxed value, its static type and its name</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ValueWithNamePattern")>]
    val (|ValueWithName|_|): input: Expr -> (obj * Type * string) option

    /// <summary>An active pattern to recognize expressions that are a value with an associated definition</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the boxed value, its static type and its definition</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("WithValuePattern")>]
    val (|WithValue|_|): input: Expr -> (obj * Type * Expr) option

    /// <summary>An active pattern to recognize expressions that represent a variable</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the variable of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("VarPattern")>]
    val (|Var|_|)  : input: Expr -> Var option

    /// <summary>An active pattern to recognize expressions that represent setting a mutable variable</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the variable and value expression of the input expression</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("VarSetPattern")>]
    val (|VarSet|_|): input: Expr -> (Var * Expr) option
    
/// <summary>Contains a set of derived F# active patterns to analyze F# expression objects</summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module DerivedPatterns =    

    /// <summary>An active pattern to recognize expressions that represent a (possibly curried or tupled) first class function value</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the curried variables and body of the input expression</returns>
    /// 
    /// <example id="lambdas-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ (fun (a1, a2) b -> ()) @&gt; with
    /// | Lambdas(curriedVars, _) ->
    ///     curriedVars |> List.map (List.map (fun arg -> arg.Name))
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>[["a1"; "a2"]; ["b"]]</c>.
    /// </example>
    [<CompiledName("LambdasPattern")>]
    val (|Lambdas|_|): input: Expr -> (Var list list * Expr) option

    /// <summary>An active pattern to recognize expressions that represent the application of a (possibly curried or tupled) first class function value</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the function and curried arguments of the input expression</returns>
    /// 
    /// <example id="applications-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ (fun f -> f (1, 2) 3) @&gt; with
    /// | Lambda(_, Applications (f, curriedArgs)) ->
    ///     curriedArgs |> List.map (fun args -> args.Length)
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>[2; 1]</c>.
    /// </example>
    [<CompiledName("ApplicationsPattern")>]
    val (|Applications|_|): input: Expr -> (Expr * Expr list list) option

    /// <summary>An active pattern to recognize expressions of the form <c>a &amp;&amp; b</c> </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the left and right parts of the input expression</returns>
    /// 
    /// <example id="andalso-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ true &amp;&amp; false @&gt; with
    /// | AndAlso (a, b) -> (a, b)
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>&lt;@ true @&gt;, &lt;@ false @&gt;</c>.
    /// </example>
    [<CompiledName("AndAlsoPattern")>]
    val (|AndAlso|_|): input: Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize expressions of the form <c>a || b</c> </summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the left and right parts of the input expression</returns>
    /// 
    /// <example id="orelse-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ true || false @&gt; with
    /// | OrElse (a, b) -> (a, b)
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>&lt;@ true @&gt;, &lt;@ false @&gt;</c>.
    /// </example>
    [<CompiledName("OrElsePattern")>]
    val (|OrElse|_|): input: Expr -> (Expr * Expr) option

    /// <summary>An active pattern to recognize <c>()</c> constant expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern does not bind any results</returns>
    /// 
    /// <example id="unit-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ () @&gt; with
    /// | Unit v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>true</c>.
    /// </example>
    [<CompiledName("UnitPattern")>]
    val (|Unit|_|): input: Expr -> unit option 

    /// <summary>An active pattern to recognize constant boolean expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="bool-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ true @&gt; with
    /// | Bool v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>true</c>.
    /// </example>
    [<CompiledName("BoolPattern")>]
    val (|Bool|_|): input: Expr -> bool option 

    /// <summary>An active pattern to recognize constant string expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="string-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ "a" @&gt; with
    /// | String v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>"a"</c>.
    /// </example>
    [<CompiledName("StringPattern")>]
    val (|String|_|): input: Expr -> string option 

    /// <summary>An active pattern to recognize constant 32-bit floating point number expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="single-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 1.0f @&gt; with
    /// | Single v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>1.0f</c>.
    /// </example>
    [<CompiledName("SinglePattern")>]
    val (|Single|_|): input: Expr -> float32 option 

    /// <summary>An active pattern to recognize constant 64-bit floating point number expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="double-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 1.0 @&gt; with
    /// | Double v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>1.0</c>.
    /// </example>
    [<CompiledName("DoublePattern")>]
    val (|Double|_|): input: Expr -> float option 

    /// <summary>An active pattern to recognize constant unicode character expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="char-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 'a' @&gt; with
    /// | Char v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>'a'</c>.
    /// </example>
    [<CompiledName("CharPattern")>]
    val (|Char|_|): input: Expr -> char  option 

    /// <summary>An active pattern to recognize constant signed byte expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="sbyte-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8y @&gt; with
    /// | SByte v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8y</c>.
    /// </example>
    [<CompiledName("SBytePattern")>]
    val (|SByte|_|): input: Expr -> sbyte option 

    /// <summary>An active pattern to recognize constant byte expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="byte-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8uy @&gt; with
    /// | Byte v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8uy</c>.
    /// </example>
    [<CompiledName("BytePattern")>]
    val (|Byte|_|): input: Expr -> byte option 

    /// <summary>An active pattern to recognize constant int16 expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="int16-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8s @&gt; with
    /// | Int16 v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8s</c>.
    /// </example>
    [<CompiledName("Int16Pattern")>]
    val (|Int16|_|): input: Expr -> int16 option 

    /// <summary>An active pattern to recognize constant unsigned int16 expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="uint16-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8us @&gt; with
    /// | UInt16 v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8us</c>.
    /// </example>
    [<CompiledName("UInt16Pattern")>]
    val (|UInt16|_|): input: Expr -> uint16 option 

    /// <summary>An active pattern to recognize constant int32 expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="int32-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8 @&gt; with
    /// | Int32 v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8</c>.
    /// </example>
    [<CompiledName("Int32Pattern")>]
    val (|Int32|_|): input: Expr -> int32 option 

    /// <summary>An active pattern to recognize constant unsigned int32 expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="uint32-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8u @&gt; with
    /// | UInt32 v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8u</c>.
    /// </example>
    [<CompiledName("UInt32Pattern")>]
    val (|UInt32|_|): input: Expr -> uint32 option 

    /// <summary>An active pattern to recognize constant int64 expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="int64-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8L @&gt; with
    /// | Int64 v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8L</c>.
    /// </example>
    [<CompiledName("Int64Pattern")>]
    val (|Int64|_|): input: Expr -> int64 option 

    /// <summary>An active pattern to recognize constant unsigned int64 expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="uint64-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8UL @&gt; with
    /// | UInt64 v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8UL</c>.
    /// </example>
    [<CompiledName("UInt64Pattern")>]
    val (|UInt64|_|): input: Expr -> uint64 option 

    /// <summary>An active pattern to recognize constant decimal expressions</summary>
    ///
    /// <param name="input">The input expression to match against.</param>
    ///
    /// <returns>When successful, the pattern binds the constant value from the input expression</returns>
    /// 
    /// <example id="decimal-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// match &lt;@ 8.0M @&gt; with
    /// | Decimal v -> v
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to <c>8.0M</c>.
    /// </example>
    [<CompiledName("DecimalPattern")>]
    val (|Decimal|_|): input: Expr -> decimal option 

    /// <summary>A parameterized active pattern to recognize calls to a specified function or method.
    /// The returned elements are the optional target object (present if the target is an 
    /// instance method), the generic type instantiation (non-empty if the target is a generic
    /// instantiation), and the arguments to the function or method.</summary>
    ///
    /// <param name="templateParameter">The input template expression to specify the method to call.</param>
    ///
    /// <returns>The optional target object (present if the target is an 
    /// instance method), the generic type instantiation (non-empty if the target is a generic
    /// instantiation), and the arguments to the function or method.</returns>
    /// 
    /// <example id="specificcall-1">Match a specific call to Console.WriteLine taking one string argument:
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// let inpExpr = &lt;@ Console.WriteLine("hello") @&gt;
    ///
    /// match inpExpr with
    /// | SpecificCall &lt;@ Console.WriteLine("1") @&gt; (None, [], [ argExpr ]) -> argExpr
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ "hello" @&gt;</c>.
    /// </example>
    /// 
    /// <example id="specificcall-2">Calls to this active pattern can be partially applied to pre-evaluate some aspects of the matching. For example:
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// let (|ConsoleWriteLineOneArg|_|) = (|SpecificCall|_|) &lt;@ Console.WriteLine("1") @&gt;
    ///
    /// let inpExpr = &lt;@ Console.WriteLine("hello") @&gt;
    ///
    /// match inpExpr with
    /// | ConsoleWriteLineOneArg (None, [], [ argExpr ]) -> argExpr
    /// | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates to a quotation with the same structure as <c>&lt;@ "hello" @&gt;</c>.
    /// </example>
    [<CompiledName("SpecificCallPattern")>]
    val (|SpecificCall|_|): templateParameter: Expr -> (Expr -> (Expr option * Type list * Expr list) option)

    /// <summary>An active pattern to recognize methods that have an associated ReflectedDefinition</summary>
    ///
    /// <param name="methodBase">The description of the method.</param>
    ///
    /// <returns>The expression of the method definition if found, or None.</returns>
    /// 
    /// <example id="methodwithreflecteddefinition-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// [&lt;ReflectedDefinition&gt;]
    /// let f x = (x, x)
    ///
    /// let inpExpr = &lt;@ f 4 @&gt;
    ///
    /// let implExpr =
    ///     match inpExpr with
    ///     | Call(None, MethodWithReflectedDefinition implExpr, [ _argExpr ]) -> implExpr
    ///     | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates <c>implExpr</c> to a quotation with the same structure as <c>&lt;@ fun (x: int) -> (x, x) @&gt;</c>, which is the implementation of the
    /// method <c>f</c>. Note that the correct generic instantaition has been applied to the implementation to reflect
    /// the the type at the callsite.
    /// </example>
    /// 
    [<CompiledName("MethodWithReflectedDefinitionPattern")>]
    val (|MethodWithReflectedDefinition|_|): methodBase: MethodBase -> Expr option
    
    /// <summary>An active pattern to recognize property getters or values in modules that have an associated ReflectedDefinition</summary>
    ///
    /// <param name="propertyInfo">The description of the property.</param>
    ///
    /// <returns>The expression of the method definition if found, or None.</returns>
    /// 
    /// <example id="propertywithreflecteddefinition-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// [&lt;ReflectedDefinition&gt;]
    /// type C&lt;'T&gt;() =
    ///    member x.Identity = x
    ///
    /// let inpExpr = &lt;@ C&lt;int&gt;().Identity @&gt;
    ///
    /// let implExpr =
    ///     match inpExpr with
    ///     | PropertyGet(Some _, PropertyGetterWithReflectedDefinition implExpr, [ ]) -> implExpr
    ///     | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates <c>implExpr</c> to a quotation with the same structure as <c>&lt;@ fun (x: C&lt;int&gt;) () -> x @&gt;</c>, which is the implementation of the
    /// property <c>Identity</c>. Note that the correct generic instantaition has been applied to the implementation to reflect
    /// the the type at the callsite.
    /// </example>
    /// 
    [<CompiledName("PropertyGetterWithReflectedDefinitionPattern")>]
    val (|PropertyGetterWithReflectedDefinition|_|): propertyInfo: PropertyInfo -> Expr option

    /// <summary>An active pattern to recognize property setters that have an associated ReflectedDefinition</summary>
    ///
    /// <param name="propertyInfo">The description of the property.</param>
    ///
    /// <returns>The expression of the method definition if found, or None.</returns>
    /// 
    /// <example id="propertysetterwithreflecteddefinition-1">
    /// <code lang="fsharp">
    /// open FSharp.Quotations
    /// open FSharp.Quotations.Patterns
    /// open FSharp.Quotations.DerivedPatterns
    ///
    /// [&lt;ReflectedDefinition&gt;]
    /// type C&lt;'T&gt;() =
    ///    member x.Count with set (v: int) = ()
    ///
    /// let inpExpr = &lt;@ C&lt;int&gt;().Count &lt;- 3 @&gt;
    ///
    /// let implExpr =
    ///     match inpExpr with
    ///     | PropertySet(Some _, PropertySetterWithReflectedDefinition implExpr, [], _valueExpr) -> implExpr
    ///     | _ -> failwith "unexpected"
    /// </code>
    /// Evaluates <c>implExpr</c> to a quotation with the same structure as <c>&lt;@ fun (x: C&lt;int&gt;) (v: int) -> () @&gt;</c>, which is the implementation of the
    /// setter for the property <c>Count</c>. Note that the correct generic instantaition has been applied to the implementation to reflect
    /// the the type at the callsite.
    /// </example>
    /// 
    [<CompiledName("PropertySetterWithReflectedDefinitionPattern")>]
    val (|PropertySetterWithReflectedDefinition|_|): propertyInfo: PropertyInfo -> Expr option

/// <summary>Active patterns for traversing, visiting, rebuilding and transforming expressions in a generic way</summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ExprShape =

    /// <summary>An active pattern that performs a complete decomposition viewing the expression tree as a binding structure</summary>
    ///
    /// <param name="input">The input expression.</param>
    ///
    /// <returns>The decomposed Var, Lambda, or ConstApp.</returns>
    /// 
    /// <example-tbd></example-tbd>
    [<CompiledName("ShapePattern")>]
    val (|ShapeVar|ShapeLambda|ShapeCombination|): 
        input: Expr
        -> Choice<Var,                // Var
                  (Var * Expr),       // Lambda
                  (obj * list<Expr>)> // ConstApp

    /// <summary>Re-build combination expressions. The first parameter should be an object
    /// returned by the <c>ShapeCombination</c> case of the active pattern in this module.</summary>
    ///
    /// <param name="shape">The input shape.</param>
    /// <param name="arguments">The list of arguments.</param>
    ///
    /// <returns>The rebuilt expression.</returns>
    /// 
    /// <example-tbd></example-tbd>
    val RebuildShapeCombination: shape: obj * arguments: list<Expr> -> Expr
