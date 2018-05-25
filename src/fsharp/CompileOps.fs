// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Coordinating compiler operations - configuration, loading initial context, reporting errors etc.
module internal Microsoft.FSharp.Compiler.CompileOps

open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Text

open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Filename
open Internal.Utilities.Text

open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.ILBinaryReader
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.AttributeChecking
open Microsoft.FSharp.Compiler.ConstraintSolver
open Microsoft.FSharp.Compiler.DiagnosticMessage
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Lexhelp
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.MethodCalls
open Microsoft.FSharp.Compiler.MethodOverrides
open Microsoft.FSharp.Compiler.NameResolution
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.ReferenceResolver
open Microsoft.FSharp.Compiler.SignatureConformance
open Microsoft.FSharp.Compiler.TastPickle
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.TcGlobals

#if !NO_EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
open Microsoft.FSharp.Core.CompilerServices
#endif

#if FX_RESHAPED_REFLECTION
open Microsoft.FSharp.Core.ReflectionAdapters
#endif

#if DEBUG
[<AutoOpen>]
module internal CompilerService =
    let showAssertForUnexpectedException = ref true
#endif // DEBUG

//----------------------------------------------------------------------------
// Some Globals
//--------------------------------------------------------------------------

let FSharpSigFileSuffixes = [".mli";".fsi"]
let mlCompatSuffixes = [".mli";".ml"]
let FSharpImplFileSuffixes = [".ml";".fs";".fsscript";".fsx"]
let resSuffixes = [".resx"]
let FSharpScriptFileSuffixes = [".fsscript";".fsx"]
let doNotRequireNamespaceOrModuleSuffixes = [".mli";".ml"] @ FSharpScriptFileSuffixes
let FSharpLightSyntaxFileSuffixes : string list = [ ".fs";".fsscript";".fsx";".fsi" ]


//----------------------------------------------------------------------------
// ERROR REPORTING
//--------------------------------------------------------------------------

exception HashIncludeNotAllowedInNonScript of range
exception HashReferenceNotAllowedInNonScript of range
exception HashDirectiveNotAllowedInNonScript of range
exception FileNameNotResolved of (*filename*) string * (*description of searched locations*) string * range
exception AssemblyNotResolved of (*originalName*) string * range
exception LoadedSourceNotFoundIgnoring of (*filename*) string * range
exception MSBuildReferenceResolutionWarning of (*MSBuild warning code*)string * (*Message*)string * range
exception MSBuildReferenceResolutionError of (*MSBuild warning code*)string * (*Message*)string * range
exception DeprecatedCommandLineOptionFull of string * range
exception DeprecatedCommandLineOptionForHtmlDoc of string * range
exception DeprecatedCommandLineOptionSuggestAlternative of string * string * range
exception DeprecatedCommandLineOptionNoDescription of string * range
exception InternalCommandLineOption of string * range
exception HashLoadedSourceHasIssues of (*warnings*) exn list * (*errors*) exn list * range
exception HashLoadedScriptConsideredSource of range


let GetRangeOfDiagnostic(err:PhasedDiagnostic) = 
  let rec RangeFromException = function
      | ErrorFromAddingConstraint(_, err2, _) -> RangeFromException err2 
#if !NO_EXTENSIONTYPING
      | ExtensionTyping.ProvidedTypeResolutionNoRange(e) -> RangeFromException e
      | ExtensionTyping.ProvidedTypeResolution(m, _)
#endif
      | ReservedKeyword(_, m)
      | IndentationProblem(_, m)
      | ErrorFromAddingTypeEquation(_, _, _, _, _, m) 
      | ErrorFromApplyingDefault(_, _, _, _, _, m) 
      | ErrorsFromAddingSubsumptionConstraint(_, _, _, _, _, _, m)
      | FunctionExpected(_, _, m)
      | BakedInMemberConstraintName(_, m)
      | StandardOperatorRedefinitionWarning(_, m)
      | BadEventTransformation(m)
      | ParameterlessStructCtor(m)
      | FieldNotMutable (_, _, m) 
      | Recursion (_, _, _, _, m) 
      | InvalidRuntimeCoercion(_, _, _, m) 
      | IndeterminateRuntimeCoercion(_, _, _, m)
      | IndeterminateStaticCoercion (_, _, _, m)
      | StaticCoercionShouldUseBox (_, _, _, m)
      | CoercionTargetSealed(_, _, m)
      | UpcastUnnecessary(m)
      | QuotationTranslator.IgnoringPartOfQuotedTermWarning (_, m) 
      
      | TypeTestUnnecessary(m)
      | RuntimeCoercionSourceSealed(_, _, m)
      | OverrideDoesntOverride(_, _, _, _, _, m)
      | UnionPatternsBindDifferentNames m 
      | UnionCaseWrongArguments (_, _, _, m) 
      | TypeIsImplicitlyAbstract m 
      | RequiredButNotSpecified (_, _, _, _, m) 
      | FunctionValueUnexpected (_, _, m)
      | UnitTypeExpected (_, _, m)
      | UnitTypeExpectedWithEquality (_, _, m)
      | UnitTypeExpectedWithPossiblePropertySetter (_, _, _, _, m)
      | UnitTypeExpectedWithPossibleAssignment (_, _, _, _, m)
      | UseOfAddressOfOperator m 
      | DeprecatedThreadStaticBindingWarning(m) 
      | NonUniqueInferredAbstractSlot (_, _, _, _, _, m) 
      | DefensiveCopyWarning (_, m)
      | LetRecCheckedAtRuntime m 
      | UpperCaseIdentifierInPattern m
      | NotUpperCaseConstructor m
      | RecursiveUseCheckedAtRuntime (_, _, m) 
      | LetRecEvaluatedOutOfOrder (_, _, _, m) 
      | Error (_, m)
      | ErrorWithSuggestions (_, m, _, _)
      | NumberedError (_, m)
      | SyntaxError (_, m) 
      | InternalError (_, m)
      | FullAbstraction(_, m)
      | InterfaceNotRevealed(_, _, m) 
      | WrappedError (_, m)
      | PatternMatchCompilation.MatchIncomplete (_, _, m)
      | PatternMatchCompilation.EnumMatchIncomplete (_, _, m)
      | PatternMatchCompilation.RuleNeverMatched m 
      | ValNotMutable(_, _, m)
      | ValNotLocal(_, _, m) 
      | MissingFields(_, m) 
      | OverrideInIntrinsicAugmentation(m)
      | IntfImplInIntrinsicAugmentation(m) 
      | OverrideInExtrinsicAugmentation(m)
      | IntfImplInExtrinsicAugmentation(m) 
      | ValueRestriction(_, _, _, _, m) 
      | LetRecUnsound (_, _, m) 
      | ObsoleteError (_, m) 
      | ObsoleteWarning (_, m) 
      | Experimental (_, m) 
      | PossibleUnverifiableCode m
      | UserCompilerMessage (_, _, m) 
      | Deprecated(_, m) 
      | LibraryUseOnly(m) 
      | FieldsFromDifferentTypes (_, _, _, m) 
      | IndeterminateType(m)
      | TyconBadArgs(_, _, _, m) -> 
          Some m

      | FieldNotContained(_, arf, _, _) -> Some arf.Range
      | ValueNotContained(_, _, aval, _, _) -> Some aval.Range
      | ConstrNotContained(_, aval, _, _) -> Some aval.Id.idRange
      | ExnconstrNotContained(_, aexnc, _, _) -> Some aexnc.Range

      | VarBoundTwice(id) 
      | UndefinedName(_, _, id, _) -> 
          Some id.idRange 

      | Duplicate(_, _, m) 
      | NameClash(_, _, _, m, _, _, _) 
      | UnresolvedOverloading(_, _, _, m) 
      | UnresolvedConversionOperator (_, _, _, m)
      | PossibleOverload(_, _, _, m) 
      | VirtualAugmentationOnNullValuedType(m)
      | NonVirtualAugmentationOnNullValuedType(m)
      | NonRigidTypar(_, _, _, _, _, m)
      | ConstraintSolverTupleDiffLengths(_, _, _, m, _) 
      | ConstraintSolverInfiniteTypes(_, _, _, _, m, _) 
      | ConstraintSolverMissingConstraint(_, _, _, m, _) 
      | ConstraintSolverTypesNotInEqualityRelation(_, _, _, m, _, _)
      | ConstraintSolverError(_, m, _) 
      | ConstraintSolverTypesNotInSubsumptionRelation(_, _, _, m, _) 
      | ConstraintSolverRelatedInformation(_, m, _) 
      | SelfRefObjCtor(_, m) -> 
          Some m

      | NotAFunction(_, _, mfun, _) -> 
          Some mfun
          
      | NotAFunctionButIndexer(_, _, _, mfun, _) -> 
          Some mfun

      | IllegalFileNameChar(_) -> Some rangeCmdArgs

      | UnresolvedReferenceError(_, m) 
      | UnresolvedPathReference(_, _, m) 
      | DeprecatedCommandLineOptionFull(_, m) 
      | DeprecatedCommandLineOptionForHtmlDoc(_, m) 
      | DeprecatedCommandLineOptionSuggestAlternative(_, _, m) 
      | DeprecatedCommandLineOptionNoDescription(_, m) 
      | InternalCommandLineOption(_, m)
      | HashIncludeNotAllowedInNonScript(m)
      | HashReferenceNotAllowedInNonScript(m) 
      | HashDirectiveNotAllowedInNonScript(m)  
      | FileNameNotResolved(_, _, m) 
      | LoadedSourceNotFoundIgnoring(_, m) 
      | MSBuildReferenceResolutionWarning(_, _, m) 
      | MSBuildReferenceResolutionError(_, _, m) 
      | AssemblyNotResolved(_, m) 
      | HashLoadedSourceHasIssues(_, _, m) 
      | HashLoadedScriptConsideredSource(m) -> 
          Some m
      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          RangeFromException e.InnerException
#if !NO_EXTENSIONTYPING
      | :? TypeProviderError as e -> e.Range |> Some
#endif
      
      | _ -> None
  
  RangeFromException err.Exception

let GetDiagnosticNumber(err:PhasedDiagnostic) = 
   let rec GetFromException(e:exn) = 
      match e with
      (* DO NOT CHANGE THESE NUMBERS *)
      | ErrorFromAddingTypeEquation _ -> 1
      | FunctionExpected _ -> 2
      | NotAFunctionButIndexer _ -> 3217
      | NotAFunction _  -> 3
      | FieldNotMutable  _ -> 5
      | Recursion _ -> 6
      | InvalidRuntimeCoercion _ -> 7
      | IndeterminateRuntimeCoercion _ -> 8
      | PossibleUnverifiableCode _ -> 9
      | SyntaxError _ -> 10
      // 11 cannot be reused
      // 12 cannot be reused
      | IndeterminateStaticCoercion  _ -> 13
      | StaticCoercionShouldUseBox  _ -> 14
      // 15 cannot be reused
      | RuntimeCoercionSourceSealed _ -> 16 
      | OverrideDoesntOverride _ -> 17
      | UnionPatternsBindDifferentNames _  -> 18
      | UnionCaseWrongArguments  _ -> 19
      | UnitTypeExpected _  -> 20
      | UnitTypeExpectedWithEquality _  -> 20
      | UnitTypeExpectedWithPossiblePropertySetter _  -> 20
      | UnitTypeExpectedWithPossibleAssignment _  -> 20
      | RecursiveUseCheckedAtRuntime  _ -> 21
      | LetRecEvaluatedOutOfOrder  _ -> 22
      | NameClash _ -> 23
      // 24 cannot be reused
      | PatternMatchCompilation.MatchIncomplete _ -> 25
      | PatternMatchCompilation.RuleNeverMatched _ -> 26
      | ValNotMutable _ -> 27
      | ValNotLocal _ -> 28
      | MissingFields _ -> 29
      | ValueRestriction _ -> 30
      | LetRecUnsound  _ -> 31
      | FieldsFromDifferentTypes  _ -> 32
      | TyconBadArgs _ -> 33
      | ValueNotContained _ -> 34
      | Deprecated  _ -> 35
      | ConstrNotContained _ -> 36
      | Duplicate _ -> 37
      | VarBoundTwice _  -> 38
      | UndefinedName _ -> 39
      | LetRecCheckedAtRuntime _ -> 40
      | UnresolvedOverloading _ -> 41
      | LibraryUseOnly _ -> 42
      | ErrorFromAddingConstraint _ -> 43
      | ObsoleteWarning _ -> 44
      | FullAbstraction _ -> 45
      | ReservedKeyword _ -> 46
      | SelfRefObjCtor _ -> 47
      | VirtualAugmentationOnNullValuedType _ -> 48
      | UpperCaseIdentifierInPattern _ -> 49
      | InterfaceNotRevealed _ -> 50
      | UseOfAddressOfOperator _ -> 51
      | DefensiveCopyWarning _ -> 52
      | NotUpperCaseConstructor _ -> 53
      | TypeIsImplicitlyAbstract _ -> 54
      // 55 cannot be reused
      | DeprecatedThreadStaticBindingWarning _ -> 56
      | Experimental _ -> 57
      | IndentationProblem _ -> 58
      | CoercionTargetSealed _ -> 59 
      | OverrideInIntrinsicAugmentation _ -> 60
      | NonVirtualAugmentationOnNullValuedType _ -> 61
      | UserCompilerMessage (_, n, _) -> n
      | ExnconstrNotContained _ -> 63
      | NonRigidTypar _ -> 64
      // 65 cannot be reused
      | UpcastUnnecessary _ -> 66
      | TypeTestUnnecessary _ -> 67
      | QuotationTranslator.IgnoringPartOfQuotedTermWarning _ -> 68
      | IntfImplInIntrinsicAugmentation _ -> 69
      | NonUniqueInferredAbstractSlot _ -> 70
      | ErrorFromApplyingDefault _ -> 71
      | IndeterminateType _ -> 72
      | InternalError _ -> 73
      | UnresolvedReferenceNoRange _
      | UnresolvedReferenceError _ 
      | UnresolvedPathReferenceNoRange _ 
      | UnresolvedPathReference _ -> 74
      | DeprecatedCommandLineOptionFull _
      | DeprecatedCommandLineOptionForHtmlDoc _
      | DeprecatedCommandLineOptionSuggestAlternative _
      | DeprecatedCommandLineOptionNoDescription _ 
      | InternalCommandLineOption _ -> 75
      | HashIncludeNotAllowedInNonScript _ 
      | HashReferenceNotAllowedInNonScript _ 
      | HashDirectiveNotAllowedInNonScript _ -> 76
      | BakedInMemberConstraintName _ -> 77
      | FileNameNotResolved _ -> 78  
      | LoadedSourceNotFoundIgnoring _ -> 79
      // 80 cannot be reused
      | ParameterlessStructCtor _ -> 81
      | MSBuildReferenceResolutionWarning _ -> 82
      | MSBuildReferenceResolutionError _ -> 83
      | AssemblyNotResolved _ -> 84
      | HashLoadedSourceHasIssues _ -> 85
      | StandardOperatorRedefinitionWarning _ -> 86
      | InvalidInternalsVisibleToAssemblyName _ -> 87
      // 88 cannot be reused
      | OverrideInExtrinsicAugmentation _ -> 89
      | IntfImplInExtrinsicAugmentation _ -> 90
      | BadEventTransformation _ -> 91
      | HashLoadedScriptConsideredSource _ -> 92
      | UnresolvedConversionOperator _ -> 93
      // avoid 94-100 for safety
      | ObsoleteError _ -> 101
#if !NO_EXTENSIONTYPING
      | ExtensionTyping.ProvidedTypeResolutionNoRange _
      | ExtensionTyping.ProvidedTypeResolution _ -> 103
#endif
      | PatternMatchCompilation.EnumMatchIncomplete _ -> 104
       (* DO NOT CHANGE THE NUMBERS *)

      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          GetFromException e.InnerException
      
      | WrappedError(e, _) -> GetFromException e   

      | Error ((n, _), _) -> n
      | ErrorWithSuggestions ((n, _), _, _, _) -> n
      | Failure _ -> 192
      | NumberedError((n, _), _) -> n
      | IllegalFileNameChar(fileName, invalidChar) -> fst (FSComp.SR.buildUnexpectedFileNameCharacter(fileName, string invalidChar))
#if !NO_EXTENSIONTYPING
      | :? TypeProviderError as e -> e.Number
#endif
      | ErrorsFromAddingSubsumptionConstraint (_, _, _, _, _, ContextInfo.DowncastUsedInsteadOfUpcast _, _) -> fst (FSComp.SR.considerUpcast("", ""))
      | _ -> 193
   GetFromException err.Exception
   
let GetWarningLevel err = 
  match err.Exception with 
  // Level 5 warnings
  | RecursiveUseCheckedAtRuntime _
  | LetRecEvaluatedOutOfOrder  _
  | DefensiveCopyWarning _
  | FullAbstraction _ ->  5
  | NumberedError((n, _), _) 
  | ErrorWithSuggestions((n, _), _, _, _) 
  | Error((n, _), _) -> 
      // 1178, tcNoComparisonNeeded1, "The struct, record or union type '%s' is not structurally comparable because the type parameter %s does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to this type to clarify that the type is not comparable"
      // 1178, tcNoComparisonNeeded2, "The struct, record or union type '%s' is not structurally comparable because the type '%s' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to this type to clarify that the type is not comparable" 
      // 1178, tcNoEqualityNeeded1, "The struct, record or union type '%s' does not support structural equality because the type parameter %s does not satisfy the 'equality' constraint. Consider adding the 'NoEquality' attribute to this type to clarify that the type does not support structural equality"
      // 1178, tcNoEqualityNeeded2, "The struct, record or union type '%s' does not support structural equality because the type '%s' does not satisfy the 'equality' constraint. Consider adding the 'NoEquality' attribute to this type to clarify that the type does not support structural equality"
      if (n = 1178) then 5 else 2
  // Level 2 
  | _ ->  2

let warningOn err level specificWarnOn = 
    let n = GetDiagnosticNumber err
    List.contains n specificWarnOn ||
    // Some specific warnings are never on by default, i.e. unused variable warnings
    match n with 
    | 1182 -> false // chkUnusedValue - off by default
    | 3218 -> false // ArgumentsInSigAndImplMismatch - off by default
    | 3180 -> false // abImplicitHeapAllocation - off by default
    | _ -> level >= GetWarningLevel err 

let SplitRelatedDiagnostics(err:PhasedDiagnostic) = 
    let ToPhased(e) = {Exception=e; Phase = err.Phase}
    let rec SplitRelatedException = function
      | UnresolvedOverloading(a, overloads, b, c) -> 
           let related = overloads |> List.map ToPhased
           UnresolvedOverloading(a, [], b, c)|>ToPhased, related
      | ConstraintSolverRelatedInformation(fopt, m2, e) -> 
          let e, related = SplitRelatedException e
          ConstraintSolverRelatedInformation(fopt, m2, e.Exception)|>ToPhased, related
      | ErrorFromAddingTypeEquation(g, denv, t1, t2, e, m) ->
          let e, related = SplitRelatedException e
          ErrorFromAddingTypeEquation(g, denv, t1, t2, e.Exception, m)|>ToPhased, related
      | ErrorFromApplyingDefault(g, denv, tp, defaultType, e, m) ->  
          let e, related = SplitRelatedException e
          ErrorFromApplyingDefault(g, denv, tp, defaultType, e.Exception, m)|>ToPhased, related
      | ErrorsFromAddingSubsumptionConstraint(g, denv, t1, t2, e, contextInfo, m) ->  
          let e, related = SplitRelatedException e
          ErrorsFromAddingSubsumptionConstraint(g, denv, t1, t2, e.Exception, contextInfo, m)|>ToPhased, related
      | ErrorFromAddingConstraint(x, e, m) ->  
          let e, related = SplitRelatedException e
          ErrorFromAddingConstraint(x, e.Exception, m)|>ToPhased, related
      | WrappedError (e, m) -> 
          let e, related = SplitRelatedException e
          WrappedError(e.Exception, m)|>ToPhased, related
      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          SplitRelatedException e.InnerException
      | e -> 
           ToPhased(e), []
    SplitRelatedException(err.Exception)


let DeclareMesssage = Microsoft.FSharp.Compiler.DiagnosticMessage.DeclareResourceString

do FSComp.SR.RunStartupValidation()
let SeeAlsoE() = DeclareResourceString("SeeAlso", "%s")
let ConstraintSolverTupleDiffLengthsE() = DeclareResourceString("ConstraintSolverTupleDiffLengths", "%d%d")
let ConstraintSolverInfiniteTypesE() = DeclareResourceString("ConstraintSolverInfiniteTypes", "%s%s")
let ConstraintSolverMissingConstraintE() = DeclareResourceString("ConstraintSolverMissingConstraint", "%s")
let ConstraintSolverTypesNotInEqualityRelation1E() = DeclareResourceString("ConstraintSolverTypesNotInEqualityRelation1", "%s%s")
let ConstraintSolverTypesNotInEqualityRelation2E() = DeclareResourceString("ConstraintSolverTypesNotInEqualityRelation2", "%s%s")
let ConstraintSolverTypesNotInSubsumptionRelationE() = DeclareResourceString("ConstraintSolverTypesNotInSubsumptionRelation", "%s%s%s")
let ErrorFromAddingTypeEquation1E() = DeclareResourceString("ErrorFromAddingTypeEquation1", "%s%s%s")
let ErrorFromAddingTypeEquation2E() = DeclareResourceString("ErrorFromAddingTypeEquation2", "%s%s%s")
let ErrorFromApplyingDefault1E() = DeclareResourceString("ErrorFromApplyingDefault1", "%s")
let ErrorFromApplyingDefault2E() = DeclareResourceString("ErrorFromApplyingDefault2", "")
let ErrorsFromAddingSubsumptionConstraintE() = DeclareResourceString("ErrorsFromAddingSubsumptionConstraint", "%s%s%s")
let UpperCaseIdentifierInPatternE() = DeclareResourceString("UpperCaseIdentifierInPattern", "")
let NotUpperCaseConstructorE() = DeclareResourceString("NotUpperCaseConstructor", "")
let PossibleOverloadE() = DeclareResourceString("PossibleOverload", "%s%s")
let FunctionExpectedE() = DeclareResourceString("FunctionExpected", "")
let BakedInMemberConstraintNameE() = DeclareResourceString("BakedInMemberConstraintName", "%s")
let BadEventTransformationE() = DeclareResourceString("BadEventTransformation", "")
let ParameterlessStructCtorE() = DeclareResourceString("ParameterlessStructCtor", "")
let InterfaceNotRevealedE() = DeclareResourceString("InterfaceNotRevealed", "%s")
let TyconBadArgsE() = DeclareResourceString("TyconBadArgs", "%s%d%d")
let IndeterminateTypeE() = DeclareResourceString("IndeterminateType", "")
let NameClash1E() = DeclareResourceString("NameClash1", "%s%s")
let NameClash2E() = DeclareResourceString("NameClash2", "%s%s%s%s%s")
let Duplicate1E() = DeclareResourceString("Duplicate1", "%s")
let Duplicate2E() = DeclareResourceString("Duplicate2", "%s%s")
let UndefinedName2E() = DeclareResourceString("UndefinedName2", "")
let FieldNotMutableE() = DeclareResourceString("FieldNotMutable", "")
let FieldsFromDifferentTypesE() = DeclareResourceString("FieldsFromDifferentTypes", "%s%s")
let VarBoundTwiceE() = DeclareResourceString("VarBoundTwice", "%s")
let RecursionE() = DeclareResourceString("Recursion", "%s%s%s%s")
let InvalidRuntimeCoercionE() = DeclareResourceString("InvalidRuntimeCoercion", "%s%s%s")
let IndeterminateRuntimeCoercionE() = DeclareResourceString("IndeterminateRuntimeCoercion", "%s%s")
let IndeterminateStaticCoercionE() = DeclareResourceString("IndeterminateStaticCoercion", "%s%s")
let StaticCoercionShouldUseBoxE() = DeclareResourceString("StaticCoercionShouldUseBox", "%s%s")
let TypeIsImplicitlyAbstractE() = DeclareResourceString("TypeIsImplicitlyAbstract", "")
let NonRigidTypar1E() = DeclareResourceString("NonRigidTypar1", "%s%s")
let NonRigidTypar2E() = DeclareResourceString("NonRigidTypar2", "%s%s")
let NonRigidTypar3E() = DeclareResourceString("NonRigidTypar3", "%s%s")
let OBlockEndSentenceE() = DeclareResourceString("BlockEndSentence", "")
let UnexpectedEndOfInputE() = DeclareResourceString("UnexpectedEndOfInput", "")
let UnexpectedE() = DeclareResourceString("Unexpected", "%s")
let NONTERM_interactionE() = DeclareResourceString("NONTERM.interaction", "")
let NONTERM_hashDirectiveE() = DeclareResourceString("NONTERM.hashDirective", "")
let NONTERM_fieldDeclE() = DeclareResourceString("NONTERM.fieldDecl", "")
let NONTERM_unionCaseReprE() = DeclareResourceString("NONTERM.unionCaseRepr", "")
let NONTERM_localBindingE() = DeclareResourceString("NONTERM.localBinding", "")
let NONTERM_hardwhiteLetBindingsE() = DeclareResourceString("NONTERM.hardwhiteLetBindings", "")
let NONTERM_classDefnMemberE() = DeclareResourceString("NONTERM.classDefnMember", "")
let NONTERM_defnBindingsE() = DeclareResourceString("NONTERM.defnBindings", "")
let NONTERM_classMemberSpfnE() = DeclareResourceString("NONTERM.classMemberSpfn", "")
let NONTERM_valSpfnE() = DeclareResourceString("NONTERM.valSpfn", "")
let NONTERM_tyconSpfnE() = DeclareResourceString("NONTERM.tyconSpfn", "")
let NONTERM_anonLambdaExprE() = DeclareResourceString("NONTERM.anonLambdaExpr", "")
let NONTERM_attrUnionCaseDeclE() = DeclareResourceString("NONTERM.attrUnionCaseDecl", "")
let NONTERM_cPrototypeE() = DeclareResourceString("NONTERM.cPrototype", "")
let NONTERM_objectImplementationMembersE() = DeclareResourceString("NONTERM.objectImplementationMembers", "")
let NONTERM_ifExprCasesE() = DeclareResourceString("NONTERM.ifExprCases", "")
let NONTERM_openDeclE() = DeclareResourceString("NONTERM.openDecl", "")
let NONTERM_fileModuleSpecE() = DeclareResourceString("NONTERM.fileModuleSpec", "")
let NONTERM_patternClausesE() = DeclareResourceString("NONTERM.patternClauses", "")
let NONTERM_beginEndExprE() = DeclareResourceString("NONTERM.beginEndExpr", "")
let NONTERM_recdExprE() = DeclareResourceString("NONTERM.recdExpr", "")
let NONTERM_tyconDefnE() = DeclareResourceString("NONTERM.tyconDefn", "")
let NONTERM_exconCoreE() = DeclareResourceString("NONTERM.exconCore", "")
let NONTERM_typeNameInfoE() = DeclareResourceString("NONTERM.typeNameInfo", "")
let NONTERM_attributeListE() = DeclareResourceString("NONTERM.attributeList", "")
let NONTERM_quoteExprE() = DeclareResourceString("NONTERM.quoteExpr", "")
let NONTERM_typeConstraintE() = DeclareResourceString("NONTERM.typeConstraint", "")
let NONTERM_Category_ImplementationFileE() = DeclareResourceString("NONTERM.Category.ImplementationFile", "")
let NONTERM_Category_DefinitionE() = DeclareResourceString("NONTERM.Category.Definition", "")
let NONTERM_Category_SignatureFileE() = DeclareResourceString("NONTERM.Category.SignatureFile", "")
let NONTERM_Category_PatternE() = DeclareResourceString("NONTERM.Category.Pattern", "")
let NONTERM_Category_ExprE() = DeclareResourceString("NONTERM.Category.Expr", "")
let NONTERM_Category_TypeE() = DeclareResourceString("NONTERM.Category.Type", "")
let NONTERM_typeArgsActualE() = DeclareResourceString("NONTERM.typeArgsActual", "")
let TokenName1E() = DeclareResourceString("TokenName1", "%s")
let TokenName1TokenName2E() = DeclareResourceString("TokenName1TokenName2", "%s%s")
let TokenName1TokenName2TokenName3E() = DeclareResourceString("TokenName1TokenName2TokenName3", "%s%s%s")
let RuntimeCoercionSourceSealed1E() = DeclareResourceString("RuntimeCoercionSourceSealed1", "%s")
let RuntimeCoercionSourceSealed2E() = DeclareResourceString("RuntimeCoercionSourceSealed2", "%s")
let CoercionTargetSealedE() = DeclareResourceString("CoercionTargetSealed", "%s")
let UpcastUnnecessaryE() = DeclareResourceString("UpcastUnnecessary", "")
let TypeTestUnnecessaryE() = DeclareResourceString("TypeTestUnnecessary", "")
let OverrideDoesntOverride1E() = DeclareResourceString("OverrideDoesntOverride1", "%s")
let OverrideDoesntOverride2E() = DeclareResourceString("OverrideDoesntOverride2", "%s")
let OverrideDoesntOverride3E() = DeclareResourceString("OverrideDoesntOverride3", "%s")
let OverrideDoesntOverride4E() = DeclareResourceString("OverrideDoesntOverride4", "%s")
let UnionCaseWrongArgumentsE() = DeclareResourceString("UnionCaseWrongArguments", "%d%d")
let UnionPatternsBindDifferentNamesE() = DeclareResourceString("UnionPatternsBindDifferentNames", "")
let RequiredButNotSpecifiedE() = DeclareResourceString("RequiredButNotSpecified", "%s%s%s")
let UseOfAddressOfOperatorE() = DeclareResourceString("UseOfAddressOfOperator", "")
let DefensiveCopyWarningE() = DeclareResourceString("DefensiveCopyWarning", "%s")
let DeprecatedThreadStaticBindingWarningE() = DeclareResourceString("DeprecatedThreadStaticBindingWarning", "")
let FunctionValueUnexpectedE() = DeclareResourceString("FunctionValueUnexpected", "%s")
let UnitTypeExpectedE() = DeclareResourceString("UnitTypeExpected", "%s")
let UnitTypeExpectedWithEqualityE() = DeclareResourceString("UnitTypeExpectedWithEquality", "%s")
let UnitTypeExpectedWithPossiblePropertySetterE() = DeclareResourceString("UnitTypeExpectedWithPossiblePropertySetter", "%s%s%s")
let UnitTypeExpectedWithPossibleAssignmentE() = DeclareResourceString("UnitTypeExpectedWithPossibleAssignment", "%s%s")
let UnitTypeExpectedWithPossibleAssignmentToMutableE() = DeclareResourceString("UnitTypeExpectedWithPossibleAssignmentToMutable", "%s%s")
let RecursiveUseCheckedAtRuntimeE() = DeclareResourceString("RecursiveUseCheckedAtRuntime", "")
let LetRecUnsound1E() = DeclareResourceString("LetRecUnsound1", "%s")
let LetRecUnsound2E() = DeclareResourceString("LetRecUnsound2", "%s%s")
let LetRecUnsoundInnerE() = DeclareResourceString("LetRecUnsoundInner", "%s")
let LetRecEvaluatedOutOfOrderE() = DeclareResourceString("LetRecEvaluatedOutOfOrder", "")
let LetRecCheckedAtRuntimeE() = DeclareResourceString("LetRecCheckedAtRuntime", "")
let SelfRefObjCtor1E() = DeclareResourceString("SelfRefObjCtor1", "")
let SelfRefObjCtor2E() = DeclareResourceString("SelfRefObjCtor2", "")
let VirtualAugmentationOnNullValuedTypeE() = DeclareResourceString("VirtualAugmentationOnNullValuedType", "")
let NonVirtualAugmentationOnNullValuedTypeE() = DeclareResourceString("NonVirtualAugmentationOnNullValuedType", "")
let NonUniqueInferredAbstractSlot1E() = DeclareResourceString("NonUniqueInferredAbstractSlot1", "%s")
let NonUniqueInferredAbstractSlot2E() = DeclareResourceString("NonUniqueInferredAbstractSlot2", "")
let NonUniqueInferredAbstractSlot3E() = DeclareResourceString("NonUniqueInferredAbstractSlot3", "%s%s")
let NonUniqueInferredAbstractSlot4E() = DeclareResourceString("NonUniqueInferredAbstractSlot4", "")
let Failure3E() = DeclareResourceString("Failure3", "%s")
let Failure4E() = DeclareResourceString("Failure4", "%s")
let FullAbstractionE() = DeclareResourceString("FullAbstraction", "%s")
let MatchIncomplete1E() = DeclareResourceString("MatchIncomplete1", "")
let MatchIncomplete2E() = DeclareResourceString("MatchIncomplete2", "%s")
let MatchIncomplete3E() = DeclareResourceString("MatchIncomplete3", "%s")
let MatchIncomplete4E() = DeclareResourceString("MatchIncomplete4", "")
let RuleNeverMatchedE() = DeclareResourceString("RuleNeverMatched", "")
let EnumMatchIncomplete1E() = DeclareResourceString("EnumMatchIncomplete1", "")
let ValNotMutableE() = DeclareResourceString("ValNotMutable", "%s")
let ValNotLocalE() = DeclareResourceString("ValNotLocal", "")
let Obsolete1E() = DeclareResourceString("Obsolete1", "")
let Obsolete2E() = DeclareResourceString("Obsolete2", "%s")
let ExperimentalE() = DeclareResourceString("Experimental", "%s")
let PossibleUnverifiableCodeE() = DeclareResourceString("PossibleUnverifiableCode", "")
let DeprecatedE() = DeclareResourceString("Deprecated", "%s")
let LibraryUseOnlyE() = DeclareResourceString("LibraryUseOnly", "")
let MissingFieldsE() = DeclareResourceString("MissingFields", "%s")
let ValueRestriction1E() = DeclareResourceString("ValueRestriction1", "%s%s%s")
let ValueRestriction2E() = DeclareResourceString("ValueRestriction2", "%s%s%s")
let ValueRestriction3E() = DeclareResourceString("ValueRestriction3", "%s")
let ValueRestriction4E() = DeclareResourceString("ValueRestriction4", "%s%s%s")
let ValueRestriction5E() = DeclareResourceString("ValueRestriction5", "%s%s%s")
let RecoverableParseErrorE() = DeclareResourceString("RecoverableParseError", "")
let ReservedKeywordE() = DeclareResourceString("ReservedKeyword", "%s")
let IndentationProblemE() = DeclareResourceString("IndentationProblem", "%s")
let OverrideInIntrinsicAugmentationE() = DeclareResourceString("OverrideInIntrinsicAugmentation", "")
let OverrideInExtrinsicAugmentationE() = DeclareResourceString("OverrideInExtrinsicAugmentation", "")
let IntfImplInIntrinsicAugmentationE() = DeclareResourceString("IntfImplInIntrinsicAugmentation", "")
let IntfImplInExtrinsicAugmentationE() = DeclareResourceString("IntfImplInExtrinsicAugmentation", "")
let UnresolvedReferenceNoRangeE() = DeclareResourceString("UnresolvedReferenceNoRange", "%s")
let UnresolvedPathReferenceNoRangeE() = DeclareResourceString("UnresolvedPathReferenceNoRange", "%s%s")
let HashIncludeNotAllowedInNonScriptE() = DeclareResourceString("HashIncludeNotAllowedInNonScript", "")
let HashReferenceNotAllowedInNonScriptE() = DeclareResourceString("HashReferenceNotAllowedInNonScript", "")
let HashDirectiveNotAllowedInNonScriptE() = DeclareResourceString("HashDirectiveNotAllowedInNonScript", "")
let FileNameNotResolvedE() = DeclareResourceString("FileNameNotResolved", "%s%s")
let AssemblyNotResolvedE() = DeclareResourceString("AssemblyNotResolved", "%s")
let HashLoadedSourceHasIssues1E() = DeclareResourceString("HashLoadedSourceHasIssues1", "")
let HashLoadedSourceHasIssues2E() = DeclareResourceString("HashLoadedSourceHasIssues2", "")
let HashLoadedScriptConsideredSourceE() = DeclareResourceString("HashLoadedScriptConsideredSource", "")  
let InvalidInternalsVisibleToAssemblyName1E() = DeclareResourceString("InvalidInternalsVisibleToAssemblyName1", "%s%s")
let InvalidInternalsVisibleToAssemblyName2E() = DeclareResourceString("InvalidInternalsVisibleToAssemblyName2", "%s")
let LoadedSourceNotFoundIgnoringE() = DeclareResourceString("LoadedSourceNotFoundIgnoring", "%s")
let MSBuildReferenceResolutionErrorE() = DeclareResourceString("MSBuildReferenceResolutionError", "%s%s")
let TargetInvocationExceptionWrapperE() = DeclareResourceString("TargetInvocationExceptionWrapper", "%s")

let getErrorString key = SR.GetString key

let (|InvalidArgument|_|) (exn:exn) = match exn with :? ArgumentException as e -> Some e.Message | _ -> None

