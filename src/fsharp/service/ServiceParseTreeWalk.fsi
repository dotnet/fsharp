// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.Text

module AstTraversal = 
  val rangeContainsPosLeftEdgeInclusive : m1:range -> p:pos -> bool
  val rangeContainsPosEdgesExclusive : m1:range -> p:pos -> bool
  val rangeContainsPosLeftEdgeExclusiveAndRightEdgeInclusive : m1:range -> p:pos -> bool

  [<RequireQualifiedAccess>]
  type TraverseStep =
    | Expr of SynExpr
    | Module of SynModuleDecl
    | ModuleOrNamespace of SynModuleOrNamespace
    | TypeDefn of SynTypeDefn
    | MemberDefn of SynMemberDefn
    | MatchClause of SynMatchClause
    | Binding of SynBinding

  type TraversePath = TraverseStep list

  [<AbstractClass>]
  type AstVisitorBase<'T> =
      new : unit -> AstVisitorBase<'T>

      default VisitBinding : defaultTraverse:(SynBinding -> 'T option) * binding:SynBinding -> 'T option
      abstract VisitBinding : (SynBinding -> 'T option) * SynBinding -> 'T option

      default VisitComponentInfo : SynComponentInfo -> 'T option
      abstract member VisitComponentInfo : SynComponentInfo -> 'T option

      abstract member VisitExpr : TraversePath * (SynExpr -> 'T option) * (SynExpr -> 'T option) * SynExpr -> 'T option
      default VisitHashDirective : range -> 'T option

      abstract VisitHashDirective : range -> 'T option
      default VisitImplicitInherit : defaultTraverse:(SynExpr -> 'T option) * _ty:SynType * expr:SynExpr * _m:range -> 'T option

      abstract member VisitImplicitInherit : (SynExpr -> 'T option) * SynType * SynExpr * range -> 'T option 
      default VisitInheritSynMemberDefn : _componentInfo:SynComponentInfo * _typeDefnKind:SynTypeDefnKind * _synType:SynType * _members:SynMemberDefns * _range:range -> 'T option

      abstract member VisitInheritSynMemberDefn : SynComponentInfo * SynTypeDefnKind * SynType * SynMemberDefns * range -> 'T option
      default VisitInterfaceSynMemberDefnType : _synType:SynType -> 'T option

      abstract member VisitInterfaceSynMemberDefnType : SynType -> 'T option
      default VisitLetOrUse : TraversePath * (SynBinding -> 'T option) * SynBinding list * range -> 'T option

      abstract member VisitLetOrUse : TraversePath * (SynBinding -> 'T option) * SynBinding list * range -> 'T option
      default VisitMatchClause : defaultTraverse:(SynMatchClause -> 'T option) * mc:SynMatchClause -> 'T option

      abstract member VisitMatchClause : (SynMatchClause -> 'T option) * SynMatchClause -> 'T option
      default VisitModuleDecl : defaultTraverse:(SynModuleDecl -> 'T option) * decl:SynModuleDecl -> 'T option

      abstract member VisitModuleDecl : (SynModuleDecl -> 'T option) * SynModuleDecl -> 'T option
      default VisitModuleOrNamespace : SynModuleOrNamespace -> 'T option

      abstract member VisitModuleOrNamespace : SynModuleOrNamespace -> 'T option
      default VisitPat : defaultTraverse:(SynPat -> 'T option) * pat:SynPat -> 'T option

      abstract member VisitPat : (SynPat -> 'T option) * SynPat -> 'T option
      default VisitRecordField : _path:TraversePath * _copyOpt:SynExpr option * _recordField:LongIdentWithDots option -> 'T option

      abstract member VisitRecordField : TraversePath * SynExpr option * LongIdentWithDots option -> 'T option
      default VisitSimplePats : SynSimplePat list -> 'T option

      abstract member VisitSimplePats : SynSimplePat list -> 'T option
      default VisitType : defaultTraverse:(SynType -> 'T option) * ty:SynType -> 'T option

      abstract member VisitType : (SynType -> 'T option) * SynType -> 'T option
      default VisitTypeAbbrev : _ty:SynType * _m:range -> 'T option

      abstract member VisitTypeAbbrev : SynType * range -> 'T option

  val dive : node:'a -> range:'b -> project:('a -> 'c) -> 'b * (unit -> 'c)

  val pick : pos:pos -> outerRange:range -> _debugObj:obj -> diveResults:(range * (unit -> 'a option)) list -> 'a option

  val Traverse : pos:pos * parseTree:ParsedInput * visitor:AstVisitorBase<'T> -> 'T option
