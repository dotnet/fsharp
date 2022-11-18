module ParallelTypeCheckingTests.Code.TrieApproach.SampleData

open System.Collections.Generic
open NUnit.Framework
open ParallelTypeCheckingTests.Code.TrieApproach.DependencyResolution

// Some helper DSL functions to construct the FileContentEntry items
// This should again be mapped from the AST

let topLevelNS (topLevelNamespaceString: string) (content: FileContentEntry list) =
    topLevelNamespaceString.Split(".")
    |> Array.toList
    |> fun name -> FileContentEntry.TopLevelNamespace(name, content)

let topLevelMod (topLevelModuleString: string) (content: FileContentEntry list) =
    let parts = topLevelModuleString.Split(".")

    parts
    |> Array.take (parts.Length - 1)
    |> Array.toList
    |> fun name -> FileContentEntry.TopLevelNamespace(name, content)

let openSt (openStatement: string) =
    openStatement.Split(".") |> Array.toList |> FileContentEntry.OpenStatement

let nestedModule name content =
    FileContentEntry.NestedModule(name, content)

let prefIdent (lid: string) =
    let parts = lid.Split(".")
    Array.take (parts.Length - 1) parts |> List.ofArray |> PrefixedIdentifier

// Some hardcoded files processing, this was done by the naked eye and some regexes.