let OutputPhasedErrorR (os:StringBuilder) (err:PhasedDiagnostic) =

    let rec OutputExceptionR (os:StringBuilder) error = 

      match error with
      | ConstraintSolverTupleDiffLengths(_, tl1, tl2, m, m2) -> 
          os.Append(ConstraintSolverTupleDiffLengthsE().Format tl1.Length tl2.Length) |> ignore
          if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore

      | ConstraintSolverInfiniteTypes(contextInfo, denv, t1, t2, m, m2) ->
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ConstraintSolverInfiniteTypesE().Format t1 t2)  |> ignore

          match contextInfo with
          | ContextInfo.ReturnInComputationExpression ->
            os.Append(" " + FSComp.SR.returnUsedInsteadOfReturnBang()) |> ignore
          | ContextInfo.YieldInComputationExpression ->
            os.Append(" " + FSComp.SR.yieldUsedInsteadOfYieldBang()) |> ignore
          | _ -> ()

          if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore

      | ConstraintSolverMissingConstraint(denv, tpr, tpc, m, m2) -> 
          os.Append(ConstraintSolverMissingConstraintE().Format (NicePrint.stringOfTyparConstraint denv (tpr, tpc))) |> ignore
          if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore

      | ConstraintSolverTypesNotInEqualityRelation(denv, (TType_measure _ as t1), (TType_measure _ as t2), m, m2, _) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          
          os.Append(ConstraintSolverTypesNotInEqualityRelation1E().Format t1 t2 )  |> ignore
          
          if m.StartLine <> m2.StartLine then
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore

      | ConstraintSolverTypesNotInEqualityRelation(denv, t1, t2, m, m2, contextInfo) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          
          match contextInfo with
          | ContextInfo.IfExpression range when range = m -> os.Append(FSComp.SR.ifExpression(t1, t2)) |> ignore
          | ContextInfo.CollectionElement (isArray, range) when range = m -> 
            if isArray then
                os.Append(FSComp.SR.arrayElementHasWrongType(t1, t2)) |> ignore
            else
                os.Append(FSComp.SR.listElementHasWrongType(t1, t2)) |> ignore
          | ContextInfo.OmittedElseBranch range when range = m -> os.Append(FSComp.SR.missingElseBranch(t2)) |> ignore
          | ContextInfo.ElseBranchResult range when range = m -> os.Append(FSComp.SR.elseBranchHasWrongType(t1, t2)) |> ignore
          | ContextInfo.FollowingPatternMatchClause range when range = m -> os.Append(FSComp.SR.followingPatternMatchClauseHasWrongType(t1, t2)) |> ignore
          | ContextInfo.PatternMatchGuard range when range = m -> os.Append(FSComp.SR.patternMatchGuardIsNotBool(t2)) |> ignore
          | _ -> os.Append(ConstraintSolverTypesNotInEqualityRelation2E().Format t1 t2) |> ignore
          if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore

      | ConstraintSolverTypesNotInSubsumptionRelation(denv, t1, t2, m, m2) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ConstraintSolverTypesNotInSubsumptionRelationE().Format t2 t1 cxs) |> ignore
          if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m2)) |> ignore

      | ConstraintSolverError(msg, m, m2) -> 
         os.Append msg |> ignore
         if m.StartLine <> m2.StartLine then 
            os.Append(SeeAlsoE().Format (stringOfRange m2)) |> ignore

      | ConstraintSolverRelatedInformation(fopt, _, e) -> 
          match e with 
          | ConstraintSolverError _ -> OutputExceptionR os e
          | _ -> ()
          fopt |> Option.iter (Printf.bprintf os " %s")

      | ErrorFromAddingTypeEquation(g, denv, t1, t2, ConstraintSolverTypesNotInEqualityRelation(_, t1', t2', m , _ , contextInfo), _) 
         when typeEquiv g t1 t1'
         &&   typeEquiv g t2 t2' ->
          let t1, t2, tpcs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          match contextInfo with
          | ContextInfo.IfExpression range when range = m -> os.Append(FSComp.SR.ifExpression(t1, t2)) |> ignore
          | ContextInfo.CollectionElement (isArray, range) when range = m -> 
            if isArray then
                os.Append(FSComp.SR.arrayElementHasWrongType(t1, t2)) |> ignore
            else
                os.Append(FSComp.SR.listElementHasWrongType(t1, t2)) |> ignore
          | ContextInfo.OmittedElseBranch range when range = m -> os.Append(FSComp.SR.missingElseBranch(t2)) |> ignore
          | ContextInfo.ElseBranchResult range when range = m -> os.Append(FSComp.SR.elseBranchHasWrongType(t1, t2)) |> ignore
          | ContextInfo.FollowingPatternMatchClause range when range = m -> os.Append(FSComp.SR.followingPatternMatchClauseHasWrongType(t1, t2)) |> ignore
          | ContextInfo.PatternMatchGuard range when range = m -> os.Append(FSComp.SR.patternMatchGuardIsNotBool(t2)) |> ignore
          | ContextInfo.TupleInRecordFields ->
                os.Append(ErrorFromAddingTypeEquation1E().Format t2 t1 tpcs) |> ignore
                os.Append(System.Environment.NewLine + FSComp.SR.commaInsteadOfSemicolonInRecord()) |> ignore
          | _ when t2 = "bool" && t1.EndsWith " ref" ->
                os.Append(ErrorFromAddingTypeEquation1E().Format t2 t1 tpcs) |> ignore
                os.Append(System.Environment.NewLine + FSComp.SR.derefInsteadOfNot()) |> ignore
          | _ -> os.Append(ErrorFromAddingTypeEquation1E().Format t2 t1 tpcs) |> ignore

      | ErrorFromAddingTypeEquation(_, _, _, _, ((ConstraintSolverTypesNotInEqualityRelation (_, _, _, _, _, contextInfo) ) as e), _) when (match contextInfo with ContextInfo.NoContext -> false | _ -> true) ->  
          OutputExceptionR os e

      | ErrorFromAddingTypeEquation(_, _, _, _, ((ConstraintSolverTypesNotInSubsumptionRelation _ | ConstraintSolverError _ ) as e), _) ->  
          OutputExceptionR os e

      | ErrorFromAddingTypeEquation(g, denv, t1, t2, e, _) ->
          if not (typeEquiv g t1 t2) then
              let t1, t2, tpcs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
              if t1<>t2 + tpcs then os.Append(ErrorFromAddingTypeEquation2E().Format t1 t2 tpcs) |> ignore

          OutputExceptionR os e

      | ErrorFromApplyingDefault(_, denv, _, defaultType, e, _) ->  
          let defaultType = NicePrint.minimalStringOfType denv defaultType
          os.Append(ErrorFromApplyingDefault1E().Format defaultType) |> ignore
          OutputExceptionR os e
          os.Append(ErrorFromApplyingDefault2E().Format) |> ignore

      | ErrorsFromAddingSubsumptionConstraint(g, denv, t1, t2, e, contextInfo, _) ->
          match contextInfo with
          | ContextInfo.DowncastUsedInsteadOfUpcast isOperator -> 
              let t1, t2, _ = NicePrint.minimalStringsOfTwoTypes denv t1 t2
              if isOperator then
                  os.Append(FSComp.SR.considerUpcastOperator(t1, t2) |> snd) |> ignore
              else
                  os.Append(FSComp.SR.considerUpcast(t1, t2) |> snd) |> ignore
          | _ ->
              if not (typeEquiv g t1 t2) then
                  let t1, t2, tpcs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
                  if t1 <> (t2 + tpcs) then 
                      os.Append(ErrorsFromAddingSubsumptionConstraintE().Format t2 t1 tpcs) |> ignore
                  else
                      OutputExceptionR os e
              else
                  OutputExceptionR os e

      | UpperCaseIdentifierInPattern(_) -> 
          os.Append(UpperCaseIdentifierInPatternE().Format) |> ignore

      | NotUpperCaseConstructor(_) -> 
          os.Append(NotUpperCaseConstructorE().Format) |> ignore

      | ErrorFromAddingConstraint(_, e, _) ->  
          OutputExceptionR os e

#if !NO_EXTENSIONTYPING
      | ExtensionTyping.ProvidedTypeResolutionNoRange(e)

      | ExtensionTyping.ProvidedTypeResolution(_, e) -> 
          OutputExceptionR os e

      | :? TypeProviderError as e ->
          os.Append(e.ContextualErrorMessage) |> ignore
