(*** hide ***)
#I "../../../artifacts/bin/fcs/net461"
(**
Compiler Services: Processing typed expression tree
=================================================

This tutorial demonstrates how to get the checked, typed expressions tree (TAST)
for F# code and how to walk over the tree. 

This can be used for creating tools such as source code analyzers and refactoring tools.
You can also combine the information with the API available
from [symbols](symbols.html). 

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published


Getting checked expressions
-----------------------

To access the type-checked, resolved expressions, you need to create an instance of `InteractiveChecker`.

To use the interactive checker, reference `FSharp.Compiler.Service.dll` and open the
`SourceCodeServices` namespace:
*)
#r "FSharp.Compiler.Service.dll"
open System
open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
(**

### Checking code

We first parse and check some code as in the [symbols](symbols.html) tutorial.
One difference is that we set keepAssemblyContents to true.

*)
// Create an interactive checker instance 
let checker = FSharpChecker.Create(keepAssemblyContents=true)

let parseAndCheckSingleFile (input) = 
    let file = Path.ChangeExtension(System.IO.Path.GetTempFileName(), "fsx")  
    File.WriteAllText(file, input)
    // Get context representing a stand-alone (script) file
    let projOptions, _errors = 
        checker.GetProjectOptionsFromScript(file, SourceText.ofString input)
        |> Async.RunSynchronously

    checker.ParseAndCheckProject(projOptions) 
    |> Async.RunSynchronously

(**
## Getting the expressions

After type checking a file, you can access the declarations and contents of the assembly, including expressions:

*)

let input2 = 
      """
module MyLibrary 

open System

let foo(x, y) = 
    let msg = String.Concat("Hello", " ", "world")
    if msg.Length > 10 then 
        10 
    else 
        20

type MyClass() = 
    member x.MyMethod() = 1
      """
let checkProjectResults = 
    parseAndCheckSingleFile(input2)

checkProjectResults.Errors // should be empty


(**

Checked assemblies are made up of a series of checked implementation files.  The "file" granularity
matters in F# because initialization actions are triggered at the granularity of files.
In this case there is only one implementation file in the project:

*)

let checkedFile = checkProjectResults.AssemblyContents.ImplementationFiles.[0]

(**

Checked assemblies are made up of a series of checked implementation files.  The "file" granularity
matters in F# because initialization actions are triggered at the granularity of files.
In this case there is only one implementation file in the project:

*)

let rec printDecl prefix d = 
    match d with 
    | FSharpImplementationFileDeclaration.Entity (e, subDecls) -> 
        printfn "%sEntity %s was declared and contains %d sub-declarations" prefix e.CompiledName subDecls.Length
        for subDecl in subDecls do 
            printDecl (prefix+"    ") subDecl
    | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(v, vs, e) -> 
        printfn "%sMember or value %s was declared" prefix  v.CompiledName
    | FSharpImplementationFileDeclaration.InitAction(e) -> 
        printfn "%sA top-level expression was declared" prefix 


for d in checkedFile.Declarations do 
   printDecl "" d

// Entity MyLibrary was declared and contains 4 sub-declarations
//     Member or value foo was declared
//     Entity MyClass was declared and contains 0 sub-declarations
//     Member or value .ctor was declared
//     Member or value MyMethod was declared

(**

As can be seen, the only declaration in the implementation file is that of the module MyLibrary, which 
contains fours sub-declarations.  

> As an aside, one peculiarity here is that the member declarations (e.g. the "MyMethod" member) are returned as part of the containing module entity, not as part of their class.

> Note that the class constructor is returned as a separate declaration. The class type definition has been "split" into a constructor and the other declarations.

*)

let myLibraryEntity, myLibraryDecls =    
   match checkedFile.Declarations.[0] with 
   | FSharpImplementationFileDeclaration.Entity (e, subDecls) -> (e, subDecls)
   | _ -> failwith "unexpected"


(**

What about the expressions, for example the body of function "foo"? Let's find it:
*)