let files =
    [|
        {
            Name = "AssemblyInfo.fs"
            Idx = 0
            Content =
                [|
                    topLevelNS
                        "System"
                        [
                            openSt "System.Runtime.CompilerServices"
                            nestedModule "AssemblyVersionInformation" []
                        ]
                |]
        }
        {
            Name = "ISourceTextExtensions.fs"
            Idx = 1
            Content =
                [|
                    topLevelMod
                        "Fantomas.Core.ISourceTextExtensions"
                        [
                            openSt "System.Text"
                            openSt "FSharp.Compiler.Text"
                            prefIdent "range.StartLine"
                            prefIdent "this.GetLineString"
                            prefIdent "range.StartLine"
                            prefIdent "range.EndLine"
                            prefIdent "range.EndColumn"
                            prefIdent "range.StartColumn"
                            prefIdent "line.Substring"
                            prefIdent "sb.AppendLine"
                            prefIdent "lastLine.Substring"
                        ]
                |]
        }
        {
            Name = "RangeHelpers.fs"
            Idx = 2
            Content =
                [|
                    topLevelNS
                        "Fantomas.Core"
                        [
                            openSt "FSharp.Compiler.Text"
                            nestedModule
                                "RangeHelpers"
                                [
                                    prefIdent "Position.posGeq"
                                    prefIdent "b.Start"
                                    prefIdent "a.Start"
                                    prefIdent "a.End"
                                    prefIdent "b.End"
                                    prefIdent "Range.equals"
                                    prefIdent "r1.FileName"
                                    prefIdent "r2.FileName"
                                    prefIdent "r1.End"
                                    prefIdent "r2.Start"
                                    prefIdent "r1.EndColumn"
                                    prefIdent "r2.StartColumn"
                                    prefIdent "Range.mkRange"
                                    prefIdent "r.FileName"
                                    prefIdent "r.Start"
                                    prefIdent "Position.mkPos"
                                    prefIdent "r.StartLine"
                                    prefIdent "r.StartColumn"
                                    prefIdent "r.EndLine"
                                    prefIdent "r.EndColumn"
                                    prefIdent "r.End"
                                    prefIdent "List.sortBy"
                                    prefIdent "List.reduce"
                                    prefIdent "Range.unionRanges"
                                ]
                            nestedModule
                                "RangePatterns"
                                [
                                    prefIdent "RangeHelpers.mkStartEndRange"
                                    prefIdent "range.FileName"
                                    prefIdent "range.Start"
                                    prefIdent "range.StartLine"
                                    prefIdent "range.StartColumn"
                                ]
                        ]
                |]
        }
        {
            Name = "AstExtensions.fsi"
            Idx = 3
            Content =
                [|
                    topLevelMod "Fantomas.Core.AstExtensions" [ openSt "FSharp.Compiler.Text"; openSt "FSharp.Compiler.Syntax" ]
                |]
        }
        {
            Name = "AstExtensions.fs"
            Idx = 4
            Content =
                [|
                    topLevelMod
                        "Fantomas.Core.AstExtensions"
                        [
                            openSt "FSharp.Compiler.SyntaxTrivia"
                            openSt "FSharp.Compiler.Text"
                            openSt "FSharp.Compiler.Text.Range"
                            openSt "FSharp.Compiler.Syntax"
                            prefIdent "range.Zero"
                            prefIdent "h.idRange"
                            prefIdent "List.last"
                            prefIdent "ident.idRange"
                            prefIdent "IdentTrivia.OriginalNotationWithParen"
                            prefIdent "IdentTrivia.HasParenthesis"
                            prefIdent "IdentTrivia.OriginalNotation"
                            prefIdent "Range.Zero"
                            prefIdent "single.FullRange"
                            prefIdent "List.fold"
                            prefIdent "head.FullRange"
                            prefIdent "fieldName.FullRange"
                            prefIdent "expr.Range"
                            prefIdent "SynModuleOrNamespaceKind.AnonModule"
                            prefIdent "List.tryHead"
                            prefIdent "List.tryLast"
                            prefIdent "d.Range"
                            prefIdent "s.Range"
                            prefIdent "e.Range"
                            prefIdent "this.Range"
                            prefIdent "CommentTrivia.LineComment"
                            prefIdent "CommentTrivia.BlockComment"
                            prefIdent "ConditionalDirectiveTrivia.If"
                            prefIdent "ConditionalDirectiveTrivia.Else"
                            prefIdent "ConditionalDirectiveTrivia.EndIf"
                            prefIdent "List.map"
                            prefIdent "c.Range"
                            prefIdent "acc.StartLine"
                            prefIdent "triviaRange.StartLine"
                            prefIdent "acc.EndLine"
                            prefIdent "triviaRange.EndLine"
                            prefIdent "ParsedInput.ImplFile"
                            prefIdent "r.Start"
                            prefIdent "m.FullRange.Start"
                            prefIdent "Range.Zero.Start"
                            prefIdent "Range.Zero.End"
                            prefIdent "r.End"
                            prefIdent "lastModule.FullRange.End"
                            prefIdent "this.Range.FileName"
                            prefIdent "trivia.CodeComments"
                            prefIdent "trivia.ConditionalDirectives"
                            prefIdent "ParsedInput.SigFile"
                            prefIdent "SynInterpolatedStringPart.String"
                            prefIdent "SynInterpolatedStringPart.FillExpr"
                            prefIdent "i.idRange"
                            prefIdent "std.FullRange"
                            prefIdent "a.Range"
                            prefIdent "RangeHelpers.mergeRanges"
                            prefIdent "synTypar.Range"
                            prefIdent "sf.FullRange"
                            prefIdent "head.Range"
                            prefIdent "b.FullRange"
                            prefIdent "xmlDoc.IsEmpty"
                            prefIdent "xmlDoc.Range"
                            prefIdent "attributes.IsEmpty"
                            prefIdent "attributes.Head.Range"
                            prefIdent "trivia.LeadingKeyword"
                            prefIdent "SynLeadingKeyword.Member"
                            prefIdent "SynPat.LongIdent"
                            prefIdent "pat.Range"
                            prefIdent "trivia.LeadingKeyword.Range"
                        ]
                |]
        }
        {
            Name = "TriviaTypes.fs"
            Idx = 5
            Content =
                [|
                    topLevelMod "Fantomas.Core.TriviaTypes" [ openSt "FSharp.Compiler.Text"; openSt "FSharp.Compiler.Syntax" ]
                |]
        }
        {
            Name = "Utils.fs"
            Idx = 6
            Content =
                [|
                    topLevelNS
                        "Fantomas.Core"
                        [
                            openSt "System"
                            openSt "System.Text.RegularExpressions"
                            nestedModule "Char" [ prefIdent "c.ToString" ]
                            nestedModule
                                "String"
                                [
                                    prefIdent "str.Replace"
                                    prefIdent "str.StartsWith"
                                    prefIdent "StringComparison.Ordinal"
                                    prefIdent "String.Empty"
                                    prefIdent "source.Split"
                                    prefIdent "StringSplitOptions.None"
                                    prefIdent "Array.mapi"
                                    prefIdent "Regex.IsMatch"
                                    prefIdent "Array.choose"
                                    prefIdent "Array.toList"
                                    prefIdent "List.tryHead"
                                    prefIdent "List.map"
                                    prefIdent "String.concat"
                                    prefIdent "List.zip"
                                    prefIdent "String.length"
                                    prefIdent "String.IsNullOrEmpty"
                                    prefIdent "String.IsNullOrWhiteSpace"
                                    prefIdent "String.exists"
                                ]
                            nestedModule
                                "Cache"
                                [
                                    prefIdent "System.Collections.Generic.HashSet"
                                    prefIdent "HashIdentity.Reference"
                                    prefIdent "cache.Contains"
                                    prefIdent "cache.Add"
                                    prefIdent "System.Collections.Concurrent.ConcurrentDictionary"
                                    prefIdent "HashIdentity.Structural"
                                    prefIdent "cache.GetOrAdd"
                                    prefIdent "this.Equals"
                                    prefIdent "Object.ReferenceEquals"
                                    prefIdent "this.GetHashCode"
                                ]
                            nestedModule
                                "Dict"
                                [
                                    prefIdent "System.Collections.Generic.IDictionary"
                                    prefIdent "d.TryGetValue"
                                ]
                            nestedModule
                                "List"
                                [
                                    prefIdent "List.takeWhile"
                                    prefIdent "List.choose"
                                    prefIdent "List.isEmpty"
                                    prefIdent "List.rev"
                                ]
                            nestedModule "Map" [ prefIdent "Map.tryFind" ]
                            nestedModule "Async" [ prefIdent "async.Bind"; prefIdent "async.Return" ]
                            nestedModule "Continuation" []
                        ]
                |]
        }
        {
            Name = "SourceParser.fs"
            Idx = 7
            Content =
                [|
                    topLevelMod
                        "Fantomas.Core.SourceParser"
                        [
                            openSt "System"
                            openSt "FSharp.Compiler.Syntax"
                            openSt "FSharp.Compiler.Syntax.PrettyNaming"
                            openSt "FSharp.Compiler.SyntaxTrivia"
                            openSt "FSharp.Compiler.Text"
                            openSt "FSharp.Compiler.Xml"
                            openSt "Fantomas.Core"
                            openSt "Fantomas.Core.AstExtensions"
                            openSt "Fantomas.Core.TriviaTypes"
                            openSt "Fantomas.Core.RangePatterns"
                            prefIdent "SynTypar.SynTypar"
                            prefIdent "TyparStaticReq.None"
                            prefIdent "TyparStaticReq.HeadType"
                            prefIdent "SynRationalConst.Integer"
                            prefIdent "SynRationalConst.Rational"
                            prefIdent "SynRationalConst.Negate"
                            prefIdent "SynConst.Unit"
                            prefIdent "ParsedInput.ImplFile"
                            prefIdent "ParsedInput.SigFile"
                            prefIdent "ParsedImplFileInput.ParsedImplFileInput"
                            prefIdent "ParsedSigFileInput.ParsedSigFileInput"
                            prefIdent "SynModuleOrNamespace.SynModuleOrNamespace"
                            prefIdent "trivia.LeadingKeyword"
                            prefIdent "m.FullRange"
                            prefIdent "SynModuleOrNamespaceSig.SynModuleOrNamespaceSig"
                            prefIdent "a.TypeName"
                            prefIdent "a.ArgExpr"
                            prefIdent "a.Target"
                            prefIdent "px.ToXmlDoc"
                            prefIdent "xmlDoc.UnprocessedLines"
                            prefIdent "xmlDoc.Range"
                            prefIdent "SynModuleDecl.Open"
                            prefIdent "SynOpenDeclTarget.ModuleOrNamespace"
                            prefIdent "SynOpenDeclTarget.Type"
                            prefIdent "SynType.LongIdent"
                            prefIdent "SynModuleDecl.ModuleAbbrev"
                            prefIdent "SynModuleDecl.HashDirective"
                            prefIdent "SynModuleDecl.NamespaceFragment"
                            prefIdent "SynModuleDecl.Attributes"
                            prefIdent "SynModuleDecl.Let"
                            prefIdent "SynModuleDecl.Expr"
                            prefIdent "SynModuleDecl.Types"
                            prefIdent "SynModuleDecl.NestedModule"
                            prefIdent "trivia.ModuleKeyword"
                            prefIdent "trivia.EqualsRange"
                            prefIdent "SynModuleDecl.Exception"
                            prefIdent "SynModuleSigDecl.Open"
                            prefIdent "SynModuleSigDecl.ModuleAbbrev"
                            prefIdent "SynModuleSigDecl.HashDirective"
                            prefIdent "SynModuleSigDecl.NamespaceFragment"
                            prefIdent "SynModuleSigDecl.Val"
                            prefIdent "SynModuleSigDecl.Types"
                            prefIdent "SynModuleSigDecl.NestedModule"
                            prefIdent "SynModuleSigDecl.Exception"
                            prefIdent "SynExceptionDefnRepr.SynExceptionDefnRepr"
                            prefIdent "SynExceptionDefn.SynExceptionDefn"
                            prefIdent "SynExceptionSig.SynExceptionSig"
                            prefIdent "px.IsEmpty"
                            prefIdent "trivia.BarRange"
                            prefIdent "Range.unionRanges"
                            prefIdent "SynUnionCaseKind.Fields"
                            prefIdent "SynUnionCaseKind.FullType"
                            prefIdent "Option.map"
                            prefIdent "i.idRange"
                            prefIdent "t.Range"
                            prefIdent "SynMemberDefn.NestedType"
                            prefIdent "SynMemberDefn.Open"
                            prefIdent "SynMemberDefn.ImplicitInherit"
                            prefIdent "SynMemberDefn.Inherit"
                            prefIdent "SynMemberDefn.ValField"
                            prefIdent "SynMemberDefn.ImplicitCtor"
                            prefIdent "SynMemberDefn.Member"
                            prefIdent "SynMemberDefn.LetBindings"
                            prefIdent "SynType.Fun"
                            prefIdent "SynMemberKind.PropertyGet"
                            prefIdent "SynMemberKind.PropertySet"
                            prefIdent "SynMemberKind.PropertyGetSet"
                            prefIdent "SynMemberDefn.AbstractSlot"
                            prefIdent "trivia.WithKeyword"
                            prefIdent "mf.MemberKind"
                            prefIdent "SynMemberDefn.Interface"
                            prefIdent "SynMemberDefn.AutoProperty"
                            prefIdent "SynMemberDefn.GetSetMember"
                            prefIdent "SynPat.LongIdent"
                            prefIdent "Position.posLt"
                            prefIdent "getKeyword.Start"
                            prefIdent "setKeyword.Start"
                            prefIdent "SynMemberKind.ClassConstructor"
                            prefIdent "SynMemberKind.Constructor"
                            prefIdent "SynMemberKind.Member"
                            prefIdent "mf.IsInstance"
                            prefIdent "mf.IsOverrideOrExplicitImpl"
                            prefIdent "SynExpr.Typed"
                            prefIdent "RangeHelpers.rangeEq"
                            prefIdent "t1.Range"
                            prefIdent "t2.Range"
                            prefIdent "Option.bind"
                            prefIdent "trivia.ColonRange"
                            prefIdent "b.FullRange"
                            prefIdent "SynBindingKind.Do"
                            prefIdent "SynLeadingKeyword.Extern"
                            prefIdent "SynExpr.TraitCall"
                            prefIdent "SynExpr.Quote"
                            prefIdent "SynExpr.Paren"
                            prefIdent "SynExpr.Lazy"
                            prefIdent "SynExpr.InferredDowncast"
                            prefIdent "SynExpr.InferredUpcast"
                            prefIdent "SynExpr.Assert"
                            prefIdent "SynExpr.AddressOf"
                            prefIdent "SynExpr.YieldOrReturn"
                            prefIdent "SynExpr.YieldOrReturnFrom"
                            prefIdent "SynExpr.Do"
                            prefIdent "SynExpr.DoBang"
                            prefIdent "SynExpr.Fixed"
                            prefIdent "SynExpr.TypeTest"
                            prefIdent "SynExpr.Downcast"
                            prefIdent "SynExpr.Upcast"
                            prefIdent "SynExpr.While"
                            prefIdent "SynExpr.For"
                            prefIdent "SynExpr.Null"
                            prefIdent "SynExpr.Const"
                            prefIdent "SynExpr.TypeApp"
                            prefIdent "SynExpr.Match"
                            prefIdent "trivia.MatchKeyword"
                            prefIdent "SynExpr.MatchBang"
                            prefIdent "trivia.MatchBangKeyword"
                            prefIdent "SynExpr.Sequential"
                            prefIdent "SynExpr.Ident"
                            prefIdent "SynExpr.LongIdent"
                            prefIdent "SynExpr.ComputationExpr"
                            prefIdent "SynExpr.App"
                            prefIdent "ExprAtomicFlag.NonAtomic"
                            prefIdent "compExpr.Range"
                            prefIdent "SynExpr.ArrayOrListComputed"
                            prefIdent "RangeHelpers.mkStartEndRange"
                            prefIdent "SynExpr.ArrayOrList"
                            prefIdent "SynExpr.Tuple"
                            prefIdent "SynExpr.InterpolatedString"
                            prefIdent "SynExpr.IndexRange"
                            prefIdent "SynExpr.IndexFromEnd"
                            prefIdent "SynExpr.Typar"
                            prefIdent "SynConst.Double"
                            prefIdent "SynConst.Decimal"
                            prefIdent "SynConst.Single"
                            prefIdent "SynConst.Int16"
                            prefIdent "SynConst.Int32"
                            prefIdent "SynConst.Int64"
                            prefIdent "List.moreThanOne"
                            prefIdent "SynExpr.Dynamic"
                            prefIdent "IdentTrivia.OriginalNotationWithParen"
                            prefIdent "originalNotation.Length"
                            prefIdent "originalNotation.StartsWith"
                            prefIdent "List.rev"
                            prefIdent "SynExpr.DotGet"
                            prefIdent "SynExpr.Lambda"
                            prefIdent "SynExpr.MatchLambda"
                            prefIdent "SynExpr.New"
                            prefIdent "IdentTrivia.OriginalNotation"
                            prefIdent "ident.idText"
                            prefIdent "newLineInfixOps.Contains"
                            prefIdent "List.length"
                            prefIdent "SynExpr.JoinIn"
                            prefIdent "SynExpr.LetOrUse"
                            prefIdent "xs.Length"
                            prefIdent "List.mapi"
                            prefIdent "trivia.InKeyword"
                            prefIdent "List.map"
                            prefIdent "SynExpr.LetOrUseBang"
                            prefIdent "List.collect"
                            prefIdent "Continuation.sequence"
                            prefIdent "SynExpr.ForEach"
                            prefIdent "SynExpr.DotIndexedSet"
                            prefIdent "SynExpr.NamedIndexedPropertySet"
                            prefIdent "SynExpr.DotNamedIndexedPropertySet"
                            prefIdent "SynExpr.DotIndexedGet"
                            prefIdent "SynExpr.DotSet"
                            prefIdent "SynExpr.IfThenElse"
                            prefIdent "trivia.IfKeyword"
                            prefIdent "trivia.IsElif"
                            prefIdent "trivia.ThenKeyword"
                            prefIdent "trivia.ElseKeyword"
                            prefIdent "unitRange.StartColumn"
                            prefIdent "unitRange.EndColumn"
                            prefIdent "SynExpr.Record"
                            prefIdent "SynExpr.AnonRecd"
                            prefIdent "SynExpr.ObjExpr"
                            prefIdent "SynExpr.LongIdentSet"
                            prefIdent "SynExpr.TryWith"
                            prefIdent "trivia.TryKeyword"
                            prefIdent "SynExpr.TryFinally"
                            prefIdent "trivia.FinallyKeyword"
                            prefIdent "SynExpr.ArbitraryAfterError"
                            prefIdent "SynExpr.FromParseError"
                            prefIdent "SynExpr.DiscardAfterMissingQualificationAfterDot"
                            prefIdent "SynExpr.LibraryOnlyILAssembly"
                            prefIdent "SynExpr.LibraryOnlyStaticOptimization"
                            prefIdent "FSharp.Core"
                            prefIdent "SynExpr.LibraryOnlyUnionCaseFieldGet"
                            prefIdent "SynExpr.LibraryOnlyUnionCaseFieldSet"
                            prefIdent "SynPat.OptionalVal"
                            prefIdent "SynPat.Attrib"
                            prefIdent "SynPat.Or"
                            prefIdent "p.Range"
                            prefIdent "SynPat.Ands"
                            prefIdent "SynPat.Null"
                            prefIdent "SynPat.Wild"
                            prefIdent "SynPat.Tuple"
                            prefIdent "SynPat.ArrayOrList"
                            prefIdent "SynPat.Typed"
                            prefIdent "SynPat.Named"
                            prefIdent "SynPat.As"
                            prefIdent "SynArgPats.NamePatPairs"
                            prefIdent "SynArgPats.Pats"
                            prefIdent "SynPat.ListCons"
                            prefIdent "trivia.ColonColonRange"
                            prefIdent "synLongIdent.IdentsWithTrivia"
                            prefIdent "synIdent.FullRange"
                            prefIdent "synLongIdent.FullRange"
                            prefIdent "SynPat.Paren"
                            prefIdent "SynPat.Record"
                            prefIdent "SynPat.Const"
                            prefIdent "SynPat.IsInst"
                            prefIdent "SynPat.QuoteExpr"
                            prefIdent "newIdent.idText"
                            prefIdent "pat.Range"
                            prefIdent "SynSimplePats.SimplePats"
                            prefIdent "SynSimplePats.Typed"
                            prefIdent "SynSimplePat.Attrib"
                            prefIdent "SynSimplePat.Id"
                            prefIdent "SynSimplePat.Typed"
                            prefIdent "trivia.ArrowRange"
                            prefIdent "SynMatchClause.SynMatchClause"
                            prefIdent "matchRange.Start"
                            prefIdent "clause.Range.Start"
                            prefIdent "me.Range"
                            prefIdent "SynTypeDefnSimpleRepr.Enum"
                            prefIdent "SynTypeDefnSimpleRepr.Union"
                            prefIdent "SynTypeDefnSimpleRepr.Record"
                            prefIdent "SynTypeDefnSimpleRepr.None"
                            prefIdent "SynTypeDefnSimpleRepr.TypeAbbrev"
                            prefIdent "SynTypeDefnSimpleRepr.General"
                            prefIdent "SynTypeDefnSimpleRepr.LibraryOnlyILAssembly"
                            prefIdent "SynTypeDefnSimpleRepr.Exception"
                            prefIdent "SynTypeDefnRepr.Simple"
                            prefIdent "SynTypeDefnRepr.ObjectModel"
                            prefIdent "SynTypeDefnRepr.Exception"
                            prefIdent "List.tryFind"
                            prefIdent "List.filter"
                            prefIdent "SynTypeDefnSigRepr.Simple"
                            prefIdent "SynTypeDefnSigRepr.ObjectModel"
                            prefIdent "SynTypeDefnSigRepr.Exception"
                            prefIdent "SynTypeDefnKind.Unspecified"
                            prefIdent "SynTypeDefnKind.Class"
                            prefIdent "SynTypeDefnKind.Interface"
                            prefIdent "SynTypeDefnKind.Struct"
                            prefIdent "SynTypeDefnKind.Record"
                            prefIdent "SynTypeDefnKind.Union"
                            prefIdent "SynTypeDefnKind.Abbrev"
                            prefIdent "SynTypeDefnKind.Opaque"
                            prefIdent "SynTypeDefnKind.Augmentation"
                            prefIdent "SynTypeDefnKind.IL"
                            prefIdent "SynTypeDefnKind.Delegate"
                            prefIdent "std.FullRange"
                            prefIdent "SynTyparDecls.PostfixList"
                            prefIdent "SynType.HashConstraint"
                            prefIdent "SynType.MeasurePower"
                            prefIdent "SynType.MeasureDivide"
                            prefIdent "SynType.StaticConstant"
                            prefIdent "SynType.StaticConstantExpr"
                            prefIdent "SynType.StaticConstantNamed"
                            prefIdent "SynType.Array"
                            prefIdent "SynType.Anon"
                            prefIdent "SynType.Var"
                            prefIdent "SynType.App"
                            prefIdent "SynType.LongIdentApp"
                            prefIdent "SynType.Tuple"
                            prefIdent "SynType.WithGlobalConstraints"
                            prefIdent "SynType.AnonRecd"
                            prefIdent "SynType.Paren"
                            prefIdent "SynType.SignatureParameter"
                            prefIdent "SynType.Or"
                            prefIdent "trivia.OrKeyword"
                            prefIdent "lid.idText"
                            prefIdent "x.ToString"
                            prefIdent "SynTypeConstraint.WhereTyparIsValueType"
                            prefIdent "SynTypeConstraint.WhereTyparIsReferenceType"
                            prefIdent "SynTypeConstraint.WhereTyparIsUnmanaged"
                            prefIdent "SynTypeConstraint.WhereTyparSupportsNull"
                            prefIdent "SynTypeConstraint.WhereTyparIsComparable"
                            prefIdent "SynTypeConstraint.WhereTyparIsEquatable"
                            prefIdent "SynTypeConstraint.WhereTyparDefaultsToType"
                            prefIdent "SynTypeConstraint.WhereTyparSubtypeOfType"
                            prefIdent "SynTypeConstraint.WhereTyparSupportsMember"
                            prefIdent "SynTypeConstraint.WhereTyparIsEnum"
                            prefIdent "SynTypeConstraint.WhereTyparIsDelegate"
                            prefIdent "SynTypeConstraint.WhereSelfConstrained"
                            prefIdent "SynMemberSig.Member"
                            prefIdent "SynMemberSig.Interface"
                            prefIdent "SynMemberSig.Inherit"
                            prefIdent "SynMemberSig.ValField"
                            prefIdent "SynMemberSig.NestedType"
                            prefIdent "ident.idRange"
                            prefIdent "e.Range"
                            prefIdent "List.tryLast"
                            prefIdent "IdentTrivia.HasParenthesis"
                            prefIdent "lp.idText"
                            prefIdent "Seq.tryHead"
                            prefIdent "Char.IsUpper"
                            prefIdent "Option.defaultValue"
                            prefIdent "ExprAtomicFlag.Atomic"
                            prefIdent "RangeHelpers.isAdjacentTo"
                            prefIdent "identifierExpr.Range"
                            prefIdent "argExpr.Range"
                            prefIdent "Seq.toList"
                            prefIdent "Seq.singleton"
                            prefIdent "List.exists"
                        ]
                |]
        }
    |]