#endif

      | UnresolvedOverloading(_, _, mtext, _) -> 
          os.Append(mtext) |> ignore

      | UnresolvedConversionOperator(denv, fromTy, toTy, _) -> 
          let t1, t2, _tpcs = NicePrint.minimalStringsOfTwoTypes denv fromTy toTy
          os.Append(FSComp.SR.csTypeDoesNotSupportConversion(t1, t2)) |> ignore

      | PossibleOverload(_, minfo, originalError, _) -> 
          // print original error that describes reason why this overload was rejected
          let buf = new StringBuilder()
          OutputExceptionR buf originalError

          os.Append(PossibleOverloadE().Format minfo (buf.ToString())) |> ignore

      //| PossibleBestOverload(_, minfo, m) -> 
      //    Printf.bprintf os "\n\nPossible best overload: '%s'." minfo

      | FunctionExpected _ ->
          os.Append(FunctionExpectedE().Format) |> ignore

      | BakedInMemberConstraintName(nm, _) ->
          os.Append(BakedInMemberConstraintNameE().Format nm) |> ignore

      | StandardOperatorRedefinitionWarning(msg, _) -> 
          os.Append(msg) |> ignore

      | BadEventTransformation(_) ->
         os.Append(BadEventTransformationE().Format) |> ignore

      | ParameterlessStructCtor(_) ->
         os.Append(ParameterlessStructCtorE().Format) |> ignore

      | InterfaceNotRevealed(denv, ity, _) ->
          os.Append(InterfaceNotRevealedE().Format (NicePrint.minimalStringOfType denv ity)) |> ignore

      | NotAFunctionButIndexer(_, _, name, _, _) ->
          match name with
          | Some name -> os.Append(FSComp.SR.notAFunctionButMaybeIndexerWithName name) |> ignore
          | _ -> os.Append(FSComp.SR.notAFunctionButMaybeIndexer()) |> ignore

      | NotAFunction(_, _, _, marg) ->
          if marg.StartColumn = 0 then
              os.Append(FSComp.SR.notAFunctionButMaybeDeclaration()) |> ignore
          else
              os.Append(FSComp.SR.notAFunction()) |> ignore

      | TyconBadArgs(_, tcref, d, _) -> 
          let exp = tcref.TyparsNoRange.Length
          if exp = 0 then
              os.Append(FSComp.SR.buildUnexpectedTypeArgs(fullDisplayTextOfTyconRef tcref, d)) |> ignore
          else
              os.Append(TyconBadArgsE().Format (fullDisplayTextOfTyconRef tcref) exp d) |> ignore

      | IndeterminateType(_) -> 
          os.Append(IndeterminateTypeE().Format) |> ignore

      | NameClash(nm, k1, nm1, _, k2, nm2, _) -> 
          if nm = nm1 && nm1 = nm2 && k1 = k2 then 
              os.Append(NameClash1E().Format k1 nm1) |> ignore
          else
              os.Append(NameClash2E().Format k1 nm1 nm k2 nm2) |> ignore

      | Duplicate(k, s, _)  -> 
          if k = "member" then 
              os.Append(Duplicate1E().Format (DecompileOpName s)) |> ignore
          else 
              os.Append(Duplicate2E().Format k (DecompileOpName s)) |> ignore

      | UndefinedName(_, k, id, suggestionsF) ->
          os.Append(k (DecompileOpName id.idText)) |> ignore
          let filtered = ErrorResolutionHints.FilterPredictions id.idText suggestionsF
          if List.isEmpty filtered |> not then
              os.Append(ErrorResolutionHints.FormatPredictions DecompileOpName filtered) |> ignore
          

      | InternalUndefinedItemRef(f, smr, ccuName, s) ->  
          let _, errs = f(smr, ccuName, s)  
          os.Append(errs) |> ignore  

      | FieldNotMutable  _ -> 
          os.Append(FieldNotMutableE().Format) |> ignore

      | FieldsFromDifferentTypes (_, fref1, fref2, _) -> 
          os.Append(FieldsFromDifferentTypesE().Format fref1.FieldName fref2.FieldName) |> ignore

      | VarBoundTwice(id) ->  
          os.Append(VarBoundTwiceE().Format (DecompileOpName id.idText)) |> ignore

      | Recursion (denv, id, ty1, ty2, _) -> 
          let t1, t2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(RecursionE().Format (DecompileOpName id.idText) t1 t2 tpcs) |> ignore

      | InvalidRuntimeCoercion(denv, ty1, ty2, _) -> 
          let t1, t2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(InvalidRuntimeCoercionE().Format t1 t2 tpcs) |> ignore

      | IndeterminateRuntimeCoercion(denv, ty1, ty2, _) -> 
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(IndeterminateRuntimeCoercionE().Format t1 t2) |> ignore

      | IndeterminateStaticCoercion(denv, ty1, ty2, _) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(IndeterminateStaticCoercionE().Format t1 t2) |> ignore

      | StaticCoercionShouldUseBox(denv, ty1, ty2, _) ->
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(StaticCoercionShouldUseBoxE().Format t1 t2) |> ignore

      | TypeIsImplicitlyAbstract(_) -> 
          os.Append(TypeIsImplicitlyAbstractE().Format) |> ignore

      | NonRigidTypar(denv, tpnmOpt, typarRange, ty1, ty, _) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let (ty1, ty), _cxs = PrettyTypes.PrettifyTypePair denv.g (ty1, ty)
          match tpnmOpt with 
          | None -> 
              os.Append(NonRigidTypar1E().Format (stringOfRange typarRange) (NicePrint.stringOfTy denv ty)) |> ignore
          | Some tpnm -> 
              match ty1 with 
              | TType_measure _ -> 
                os.Append(NonRigidTypar2E().Format tpnm  (NicePrint.stringOfTy denv ty)) |> ignore
              | _ -> 
                os.Append(NonRigidTypar3E().Format tpnm  (NicePrint.stringOfTy denv ty)) |> ignore

      | SyntaxError (ctxt, _) -> 
          let ctxt = unbox<Parsing.ParseErrorContext<Parser.token>>(ctxt)
          
          let (|EndOfStructuredConstructToken|_|) token = 
              match token with
              | Parser.TOKEN_ODECLEND 
              | Parser.TOKEN_OBLOCKSEP 
              | Parser.TOKEN_OEND 
              | Parser.TOKEN_ORIGHT_BLOCK_END 
              | Parser.TOKEN_OBLOCKEND | Parser.TOKEN_OBLOCKEND_COMING_SOON | Parser.TOKEN_OBLOCKEND_IS_HERE -> Some()
              | _ -> None

          let tokenIdToText tid = 
              match tid with 
              | Parser.TOKEN_IDENT -> getErrorString("Parser.TOKEN.IDENT")
              | Parser.TOKEN_BIGNUM 
              | Parser.TOKEN_INT8  
              | Parser.TOKEN_UINT8 
              | Parser.TOKEN_INT16  
              | Parser.TOKEN_UINT16 
              | Parser.TOKEN_INT32 
              | Parser.TOKEN_UINT32 
              | Parser.TOKEN_INT64 
              | Parser.TOKEN_UINT64 
              | Parser.TOKEN_UNATIVEINT 
              | Parser.TOKEN_NATIVEINT -> getErrorString("Parser.TOKEN.INT")
              | Parser.TOKEN_IEEE32 
              | Parser.TOKEN_IEEE64 -> getErrorString("Parser.TOKEN.FLOAT")
              | Parser.TOKEN_DECIMAL -> getErrorString("Parser.TOKEN.DECIMAL")
              | Parser.TOKEN_CHAR -> getErrorString("Parser.TOKEN.CHAR")
                
              | Parser.TOKEN_BASE -> getErrorString("Parser.TOKEN.BASE")
              | Parser.TOKEN_LPAREN_STAR_RPAREN -> getErrorString("Parser.TOKEN.LPAREN.STAR.RPAREN")
              | Parser.TOKEN_DOLLAR -> getErrorString("Parser.TOKEN.DOLLAR")
              | Parser.TOKEN_INFIX_STAR_STAR_OP -> getErrorString("Parser.TOKEN.INFIX.STAR.STAR.OP")
              | Parser.TOKEN_INFIX_COMPARE_OP -> getErrorString("Parser.TOKEN.INFIX.COMPARE.OP")
              | Parser.TOKEN_COLON_GREATER -> getErrorString("Parser.TOKEN.COLON.GREATER")  
              | Parser.TOKEN_COLON_COLON  ->getErrorString("Parser.TOKEN.COLON.COLON")
              | Parser.TOKEN_PERCENT_OP -> getErrorString("Parser.TOKEN.PERCENT.OP")
              | Parser.TOKEN_INFIX_AT_HAT_OP -> getErrorString("Parser.TOKEN.INFIX.AT.HAT.OP")
              | Parser.TOKEN_INFIX_BAR_OP -> getErrorString("Parser.TOKEN.INFIX.BAR.OP")
              | Parser.TOKEN_PLUS_MINUS_OP -> getErrorString("Parser.TOKEN.PLUS.MINUS.OP")
              | Parser.TOKEN_PREFIX_OP -> getErrorString("Parser.TOKEN.PREFIX.OP")
              | Parser.TOKEN_COLON_QMARK_GREATER   -> getErrorString("Parser.TOKEN.COLON.QMARK.GREATER")
              | Parser.TOKEN_INFIX_STAR_DIV_MOD_OP -> getErrorString("Parser.TOKEN.INFIX.STAR.DIV.MOD.OP")
              | Parser.TOKEN_INFIX_AMP_OP -> getErrorString("Parser.TOKEN.INFIX.AMP.OP")
              | Parser.TOKEN_AMP   -> getErrorString("Parser.TOKEN.AMP")
              | Parser.TOKEN_AMP_AMP  -> getErrorString("Parser.TOKEN.AMP.AMP")
              | Parser.TOKEN_BAR_BAR  -> getErrorString("Parser.TOKEN.BAR.BAR")
              | Parser.TOKEN_LESS   -> getErrorString("Parser.TOKEN.LESS")
              | Parser.TOKEN_GREATER  -> getErrorString("Parser.TOKEN.GREATER")
              | Parser.TOKEN_QMARK   -> getErrorString("Parser.TOKEN.QMARK")
              | Parser.TOKEN_QMARK_QMARK -> getErrorString("Parser.TOKEN.QMARK.QMARK")
              | Parser.TOKEN_COLON_QMARK-> getErrorString("Parser.TOKEN.COLON.QMARK")
              | Parser.TOKEN_INT32_DOT_DOT -> getErrorString("Parser.TOKEN.INT32.DOT.DOT")
              | Parser.TOKEN_DOT_DOT       -> getErrorString("Parser.TOKEN.DOT.DOT")
              | Parser.TOKEN_QUOTE   -> getErrorString("Parser.TOKEN.QUOTE")
              | Parser.TOKEN_STAR  -> getErrorString("Parser.TOKEN.STAR")
              | Parser.TOKEN_HIGH_PRECEDENCE_TYAPP  -> getErrorString("Parser.TOKEN.HIGH.PRECEDENCE.TYAPP")
              | Parser.TOKEN_COLON    -> getErrorString("Parser.TOKEN.COLON")
              | Parser.TOKEN_COLON_EQUALS   -> getErrorString("Parser.TOKEN.COLON.EQUALS")
              | Parser.TOKEN_LARROW   -> getErrorString("Parser.TOKEN.LARROW")
              | Parser.TOKEN_EQUALS -> getErrorString("Parser.TOKEN.EQUALS")
              | Parser.TOKEN_GREATER_BAR_RBRACK -> getErrorString("Parser.TOKEN.GREATER.BAR.RBRACK")
              | Parser.TOKEN_MINUS -> getErrorString("Parser.TOKEN.MINUS")
              | Parser.TOKEN_ADJACENT_PREFIX_OP    -> getErrorString("Parser.TOKEN.ADJACENT.PREFIX.OP")
              | Parser.TOKEN_FUNKY_OPERATOR_NAME -> getErrorString("Parser.TOKEN.FUNKY.OPERATOR.NAME") 
              | Parser.TOKEN_COMMA-> getErrorString("Parser.TOKEN.COMMA")
              | Parser.TOKEN_DOT -> getErrorString("Parser.TOKEN.DOT")
              | Parser.TOKEN_BAR-> getErrorString("Parser.TOKEN.BAR")
              | Parser.TOKEN_HASH -> getErrorString("Parser.TOKEN.HASH")
              | Parser.TOKEN_UNDERSCORE   -> getErrorString("Parser.TOKEN.UNDERSCORE")
              | Parser.TOKEN_SEMICOLON   -> getErrorString("Parser.TOKEN.SEMICOLON")
              | Parser.TOKEN_SEMICOLON_SEMICOLON-> getErrorString("Parser.TOKEN.SEMICOLON.SEMICOLON")
              | Parser.TOKEN_LPAREN-> getErrorString("Parser.TOKEN.LPAREN")
              | Parser.TOKEN_RPAREN | Parser.TOKEN_RPAREN_COMING_SOON | Parser.TOKEN_RPAREN_IS_HERE -> getErrorString("Parser.TOKEN.RPAREN")
              | Parser.TOKEN_LQUOTE  -> getErrorString("Parser.TOKEN.LQUOTE")
              | Parser.TOKEN_LBRACK  -> getErrorString("Parser.TOKEN.LBRACK")
              | Parser.TOKEN_LBRACK_BAR  -> getErrorString("Parser.TOKEN.LBRACK.BAR")
              | Parser.TOKEN_LBRACK_LESS  -> getErrorString("Parser.TOKEN.LBRACK.LESS")
              | Parser.TOKEN_LBRACE   -> getErrorString("Parser.TOKEN.LBRACE")
              | Parser.TOKEN_LBRACE_LESS-> getErrorString("Parser.TOKEN.LBRACE.LESS")
              | Parser.TOKEN_BAR_RBRACK   -> getErrorString("Parser.TOKEN.BAR.RBRACK")
              | Parser.TOKEN_GREATER_RBRACE   -> getErrorString("Parser.TOKEN.GREATER.RBRACE")
              | Parser.TOKEN_GREATER_RBRACK  -> getErrorString("Parser.TOKEN.GREATER.RBRACK")
              | Parser.TOKEN_RQUOTE_DOT _ 
              | Parser.TOKEN_RQUOTE  -> getErrorString("Parser.TOKEN.RQUOTE")
              | Parser.TOKEN_RBRACK  -> getErrorString("Parser.TOKEN.RBRACK")
              | Parser.TOKEN_RBRACE | Parser.TOKEN_RBRACE_COMING_SOON | Parser.TOKEN_RBRACE_IS_HERE -> getErrorString("Parser.TOKEN.RBRACE")
              | Parser.TOKEN_PUBLIC -> getErrorString("Parser.TOKEN.PUBLIC")
              | Parser.TOKEN_PRIVATE -> getErrorString("Parser.TOKEN.PRIVATE")
              | Parser.TOKEN_INTERNAL -> getErrorString("Parser.TOKEN.INTERNAL")
              | Parser.TOKEN_CONSTRAINT -> getErrorString("Parser.TOKEN.CONSTRAINT")
              | Parser.TOKEN_INSTANCE -> getErrorString("Parser.TOKEN.INSTANCE")
              | Parser.TOKEN_DELEGATE -> getErrorString("Parser.TOKEN.DELEGATE")
              | Parser.TOKEN_INHERIT -> getErrorString("Parser.TOKEN.INHERIT")
              | Parser.TOKEN_CONSTRUCTOR-> getErrorString("Parser.TOKEN.CONSTRUCTOR")
              | Parser.TOKEN_DEFAULT -> getErrorString("Parser.TOKEN.DEFAULT")
              | Parser.TOKEN_OVERRIDE-> getErrorString("Parser.TOKEN.OVERRIDE")
              | Parser.TOKEN_ABSTRACT-> getErrorString("Parser.TOKEN.ABSTRACT")
              | Parser.TOKEN_CLASS-> getErrorString("Parser.TOKEN.CLASS")
              | Parser.TOKEN_MEMBER -> getErrorString("Parser.TOKEN.MEMBER")
              | Parser.TOKEN_STATIC -> getErrorString("Parser.TOKEN.STATIC")
              | Parser.TOKEN_NAMESPACE-> getErrorString("Parser.TOKEN.NAMESPACE")
              | Parser.TOKEN_OBLOCKBEGIN  -> getErrorString("Parser.TOKEN.OBLOCKBEGIN") 
              | EndOfStructuredConstructToken -> getErrorString("Parser.TOKEN.OBLOCKEND") 
              | Parser.TOKEN_THEN  
              | Parser.TOKEN_OTHEN -> getErrorString("Parser.TOKEN.OTHEN")
              | Parser.TOKEN_ELSE
              | Parser.TOKEN_OELSE -> getErrorString("Parser.TOKEN.OELSE")
              | Parser.TOKEN_LET(_) 
              | Parser.TOKEN_OLET(_)  -> getErrorString("Parser.TOKEN.OLET")
              | Parser.TOKEN_OBINDER 
              | Parser.TOKEN_BINDER -> getErrorString("Parser.TOKEN.BINDER")
              | Parser.TOKEN_ODO -> getErrorString("Parser.TOKEN.ODO")
              | Parser.TOKEN_OWITH -> getErrorString("Parser.TOKEN.OWITH")
              | Parser.TOKEN_OFUNCTION -> getErrorString("Parser.TOKEN.OFUNCTION")
              | Parser.TOKEN_OFUN -> getErrorString("Parser.TOKEN.OFUN")
              | Parser.TOKEN_ORESET -> getErrorString("Parser.TOKEN.ORESET")
              | Parser.TOKEN_ODUMMY -> getErrorString("Parser.TOKEN.ODUMMY")
              | Parser.TOKEN_DO_BANG 
              | Parser.TOKEN_ODO_BANG -> getErrorString("Parser.TOKEN.ODO.BANG")
              | Parser.TOKEN_YIELD -> getErrorString("Parser.TOKEN.YIELD")
              | Parser.TOKEN_YIELD_BANG  -> getErrorString("Parser.TOKEN.YIELD.BANG")
              | Parser.TOKEN_OINTERFACE_MEMBER-> getErrorString("Parser.TOKEN.OINTERFACE.MEMBER")
              | Parser.TOKEN_ELIF -> getErrorString("Parser.TOKEN.ELIF")
              | Parser.TOKEN_RARROW -> getErrorString("Parser.TOKEN.RARROW")
              | Parser.TOKEN_SIG -> getErrorString("Parser.TOKEN.SIG")
              | Parser.TOKEN_STRUCT  -> getErrorString("Parser.TOKEN.STRUCT")
              | Parser.TOKEN_UPCAST   -> getErrorString("Parser.TOKEN.UPCAST")
              | Parser.TOKEN_DOWNCAST   -> getErrorString("Parser.TOKEN.DOWNCAST")
              | Parser.TOKEN_NULL   -> getErrorString("Parser.TOKEN.NULL")
              | Parser.TOKEN_RESERVED    -> getErrorString("Parser.TOKEN.RESERVED")
              | Parser.TOKEN_MODULE | Parser.TOKEN_MODULE_COMING_SOON | Parser.TOKEN_MODULE_IS_HERE   -> getErrorString("Parser.TOKEN.MODULE")
              | Parser.TOKEN_AND    -> getErrorString("Parser.TOKEN.AND")
              | Parser.TOKEN_AS   -> getErrorString("Parser.TOKEN.AS")
              | Parser.TOKEN_ASSERT   -> getErrorString("Parser.TOKEN.ASSERT")
              | Parser.TOKEN_OASSERT   -> getErrorString("Parser.TOKEN.ASSERT")
              | Parser.TOKEN_ASR-> getErrorString("Parser.TOKEN.ASR")
              | Parser.TOKEN_DOWNTO   -> getErrorString("Parser.TOKEN.DOWNTO")
              | Parser.TOKEN_EXCEPTION   -> getErrorString("Parser.TOKEN.EXCEPTION")
              | Parser.TOKEN_FALSE   -> getErrorString("Parser.TOKEN.FALSE")
              | Parser.TOKEN_FOR   -> getErrorString("Parser.TOKEN.FOR")
              | Parser.TOKEN_FUN   -> getErrorString("Parser.TOKEN.FUN")
              | Parser.TOKEN_FUNCTION-> getErrorString("Parser.TOKEN.FUNCTION")
              | Parser.TOKEN_FINALLY   -> getErrorString("Parser.TOKEN.FINALLY")
              | Parser.TOKEN_LAZY   -> getErrorString("Parser.TOKEN.LAZY")
              | Parser.TOKEN_OLAZY   -> getErrorString("Parser.TOKEN.LAZY")
              | Parser.TOKEN_MATCH   -> getErrorString("Parser.TOKEN.MATCH")
              | Parser.TOKEN_MATCH_BANG -> getErrorString("Parser.TOKEN.MATCH.BANG")
              | Parser.TOKEN_MUTABLE   -> getErrorString("Parser.TOKEN.MUTABLE")
              | Parser.TOKEN_NEW   -> getErrorString("Parser.TOKEN.NEW")
              | Parser.TOKEN_OF    -> getErrorString("Parser.TOKEN.OF")
              | Parser.TOKEN_OPEN   -> getErrorString("Parser.TOKEN.OPEN")
              | Parser.TOKEN_OR -> getErrorString("Parser.TOKEN.OR")
              | Parser.TOKEN_VOID -> getErrorString("Parser.TOKEN.VOID")
              | Parser.TOKEN_EXTERN-> getErrorString("Parser.TOKEN.EXTERN")
              | Parser.TOKEN_INTERFACE -> getErrorString("Parser.TOKEN.INTERFACE")
              | Parser.TOKEN_REC   -> getErrorString("Parser.TOKEN.REC")
              | Parser.TOKEN_TO   -> getErrorString("Parser.TOKEN.TO")
              | Parser.TOKEN_TRUE   -> getErrorString("Parser.TOKEN.TRUE")
              | Parser.TOKEN_TRY   -> getErrorString("Parser.TOKEN.TRY")
              | Parser.TOKEN_TYPE | Parser.TOKEN_TYPE_COMING_SOON | Parser.TOKEN_TYPE_IS_HERE   -> getErrorString("Parser.TOKEN.TYPE")
              | Parser.TOKEN_VAL   -> getErrorString("Parser.TOKEN.VAL")
              | Parser.TOKEN_INLINE   -> getErrorString("Parser.TOKEN.INLINE")
              | Parser.TOKEN_WHEN  -> getErrorString("Parser.TOKEN.WHEN")
              | Parser.TOKEN_WHILE   -> getErrorString("Parser.TOKEN.WHILE")
              | Parser.TOKEN_WITH-> getErrorString("Parser.TOKEN.WITH")
              | Parser.TOKEN_IF -> getErrorString("Parser.TOKEN.IF")
              | Parser.TOKEN_DO -> getErrorString("Parser.TOKEN.DO")
              | Parser.TOKEN_GLOBAL -> getErrorString("Parser.TOKEN.GLOBAL")
              | Parser.TOKEN_DONE -> getErrorString("Parser.TOKEN.DONE")
              | Parser.TOKEN_IN | Parser.TOKEN_JOIN_IN -> getErrorString("Parser.TOKEN.IN")
              | Parser.TOKEN_HIGH_PRECEDENCE_PAREN_APP-> getErrorString("Parser.TOKEN.HIGH.PRECEDENCE.PAREN.APP")
              | Parser.TOKEN_HIGH_PRECEDENCE_BRACK_APP-> getErrorString("Parser.TOKEN.HIGH.PRECEDENCE.BRACK.APP")
              | Parser.TOKEN_BEGIN  -> getErrorString("Parser.TOKEN.BEGIN")
              | Parser.TOKEN_END -> getErrorString("Parser.TOKEN.END")
              | Parser.TOKEN_HASH_LIGHT
              | Parser.TOKEN_HASH_LINE 
              | Parser.TOKEN_HASH_IF 
              | Parser.TOKEN_HASH_ELSE 
              | Parser.TOKEN_HASH_ENDIF  -> getErrorString("Parser.TOKEN.HASH.ENDIF")
              | Parser.TOKEN_INACTIVECODE -> getErrorString("Parser.TOKEN.INACTIVECODE")
              | Parser.TOKEN_LEX_FAILURE-> getErrorString("Parser.TOKEN.LEX.FAILURE")
              | Parser.TOKEN_WHITESPACE -> getErrorString("Parser.TOKEN.WHITESPACE")
              | Parser.TOKEN_COMMENT -> getErrorString("Parser.TOKEN.COMMENT")
              | Parser.TOKEN_LINE_COMMENT -> getErrorString("Parser.TOKEN.LINE.COMMENT")
              | Parser.TOKEN_STRING_TEXT -> getErrorString("Parser.TOKEN.STRING.TEXT")
              | Parser.TOKEN_BYTEARRAY -> getErrorString("Parser.TOKEN.BYTEARRAY")
              | Parser.TOKEN_STRING -> getErrorString("Parser.TOKEN.STRING")
              | Parser.TOKEN_KEYWORD_STRING -> getErrorString("Parser.TOKEN.KEYWORD_STRING")
              | Parser.TOKEN_EOF -> getErrorString("Parser.TOKEN.EOF")
              | Parser.TOKEN_CONST -> getErrorString("Parser.TOKEN.CONST")
              | Parser.TOKEN_FIXED -> getErrorString("Parser.TOKEN.FIXED")
              | unknown ->           
                  Debug.Assert(false, "unknown token tag")
                  let result = sprintf "%+A" unknown
                  Debug.Assert(false, result)
                  result

          match ctxt.CurrentToken with 
          | None -> os.Append(UnexpectedEndOfInputE().Format) |> ignore
          | Some token -> 
              match (token |> Parser.tagOfToken |> Parser.tokenTagToTokenId), token with 
              | EndOfStructuredConstructToken, _ -> os.Append(OBlockEndSentenceE().Format) |> ignore
              | Parser.TOKEN_LEX_FAILURE, Parser.LEX_FAILURE str -> Printf.bprintf os "%s" str (* Fix bug://2431 *)
              | token, _ -> os.Append(UnexpectedE().Format (token |> tokenIdToText)) |> ignore

              (* Search for a state producing a single recognized non-terminal in the states on the stack *)
              let foundInContext =
              
                  (* Merge a bunch of expression non terminals *)
                  let (|NONTERM_Category_Expr|_|) = function
                        | Parser.NONTERM_argExpr|Parser.NONTERM_minusExpr|Parser.NONTERM_parenExpr|Parser.NONTERM_atomicExpr
                        | Parser.NONTERM_appExpr|Parser.NONTERM_tupleExpr|Parser.NONTERM_declExpr|Parser.NONTERM_braceExpr
                        | Parser.NONTERM_typedSeqExprBlock
                        | Parser.NONTERM_interactiveExpr -> Some()
                        | _ -> None
                        
                  (* Merge a bunch of pattern non terminals *)
                  let (|NONTERM_Category_Pattern|_|) = function 
                        | Parser.NONTERM_constrPattern|Parser.NONTERM_parenPattern|Parser.NONTERM_atomicPattern -> Some() 
                        | _ -> None
                  
                  (* Merge a bunch of if/then/else non terminals *)
                  let (|NONTERM_Category_IfThenElse|_|) = function
                        | Parser.NONTERM_ifExprThen|Parser.NONTERM_ifExprElifs|Parser.NONTERM_ifExprCases -> Some()
                        | _ -> None
                        
                  (* Merge a bunch of non terminals *)
                  let (|NONTERM_Category_SignatureFile|_|) = function
                        | Parser.NONTERM_signatureFile|Parser.NONTERM_moduleSpfn|Parser.NONTERM_moduleSpfns -> Some()
                        | _ -> None
                  let (|NONTERM_Category_ImplementationFile|_|) = function
                        | Parser.NONTERM_implementationFile|Parser.NONTERM_fileNamespaceImpl|Parser.NONTERM_fileNamespaceImpls -> Some()
                        | _ -> None
                  let (|NONTERM_Category_Definition|_|) = function
                        | Parser.NONTERM_fileModuleImpl|Parser.NONTERM_moduleDefn|Parser.NONTERM_interactiveDefns
                        |Parser.NONTERM_moduleDefns|Parser.NONTERM_moduleDefnsOrExpr -> Some()
                        | _ -> None
                  
                  let (|NONTERM_Category_Type|_|) = function
                        | Parser.NONTERM_typ|Parser.NONTERM_tupleType -> Some()
                        | _ -> None

                  let (|NONTERM_Category_Interaction|_|) = function
                        | Parser.NONTERM_interactiveItemsTerminator|Parser.NONTERM_interaction|Parser.NONTERM__startinteraction -> Some()
                        | _ -> None
         
                  
                  // Canonicalize the categories and check for a unique category
                  ctxt.ReducibleProductions |> List.exists (fun prods -> 
                      match prods 
                            |> List.map Parser.prodIdxToNonTerminal 
                            |> List.map (function 
                                         | NONTERM_Category_Type -> Parser.NONTERM_typ
                                         | NONTERM_Category_Expr -> Parser.NONTERM_declExpr 
                                         | NONTERM_Category_Pattern -> Parser.NONTERM_atomicPattern 
                                         | NONTERM_Category_IfThenElse -> Parser.NONTERM_ifExprThen
                                         | NONTERM_Category_SignatureFile -> Parser.NONTERM_signatureFile
                                         | NONTERM_Category_ImplementationFile -> Parser.NONTERM_implementationFile
                                         | NONTERM_Category_Definition -> Parser.NONTERM_moduleDefn
                                         | NONTERM_Category_Interaction -> Parser.NONTERM_interaction
                                         | nt -> nt)
                            |> Set.ofList 
                            |> Set.toList with 
                      | [Parser.NONTERM_interaction] -> os.Append(NONTERM_interactionE().Format) |> ignore; true
                      | [Parser.NONTERM_hashDirective] -> os.Append(NONTERM_hashDirectiveE().Format) |> ignore; true
                      | [Parser.NONTERM_fieldDecl] -> os.Append(NONTERM_fieldDeclE().Format) |> ignore; true
                      | [Parser.NONTERM_unionCaseRepr] -> os.Append(NONTERM_unionCaseReprE().Format) |> ignore; true
                      | [Parser.NONTERM_localBinding] -> os.Append(NONTERM_localBindingE().Format) |> ignore; true
                      | [Parser.NONTERM_hardwhiteLetBindings] -> os.Append(NONTERM_hardwhiteLetBindingsE().Format) |> ignore; true
                      | [Parser.NONTERM_classDefnMember] -> os.Append(NONTERM_classDefnMemberE().Format) |> ignore; true
                      | [Parser.NONTERM_defnBindings] -> os.Append(NONTERM_defnBindingsE().Format) |> ignore; true
                      | [Parser.NONTERM_classMemberSpfn] -> os.Append(NONTERM_classMemberSpfnE().Format) |> ignore; true
                      | [Parser.NONTERM_valSpfn] -> os.Append(NONTERM_valSpfnE().Format) |> ignore; true
                      | [Parser.NONTERM_tyconSpfn] -> os.Append(NONTERM_tyconSpfnE().Format) |> ignore; true
                      | [Parser.NONTERM_anonLambdaExpr] -> os.Append(NONTERM_anonLambdaExprE().Format) |> ignore; true
                      | [Parser.NONTERM_attrUnionCaseDecl] -> os.Append(NONTERM_attrUnionCaseDeclE().Format) |> ignore; true
                      | [Parser.NONTERM_cPrototype] -> os.Append(NONTERM_cPrototypeE().Format) |> ignore; true
                      | [Parser.NONTERM_objExpr|Parser.NONTERM_objectImplementationMembers] -> os.Append(NONTERM_objectImplementationMembersE().Format) |> ignore; true
                      | [Parser.NONTERM_ifExprThen|Parser.NONTERM_ifExprElifs|Parser.NONTERM_ifExprCases] -> os.Append(NONTERM_ifExprCasesE().Format) |> ignore; true
                      | [Parser.NONTERM_openDecl] -> os.Append(NONTERM_openDeclE().Format) |> ignore; true
                      | [Parser.NONTERM_fileModuleSpec] -> os.Append(NONTERM_fileModuleSpecE().Format) |> ignore; true
                      | [Parser.NONTERM_patternClauses] -> os.Append(NONTERM_patternClausesE().Format) |> ignore; true
                      | [Parser.NONTERM_beginEndExpr] -> os.Append(NONTERM_beginEndExprE().Format) |> ignore; true
                      | [Parser.NONTERM_recdExpr] -> os.Append(NONTERM_recdExprE().Format) |> ignore; true
                      | [Parser.NONTERM_tyconDefn] -> os.Append(NONTERM_tyconDefnE().Format) |> ignore; true
                      | [Parser.NONTERM_exconCore] -> os.Append(NONTERM_exconCoreE().Format) |> ignore; true
                      | [Parser.NONTERM_typeNameInfo] -> os.Append(NONTERM_typeNameInfoE().Format) |> ignore; true
                      | [Parser.NONTERM_attributeList] -> os.Append(NONTERM_attributeListE().Format) |> ignore; true
                      | [Parser.NONTERM_quoteExpr] -> os.Append(NONTERM_quoteExprE().Format) |> ignore; true
                      | [Parser.NONTERM_typeConstraint] -> os.Append(NONTERM_typeConstraintE().Format) |> ignore; true
                      | [NONTERM_Category_ImplementationFile] -> os.Append(NONTERM_Category_ImplementationFileE().Format) |> ignore; true
                      | [NONTERM_Category_Definition] -> os.Append(NONTERM_Category_DefinitionE().Format) |> ignore; true
                      | [NONTERM_Category_SignatureFile] -> os.Append(NONTERM_Category_SignatureFileE().Format) |> ignore; true
                      | [NONTERM_Category_Pattern] -> os.Append(NONTERM_Category_PatternE().Format) |> ignore; true
                      | [NONTERM_Category_Expr] ->  os.Append(NONTERM_Category_ExprE().Format) |> ignore; true
                      | [NONTERM_Category_Type] ->  os.Append(NONTERM_Category_TypeE().Format) |> ignore; true
                      | [Parser.NONTERM_typeArgsActual] -> os.Append(NONTERM_typeArgsActualE().Format) |> ignore; true
                      | _ -> 
                          false)
                          
#if DEBUG
              if not foundInContext then
                  Printf.bprintf os ". (no 'in' context found: %+A)" (List.map (List.map Parser.prodIdxToNonTerminal) ctxt.ReducibleProductions)
#else
              foundInContext |> ignore // suppress unused variable warning in RELEASE
#endif
              let fix (s:string) = s.Replace(SR.GetString("FixKeyword"), "").Replace(SR.GetString("FixSymbol"), "").Replace(SR.GetString("FixReplace"), "")
              match (ctxt.ShiftTokens 
                           |> List.map Parser.tokenTagToTokenId 
                           |> List.filter (function Parser.TOKEN_error | Parser.TOKEN_EOF -> false | _ -> true) 
                           |> List.map tokenIdToText 
                           |> Set.ofList 
                           |> Set.toList) with 
              | [tokenName1]            -> os.Append(TokenName1E().Format (fix tokenName1)) |> ignore
              | [tokenName1;tokenName2] -> os.Append(TokenName1TokenName2E().Format (fix tokenName1) (fix tokenName2)) |> ignore
              | [tokenName1;tokenName2;tokenName3] -> os.Append(TokenName1TokenName2TokenName3E().Format (fix tokenName1) (fix tokenName2) (fix tokenName3)) |> ignore
              | _ -> ()
        (*
              Printf.bprintf os ".\n\n    state = %A\n    token = %A\n    expect (shift) %A\n    expect (reduce) %A\n   prods=%A\n     non terminals: %A" 
                  ctxt.StateStack
                  ctxt.CurrentToken
                  (List.map Parser.tokenTagToTokenId ctxt.ShiftTokens)
                  (List.map Parser.tokenTagToTokenId ctxt.ReduceTokens)
                  ctxt.ReducibleProductions
                  (List.mapSquared Parser.prodIdxToNonTerminal ctxt.ReducibleProductions)
        *)

      | RuntimeCoercionSourceSealed(denv, ty, _) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
          if isTyparTy denv.g ty 
          then os.Append(RuntimeCoercionSourceSealed1E().Format (NicePrint.stringOfTy denv ty)) |> ignore
          else os.Append(RuntimeCoercionSourceSealed2E().Format (NicePrint.stringOfTy denv ty)) |> ignore

      | CoercionTargetSealed(denv, ty, _) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let ty, _cxs= PrettyTypes.PrettifyType denv.g ty
          os.Append(CoercionTargetSealedE().Format (NicePrint.stringOfTy denv ty)) |> ignore

      | UpcastUnnecessary(_) -> 
          os.Append(UpcastUnnecessaryE().Format) |> ignore

      | TypeTestUnnecessary(_) -> 
          os.Append(TypeTestUnnecessaryE().Format) |> ignore

      | QuotationTranslator.IgnoringPartOfQuotedTermWarning (msg, _) -> 
          Printf.bprintf os "%s" msg

      | OverrideDoesntOverride(denv, impl, minfoVirtOpt, g, amap, m) ->
          let sig1 = DispatchSlotChecking.FormatOverride denv impl
          match minfoVirtOpt with 
          | None -> 
              os.Append(OverrideDoesntOverride1E().Format sig1) |> ignore
          | Some minfoVirt ->
              // https://github.com/Microsoft/visualfsharp/issues/35 
              // Improve error message when attempting to override generic return type with unit:
              // we need to check if unit was used as a type argument
              let rec hasUnitTType_app (types: TType list) =
                  match types with
                  | TType_app (maybeUnit, []) :: ts -> 
                      match maybeUnit.TypeAbbrev with
                      | Some ttype when Tastops.isUnitTy g ttype -> true
                      | _ -> hasUnitTType_app ts
                  | _ :: ts -> hasUnitTType_app ts
                  | [] -> false

              match minfoVirt.ApparentEnclosingType with
              | TType_app (t, types) when t.IsFSharpInterfaceTycon && hasUnitTType_app types ->
                  // match abstract member with 'unit' passed as generic argument
                  os.Append(OverrideDoesntOverride4E().Format sig1) |> ignore
              | _ -> 
                  os.Append(OverrideDoesntOverride2E().Format sig1) |> ignore
                  let sig2 = DispatchSlotChecking.FormatMethInfoSig g amap m denv minfoVirt
                  if sig1 <> sig2 then 
                      os.Append(OverrideDoesntOverride3E().Format  sig2) |> ignore

      | UnionCaseWrongArguments (_, n1, n2, _) ->
          os.Append(UnionCaseWrongArgumentsE().Format n2 n1) |> ignore

      | UnionPatternsBindDifferentNames _ -> 
          os.Append(UnionPatternsBindDifferentNamesE().Format) |> ignore

      | ValueNotContained (denv, mref, implVal, sigVal, f) ->
          let text1, text2 = NicePrint.minimalStringsOfTwoValues denv implVal sigVal
          os.Append(f((fullDisplayTextOfModRef mref), text1, text2)) |> ignore

      | ConstrNotContained (denv, v1, v2, f) ->
          os.Append(f((NicePrint.stringOfUnionCase denv v1), (NicePrint.stringOfUnionCase denv v2))) |> ignore

      | ExnconstrNotContained (denv, v1, v2, f) ->
          os.Append(f((NicePrint.stringOfExnDef denv v1), (NicePrint.stringOfExnDef denv v2))) |> ignore

      | FieldNotContained (denv, v1, v2, f) ->
          os.Append(f((NicePrint.stringOfRecdField denv v1), (NicePrint.stringOfRecdField denv v2))) |> ignore

      | RequiredButNotSpecified (_, mref, k, name, _) ->
          let nsb = new System.Text.StringBuilder()
          name nsb;
          os.Append(RequiredButNotSpecifiedE().Format (fullDisplayTextOfModRef mref) k (nsb.ToString())) |> ignore

      | UseOfAddressOfOperator _ -> 
          os.Append(UseOfAddressOfOperatorE().Format) |> ignore

      | DefensiveCopyWarning(s, _) -> os.Append(DefensiveCopyWarningE().Format s) |> ignore

      | DeprecatedThreadStaticBindingWarning(_) -> 
          os.Append(DeprecatedThreadStaticBindingWarningE().Format) |> ignore

      | FunctionValueUnexpected (denv, ty, _) ->
          let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
          let errorText = FunctionValueUnexpectedE().Format (NicePrint.stringOfTy denv ty)
          os.Append errorText |> ignore

      | UnitTypeExpected (denv, ty, _) ->
          let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
          let warningText = UnitTypeExpectedE().Format (NicePrint.stringOfTy denv ty)
          os.Append warningText |> ignore

      | UnitTypeExpectedWithEquality (denv, ty, _) ->
          let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
          let warningText = UnitTypeExpectedWithEqualityE().Format (NicePrint.stringOfTy denv ty)
          os.Append warningText |> ignore

      | UnitTypeExpectedWithPossiblePropertySetter (denv, ty, bindingName, propertyName, _) ->
          let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
          let warningText = UnitTypeExpectedWithPossiblePropertySetterE().Format (NicePrint.stringOfTy denv ty) bindingName propertyName
          os.Append warningText |> ignore

      | UnitTypeExpectedWithPossibleAssignment (denv, ty, isAlreadyMutable, bindingName, _) ->
          let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
          let warningText = 
            if isAlreadyMutable then
                UnitTypeExpectedWithPossibleAssignmentToMutableE().Format (NicePrint.stringOfTy denv ty) bindingName
            else
                UnitTypeExpectedWithPossibleAssignmentE().Format (NicePrint.stringOfTy denv ty)  bindingName
          os.Append warningText |> ignore

      | RecursiveUseCheckedAtRuntime  _ -> 
          os.Append(RecursiveUseCheckedAtRuntimeE().Format) |> ignore

      | LetRecUnsound (_, [v], _) ->  
          os.Append(LetRecUnsound1E().Format v.DisplayName) |> ignore

      | LetRecUnsound (_, path, _) -> 
          let bos = new System.Text.StringBuilder()
          (path.Tail @ [path.Head]) |> List.iter (fun (v:ValRef) -> bos.Append(LetRecUnsoundInnerE().Format v.DisplayName) |> ignore) 
          os.Append(LetRecUnsound2E().Format (List.head path).DisplayName (bos.ToString())) |> ignore

      | LetRecEvaluatedOutOfOrder (_, _, _, _) -> 
          os.Append(LetRecEvaluatedOutOfOrderE().Format) |> ignore

      | LetRecCheckedAtRuntime _ -> 
          os.Append(LetRecCheckedAtRuntimeE().Format) |> ignore

      | SelfRefObjCtor(false, _) -> 
          os.Append(SelfRefObjCtor1E().Format) |> ignore

      | SelfRefObjCtor(true, _) -> 
          os.Append(SelfRefObjCtor2E().Format) |> ignore

      | VirtualAugmentationOnNullValuedType(_) ->
          os.Append(VirtualAugmentationOnNullValuedTypeE().Format) |> ignore

      | NonVirtualAugmentationOnNullValuedType(_) ->
          os.Append(NonVirtualAugmentationOnNullValuedTypeE().Format) |> ignore

      | NonUniqueInferredAbstractSlot(_, denv, bindnm, bvirt1, bvirt2, _) ->
          os.Append(NonUniqueInferredAbstractSlot1E().Format bindnm) |> ignore
          let ty1 = bvirt1.ApparentEnclosingType
          let ty2 = bvirt2.ApparentEnclosingType
          // REVIEW: consider if we need to show _cxs (the type parameter constraints)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(NonUniqueInferredAbstractSlot2E().Format) |> ignore
          if t1 <> t2 then 
              os.Append(NonUniqueInferredAbstractSlot3E().Format t1 t2) |> ignore
          os.Append(NonUniqueInferredAbstractSlot4E().Format) |> ignore

      | Error ((_, s), _) -> os.Append(s) |> ignore

      | ErrorWithSuggestions ((_, s), _, idText, suggestionF) -> 
          os.Append(DecompileOpName s) |> ignore
          let filtered = ErrorResolutionHints.FilterPredictions idText suggestionF
          if List.isEmpty filtered |> not then
              os.Append(ErrorResolutionHints.FormatPredictions DecompileOpName filtered) |> ignore

      | NumberedError ((_, s), _) -> os.Append(s) |> ignore

      | InternalError (s, _) 

      | InvalidArgument s 

      | Failure s  as exn ->
          ignore exn // use the argument, even in non DEBUG
          let f1 = SR.GetString("Failure1")
          let f2 = SR.GetString("Failure2") 
          match s with 
          | f when f = f1 -> os.Append(Failure3E().Format s) |> ignore
          | f when f = f2 -> os.Append(Failure3E().Format s) |> ignore
          | _ -> os.Append(Failure4E().Format s) |> ignore
#if DEBUG
          Printf.bprintf os "\nStack Trace\n%s\n" (exn.ToString())
          if !showAssertForUnexpectedException then 
              System.Diagnostics.Debug.Assert(false, sprintf "Unexpected exception seen in compiler: %s\n%s" s (exn.ToString()))
#endif

      | FullAbstraction(s, _) -> os.Append(FullAbstractionE().Format s) |> ignore

      | WrappedError (exn, _) -> OutputExceptionR os exn

      | PatternMatchCompilation.MatchIncomplete (isComp, cexOpt, _) -> 
          os.Append(MatchIncomplete1E().Format) |> ignore
          match cexOpt with 
          | None -> ()
          | Some (cex, false) ->  os.Append(MatchIncomplete2E().Format cex) |> ignore
          | Some (cex, true) ->  os.Append(MatchIncomplete3E().Format cex) |> ignore
          if isComp then 
              os.Append(MatchIncomplete4E().Format) |> ignore

      | PatternMatchCompilation.EnumMatchIncomplete (isComp, cexOpt, _) ->
          os.Append(EnumMatchIncomplete1E().Format) |> ignore
          match cexOpt with
          | None -> ()
          | Some (cex, false) ->  os.Append(MatchIncomplete2E().Format cex) |> ignore
          | Some (cex, true) ->  os.Append(MatchIncomplete3E().Format cex) |> ignore
          if isComp then
              os.Append(MatchIncomplete4E().Format) |> ignore

      | PatternMatchCompilation.RuleNeverMatched _ -> os.Append(RuleNeverMatchedE().Format) |> ignore

      | ValNotMutable(_, valRef, _) -> os.Append(ValNotMutableE().Format(valRef.DisplayName)) |> ignore

      | ValNotLocal _ -> os.Append(ValNotLocalE().Format) |> ignore

      | ObsoleteError (s, _) 

      | ObsoleteWarning (s, _) -> 
            os.Append(Obsolete1E().Format) |> ignore
            if s <> "" then os.Append(Obsolete2E().Format s) |> ignore

      | Experimental (s, _) -> os.Append(ExperimentalE().Format s) |> ignore

      | PossibleUnverifiableCode _ -> os.Append(PossibleUnverifiableCodeE().Format) |> ignore

      | UserCompilerMessage (msg, _, _) -> os.Append(msg) |> ignore

      | Deprecated(s, _) -> os.Append(DeprecatedE().Format s) |> ignore

      | LibraryUseOnly(_) -> os.Append(LibraryUseOnlyE().Format) |> ignore

      | MissingFields(sl, _) -> os.Append(MissingFieldsE().Format (String.concat "," sl + ".")) |> ignore

      | ValueRestriction(denv, hassig, v, _, _) -> 
          let denv = { denv with showImperativeTyparAnnotations=true }
          let tau = v.TauType
          if hassig then 
              if isFunTy denv.g tau && (arityOfVal v).HasNoArgs then 
                os.Append(ValueRestriction1E().Format
                  v.DisplayName 
                  (NicePrint.stringOfQualifiedValOrMember denv v)
                  v.DisplayName) |> ignore
              else
                os.Append(ValueRestriction2E().Format
                  v.DisplayName 
                  (NicePrint.stringOfQualifiedValOrMember denv v)
                  v.DisplayName) |> ignore
          else
              match v.MemberInfo with 
              | Some(membInfo) when 
                  begin match membInfo.MemberFlags.MemberKind with 
                  | MemberKind.PropertyGet 
                  | MemberKind.PropertySet 
                  | MemberKind.Constructor -> true (* can't infer extra polymorphism *)
                  | _ -> false                     (* can infer extra polymorphism *)
                  end -> 
                      os.Append(ValueRestriction3E().Format (NicePrint.stringOfQualifiedValOrMember denv v)) |> ignore
              | _ -> 
                if isFunTy denv.g tau && (arityOfVal v).HasNoArgs then 
                    os.Append(ValueRestriction4E().Format
                      v.DisplayName
                      (NicePrint.stringOfQualifiedValOrMember denv v)
                      v.DisplayName) |> ignore
                else
                    os.Append(ValueRestriction5E().Format
                      v.DisplayName
                      (NicePrint.stringOfQualifiedValOrMember denv v)
                      v.DisplayName) |> ignore
                

      | Parsing.RecoverableParseError -> os.Append(RecoverableParseErrorE().Format) |> ignore

      | ReservedKeyword (s, _) -> os.Append(ReservedKeywordE().Format s) |> ignore

      | IndentationProblem (s, _) -> os.Append(IndentationProblemE().Format s) |> ignore

      | OverrideInIntrinsicAugmentation(_) -> os.Append(OverrideInIntrinsicAugmentationE().Format) |> ignore

      | OverrideInExtrinsicAugmentation(_) -> os.Append(OverrideInExtrinsicAugmentationE().Format) |> ignore

      | IntfImplInIntrinsicAugmentation(_) -> os.Append(IntfImplInIntrinsicAugmentationE().Format) |> ignore

      | IntfImplInExtrinsicAugmentation(_) -> os.Append(IntfImplInExtrinsicAugmentationE().Format) |> ignore

      | UnresolvedReferenceError(assemblyname, _)

      | UnresolvedReferenceNoRange(assemblyname) ->
          os.Append(UnresolvedReferenceNoRangeE().Format assemblyname) |> ignore

      | UnresolvedPathReference(assemblyname, pathname, _) 

      | UnresolvedPathReferenceNoRange(assemblyname, pathname) ->
          os.Append(UnresolvedPathReferenceNoRangeE().Format pathname assemblyname) |> ignore

      | DeprecatedCommandLineOptionFull(fullText, _) ->
          os.Append(fullText) |> ignore

      | DeprecatedCommandLineOptionForHtmlDoc(optionName, _) ->
          os.Append(FSComp.SR.optsDCLOHtmlDoc(optionName)) |> ignore

      | DeprecatedCommandLineOptionSuggestAlternative(optionName, altOption, _) ->
          os.Append(FSComp.SR.optsDCLODeprecatedSuggestAlternative(optionName, altOption)) |> ignore

      | InternalCommandLineOption(optionName, _) ->
          os.Append(FSComp.SR.optsInternalNoDescription(optionName)) |> ignore

      | DeprecatedCommandLineOptionNoDescription(optionName, _) ->
          os.Append(FSComp.SR.optsDCLONoDescription(optionName)) |> ignore

      | HashIncludeNotAllowedInNonScript(_) ->
          os.Append(HashIncludeNotAllowedInNonScriptE().Format) |> ignore

      | HashReferenceNotAllowedInNonScript(_) ->
          os.Append(HashReferenceNotAllowedInNonScriptE().Format) |> ignore

      | HashDirectiveNotAllowedInNonScript(_) ->
          os.Append(HashDirectiveNotAllowedInNonScriptE().Format) |> ignore

      | FileNameNotResolved(filename, locations, _) -> 
          os.Append(FileNameNotResolvedE().Format filename locations) |> ignore

      | AssemblyNotResolved(originalName, _) ->
          os.Append(AssemblyNotResolvedE().Format originalName) |> ignore

      | IllegalFileNameChar(fileName, invalidChar) ->
          os.Append(FSComp.SR.buildUnexpectedFileNameCharacter(fileName, string invalidChar)|>snd) |> ignore

      | HashLoadedSourceHasIssues(warnings, errors, _) -> 
        let Emit(l:exn list) =
            OutputExceptionR os (List.head l)
        if errors=[] then 
            os.Append(HashLoadedSourceHasIssues1E().Format) |> ignore
            Emit(warnings)
        else
            os.Append(HashLoadedSourceHasIssues2E().Format) |> ignore
            Emit(errors)

      | HashLoadedScriptConsideredSource(_) ->
          os.Append(HashLoadedScriptConsideredSourceE().Format) |> ignore

      | InvalidInternalsVisibleToAssemblyName(badName, fileNameOption) ->      
          match fileNameOption with      
          | Some file -> os.Append(InvalidInternalsVisibleToAssemblyName1E().Format badName file) |> ignore
          | None      -> os.Append(InvalidInternalsVisibleToAssemblyName2E().Format badName) |> ignore

      | LoadedSourceNotFoundIgnoring(filename, _) ->
          os.Append(LoadedSourceNotFoundIgnoringE().Format filename) |> ignore

      | MSBuildReferenceResolutionWarning(code, message, _) 

      | MSBuildReferenceResolutionError(code, message, _) -> 
          os.Append(MSBuildReferenceResolutionErrorE().Format message code) |> ignore

      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          OutputExceptionR os e.InnerException

      | :? FileNotFoundException as e -> Printf.bprintf os "%s" e.Message

      | :? DirectoryNotFoundException as e -> Printf.bprintf os "%s" e.Message

      | :? System.ArgumentException as e -> Printf.bprintf os "%s" e.Message

      | :? System.NotSupportedException as e -> Printf.bprintf os "%s" e.Message

      | :? IOException as e -> Printf.bprintf os "%s" e.Message

      | :? System.UnauthorizedAccessException as e -> Printf.bprintf os "%s" e.Message

      | e -> 
          os.Append(TargetInvocationExceptionWrapperE().Format e.Message) |> ignore
#if DEBUG
          Printf.bprintf os "\nStack Trace\n%s\n" (e.ToString())
          if !showAssertForUnexpectedException then 
              System.Diagnostics.Debug.Assert(false, sprintf "Unknown exception seen in compiler: %s" (e.ToString()))
#endif

    OutputExceptionR os (err.Exception)


// remove any newlines and tabs 
let OutputPhasedDiagnostic (os:System.Text.StringBuilder) (err:PhasedDiagnostic) (flattenErrors:bool) = 
    let buf = new System.Text.StringBuilder()

    OutputPhasedErrorR buf err
    let s = if flattenErrors then ErrorLogger.NormalizeErrorString (buf.ToString()) else buf.ToString()
    
    os.Append(s) |> ignore

let SanitizeFileName fileName implicitIncludeDir =
    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy filename
    //  - if you have a #line directive, e.g. 
    //        # 1000 "Line01.fs"
    //    then it also asserts.  But these are edge cases that can be fixed later, e.g. in bug 4651.
    //System.Diagnostics.Debug.Assert(FileSystem.IsPathRootedShim(fileName), sprintf "filename should be absolute: '%s'" fileName)
    try
        let fullPath = FileSystem.GetFullPathShim(fileName)
        let currentDir = implicitIncludeDir
        
        // if the file name is not rooted in the current directory, return the full path
        if not(fullPath.StartsWith(currentDir)) then
            fullPath
        // if the file name is rooted in the current directory, return the relative path
        else
            fullPath.Replace(currentDir+"\\", "")
    with _ ->
        fileName

[<RequireQualifiedAccess>]
type DiagnosticLocation =
    { Range : range
      File : string
      TextRepresentation : string
      IsEmpty : bool }

[<RequireQualifiedAccess>]
type DiagnosticCanonicalInformation = 
    { ErrorNumber : int
      Subcategory : string
      TextRepresentation : string }

[<RequireQualifiedAccess>]
type DiagnosticDetailedInfo = 
    { Location : DiagnosticLocation option
      Canonical : DiagnosticCanonicalInformation
      Message : string }

[<RequireQualifiedAccess>]
type Diagnostic = 
    | Short of bool * string
    | Long of bool * DiagnosticDetailedInfo

/// returns sequence that contains Diagnostic for the given error + Diagnostic for all related errors
let CollectDiagnostic (implicitIncludeDir, showFullPaths, flattenErrors, errorStyle, isError, err:PhasedDiagnostic) = 
    let outputWhere (showFullPaths, errorStyle) m : DiagnosticLocation = 
        if m = rangeStartup || m = rangeCmdArgs then 
            { Range = m; TextRepresentation = ""; IsEmpty = true; File = "" }
        else
            let file = m.FileName
            let file = if showFullPaths then 
                            Filename.fullpath implicitIncludeDir file
                       else 
                            SanitizeFileName file implicitIncludeDir
            let text, m, file = 
                match errorStyle with
                  | ErrorStyle.EmacsErrors   -> 
                    let file = file.Replace("\\", "/")
                    (sprintf "File \"%s\", line %d, characters %d-%d: " file m.StartLine m.StartColumn m.EndColumn), m, file

                  // We're adjusting the columns here to be 1-based - both for parity with C# and for MSBuild, which assumes 1-based columns for error output
                  | ErrorStyle.DefaultErrors -> 
                    let file = file.Replace('/', System.IO.Path.DirectorySeparatorChar)
                    let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) m.End
                    (sprintf "%s(%d,%d): " file m.StartLine m.StartColumn), m, file

                  // We may also want to change TestErrors to be 1-based
                  | ErrorStyle.TestErrors    -> 
                    let file = file.Replace("/", "\\")
                    let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1) )
                    sprintf "%s(%d,%d-%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file

                  | ErrorStyle.GccErrors     -> 
                    let file = file.Replace('/', System.IO.Path.DirectorySeparatorChar)
                    let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1) )
                    sprintf "%s:%d:%d: " file m.StartLine m.StartColumn, m, file

                  // Here, we want the complete range information so Project Systems can generate proper squiggles
                  | ErrorStyle.VSErrors      -> 
                        // Show prefix only for real files. Otherwise, we just want a truncated error like:
                        //      parse error FS0031 : blah blah
                        if m<>range0 && m<>rangeStartup && m<>rangeCmdArgs then 
                            let file = file.Replace("/", "\\")
                            let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1) )
                            sprintf "%s(%d,%d,%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file
                        else
                            "", m, file
            { Range = m; TextRepresentation = text; IsEmpty = false; File = file }

    match err.Exception with 
    | ReportedError _ -> 
        assert ("" = "Unexpected ReportedError") //  this should never happen 
        Seq.empty
    | StopProcessing -> 
        assert ("" = "Unexpected StopProcessing") // this should never happen 
        Seq.empty
    | _ -> 
        let errors = ResizeArray()
        let report err =
            let OutputWhere(err) = 
                match GetRangeOfDiagnostic err with 
                | Some m -> Some(outputWhere (showFullPaths, errorStyle) m)
                | None -> None

            let OutputCanonicalInformation(subcategory, errorNumber) : DiagnosticCanonicalInformation = 
                let text = 
                    match errorStyle with
                    // Show the subcategory for --vserrors so that we can fish it out in Visual Studio and use it to determine error stickiness.
                    | ErrorStyle.VSErrors -> sprintf "%s %s FS%04d: " subcategory (if isError then "error" else "warning") errorNumber
                    | _ -> sprintf "%s FS%04d: " (if isError then "error" else "warning") errorNumber
                {  ErrorNumber = errorNumber; Subcategory = subcategory; TextRepresentation = text}
        
            let mainError, relatedErrors = SplitRelatedDiagnostics err
            let where = OutputWhere(mainError)
            let canonical = OutputCanonicalInformation(err.Subcategory(), GetDiagnosticNumber mainError)
            let message = 
                let os = System.Text.StringBuilder()
                OutputPhasedDiagnostic os mainError flattenErrors
                os.ToString()
            
            let entry : DiagnosticDetailedInfo = { Location = where; Canonical = canonical; Message = message }
            
            errors.Add ( Diagnostic.Long(isError, entry ) )

            let OutputRelatedError(err:PhasedDiagnostic) =
                match errorStyle with
                // Give a canonical string when --vserror.
                | ErrorStyle.VSErrors -> 
                    let relWhere = OutputWhere(mainError) // mainError?
                    let relCanonical = OutputCanonicalInformation(err.Subcategory(), GetDiagnosticNumber mainError) // Use main error for code
                    let relMessage = 
                        let os = System.Text.StringBuilder()
                        OutputPhasedDiagnostic os err flattenErrors
                        os.ToString()

                    let entry : DiagnosticDetailedInfo = { Location = relWhere; Canonical = relCanonical; Message = relMessage}
                    errors.Add( Diagnostic.Long (isError, entry) )

                | _ -> 
                    let os = System.Text.StringBuilder()
                    OutputPhasedDiagnostic os err flattenErrors
                    errors.Add( Diagnostic.Short(isError, os.ToString()) )
        
            relatedErrors |> List.iter OutputRelatedError

        match err with
#if !NO_EXTENSIONTYPING
        | {Exception = (:? TypeProviderError as tpe)} ->
            tpe.Iter (fun e ->
                let newErr = {err with Exception = e}
                report newErr
            )
#endif
        | x -> report x

        errors :> seq<_>

/// used by fsc.exe and fsi.exe, but not by VS
/// prints error and related errors to the specified StringBuilder
let rec OutputDiagnostic (implicitIncludeDir, showFullPaths, flattenErrors, errorStyle, isError) os (err:PhasedDiagnostic) = 
    
    let errors = CollectDiagnostic (implicitIncludeDir, showFullPaths, flattenErrors, errorStyle, isError, err)
    for e in errors do
        Printf.bprintf os "\n"
        match e with
        | Diagnostic.Short(_, txt) -> 
            os.Append txt |> ignore
        | Diagnostic.Long(_, details) ->
            match details.Location with
            | Some l when not l.IsEmpty -> os.Append(l.TextRepresentation) |> ignore
            | _ -> ()
            os.Append( details.Canonical.TextRepresentation ) |> ignore
            os.Append( details.Message ) |> ignore
      
let OutputDiagnosticContext prefix fileLineFn os err =
    match GetRangeOfDiagnostic err with
    | None   -> ()      
    | Some m -> 
        let filename = m.FileName
        let lineA = m.StartLine
        let lineB = m.EndLine
        let line  = fileLineFn filename lineA
        if line<>"" then 
            let iA    = m.StartColumn
            let iB    = m.EndColumn
            let iLen  = if lineA = lineB then max (iB - iA) 1  else 1
            Printf.bprintf os "%s%s\n"   prefix line
            Printf.bprintf os "%s%s%s\n" prefix (String.make iA '-') (String.make iLen '^')

//----------------------------------------------------------------------------

let GetFSharpCoreLibraryName () = "FSharp.Core"

// If necessary assume a reference to the latest .NET Framework FSharp.Core with which those tools are built.
let GetDefaultFSharpCoreReference () = typeof<list<int>>.Assembly.Location

type private TypeInThisAssembly = class end

// Use the ValueTuple that is executing with the compiler if it is from System.ValueTuple
// or the System.ValueTuple.dll that sits alongside the compiler.  (Note we always ship one with the compiler)
let GetDefaultSystemValueTupleReference () =
    try
        let asm = typeof<System.ValueTuple<int, int>>.Assembly 
        if asm.FullName.StartsWith "System.ValueTuple" then  
            Some asm.Location
        else
            let location = Path.GetDirectoryName(typeof<TypeInThisAssembly>.Assembly.Location)
            let valueTuplePath = Path.Combine(location, "System.ValueTuple.dll")
            if File.Exists(valueTuplePath) then
                Some valueTuplePath
            else
                None
    with _ -> None

let GetFsiLibraryName () = "FSharp.Compiler.Interactive.Settings"  

// This list is the default set of references for "non-project" files. 
//
// These DLLs are
//    (a) included in the environment used for all .fsx files (see service.fs)
//    (b) included in environment for files 'orphaned' from a project context
//            -- for orphaned files (files in VS without a project context)
//            -- for files given on a command line without --noframework set
let DefaultReferencesForScriptsAndOutOfProjectSources(assumeDotNetFramework) = 
    [ if assumeDotNetFramework then 
          yield "System"
          yield "System.Xml" 
          yield "System.Runtime.Remoting"
          yield "System.Runtime.Serialization.Formatters.Soap"
          yield "System.Data"
          yield "System.Drawing"
          yield "System.Core"
          // These are the Portable-profile and .NET Standard 1.6 dependencies of FSharp.Core.dll.  These are needed
          // when an F# sript references an F# profile 7, 78, 259 or .NET Standard 1.6 component which in turn refers 
          // to FSharp.Core for profile 7, 78, 259 or .NET Standard.
          yield "System.Runtime" // lots of types
          yield "System.Linq" // System.Linq.Expressions.Expression<T> 
          yield "System.Reflection" // System.Reflection.ParameterInfo
          yield "System.Linq.Expressions" // System.Linq.IQueryable<T>
          yield "System.Threading.Tasks" // valuetype [System.Threading.Tasks]System.Threading.CancellationToken
          yield "System.IO"  //  System.IO.TextWriter
          //yield "System.Console"  //  System.Console.Out etc.
          yield "System.Net.Requests"  //  System.Net.WebResponse etc.
          yield "System.Collections" // System.Collections.Generic.List<T>
          yield "System.Runtime.Numerics" // BigInteger
          yield "System.Threading"  // OperationCanceledException

          // always include a default reference to System.ValueTuple.dll in scripts and out-of-project sources 
          match GetDefaultSystemValueTupleReference() with  
          | None -> () 
          | Some v -> yield v 

          yield "System.Web"
          yield "System.Web.Services"
          yield "System.Windows.Forms"
          yield "System.Numerics" 
     else
          yield Path.Combine(Path.GetDirectoryName(typeof<System.Object>.Assembly.Location), "mscorlib.dll"); // mscorlib
          yield typeof<System.Console>.Assembly.Location; // System.Console
          yield typeof<System.ComponentModel.DefaultValueAttribute>.Assembly.Location; // System.Runtime
          yield typeof<System.ComponentModel.PropertyChangedEventArgs>.Assembly.Location; // System.ObjectModel             
          yield typeof<System.IO.BufferedStream>.Assembly.Location; // System.IO
          yield typeof<System.Linq.Enumerable>.Assembly.Location; // System.Linq
          //yield typeof<System.Xml.Linq.XDocument>.Assembly.Location; // System.Xml.Linq
          yield typeof<System.Net.WebRequest>.Assembly.Location; // System.Net.Requests
          yield typeof<System.Numerics.BigInteger>.Assembly.Location; // System.Runtime.Numerics
          yield typeof<System.Threading.Tasks.TaskExtensions>.Assembly.Location; // System.Threading.Tasks
          yield typeof<Microsoft.FSharp.Core.MeasureAttribute>.Assembly.Location; // FSharp.Core
    ]