let (fooSymbol, fooArgs, fooExpression) = 
    match myLibraryDecls.[0] with 
    | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(v, vs, e) -> (v, vs, e)
    | _ -> failwith "unexpected"


(** Here 'fooSymbol' is a symbol associated with the declaration of 'foo', 
'fooArgs' represents the formal arguments to the 'foo' function, and 'fooExpression' 
is an expression for the implementation of the 'foo' function.

Once you have an expression, you can work with it much like an F# quotation.  For example, 
you can find its declaration range and its type:

*)

fooExpression.Type  // shows that the return type of the body expression is 'int'
fooExpression.Range  // shows the declaration range of the expression implementing 'foo'

(**

### Walking over expressions


Expressions are analyzed using active patterns, much like F# quotations.
Here is a generic expression visitor:

*)

let rec visitExpr f (e:FSharpExpr) = 
    f e
    match e with 
    | BasicPatterns.AddressOf(lvalueExpr) -> 
        visitExpr f lvalueExpr
    | BasicPatterns.AddressSet(lvalueExpr, rvalueExpr) -> 
        visitExpr f lvalueExpr; visitExpr f rvalueExpr
    | BasicPatterns.Application(funcExpr, typeArgs, argExprs) -> 
        visitExpr f funcExpr; visitExprs f argExprs
    | BasicPatterns.Call(objExprOpt, memberOrFunc, typeArgs1, typeArgs2, argExprs) -> 
        visitObjArg f objExprOpt; visitExprs f argExprs
    | BasicPatterns.Coerce(targetType, inpExpr) -> 
        visitExpr f inpExpr
    | BasicPatterns.FastIntegerForLoop(startExpr, limitExpr, consumeExpr, isUp) -> 
        visitExpr f startExpr; visitExpr f limitExpr; visitExpr f consumeExpr
    | BasicPatterns.ILAsm(asmCode, typeArgs, argExprs) -> 
        visitExprs f argExprs
    | BasicPatterns.ILFieldGet (objExprOpt, fieldType, fieldName) -> 
        visitObjArg f objExprOpt
    | BasicPatterns.ILFieldSet (objExprOpt, fieldType, fieldName, valueExpr) -> 
        visitObjArg f objExprOpt
    | BasicPatterns.IfThenElse (guardExpr, thenExpr, elseExpr) -> 
        visitExpr f guardExpr; visitExpr f thenExpr; visitExpr f elseExpr
    | BasicPatterns.Lambda(lambdaVar, bodyExpr) -> 
        visitExpr f bodyExpr
    | BasicPatterns.Let((bindingVar, bindingExpr), bodyExpr) -> 
        visitExpr f bindingExpr; visitExpr f bodyExpr
    | BasicPatterns.LetRec(recursiveBindings, bodyExpr) -> 
        List.iter (snd >> visitExpr f) recursiveBindings; visitExpr f bodyExpr
    | BasicPatterns.NewArray(arrayType, argExprs) -> 
        visitExprs f argExprs
    | BasicPatterns.NewDelegate(delegateType, delegateBodyExpr) -> 
        visitExpr f delegateBodyExpr
    | BasicPatterns.NewObject(objType, typeArgs, argExprs) -> 
        visitExprs f argExprs
    | BasicPatterns.NewRecord(recordType, argExprs) ->  
        visitExprs f argExprs
    | BasicPatterns.NewAnonRecord(recordType, argExprs) ->  
        visitExprs f argExprs
    | BasicPatterns.NewTuple(tupleType, argExprs) -> 
        visitExprs f argExprs
    | BasicPatterns.NewUnionCase(unionType, unionCase, argExprs) -> 
        visitExprs f argExprs
    | BasicPatterns.Quote(quotedExpr) -> 
        visitExpr f quotedExpr
    | BasicPatterns.FSharpFieldGet(objExprOpt, recordOrClassType, fieldInfo) -> 
        visitObjArg f objExprOpt
    | BasicPatterns.AnonRecordGet(objExpr, recordOrClassType, fieldInfo) -> 
        visitExpr f objExpr
    | BasicPatterns.FSharpFieldSet(objExprOpt, recordOrClassType, fieldInfo, argExpr) -> 
        visitObjArg f objExprOpt; visitExpr f argExpr
    | BasicPatterns.Sequential(firstExpr, secondExpr) -> 
        visitExpr f firstExpr; visitExpr f secondExpr
    | BasicPatterns.TryFinally(bodyExpr, finalizeExpr) -> 
        visitExpr f bodyExpr; visitExpr f finalizeExpr
    | BasicPatterns.TryWith(bodyExpr, _, _, catchVar, catchExpr) -> 
        visitExpr f bodyExpr; visitExpr f catchExpr
    | BasicPatterns.TupleGet(tupleType, tupleElemIndex, tupleExpr) -> 
        visitExpr f tupleExpr
    | BasicPatterns.DecisionTree(decisionExpr, decisionTargets) -> 
        visitExpr f decisionExpr; List.iter (snd >> visitExpr f) decisionTargets
    | BasicPatterns.DecisionTreeSuccess (decisionTargetIdx, decisionTargetExprs) -> 
        visitExprs f decisionTargetExprs
    | BasicPatterns.TypeLambda(genericParam, bodyExpr) -> 
        visitExpr f bodyExpr
    | BasicPatterns.TypeTest(ty, inpExpr) -> 
        visitExpr f inpExpr
    | BasicPatterns.UnionCaseSet(unionExpr, unionType, unionCase, unionCaseField, valueExpr) -> 
        visitExpr f unionExpr; visitExpr f valueExpr
    | BasicPatterns.UnionCaseGet(unionExpr, unionType, unionCase, unionCaseField) -> 
        visitExpr f unionExpr
    | BasicPatterns.UnionCaseTest(unionExpr, unionType, unionCase) -> 
        visitExpr f unionExpr
    | BasicPatterns.UnionCaseTag(unionExpr, unionType) -> 
        visitExpr f unionExpr
    | BasicPatterns.ObjectExpr(objType, baseCallExpr, overrides, interfaceImplementations) -> 
        visitExpr f baseCallExpr
        List.iter (visitObjMember f) overrides
        List.iter (snd >> List.iter (visitObjMember f)) interfaceImplementations
    | BasicPatterns.TraitCall(sourceTypes, traitName, typeArgs, typeInstantiation, argTypes, argExprs) -> 
        visitExprs f argExprs
    | BasicPatterns.ValueSet(valToSet, valueExpr) -> 
        visitExpr f valueExpr
    | BasicPatterns.WhileLoop(guardExpr, bodyExpr) -> 
        visitExpr f guardExpr; visitExpr f bodyExpr
    | BasicPatterns.BaseValue baseType -> ()
    | BasicPatterns.DefaultValue defaultType -> ()
    | BasicPatterns.ThisValue thisType -> ()
    | BasicPatterns.Const(constValueObj, constType) -> ()
    | BasicPatterns.Value(valueToGet) -> ()
    | _ -> failwith (sprintf "unrecognized %+A" e)

and visitExprs f exprs = 
    List.iter (visitExpr f) exprs

and visitObjArg f objOpt = 
    Option.iter (visitExpr f) objOpt

and visitObjMember f memb = 
    visitExpr f memb.Body

(**
Let's use this expresssion walker:

*)
fooExpression |> visitExpr (fun e -> printfn "Visiting %A" e)

// Prints:
//
// Visiting Let...
// Visiting Call...
// Visiting Const ("Hello", ...)
// Visiting Const (" ", ...)
// Visiting Const ("world", ...)
// Visiting IfThenElse...
// Visiting Call...
// Visiting Call...
// Visiting Value ...
// Visiting Const ...
// Visiting Const ...
// Visiting Const ...

(**
Note that 

* The visitExpr function is recursive (for nested expressions).

* Pattern matching is removed from the tree, into a form called 'decision trees'. 

Summary
-------
In this tutorial, we looked at basic of working with checked declarations and expressions. 

In practice, it is also useful to combine the information here
with some information you can obtain from the [symbols](symbols.html) 
tutorial.
*)