let dictionary<'key, 'value when 'key: equality> (entries: ('key * 'value) seq) =
    entries |> Seq.map (fun (k, v) -> KeyValuePair(k, v)) |> Dictionary

let noChildren = Dictionary(0)
let emptyHS () = HashSet(0)

let indexOf name =
    Array.find (fun (fc: FileContent) -> fc.Name = name) files |> fun fc -> fc.Idx

// This should be constructed from the AST
let fantomasCoreTrie: TrieNode =
    {
        Current = TrieNodeInfo.Root
        Children =
            dictionary
                [|
                    "System",
                    {
                        Current = TrieNodeInfo.Namespace("System", emptyHS ())
                        Children =
                            dictionary
                                [|
                                    "AssemblyVersionInformation",
                                    {
                                        Current = TrieNodeInfo.Module("AssemblyVersionInformation", indexOf "AssemblyInfo.fs")
                                        Children = noChildren
                                    }
                                |]
                    }
                    "Fantomas",
                    {
                        Current = TrieNodeInfo.Namespace("Fantomas", emptyHS ())
                        Children =
                            dictionary
                                [|
                                    "Core",
                                    {
                                        Current = TrieNodeInfo.Namespace("Core", emptyHS ())
                                        Children =
                                            dictionary
                                                [|
                                                    "ISourceTextExtensions",
                                                    {
                                                        Current =
                                                            TrieNodeInfo.Module("ISourceTextExtensions", indexOf "ISourceTextExtensions.fs")
                                                        Children = noChildren
                                                    }
                                                    "RangeHelpers",
                                                    {
                                                        Current = TrieNodeInfo.Module("RangeHelpers", indexOf "RangeHelpers.fs")
                                                        Children = noChildren
                                                    }
                                                    "RangePatterns",
                                                    {
                                                        Current = TrieNodeInfo.Module("RangePatterns", indexOf "RangeHelpers.fs")
                                                        Children = noChildren
                                                    }
                                                    "AstExtensions",
                                                    {
                                                        Current = TrieNodeInfo.Module("AstExtensions", indexOf "AstExtensions.fsi")
                                                        Children = noChildren
                                                    }
                                                    "TriviaTypes",
                                                    {
                                                        Current = TrieNodeInfo.Module("TriviaTypes", indexOf "TriviaTypes.fs")
                                                        Children = noChildren
                                                    }
                                                    "Char",
                                                    {
                                                        Current = TrieNodeInfo.Module("Char", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "String",
                                                    {
                                                        Current = TrieNodeInfo.Module("String", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "Cache",
                                                    {
                                                        Current = TrieNodeInfo.Module("Cache", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "Dict",
                                                    {
                                                        Current = TrieNodeInfo.Module("Dict", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "List",
                                                    {
                                                        Current = TrieNodeInfo.Module("List", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "Map",
                                                    {
                                                        Current = TrieNodeInfo.Module("Map", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "Async",
                                                    {
                                                        Current = TrieNodeInfo.Module("Async", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "Continuation",
                                                    {
                                                        Current = TrieNodeInfo.Module("Continuation", indexOf "Utils.fs")
                                                        Children = noChildren
                                                    }
                                                    "SourceParser",
                                                    {
                                                        Current = TrieNodeInfo.Module("SourceParser", indexOf "SourceParser.fs")
                                                        Children = noChildren
                                                    }
                                                |]
                                    }
                                |]
                    }
                |]
    }

[<Test>]
let ``Full project simulation`` () =
    let graph =
        files
        |> Array.map (fun fileContent ->
            let knownFiles =
                files.[0 .. (fileContent.Idx - 1)] |> Array.map (fun f -> f.Idx) |> set

            let queryTrie: QueryTrie = queryTrieMemoized fantomasCoreTrie

            let result =
                Seq.fold (processStateEntry queryTrie) (FileContentQueryState.Create fileContent.Idx knownFiles) fileContent.Content

            fileContent.Name, Set.toArray result.FoundDependencies)

    for fileName, deps in graph do
        let depString =
            deps |> Array.map (fun depIdx -> files.[depIdx].Name) |> String.concat ", "

        printfn $"%s{fileName}: [{depString}]"

[<Test>]
let ``Query non existing node in trie`` () =
    let result =
        queryTrie fantomasCoreTrie [ "System"; "System"; "Runtime"; "CompilerServices" ]

    match result with
    | QueryTrieNodeResult.NodeDoesNotExist -> Assert.Pass()
    | result -> Assert.Fail $"Unexpected result: %A{result}"

[<Test>]
let ``Query node that does not expose data in trie`` () =
    let result = queryTrie fantomasCoreTrie [ "Fantomas"; "Core" ]

    match result with
    | QueryTrieNodeResult.NodeDoesNotExposeData -> Assert.Pass()
    | result -> Assert.Fail $"Unexpected result: %A{result}"

[<Test>]
let ``Query module node that exposes one file`` () =
    let result =
        queryTrie fantomasCoreTrie [ "Fantomas"; "Core"; "ISourceTextExtensions" ]

    match result with
    | QueryTrieNodeResult.NodeExposesData file ->
        let file = Seq.exactlyOne file
        Assert.AreEqual(indexOf "ISourceTextExtensions.fs", file)
    | result -> Assert.Fail $"Unexpected result: %A{result}"

[<Test>]
let ``ProcessOpenStatement full path match`` () =
    let sourceParser =
        Array.find (fun (f: FileContent) -> f.Name = "SourceParser.fs") files

    let state =
        FileContentQueryState.Create
            sourceParser.Idx
            (set
                [|
                    indexOf "AssemblyInfo.fs"
                    indexOf "ISourceTextExtensions.fs"
                    indexOf "RangeHelpers.fs"
                    indexOf "AstExtensions.fsi"
                    indexOf "TriviaTypes.fs"
                    indexOf "Utils.fs"
                |])

    let result =
        processOpenPath (queryTrie fantomasCoreTrie) [ "Fantomas"; "Core"; "AstExtensions" ] state

    let dep = Seq.exactlyOne result.FoundDependencies
    Assert.AreEqual(indexOf "AstExtensions.fsi", dep)

#if INTERACTIVE
open System.Text.RegularExpressions

let fileContent =
    System.IO.File.ReadAllText(@"C:\Users\nojaf\Projects\main-fantomas\src\Fantomas.Core\SourceParser.fs")

Regex.Matches(fileContent, "(\\w)+(\\.(\\w)+)+")
|> Seq.cast<Match>
|> Seq.distinctBy (fun m -> m.Value)
|> Seq.iter (fun m -> printfn "prefIdent \"%s\"" m.Value)
#endif