// A set of assemblies to always consider to be system assemblies.  A common set of these can be used a shared 
// resources between projects in the compiler services.  Also all assembles where well-known system types exist
// referenced from TcGlobals must be listed here.
let SystemAssemblies () = 
   HashSet
    [ yield "mscorlib"
      yield "netstandard"
      yield "System.Runtime"
      yield GetFSharpCoreLibraryName() 
      yield "System"
      yield "System.Xml" 
      yield "System.Runtime.Remoting"
      yield "System.Runtime.Serialization.Formatters.Soap"
      yield "System.Data"
      yield "System.Deployment"
      yield "System.Design"
      yield "System.Messaging"
      yield "System.Drawing"
      yield "System.Net"
      yield "System.Web"
      yield "System.Web.Services"
      yield "System.Windows.Forms"
      yield "System.Core"
      yield "System.Runtime"
      yield "System.Observable"
      yield "System.Numerics"
      yield "System.ValueTuple"

      // Additions for coreclr and portable profiles
      yield "System.Collections"
      yield "System.Collections.Concurrent"
      yield "System.Console"
      yield "System.Diagnostics.Debug"
      yield "System.Diagnostics.Tools"
      yield "System.Globalization"
      yield "System.IO"
      yield "System.Linq"
      yield "System.Linq.Expressions"
      yield "System.Linq.Queryable"
      yield "System.Net.Requests"
      yield "System.Reflection"
      yield "System.Reflection.Emit"
      yield "System.Reflection.Emit.ILGeneration"
      yield "System.Reflection.Extensions"
      yield "System.Resources.ResourceManager"
      yield "System.Runtime.Extensions"
      yield "System.Runtime.InteropServices"
      yield "System.Runtime.InteropServices.PInvoke"
      yield "System.Runtime.Numerics"
      yield "System.Text.Encoding"
      yield "System.Text.Encoding.Extensions"
      yield "System.Text.RegularExpressions"
      yield "System.Threading"
      yield "System.Threading.Tasks"
      yield "System.Threading.Tasks.Parallel"
      yield "System.Threading.Thread"
      yield "System.Threading.ThreadPool"
      yield "System.Threading.Timer"

      yield "FSharp.Compiler.Interactive.Settings"
      yield "Microsoft.Win32.Registry"
      yield "System.Diagnostics.Tracing"
      yield "System.Globalization.Calendars"
      yield "System.Reflection.Primitives"
      yield "System.Runtime.Handles"
      yield "Microsoft.Win32.Primitives"
      yield "System.IO.FileSystem"
      yield "System.Net.Primitives"
      yield "System.Net.Sockets"
      yield "System.Private.Uri"
      yield "System.AppContext"
      yield "System.Buffers"
      yield "System.Collections.Immutable"
      yield "System.Diagnostics.DiagnosticSource"
      yield "System.Diagnostics.Process"
      yield "System.Diagnostics.TraceSource"
      yield "System.Globalization.Extensions"
      yield "System.IO.Compression"
      yield "System.IO.Compression.ZipFile"
      yield "System.IO.FileSystem.Primitives"
      yield "System.Net.Http"
      yield "System.Net.NameResolution"
      yield "System.Net.WebHeaderCollection"
      yield "System.ObjectModel"
      yield "System.Reflection.Emit.Lightweight"
      yield "System.Reflection.Metadata"
      yield "System.Reflection.TypeExtensions"
      yield "System.Runtime.InteropServices.RuntimeInformation"
      yield "System.Runtime.Loader"
      yield "System.Security.Claims"
      yield "System.Security.Cryptography.Algorithms"
      yield "System.Security.Cryptography.Cng"
      yield "System.Security.Cryptography.Csp"
      yield "System.Security.Cryptography.Encoding"
      yield "System.Security.Cryptography.OpenSsl"
      yield "System.Security.Cryptography.Primitives"
      yield "System.Security.Cryptography.X509Certificates"
      yield "System.Security.Principal"
      yield "System.Security.Principal.Windows"
      yield "System.Threading.Overlapped"
      yield "System.Threading.Tasks.Extensions"
      yield "System.Xml.ReaderWriter"
      yield "System.Xml.XDocument"

      ] 

// The set of references entered into the TcConfigBuilder for scripts prior to computing
// the load closure. 
//
// REVIEW: it isn't clear if there is any negative effect
// of leaving an assembly off this list.
let BasicReferencesForScriptLoadClosure(useFsiAuxLib, assumeDotNetFramework) = 
    [
     if assumeDotNetFramework then 
         
#if COMPILER_SERVICE_ASSUMES_DOTNETCORE_COMPILATION
         yield Path.Combine(Path.GetDirectoryName(typeof<System.Object>.Assembly.Location), "mscorlib.dll"); // mscorlib
#else
         yield "mscorlib"
#endif
         yield GetDefaultFSharpCoreReference() ] @ // Need to resolve these explicitly so they will be found in the reference assemblies directory which is where the .xml files are.
    DefaultReferencesForScriptsAndOutOfProjectSources(assumeDotNetFramework) @ 
    [ if useFsiAuxLib then yield GetFsiLibraryName () ]

let (++) x s = x @ [s]



//----------------------------------------------------------------------------
// General file name resolver
//--------------------------------------------------------------------------

/// Will return None if the filename is not found.
let TryResolveFileUsingPaths(paths, m, name) =
    let () = 
        try FileSystem.IsPathRootedShim(name)  |> ignore 
        with :? System.ArgumentException as e -> error(Error(FSComp.SR.buildProblemWithFilename(name, e.Message), m))
    if FileSystem.IsPathRootedShim(name) && FileSystem.SafeExists name 
    then Some name 
    else
        let res = paths |> List.tryPick (fun path ->  
                    let n = Path.Combine (path, name)
                    if FileSystem.SafeExists n then  Some n 
                    else None)
        res

/// Will raise FileNameNotResolved if the filename was not found
let ResolveFileUsingPaths(paths, m, name) =
    match TryResolveFileUsingPaths(paths, m, name) with
    | Some(res) -> res
    | None ->
        let searchMessage = String.concat "\n " paths
        raise (FileNameNotResolved(name, searchMessage, m))            

let GetWarningNumber(m, s:string) =
    try
        // Okay so ...
        //      #pragma strips FS of the #pragma "FS0004" and validates the warning number
        //      therefore if we have warning id that starts with a numeric digit we convert it to Some (int32)
        //      anything else is ignored None
        if Char.IsDigit(s.[0]) then Some (int32 s)
        elif s.StartsWith("FS", StringComparison.Ordinal) = true then raise (new ArgumentException())
        else None
    with err ->
        warning(Error(FSComp.SR.buildInvalidWarningNumber(s), m))
        None

let ComputeMakePathAbsolute implicitIncludeDir (path : string) = 
    try  
        // remove any quotation marks from the path first
        let path = path.Replace("\"", "")
        if not (FileSystem.IsPathRootedShim(path)) 
        then Path.Combine (implicitIncludeDir, path)
        else path 
    with 
        :? System.ArgumentException -> path  

//----------------------------------------------------------------------------
// Configuration
//----------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type CompilerTarget = 
    | WinExe 
    | ConsoleExe 
    | Dll 
    | Module
    member x.IsExe = (match x with ConsoleExe | WinExe -> true | _ -> false)

[<RequireQualifiedAccess>]
type ResolveAssemblyReferenceMode = Speculative | ReportErrors

[<RequireQualifiedAccess>]
type CopyFSharpCoreFlag = Yes | No

/// Represents the file or string used for the --version flag
type VersionFlag = 
    | VersionString of string
    | VersionFile of string
    | VersionNone
    member x.GetVersionInfo(implicitIncludeDir) =
        let vstr = x.GetVersionString(implicitIncludeDir)
        try 
            IL.parseILVersion vstr
        with _ -> errorR(Error(FSComp.SR.buildInvalidVersionString(vstr), rangeStartup)); IL.parseILVersion "0.0.0.0"

    member x.GetVersionString(implicitIncludeDir) = 
         match x with 
         | VersionString s -> s
         | VersionFile s ->
             let s = if FileSystem.IsPathRootedShim(s) then s else Path.Combine(implicitIncludeDir, s)
             if not(FileSystem.SafeExists(s)) then 
                 errorR(Error(FSComp.SR.buildInvalidVersionFile(s), rangeStartup)); "0.0.0.0"
             else
                 use is = System.IO.File.OpenText s
                 is.ReadLine()
         | VersionNone -> "0.0.0.0"


/// Represents a reference to an assembly. May be backed by a real assembly on disk, or a cross-project
/// reference backed by information generated by the the compiler service.
type IRawFSharpAssemblyData = 
    ///  The raw list AutoOpenAttribute attributes in the assembly
    abstract GetAutoOpenAttributes : ILGlobals -> string list
    ///  The raw list InternalsVisibleToAttribute attributes in the assembly
    abstract GetInternalsVisibleToAttributes : ILGlobals  -> string list
    ///  The raw IL module definition in the assembly, if any. This is not present for cross-project references
    /// in the language service
    abstract TryGetILModuleDef : unit -> ILModuleDef option
    ///  The raw F# signature data in the assembly, if any
    abstract GetRawFSharpSignatureData : range * ilShortAssemName: string * fileName: string -> (string * (unit -> byte[])) list
    ///  The raw F# optimization data in the assembly, if any
    abstract GetRawFSharpOptimizationData : range * ilShortAssemName: string * fileName: string -> (string * (unit -> byte[])) list
    ///  The table of type forwarders in the assembly
    abstract GetRawTypeForwarders : unit -> ILExportedTypesAndForwarders
    /// The identity of the module
    abstract ILScopeRef : ILScopeRef
    abstract ILAssemblyRefs : ILAssemblyRef list
    abstract ShortAssemblyName : string
    abstract HasAnyFSharpSignatureDataAttribute : bool
    abstract HasMatchingFSharpSignatureDataAttribute : ILGlobals -> bool

/// Cache of time stamps as we traverse a project description
type TimeStampCache(defaultTimeStamp: DateTime) = 
    let files = Dictionary<string, DateTime>()
    let projects = Dictionary<IProjectReference, DateTime>(HashIdentity.Reference)
    member cache.GetFileTimeStamp fileName = 
        let ok, v = files.TryGetValue(fileName)
        if ok then v else 
        let v = 
            if FileSystem.SafeExists(fileName) then
                FileSystem.GetLastWriteTimeShim(fileName)
            else
                defaultTimeStamp            
        files.[fileName] <- v
        v

    member cache.GetProjectReferenceTimeStamp (pr: IProjectReference, ctok) = 
        let ok, v = projects.TryGetValue(pr)
        if ok then v else 
        let v = defaultArg (pr.TryGetLogicalTimeStamp (cache, ctok)) defaultTimeStamp
        projects.[pr] <- v
        v

and IProjectReference = 
    /// The name of the assembly file generated by the project
    abstract FileName : string 

    /// Evaluate raw contents of the assembly file generated by the project
    abstract EvaluateRawContents : CompilationThreadToken -> Cancellable<IRawFSharpAssemblyData option>

    /// Get the logical timestamp that would be the timestamp of the assembly file generated by the project
    ///
    /// For project references this is maximum of the timestamps of all dependent files.
    /// The project is not actually built, nor are any assemblies read, but the timestamps for each dependent file 
    /// are read via the FileSystem.  If the files don't exist, then a default timestamp is used.
    ///
    /// The operation returns None only if it is not possible to create an IncrementalBuilder for the project at all, e.g. if there
    /// are fatal errors in the options for the project.
    abstract TryGetLogicalTimeStamp : TimeStampCache * CompilationThreadToken -> System.DateTime option

type AssemblyReference = 
    | AssemblyReference of range * string * IProjectReference option
    member x.Range = (let (AssemblyReference(m, _, _)) = x in m)
    member x.Text = (let (AssemblyReference(_, text, _)) = x in text)
    member x.ProjectReference = (let (AssemblyReference(_, _, contents)) = x in contents)
    member x.SimpleAssemblyNameIs(name) = 
        (String.Compare(fileNameWithoutExtensionWithValidate false x.Text, name, StringComparison.OrdinalIgnoreCase) = 0) ||
        (let text = x.Text.ToLowerInvariant()
         not (text.Contains "/") && not (text.Contains "\\") && not (text.Contains ".dll") && not (text.Contains ".exe") &&
           try let aname = System.Reflection.AssemblyName(x.Text) in aname.Name = name 
           with _ -> false) 
    override x.ToString() = sprintf "AssemblyReference(%s)" x.Text

type UnresolvedAssemblyReference = UnresolvedAssemblyReference of string * AssemblyReference list
#if !NO_EXTENSIONTYPING
type ResolvedExtensionReference = ResolvedExtensionReference of string * AssemblyReference list * Tainted<ITypeProvider> list
#endif

type ImportedBinary = 
    { FileName: string
      RawMetadata: IRawFSharpAssemblyData 
#if !NO_EXTENSIONTYPING
      ProviderGeneratedAssembly: System.Reflection.Assembly option
      IsProviderGenerated: bool
      ProviderGeneratedStaticLinkMap : ProvidedAssemblyStaticLinkingMap option
#endif
      ILAssemblyRefs : ILAssemblyRef list
      ILScopeRef: ILScopeRef }

type ImportedAssembly = 
    { ILScopeRef: ILScopeRef 
      FSharpViewOfMetadata: CcuThunk
      AssemblyAutoOpenAttributes: string list
      AssemblyInternalsVisibleToAttributes: string list
#if !NO_EXTENSIONTYPING
      IsProviderGenerated: bool
      mutable TypeProviders: Tainted<Microsoft.FSharp.Core.CompilerServices.ITypeProvider> list
#endif
      FSharpOptimizationData : Microsoft.FSharp.Control.Lazy<Option<Optimizer.LazyModuleInfo>> }

type AvailableImportedAssembly =
    | ResolvedImportedAssembly of ImportedAssembly
    | UnresolvedImportedAssembly of string

type CcuLoadFailureAction = 
    | RaiseError
    | ReturnNone

