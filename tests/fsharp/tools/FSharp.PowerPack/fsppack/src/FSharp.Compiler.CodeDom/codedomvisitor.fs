namespace Microsoft.Test.Compiler.CodeDom.Internal

open System
open System.IO
open System.Text
open System.Collections
open System.CodeDom
open System.CodeDom.Compiler
open Microsoft.FSharp.Collections

//---------------------------------------------------------------------------------------------
// This module contains several utility functions for walking through CodeDom tree
module Visitor = 

  // Get all relevant CodeDom properties of an object 
  // - more functions can return properties for one object because of class hierarchy 
  let memberMap = [
      (fun (c:obj) -> match c with | :? CodeArrayCreateExpression as co -> [(co.CreateType:>obj); (co.Initializers:>obj); (co.SizeExpression:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeArrayIndexerExpression as co -> [(co.Indices:>obj); (co.TargetObject:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeAssignStatement as co -> [(co.Left:>obj); (co.Right:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeAttachEventStatement as co -> [(co.Event:>obj); (co.Listener:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeAttributeArgument as co -> [(co.Value:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeAttributeDeclaration as co -> [(co.AttributeType:>obj); (co.Arguments:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeBinaryOperatorExpression as co -> [(co.Left:>obj); (co.Operator:>obj); (co.Right:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeCastExpression as co -> [(co.Expression:>obj); (co.TargetType:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeCatchClause as co -> [(co.CatchExceptionType:>obj); (co.Statements:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeCommentStatement as co -> [(co.Comment:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeCompileUnit as co -> [(co.AssemblyCustomAttributes:>obj); (co.EndDirectives:>obj); (co.Namespaces:>obj); (co.StartDirectives:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeConditionStatement as co -> [(co.Condition:>obj); (co.FalseStatements:>obj); (co.TrueStatements:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeConstructor as co -> [(co.BaseConstructorArgs:>obj); (co.ChainedConstructorArgs:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeDefaultValueExpression as co -> [(co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeDelegateCreateExpression as co -> [(co.TargetObject:>obj); (co.DelegateType:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeDelegateInvokeExpression as co -> [(co.TargetObject:>obj); (co.Parameters:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeDirectionExpression as co -> [(co.Expression:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeEventReferenceExpression as co -> [(co.TargetObject:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeExpressionStatement as co -> [(co.Expression:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeFieldReferenceExpression as co -> [(co.TargetObject:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeIndexerExpression as co -> [(co.Indices:>obj); (co.TargetObject:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeIterationStatement as co -> [(co.IncrementStatement:>obj); (co.InitStatement:>obj); (co.Statements:>obj); (co.TestExpression:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeLabeledStatement as co -> [(co.Statement:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMemberEvent as co -> [(co.ImplementationTypes:>obj); (co.PrivateImplementationType:>obj); (co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMemberField as co -> [(co.InitExpression:>obj); (co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMemberMethod as co -> [(co.ImplementationTypes:>obj); (co.Parameters:>obj); (co.PrivateImplementationType:>obj); (co.ReturnType:>obj); (co.ReturnTypeCustomAttributes:>obj); (co.Statements:>obj); (co.TypeParameters:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMemberProperty as co -> [(co.GetStatements:>obj); (co.ImplementationTypes:>obj); (co.Parameters:>obj); (co.PrivateImplementationType:>obj); (co.SetStatements:>obj); (co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMethodInvokeExpression as co -> [(co.Method:>obj); (co.Parameters:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMethodReferenceExpression as co -> [(co.TargetObject:>obj); (co.TypeArguments:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeMethodReturnStatement as co -> [(co.Expression:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeNamespace as co -> [(co.Comments:>obj); (co.Imports:>obj); (co.Types:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeNamespaceImport as co -> [(co.LinePragma:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeObjectCreateExpression as co -> [(co.CreateType:>obj); (co.Parameters:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeParameterDeclarationExpression as co -> [(co.CustomAttributes:>obj); (co.Direction:>obj); (co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodePropertyReferenceExpression as co -> [(co.TargetObject:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeRemoveEventStatement as co -> [(co.Event:>obj); (co.Listener:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeStatement as co -> [(co.EndDirectives:>obj); (co.StartDirectives:>obj); (co.LinePragma:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeThrowExceptionStatement as co -> [(co.ToThrow:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTryCatchFinallyStatement as co -> [(co.CatchClauses:>obj); (co.FinallyStatements:>obj); (co.TryStatements:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeDeclaration as co -> [(co.BaseTypes:>obj); (co.Members:>obj); (co.TypeAttributes:>obj); (co.TypeParameters:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeDelegate as co -> [(co.Parameters:>obj); (co.ReturnType:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeMember as co -> [(co.Attributes:>obj); (co.Comments:>obj); (co.CustomAttributes:>obj); (co.EndDirectives:>obj); (co.LinePragma:>obj); (co.StartDirectives:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeOfExpression as co -> [(co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeParameter as co -> [(co.Constraints:>obj); (co.CustomAttributes:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeReference as co -> [(co.ArrayElementType:>obj); (co.TypeArguments:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeTypeReferenceExpression as co -> [(co.Type:>obj);] | _ -> []);
      (fun (c:obj) -> match c with | :? CodeVariableDeclarationStatement as co -> [(co.InitExpression:>obj); (co.Type:>obj);] | _ -> []) ];

  let children o = memberMap |> Seq.collect (fun e -> e o) 

  let rec codeDomFold' f st o = 
    match box o with 
      | :? CollectionBase as cl -> 
           cl |> Seq.cast |> Seq.fold (codeDomFold' f) st;
      | _ ->
           let (nst,recurse) = f st o;
           if (recurse) then
             o |> children |> Seq.fold (codeDomFold' f) nst;
           else nst

  let codeDomCallbackWithScope' f  = 
    let rec callback oscope res o =
        match box o with 
          | :? CollectionBase as cl -> 
              cl |> Seq.cast |> Seq.fold (f callback oscope) res;
          | _ ->
              o |> children |> Seq.fold (f callback oscope) res;
    f callback;
                       
  /// Search for members and return flat list of selected members
  /// Function given as an argument returns tuple - first item specifies
  /// if the current element should be included in the result, the second
  /// specifies if we should walk through child members of the current object
  let codeDomFlatFilter f o = codeDomFold' ( fun st o -> let (inc,rc) = (f o) in if (inc) then (o::st,rc) else (st,rc) ) [] (box o)            
  
  /// Walks through the CodeDom tree and keeps current "scope" and the result.
  /// The result is collected through entire tree, but the modified scope is 
  /// passed only to sub-nodes of the current node.
  ///
  /// First argument is a function that is called for nodes and has a 
  /// function as a first argument, scope and result as a second and current node as a third.
  /// The function argument can be used to walk deeper in the tree if wanted.
  let codeDomCallbackWithScope f scope st o = codeDomCallbackWithScope' f scope st (box o)
  let codeDomCallBackNoScope f st o = codeDomCallbackWithScope (fun rcall () res x -> f (rcall ()) res x) () st o