[<NoEquality; NoComparison>]
type TcConfigBuilder =
    { mutable primaryAssembly : PrimaryAssembly
      mutable autoResolveOpenDirectivesToDlls: bool
      mutable noFeedback: bool
      mutable stackReserveSize: int32 option
      mutable implicitIncludeDir: string (* normally "." *)
      mutable openDebugInformationForLaterStaticLinking: bool (* only for --standalone *)
      defaultFSharpBinariesDir: string
      mutable compilingFslib: bool
      mutable compilingFslib20: string option
      mutable compilingFslib40: bool
      mutable compilingFslibNoBigInt: bool
      mutable useIncrementalBuilder: bool
      mutable includes: string list
      mutable implicitOpens: string list
      mutable useFsiAuxLib: bool
      mutable framework: bool
      mutable resolutionEnvironment : ReferenceResolver.ResolutionEnvironment
      mutable implicitlyResolveAssemblies: bool
      mutable light: bool option
      mutable conditionalCompilationDefines: string list
      mutable loadedSources: (range * string) list
      mutable referencedDLLs : AssemblyReference list
      mutable projectReferences : IProjectReference list
      mutable knownUnresolvedReferences : UnresolvedAssemblyReference list
      reduceMemoryUsage: ReduceMemoryFlag
      mutable subsystemVersion : int * int
      mutable useHighEntropyVA : bool
      mutable inputCodePage: int option
      mutable embedResources : string list
      mutable errorSeverityOptions: FSharpErrorSeverityOptions
      mutable mlCompatibility: bool
      mutable checkOverflow: bool
      mutable showReferenceResolutions:bool
      mutable outputFile : string option
      mutable platform : ILPlatform option
      mutable prefer32Bit : bool
      mutable useSimpleResolution : bool
      mutable target : CompilerTarget
      mutable debuginfo : bool
      mutable testFlagEmitFeeFeeAs100001 : bool
      mutable dumpDebugInfo : bool
      mutable debugSymbolFile : string option
      (* Backend configuration *)
      mutable typeCheckOnly : bool
      mutable parseOnly : bool
      mutable importAllReferencesOnly : bool
      mutable simulateException : string option
      mutable printAst : bool
      mutable tokenizeOnly : bool
      mutable testInteractionParser : bool
      mutable reportNumDecls : bool
      mutable printSignature : bool
      mutable printSignatureFile : string
      mutable xmlDocOutputFile : string option
      mutable stats : bool
      mutable generateFilterBlocks : bool (* don't generate filter blocks due to bugs on Mono *)

      mutable signer : string option
      mutable container : string option

      mutable delaysign : bool
      mutable publicsign : bool
      mutable version : VersionFlag 
      mutable metadataVersion : string option
      mutable standalone : bool
      mutable extraStaticLinkRoots : string list 
      mutable noSignatureData : bool
      mutable onlyEssentialOptimizationData : bool
      mutable useOptimizationDataFile : bool
      mutable jitTracking : bool
      mutable portablePDB : bool
      mutable embeddedPDB : bool
      mutable embedAllSource : bool
      mutable embedSourceList : string list 
      mutable sourceLink : string

      mutable ignoreSymbolStoreSequencePoints : bool
      mutable internConstantStrings : bool
      mutable extraOptimizationIterations : int

      mutable win32res : string 
      mutable win32manifest : string
      mutable includewin32manifest : bool
      mutable linkResources : string list
      mutable legacyReferenceResolver: ReferenceResolver.Resolver 

      mutable showFullPaths : bool
      mutable errorStyle : ErrorStyle
      mutable utf8output : bool
      mutable flatErrors: bool

      mutable maxErrors : int
      mutable abortOnError : bool (* intended for fsi scripts that should exit on first error *)
      mutable baseAddress : int32 option
#if DEBUG
      mutable showOptimizationData : bool
#endif
      mutable showTerms     : bool (* show terms between passes? *)
      mutable writeTermsToFiles : bool (* show terms to files? *)
      mutable doDetuple     : bool (* run detuple pass? *)
      mutable doTLR         : bool (* run TLR     pass? *)
      mutable doFinalSimplify : bool (* do final simplification pass *)
      mutable optsOn        : bool (* optimizations are turned on *)
      mutable optSettings   : Optimizer.OptimizationSettings 
      mutable emitTailcalls : bool
      mutable deterministic : bool
      mutable preferredUiLang: string option
      mutable lcid          : int option
      mutable productNameForBannerText : string
      /// show the MS (c) notice, e.g. with help or fsi? 
      mutable showBanner  : bool
        
      /// show times between passes? 
      mutable showTimes : bool
      mutable showLoadedAssemblies : bool
      mutable continueAfterParseFailure : bool
#if !NO_EXTENSIONTYPING
      /// show messages about extension type resolution?
      mutable showExtensionTypeMessages : bool
#endif

      /// pause between passes? 
      mutable pause : bool
      /// whenever possible, emit callvirt instead of call
      mutable alwaysCallVirt : bool

      /// if true, strip away data that would not be of use to end users, but is useful to us for debugging
      // REVIEW: "stripDebugData"?
      mutable noDebugData : bool

      /// if true, indicates all type checking and code generation is in the context of fsi.exe
      isInteractive : bool
      isInvalidationSupported : bool

      /// used to log sqm data

      /// if true - every expression in quotations will be augmented with full debug info (filename, location in file)
      mutable emitDebugInfoInQuotations : bool

      mutable exename : string option
      
      // If true - the compiler will copy FSharp.Core.dll along the produced binaries
      mutable copyFSharpCore : CopyFSharpCoreFlag

      /// When false FSI will lock referenced assemblies requiring process restart, false = disable Shadow Copy false (*default*)
      mutable shadowCopyReferences : bool

     /// A function to call to try to get an object that acts as a snapshot of the metadata section of a .NET binary,
     /// and from which we can read the metadata. Only used when metadataOnly=true.
      mutable tryGetMetadataSnapshot : ILReaderTryGetMetadataSnapshot

      }

    static member Initial =
        {
#if COMPILER_SERVICE_ASSUMES_DOTNETCORE_COMPILATION
          primaryAssembly = PrimaryAssembly.System_Runtime // defaut value, can be overridden using the command line switch
#else
          primaryAssembly = PrimaryAssembly.Mscorlib // defaut value, can be overridden using the command line switch
#endif          
          light = None
          noFeedback = false
          stackReserveSize = None
          conditionalCompilationDefines = []
          implicitIncludeDir = String.Empty
          autoResolveOpenDirectivesToDlls = false
          openDebugInformationForLaterStaticLinking = false
          defaultFSharpBinariesDir = String.Empty
          compilingFslib = false
          compilingFslib20 = None
          compilingFslib40 = false
          compilingFslibNoBigInt = false
          useIncrementalBuilder = false
          useFsiAuxLib = false
          implicitOpens = []
          includes = []
          resolutionEnvironment = ResolutionEnvironment.EditingOrCompilation false
          framework = true
          implicitlyResolveAssemblies = true
          referencedDLLs = []
          projectReferences = []
          knownUnresolvedReferences = []
          loadedSources = []
          errorSeverityOptions = FSharpErrorSeverityOptions.Default
          embedResources = []
          inputCodePage = None
          reduceMemoryUsage = ReduceMemoryFlag.Yes // always gets set explicitly 
          subsystemVersion = 4, 0 // per spec for 357994
          useHighEntropyVA = false
          mlCompatibility = false
          checkOverflow = false
          showReferenceResolutions = false
          outputFile = None
          platform = None
          prefer32Bit = false
          useSimpleResolution = runningOnMono
          target = CompilerTarget.ConsoleExe
          debuginfo = false
          testFlagEmitFeeFeeAs100001 = false
          dumpDebugInfo = false
          debugSymbolFile = None          

          (* Backend configuration *)
          typeCheckOnly = false
          parseOnly = false
          importAllReferencesOnly = false
          simulateException = None
          printAst = false
          tokenizeOnly = false
          testInteractionParser = false
          reportNumDecls = false
          printSignature = false
          printSignatureFile = ""
          xmlDocOutputFile = None
          stats = false
          generateFilterBlocks = false (* don't generate filter blocks *)

          signer = None
          container = None
          maxErrors = 100
          abortOnError = false
          baseAddress = None

          delaysign = false
          publicsign = false
          version = VersionNone
          metadataVersion = None
          standalone = false
          extraStaticLinkRoots = []
          noSignatureData = false
          onlyEssentialOptimizationData = false
          useOptimizationDataFile = false
          jitTracking = true
          portablePDB = true
          embeddedPDB = false
          embedAllSource = false
          embedSourceList = []
          sourceLink = ""
          ignoreSymbolStoreSequencePoints = false
          internConstantStrings = true
          extraOptimizationIterations = 0

          win32res = ""
          win32manifest = ""
          includewin32manifest = true
          linkResources = []
          legacyReferenceResolver = null
          showFullPaths = false
          errorStyle = ErrorStyle.DefaultErrors

          utf8output = false
          flatErrors = false

 #if DEBUG
          showOptimizationData = false
 #endif
          showTerms = false
          writeTermsToFiles = false

          doDetuple = false
          doTLR = false
          doFinalSimplify = false
          optsOn = false
          optSettings = Optimizer.OptimizationSettings.Defaults
          emitTailcalls = true
          deterministic = false
          preferredUiLang = None
          lcid = None
          // See bug 6071 for product banner spec
          productNameForBannerText = FSComp.SR.buildProductName(FSharpEnvironment.FSharpBannerVersion)
          showBanner = true
          showTimes = false
          showLoadedAssemblies = false
          continueAfterParseFailure = false
#if !NO_EXTENSIONTYPING
          showExtensionTypeMessages = false
#endif
          pause = false 
          alwaysCallVirt = true
          noDebugData = false
          isInteractive = false
          isInvalidationSupported = false
          emitDebugInfoInQuotations = false
          exename = None
          copyFSharpCore = CopyFSharpCoreFlag.No
          shadowCopyReferences = false
          tryGetMetadataSnapshot = (fun _ -> None)
        }

    static member CreateNew(legacyReferenceResolver, defaultFSharpBinariesDir, reduceMemoryUsage, implicitIncludeDir,
                            isInteractive, isInvalidationSupported, defaultCopyFSharpCore, tryGetMetadataSnapshot) =

        Debug.Assert(FileSystem.IsPathRootedShim(implicitIncludeDir), sprintf "implicitIncludeDir should be absolute: '%s'" implicitIncludeDir)

        if (String.IsNullOrEmpty(defaultFSharpBinariesDir)) then
            failwith "Expected a valid defaultFSharpBinariesDir"

        { TcConfigBuilder.Initial with 
            implicitIncludeDir = implicitIncludeDir
            defaultFSharpBinariesDir = defaultFSharpBinariesDir
            reduceMemoryUsage = reduceMemoryUsage
            legacyReferenceResolver = legacyReferenceResolver
            isInteractive = isInteractive
            isInvalidationSupported = isInvalidationSupported
            copyFSharpCore = defaultCopyFSharpCore
            tryGetMetadataSnapshot = tryGetMetadataSnapshot
        }

    member tcConfigB.ResolveSourceFile(m, nm, pathLoadedFrom) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        ResolveFileUsingPaths(tcConfigB.includes @ [pathLoadedFrom], m, nm)

    /// Decide names of output file, pdb and assembly
    member tcConfigB.DecideNames (sourceFiles) =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        if sourceFiles = [] then errorR(Error(FSComp.SR.buildNoInputsSpecified(), rangeCmdArgs))
        let ext() = match tcConfigB.target with CompilerTarget.Dll -> ".dll" | CompilerTarget.Module -> ".netmodule" | CompilerTarget.ConsoleExe | CompilerTarget.WinExe -> ".exe"
        let implFiles = sourceFiles |> List.filter (fun lower -> List.exists (Filename.checkSuffix (String.lowercase lower)) FSharpImplFileSuffixes)
        let outfile = 
            match tcConfigB.outputFile, List.rev implFiles with 
            | None, [] -> "out" + ext()
            | None, h :: _  -> 
                let basic = fileNameOfPath h
                let modname = try Filename.chopExtension basic with _ -> basic
                modname+(ext())
            | Some f, _ -> f
        let assemblyName = 
            let baseName = fileNameOfPath outfile
            (fileNameWithoutExtension baseName)

        let pdbfile = 
            if tcConfigB.debuginfo then
              Some (match tcConfigB.debugSymbolFile with 
                    | None -> Microsoft.FSharp.Compiler.AbstractIL.ILPdbWriter.getDebugFileName outfile tcConfigB.portablePDB
#if ENABLE_MONO_SUPPORT
                    | Some _ when runningOnMono ->
                        // On Mono, the name of the debug file has to be "<assemblyname>.mdb" so specifying it explicitly is an error
                        warning(Error(FSComp.SR.ilwriteMDBFileNameCannotBeChangedWarning(), rangeCmdArgs))
                        Microsoft.FSharp.Compiler.AbstractIL.ILPdbWriter.getDebugFileName outfile tcConfigB.portablePDB
#endif
                    | Some f -> f)   
            elif (tcConfigB.debugSymbolFile <> None) && (not (tcConfigB.debuginfo)) then
                error(Error(FSComp.SR.buildPdbRequiresDebug(), rangeStartup))  
            else
                None
        tcConfigB.outputFile <- Some(outfile)
        outfile, pdbfile, assemblyName

    member tcConfigB.TurnWarningOff(m, s:string) =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        match GetWarningNumber(m, s) with 
        | None -> ()
        | Some n -> 
            // nowarn:62 turns on mlCompatibility, e.g. shows ML compat items in intellisense menus
            if n = 62 then tcConfigB.mlCompatibility <- true
            tcConfigB.errorSeverityOptions <-
                { tcConfigB.errorSeverityOptions with WarnOff = ListSet.insert (=) n tcConfigB.errorSeverityOptions.WarnOff }

    member tcConfigB.TurnWarningOn(m, s:string) =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        match GetWarningNumber(m, s) with 
        | None -> ()
        | Some n -> 
            // warnon 62 turns on mlCompatibility, e.g. shows ML compat items in intellisense menus
            if n = 62 then tcConfigB.mlCompatibility <- false
            tcConfigB.errorSeverityOptions <-
                { tcConfigB.errorSeverityOptions with WarnOn = ListSet.insert (=) n tcConfigB.errorSeverityOptions.WarnOn }

    member tcConfigB.AddIncludePath (m, path, pathIncludedFrom) = 
        let absolutePath = ComputeMakePathAbsolute pathIncludedFrom path
        let ok = 
            let existsOpt = 
                try Some(Directory.Exists(absolutePath)) 
                with e -> warning(Error(FSComp.SR.buildInvalidSearchDirectory(path), m)); None
            match existsOpt with 
            | Some(exists) -> 
                if not exists then warning(Error(FSComp.SR.buildSearchDirectoryNotFound(absolutePath), m))         
                exists
            | None -> false
        if ok && not (List.contains absolutePath tcConfigB.includes) then 
           tcConfigB.includes <- tcConfigB.includes ++ absolutePath

    member tcConfigB.AddLoadedSource(m, path, pathLoadedFrom) =
        if FileSystem.IsInvalidPathShim(path) then
            warning(Error(FSComp.SR.buildInvalidFilename(path), m))    
        else 
            let path = 
                match TryResolveFileUsingPaths(tcConfigB.includes @ [pathLoadedFrom], m, path) with 
                | Some(path) -> path
                | None ->
                    // File doesn't exist in the paths. Assume it will be in the load-ed from directory.
                    ComputeMakePathAbsolute pathLoadedFrom path
            if not (List.contains path (List.map snd tcConfigB.loadedSources)) then 
                tcConfigB.loadedSources <- tcConfigB.loadedSources ++ (m, path)

    member tcConfigB.AddEmbeddedSourceFile (file) = 
        tcConfigB.embedSourceList <- tcConfigB.embedSourceList ++ file

    member tcConfigB.AddEmbeddedResource filename =
        tcConfigB.embedResources <- tcConfigB.embedResources ++ filename

    member tcConfigB.AddReferencedAssemblyByPath (m, path) = 
        if FileSystem.IsInvalidPathShim(path) then
            warning(Error(FSComp.SR.buildInvalidAssemblyName(path), m))
        elif not (tcConfigB.referencedDLLs  |> List.exists (fun ar2 -> m=ar2.Range && path=ar2.Text)) then // NOTE: We keep same paths if range is different.
             let projectReference = tcConfigB.projectReferences |> List.tryPick (fun pr -> if pr.FileName = path then Some pr else None)
             tcConfigB.referencedDLLs <- tcConfigB.referencedDLLs ++ AssemblyReference(m, path, projectReference)
             
    member tcConfigB.RemoveReferencedAssemblyByPath (m, path) =
        tcConfigB.referencedDLLs <- tcConfigB.referencedDLLs |> List.filter (fun ar-> ar.Range <> m || ar.Text <> path)
    
    static member SplitCommandLineResourceInfo ri = 
        if String.contains ri ',' then 
            let p = String.index ri ',' 
            let file = String.sub ri 0 p 
            let rest = String.sub ri (p+1) (String.length ri - p - 1) 
            if String.contains rest ',' then 
                let p = String.index rest ',' 
                let name = String.sub rest 0 p+".resources" 
                let pubpri = String.sub rest (p+1) (rest.Length - p - 1) 
                if pubpri = "public" then file, name, ILResourceAccess.Public 
                elif pubpri = "private" then file, name, ILResourceAccess.Private
                else error(Error(FSComp.SR.buildInvalidPrivacy(pubpri), rangeStartup))
            else 
                file, rest, ILResourceAccess.Public
        else 
            ri, fileNameOfPath ri, ILResourceAccess.Public 


let OpenILBinary(filename, reduceMemoryUsage, ilGlobalsOpt, pdbDirPath, shadowCopyReferences, tryGetMetadataSnapshot) = 
      let ilGlobals   = 
          // ILScopeRef.Local can be used only for primary assembly (mscorlib or System.Runtime) itself
          // Remaining assemblies should be opened using existing ilGlobals (so they can properly locate fundamental types)
          match ilGlobalsOpt with 
          | None -> mkILGlobals ILScopeRef.Local
          | Some g -> g

      let opts : ILReaderOptions = 
          { ilGlobals = ilGlobals
            metadataOnly = MetadataOnlyFlag.Yes
            reduceMemoryUsage = reduceMemoryUsage
            pdbDirPath = pdbDirPath
            tryGetMetadataSnapshot = tryGetMetadataSnapshot } 
                      
      let location =
#if !FX_RESHAPED_REFLECTION // shadow copy not supported
          // In order to use memory mapped files on the shadow copied version of the Assembly, we `preload the assembly
          // We swallow all exceptions so that we do not change the exception contract of this API
          if shadowCopyReferences then 
            try
              System.Reflection.Assembly.ReflectionOnlyLoadFrom(filename).Location
            with e -> filename
          else
#else
            ignore shadowCopyReferences
#endif
            filename
      OpenILModuleReader location opts

#if DEBUG
[<System.Diagnostics.DebuggerDisplayAttribute("AssemblyResolution({resolvedPath})")>]
#endif
type AssemblyResolution = 
    { originalReference : AssemblyReference
      resolvedPath : string    
      prepareToolTip : unit -> string
      sysdir : bool 
      ilAssemblyRef : ILAssemblyRef option ref
    }
    override this.ToString() = sprintf "%s%s" (if this.sysdir then "[sys]" else "") this.resolvedPath

    member this.ProjectReference = this.originalReference.ProjectReference

    /// Compute the ILAssemblyRef for a resolved assembly.  This is done by reading the binary if necessary. The result
    /// is cached.
    /// 
    /// For project references in the language service, this would result in a build of the project.
    /// This is because ``EvaluateRawContents(ctok)`` is used.  However this path is only currently used
    /// in fsi.fs, which does not use project references.
    //
    member this.GetILAssemblyRef(ctok, reduceMemoryUsage, tryGetMetadataSnapshot) = 
      cancellable {
        match !this.ilAssemblyRef with 
        | Some(assref) -> return assref
        | None ->
            let! assRefOpt = 
              cancellable {
                match this.ProjectReference with 
                | Some r ->   
                    let! contents = r.EvaluateRawContents(ctok)
                    match contents with 
                    | None -> return None
                    | Some contents -> 
                        match contents.ILScopeRef with 
                        | ILScopeRef.Assembly aref -> return Some aref
                        |  _ -> return None
                | None -> return None
              }
            let assRef = 
                match assRefOpt with 
                | Some aref -> aref
                | None -> 
                    let readerSettings : ILReaderOptions = 
                        { pdbDirPath=None
                          ilGlobals = EcmaMscorlibILGlobals
                          reduceMemoryUsage = reduceMemoryUsage
                          metadataOnly = MetadataOnlyFlag.Yes
                          tryGetMetadataSnapshot = tryGetMetadataSnapshot } 
                    use reader = OpenILModuleReader this.resolvedPath readerSettings
                    mkRefToILAssembly reader.ILModuleDef.ManifestOfAssembly
            this.ilAssemblyRef := Some(assRef)
            return assRef
      }

//----------------------------------------------------------------------------
// Names to match up refs and defs for assemblies and modules
//--------------------------------------------------------------------------

let GetNameOfILModule (m: ILModuleDef) = 
    match m.Manifest with 
    | Some manifest -> manifest.Name
    | None -> m.Name


let MakeScopeRefForILModule (ilModule: ILModuleDef) = 
    match ilModule.Manifest with 
    | Some m -> ILScopeRef.Assembly (mkRefToILAssembly m)
    | None -> ILScopeRef.Module (mkRefToILModule ilModule)

let GetCustomAttributesOfILModule (ilModule:ILModuleDef) = 
    (match ilModule.Manifest with Some m -> m.CustomAttrs | None -> ilModule.CustomAttrs).AsList 

let GetAutoOpenAttributes ilg ilModule = 
    ilModule |> GetCustomAttributesOfILModule |> List.choose (TryFindAutoOpenAttr ilg)

let GetInternalsVisibleToAttributes ilg ilModule = 
    ilModule |> GetCustomAttributesOfILModule |> List.choose (TryFindInternalsVisibleToAttr ilg)
    
//----------------------------------------------------------------------------
// TcConfig 
//--------------------------------------------------------------------------

[<Sealed>]
/// This type is immutable and must be kept as such. Do not extract or mutate the underlying data except by cloning it.
type TcConfig private (data : TcConfigBuilder, validate:bool) =

    // Validate the inputs - this helps ensure errors in options are shown in visual studio rather than only when built
    // However we only validate a minimal number of options at the moment
    do if validate then try data.version.GetVersionInfo(data.implicitIncludeDir) |> ignore with e -> errorR(e) 

    // clone the input builder to ensure nobody messes with it.
    let data = { data with pause = data.pause }

    let computeKnownDllReference(libraryName) = 
        let defaultCoreLibraryReference = AssemblyReference(range0, libraryName+".dll", None)
        let nameOfDll(r:AssemblyReference) = 
            let filename = ComputeMakePathAbsolute data.implicitIncludeDir r.Text
            if FileSystem.SafeExists(filename) then 
                r, Some(filename)
            else   
                // If the file doesn't exist, let reference resolution logic report the error later...
                defaultCoreLibraryReference, if r.Range =rangeStartup then Some(filename) else None
        match data.referencedDLLs |> List.filter (fun assemblyReference -> assemblyReference.SimpleAssemblyNameIs libraryName) with
        | [r] -> nameOfDll r
        | [] -> 
            defaultCoreLibraryReference, None
        | r:: _ -> 
            // Recover by picking the first one.
            errorR(Error(FSComp.SR.buildMultipleReferencesNotAllowed(libraryName), rangeCmdArgs)) 
            nameOfDll(r)

    // Look for an explicit reference to mscorlib and use that to compute clrRoot and targetFrameworkVersion
    let primaryAssemblyReference, primaryAssemblyExplicitFilenameOpt = computeKnownDllReference(data.primaryAssembly.Name)
    let fslibReference, fslibExplicitFilenameOpt = 
        let (_, fileNameOpt) as res = computeKnownDllReference(GetFSharpCoreLibraryName())
        match fileNameOpt with
        | None -> 
            // if FSharp.Core was not provided explicitly - use version that was referenced by compiler
            AssemblyReference(range0, GetDefaultFSharpCoreReference(), None), None
        | _ -> res

    // If either mscorlib.dll/System.Runtime.dll/netstandard.dll or FSharp.Core.dll are explicitly specified then we require the --noframework flag.
    // The reason is that some non-default frameworks may not have the default dlls. For example, Client profile does
    // not have System.Web.dll.
    do if ((primaryAssemblyExplicitFilenameOpt.IsSome || fslibExplicitFilenameOpt.IsSome) && data.framework) then
            error(Error(FSComp.SR.buildExplicitCoreLibRequiresNoFramework("--noframework"), rangeStartup))

    let clrRootValue, targetFrameworkVersionValue  = 
        match primaryAssemblyExplicitFilenameOpt with
        | Some(primaryAssemblyFilename) ->
            let filename = ComputeMakePathAbsolute data.implicitIncludeDir primaryAssemblyFilename
            try 
                use ilReader = OpenILBinary(filename, data.reduceMemoryUsage, None, None, data.shadowCopyReferences, data.tryGetMetadataSnapshot)
                let ilModule = ilReader.ILModuleDef
                match ilModule.ManifestOfAssembly.Version with 
                | Some(v1, v2, _, _) -> 
                    let clrRoot = Some(Path.GetDirectoryName(FileSystem.GetFullPathShim(filename)))
                    clrRoot, (sprintf "v%d.%d" v1 v2)
                | _ -> 
                    failwith (FSComp.SR.buildCouldNotReadVersionInfoFromMscorlib())
            with e ->
                error(Error(FSComp.SR.buildErrorOpeningBinaryFile(filename, e.Message), rangeStartup))
        | _ ->
#if !ENABLE_MONO_SUPPORT
            // TODO:  we have to get msbuild out of this
            if data.useSimpleResolution then
                None, ""
            else
#endif
                None, data.legacyReferenceResolver.HighestInstalledNetFrameworkVersion()

    let systemAssemblies = SystemAssemblies ()

    // Look for an explicit reference to FSharp.Core and use that to compute fsharpBinariesDir
    // FUTURE: remove this, we only read the binary for the exception it raises
    let fsharpBinariesDirValue = 
// NOTE: It's not clear why this behaviour has been changed for the NETSTANDARD compilations of the F# compiler
#if NETSTANDARD1_6 || NETSTANDARD2_0
        data.defaultFSharpBinariesDir
#else
        match fslibExplicitFilenameOpt with
        | Some fslibFilename ->
            let filename = ComputeMakePathAbsolute data.implicitIncludeDir fslibFilename
            if fslibReference.ProjectReference.IsNone then 
                try 
                    use ilReader = OpenILBinary(filename, data.reduceMemoryUsage, None, None, data.shadowCopyReferences, data.tryGetMetadataSnapshot)
                    ()
                with e -> 
                    error(Error(FSComp.SR.buildErrorOpeningBinaryFile(filename, e.Message), rangeStartup))
                
            let fslibRoot = Path.GetDirectoryName(FileSystem.GetFullPathShim(filename))
            fslibRoot
        | _ ->
            data.defaultFSharpBinariesDir
#endif

    member x.primaryAssembly = data.primaryAssembly
    member x.autoResolveOpenDirectivesToDlls = data.autoResolveOpenDirectivesToDlls
    member x.noFeedback = data.noFeedback
    member x.stackReserveSize = data.stackReserveSize   
    member x.implicitIncludeDir = data.implicitIncludeDir
    member x.openDebugInformationForLaterStaticLinking = data.openDebugInformationForLaterStaticLinking
    member x.fsharpBinariesDir = fsharpBinariesDirValue
    member x.compilingFslib = data.compilingFslib
    member x.compilingFslib20 = data.compilingFslib20
    member x.compilingFslib40 = data.compilingFslib40
    member x.compilingFslibNoBigInt = data.compilingFslibNoBigInt
    member x.useIncrementalBuilder = data.useIncrementalBuilder
    member x.includes = data.includes
    member x.implicitOpens = data.implicitOpens
    member x.useFsiAuxLib = data.useFsiAuxLib
    member x.framework = data.framework
    member x.implicitlyResolveAssemblies = data.implicitlyResolveAssemblies
    member x.resolutionEnvironment = data.resolutionEnvironment
    member x.light = data.light
    member x.conditionalCompilationDefines = data.conditionalCompilationDefines
    member x.loadedSources = data.loadedSources
    member x.referencedDLLs = data.referencedDLLs
    member x.knownUnresolvedReferences = data.knownUnresolvedReferences
    member x.clrRoot = clrRootValue
    member x.reduceMemoryUsage = data.reduceMemoryUsage
    member x.subsystemVersion = data.subsystemVersion
    member x.useHighEntropyVA = data.useHighEntropyVA
    member x.inputCodePage = data.inputCodePage
    member x.embedResources  = data.embedResources
    member x.errorSeverityOptions = data.errorSeverityOptions
    member x.mlCompatibility = data.mlCompatibility
    member x.checkOverflow = data.checkOverflow
    member x.showReferenceResolutions = data.showReferenceResolutions
    member x.outputFile  = data.outputFile
    member x.platform  = data.platform
    member x.prefer32Bit = data.prefer32Bit
    member x.useSimpleResolution  = data.useSimpleResolution
    member x.target  = data.target
    member x.debuginfo  = data.debuginfo
    member x.testFlagEmitFeeFeeAs100001 = data.testFlagEmitFeeFeeAs100001
    member x.dumpDebugInfo = data.dumpDebugInfo
    member x.debugSymbolFile  = data.debugSymbolFile
    member x.typeCheckOnly  = data.typeCheckOnly
    member x.parseOnly  = data.parseOnly
    member x.importAllReferencesOnly = data.importAllReferencesOnly
    member x.simulateException = data.simulateException
    member x.printAst  = data.printAst
    member x.targetFrameworkVersion = targetFrameworkVersionValue
    member x.tokenizeOnly  = data.tokenizeOnly
    member x.testInteractionParser  = data.testInteractionParser
    member x.reportNumDecls  = data.reportNumDecls
    member x.printSignature  = data.printSignature
    member x.printSignatureFile  = data.printSignatureFile
    member x.xmlDocOutputFile  = data.xmlDocOutputFile
    member x.stats  = data.stats
    member x.generateFilterBlocks  = data.generateFilterBlocks
    member x.signer  = data.signer
    member x.container = data.container
    member x.delaysign  = data.delaysign
    member x.publicsign  = data.publicsign
    member x.version  = data.version
    member x.metadataVersion = data.metadataVersion
    member x.standalone  = data.standalone
    member x.extraStaticLinkRoots  = data.extraStaticLinkRoots
    member x.noSignatureData  = data.noSignatureData
    member x.onlyEssentialOptimizationData  = data.onlyEssentialOptimizationData
    member x.useOptimizationDataFile  = data.useOptimizationDataFile
    member x.jitTracking  = data.jitTracking
    member x.portablePDB  = data.portablePDB
    member x.embeddedPDB  = data.embeddedPDB
    member x.embedAllSource  = data.embedAllSource
    member x.embedSourceList  = data.embedSourceList
    member x.sourceLink  = data.sourceLink
    member x.ignoreSymbolStoreSequencePoints  = data.ignoreSymbolStoreSequencePoints
    member x.internConstantStrings  = data.internConstantStrings
    member x.extraOptimizationIterations  = data.extraOptimizationIterations
    member x.win32res  = data.win32res
    member x.win32manifest = data.win32manifest
    member x.includewin32manifest = data.includewin32manifest
    member x.linkResources  = data.linkResources
    member x.showFullPaths  = data.showFullPaths
    member x.errorStyle  = data.errorStyle
    member x.utf8output  = data.utf8output
    member x.flatErrors = data.flatErrors
    member x.maxErrors  = data.maxErrors
    member x.baseAddress  = data.baseAddress
 #if DEBUG
    member x.showOptimizationData  = data.showOptimizationData
#endif
    member x.showTerms          = data.showTerms
    member x.writeTermsToFiles  = data.writeTermsToFiles
    member x.doDetuple          = data.doDetuple
    member x.doTLR              = data.doTLR
    member x.doFinalSimplify    = data.doFinalSimplify
    member x.optSettings        = data.optSettings
    member x.emitTailcalls      = data.emitTailcalls
    member x.deterministic      = data.deterministic
    member x.preferredUiLang    = data.preferredUiLang
    member x.lcid               = data.lcid
    member x.optsOn             = data.optsOn
    member x.productNameForBannerText  = data.productNameForBannerText
    member x.showBanner   = data.showBanner
    member x.showTimes  = data.showTimes
    member x.showLoadedAssemblies = data.showLoadedAssemblies
    member x.continueAfterParseFailure = data.continueAfterParseFailure
#if !NO_EXTENSIONTYPING
    member x.showExtensionTypeMessages  = data.showExtensionTypeMessages    
#endif
    member x.pause  = data.pause
    member x.alwaysCallVirt = data.alwaysCallVirt
    member x.noDebugData = data.noDebugData
    member x.isInteractive = data.isInteractive
    member x.isInvalidationSupported = data.isInvalidationSupported
    member x.emitDebugInfoInQuotations = data.emitDebugInfoInQuotations
    member x.copyFSharpCore = data.copyFSharpCore
    member x.shadowCopyReferences = data.shadowCopyReferences
    member x.tryGetMetadataSnapshot = data.tryGetMetadataSnapshot
    static member Create(builder, validate) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        TcConfig(builder, validate)

    member x.legacyReferenceResolver = data.legacyReferenceResolver
    member tcConfig.CloneOfOriginalBuilder = 
        { data with conditionalCompilationDefines=data.conditionalCompilationDefines }

    member tcConfig.ComputeCanContainEntryPoint(sourceFiles:string list) = 
        let n = sourceFiles.Length in 
        (sourceFiles |> List.mapi (fun i _ -> (i = n-1)), tcConfig.target.IsExe)
            
    // This call can fail if no CLR is found (this is the path to mscorlib)
    member tcConfig.GetTargetFrameworkDirectories() = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        try 
          [ 
            // Check if we are given an explicit framework root - if so, use that
            match tcConfig.clrRoot with 
            | Some x -> 
                yield tcConfig.MakePathAbsolute x

            | None -> 
// "there is no really good notion of runtime directory on .NETCore"
#if NETSTANDARD1_6 || NETSTANDARD2_0
                let runtimeRoot = Path.GetDirectoryName(typeof<System.Object>.Assembly.Location)
#else
                let runtimeRoot = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
#endif
                let runtimeRootWithoutSlash = runtimeRoot.TrimEnd('/', '\\')
                let runtimeRootFacades = Path.Combine(runtimeRootWithoutSlash, "Facades")
                let runtimeRootWPF = Path.Combine(runtimeRootWithoutSlash, "WPF")

                match tcConfig.resolutionEnvironment with
                | ResolutionEnvironment.CompilationAndEvaluation ->
                    // Default compilation-and-execution-time references on .NET Framework and Mono, e.g. for F# Interactive
                    //
                    // In the current way of doing things, F# Interactive refers to implementation assemblies.
                    yield runtimeRoot
                    if Directory.Exists(runtimeRootFacades) then
                        yield runtimeRootFacades // System.Runtime.dll is in /usr/lib/mono/4.5/Facades
                    if Directory.Exists(runtimeRootWPF) then
                        yield runtimeRootWPF // PresentationCore.dll is in C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF

                | ResolutionEnvironment.EditingOrCompilation _ ->
#if ENABLE_MONO_SUPPORT
                    if runningOnMono then 
                        // Default compilation-time references on Mono
                        //
                        // On Mono, the default references come from the implementation assemblies.
                        // This is because we have had trouble reliably using MSBuild APIs to compute DotNetFrameworkReferenceAssembliesRootDirectory on Mono.
                        yield runtimeRoot
                        if Directory.Exists(runtimeRootFacades) then
                            yield runtimeRootFacades // System.Runtime.dll is in /usr/lib/mono/4.5/Facades
                        if Directory.Exists(runtimeRootWPF) then
                            yield runtimeRootWPF // PresentationCore.dll is in C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF
                        // On Mono we also add a default reference to the 4.5-api and 4.5-api/Facades directories.  
                        let runtimeRootApi = runtimeRootWithoutSlash + "-api"
                        let runtimeRootApiFacades = Path.Combine(runtimeRootApi, "Facades")
                        if Directory.Exists(runtimeRootApi) then
                            yield runtimeRootApi
                        if Directory.Exists(runtimeRootApiFacades) then
                             yield runtimeRootApiFacades
                    else                                
#endif
                        // Default compilation-time references on .NET Framework
                        //
                        // This is the normal case for "fsc.exe a.fs". We refer to the reference assemblies folder.
                        let frameworkRoot = tcConfig.legacyReferenceResolver.DotNetFrameworkReferenceAssembliesRootDirectory
                        let frameworkRootVersion = Path.Combine(frameworkRoot, tcConfig.targetFrameworkVersion)
                        yield frameworkRootVersion
                        let facades = Path.Combine(frameworkRootVersion, "Facades")
                        if Directory.Exists(facades) then
                            yield facades
                  ]                    
        with e -> 
            errorRecovery e range0; [] 

    member tcConfig.ComputeLightSyntaxInitialStatus(filename) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        let lower = String.lowercase filename
        let lightOnByDefault = List.exists (Filename.checkSuffix lower) FSharpLightSyntaxFileSuffixes
        if lightOnByDefault then (tcConfig.light <> Some(false)) else (tcConfig.light = Some(true) )

    member tcConfig.GetAvailableLoadedSources() =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        let resolveLoadedSource (m, path) =
            try
                if not(FileSystem.SafeExists(path)) then 
                    error(LoadedSourceNotFoundIgnoring(path, m))                         
                    None
                else Some(m, path)
            with e -> errorRecovery e m; None
        tcConfig.loadedSources 
        |> List.choose resolveLoadedSource 
        |> List.distinct     

    /// A closed set of assemblies where, for any subset S:
    ///    -  the TcImports object built for S (and thus the F# Compiler CCUs for the assemblies in S) 
    ///       is a resource that can be shared between any two IncrementalBuild objects that reference
    ///       precisely S
    ///
    /// Determined by looking at the set of assemblies referenced by f# .
    ///
    /// Returning true may mean that the file is locked and/or placed into the
    /// 'framework' reference set that is potentially shared across multiple compilations.
    member tcConfig.IsSystemAssembly (filename:string) =  
        try 
            FileSystem.SafeExists filename && 
            ((tcConfig.GetTargetFrameworkDirectories() |> List.exists (fun clrRoot -> clrRoot = Path.GetDirectoryName filename)) ||
             (systemAssemblies.Contains(fileNameWithoutExtension filename)))
        with _ ->
            false    

    // This is not the complete set of search paths, it is just the set 
    // that is special to F# (as compared to MSBuild resolution)
    member tcConfig.GetSearchPathsForLibraryFiles() = 
        [ yield! tcConfig.GetTargetFrameworkDirectories()
          yield! List.map (tcConfig.MakePathAbsolute) tcConfig.includes
          yield tcConfig.implicitIncludeDir 
          yield tcConfig.fsharpBinariesDir ]

    member tcConfig.MakePathAbsolute path = 
        let result = ComputeMakePathAbsolute tcConfig.implicitIncludeDir path
        result

    member tcConfig.TryResolveLibWithDirectories (r:AssemblyReference) = 
        let m, nm = r.Range, r.Text
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        // Only want to resolve certain extensions (otherwise, 'System.Xml' is ambiguous).
        // MSBuild resolution is limited to .exe and .dll so do the same here.
        let ext = System.IO.Path.GetExtension(nm)
        let isNetModule = String.Compare(ext, ".netmodule", StringComparison.OrdinalIgnoreCase)=0 
        
        // See if the language service has already produced the contents of the assembly for us, virtually
        match r.ProjectReference with 
        | Some _ -> 
            let resolved = r.Text
            let sysdir = tcConfig.IsSystemAssembly resolved
            Some
                { originalReference = r
                  resolvedPath = resolved
                  prepareToolTip = (fun () -> resolved)
                  sysdir = sysdir
                  ilAssemblyRef = ref None }
        | None -> 

        if String.Compare(ext, ".dll", StringComparison.OrdinalIgnoreCase)=0 
           || String.Compare(ext, ".exe", StringComparison.OrdinalIgnoreCase)=0 
           || isNetModule then

            let searchPaths =
                // if this is a #r reference (not from dummy range), make sure the directory of the declaring
                // file is included in the search path. This should ideally already be one of the search paths, but
                // during some global checks it won't be.  We append to the end of the search list so that this is the last
                // place that is checked.
                if m <> range0 && m <> rangeStartup && m <> rangeCmdArgs && FileSystem.IsPathRootedShim m.FileName then
                    tcConfig.GetSearchPathsForLibraryFiles() @ [Path.GetDirectoryName(m.FileName)]
                else    
                    tcConfig.GetSearchPathsForLibraryFiles()

            let resolved = TryResolveFileUsingPaths(searchPaths, m, nm)
            match resolved with 
            | Some(resolved) -> 
                let sysdir = tcConfig.IsSystemAssembly resolved
                Some
                    { originalReference = r
                      resolvedPath = resolved
                      prepareToolTip = (fun () -> 
                            let fusionName = System.Reflection.AssemblyName.GetAssemblyName(resolved).ToString()
                            let line(append:string) = append.Trim([|' '|])+"\n"
                            line(resolved) + line(fusionName))
                      sysdir = sysdir
                      ilAssemblyRef = ref None }
            | None -> None
        else None

    member tcConfig.ResolveLibWithDirectories (ccuLoadFaulureAction, r:AssemblyReference) =
        let m, nm = r.Range, r.Text
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        // test for both libraries and executables
        let ext = System.IO.Path.GetExtension(nm)
        let isExe = (String.Compare(ext, ".exe", StringComparison.OrdinalIgnoreCase) = 0)
        let isDLL = (String.Compare(ext, ".dll", StringComparison.OrdinalIgnoreCase) = 0)
        let isNetModule = (String.Compare(ext, ".netmodule", StringComparison.OrdinalIgnoreCase) = 0)

        let rs = 
            if isExe || isDLL || isNetModule then
                [r]
            else
                [AssemblyReference(m, nm+".dll", None);AssemblyReference(m, nm+".exe", None);AssemblyReference(m, nm+".netmodule", None)]

        match rs |> List.tryPick (fun r -> tcConfig.TryResolveLibWithDirectories r) with
        | Some(res) -> Some res
        | None ->
            match ccuLoadFaulureAction with
            | CcuLoadFailureAction.RaiseError ->
                let searchMessage = String.concat "\n " (tcConfig.GetSearchPathsForLibraryFiles())
                raise (FileNameNotResolved(nm, searchMessage, m))
            | CcuLoadFailureAction.ReturnNone -> None

    member tcConfig.ResolveSourceFile(m, nm, pathLoadedFrom) = 
        data.ResolveSourceFile(m, nm, pathLoadedFrom)

    // NOTE!! if mode=Speculative then this method must not report ANY warnings or errors through 'warning' or 'error'. Instead
    // it must return warnings and errors as data
    //
    // NOTE!! if mode=ReportErrors then this method must not raise exceptions. It must just report the errors and recover
    static member TryResolveLibsUsingMSBuildRules (tcConfig:TcConfig, originalReferences:AssemblyReference list, errorAndWarningRange:range, mode:ResolveAssemblyReferenceMode) : AssemblyResolution list * UnresolvedAssemblyReference list =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
        if tcConfig.useSimpleResolution then
            failwith "MSBuild resolution is not supported."
        if originalReferences=[] then [], []
        else            
            // Group references by name with range values in the grouped value list.
            // In the grouped reference, store the index of the last use of the reference.
            let groupedReferences = 
                originalReferences
                |> List.indexed
                |> Seq.groupBy(fun (_, reference) -> reference.Text)
                |> Seq.map(fun (assemblyName, assemblyAndIndexGroup)->
                    let assemblyAndIndexGroup = assemblyAndIndexGroup |> List.ofSeq
                    let highestPosition = assemblyAndIndexGroup |> List.maxBy fst |> fst
                    let assemblyGroup = assemblyAndIndexGroup |> List.map snd
                    assemblyName, highestPosition, assemblyGroup)
                |> Array.ofSeq

            let logMessage showMessages  = 
                if showMessages && tcConfig.showReferenceResolutions then (fun (message:string)->dprintf "%s\n" message)
                else ignore

            let logDiagnostic showMessages = 
                (fun isError code message->
                    if showMessages && mode = ResolveAssemblyReferenceMode.ReportErrors then 
                      if isError then
                        errorR(MSBuildReferenceResolutionError(code, message, errorAndWarningRange))
                      else
                        match code with 
                        // These are warnings that mean 'not resolved' for some assembly.
                        // Note that we don't get to know the name of the assembly that couldn't be resolved.
                        // Ignore these and rely on the logic below to emit an error for each unresolved reference.
                        | "MSB3246" // Resolved file has a bad image, no metadata, or is otherwise inaccessible.
                        | "MSB3106"  
                            -> ()
                        | _ -> 
                            if code = "MSB3245" then 
                                errorR(MSBuildReferenceResolutionWarning(code, message, errorAndWarningRange))
                            else
                                warning(MSBuildReferenceResolutionWarning(code, message, errorAndWarningRange)))

            let targetProcessorArchitecture = 
                    match tcConfig.platform with
                    | None -> "MSIL"
                    | Some(X86) -> "x86"
                    | Some(AMD64) -> "amd64"
                    | Some(IA64) -> "ia64"

            // First, try to resolve everything as a file using simple resolution
            let resolvedAsFile = 
                groupedReferences 
                |> Array.map(fun (_filename, maxIndexOfReference, references)->
                                let assemblyResolution = references |> List.choose (fun r -> tcConfig.TryResolveLibWithDirectories r)
                                (maxIndexOfReference, assemblyResolution))  
                |> Array.filter(fun (_, refs)->refs |> isNil |> not)
                
                                       
            // Whatever is left, pass to MSBuild.
            let Resolve(references, showMessages) =
                try 
                    tcConfig.legacyReferenceResolver.Resolve
                       (tcConfig.resolutionEnvironment, 
                        references, 
                        tcConfig.targetFrameworkVersion, 
                        tcConfig.GetTargetFrameworkDirectories(), 
                        targetProcessorArchitecture, 
                        tcConfig.fsharpBinariesDir, // FSharp binaries directory
                        tcConfig.includes, // Explicit include directories
                        tcConfig.implicitIncludeDir, // Implicit include directory (likely the project directory)
                        logMessage showMessages, logDiagnostic showMessages)
                with 
                    ReferenceResolver.ResolutionFailure -> error(Error(FSComp.SR.buildAssemblyResolutionFailed(), errorAndWarningRange))
            
            let toMsBuild = [|0..groupedReferences.Length-1|] 
                             |> Array.map(fun i->(p13 groupedReferences.[i]), (p23 groupedReferences.[i]), i) 
                             |> Array.filter (fun (_, i0, _)->resolvedAsFile|>Array.exists(fun (i1, _) -> i0=i1)|>not)
                             |> Array.map(fun (ref, _, i)->ref, string i)

            let resolutions = Resolve(toMsBuild, (*showMessages*)true)  

            // Map back to original assembly resolutions.
            let resolvedByMsbuild = 
                resolutions
                    |> Array.map(fun resolvedFile -> 
                                    let i = int resolvedFile.baggage
                                    let _, maxIndexOfReference, ms = groupedReferences.[i]
                                    let assemblyResolutions =
                                        ms|>List.map(fun originalReference ->
                                                    System.Diagnostics.Debug.Assert(FileSystem.IsPathRootedShim(resolvedFile.itemSpec), sprintf "msbuild-resolved path is not absolute: '%s'" resolvedFile.itemSpec)
                                                    let canonicalItemSpec = FileSystem.GetFullPathShim(resolvedFile.itemSpec)
                                                    { originalReference=originalReference 
                                                      resolvedPath=canonicalItemSpec 
                                                      prepareToolTip = (fun () -> resolvedFile.prepareToolTip (originalReference.Text, canonicalItemSpec))
                                                      sysdir= tcConfig.IsSystemAssembly canonicalItemSpec
                                                      ilAssemblyRef = ref None })
                                    (maxIndexOfReference, assemblyResolutions))

            // When calculating the resulting resolutions, we're going to use the index of the reference
            // in the original specification and resort it to match the ordering that we had.
            let resultingResolutions =
                    [resolvedByMsbuild;resolvedAsFile]
                    |> Array.concat                                  
                    |> Array.sortBy fst
                    |> Array.map snd
                    |> List.ofArray
                    |> List.concat                                                 
                    
            // O(N^2) here over a small set of referenced assemblies.
            let IsResolved(originalName:string) =
                if resultingResolutions |> List.exists(fun resolution -> resolution.originalReference.Text = originalName) then true
                else 
                    // MSBuild resolution may have unified the result of two duplicate references. Try to re-resolve now.
                    // If re-resolution worked then this was a removed duplicate.
                    Resolve([|originalName, ""|], (*showMessages*)false).Length<>0 
                    
            let unresolvedReferences =                     
                    groupedReferences 
                    //|> Array.filter(p13 >> IsNotFileOrIsAssembly)
                    |> Array.filter(p13 >> IsResolved >> not)   
                    |> List.ofArray                 

            // If mode=Speculative, then we haven't reported any errors.
            // We report the error condition by returning an empty list of resolutions
            if mode = ResolveAssemblyReferenceMode.Speculative && (List.length unresolvedReferences) > 0 then 
                [], (List.ofArray groupedReferences) |> List.map (fun (name, _, r) -> (name, r)) |> List.map UnresolvedAssemblyReference
            else 
                resultingResolutions, unresolvedReferences |> List.map (fun (name, _, r) -> (name, r)) |> List.map UnresolvedAssemblyReference    


    member tcConfig.PrimaryAssemblyDllReference() = primaryAssemblyReference
    member tcConfig.CoreLibraryDllReference() = fslibReference
               

let ReportWarning options err = 
    warningOn err (options.WarnLevel) (options.WarnOn) && not (List.contains (GetDiagnosticNumber err) (options.WarnOff))

let ReportWarningAsError options err =
    warningOn err (options.WarnLevel) (options.WarnOn) &&
    not (List.contains (GetDiagnosticNumber err) (options.WarnAsWarn)) &&
    ((options.GlobalWarnAsError && not (List.contains (GetDiagnosticNumber err) options.WarnOff)) ||
     List.contains (GetDiagnosticNumber err) (options.WarnAsError))

//----------------------------------------------------------------------------
// Scoped #nowarn pragmas


let GetScopedPragmasForHashDirective hd = 
    [ match hd with 
      | ParsedHashDirective("nowarn", numbers, m) ->
          for s in numbers do
          match GetWarningNumber(m, s) with 
            | None -> ()
            | Some n -> yield ScopedPragma.WarningOff(m, n) 
      | _ -> () ]


let GetScopedPragmasForInput input = 

    match input with 
    | ParsedInput.SigFile (ParsedSigFileInput(scopedPragmas=pragmas)) -> pragmas
    | ParsedInput.ImplFile (ParsedImplFileInput(scopedPragmas=pragmas)) -> pragmas



/// Build an ErrorLogger that delegates to another ErrorLogger but filters warnings turned off by the given pragma declarations
//
// NOTE: we allow a flag to turn of strict file checking. This is because file names sometimes don't match due to use of 
// #line directives, e.g. for pars.fs/pars.fsy. In this case we just test by line number - in most cases this is sufficient
// because we install a filtering error handler on a file-by-file basis for parsing and type-checking.
// However this is indicative of a more systematic problem where source-line 
// sensitive operations (lexfilter and warning filtering) do not always
// interact well with #line directives.
type ErrorLoggerFilteringByScopedPragmas (checkFile, scopedPragmas, errorLogger:ErrorLogger) =
    inherit ErrorLogger("ErrorLoggerFilteringByScopedPragmas")

    override x.DiagnosticSink (phasedError, isError) = 
        if isError then 
            errorLogger.DiagnosticSink (phasedError, isError)
        else 
          let report = 
            let warningNum = GetDiagnosticNumber phasedError
            match GetRangeOfDiagnostic phasedError with 
            | Some m -> 
                not (scopedPragmas |> List.exists (fun pragma ->
                    match pragma with 
                    | ScopedPragma.WarningOff(pragmaRange, warningNumFromPragma) -> 
                        warningNum = warningNumFromPragma && 
                        (not checkFile || m.FileIndex = pragmaRange.FileIndex) &&
                        Range.posGeq m.Start pragmaRange.Start))  
            | None -> true
          if report then errorLogger.DiagnosticSink(phasedError, false)

    override x.ErrorCount = errorLogger.ErrorCount

let GetErrorLoggerFilteringByScopedPragmas(checkFile, scopedPragmas, errorLogger) = 
    (ErrorLoggerFilteringByScopedPragmas(checkFile, scopedPragmas, errorLogger) :> ErrorLogger)


//----------------------------------------------------------------------------
// Parsing
//--------------------------------------------------------------------------


let CanonicalizeFilename filename = 
    let basic = fileNameOfPath filename
    String.capitalize (try Filename.chopExtension basic with _ -> basic)

let IsScript filename = 
    let lower = String.lowercase filename 
    FSharpScriptFileSuffixes |> List.exists (Filename.checkSuffix lower)
    
// Give a unique name to the different kinds of inputs. Used to correlate signature and implementation files
//   QualFileNameOfModuleName - files with a single module declaration or an anonymous module
let QualFileNameOfModuleName m filename modname = QualifiedNameOfFile(mkSynId m (textOfLid modname + (if IsScript filename then "$fsx" else "")))
let QualFileNameOfFilename m filename = QualifiedNameOfFile(mkSynId m (CanonicalizeFilename filename + (if IsScript filename then "$fsx" else "")))

// Interactive fragments
let ComputeQualifiedNameOfFileFromUniquePath (m, p: string list) = QualifiedNameOfFile(mkSynId m (String.concat "_" p))

let QualFileNameOfSpecs filename specs = 
    match specs with 
    | [SynModuleOrNamespaceSig(modname, _, true, _, _, _, _, m)] -> QualFileNameOfModuleName m filename modname
    | [SynModuleOrNamespaceSig(_, _, false, _, _, _, _, m)] -> QualFileNameOfFilename m filename
    | _ -> QualFileNameOfFilename (mkRange filename pos0 pos0) filename

let QualFileNameOfImpls filename specs = 
    match specs with 
    | [SynModuleOrNamespace(modname, _, true, _, _, _, _, m)] -> QualFileNameOfModuleName m filename modname
    | [SynModuleOrNamespace(_, _, false, _, _, _, _, m)] -> QualFileNameOfFilename m filename
    | _ -> QualFileNameOfFilename (mkRange filename pos0 pos0) filename

let PrepandPathToQualFileName x (QualifiedNameOfFile(q)) = ComputeQualifiedNameOfFileFromUniquePath (q.idRange, pathOfLid x@[q.idText])
let PrepandPathToImpl x (SynModuleOrNamespace(p, b, c, d, e, f, g, h)) = SynModuleOrNamespace(x@p, b, c, d, e, f, g, h)
let PrepandPathToSpec x (SynModuleOrNamespaceSig(p, b, c, d, e, f, g, h)) = SynModuleOrNamespaceSig(x@p, b, c, d, e, f, g, h)

let PrependPathToInput x inp = 
    match inp with 
    | ParsedInput.ImplFile (ParsedImplFileInput(b, c, q, d, hd, impls, e)) -> ParsedInput.ImplFile (ParsedImplFileInput(b, c, PrepandPathToQualFileName x q, d, hd, List.map (PrepandPathToImpl x) impls, e))
    | ParsedInput.SigFile (ParsedSigFileInput(b, q, d, hd, specs)) -> ParsedInput.SigFile(ParsedSigFileInput(b, PrepandPathToQualFileName x q, d, hd, List.map (PrepandPathToSpec x) specs))

let ComputeAnonModuleName check defaultNamespace filename (m: range) = 
    let modname = CanonicalizeFilename filename
    if check && not (modname |> String.forall (fun c -> System.Char.IsLetterOrDigit(c) || c = '_')) then
          if not (filename.EndsWith("fsx", StringComparison.OrdinalIgnoreCase) || filename.EndsWith("fsscript", StringComparison.OrdinalIgnoreCase)) then
              warning(Error(FSComp.SR.buildImplicitModuleIsNotLegalIdentifier(modname, (fileNameOfPath filename)), m))
    let combined = 
      match defaultNamespace with 
      | None -> modname
      | Some ns -> textOfPath [ns;modname]

    let anonymousModuleNameRange  =
        let filename = m.FileName
        mkRange filename pos0 pos0
    pathToSynLid anonymousModuleNameRange (splitNamespace combined)

let PostParseModuleImpl (_i, defaultNamespace, isLastCompiland, filename, impl) = 
    match impl with 
    | ParsedImplFileFragment.NamedModule(SynModuleOrNamespace(lid, isRec, isModule, decls, xmlDoc, attribs, access, m)) -> 
        let lid = 
            match lid with 
            | [id] when isModule && id.idText = MangledGlobalName -> error(Error(FSComp.SR.buildInvalidModuleOrNamespaceName(), id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespace(lid, isRec, isModule, decls, xmlDoc, attribs, access, m)

    | ParsedImplFileFragment.AnonModule (defs, m)-> 
        let isLast, isExe = isLastCompiland 
        let lower = String.lowercase filename
        if not (isLast && isExe) && not (doNotRequireNamespaceOrModuleSuffixes |> List.exists (Filename.checkSuffix lower)) then
            match defs with
            | SynModuleDecl.NestedModule(_) :: _ -> errorR(Error(FSComp.SR.noEqualSignAfterModule(), trimRangeToLine m))
            | _ -> errorR(Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule(), trimRangeToLine m))

        let modname = ComputeAnonModuleName (not (isNil defs)) defaultNamespace filename (trimRangeToLine m)
        SynModuleOrNamespace(modname, false, true, defs, PreXmlDoc.Empty, [], None, m)

    | ParsedImplFileFragment.NamespaceFragment (lid, a, b, c, d, e, m)-> 
        let lid = 
            match lid with 
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespace(lid, a, b, c, d, e, None, m)

let PostParseModuleSpec (_i, defaultNamespace, isLastCompiland, filename, intf) = 
    match intf with 
    | ParsedSigFileFragment.NamedModule(SynModuleOrNamespaceSig(lid, isRec, isModule, decls, xmlDoc, attribs, access, m)) -> 
        let lid = 
            match lid with 
            | [id] when isModule && id.idText = MangledGlobalName -> error(Error(FSComp.SR.buildInvalidModuleOrNamespaceName(), id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespaceSig(lid, isRec, isModule, decls, xmlDoc, attribs, access, m)

    | ParsedSigFileFragment.AnonModule (defs, m) -> 
        let isLast, isExe = isLastCompiland
        let lower = String.lowercase filename
        if not (isLast && isExe) && not (doNotRequireNamespaceOrModuleSuffixes |> List.exists (Filename.checkSuffix lower)) then 
            match defs with
            | SynModuleSigDecl.NestedModule(_) :: _ -> errorR(Error(FSComp.SR.noEqualSignAfterModule(), m))
            | _ -> errorR(Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule(), m))

        let modname = ComputeAnonModuleName (not (isNil defs)) defaultNamespace filename (trimRangeToLine m)
        SynModuleOrNamespaceSig(modname, false, true, defs, PreXmlDoc.Empty, [], None, m)

    | ParsedSigFileFragment.NamespaceFragment (lid, a, b, c, d, e, m)-> 
        let lid = 
            match lid with 
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespaceSig(lid, a, b, c, d, e, None, m)



let PostParseModuleImpls (defaultNamespace, filename, isLastCompiland, ParsedImplFile(hashDirectives, impls)) = 
    match impls |> List.rev |> List.tryPick (function ParsedImplFileFragment.NamedModule(SynModuleOrNamespace(lid, _, _, _, _, _, _, _)) -> Some(lid) | _ -> None) with
    | Some lid when impls.Length > 1 -> 
        errorR(Error(FSComp.SR.buildMultipleToplevelModules(), rangeOfLid lid))
    | _ -> 
        ()
    let impls = impls |> List.mapi (fun i x -> PostParseModuleImpl (i, defaultNamespace, isLastCompiland, filename, x)) 
    let qualName = QualFileNameOfImpls filename impls
    let isScript = IsScript filename

    let scopedPragmas = 
        [ for (SynModuleOrNamespace(_, _, _, decls, _, _, _, _)) in impls do 
            for d in decls do
                match d with 
                | SynModuleDecl.HashDirective (hd, _) -> yield! GetScopedPragmasForHashDirective hd
                | _ -> () 
          for hd in hashDirectives do 
              yield! GetScopedPragmasForHashDirective hd ]
    ParsedInput.ImplFile(ParsedImplFileInput(filename, isScript, qualName, scopedPragmas, hashDirectives, impls, isLastCompiland))
  
let PostParseModuleSpecs (defaultNamespace, filename, isLastCompiland, ParsedSigFile(hashDirectives, specs)) = 
    match specs |> List.rev |> List.tryPick (function ParsedSigFileFragment.NamedModule(SynModuleOrNamespaceSig(lid, _, _, _, _, _, _, _)) -> Some(lid) | _ -> None) with
    | Some  lid when specs.Length > 1 -> 
        errorR(Error(FSComp.SR.buildMultipleToplevelModules(), rangeOfLid lid))
    | _ -> 
        ()
        
    let specs = specs |> List.mapi (fun i x -> PostParseModuleSpec(i, defaultNamespace, isLastCompiland, filename, x)) 
    let qualName = QualFileNameOfSpecs filename specs
    let scopedPragmas = 
        [ for (SynModuleOrNamespaceSig(_, _, _, decls, _, _, _, _)) in specs do 
            for d in decls do
                match d with 
                | SynModuleSigDecl.HashDirective(hd, _) -> yield! GetScopedPragmasForHashDirective hd
                | _ -> () 
          for hd in hashDirectives do 
              yield! GetScopedPragmasForHashDirective hd ]

    ParsedInput.SigFile(ParsedSigFileInput(filename, qualName, scopedPragmas, hashDirectives, specs))

/// Checks if a module name is already given and deduplicates the name if needed.
let DeduplicateModuleName (moduleNamesDict:IDictionary<string, Set<string>>) (paths: Set<string>) path (qualifiedNameOfFile: QualifiedNameOfFile) =
    let count = if paths.Contains path then paths.Count else paths.Count + 1
    moduleNamesDict.[qualifiedNameOfFile.Text] <- Set.add path paths
    let id = qualifiedNameOfFile.Id
    if count = 1 then qualifiedNameOfFile else QualifiedNameOfFile(Ident(id.idText + "___" + count.ToString(), id.idRange))

/// Checks if a ParsedInput is using a module name that was already given and deduplicates the name if needed.
let DeduplicateParsedInputModuleName (moduleNamesDict:IDictionary<string, Set<string>>) input =
    match input with
    | ParsedInput.ImplFile (ParsedImplFileInput.ParsedImplFileInput(fileName, isScript, qualifiedNameOfFile, scopedPragmas, hashDirectives, modules, (isLastCompiland, isExe))) ->
        let path = Path.GetDirectoryName fileName
        match moduleNamesDict.TryGetValue qualifiedNameOfFile.Text with
        | true, paths ->
            let qualifiedNameOfFile = DeduplicateModuleName moduleNamesDict paths path qualifiedNameOfFile
            ParsedInput.ImplFile(ParsedImplFileInput.ParsedImplFileInput(fileName, isScript, qualifiedNameOfFile, scopedPragmas, hashDirectives, modules, (isLastCompiland, isExe)))
        | _ ->
            moduleNamesDict.[qualifiedNameOfFile.Text] <- Set.singleton path
            input
    | ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput(fileName, qualifiedNameOfFile, scopedPragmas, hashDirectives, modules)) ->
        let path = Path.GetDirectoryName fileName
        match moduleNamesDict.TryGetValue qualifiedNameOfFile.Text with
        | true, paths ->
            let qualifiedNameOfFile = DeduplicateModuleName moduleNamesDict paths path qualifiedNameOfFile
            ParsedInput.SigFile (ParsedSigFileInput.ParsedSigFileInput(fileName, qualifiedNameOfFile, scopedPragmas, hashDirectives, modules))
        | _ ->
            moduleNamesDict.[qualifiedNameOfFile.Text] <- Set.singleton path
            input

let ParseInput (lexer, errorLogger:ErrorLogger, lexbuf:UnicodeLexing.Lexbuf, defaultNamespace, filename, isLastCompiland) = 
    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy filename
    //  - if you have a #line directive, e.g. 
    //        # 1000 "Line01.fs"
    //    then it also asserts.  But these are edge cases that can be fixed later, e.g. in bug 4651.
    //System.Diagnostics.Debug.Assert(System.IO.Path.IsPathRooted(filename), sprintf "should be absolute: '%s'" filename)
    let lower = String.lowercase filename 
    // Delay sending errors and warnings until after the file is parsed. This gives us a chance to scrape the
    // #nowarn declarations for the file
    let delayLogger = CapturingErrorLogger("Parsing")
    use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayLogger)
    use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
    let mutable scopedPragmas  = []
    try     
        let input = 
            if mlCompatSuffixes |> List.exists (Filename.checkSuffix lower)   then  
                mlCompatWarning (FSComp.SR.buildCompilingExtensionIsForML()) rangeStartup 

            if FSharpImplFileSuffixes |> List.exists (Filename.checkSuffix lower)   then  
                let impl = Parser.implementationFile lexer lexbuf 
                PostParseModuleImpls (defaultNamespace, filename, isLastCompiland, impl)
            elif FSharpSigFileSuffixes |> List.exists (Filename.checkSuffix lower)  then  
                let intfs = Parser.signatureFile lexer lexbuf 
                PostParseModuleSpecs (defaultNamespace, filename, isLastCompiland, intfs)
            else 
                delayLogger.Error(Error(FSComp.SR.buildInvalidSourceFileExtension(filename), Range.rangeStartup))
        scopedPragmas <- GetScopedPragmasForInput input
        input
    finally
        // OK, now commit the errors, since the ScopedPragmas will (hopefully) have been scraped
        let filteringErrorLogger = ErrorLoggerFilteringByScopedPragmas(false, scopedPragmas, errorLogger)
        delayLogger.CommitDelayedDiagnostics(filteringErrorLogger)

//----------------------------------------------------------------------------
// parsing - ParseOneInputFile
// Filename is (ml/mli/fs/fsi source). Parse it to AST. 
//----------------------------------------------------------------------------
let ParseOneInputLexbuf (tcConfig:TcConfig, lexResourceManager, conditionalCompilationDefines, lexbuf, filename, isLastCompiland, errorLogger) =
    use unwindbuildphase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
    try 
        let skip = true in (* don't report whitespace from lexer *)
        let lightSyntaxStatus = LightSyntaxStatus (tcConfig.ComputeLightSyntaxInitialStatus(filename), true) 
        let lexargs = mkLexargs (filename, conditionalCompilationDefines@tcConfig.conditionalCompilationDefines, lightSyntaxStatus, lexResourceManager, ref [], errorLogger)
        let shortFilename = SanitizeFileName filename tcConfig.implicitIncludeDir 
        let input = 
            Lexhelp.usingLexbufForParsing (lexbuf, filename) (fun lexbuf ->
                if verbose then dprintn ("Parsing... "+shortFilename)
                let tokenizer = LexFilter.LexFilter(lightSyntaxStatus, tcConfig.compilingFslib, Lexer.token lexargs skip, lexbuf)

                if tcConfig.tokenizeOnly then 
                    while true do 
                        printf "tokenize - getting one token from %s\n" shortFilename
                        let t = tokenizer.Lexer lexbuf
                        printf "tokenize - got %s @ %a\n" (Parser.token_to_string t) outputRange lexbuf.LexemeRange
                        (match t with Parser.EOF _ -> exit 0 | _ -> ())
                        if lexbuf.IsPastEndOfStream then  printf "!!! at end of stream\n"

                if tcConfig.testInteractionParser then 
                    while true do 
                        match (Parser.interaction tokenizer.Lexer lexbuf) with
                        | IDefns(l, m) -> dprintf "Parsed OK, got %d defs @ %a\n" l.Length outputRange m
                        | IHash (_, m) -> dprintf "Parsed OK, got hash @ %a\n" outputRange m
                    exit 0

                let res = ParseInput(tokenizer.Lexer, errorLogger, lexbuf, None, filename, isLastCompiland)

                if tcConfig.reportNumDecls then 
                    let rec flattenSpecs specs = 
                          specs |> List.collect (function (SynModuleSigDecl.NestedModule (_, _, subDecls, _)) -> flattenSpecs subDecls | spec -> [spec])
                    let rec flattenDefns specs = 
                          specs |> List.collect (function (SynModuleDecl.NestedModule (_, _, subDecls, _, _)) -> flattenDefns subDecls | defn -> [defn])

                    let flattenModSpec (SynModuleOrNamespaceSig(_, _, _, decls, _, _, _, _)) = flattenSpecs decls
                    let flattenModImpl (SynModuleOrNamespace(_, _, _, decls, _, _, _, _)) = flattenDefns decls
                    match res with 
                    | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, _, specs)) -> 
                        dprintf "parsing yielded %d specs" (List.collect flattenModSpec specs).Length
                    | ParsedInput.ImplFile(ParsedImplFileInput(modules = impls)) -> 
                        dprintf "parsing yielded %d definitions" (List.collect flattenModImpl impls).Length
                res
            )
        if verbose then dprintn ("Parsed "+shortFilename)
        Some input 
    with e -> (* errorR(Failure("parse failed")); *) errorRecovery e rangeStartup; None 
            
            
let ParseOneInputFile (tcConfig: TcConfig, lexResourceManager, conditionalCompilationDefines, filename, isLastCompiland, errorLogger, retryLocked) =
    try 
       let lower = String.lowercase filename
       if List.exists (Filename.checkSuffix lower) (FSharpSigFileSuffixes@FSharpImplFileSuffixes)  then  
            if not(FileSystem.SafeExists(filename)) then
                error(Error(FSComp.SR.buildCouldNotFindSourceFile(filename), rangeStartup))
            // bug 3155: if the file name is indirect, use a full path
            let lexbuf = UnicodeLexing.UnicodeFileAsLexbuf(filename, tcConfig.inputCodePage, retryLocked) 
            ParseOneInputLexbuf(tcConfig, lexResourceManager, conditionalCompilationDefines, lexbuf, filename, isLastCompiland, errorLogger)
       else error(Error(FSComp.SR.buildInvalidSourceFileExtension(SanitizeFileName filename tcConfig.implicitIncludeDir), rangeStartup))
    with e -> (* errorR(Failure("parse failed")); *) errorRecovery e rangeStartup; None 
     

[<Sealed>] 
type TcAssemblyResolutions(tcConfig: TcConfig, results: AssemblyResolution list, unresolved : UnresolvedAssemblyReference list) = 

    let originalReferenceToResolution = results |> List.map (fun r -> r.originalReference.Text, r) |> Map.ofList
    let resolvedPathToResolution      = results |> List.map (fun r -> r.resolvedPath, r) |> Map.ofList

    /// Add some resolutions to the map of resolution results.                
    member tcResolutions.AddResolutionResults(newResults) = TcAssemblyResolutions(tcConfig, results @ newResults, unresolved)

    /// Add some unresolved results.
    member tcResolutions.AddUnresolvedReferences(newUnresolved) = TcAssemblyResolutions(tcConfig, results, unresolved @ newUnresolved)

    /// Get information about referenced DLLs
    member tcResolutions.GetAssemblyResolutions() = results
    member tcResolutions.GetUnresolvedReferences() = unresolved
    member tcResolutions.TryFindByOriginalReference(assemblyReference:AssemblyReference) = originalReferenceToResolution.TryFind assemblyReference.Text

    /// This doesn't need to be cancellable, it is only used by F# Interactive
    member tcResolution.TryFindByExactILAssemblyRef (ctok, assref) = 
        results |> List.tryFind (fun ar->
            let r = ar.GetILAssemblyRef(ctok, tcConfig.reduceMemoryUsage, tcConfig.tryGetMetadataSnapshot) |> Cancellable.runWithoutCancellation 
            r = assref)

    /// This doesn't need to be cancellable, it is only used by F# Interactive
    member tcResolution.TryFindBySimpleAssemblyName (ctok, simpleAssemName) = 
        results |> List.tryFind (fun ar->
            let r = ar.GetILAssemblyRef(ctok, tcConfig.reduceMemoryUsage, tcConfig.tryGetMetadataSnapshot) |> Cancellable.runWithoutCancellation 
            r.Name = simpleAssemName)

    member tcResolutions.TryFindByResolvedPath nm = resolvedPathToResolution.TryFind nm
    member tcResolutions.TryFindByOriginalReferenceText nm = originalReferenceToResolution.TryFind nm
        
    static member ResolveAssemblyReferences (ctok, tcConfig:TcConfig, assemblyList:AssemblyReference list, knownUnresolved:UnresolvedAssemblyReference list) : TcAssemblyResolutions =
        let resolved, unresolved = 
            if tcConfig.useSimpleResolution then 
                let resolutions = 
                    assemblyList 
                    |> List.map (fun assemblyReference -> 
                           try 
                               Choice1Of2 (tcConfig.ResolveLibWithDirectories (CcuLoadFailureAction.RaiseError, assemblyReference) |> Option.get)
                           with e -> 
                               errorRecovery e assemblyReference.Range
                               Choice2Of2 assemblyReference)
                let successes = resolutions |> List.choose (function Choice1Of2 x -> Some x | _ -> None)
                let failures = resolutions |> List.choose (function Choice2Of2 x -> Some (UnresolvedAssemblyReference(x.Text, [x])) | _ -> None)
                successes, failures
            else
                RequireCompilationThread ctok // we don't want to do assembly resolution concurrently, we assume MSBuild doesn't handle this
                TcConfig.TryResolveLibsUsingMSBuildRules (tcConfig, assemblyList, rangeStartup, ResolveAssemblyReferenceMode.ReportErrors)
        TcAssemblyResolutions(tcConfig, resolved, unresolved @ knownUnresolved)                    


    static member GetAllDllReferences (tcConfig:TcConfig) =
        [
            let primaryReference = tcConfig.PrimaryAssemblyDllReference()
            yield primaryReference

            if not tcConfig.compilingFslib then 
                yield tcConfig.CoreLibraryDllReference()

            let assumeDotNetFramework = primaryReference.SimpleAssemblyNameIs("mscorlib")
            if tcConfig.framework then 
                for s in DefaultReferencesForScriptsAndOutOfProjectSources(assumeDotNetFramework) do 
                    yield AssemblyReference(rangeStartup, (if s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) then s else s+".dll"), None)

            if tcConfig.useFsiAuxLib then
                let name = Path.Combine(tcConfig.fsharpBinariesDir, GetFsiLibraryName() + ".dll")
                yield AssemblyReference(rangeStartup, name, None)

            yield! tcConfig.referencedDLLs
        ]

    static member SplitNonFoundationalResolutions (ctok, tcConfig:TcConfig) =
        let assemblyList = TcAssemblyResolutions.GetAllDllReferences tcConfig
        let resolutions = TcAssemblyResolutions.ResolveAssemblyReferences (ctok, tcConfig, assemblyList, tcConfig.knownUnresolvedReferences)
        let frameworkDLLs, nonFrameworkReferences = resolutions.GetAssemblyResolutions() |> List.partition (fun r -> r.sysdir) 
        let unresolved = resolutions.GetUnresolvedReferences()
#if DEBUG
        let itFailed = ref false
        let addedText = "\nIf you want to debug this right now, attach a debugger, and put a breakpoint in 'CompileOps.fs' near the text '!itFailed', and you can re-step through the assembly resolution logic."
        unresolved 
        |> List.iter (fun (UnresolvedAssemblyReference(referenceText, _ranges)) ->
            if referenceText.Contains("mscorlib") then
                System.Diagnostics.Debug.Assert(false, sprintf "whoops, did not resolve mscorlib: '%s'%s" referenceText addedText)
                itFailed := true)
        frameworkDLLs 
        |> List.iter (fun x ->
            if not(FileSystem.IsPathRootedShim(x.resolvedPath)) then
                System.Diagnostics.Debug.Assert(false, sprintf "frameworkDLL should be absolute path: '%s'%s" x.resolvedPath addedText)
                itFailed := true)
        nonFrameworkReferences 
        |> List.iter (fun x -> 
            if not(FileSystem.IsPathRootedShim(x.resolvedPath)) then
                System.Diagnostics.Debug.Assert(false, sprintf "nonFrameworkReference should be absolute path: '%s'%s" x.resolvedPath addedText) 
                itFailed := true)
        if !itFailed then
            // idea is, put a breakpoint here and then step through
            let assemblyList = TcAssemblyResolutions.GetAllDllReferences tcConfig
            let resolutions = TcAssemblyResolutions.ResolveAssemblyReferences (ctok, tcConfig, assemblyList, [])
            let _frameworkDLLs, _nonFrameworkReferences = resolutions.GetAssemblyResolutions() |> List.partition (fun r -> r.sysdir) 
            ()
#endif
        frameworkDLLs, nonFrameworkReferences, unresolved

    static member BuildFromPriorResolutions (ctok, tcConfig:TcConfig, resolutions, knownUnresolved) =
        let references = resolutions |> List.map (fun r -> r.originalReference)
        TcAssemblyResolutions.ResolveAssemblyReferences (ctok, tcConfig, references, knownUnresolved)
            

//----------------------------------------------------------------------------
// Typecheck and optimization environments on disk
//--------------------------------------------------------------------------

let IsSignatureDataResource         (r: ILResource) = 
    r.Name.StartsWith FSharpSignatureDataResourceName ||
    r.Name.StartsWith FSharpSignatureDataResourceName2

let IsOptimizationDataResource      (r: ILResource) = 
    r.Name.StartsWith FSharpOptimizationDataResourceName || 
    r.Name.StartsWith FSharpOptimizationDataResourceName2

let GetSignatureDataResourceName    (r: ILResource) = 
    if r.Name.StartsWith FSharpSignatureDataResourceName then 
        String.dropPrefix r.Name FSharpSignatureDataResourceName
    elif r.Name.StartsWith FSharpSignatureDataResourceName2 then 
        String.dropPrefix r.Name FSharpSignatureDataResourceName2
    else failwith "GetSignatureDataResourceName"

let GetOptimizationDataResourceName (r: ILResource) = 
    if r.Name.StartsWith FSharpOptimizationDataResourceName then 
        String.dropPrefix r.Name FSharpOptimizationDataResourceName
    elif r.Name.StartsWith FSharpOptimizationDataResourceName2 then 
        String.dropPrefix r.Name FSharpOptimizationDataResourceName2
    else failwith "GetOptimizationDataResourceName"

let IsReflectedDefinitionsResource  (r: ILResource) = r.Name.StartsWith QuotationPickler.SerializedReflectedDefinitionsResourceNameBase

let MakeILResource rname bytes = 
    { Name = rname
      Location = ILResourceLocation.LocalOut bytes
      Access = ILResourceAccess.Public
      CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
      MetadataIndex = NoMetadataIdx }

let PickleToResource inMem file g scope rname p x = 
    { Name = rname
      Location = (let bytes = pickleObjWithDanglingCcus inMem file g scope p x in ILResourceLocation.LocalOut bytes)
      Access = ILResourceAccess.Public
      CustomAttrsStored = storeILCustomAttrs emptyILCustomAttrs
      MetadataIndex = NoMetadataIdx }

let GetSignatureData (file, ilScopeRef, ilModule, byteReader) : PickledDataWithReferences<PickledCcuInfo> = 
    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleCcuInfo (byteReader())

let WriteSignatureData (tcConfig: TcConfig, tcGlobals, exportRemapping, ccu: CcuThunk, file, inMem) : ILResource = 
    let mspec = ccu.Contents
    let mspec = ApplyExportRemappingToEntity tcGlobals exportRemapping mspec
    // For historical reasons, we use a different resource name for FSharp.Core, so older F# compilers 
    // don't complain when they see the resource.
    let rname = if ccu.AssemblyName = GetFSharpCoreLibraryName() then FSharpSignatureDataResourceName2 else FSharpSignatureDataResourceName 
    PickleToResource inMem file tcGlobals ccu (rname+ccu.AssemblyName) pickleCcuInfo 
        { mspec=mspec 
          compileTimeWorkingDir=tcConfig.implicitIncludeDir
          usesQuotations = ccu.UsesFSharp20PlusQuotations }

let GetOptimizationData (file, ilScopeRef, ilModule, byteReader) = 
    unpickleObjWithDanglingCcus file ilScopeRef ilModule Optimizer.u_CcuOptimizationInfo (byteReader())

let WriteOptimizationData (tcGlobals, file, inMem, ccu: CcuThunk, modulInfo) = 
    // For historical reasons, we use a different resource name for FSharp.Core, so older F# compilers 
    // don't complain when they see the resource.
    let rname = if ccu.AssemblyName = GetFSharpCoreLibraryName() then FSharpOptimizationDataResourceName2 else FSharpOptimizationDataResourceName 
    PickleToResource inMem file tcGlobals ccu (rname+ccu.AssemblyName) Optimizer.p_CcuOptimizationInfo modulInfo

//----------------------------------------------------------------------------
// Abstraction for project reference

type RawFSharpAssemblyDataBackedByFileOnDisk (ilModule: ILModuleDef, ilAssemblyRefs) = 
    let externalSigAndOptData = ["FSharp.Core"]
    interface IRawFSharpAssemblyData with 
         member __.GetAutoOpenAttributes(ilg) = GetAutoOpenAttributes ilg ilModule 
         member __.GetInternalsVisibleToAttributes(ilg) = GetInternalsVisibleToAttributes ilg ilModule 
         member __.TryGetILModuleDef() = Some ilModule 
         member __.GetRawFSharpSignatureData(m, ilShortAssemName, filename) = 
            let resources = ilModule.Resources.AsList
            let sigDataReaders = 
                [ for iresource in resources do
                    if IsSignatureDataResource iresource then 
                        let ccuName = GetSignatureDataResourceName iresource
                        yield (ccuName, fun () -> iresource.GetBytes()) ]
                        
            let sigDataReaders = 
                if sigDataReaders.IsEmpty && List.contains ilShortAssemName externalSigAndOptData then 
                    let sigFileName = Path.ChangeExtension(filename, "sigdata")
                    if not (FileSystem.SafeExists sigFileName) then 
                        error(Error(FSComp.SR.buildExpectedSigdataFile (FileSystem.GetFullPathShim sigFileName), m))
                    [ (ilShortAssemName, fun () -> FileSystem.ReadAllBytesShim sigFileName)]
                else
                    sigDataReaders
            sigDataReaders
         member __.GetRawFSharpOptimizationData(m, ilShortAssemName, filename) =             
            let optDataReaders = 
                ilModule.Resources.AsList
                |> List.choose (fun r -> if IsOptimizationDataResource r then Some(GetOptimizationDataResourceName r, (fun () -> r.GetBytes())) else None)

            // Look for optimization data in a file 
            let optDataReaders = 
                if optDataReaders.IsEmpty && List.contains ilShortAssemName externalSigAndOptData then 
                    let optDataFile = Path.ChangeExtension(filename, "optdata")
                    if not (FileSystem.SafeExists optDataFile)  then 
                        error(Error(FSComp.SR.buildExpectedFileAlongSideFSharpCore(optDataFile, FileSystem.GetFullPathShim optDataFile), m))
                    [ (ilShortAssemName, (fun () -> FileSystem.ReadAllBytesShim optDataFile))]
                else
                    optDataReaders
            optDataReaders
         member __.GetRawTypeForwarders() =
            match ilModule.Manifest with 
            | Some manifest -> manifest.ExportedTypes
            | None -> mkILExportedTypes []
         member __.ShortAssemblyName = GetNameOfILModule ilModule 
         member __.ILScopeRef = MakeScopeRefForILModule ilModule
         member __.ILAssemblyRefs = ilAssemblyRefs
         member __.HasAnyFSharpSignatureDataAttribute = 
            let attrs = GetCustomAttributesOfILModule ilModule
            List.exists IsSignatureDataVersionAttr attrs
         member __.HasMatchingFSharpSignatureDataAttribute(ilg) = 
            let attrs = GetCustomAttributesOfILModule ilModule
            List.exists (IsMatchingSignatureDataVersionAttr ilg (IL.parseILVersion Internal.Utilities.FSharpEnvironment.FSharpBinaryMetadataFormatRevision)) attrs


//----------------------------------------------------------------------------
// Relink blobs of saved data by fixing up ccus.
//--------------------------------------------------------------------------

let availableToOptionalCcu = function
    | ResolvedCcu(ccu) -> Some(ccu)
    | UnresolvedCcu _ -> None


//----------------------------------------------------------------------------
// TcConfigProvider
//--------------------------------------------------------------------------

/// Represents a computation to return a TcConfig. Normally this is just a constant immutable TcConfig, 
/// but for F# Interactive it may be based on an underlying mutable TcConfigBuilder.
type TcConfigProvider = 
    | TcConfigProvider of (CompilationThreadToken -> TcConfig)
    member x.Get(ctok) = (let (TcConfigProvider(f)) = x in f ctok)

    /// Get a TcConfigProvider which will return only the exact TcConfig.
    static member Constant(tcConfig) = TcConfigProvider(fun _ctok -> tcConfig)

    /// Get a TcConfigProvider which will continue to respect changes in the underlying
    /// TcConfigBuilder rather than delivering snapshots.
    static member BasedOnMutableBuilder(tcConfigB) = TcConfigProvider(fun _ctok -> TcConfig.Create(tcConfigB, validate=false))
    
    
//----------------------------------------------------------------------------
// TcImports
//--------------------------------------------------------------------------

          
/// Represents a table of imported assemblies with their resolutions.
[<Sealed>] 
type TcImports(tcConfigP:TcConfigProvider, initialResolutions:TcAssemblyResolutions, importsBase:TcImports option, ilGlobalsOpt) = 

    let mutable resolutions = initialResolutions
    let mutable importsBase : TcImports option = importsBase
    let mutable dllInfos: ImportedBinary list = []
    let mutable dllTable: NameMap<ImportedBinary> = NameMap.empty
    let mutable ccuInfos: ImportedAssembly list = []
    let mutable ccuTable: NameMap<ImportedAssembly> = NameMap.empty
    let mutable disposeActions = []
    let mutable disposed = false
    let mutable ilGlobalsOpt = ilGlobalsOpt
    let mutable tcGlobals = None
#if !NO_EXTENSIONTYPING
    let mutable generatedTypeRoots = new System.Collections.Generic.Dictionary<ILTypeRef, int * ProviderGeneratedType>()
#endif
    
    let CheckDisposed() =
        if disposed then assert false

    static let ccuHasType (ccu : CcuThunk) (nsname : string list) (tname : string) =
        match (Some ccu.Contents, nsname) ||> List.fold (fun entityOpt n -> match entityOpt with None -> None | Some entity -> entity.ModuleOrNamespaceType.AllEntitiesByCompiledAndLogicalMangledNames.TryFind n) with
        | Some ns ->
                match Map.tryFind tname ns.ModuleOrNamespaceType.TypesByMangledName with
                | Some _ -> true
                | None -> false
        | None -> false
  
    member private tcImports.Base  = 
            CheckDisposed()
            importsBase

    member tcImports.CcuTable =
            CheckDisposed()
            ccuTable
        
    member tcImports.DllTable =
            CheckDisposed()
            dllTable        
        
    member tcImports.RegisterCcu(ccuInfo) =
        CheckDisposed()
        ccuInfos <- ccuInfos ++ ccuInfo
        // Assembly Ref Resolution: remove this use of ccu.AssemblyName
        ccuTable <- NameMap.add (ccuInfo.FSharpViewOfMetadata.AssemblyName) ccuInfo ccuTable
    
    member tcImports.RegisterDll(dllInfo) =
        CheckDisposed()
        dllInfos <- dllInfos ++ dllInfo
        dllTable <- NameMap.add (getNameOfScopeRef dllInfo.ILScopeRef) dllInfo dllTable

    member tcImports.GetDllInfos() = 
        CheckDisposed()
        match importsBase with 
        | Some(importsBase)-> importsBase.GetDllInfos() @ dllInfos
        | None -> dllInfos
        
    member tcImports.AllAssemblyResolutions() = 
        CheckDisposed()
        let ars = resolutions.GetAssemblyResolutions()
        match importsBase with 
        | Some(importsBase)-> importsBase.AllAssemblyResolutions() @ ars
        | None -> ars
        
    member tcImports.TryFindDllInfo (ctok: CompilationThreadToken, m, assemblyName, lookupOnly) =
        CheckDisposed()
        let rec look (t:TcImports) =       
            match NameMap.tryFind assemblyName t.DllTable with
            | Some res -> Some(res)
            | None -> 
                match t.Base with 
                | Some t2 -> look(t2)
                | None -> None
        match look tcImports with
        | Some res -> Some res
        | None ->
            tcImports.ImplicitLoadIfAllowed(ctok, m, assemblyName, lookupOnly)
            look tcImports
    

    member tcImports.FindDllInfo (ctok, m, assemblyName) =
        match tcImports.TryFindDllInfo (ctok, m, assemblyName, lookupOnly=false) with 
        | Some res -> res
        | None -> error(Error(FSComp.SR.buildCouldNotResolveAssembly(assemblyName), m))

    member tcImports.GetImportedAssemblies() = 
        CheckDisposed()
        match importsBase with 
        | Some(importsBase)-> List.append (importsBase.GetImportedAssemblies())  ccuInfos
        | None -> ccuInfos        
        
    member tcImports.GetCcusExcludingBase() = 
        CheckDisposed()
        ccuInfos |> List.map (fun x -> x.FSharpViewOfMetadata)        

    member tcImports.GetCcusInDeclOrder() =         
        CheckDisposed()
        List.map (fun x -> x.FSharpViewOfMetadata) (tcImports.GetImportedAssemblies())  
        
    // This is the main "assembly reference --> assembly" resolution routine. 
    member tcImports.FindCcuInfo (ctok, m, assemblyName, lookupOnly) = 
        CheckDisposed()
        let rec look (t:TcImports) = 
            match NameMap.tryFind assemblyName t.CcuTable with
            | Some res -> Some(res)
            | None -> 
                 match t.Base with 
                 | Some t2 -> look t2 
                 | None -> None

        match look tcImports with
        | Some res -> ResolvedImportedAssembly(res)
        | None ->
            tcImports.ImplicitLoadIfAllowed(ctok, m, assemblyName, lookupOnly)
            match look tcImports with 
            | Some res -> ResolvedImportedAssembly(res)
            | None -> UnresolvedImportedAssembly(assemblyName)
        

    member tcImports.FindCcu (ctok, m, assemblyName, lookupOnly) = 
        CheckDisposed()
        match tcImports.FindCcuInfo(ctok, m, assemblyName, lookupOnly) with
        | ResolvedImportedAssembly(importedAssembly) -> ResolvedCcu(importedAssembly.FSharpViewOfMetadata)
        | UnresolvedImportedAssembly(assemblyName) -> UnresolvedCcu(assemblyName)

    member tcImports.FindCcuFromAssemblyRef(ctok, m, assref:ILAssemblyRef) = 
        CheckDisposed()
        match tcImports.FindCcuInfo(ctok, m, assref.Name, lookupOnly=false) with
        | ResolvedImportedAssembly(importedAssembly) -> ResolvedCcu(importedAssembly.FSharpViewOfMetadata)
        | UnresolvedImportedAssembly _ -> UnresolvedCcu(assref.QualifiedName)


#if !NO_EXTENSIONTYPING
    member tcImports.GetProvidedAssemblyInfo(ctok, m, assembly: Tainted<ProvidedAssembly>) = 
        let anameOpt = assembly.PUntaint((fun assembly -> match assembly with null -> None | a -> Some (a.GetName())), m)
        match anameOpt with 
        | None -> false, None
        | Some aname -> 
        let ilShortAssemName = aname.Name
        match tcImports.FindCcu (ctok, m, ilShortAssemName, lookupOnly=true) with 
        | ResolvedCcu ccu -> 
            if ccu.IsProviderGenerated then 
                let dllinfo = tcImports.FindDllInfo(ctok, m, ilShortAssemName)
                true, dllinfo.ProviderGeneratedStaticLinkMap
            else
                false, None

        | UnresolvedCcu _ -> 
            let g = tcImports.GetTcGlobals()
            let ilScopeRef = ILScopeRef.Assembly (ILAssemblyRef.FromAssemblyName aname)
            let fileName = aname.Name + ".dll"
            let bytes = assembly.PApplyWithProvider((fun (assembly, provider) -> assembly.GetManifestModuleContents(provider)), m).PUntaint(id, m)
            let tcConfig = tcConfigP.Get(ctok)
            let ilModule, ilAssemblyRefs = 
                let opts : ILReaderOptions = 
                    { ilGlobals = g.ilg 
                      reduceMemoryUsage = tcConfig.reduceMemoryUsage
                      pdbDirPath = None 
                      metadataOnly = MetadataOnlyFlag.Yes
                      tryGetMetadataSnapshot = tcConfig.tryGetMetadataSnapshot }
                let reader = OpenILModuleReaderFromBytes fileName bytes opts
                reader.ILModuleDef, reader.ILAssemblyRefs

            let theActualAssembly = assembly.PUntaint((fun x -> x.Handle), m)
            let dllinfo = 
                { RawMetadata= RawFSharpAssemblyDataBackedByFileOnDisk (ilModule, ilAssemblyRefs) 
                  FileName=fileName
                  ProviderGeneratedAssembly=Some theActualAssembly
                  IsProviderGenerated=true
                  ProviderGeneratedStaticLinkMap= if g.isInteractive then None else Some (ProvidedAssemblyStaticLinkingMap.CreateNew())
                  ILScopeRef = ilScopeRef
                  ILAssemblyRefs = ilAssemblyRefs }
            tcImports.RegisterDll(dllinfo)
            let ccuData : CcuData = 
              { IsFSharp=false
                UsesFSharp20PlusQuotations=false
                InvalidateEvent=(new Event<_>()).Publish
                IsProviderGenerated = true
                QualifiedName= Some (assembly.PUntaint((fun a -> a.FullName), m))
                Contents = NewCcuContents ilScopeRef m ilShortAssemName (NewEmptyModuleOrNamespaceType Namespace) 
                ILScopeRef = ilScopeRef
                Stamp = newStamp()
                SourceCodeDirectory = ""  
                FileName = Some fileName
                MemberSignatureEquality = (fun ty1 ty2 -> Tastops.typeEquivAux EraseAll g ty1 ty2)
                ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
                TryGetILModuleDef = (fun () -> Some ilModule)
                TypeForwarders = Map.empty }
                    
            let ccu = CcuThunk.Create(ilShortAssemName, ccuData)
            let ccuinfo = 
                { FSharpViewOfMetadata=ccu 
                  ILScopeRef = ilScopeRef 
                  AssemblyAutoOpenAttributes = []
                  AssemblyInternalsVisibleToAttributes = []
                  IsProviderGenerated = true
                  TypeProviders=[]
                  FSharpOptimizationData = notlazy None }
            tcImports.RegisterCcu(ccuinfo)
            // Yes, it is generative
            true, dllinfo.ProviderGeneratedStaticLinkMap

    member tcImports.RecordGeneratedTypeRoot root = 
        // checking if given ProviderGeneratedType was already recorded before (probably for another set of static parameters) 
        let (ProviderGeneratedType(_, ilTyRef, _)) = root
        let index = 
            match generatedTypeRoots.TryGetValue ilTyRef with
            | true, (index, _) -> index
            | false, _ -> generatedTypeRoots.Count
        generatedTypeRoots.[ilTyRef] <- (index, root)

    member tcImports.ProviderGeneratedTypeRoots = 
        generatedTypeRoots.Values
        |> Seq.sortBy fst
        |> Seq.map snd
        |> Seq.toList
#endif

    member tcImports.AttachDisposeAction(action) =
        CheckDisposed()
        disposeActions <- action :: disposeActions
  
    // Note: the returned binary reader is associated with the tcImports, i.e. when the tcImports are closed 
    // then the reader is closed. 
    member tcImports.OpenILBinaryModule(ctok, filename, m) = 
      try
        CheckDisposed()
        let tcConfig = tcConfigP.Get(ctok)
        let pdbDirPath =
            // We open the pdb file if one exists parallel to the binary we 
            // are reading, so that --standalone will preserve debug information. 
            if tcConfig.openDebugInformationForLaterStaticLinking then 
                let pdbDir = try Filename.directoryName filename with _ -> "."
                let pdbFile = (try Filename.chopExtension filename with _ -> filename) + ".pdb" 

                if FileSystem.SafeExists(pdbFile) then 
                    if verbose then dprintf "reading PDB file %s from directory %s\n" pdbFile pdbDir
                    Some pdbDir
                else
                    None
            else
                None
        let ilILBinaryReader = OpenILBinary(filename, tcConfig.reduceMemoryUsage, ilGlobalsOpt, pdbDirPath, tcConfig.shadowCopyReferences, tcConfig.tryGetMetadataSnapshot)
        tcImports.AttachDisposeAction(fun _ -> (ilILBinaryReader :> IDisposable).Dispose())
        ilILBinaryReader.ILModuleDef, ilILBinaryReader.ILAssemblyRefs
      with e ->
        error(Error(FSComp.SR.buildErrorOpeningBinaryFile(filename, e.Message), m))

    (* auxModTable is used for multi-module assemblies *)
    member tcImports.MkLoaderForMultiModuleILAssemblies ctok m =
        CheckDisposed()
        let auxModTable = HashMultiMap(10, HashIdentity.Structural)
        fun viewedScopeRef ->
        
            let tcConfig = tcConfigP.Get(ctok)
            match viewedScopeRef with
            | ILScopeRef.Module modref -> 
                let key = modref.Name
                if not (auxModTable.ContainsKey(key)) then
                    let resolution = tcConfig.ResolveLibWithDirectories (CcuLoadFailureAction.RaiseError, AssemblyReference(m, key, None)) |> Option.get
                    let ilModule, _ = tcImports.OpenILBinaryModule(ctok, resolution.resolvedPath, m)
                    auxModTable.[key] <- ilModule
                auxModTable.[key] 

            | _ -> 
                error(InternalError("Unexpected ILScopeRef.Local or ILScopeRef.Assembly in exported type table", m))

    member tcImports.IsAlreadyRegistered nm =
        CheckDisposed()
        tcImports.GetDllInfos() |> List.exists (fun dll -> 
            match dll.ILScopeRef with 
            | ILScopeRef.Assembly a -> a.Name =  nm 
            | _ -> false)

    member tcImports.GetImportMap() = 
        CheckDisposed()
        let loaderInterface = 
            { new Import.AssemblyLoader with 
                 member x.FindCcuFromAssemblyRef (ctok, m, ilAssemblyRef) = 
                     tcImports.FindCcuFromAssemblyRef (ctok, m, ilAssemblyRef)
#if !NO_EXTENSIONTYPING
                 member x.GetProvidedAssemblyInfo (ctok, m, assembly) = tcImports.GetProvidedAssemblyInfo (ctok, m, assembly)
                 member x.RecordGeneratedTypeRoot root = tcImports.RecordGeneratedTypeRoot root
#endif
             }
        new Import.ImportMap (tcImports.GetTcGlobals(), loaderInterface)

    // Note the tcGlobals are only available once mscorlib and fslib have been established. For TcImports, 
    // they are logically only needed when converting AbsIL data structures into F# data structures, and
    // when converting AbsIL types in particular, since these types are normalized through the tables
    // in the tcGlobals (E.g. normalizing 'System.Int32' to 'int'). On the whole ImportILAssembly doesn't
    // actually convert AbsIL types - it only converts the outer shell of type definitions - the vast majority of
    // types such as those in method signatures are currently converted on-demand. However ImportILAssembly does have to
    // convert the types that are constraints in generic parameters, which was the original motivation for making sure that
    // ImportILAssembly had a tcGlobals available when it really needs it.
    member tcImports.GetTcGlobals() : TcGlobals =
        CheckDisposed()
        match tcGlobals with 
        | Some g -> g 
        | None -> 
            match importsBase with 
            | Some b -> b.GetTcGlobals() 
            | None -> failwith "unreachable: GetGlobals - are the references to mscorlib.dll and FSharp.Core.dll valid?"

    member private tcImports.SetILGlobals ilg =
        CheckDisposed()
        ilGlobalsOpt <- Some ilg

    member private tcImports.SetTcGlobals g =
        CheckDisposed()
        tcGlobals <- Some g

#if !NO_EXTENSIONTYPING
    member private tcImports.InjectProvidedNamespaceOrTypeIntoEntity 
            (typeProviderEnvironment, 
             tcConfig:TcConfig, 
             m, entity:Entity, 
             injectedNamspace, remainingNamespace, 
             provider, 
             st:Tainted<ProvidedType> option) = 
        match remainingNamespace with
        | next::rest ->
            // Inject the namespace entity 
            match entity.ModuleOrNamespaceType.ModulesAndNamespacesByDemangledName.TryFind(next) with
            | Some childEntity ->
                tcImports.InjectProvidedNamespaceOrTypeIntoEntity (typeProviderEnvironment, tcConfig, m, childEntity, next::injectedNamspace, rest, provider, st)
            | None -> 
                // Build up the artificial namespace if there is not a real one.
                let cpath = CompPath(ILScopeRef.Local, injectedNamspace |> List.rev |> List.map (fun n -> (n, ModuleOrNamespaceKind.Namespace)) )
                let newNamespace = NewModuleOrNamespace (Some cpath) taccessPublic (ident(next, rangeStartup)) XmlDoc.Empty [] (MaybeLazy.Strict (NewEmptyModuleOrNamespaceType Namespace)) 
                entity.ModuleOrNamespaceType.AddModuleOrNamespaceByMutation(newNamespace)
                tcImports.InjectProvidedNamespaceOrTypeIntoEntity (typeProviderEnvironment, tcConfig, m, newNamespace, next::injectedNamspace, rest, provider, st)
        | [] -> 
            match st with
            | Some st ->
                // Inject the wrapper type into the provider assembly.
                //
                // Generated types get properly injected into the provided (i.e. generated) assembly CCU in tc.fs

                let importProvidedType t = Import.ImportProvidedType (tcImports.GetImportMap()) m t
                let isSuppressRelocate = tcConfig.isInteractive || st.PUntaint((fun st -> st.IsSuppressRelocate), m) 
                let newEntity = Construct.NewProvidedTycon(typeProviderEnvironment, st, importProvidedType, isSuppressRelocate, m) 
                entity.ModuleOrNamespaceType.AddProvidedTypeEntity(newEntity)
            | None -> ()

            entity.entity_tycon_repr <-
                match entity.TypeReprInfo with 
                // This is the first extension 
                | TNoRepr -> 
                    TProvidedNamespaceExtensionPoint(typeProviderEnvironment, [provider])
                                
                // Add to the existing list of extensions
                | TProvidedNamespaceExtensionPoint(resolutionFolder, prior) as repr -> 
                    if not(prior |> List.exists(fun r->Tainted.EqTainted r provider)) then 
                        TProvidedNamespaceExtensionPoint(resolutionFolder, provider::prior)
                    else 
                        repr

                | _ -> failwith "Unexpected representation in namespace entity referred to by a type provider"

    member tcImports.ImportTypeProviderExtensions 
               (ctok, tcConfig:TcConfig, 
                fileNameOfRuntimeAssembly, 
                ilScopeRefOfRuntimeAssembly, 
                runtimeAssemblyAttributes:ILAttribute list, 
                entityToInjectInto, invalidateCcu:Event<_>, m) = 

        let startingErrorCount = CompileThreadStatic.ErrorLogger.ErrorCount

        // Find assembly level TypeProviderAssemblyAttributes. These will point to the assemblies that 
        // have class which implement ITypeProvider and which have TypeProviderAttribute on them.
        let designTimeAssemblyNames = 
            runtimeAssemblyAttributes 
            |> List.choose (TryDecodeTypeProviderAssemblyAttr (defaultArg ilGlobalsOpt EcmaMscorlibILGlobals))
            // If no design-time assembly is specified, use the runtime assembly
            |> List.map (function null -> fileNameOfRuntimeAssembly | s -> s)
            // For each simple name of a design-time assembly, we take the first matching one in the order they are 
            // specified in the attributes
            |> List.distinctBy (fun s -> try Path.GetFileNameWithoutExtension(s) with _ -> s)

        if not (List.isEmpty designTimeAssemblyNames) then

            // Find the SystemRuntimeAssemblyVersion value to report in the TypeProviderConfig.
            let primaryAssemblyVersion = 
                let primaryAssemblyRef = tcConfig.PrimaryAssemblyDllReference()
                let resolution = tcConfig.ResolveLibWithDirectories (CcuLoadFailureAction.RaiseError, primaryAssemblyRef) |> Option.get
                 // MSDN: this method causes the file to be opened and closed, but the assembly is not added to this domain
                let name = System.Reflection.AssemblyName.GetAssemblyName(resolution.resolvedPath)
                name.Version

            let typeProviderEnvironment = 
                 { resolutionFolder       = tcConfig.implicitIncludeDir
                   outputFile             = tcConfig.outputFile
                   showResolutionMessages = tcConfig.showExtensionTypeMessages 
                   referencedAssemblies   = Array.distinct [| for r in tcImports.AllAssemblyResolutions() -> r.resolvedPath |]
                   temporaryFolder        = FileSystem.GetTempPathShim() }

            // The type provider should not hold strong references to disposed
            // TcImport objects.  So the callbacks provided in the type provider config
            // dispatch via a thunk which gets set to a non-resource-capturing 
            // failing function when the object is disposed. 
            let systemRuntimeContainsType =  
                // NOTE: do not touch this
                let systemRuntimeContainsTypeRef = ref (fun typeName -> tcImports.SystemRuntimeContainsType(typeName))
                tcImports.AttachDisposeAction(fun () -> systemRuntimeContainsTypeRef := (fun _ -> raise (System.ObjectDisposedException("The type provider has been disposed"))))  
                fun arg -> systemRuntimeContainsTypeRef.Value arg  

            let providers = 
                [ for designTimeAssemblyName in designTimeAssemblyNames do
                      yield! ExtensionTyping.GetTypeProvidersOfAssembly(fileNameOfRuntimeAssembly, ilScopeRefOfRuntimeAssembly, designTimeAssemblyName, typeProviderEnvironment, 
                                                                        tcConfig.isInvalidationSupported, tcConfig.isInteractive, systemRuntimeContainsType, primaryAssemblyVersion, m) ]

            // Note, type providers are disposable objects. The TcImports owns the provider objects - when/if it is disposed, the providers are disposed.
            // We ignore all exceptions from provider disposal.
            for provider in providers do 
                tcImports.AttachDisposeAction(fun () -> 
                    try 
                        provider.PUntaintNoFailure(fun x -> x).Dispose() 
                    with e -> 
                        ())
            
            // Add the invalidation signal handlers to each provider
            for provider in providers do 
                provider.PUntaint((fun tp -> 
                    let handler = tp.Invalidate.Subscribe(fun _ -> invalidateCcu.Trigger ("The provider '" + fileNameOfRuntimeAssembly + "' reported a change"))  
                    tcImports.AttachDisposeAction(fun () -> try handler.Dispose() with _ -> ())), m)  
                
            match providers with
            | [] -> 
                warning(Error(FSComp.SR.etHostingAssemblyFoundWithoutHosts(fileNameOfRuntimeAssembly, typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>.FullName), m)) 
            | _ -> 

#if DEBUG
                if typeProviderEnvironment.showResolutionMessages then
                    dprintfn "Found extension type hosting hosting assembly '%s' with the following extensions:" fileNameOfRuntimeAssembly
                    providers |> List.iter(fun provider ->dprintfn " %s" (ExtensionTyping.DisplayNameOfTypeProvider(provider.TypeProvider, m)))
#endif
                    
                for provider in providers do 
                    try
                        // Inject an entity for the namespace, or if one already exists, then record this as a provider
                        // for that namespace.
                        let rec loop (providedNamespace: Tainted<IProvidedNamespace>) =
                            let path = ExtensionTyping.GetProvidedNamespaceAsPath(m, provider, providedNamespace.PUntaint((fun r -> r.NamespaceName), m))
                            tcImports.InjectProvidedNamespaceOrTypeIntoEntity (typeProviderEnvironment, tcConfig, m, entityToInjectInto, [], path, provider, None)

                            // Inject entities for the types returned by provider.GetTypes(). 
                            //
                            // NOTE: The types provided by GetTypes() are available for name resolution
                            // when the namespace is "opened". This is part of the specification of the language
                            // feature.
                            let tys = providedNamespace.PApplyArray((fun provider -> provider.GetTypes()), "GetTypes", m)
                            let ptys = [| for ty in tys -> ty.PApply((fun ty -> ty |> ProvidedType.CreateNoContext), m) |]
                            for st in ptys do 
                                tcImports.InjectProvidedNamespaceOrTypeIntoEntity (typeProviderEnvironment, tcConfig, m, entityToInjectInto, [], path, provider, Some st)

                            for providedNestedNamespace in providedNamespace.PApplyArray((fun provider -> provider.GetNestedNamespaces()), "GetNestedNamespaces", m) do 
                                loop providedNestedNamespace

                        RequireCompilationThread ctok // IProvidedType.GetNamespaces is an example of a type provider call
                        let providedNamespaces = provider.PApplyArray((fun r -> r.GetNamespaces()), "GetNamespaces", m)

                        for providedNamespace in providedNamespaces do
                            loop providedNamespace
                    with e -> 
                        errorRecovery e m

                if startingErrorCount<CompileThreadStatic.ErrorLogger.ErrorCount then
                    error(Error(FSComp.SR.etOneOrMoreErrorsSeenDuringExtensionTypeSetting(), m))  

            providers 
        else []
#endif

    /// Query information about types available in target system runtime library
    member tcImports.SystemRuntimeContainsType (typeName : string) : bool = 
        let ns, typeName = IL.splitILTypeName typeName
        let tcGlobals = tcImports.GetTcGlobals()
        tcGlobals.TryFindSysTyconRef ns typeName |> Option.isSome

    // Add a referenced assembly
    //
    // Retargetable assembly refs are required for binaries that must run 
    // against DLLs supported by multiple publishers. For example
    // Compact Framework binaries must use this. However it is not
    // clear when else it is required, e.g. for Mono.
    
    member tcImports.PrepareToImportReferencedILAssembly (ctok, m, filename, dllinfo:ImportedBinary) =
        CheckDisposed()
        let tcConfig = tcConfigP.Get(ctok)
        assert dllinfo.RawMetadata.TryGetILModuleDef().IsSome
        let ilModule = dllinfo.RawMetadata.TryGetILModuleDef().Value
        let ilScopeRef = dllinfo.ILScopeRef
        let aref =   
            match ilScopeRef with 
            | ILScopeRef.Assembly aref -> aref 
            | _ -> error(InternalError("PrepareToImportReferencedILAssembly: cannot reference .NET netmodules directly, reference the containing assembly instead", m))

        let nm = aref.Name
        if verbose then dprintn ("Converting IL assembly to F# data structures "+nm)
        let auxModuleLoader = tcImports.MkLoaderForMultiModuleILAssemblies ctok m
        let invalidateCcu = new Event<_>()
        let ccu = Import.ImportILAssembly(tcImports.GetImportMap, m, auxModuleLoader, ilScopeRef, tcConfig.implicitIncludeDir, Some filename, ilModule, invalidateCcu.Publish)
        
        let ilg = defaultArg ilGlobalsOpt EcmaMscorlibILGlobals

        let ccuinfo = 
            { FSharpViewOfMetadata=ccu 
              ILScopeRef = ilScopeRef 
              AssemblyAutoOpenAttributes = GetAutoOpenAttributes ilg ilModule
              AssemblyInternalsVisibleToAttributes = GetInternalsVisibleToAttributes ilg ilModule
#if !NO_EXTENSIONTYPING
              IsProviderGenerated = false 
              TypeProviders = []
#endif
              FSharpOptimizationData = notlazy None }
        tcImports.RegisterCcu(ccuinfo)
        let phase2 () = 
#if !NO_EXTENSIONTYPING
            ccuinfo.TypeProviders <- tcImports.ImportTypeProviderExtensions (ctok, tcConfig, filename, ilScopeRef, ilModule.ManifestOfAssembly.CustomAttrs.AsList, ccu.Contents, invalidateCcu, m)
#endif
            [ResolvedImportedAssembly(ccuinfo)]
        phase2

    member tcImports.PrepareToImportReferencedFSharpAssembly (ctok, m, filename, dllinfo:ImportedBinary) =
        CheckDisposed()
#if !NO_EXTENSIONTYPING
        let tcConfig = tcConfigP.Get(ctok)
#endif
        let ilModule = dllinfo.RawMetadata 
        let ilScopeRef = dllinfo.ILScopeRef 
        let ilShortAssemName = getNameOfScopeRef ilScopeRef 
        if verbose then dprintn ("Converting F# assembly to F# data structures "+(getNameOfScopeRef ilScopeRef))
        if verbose then dprintn ("Relinking interface info from F# assembly "+ilShortAssemName)
        let optDataReaders = ilModule.GetRawFSharpOptimizationData(m, ilShortAssemName, filename)

        let ccuRawDataAndInfos = 
            ilModule.GetRawFSharpSignatureData(m, ilShortAssemName, filename)
            |> List.map (fun (ccuName, sigDataReader) -> 
                let data = GetSignatureData (filename, ilScopeRef, ilModule.TryGetILModuleDef(), sigDataReader)

                let optDatas = Map.ofList optDataReaders

                let minfo : PickledCcuInfo = data.RawData 
                let mspec = minfo.mspec 

#if !NO_EXTENSIONTYPING
                let invalidateCcu = new Event<_>()
#endif

                let codeDir = minfo.compileTimeWorkingDir
                let ccuData : CcuData = 
                    { ILScopeRef=ilScopeRef
                      Stamp = newStamp()
                      FileName = Some filename 
                      QualifiedName= Some(ilScopeRef.QualifiedName)
                      SourceCodeDirectory = codeDir  (* note: in some cases we fix up this information later *)
                      IsFSharp=true
                      Contents = mspec 
#if !NO_EXTENSIONTYPING
                      InvalidateEvent=invalidateCcu.Publish
                      IsProviderGenerated = false
                      ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
                      TryGetILModuleDef = ilModule.TryGetILModuleDef
                      UsesFSharp20PlusQuotations = minfo.usesQuotations
                      MemberSignatureEquality= (fun ty1 ty2 -> Tastops.typeEquivAux EraseAll (tcImports.GetTcGlobals()) ty1 ty2)
                      TypeForwarders = ImportILAssemblyTypeForwarders(tcImports.GetImportMap, m, ilModule.GetRawTypeForwarders()) }

                let ccu = CcuThunk.Create(ccuName, ccuData)

                let optdata = 
                    lazy 
                        (match Map.tryFind ccuName optDatas  with 
                         | None -> 
                            if verbose then dprintf "*** no optimization data for CCU %s, was DLL compiled with --no-optimization-data??\n" ccuName 
                            None
                         | Some info -> 
                            let data = GetOptimizationData (filename, ilScopeRef, ilModule.TryGetILModuleDef(), info)
                            let res = data.OptionalFixup(fun nm -> availableToOptionalCcu(tcImports.FindCcu(ctok, m, nm, lookupOnly=false))) 
                            if verbose then dprintf "found optimization data for CCU %s\n" ccuName 
                            Some res)
                let ilg = defaultArg ilGlobalsOpt EcmaMscorlibILGlobals
                let ccuinfo = 
                    { FSharpViewOfMetadata=ccu 
                      AssemblyAutoOpenAttributes = ilModule.GetAutoOpenAttributes(ilg)
                      AssemblyInternalsVisibleToAttributes = ilModule.GetInternalsVisibleToAttributes(ilg)
                      FSharpOptimizationData=optdata 
#if !NO_EXTENSIONTYPING
                      IsProviderGenerated = false
                      TypeProviders = []
#endif
                      ILScopeRef = ilScopeRef }  
                let phase2() = 
#if !NO_EXTENSIONTYPING
                     match ilModule.TryGetILModuleDef() with 
                     | None -> () // no type providers can be used without a real IL Module present
                     | Some ilModule ->
                         ccuinfo.TypeProviders <- tcImports.ImportTypeProviderExtensions (ctok, tcConfig, filename, ilScopeRef, ilModule.ManifestOfAssembly.CustomAttrs.AsList, ccu.Contents, invalidateCcu, m)
#else
                     ()
#endif
                data, ccuinfo, phase2)
                     
        // Register all before relinking to cope with mutually-referential ccus 
        ccuRawDataAndInfos |> List.iter (p23 >> tcImports.RegisterCcu)
        let phase2 () = 
            (* Relink *)
            (* dprintf "Phase2: %s\n" filename; REMOVE DIAGNOSTICS *)
            ccuRawDataAndInfos |> List.iter (fun (data, _, _) -> data.OptionalFixup(fun nm -> availableToOptionalCcu(tcImports.FindCcu(ctok, m, nm, lookupOnly=false))) |> ignore)
#if !NO_EXTENSIONTYPING
            ccuRawDataAndInfos |> List.iter (fun (_, _, phase2) -> phase2())
#endif
            ccuRawDataAndInfos |> List.map p23 |> List.map ResolvedImportedAssembly  
        phase2
         

    // NOTE: When used in the Language Service this can cause the transitive checking of projects.  Hence it must be cancellable.
    member tcImports.RegisterAndPrepareToImportReferencedDll (ctok, r:AssemblyResolution) : Cancellable<_ * (unit -> AvailableImportedAssembly list)> =
      cancellable {
        CheckDisposed()
        let m = r.originalReference.Range
        let filename = r.resolvedPath
        let! contentsOpt = 
          cancellable {
            match r.ProjectReference with 
            | Some ilb -> return! ilb.EvaluateRawContents(ctok)
            | None -> return None
          }

        let assemblyData = 
            match contentsOpt with 
            | Some ilb -> ilb
            | None -> 
                let ilModule, ilAssemblyRefs = tcImports.OpenILBinaryModule(ctok, filename, m)
                RawFSharpAssemblyDataBackedByFileOnDisk (ilModule, ilAssemblyRefs) :> IRawFSharpAssemblyData

        let ilShortAssemName = assemblyData.ShortAssemblyName 
        let ilScopeRef = assemblyData.ILScopeRef

        if tcImports.IsAlreadyRegistered ilShortAssemName then 
            let dllinfo = tcImports.FindDllInfo(ctok, m, ilShortAssemName)
            let phase2() = [tcImports.FindCcuInfo(ctok, m, ilShortAssemName, lookupOnly=true)] 
            return dllinfo, phase2
        else 
            let dllinfo = 
                { RawMetadata=assemblyData 
                  FileName=filename
#if !NO_EXTENSIONTYPING
                  ProviderGeneratedAssembly=None
                  IsProviderGenerated=false
                  ProviderGeneratedStaticLinkMap = None
#endif
                  ILScopeRef = ilScopeRef
                  ILAssemblyRefs = assemblyData.ILAssemblyRefs }
            tcImports.RegisterDll(dllinfo)
            let ilg = defaultArg ilGlobalsOpt EcmaMscorlibILGlobals
            let phase2 = 
                if assemblyData.HasAnyFSharpSignatureDataAttribute  then 
                    if not (assemblyData.HasMatchingFSharpSignatureDataAttribute(ilg)) then 
                      errorR(Error(FSComp.SR.buildDifferentVersionMustRecompile(filename), m))
                      tcImports.PrepareToImportReferencedILAssembly (ctok, m, filename, dllinfo)
                    else 
                      try
                        tcImports.PrepareToImportReferencedFSharpAssembly (ctok, m, filename, dllinfo)
                      with e -> error(Error(FSComp.SR.buildErrorOpeningBinaryFile(filename, e.Message), m))
                else 
                    tcImports.PrepareToImportReferencedILAssembly (ctok, m, filename, dllinfo)
            return dllinfo, phase2
         }

    // NOTE: When used in the Language Service this can cause the transitive checking of projects.  Hence it must be cancellable.
    member tcImports.RegisterAndImportReferencedAssemblies (ctok, nms:AssemblyResolution list) =
      cancellable {
        CheckDisposed()

        let! results = 
           nms |> Cancellable.each (fun nm -> 
               cancellable {
                   try
                            let! res = tcImports.RegisterAndPrepareToImportReferencedDll (ctok, nm)
                            return Some res
                   with e ->
                            errorR(Error(FSComp.SR.buildProblemReadingAssembly(nm.resolvedPath, e.Message), nm.originalReference.Range))
                            return None 
               })

        let dllinfos, phase2s = results |> List.choose id |> List.unzip
        let ccuinfos = (List.collect (fun phase2 -> phase2()) phase2s) 
        return dllinfos, ccuinfos
      }
      
    /// Note that implicit loading is not used for compilations from MSBuild, which passes ``--noframework``
    /// Implicit loading is done in non-cancellation mode.  Implicit loading is never used in the language service, so 
    /// no cancellation is needed.
    member tcImports.ImplicitLoadIfAllowed (ctok, m, assemblyName, lookupOnly) = 
        CheckDisposed()
        // If the user is asking for the default framework then also try to resolve other implicit assemblies as they are discovered.
        // Using this flag to mean 'allow implicit discover of assemblies'.
        let tcConfig = tcConfigP.Get(ctok)
        if not lookupOnly && tcConfig.implicitlyResolveAssemblies then 
            let tryFile speculativeFileName = 
                let foundFile = tcImports.TryResolveAssemblyReference (ctok, AssemblyReference (m, speculativeFileName, None), ResolveAssemblyReferenceMode.Speculative)
                match foundFile with 
                | OkResult (warns, res) ->
                    ReportWarnings warns
                    tcImports.RegisterAndImportReferencedAssemblies(ctok, res) |> Cancellable.runWithoutCancellation |> ignore
                    true
                | ErrorResult (_warns, _err) -> 
                    // Throw away warnings and errors - this is speculative loading
                    false

            if tryFile (assemblyName + ".dll") then ()
            else tryFile (assemblyName + ".exe")  |> ignore

#if !NO_EXTENSIONTYPING
    member tcImports.TryFindProviderGeneratedAssemblyByName(ctok, assemblyName:string) :  System.Reflection.Assembly option = 
        // The assembly may not be in the resolutions, but may be in the load set including EST injected assemblies
        match tcImports.TryFindDllInfo (ctok, range0, assemblyName, lookupOnly=true) with 
        | Some res -> 
            // Provider-generated assemblies don't necessarily have an on-disk representation we can load.
            res.ProviderGeneratedAssembly 
        | _ -> None
#endif

    /// This doesn't need to be cancellable, it is only used by F# Interactive
    member tcImports.TryFindExistingFullyQualifiedPathBySimpleAssemblyName (ctok, simpleAssemName) :  string option = 
        resolutions.TryFindBySimpleAssemblyName (ctok, simpleAssemName) |> Option.map (fun r -> r.resolvedPath)

    /// This doesn't need to be cancellable, it is only used by F# Interactive
    member tcImports.TryFindExistingFullyQualifiedPathByExactAssemblyRef(ctok, assref:ILAssemblyRef) :  string option = 
        resolutions.TryFindByExactILAssemblyRef (ctok, assref)  |> Option.map (fun r -> r.resolvedPath)

    member tcImports.TryResolveAssemblyReference(ctok, assemblyReference:AssemblyReference, mode:ResolveAssemblyReferenceMode) : OperationResult<AssemblyResolution list> = 
        let tcConfig = tcConfigP.Get(ctok)
        // First try to lookup via the original reference text.
        match resolutions.TryFindByOriginalReference assemblyReference with
        | Some assemblyResolution -> 
            ResultD [assemblyResolution]
        | None ->
#if NO_MSBUILD_REFERENCE_RESOLUTION
           try 
               ResultD [tcConfig.ResolveLibWithDirectories assemblyReference]
           with e -> 
               ErrorD(e)
#else                      
            // Next try to lookup up by the exact full resolved path.
            match resolutions.TryFindByResolvedPath assemblyReference.Text with 
            | Some assemblyResolution -> 
                ResultD [assemblyResolution]
            | None ->      
                if tcConfigP.Get(ctok).useSimpleResolution then
                    let action = 
                        match mode with 
                        | ResolveAssemblyReferenceMode.ReportErrors -> CcuLoadFailureAction.RaiseError
                        | ResolveAssemblyReferenceMode.Speculative -> CcuLoadFailureAction.ReturnNone
                    match tcConfig.ResolveLibWithDirectories (action, assemblyReference) with 
                    | Some resolved -> 
                        resolutions <- resolutions.AddResolutionResults [resolved]
                        ResultD [resolved]
                    | None ->
                        ErrorD(AssemblyNotResolved(assemblyReference.Text, assemblyReference.Range))
                else 
                    // This is a previously unencounterd assembly. Resolve it and add it to the list.
                    // But don't cache resolution failures because the assembly may appear on the disk later.
                    let resolved, unresolved = TcConfig.TryResolveLibsUsingMSBuildRules(tcConfig, [ assemblyReference ], assemblyReference.Range, mode)
                    match resolved, unresolved with
                    | (assemblyResolution::_, _)  -> 
                        resolutions <- resolutions.AddResolutionResults resolved
                        ResultD [assemblyResolution]
                    | (_, _::_)  -> 
                        resolutions <- resolutions.AddUnresolvedReferences unresolved
                        ErrorD(AssemblyNotResolved(assemblyReference.Text, assemblyReference.Range))
                    | [], [] -> 
                        // Note, if mode=ResolveAssemblyReferenceMode.Speculative and the resolution failed then TryResolveLibsUsingMSBuildRules returns
                        // the empty list and we convert the failure into an AssemblyNotResolved here.
                        ErrorD(AssemblyNotResolved(assemblyReference.Text, assemblyReference.Range))

#endif                        
     

    member tcImports.ResolveAssemblyReference(ctok, assemblyReference, mode) : AssemblyResolution list = 
        CommitOperationResult(tcImports.TryResolveAssemblyReference(ctok, assemblyReference, mode))

    // Note: This returns a TcImports object. However, framework TcImports are not currently disposed. The only reason
    // we dispose TcImports is because we need to dispose type providers, and type providers are never included in the framework DLL set.
    //
    // If this ever changes then callers may need to begin disposing the TcImports (though remember, not before all derived 
    // non-framework TcImports built related to this framework TcImports are disposed).
    static member BuildFrameworkTcImports (ctok, tcConfigP:TcConfigProvider, frameworkDLLs, nonFrameworkDLLs) =
      cancellable {

        let tcConfig = tcConfigP.Get(ctok)
        let tcResolutions = TcAssemblyResolutions.BuildFromPriorResolutions(ctok, tcConfig, frameworkDLLs, [])
        let tcAltResolutions = TcAssemblyResolutions.BuildFromPriorResolutions(ctok, tcConfig, nonFrameworkDLLs, [])

        // Note: TcImports are disposable - the caller owns this object and must dispose
        let frameworkTcImports = new TcImports(tcConfigP, tcResolutions, None, None) 
        
        let primaryAssemblyReference = tcConfig.PrimaryAssemblyDllReference()
        let primaryAssemblyResolution = frameworkTcImports.ResolveAssemblyReference(ctok, primaryAssemblyReference, ResolveAssemblyReferenceMode.ReportErrors)
        let! primaryAssem = frameworkTcImports.RegisterAndImportReferencedAssemblies(ctok, primaryAssemblyResolution)
        let primaryScopeRef = 
            match primaryAssem with
              | (_, [ResolvedImportedAssembly(ccu)]) -> ccu.FSharpViewOfMetadata.ILScopeRef
              | _        -> failwith "unexpected"

        let ilGlobals = mkILGlobals primaryScopeRef
        frameworkTcImports.SetILGlobals ilGlobals

        // Load the rest of the framework DLLs all at once (they may be mutually recursive)
        let! _assemblies = frameworkTcImports.RegisterAndImportReferencedAssemblies (ctok, tcResolutions.GetAssemblyResolutions())

        // These are the DLLs we can search for well-known types
        let sysCcus =  
             [| for ccu in frameworkTcImports.GetCcusInDeclOrder() do
                   //printfn "found sys ccu %s" ccu.AssemblyName
                   yield ccu |]

        //for ccu in nonFrameworkDLLs do
        //    printfn "found non-sys ccu %s" ccu.resolvedPath

        let tryFindSysTypeCcu path typeName =
            sysCcus |> Array.tryFind (fun ccu -> ccuHasType ccu path typeName) 

        let fslibCcu = 
            if tcConfig.compilingFslib then 
                // When compiling FSharp.Core.dll, the fslibCcu reference to FSharp.Core.dll is a delayed ccu thunk fixed up during type checking
                CcuThunk.CreateDelayed(GetFSharpCoreLibraryName())
            else
                let fslibCcuInfo =
                    let coreLibraryReference = tcConfig.CoreLibraryDllReference()
                    
                    let resolvedAssemblyRef = 
                        match tcResolutions.TryFindByOriginalReference coreLibraryReference with
                        | Some resolution -> Some resolution
                        | _ -> 
                            // Are we using a "non-canonical" FSharp.Core?
                            match tcAltResolutions.TryFindByOriginalReference coreLibraryReference with
                            | Some resolution -> Some resolution
                            | _ -> tcResolutions.TryFindByOriginalReferenceText (GetFSharpCoreLibraryName())  // was the ".dll" elided?
                    
                    match resolvedAssemblyRef with 
                    | Some coreLibraryResolution -> 
                        match frameworkTcImports.RegisterAndImportReferencedAssemblies(ctok, [coreLibraryResolution]) |> Cancellable.runWithoutCancellation with
                        | (_, [ResolvedImportedAssembly(fslibCcuInfo) ]) -> fslibCcuInfo
                        | _ -> 
                            error(InternalError("BuildFrameworkTcImports: no successful import of "+coreLibraryResolution.resolvedPath, coreLibraryResolution.originalReference.Range))
                    | None -> 
                        error(InternalError(sprintf "BuildFrameworkTcImports: no resolution of '%s'" coreLibraryReference.Text, rangeStartup))
                IlxSettings.ilxFsharpCoreLibAssemRef := 
                    (let scoref = fslibCcuInfo.ILScopeRef
                     match scoref with
                     | ILScopeRef.Assembly aref             -> Some aref
                     | ILScopeRef.Local | ILScopeRef.Module _ -> error(InternalError("not ILScopeRef.Assembly", rangeStartup)))
                fslibCcuInfo.FSharpViewOfMetadata            
                  
        // OK, now we have both mscorlib.dll and FSharp.Core.dll we can create TcGlobals
        let tcGlobals = TcGlobals(tcConfig.compilingFslib, ilGlobals, fslibCcu, 
                                    tcConfig.implicitIncludeDir, tcConfig.mlCompatibility, 
                                    tcConfig.isInteractive, tryFindSysTypeCcu, tcConfig.emitDebugInfoInQuotations, tcConfig.noDebugData )

#if DEBUG
        // the global_g reference cell is used only for debug printing
        global_g := Some tcGlobals
#endif
        // do this prior to parsing, since parsing IL assembly code may refer to mscorlib
#if !NO_INLINE_IL_PARSER
        Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiConstants.parseILGlobals := tcGlobals.ilg 
#endif
        frameworkTcImports.SetTcGlobals(tcGlobals)
        return tcGlobals, frameworkTcImports
      }

    member tcImports.ReportUnresolvedAssemblyReferences(knownUnresolved) =
        // Report that an assembly was not resolved.
        let reportAssemblyNotResolved(file, originalReferences:AssemblyReference list) = 
            originalReferences |> List.iter(fun originalReference -> errorR(AssemblyNotResolved(file, originalReference.Range)))
        knownUnresolved
        |> List.map (function UnresolvedAssemblyReference(file, originalReferences) -> file, originalReferences)
        |> List.iter reportAssemblyNotResolved
        
    // Note: This returns a TcImports object. TcImports are disposable - the caller owns the returned TcImports object 
    // and when hosted in Visual Studio or another long-running process must dispose this object. 
    static member BuildNonFrameworkTcImports (ctok, tcConfigP:TcConfigProvider, tcGlobals:TcGlobals, baseTcImports, nonFrameworkReferences, knownUnresolved) = 
      cancellable {
        let tcConfig = tcConfigP.Get(ctok)
        let tcResolutions = TcAssemblyResolutions.BuildFromPriorResolutions(ctok, tcConfig, nonFrameworkReferences, knownUnresolved)
        let references = tcResolutions.GetAssemblyResolutions()
        let tcImports = new TcImports(tcConfigP, tcResolutions, Some baseTcImports, Some tcGlobals.ilg)
        let! _assemblies = tcImports.RegisterAndImportReferencedAssemblies(ctok, references)
        tcImports.ReportUnresolvedAssemblyReferences(knownUnresolved)
        return tcImports
      }
      
    // Note: This returns a TcImports object. TcImports are disposable - the caller owns the returned TcImports object 
    // and if hosted in Visual Studio or another long-running process must dispose this object. However this
    // function is currently only used from fsi.exe. If we move to a long-running hosted evaluation service API then
    // we should start disposing these objects.
    static member BuildTcImports(ctok, tcConfigP:TcConfigProvider) = 
      cancellable {
        let tcConfig = tcConfigP.Get(ctok)
        //let foundationalTcImports, tcGlobals = TcImports.BuildFoundationalTcImports(tcConfigP)
        let frameworkDLLs, nonFrameworkReferences, knownUnresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(ctok, tcConfig)
        let! tcGlobals, frameworkTcImports = TcImports.BuildFrameworkTcImports (ctok, tcConfigP, frameworkDLLs, nonFrameworkReferences)
        let! tcImports = TcImports.BuildNonFrameworkTcImports(ctok, tcConfigP, tcGlobals, frameworkTcImports, nonFrameworkReferences, knownUnresolved)
        return tcGlobals, tcImports
      }
        
    interface System.IDisposable with 
        member tcImports.Dispose() = 
            CheckDisposed()
            // disposing deliberately only closes this tcImports, not the ones up the chain 
            disposed <- true        
            if verbose then 
                dprintf "disposing of TcImports, %d binaries\n" disposeActions.Length
            let actions = disposeActions
            disposeActions <- []
            for action in actions do action()

    override tcImports.ToString() = "TcImports(...)"
        
/// Process #r in F# Interactive.
/// Adds the reference to the tcImports and add the ccu to the type checking environment.
let RequireDLL (ctok, tcImports:TcImports, tcEnv, thisAssemblyName, m, file) = 
    let resolutions = CommitOperationResult(tcImports.TryResolveAssemblyReference(ctok, AssemblyReference(m, file, None), ResolveAssemblyReferenceMode.ReportErrors))
    let dllinfos, ccuinfos = tcImports.RegisterAndImportReferencedAssemblies(ctok, resolutions) |> Cancellable.runWithoutCancellation
   
    let asms = 
        ccuinfos |> List.map  (function
            | ResolvedImportedAssembly(asm) -> asm
            | UnresolvedImportedAssembly(assemblyName) -> error(Error(FSComp.SR.buildCouldNotResolveAssemblyRequiredByFile(assemblyName, file), m)))

    let g = tcImports.GetTcGlobals()
    let amap = tcImports.GetImportMap()
    let tcEnv = (tcEnv, asms) ||> List.fold (fun tcEnv asm -> AddCcuToTcEnv(g, amap, m, tcEnv, thisAssemblyName, asm.FSharpViewOfMetadata, asm.AssemblyAutoOpenAttributes, asm.AssemblyInternalsVisibleToAttributes)) 
    tcEnv, (dllinfos, asms)

       
       
let ProcessMetaCommandsFromInput 
     (nowarnF: 'state -> range * string -> 'state, 
      dllRequireF: 'state -> range * string -> 'state, 
      loadSourceF: 'state -> range * string -> unit) 
     (tcConfig:TcConfigBuilder, inp, pathOfMetaCommandSource, state0) =
     
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

    let canHaveScriptMetaCommands = 
        match inp with 
        | ParsedInput.SigFile(_) ->  false
        | ParsedInput.ImplFile(ParsedImplFileInput(isScript = isScript)) -> isScript

    let ProcessMetaCommand state hash  =
        let mutable matchedm = range0
        try 
            match hash with 
            | ParsedHashDirective("I", args, m) ->
               if not canHaveScriptMetaCommands then 
                   errorR(HashIncludeNotAllowedInNonScript(m))
               match args with 
               | [path] -> 
                   matchedm<-m
                   tcConfig.AddIncludePath(m, path, pathOfMetaCommandSource)
                   state
               | _ -> 
                   errorR(Error(FSComp.SR.buildInvalidHashIDirective(), m))
                   state
            | ParsedHashDirective("nowarn", numbers, m) ->
               List.fold (fun state d -> nowarnF state (m, d)) state numbers
            | ParsedHashDirective(("reference" | "r"), args, m) -> 
               if not canHaveScriptMetaCommands then 
                   errorR(HashReferenceNotAllowedInNonScript(m))
               match args with 
               | [path] -> 
                   matchedm<-m
                   dllRequireF state (m, path)
               | _ -> 
                   errorR(Error(FSComp.SR.buildInvalidHashrDirective(), m))
                   state
            | ParsedHashDirective("load", args, m) -> 
               if not canHaveScriptMetaCommands then 
                   errorR(HashDirectiveNotAllowedInNonScript(m))
               match args with 
               | _ :: _ -> 
                  matchedm<-m
                  args |> List.iter (fun path -> loadSourceF state (m, path))
               | _ -> 
                  errorR(Error(FSComp.SR.buildInvalidHashloadDirective(), m))
               state
            | ParsedHashDirective("time", args, m) -> 
               if not canHaveScriptMetaCommands then 
                   errorR(HashDirectiveNotAllowedInNonScript(m))
               match args with 
               | [] -> 
                   ()
               | ["on" | "off"] -> 
                   ()
               | _ -> 
                   errorR(Error(FSComp.SR.buildInvalidHashtimeDirective(), m))
               state
               
            | _ -> 
               
            (* warning(Error("This meta-command has been ignored", m)) *) 
               state
        with e -> errorRecovery e matchedm; state

    let rec WarnOnIgnoredSpecDecls decls = 
        decls |> List.iter (fun d -> 
            match d with 
            | SynModuleSigDecl.HashDirective (_, m) -> warning(Error(FSComp.SR.buildDirectivesInModulesAreIgnored(), m)) 
            | SynModuleSigDecl.NestedModule (_, _, subDecls, _) -> WarnOnIgnoredSpecDecls subDecls
            | _ -> ())

    let rec WarnOnIgnoredImplDecls decls = 
        decls |> List.iter (fun d -> 
            match d with 
            | SynModuleDecl.HashDirective (_, m) -> warning(Error(FSComp.SR.buildDirectivesInModulesAreIgnored(), m)) 
            | SynModuleDecl.NestedModule (_, _, subDecls, _, _) -> WarnOnIgnoredImplDecls subDecls
            | _ -> ())

    let ProcessMetaCommandsFromModuleSpec state (SynModuleOrNamespaceSig(_, _, _, decls, _, _, _, _)) =
        List.fold (fun s d -> 
            match d with 
            | SynModuleSigDecl.HashDirective (h, _) -> ProcessMetaCommand s h
            | SynModuleSigDecl.NestedModule (_, _, subDecls, _) -> WarnOnIgnoredSpecDecls subDecls; s
            | _ -> s)
         state
         decls 

    let ProcessMetaCommandsFromModuleImpl state (SynModuleOrNamespace(_, _, _, decls, _, _, _, _)) =
        List.fold (fun s d -> 
            match d with 
            | SynModuleDecl.HashDirective (h, _) -> ProcessMetaCommand s h
            | SynModuleDecl.NestedModule (_, _, subDecls, _, _) -> WarnOnIgnoredImplDecls subDecls; s
            | _ -> s)
         state
         decls

    match inp with 
    | ParsedInput.SigFile(ParsedSigFileInput(_, _, _, hashDirectives, specs)) -> 
        let state = List.fold ProcessMetaCommand state0 hashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleSpec state specs
        state
    | ParsedInput.ImplFile(ParsedImplFileInput(_, _, _, _, hashDirectives, impls, _)) -> 
        let state = List.fold ProcessMetaCommand state0 hashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleImpl state impls
        state

let ApplyNoWarnsToTcConfig (tcConfig:TcConfig, inp:ParsedInput, pathOfMetaCommandSource) = 
    // Clone
    let tcConfigB = tcConfig.CloneOfOriginalBuilder 
    let addNoWarn = fun () (m, s) -> tcConfigB.TurnWarningOff(m, s)
    let addReferencedAssemblyByPath = fun () (_m, _s) -> ()
    let addLoadedSource = fun () (_m, _s) -> ()
    ProcessMetaCommandsFromInput (addNoWarn, addReferencedAssemblyByPath, addLoadedSource) (tcConfigB, inp, pathOfMetaCommandSource, ())
    TcConfig.Create(tcConfigB, validate=false)

let ApplyMetaCommandsFromInputToTcConfig (tcConfig:TcConfig, inp:ParsedInput, pathOfMetaCommandSource) = 
    // Clone
    let tcConfigB = tcConfig.CloneOfOriginalBuilder 
    let getWarningNumber = fun () _ -> () 
    let addReferencedAssemblyByPath = fun () (m, s) -> tcConfigB.AddReferencedAssemblyByPath(m, s)
    let addLoadedSource = fun () (m, s) -> tcConfigB.AddLoadedSource(m, s, pathOfMetaCommandSource)
    ProcessMetaCommandsFromInput (getWarningNumber, addReferencedAssemblyByPath, addLoadedSource) (tcConfigB, inp, pathOfMetaCommandSource, ())
    TcConfig.Create(tcConfigB, validate=false)

//----------------------------------------------------------------------------
// Compute the load closure of a set of script files
//--------------------------------------------------------------------------

let GetAssemblyResolutionInformation(ctok, tcConfig : TcConfig) =
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parameter
    let assemblyList = TcAssemblyResolutions.GetAllDllReferences(tcConfig)
    let resolutions = TcAssemblyResolutions.ResolveAssemblyReferences (ctok, tcConfig, assemblyList, [])
    resolutions.GetAssemblyResolutions(), resolutions.GetUnresolvedReferences()
    

[<RequireQualifiedAccess>]
type LoadClosureInput = 
    { FileName: string
      SyntaxTree: ParsedInput option
      ParseDiagnostics: (PhasedDiagnostic * bool) list 
      MetaCommandDiagnostics: (PhasedDiagnostic * bool) list  }

[<RequireQualifiedAccess>]
type LoadClosure = 
    { /// The source files along with the ranges of the #load positions in each file.
      SourceFiles: (string * range list) list
      /// The resolved references along with the ranges of the #r positions in each file.
      References: (string * AssemblyResolution list) list
      /// The list of references that were not resolved during load closure. These may still be extension references.
      UnresolvedReferences : UnresolvedAssemblyReference list
      /// The list of all sources in the closure with inputs when available
      Inputs: LoadClosureInput list
      /// The #load, including those that didn't resolve
      OriginalLoadReferences: (range * string) list
      /// The #nowarns
      NoWarns: (string * range list) list
      /// Diagnostics seen while processing resolutions
      ResolutionDiagnostics : (PhasedDiagnostic * bool)  list
      /// Diagnostics seen while parsing root of closure
      AllRootFileDiagnostics : (PhasedDiagnostic * bool) list
      /// Diagnostics seen while processing the compiler options implied root of closure
      LoadClosureRootFileDiagnostics : (PhasedDiagnostic * bool) list }   


[<RequireQualifiedAccess>]
type CodeContext =
    | CompilationAndEvaluation // in fsi.exe
    | Compilation  // in fsc.exe
    | Editing // in VS
    

module private ScriptPreprocessClosure = 
    open Internal.Utilities.Text.Lexing
    
    /// Represents an input to the closure finding process
    type ClosureSource = ClosureSource of filename: string * referenceRange: range * sourceText: string * parseRequired: bool 
        
    /// Represents an output of the closure finding process
    type ClosureFile = ClosureFile  of string * range * ParsedInput option * (PhasedDiagnostic * bool) list * (PhasedDiagnostic * bool) list * (string * range) list // filename, range, errors, warnings, nowarns

    type Observed() =
        let seen = System.Collections.Generic.Dictionary<_, bool>()
        member ob.SetSeen(check) = 
            if not(seen.ContainsKey(check)) then 
                seen.Add(check, true)
        
        member ob.HaveSeen(check) =
            seen.ContainsKey(check)
    
    /// Parse a script from source.
    let ParseScriptText(filename:string, source:string, tcConfig:TcConfig, codeContext, lexResourceManager:Lexhelp.LexResourceManager, errorLogger:ErrorLogger) =    

        // fsc.exe -- COMPILED\!INTERACTIVE
        // fsi.exe -- !COMPILED\INTERACTIVE
        // Language service
        //     .fs -- EDITING + COMPILED\!INTERACTIVE
        //     .fsx -- EDITING + !COMPILED\INTERACTIVE    
        let defines =
            match codeContext with 
            | CodeContext.CompilationAndEvaluation -> ["INTERACTIVE"]
            | CodeContext.Compilation -> ["COMPILED"]
            | CodeContext.Editing -> "EDITING" :: (if IsScript filename then ["INTERACTIVE"] else ["COMPILED"])
        let lexbuf = UnicodeLexing.StringAsLexbuf source 
        
        let isLastCompiland = (IsScript filename), tcConfig.target.IsExe        // The root compiland is last in the list of compilands.
        ParseOneInputLexbuf (tcConfig, lexResourceManager, defines, lexbuf, filename, isLastCompiland, errorLogger) 
          
    /// Create a TcConfig for load closure starting from a single .fsx file
    let CreateScriptTextTcConfig (legacyReferenceResolver, defaultFSharpBinariesDir, filename:string, codeContext, useSimpleResolution, useFsiAuxLib, basicReferences, applyCommandLineArgs, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage) =  
        let projectDir = Path.GetDirectoryName(filename)
        let isInteractive = (codeContext = CodeContext.CompilationAndEvaluation)
        let isInvalidationSupported = (codeContext = CodeContext.Editing)
        let tcConfigB = TcConfigBuilder.CreateNew(legacyReferenceResolver, defaultFSharpBinariesDir, reduceMemoryUsage, projectDir, isInteractive, isInvalidationSupported, defaultCopyFSharpCore=CopyFSharpCoreFlag.No, tryGetMetadataSnapshot=tryGetMetadataSnapshot) 
        applyCommandLineArgs tcConfigB
        match basicReferences with 
        | None -> BasicReferencesForScriptLoadClosure(useFsiAuxLib, assumeDotNetFramework) |> List.iter(fun f->tcConfigB.AddReferencedAssemblyByPath(range0, f)) // Add script references
        | Some rs -> for m, r in rs do tcConfigB.AddReferencedAssemblyByPath(m, r)

        tcConfigB.resolutionEnvironment <-
            match codeContext with 
            | CodeContext.Editing -> ResolutionEnvironment.EditingOrCompilation true
            | CodeContext.Compilation -> ResolutionEnvironment.EditingOrCompilation false
            | CodeContext.CompilationAndEvaluation -> ResolutionEnvironment.CompilationAndEvaluation
        tcConfigB.framework <- false 
        tcConfigB.useSimpleResolution <- useSimpleResolution
        // Indicates that there are some references not in BasicReferencesForScriptLoadClosure which should
        // be added conditionally once the relevant version of mscorlib.dll has been detected.
        tcConfigB.implicitlyResolveAssemblies <- false
        TcConfig.Create(tcConfigB, validate=true)
        
    let ClosureSourceOfFilename(filename, m, inputCodePage, parseRequired) = 
        try
            let filename = FileSystem.GetFullPathShim(filename)
            use stream = FileSystem.FileStreamReadShim filename
            use reader = 
                match inputCodePage with 
                | None -> new  StreamReader(stream, true)
                | Some (n: int) -> new  StreamReader(stream, Encoding.GetEncoding(n)) 
            let source = reader.ReadToEnd()
            [ClosureSource(filename, m, source, parseRequired)]
        with e -> 
            errorRecovery e m 
            []
            
    let ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn (tcConfig:TcConfig, inp:ParsedInput, pathOfMetaCommandSource) = 
        let tcConfigB = tcConfig.CloneOfOriginalBuilder 
        let nowarns = ref [] 
        let getWarningNumber = fun () (m, s) -> nowarns := (s, m) :: !nowarns
        let addReferencedAssemblyByPath = fun () (m, s) -> tcConfigB.AddReferencedAssemblyByPath(m, s)
        let addLoadedSource = fun () (m, s) -> tcConfigB.AddLoadedSource(m, s, pathOfMetaCommandSource)
        try 
            ProcessMetaCommandsFromInput (getWarningNumber, addReferencedAssemblyByPath, addLoadedSource) (tcConfigB, inp, pathOfMetaCommandSource, ())
        with ReportedError _ ->
            // Recover by using whatever did end up in the tcConfig
            ()
            
        try
            TcConfig.Create(tcConfigB, validate=false), nowarns
        with ReportedError _ ->
            // Recover by  using a default TcConfig.
            let tcConfigB = tcConfig.CloneOfOriginalBuilder 
            TcConfig.Create(tcConfigB, validate=false), nowarns
    
    let FindClosureFiles(closureSources, tcConfig:TcConfig, codeContext, lexResourceManager:Lexhelp.LexResourceManager) =
        let tcConfig = ref tcConfig
        
        let observedSources = Observed()
        let rec loop (ClosureSource(filename, m, source, parseRequired)) = 
          [     if not (observedSources.HaveSeen(filename)) then
                    observedSources.SetSeen(filename)
                    //printfn "visiting %s" filename
                    if IsScript(filename) || parseRequired then 
                        let parseResult, parseDiagnostics =
                            let errorLogger = CapturingErrorLogger("FindClosureParse")                    
                            use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
                            let result = ParseScriptText (filename, source, !tcConfig, codeContext, lexResourceManager, errorLogger) 
                            result, errorLogger.Diagnostics

                        match parseResult with 
                        | Some parsedScriptAst ->                    
                            let errorLogger = CapturingErrorLogger("FindClosureMetaCommands")                    
                            use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
                            let pathOfMetaCommandSource = Path.GetDirectoryName(filename)
                            let preSources = (!tcConfig).GetAvailableLoadedSources()

                            let tcConfigResult, noWarns = ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn (!tcConfig, parsedScriptAst, pathOfMetaCommandSource)
                            tcConfig := tcConfigResult // We accumulate the tcConfig in order to collect assembly references
                        
                            let postSources = (!tcConfig).GetAvailableLoadedSources()
                            let sources = if preSources.Length < postSources.Length then postSources.[preSources.Length..] else []

                            //for (_, subFile) in sources do
                            //   printfn "visiting %s - has subsource of %s " filename subFile

                            for (m, subFile) in sources do
                                if IsScript(subFile) then 
                                    for subSource in ClosureSourceOfFilename(subFile, m, tcConfigResult.inputCodePage, false) do
                                        yield! loop subSource
                                else
                                    yield ClosureFile(subFile, m, None, [], [], []) 

                            //printfn "yielding source %s" filename
                            yield ClosureFile(filename, m, Some parsedScriptAst, parseDiagnostics, errorLogger.Diagnostics, !noWarns)

                        | None -> 
                            //printfn "yielding source %s (failed parse)" filename
                            yield ClosureFile(filename, m, None, parseDiagnostics, [], [])
                    else 
                        // Don't traverse into .fs leafs.
                        //printfn "yielding non-script source %s" filename
                        yield ClosureFile(filename, m, None, [], [], []) ]

        closureSources |> List.collect loop, !tcConfig
        
    /// Reduce the full directive closure into LoadClosure
    let GetLoadClosure(ctok, rootFilename, closureFiles, tcConfig:TcConfig, codeContext) = 
    
        // Mark the last file as isLastCompiland. 
        let closureFiles =
            if isNil closureFiles then  
                closureFiles 
            else 
                match List.frontAndBack closureFiles with
                | rest, ClosureFile(filename, m, Some(ParsedInput.ImplFile(ParsedImplFileInput(name, isScript, qualNameOfFile, scopedPragmas, hashDirectives, implFileFlags, _))), parseDiagnostics, metaDiagnostics, nowarns) -> 
                    rest @ [ClosureFile(filename, m, Some(ParsedInput.ImplFile(ParsedImplFileInput(name, isScript, qualNameOfFile, scopedPragmas, hashDirectives, implFileFlags, (true, tcConfig.target.IsExe)))), parseDiagnostics, metaDiagnostics, nowarns)]
                | _ -> closureFiles

        // Get all source files.
        let sourceFiles = [  for (ClosureFile(filename, m, _, _, _, _)) in closureFiles -> (filename, m) ]
        let sourceInputs = [  for (ClosureFile(filename, _, input, parseDiagnostics, metaDiagnostics, _nowarns)) in closureFiles -> ({ FileName=filename; SyntaxTree=input; ParseDiagnostics=parseDiagnostics; MetaCommandDiagnostics=metaDiagnostics }: LoadClosureInput)  ]
        let globalNoWarns = closureFiles |> List.collect (fun (ClosureFile(_, _, _, _, _, noWarns)) -> noWarns)

        // Resolve all references.
        let references, unresolvedReferences, resolutionDiagnostics = 
            let errorLogger = CapturingErrorLogger("GetLoadClosure") 
        
            use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
            let references, unresolvedReferences = GetAssemblyResolutionInformation(ctok, tcConfig)
            let references =  references |> List.map (fun ar -> ar.resolvedPath, ar)
            references, unresolvedReferences, errorLogger.Diagnostics

        // Root errors and warnings - look at the last item in the closureFiles list
        let loadClosureRootDiagnostics, allRootDiagnostics = 
            match List.rev closureFiles with
            | ClosureFile(_, _, _, parseDiagnostics, metaDiagnostics, _) :: _ -> 
                (metaDiagnostics @ resolutionDiagnostics), 
                (parseDiagnostics @ metaDiagnostics @ resolutionDiagnostics)
            | _ -> [], [] // When no file existed.
        
        let isRootRange exn =
            match GetRangeOfDiagnostic exn with
            | Some m -> 
                // Return true if the error was *not* from a #load-ed file.
                let isArgParameterWhileNotEditing = (codeContext <> CodeContext.Editing) && (m = range0 || m = rangeStartup || m = rangeCmdArgs)
                let isThisFileName = (0 = String.Compare(rootFilename, m.FileName, StringComparison.OrdinalIgnoreCase))
                isArgParameterWhileNotEditing || isThisFileName
            | None -> true
        
        // Filter out non-root errors and warnings
        let allRootDiagnostics = allRootDiagnostics |> List.filter (fst >> isRootRange)
        
        let result : LoadClosure = 
            { SourceFiles = List.groupBy fst sourceFiles |> List.map (map2Of2 (List.map snd))
              References = List.groupBy fst references  |> List.map (map2Of2 (List.map snd))
              UnresolvedReferences = unresolvedReferences
              Inputs = sourceInputs
              NoWarns = List.groupBy fst globalNoWarns  |> List.map (map2Of2 (List.map snd))
              OriginalLoadReferences = tcConfig.loadedSources
              ResolutionDiagnostics = resolutionDiagnostics
              AllRootFileDiagnostics = allRootDiagnostics
              LoadClosureRootFileDiagnostics = loadClosureRootDiagnostics }       

        result

    /// Given source text, find the full load closure. Used from service.fs, when editing a script file
    let GetFullClosureOfScriptText(ctok, legacyReferenceResolver, defaultFSharpBinariesDir, filename, source, codeContext, useSimpleResolution, useFsiAuxLib, lexResourceManager:Lexhelp.LexResourceManager, applyCommmandLineArgs, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage) = 
        // Resolve the basic references such as FSharp.Core.dll first, before processing any #I directives in the script
        //
        // This is tries to mimic the action of running the script in F# Interactive - the initial context for scripting is created
        // first, then #I and other directives are processed.
        let references0 = 
            let tcConfig = CreateScriptTextTcConfig(legacyReferenceResolver, defaultFSharpBinariesDir, filename, codeContext, useSimpleResolution, useFsiAuxLib, None, applyCommmandLineArgs, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage)
            let resolutions0, _unresolvedReferences = GetAssemblyResolutionInformation(ctok, tcConfig)
            let references0 =  resolutions0 |> List.map (fun r->r.originalReference.Range, r.resolvedPath) |> Seq.distinct |> List.ofSeq
            references0

        let tcConfig = CreateScriptTextTcConfig(legacyReferenceResolver, defaultFSharpBinariesDir, filename, codeContext, useSimpleResolution, useFsiAuxLib, Some references0, applyCommmandLineArgs, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage)

        let closureSources = [ClosureSource(filename, range0, source, true)]
        let closureFiles, tcConfig = FindClosureFiles(closureSources, tcConfig, codeContext, lexResourceManager)
        GetLoadClosure(ctok, filename, closureFiles, tcConfig, codeContext)
        
    /// Given source filename, find the full load closure
    /// Used from fsi.fs and fsc.fs, for #load and command line
    let GetFullClosureOfScriptFiles(ctok, tcConfig:TcConfig, files:(string*range) list, codeContext, lexResourceManager:Lexhelp.LexResourceManager) = 
        let mainFile = fst (List.last files)
        let closureSources = files |> List.collect (fun (filename, m) -> ClosureSourceOfFilename(filename, m, tcConfig.inputCodePage, true))
        let closureFiles, tcConfig = FindClosureFiles(closureSources, tcConfig, codeContext, lexResourceManager)
        GetLoadClosure(ctok, mainFile, closureFiles, tcConfig, codeContext)        

type LoadClosure with
    /// Analyze a script text and find the closure of its references. 
    /// Used from FCS, when editing a script file.  
    //
    /// A temporary TcConfig is created along the way, is why this routine takes so many arguments. We want to be sure to use exactly the
    /// same arguments as the rest of the application.
    static member ComputeClosureOfScriptText(ctok, legacyReferenceResolver, defaultFSharpBinariesDir, filename:string, source:string, codeContext, useSimpleResolution:bool, useFsiAuxLib, lexResourceManager:Lexhelp.LexResourceManager, applyCommmandLineArgs, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage) : LoadClosure = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        ScriptPreprocessClosure.GetFullClosureOfScriptText(ctok, legacyReferenceResolver, defaultFSharpBinariesDir, filename, source, codeContext, useSimpleResolution, useFsiAuxLib, lexResourceManager, applyCommmandLineArgs, assumeDotNetFramework, tryGetMetadataSnapshot, reduceMemoryUsage)

    /// Analyze a set of script files and find the closure of their references. The resulting references are then added to the given TcConfig.
    /// Used from fsi.fs and fsc.fs, for #load and command line. 
    static member ComputeClosureOfScriptFiles (ctok, tcConfig:TcConfig, files:(string*range) list, codeContext, lexResourceManager:Lexhelp.LexResourceManager) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        ScriptPreprocessClosure.GetFullClosureOfScriptFiles (ctok, tcConfig, files, codeContext, lexResourceManager)
        
              

//----------------------------------------------------------------------------
// Initial type checking environment
//--------------------------------------------------------------------------

/// Build the initial type checking environment
let GetInitialTcEnv (thisAssemblyName:string, initm:range, tcConfig:TcConfig, tcImports:TcImports, tcGlobals)  =    
    let initm = initm.StartRange

    let ccus = 
        tcImports.GetImportedAssemblies() 
        |> List.map (fun asm -> asm.FSharpViewOfMetadata, asm.AssemblyAutoOpenAttributes, asm.AssemblyInternalsVisibleToAttributes)    

    let amap = tcImports.GetImportMap()

    let tcEnv = CreateInitialTcEnv(tcGlobals, amap, initm, thisAssemblyName, ccus)

    if tcConfig.checkOverflow then
        try TcOpenDecl TcResultsSink.NoSink tcGlobals amap initm initm tcEnv (pathToSynLid initm (splitNamespace FSharpLib.CoreOperatorsCheckedName))
        with e -> errorRecovery e initm; tcEnv
    else
        tcEnv

//----------------------------------------------------------------------------
// Fault injection

/// Inject faults into checking
let CheckSimulateException(tcConfig:TcConfig) = 
    match tcConfig.simulateException with
    | Some("tc-oom") -> raise(System.OutOfMemoryException())
    | Some("tc-an") -> raise(System.ArgumentNullException("simulated"))
    | Some("tc-invop") -> raise(System.InvalidOperationException())
#if FX_REDUCED_EXCEPTIONS
#else
    | Some("tc-av") -> raise(System.AccessViolationException())
    | Some("tc-nfn") -> raise(System.NotFiniteNumberException())
#endif
    | Some("tc-aor") -> raise(System.ArgumentOutOfRangeException())
    | Some("tc-dv0") -> raise(System.DivideByZeroException())
    | Some("tc-oe") -> raise(System.OverflowException())
    | Some("tc-atmm") -> raise(System.ArrayTypeMismatchException())
    | Some("tc-bif") -> raise(System.BadImageFormatException())
    | Some("tc-knf") -> raise(System.Collections.Generic.KeyNotFoundException())
    | Some("tc-ior") -> raise(System.IndexOutOfRangeException())
    | Some("tc-ic") -> raise(System.InvalidCastException())
    | Some("tc-ip") -> raise(System.InvalidProgramException())
    | Some("tc-ma") -> raise(System.MemberAccessException())
    | Some("tc-ni") -> raise(System.NotImplementedException())
    | Some("tc-nr") -> raise(System.NullReferenceException())
    | Some("tc-oc") -> raise(System.OperationCanceledException())
    | Some("tc-fail") -> failwith "simulated"
    | _ -> ()

//----------------------------------------------------------------------------
// Type-check sets of files
//--------------------------------------------------------------------------

type RootSigs =  Zmap<QualifiedNameOfFile, ModuleOrNamespaceType>
type RootImpls = Zset<QualifiedNameOfFile >

let qnameOrder = Order.orderBy (fun (q:QualifiedNameOfFile) -> q.Text)

type TcState = 
    { tcsCcu: CcuThunk
      tcsCcuType: ModuleOrNamespace
      tcsNiceNameGen: NiceNameGenerator
      tcsTcSigEnv: TcEnv
      tcsTcImplEnv: TcEnv
      tcsCreatesGeneratedProvidedTypes: bool
      tcsRootSigs: RootSigs 
      tcsRootImpls: RootImpls 
      tcsCcuSig: ModuleOrNamespaceType  }

    member x.NiceNameGenerator = x.tcsNiceNameGen

    member x.TcEnvFromSignatures = x.tcsTcSigEnv

    member x.TcEnvFromImpls = x.tcsTcImplEnv

    member x.Ccu = x.tcsCcu

    member x.CreatesGeneratedProvidedTypes = x.tcsCreatesGeneratedProvidedTypes

    // Assem(a.fsi + b.fsi + c.fsi) (after checking implementation file )
    member x.CcuType = x.tcsCcuType
 
    // a.fsi + b.fsi + c.fsi (after checking implementation file for c.fs)
    member x.CcuSig = x.tcsCcuSig
 
    member x.NextStateAfterIncrementalFragment(tcEnvAtEndOfLastInput) = 
        { x with tcsTcSigEnv = tcEnvAtEndOfLastInput
                 tcsTcImplEnv = tcEnvAtEndOfLastInput } 

 
/// Create the initial type checking state for compiling an assembly
let GetInitialTcState(m, ccuName, tcConfig:TcConfig, tcGlobals, tcImports:TcImports, niceNameGen, tcEnv0) =
    ignore tcImports

    // Create a ccu to hold all the results of compilation 
    let ccuType = NewCcuContents ILScopeRef.Local m ccuName (NewEmptyModuleOrNamespaceType Namespace)

    let ccuData : CcuData = 
        { IsFSharp=true
          UsesFSharp20PlusQuotations=false
#if !NO_EXTENSIONTYPING
          InvalidateEvent=(new Event<_>()).Publish
          IsProviderGenerated = false
          ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
          TryGetILModuleDef = (fun () -> None)
          FileName=None 
          Stamp = newStamp()
          QualifiedName= None
          SourceCodeDirectory = tcConfig.implicitIncludeDir 
          ILScopeRef=ILScopeRef.Local
          Contents=ccuType
          MemberSignatureEquality= (Tastops.typeEquivAux EraseAll tcGlobals)
          TypeForwarders=Map.empty }

    let ccu = CcuThunk.Create(ccuName, ccuData)

    // OK, is this is the FSharp.Core CCU then fix it up. 
    if tcConfig.compilingFslib then 
        tcGlobals.fslibCcu.Fixup(ccu)

    { tcsCcu= ccu
      tcsCcuType=ccuType
      tcsNiceNameGen=niceNameGen
      tcsTcSigEnv=tcEnv0
      tcsTcImplEnv=tcEnv0
      tcsCreatesGeneratedProvidedTypes=false
      tcsRootSigs = Zmap.empty qnameOrder
      tcsRootImpls = Zset.empty qnameOrder
      tcsCcuSig = NewEmptyModuleOrNamespaceType Namespace }



/// Typecheck a single file (or interactive entry into F# Interactive)
let TypeCheckOneInputEventually (checkForErrors, tcConfig:TcConfig, tcImports:TcImports, tcGlobals, prefixPathOpt, tcSink, tcState: TcState, inp: ParsedInput) =

    eventually {
        try 
          let! ctok = Eventually.token
          RequireCompilationThread ctok // Everything here requires the compilation thread since it works on the TAST

          CheckSimulateException(tcConfig)

          let m = inp.Range
          let amap = tcImports.GetImportMap()
          match inp with 
          | ParsedInput.SigFile (ParsedSigFileInput(_, qualNameOfFile, _, _, _) as file) ->
                
              // Check if we've seen this top module signature before. 
              if Zmap.mem qualNameOfFile tcState.tcsRootSigs then 
                  errorR(Error(FSComp.SR.buildSignatureAlreadySpecified(qualNameOfFile.Text), m.StartRange))

              // Check if the implementation came first in compilation order 
              if Zset.contains qualNameOfFile tcState.tcsRootImpls then 
                  errorR(Error(FSComp.SR.buildImplementationAlreadyGivenDetail(qualNameOfFile.Text), m))

              // Typecheck the signature file 
              let! (tcEnv, sigFileType, createsGeneratedProvidedTypes) = 
                  TypeCheckOneSigFile (tcGlobals, tcState.tcsNiceNameGen, amap, tcState.tcsCcu, checkForErrors, tcConfig.conditionalCompilationDefines, tcSink) tcState.tcsTcSigEnv file

              let rootSigs = Zmap.add qualNameOfFile  sigFileType tcState.tcsRootSigs

              // Add the  signature to the signature env (unless it had an explicit signature)
              let ccuSigForFile = CombineCcuContentFragments m [sigFileType; tcState.tcsCcuSig]
                
              // Open the prefixPath for fsi.exe 
              let tcEnv = 
                  match prefixPathOpt with 
                  | None -> tcEnv 
                  | Some prefixPath -> 
                      let m = qualNameOfFile.Range
                      TcOpenDecl tcSink tcGlobals amap m m tcEnv prefixPath

              let tcState = 
                   { tcState with 
                        tcsTcSigEnv=tcEnv
                        tcsTcImplEnv=tcState.tcsTcImplEnv
                        tcsRootSigs=rootSigs
                        tcsCreatesGeneratedProvidedTypes=tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes}

              return (tcEnv, EmptyTopAttrs, None, ccuSigForFile), tcState

          | ParsedInput.ImplFile (ParsedImplFileInput(_, _, qualNameOfFile, _, _, _, _) as file) ->
            
              // Check if we've got an interface for this fragment 
              let rootSigOpt = tcState.tcsRootSigs.TryFind(qualNameOfFile)

              // Check if we've already seen an implementation for this fragment 
              if Zset.contains qualNameOfFile tcState.tcsRootImpls then 
                  errorR(Error(FSComp.SR.buildImplementationAlreadyGiven(qualNameOfFile.Text), m))

              let tcImplEnv = tcState.tcsTcImplEnv

              // Typecheck the implementation file 
              let! topAttrs, implFile, _implFileHiddenType, tcEnvAtEnd, createsGeneratedProvidedTypes = 
                  TypeCheckOneImplFile  (tcGlobals, tcState.tcsNiceNameGen, amap, tcState.tcsCcu, checkForErrors, tcConfig.conditionalCompilationDefines, tcSink) tcImplEnv rootSigOpt file

              let hadSig = rootSigOpt.IsSome
              let implFileSigType = SigTypeOfImplFile implFile

              let rootImpls = Zset.add qualNameOfFile tcState.tcsRootImpls
        
              // Only add it to the environment if it didn't have a signature 
              let m = qualNameOfFile.Range

              // Add the implementation as to the implementation env
              let tcImplEnv = AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcImplEnv implFileSigType

              // Add the implementation as to the signature env (unless it had an explicit signature)
              let tcSigEnv = 
                  if hadSig then tcState.tcsTcSigEnv 
                  else AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcState.tcsTcSigEnv implFileSigType
                
              // Open the prefixPath for fsi.exe (tcImplEnv)
              let tcImplEnv = 
                  match prefixPathOpt with 
                  | Some prefixPath -> TcOpenDecl tcSink tcGlobals amap m m tcImplEnv prefixPath
                  | _ -> tcImplEnv 

              // Open the prefixPath for fsi.exe (tcSigEnv)
              let tcSigEnv = 
                  match prefixPathOpt with 
                  | Some prefixPath when not hadSig -> TcOpenDecl tcSink tcGlobals amap m m tcSigEnv prefixPath
                  | _ -> tcSigEnv 

              let ccuSig = CombineCcuContentFragments m [implFileSigType; tcState.tcsCcuSig ]

              let ccuSigForFile = CombineCcuContentFragments m [implFileSigType; tcState.tcsCcuSig]

              let tcState = 
                   { tcState with 
                        tcsTcSigEnv=tcSigEnv
                        tcsTcImplEnv=tcImplEnv
                        tcsRootImpls=rootImpls
                        tcsCcuSig=ccuSig
                        tcsCreatesGeneratedProvidedTypes=tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes }
              return (tcEnvAtEnd, topAttrs, Some implFile, ccuSigForFile), tcState
     
        with e -> 
            errorRecovery e range0 
            return (tcState.TcEnvFromSignatures, EmptyTopAttrs, None, tcState.tcsCcuSig), tcState
    }

/// Typecheck a single file (or interactive entry into F# Interactive)
let TypeCheckOneInput (ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt) tcState  inp =
    // 'use' ensures that the warning handler is restored at the end
    use unwindEL = PushErrorLoggerPhaseUntilUnwind(fun oldLogger -> GetErrorLoggerFilteringByScopedPragmas(false, GetScopedPragmasForInput(inp), oldLogger) )
    use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck
    TypeCheckOneInputEventually (checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, TcResultsSink.NoSink, tcState, inp) 
        |> Eventually.force ctok

/// Finish checking multiple files (or one interactive entry into F# Interactive)
let TypeCheckMultipleInputsFinish(results, tcState: TcState) =
    let tcEnvsAtEndFile, topAttrs, implFiles, ccuSigsForFiles = List.unzip4 results
    let topAttrs = List.foldBack CombineTopAttrs topAttrs EmptyTopAttrs
    let implFiles = List.choose id implFiles
    // This is the environment required by fsi.exe when incrementally adding definitions 
    let tcEnvAtEndOfLastFile = (match tcEnvsAtEndFile with h :: _ -> h | _ -> tcState.TcEnvFromSignatures)
    (tcEnvAtEndOfLastFile, topAttrs, implFiles, ccuSigsForFiles), tcState

let TypeCheckOneInputAndFinishEventually(checkForErrors, tcConfig: TcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input) =
    eventually {
        Logger.LogBlockStart LogCompilerFunctionId.CompileOps_TypeCheckOneInputAndFinishEventually
        let! results, tcState =  TypeCheckOneInputEventually(checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input)
        let result = TypeCheckMultipleInputsFinish([results], tcState)
        Logger.LogBlockStop LogCompilerFunctionId.CompileOps_TypeCheckOneInputAndFinishEventually
        return result
    }

let TypeCheckClosedInputSetFinish (declaredImpls: TypedImplFile list, tcState) =
    // Publish the latest contents to the CCU 
    tcState.tcsCcu.Deref.Contents <- NewCcuContents ILScopeRef.Local range0 tcState.tcsCcu.AssemblyName tcState.tcsCcuSig

    // Check all interfaces have implementations 
    tcState.tcsRootSigs |> Zmap.iter (fun qualNameOfFile _ ->  
      if not (Zset.contains qualNameOfFile tcState.tcsRootImpls) then 
        errorR(Error(FSComp.SR.buildSignatureWithoutImplementation(qualNameOfFile.Text), qualNameOfFile.Range)))

    tcState, declaredImpls
    
let TypeCheckClosedInputSet (ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcState, inputs) =
    // tcEnvAtEndOfLastFile is the environment required by fsi.exe when incrementally adding definitions 
    let results, tcState =  (tcState, inputs) ||> List.mapFold (TypeCheckOneInput (ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt)) 
    let (tcEnvAtEndOfLastFile, topAttrs, implFiles, _), tcState = TypeCheckMultipleInputsFinish(results, tcState)
    let tcState, declaredImpls = TypeCheckClosedInputSetFinish (implFiles, tcState)
    tcState, topAttrs, declaredImpls, tcEnvAtEndOfLastFile
