// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Loading initial context, reporting errors etc.
module internal Microsoft.FSharp.Compiler.Build
open System
open System.Text
open System.IO
open System.Collections.Generic
open Internal.Utilities
open Internal.Utilities.Text
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.Pickle
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.SR
open Microsoft.FSharp.Compiler.DiagnosticMessage

module Tc = Microsoft.FSharp.Compiler.TypeChecker
module SR = Microsoft.FSharp.Compiler.SR

open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Tastops.DebugPrint
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.Lexhelp
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.ConstraintSolver
open Microsoft.FSharp.Compiler.MSBuildResolver
open Microsoft.FSharp.Compiler.Typrelns
open Microsoft.FSharp.Compiler.Nameres
open Microsoft.FSharp.Compiler.PrettyNaming
open Internal.Utilities.FileSystem
open Internal.Utilities.Collections
open Internal.Utilities.Filename
open Microsoft.FSharp.Compiler.Import

#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
open Microsoft.FSharp.Core.CompilerServices
#endif
open System.Runtime.CompilerServices

#if DEBUG

#if COMPILED_AS_LANGUAGE_SERVICE_DLL
module internal CompilerService =
#else
module internal FullCompiler =
#endif
    let showAssertForUnexpectedException = ref true
#if COMPILED_AS_LANGUAGE_SERVICE_DLL
open CompilerService
#else
open FullCompiler
#endif

#endif

//----------------------------------------------------------------------------
// Some Globals
//--------------------------------------------------------------------------

let sigSuffixes = [".mli";".fsi"]
let mlCompatSuffixes = [".mli";".ml"]
let implSuffixes = [".ml";".fs";".fsscript";".fsx"]
let resSuffixes = [".resx"]
let scriptSuffixes = [".fsscript";".fsx"]
let doNotRequireNamespaceOrModuleSuffixes = [".mli";".ml"] @ scriptSuffixes
let lightSyntaxDefaultExtensions : string list = [ ".fs";".fsscript";".fsx";".fsi" ]


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
exception InvalidInternalsVisibleToAssemblyName of (*badName*)string * (*fileName option*) string option


let RangeOfError(err:PhasedError) = 
  let rec RangeFromException = function
      | ErrorFromAddingConstraint(_,err2,_) -> RangeFromException err2 
#if EXTENSIONTYPING
      | ExtensionTyping.ProvidedTypeResolutionNoRange(e) -> RangeFromException e
      | ExtensionTyping.ProvidedTypeResolution(m,_)
#endif
      | ReservedKeyword(_,m)
      | IndentationProblem(_,m)
      | ErrorFromAddingTypeEquation(_,_,_,_,_,m) 
      | ErrorFromApplyingDefault(_,_,_,_,_,m) 
      | ErrorsFromAddingSubsumptionConstraint(_,_,_,_,_,m) 
      | FunctionExpected(_,_,m)
      | BakedInMemberConstraintName(_,m)
      | StandardOperatorRedefinitionWarning(_,m)
      | BadEventTransformation(m)
      | ParameterlessStructCtor(m)
      | FieldNotMutable (_,_,m) 
      | Recursion (_,_,_,_,m) 
      | InvalidRuntimeCoercion(_,_,_,m) 
      | IndeterminateRuntimeCoercion(_,_,_,m)
      | IndeterminateStaticCoercion (_,_,_,m)
      | StaticCoercionShouldUseBox (_,_,_,m)
      | CoercionTargetSealed(_,_,m)
      | UpcastUnnecessary(m)
      | QuotationTranslator.IgnoringPartOfQuotedTermWarning (_,m) 
      
      | TypeTestUnnecessary(m)
      | RuntimeCoercionSourceSealed(_,_,m)
      | OverrideDoesntOverride(_,_,_,_,_,m)
      | UnionPatternsBindDifferentNames m 
      | UnionCaseWrongArguments (_,_,_,m) 
      | TypeIsImplicitlyAbstract m 
      | RequiredButNotSpecified (_,_,_,_,m) 
      | FunctionValueUnexpected (_,_,m)
      | UnitTypeExpected (_,_,_,m )
      | UseOfAddressOfOperator m 
      | DeprecatedThreadStaticBindingWarning(m) 
      | NonUniqueInferredAbstractSlot (_,_,_,_,_,m) 
      | DefensiveCopyWarning (_,m)
      | LetRecCheckedAtRuntime m 
      | UpperCaseIdentifierInPattern m
      | NotUpperCaseConstructor m
      | RecursiveUseCheckedAtRuntime (_,_,m) 
      | LetRecEvaluatedOutOfOrder (_,_,_,m) 
      | Error (_,m)
      | NumberedError (_,m)
      | SyntaxError (_,m) 
      | InternalError (_,m)
      | FullAbstraction(_,m)
      | InterfaceNotRevealed(_,_,m) 
      | WrappedError (_,m)
      | Patcompile.MatchIncomplete (_,_,m)
      | Patcompile.RuleNeverMatched m 
      | ValNotMutable(_,_,m)
      | ValNotLocal(_,_,m) 
      | MissingFields(_,m) 
      | OverrideInIntrinsicAugmentation(m)
      | IntfImplInIntrinsicAugmentation(m) 
      | OverrideInExtrinsicAugmentation(m)
      | IntfImplInExtrinsicAugmentation(m) 
      | ValueRestriction(_,_,_,_,m) 
      | LetRecUnsound (_,_,m) 
      | ObsoleteError (_,m) 
      | ObsoleteWarning (_,m) 
      | Experimental (_,m) 
      | PossibleUnverifiableCode m
      | UserCompilerMessage (_,_,m) 
      | Deprecated(_,m) 
      | LibraryUseOnly(m) 
      | FieldsFromDifferentTypes (_,_,_,m) 
      | IndeterminateType(m)
      | TyconBadArgs(_,_,_,m) -> 
          Some m

      | FieldNotContained(_,arf,_,_) -> Some arf.Range
      | ValueNotContained(_,_,aval,_,_) -> Some aval.Range
      | ConstrNotContained(_,aval,_,_) -> Some aval.Id.idRange
      | ExnconstrNotContained(_,aexnc,_,_) -> Some aexnc.Range

      | VarBoundTwice(id) 
      | UndefinedName(_,_,id,_) -> 
          Some id.idRange 

      | Duplicate(_,_,m) 
      | NameClash(_,_,_,m,_,_,_) 
      | UnresolvedOverloading(_,_,_,m) 
      | UnresolvedConversionOperator (_,_,_,m)
      | PossibleOverload(_,_,_, m) 
      //| PossibleBestOverload(_,_,m) 
      | VirtualAugmentationOnNullValuedType(m)
      | NonVirtualAugmentationOnNullValuedType(m)
      | NonRigidTypar(_,_,_,_,_,m)
      | ConstraintSolverTupleDiffLengths(_,_,_,m,_) 
      | ConstraintSolverInfiniteTypes(_,_,_,m,_) 
      | ConstraintSolverMissingConstraint(_,_,_,m,_) 
      | ConstraintSolverTypesNotInEqualityRelation(_,_,_,m,_) 
      | ConstraintSolverError(_,m,_) 
      | ConstraintSolverTypesNotInSubsumptionRelation(_,_,_,m,_) 
      | ConstraintSolverRelatedInformation(_,m,_) 
      | SelfRefObjCtor(_,m) -> 
          Some m

      | NotAFunction(_,_,mfun,_) -> 
          Some mfun

      | IllegalFileNameChar(_) -> Some rangeCmdArgs

      | UnresolvedReferenceError(_,m) 
      | UnresolvedPathReference(_,_,m) 
      | DeprecatedCommandLineOptionFull(_,m) 
      | DeprecatedCommandLineOptionForHtmlDoc(_,m) 
      | DeprecatedCommandLineOptionSuggestAlternative(_,_,m) 
      | DeprecatedCommandLineOptionNoDescription(_,m) 
      | InternalCommandLineOption(_,m)
      | HashIncludeNotAllowedInNonScript(m)
      | HashReferenceNotAllowedInNonScript(m) 
      | HashDirectiveNotAllowedInNonScript(m)  
      | FileNameNotResolved(_,_,m) 
      | LoadedSourceNotFoundIgnoring(_,m) 
      | MSBuildReferenceResolutionWarning(_,_,m) 
      | MSBuildReferenceResolutionError(_,_,m) 
      | AssemblyNotResolved(_,m) 
      | HashLoadedSourceHasIssues(_,_,m) 
      | HashLoadedScriptConsideredSource(m) -> 
          Some m
      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          RangeFromException e.InnerException
#if EXTENSIONTYPING
      | :? TypeProviderError as e -> e.Range |> Some
#endif
      
      | _ -> None
  
  RangeFromException err.Exception

let GetErrorNumber(err:PhasedError) = 
   let rec GetFromException(e:exn) = 
      match e with
      (* DO NOT CHANGE THESE NUMBERS *)
      | ErrorFromAddingTypeEquation _ -> 1
      | FunctionExpected _ -> 2
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
      | RecursiveUseCheckedAtRuntime  _ -> 21
      | LetRecEvaluatedOutOfOrder  _ -> 22
      | NameClash _ -> 23
      // 24 cannot be reused
      | Patcompile.MatchIncomplete _ -> 25
      | Patcompile.RuleNeverMatched _ -> 26
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
      | UserCompilerMessage (_,n,_) -> n
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
#if EXTENSIONTYPING
      | ExtensionTyping.ProvidedTypeResolutionNoRange _
      | ExtensionTyping.ProvidedTypeResolution _ -> 103
#endif
       (* DO NOT CHANGE THE NUMBERS *)

      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          GetFromException e.InnerException
      
      | WrappedError(e,_) -> GetFromException e   

      | Error ((n,_),_) -> n
      | Failure _ -> 192
      | NumberedError((n,_),_) -> n
      | IllegalFileNameChar(fileName,invalidChar) -> fst (FSComp.SR.buildUnexpectedFileNameCharacter(fileName,string invalidChar))
#if EXTENSIONTYPING
      | :? TypeProviderError as e -> e.Number
#endif
      | _ -> 193
   GetFromException err.Exception
   
let GetWarningLevel err = 
  match err.Exception with 
  // Level 5 warnings
  | RecursiveUseCheckedAtRuntime _
  | LetRecEvaluatedOutOfOrder  _
  | DefensiveCopyWarning _
  | FullAbstraction _ ->  5
  | NumberedError((n,_),_) 
  | Error((n,_),_) -> 
      // 1178,tcNoComparisonNeeded1,"The struct, record or union type '%s' is not structurally comparable because the type parameter %s does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to this type to clarify that the type is not comparable"
      // 1178,tcNoComparisonNeeded2,"The struct, record or union type '%s' is not structurally comparable because the type '%s' does not satisfy the 'comparison' constraint. Consider adding the 'NoComparison' attribute to this type to clarify that the type is not comparable" 
      // 1178,tcNoEqualityNeeded1,"The struct, record or union type '%s' does not support structural equality because the type parameter %s does not satisfy the 'equality' constraint. Consider adding the 'NoEquality' attribute to this type to clarify that the type does not support structural equality"
      // 1178,tcNoEqualityNeeded2,"The struct, record or union type '%s' does not support structural equality because the type '%s' does not satisfy the 'equality' constraint. Consider adding the 'NoEquality' attribute to this type to clarify that the type does not support structural equality"
      if (n = 1178) then 5 else 2
  // Level 2 
  | _ ->  2

let warningOn err level specificWarnOn = 
    let n = GetErrorNumber err
    List.mem n specificWarnOn ||
    // Some specific warnings are never on by default, i.e. unused variable warnings
    match n with 
    | 1182 -> false // chkUnusedValue - off by default
    | 3180 -> false // abImplicitHeapAllocation - off by default
    | _ -> level >= GetWarningLevel err 

let SplitRelatedErrors(err:PhasedError) = 
    let ToPhased(e) = {Exception=e; Phase = err.Phase}
    let rec SplitRelatedException = function
      | UnresolvedOverloading(a,overloads,b,c) -> 
           let related = overloads |> List.map ToPhased
           UnresolvedOverloading(a,[],b,c)|>ToPhased, related
      | ConstraintSolverRelatedInformation(fopt,m2,e) -> 
          let e,related = SplitRelatedException e
          ConstraintSolverRelatedInformation(fopt,m2,e.Exception)|>ToPhased, related
      | ErrorFromAddingTypeEquation(g,denv,t1,t2,e,m) ->
          let e,related = SplitRelatedException e
          ErrorFromAddingTypeEquation(g,denv,t1,t2,e.Exception,m)|>ToPhased, related
      | ErrorFromApplyingDefault(g,denv,tp,defaultType,e,m) ->  
          let e,related = SplitRelatedException e
          ErrorFromApplyingDefault(g,denv,tp,defaultType,e.Exception,m)|>ToPhased, related
      | ErrorsFromAddingSubsumptionConstraint(g,denv,t1,t2,e,m) ->  
          let e,related = SplitRelatedException e
          ErrorsFromAddingSubsumptionConstraint(g,denv,t1,t2,e.Exception,m)|>ToPhased, related
      | ErrorFromAddingConstraint(x,e,m) ->  
          let e,related = SplitRelatedException e
          ErrorFromAddingConstraint(x,e.Exception,m)|>ToPhased, related
      | WrappedError (e,m) -> 
          let e,related = SplitRelatedException e
          WrappedError(e.Exception,m)|>ToPhased, related
      // Strip TargetInvocationException wrappers
      | :? System.Reflection.TargetInvocationException as e -> 
          SplitRelatedException e.InnerException
      | e -> 
           ToPhased(e), []
    SplitRelatedException(err.Exception)


let DeclareMesssage = Microsoft.FSharp.Compiler.DiagnosticMessage.DeclareResourceString

do FSComp.SR.RunStartupValidation()
let SeeAlsoE() = DeclareResourceString("SeeAlso","%s")
let ConstraintSolverTupleDiffLengthsE() = DeclareResourceString("ConstraintSolverTupleDiffLengths","%d%d")
let ConstraintSolverInfiniteTypesE() = DeclareResourceString("ConstraintSolverInfiniteTypes", "%s%s")
let ConstraintSolverMissingConstraintE() = DeclareResourceString("ConstraintSolverMissingConstraint","%s")
let ConstraintSolverTypesNotInEqualityRelation1E() = DeclareResourceString("ConstraintSolverTypesNotInEqualityRelation1","%s%s")
let ConstraintSolverTypesNotInEqualityRelation2E() = DeclareResourceString("ConstraintSolverTypesNotInEqualityRelation2", "%s%s")
let ConstraintSolverTypesNotInSubsumptionRelationE() = DeclareResourceString("ConstraintSolverTypesNotInSubsumptionRelation","%s%s%s")
let ConstraintSolverErrorE() = DeclareResourceString("ConstraintSolverError","%s")
let ErrorFromAddingTypeEquation1E() = DeclareResourceString("ErrorFromAddingTypeEquation1","%s%s%s")
let ErrorFromAddingTypeEquation2E() = DeclareResourceString("ErrorFromAddingTypeEquation2","%s%s%s")
let ErrorFromApplyingDefault1E() = DeclareResourceString("ErrorFromApplyingDefault1","%s")
let ErrorFromApplyingDefault2E() = DeclareResourceString("ErrorFromApplyingDefault2","")
let ErrorsFromAddingSubsumptionConstraintE() = DeclareResourceString("ErrorsFromAddingSubsumptionConstraint","%s%s%s")
let UpperCaseIdentifierInPatternE() = DeclareResourceString("UpperCaseIdentifierInPattern","")
let NotUpperCaseConstructorE() = DeclareResourceString("NotUpperCaseConstructor","")
let PossibleOverloadE() = DeclareResourceString("PossibleOverload","%s%s")
let FunctionExpectedE() = DeclareResourceString("FunctionExpected","")
let BakedInMemberConstraintNameE() = DeclareResourceString("BakedInMemberConstraintName","%s")
let BadEventTransformationE() = DeclareResourceString("BadEventTransformation","")
let ParameterlessStructCtorE() = DeclareResourceString("ParameterlessStructCtor","")
let InterfaceNotRevealedE() = DeclareResourceString("InterfaceNotRevealed","%s")
let NotAFunction1E() = DeclareResourceString("NotAFunction1","")
let NotAFunction2E() = DeclareResourceString("NotAFunction2","")
let TyconBadArgsE() = DeclareResourceString("TyconBadArgs","%s%d%d")
let IndeterminateTypeE() = DeclareResourceString("IndeterminateType","")
let NameClash1E() = DeclareResourceString("NameClash1","%s%s")
let NameClash2E() = DeclareResourceString("NameClash2","%s%s%s%s%s")
let Duplicate1E() = DeclareResourceString("Duplicate1","%s")
let Duplicate2E() = DeclareResourceString("Duplicate2","%s%s")
let UndefinedName2E() = DeclareResourceString("UndefinedName2","")
let FieldNotMutableE() = DeclareResourceString("FieldNotMutable","")
let FieldsFromDifferentTypesE() = DeclareResourceString("FieldsFromDifferentTypes","%s%s")
let VarBoundTwiceE() = DeclareResourceString("VarBoundTwice","%s")
let RecursionE() = DeclareResourceString("Recursion","%s%s%s%s")
let InvalidRuntimeCoercionE() = DeclareResourceString("InvalidRuntimeCoercion","%s%s%s")
let IndeterminateRuntimeCoercionE() = DeclareResourceString("IndeterminateRuntimeCoercion","%s%s")
let IndeterminateStaticCoercionE() = DeclareResourceString("IndeterminateStaticCoercion","%s%s")
let StaticCoercionShouldUseBoxE() = DeclareResourceString("StaticCoercionShouldUseBox","%s%s")
let TypeIsImplicitlyAbstractE() = DeclareResourceString("TypeIsImplicitlyAbstract","")
let NonRigidTypar1E() = DeclareResourceString("NonRigidTypar1","%s%s")
let NonRigidTypar2E() = DeclareResourceString("NonRigidTypar2","%s%s")
let NonRigidTypar3E() = DeclareResourceString("NonRigidTypar3","%s%s")
let OBlockEndSentenceE() = DeclareResourceString("BlockEndSentence","")
let UnexpectedEndOfInputE() = DeclareResourceString("UnexpectedEndOfInput","")
let UnexpectedE() = DeclareResourceString("Unexpected","%s")
let NONTERM_interactionE() = DeclareResourceString("NONTERM.interaction","")
let NONTERM_hashDirectiveE() = DeclareResourceString("NONTERM.hashDirective","")
let NONTERM_fieldDeclE() = DeclareResourceString("NONTERM.fieldDecl","")
let NONTERM_unionCaseReprE() = DeclareResourceString("NONTERM.unionCaseRepr","")
let NONTERM_localBindingE() = DeclareResourceString("NONTERM.localBinding","")
let NONTERM_hardwhiteLetBindingsE() = DeclareResourceString("NONTERM.hardwhiteLetBindings","")
let NONTERM_classDefnMemberE() = DeclareResourceString("NONTERM.classDefnMember","")
let NONTERM_defnBindingsE() = DeclareResourceString("NONTERM.defnBindings","")
let NONTERM_classMemberSpfnE() = DeclareResourceString("NONTERM.classMemberSpfn","")
let NONTERM_valSpfnE() = DeclareResourceString("NONTERM.valSpfn","")
let NONTERM_tyconSpfnE() = DeclareResourceString("NONTERM.tyconSpfn","")
let NONTERM_anonLambdaExprE() = DeclareResourceString("NONTERM.anonLambdaExpr","")
let NONTERM_attrUnionCaseDeclE() = DeclareResourceString("NONTERM.attrUnionCaseDecl","")
let NONTERM_cPrototypeE() = DeclareResourceString("NONTERM.cPrototype","")
let NONTERM_objectImplementationMembersE() = DeclareResourceString("NONTERM.objectImplementationMembers","")
let NONTERM_ifExprCasesE() = DeclareResourceString("NONTERM.ifExprCases","")
let NONTERM_openDeclE() = DeclareResourceString("NONTERM.openDecl","")
let NONTERM_fileModuleSpecE() = DeclareResourceString("NONTERM.fileModuleSpec","")
let NONTERM_patternClausesE() = DeclareResourceString("NONTERM.patternClauses","")
let NONTERM_beginEndExprE() = DeclareResourceString("NONTERM.beginEndExpr","")
let NONTERM_recdExprE() = DeclareResourceString("NONTERM.recdExpr","")
let NONTERM_tyconDefnE() = DeclareResourceString("NONTERM.tyconDefn","")
let NONTERM_exconCoreE() = DeclareResourceString("NONTERM.exconCore","")
let NONTERM_typeNameInfoE() = DeclareResourceString("NONTERM.typeNameInfo","")
let NONTERM_attributeListE() = DeclareResourceString("NONTERM.attributeList","")
let NONTERM_quoteExprE() = DeclareResourceString("NONTERM.quoteExpr","")
let NONTERM_typeConstraintE() = DeclareResourceString("NONTERM.typeConstraint","")
let NONTERM_Category_ImplementationFileE() = DeclareResourceString("NONTERM.Category.ImplementationFile","")
let NONTERM_Category_DefinitionE() = DeclareResourceString("NONTERM.Category.Definition","")
let NONTERM_Category_SignatureFileE() = DeclareResourceString("NONTERM.Category.SignatureFile","")
let NONTERM_Category_PatternE() = DeclareResourceString("NONTERM.Category.Pattern","")
let NONTERM_Category_ExprE() = DeclareResourceString("NONTERM.Category.Expr","")
let NONTERM_Category_TypeE() = DeclareResourceString("NONTERM.Category.Type","")
let NONTERM_typeArgsActualE() = DeclareResourceString("NONTERM.typeArgsActual","")
let TokenName1E() = DeclareResourceString("TokenName1","%s")
let TokenName1TokenName2E() = DeclareResourceString("TokenName1TokenName2","%s%s")
let TokenName1TokenName2TokenName3E() = DeclareResourceString("TokenName1TokenName2TokenName3","%s%s%s")
let RuntimeCoercionSourceSealed1E() = DeclareResourceString("RuntimeCoercionSourceSealed1","%s")
let RuntimeCoercionSourceSealed2E() = DeclareResourceString("RuntimeCoercionSourceSealed2","%s")
let CoercionTargetSealedE() = DeclareResourceString("CoercionTargetSealed","%s")
let UpcastUnnecessaryE() = DeclareResourceString("UpcastUnnecessary","")
let TypeTestUnnecessaryE() = DeclareResourceString("TypeTestUnnecessary","")
let OverrideDoesntOverride1E() = DeclareResourceString("OverrideDoesntOverride1","%s")
let OverrideDoesntOverride2E() = DeclareResourceString("OverrideDoesntOverride2","%s")
let OverrideDoesntOverride3E() = DeclareResourceString("OverrideDoesntOverride3","%s")
let UnionCaseWrongArgumentsE() = DeclareResourceString("UnionCaseWrongArguments","%d%d")
let UnionPatternsBindDifferentNamesE() = DeclareResourceString("UnionPatternsBindDifferentNames","")
let RequiredButNotSpecifiedE() = DeclareResourceString("RequiredButNotSpecified","%s%s%s")
let UseOfAddressOfOperatorE() = DeclareResourceString("UseOfAddressOfOperator","")
let DefensiveCopyWarningE() = DeclareResourceString("DefensiveCopyWarning","%s")
let DeprecatedThreadStaticBindingWarningE() = DeclareResourceString("DeprecatedThreadStaticBindingWarning","")
let FunctionValueUnexpectedE() = DeclareResourceString("FunctionValueUnexpected","%s")
let UnitTypeExpected1E() = DeclareResourceString("UnitTypeExpected1","%s")
let UnitTypeExpected2E() = DeclareResourceString("UnitTypeExpected2","%s")
let RecursiveUseCheckedAtRuntimeE() = DeclareResourceString("RecursiveUseCheckedAtRuntime","")
let LetRecUnsound1E() = DeclareResourceString("LetRecUnsound1","%s")
let LetRecUnsound2E() = DeclareResourceString("LetRecUnsound2","%s%s")
let LetRecUnsoundInnerE() = DeclareResourceString("LetRecUnsoundInner","%s")
let LetRecEvaluatedOutOfOrderE() = DeclareResourceString("LetRecEvaluatedOutOfOrder","")
let LetRecCheckedAtRuntimeE() = DeclareResourceString("LetRecCheckedAtRuntime","")
let SelfRefObjCtor1E() = DeclareResourceString("SelfRefObjCtor1","")
let SelfRefObjCtor2E() = DeclareResourceString("SelfRefObjCtor2","")
let VirtualAugmentationOnNullValuedTypeE() = DeclareResourceString("VirtualAugmentationOnNullValuedType","")
let NonVirtualAugmentationOnNullValuedTypeE() = DeclareResourceString("NonVirtualAugmentationOnNullValuedType","")
let NonUniqueInferredAbstractSlot1E() = DeclareResourceString("NonUniqueInferredAbstractSlot1","%s")
let NonUniqueInferredAbstractSlot2E() = DeclareResourceString("NonUniqueInferredAbstractSlot2","")
let NonUniqueInferredAbstractSlot3E() = DeclareResourceString("NonUniqueInferredAbstractSlot3","%s%s")
let NonUniqueInferredAbstractSlot4E() = DeclareResourceString("NonUniqueInferredAbstractSlot4","")
let Failure3E() = DeclareResourceString("Failure3","%s")
let Failure4E() = DeclareResourceString("Failure4","%s")
let FullAbstractionE() = DeclareResourceString("FullAbstraction","%s")
let MatchIncomplete1E() = DeclareResourceString("MatchIncomplete1","")
let MatchIncomplete2E() = DeclareResourceString("MatchIncomplete2","%s")
let MatchIncomplete3E() = DeclareResourceString("MatchIncomplete3","%s")
let MatchIncomplete4E() = DeclareResourceString("MatchIncomplete4","")
let RuleNeverMatchedE() = DeclareResourceString("RuleNeverMatched","")
let ValNotMutableE() = DeclareResourceString("ValNotMutable","")
let ValNotLocalE() = DeclareResourceString("ValNotLocal","")
let Obsolete1E() = DeclareResourceString("Obsolete1","")
let Obsolete2E() = DeclareResourceString("Obsolete2","%s")
let ExperimentalE() = DeclareResourceString("Experimental","%s")
let PossibleUnverifiableCodeE() = DeclareResourceString("PossibleUnverifiableCode","")
let DeprecatedE() = DeclareResourceString("Deprecated","%s")
let LibraryUseOnlyE() = DeclareResourceString("LibraryUseOnly","")
let MissingFieldsE() = DeclareResourceString("MissingFields","%s")
let ValueRestriction1E() = DeclareResourceString("ValueRestriction1","%s%s%s")
let ValueRestriction2E() = DeclareResourceString("ValueRestriction2","%s%s%s")
let ValueRestriction3E() = DeclareResourceString("ValueRestriction3","%s")
let ValueRestriction4E() = DeclareResourceString("ValueRestriction4","%s%s%s")
let ValueRestriction5E() = DeclareResourceString("ValueRestriction5","%s%s%s")
let RecoverableParseErrorE() = DeclareResourceString("RecoverableParseError","")
let ReservedKeywordE() = DeclareResourceString("ReservedKeyword","%s")
let IndentationProblemE() = DeclareResourceString("IndentationProblem","%s")
let OverrideInIntrinsicAugmentationE() = DeclareResourceString("OverrideInIntrinsicAugmentation","")
let OverrideInExtrinsicAugmentationE() = DeclareResourceString("OverrideInExtrinsicAugmentation","")
let IntfImplInIntrinsicAugmentationE() = DeclareResourceString("IntfImplInIntrinsicAugmentation","")
let IntfImplInExtrinsicAugmentationE() = DeclareResourceString("IntfImplInExtrinsicAugmentation","")
let UnresolvedReferenceNoRangeE() = DeclareResourceString("UnresolvedReferenceNoRange","%s")
let UnresolvedPathReferenceNoRangeE() = DeclareResourceString("UnresolvedPathReferenceNoRange","%s%s")
let HashIncludeNotAllowedInNonScriptE() = DeclareResourceString("HashIncludeNotAllowedInNonScript","")
let HashReferenceNotAllowedInNonScriptE() = DeclareResourceString("HashReferenceNotAllowedInNonScript","")
let HashDirectiveNotAllowedInNonScriptE() = DeclareResourceString("HashDirectiveNotAllowedInNonScript","")
let FileNameNotResolvedE() = DeclareResourceString("FileNameNotResolved","%s%s")
let AssemblyNotResolvedE() = DeclareResourceString("AssemblyNotResolved","%s")
let HashLoadedSourceHasIssues1E() = DeclareResourceString("HashLoadedSourceHasIssues1","")
let HashLoadedSourceHasIssues2E() = DeclareResourceString("HashLoadedSourceHasIssues2","")
let HashLoadedScriptConsideredSourceE() = DeclareResourceString("HashLoadedScriptConsideredSource","")  
let InvalidInternalsVisibleToAssemblyName1E() = DeclareResourceString("InvalidInternalsVisibleToAssemblyName1","%s%s")
let InvalidInternalsVisibleToAssemblyName2E() = DeclareResourceString("InvalidInternalsVisibleToAssemblyName2","%s")
let LoadedSourceNotFoundIgnoringE() = DeclareResourceString("LoadedSourceNotFoundIgnoring","%s")
let MSBuildReferenceResolutionErrorE() = DeclareResourceString("MSBuildReferenceResolutionError","%s%s")
let TargetInvocationExceptionWrapperE() = DeclareResourceString("TargetInvocationExceptionWrapper","%s")

let getErrorString key = SR.GetString key

let (|InvalidArgument|_|) (exn:exn) = match exn with :? ArgumentException as e -> Some e.Message | _ -> None

let OutputPhasedErrorR (os:System.Text.StringBuilder) (err:PhasedError) =
    let rec OutputExceptionR (os:System.Text.StringBuilder) = function        
      | ConstraintSolverTupleDiffLengths(_,tl1,tl2,m,m2) -> 
          os.Append(ConstraintSolverTupleDiffLengthsE().Format tl1.Length tl2.Length) |> ignore
          (if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore)
      | ConstraintSolverInfiniteTypes(denv,t1,t2,m,m2) ->
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ConstraintSolverInfiniteTypesE().Format t1 t2)  |> ignore
          (if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore )
      | ConstraintSolverMissingConstraint(denv,tpr,tpc,m,m2) -> 
          os.Append(ConstraintSolverMissingConstraintE().Format (NicePrint.stringOfTyparConstraint denv (tpr,tpc))) |> ignore
          (if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore )
      | ConstraintSolverTypesNotInEqualityRelation(denv,(TType_measure _ as t1),(TType_measure _ as t2),m,m2) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ConstraintSolverTypesNotInEqualityRelation1E().Format t1 t2)  |> ignore
          (if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m))  |> ignore)
      | ConstraintSolverTypesNotInEqualityRelation(denv,t1,t2,m,m2) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ConstraintSolverTypesNotInEqualityRelation2E().Format t1 t2)  |> ignore
          (if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m)) |> ignore)
      | ConstraintSolverTypesNotInSubsumptionRelation(denv,t1,t2,m,m2) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, cxs= NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ConstraintSolverTypesNotInSubsumptionRelationE().Format t2 t1 cxs) |> ignore
          (if m.StartLine <> m2.StartLine then 
             os.Append(SeeAlsoE().Format (stringOfRange m2)) |> ignore)
      | ConstraintSolverError(msg,m,m2) -> 
         os.Append(ConstraintSolverErrorE().Format msg) |> ignore
         if m.StartLine <> m2.StartLine then 
            os.Append(SeeAlsoE().Format (stringOfRange m2)) |> ignore
      | ConstraintSolverRelatedInformation(fopt,_,e) -> 
          match e with 
          | ConstraintSolverError _ -> OutputExceptionR os e
          | _ -> ()
          fopt |> Option.iter (Printf.bprintf os " %s")
      | ErrorFromAddingTypeEquation(g,denv,t1,t2,ConstraintSolverTypesNotInEqualityRelation(_, t1', t2',_ ,_ ),_) 
         when typeEquiv g t1 t1'
         &&   typeEquiv g t2 t2' ->  
          let t1,t2,tpcs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
          os.Append(ErrorFromAddingTypeEquation1E().Format t2 t1 tpcs) |> ignore
      | ErrorFromAddingTypeEquation(_,_,_,_,((ConstraintSolverTypesNotInSubsumptionRelation _ | ConstraintSolverError _) as e),_)  ->  
          OutputExceptionR os e
      | ErrorFromAddingTypeEquation(g,denv,t1,t2,e,_) ->
          if not (typeEquiv g t1 t2) then (
              let t1,t2,tpcs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
              if t1<>t2 + tpcs then os.Append(ErrorFromAddingTypeEquation2E().Format t1 t2 tpcs) |> ignore
          )
          OutputExceptionR os e
      | ErrorFromApplyingDefault(_,denv,_,defaultType,e,_) ->  
          let defaultType = NicePrint.minimalStringOfType denv defaultType
          os.Append(ErrorFromApplyingDefault1E().Format defaultType) |> ignore
          OutputExceptionR os e
          os.Append(ErrorFromApplyingDefault2E().Format) |> ignore
      | ErrorsFromAddingSubsumptionConstraint(g,denv,t1,t2,e,_) ->  
          if not (typeEquiv g t1 t2) then (
              let t1,t2,tpcs = NicePrint.minimalStringsOfTwoTypes denv t1 t2
              if t1 <> (t2 + tpcs) then 
                  os.Append(ErrorsFromAddingSubsumptionConstraintE().Format t2 t1 tpcs) |> ignore
          )
          OutputExceptionR os e
      | UpperCaseIdentifierInPattern(_) -> 
          os.Append(UpperCaseIdentifierInPatternE().Format) |> ignore
      | NotUpperCaseConstructor(_) -> 
          os.Append(NotUpperCaseConstructorE().Format) |> ignore
      | ErrorFromAddingConstraint(_,e,_) ->  
          OutputExceptionR os e
#if EXTENSIONTYPING
      | ExtensionTyping.ProvidedTypeResolutionNoRange(e)
      | ExtensionTyping.ProvidedTypeResolution(_,e) -> 
          OutputExceptionR os e
      | :? TypeProviderError as e ->
          os.Append(e.ContextualErrorMessage) |> ignore
#endif
      | UnresolvedOverloading(_,_,mtext,_) -> 
          os.Append(mtext) |> ignore
      | UnresolvedConversionOperator(denv,fromTy,toTy,_) -> 
          let t1,t2,_tpcs = NicePrint.minimalStringsOfTwoTypes denv fromTy toTy
          os.Append(FSComp.SR.csTypeDoesNotSupportConversion(t1,t2)) |> ignore
      | PossibleOverload(_,minfo, originalError, _) -> 
          // print original error that describes reason why this overload was rejected
          let buf = new StringBuilder()
          OutputExceptionR buf originalError

          os.Append(PossibleOverloadE().Format minfo (buf.ToString())) |> ignore
      //| PossibleBestOverload(_,minfo,m) -> 
      //    Printf.bprintf os "\n\nPossible best overload: '%s'." minfo
      | FunctionExpected _ ->
          os.Append(FunctionExpectedE().Format) |> ignore
      | BakedInMemberConstraintName(nm,_) ->
          os.Append(BakedInMemberConstraintNameE().Format nm) |> ignore
      | StandardOperatorRedefinitionWarning(msg,_) -> 
          os.Append(msg) |> ignore
      | BadEventTransformation(_) ->
         os.Append(BadEventTransformationE().Format) |> ignore
      | ParameterlessStructCtor(_) ->
         os.Append(ParameterlessStructCtorE().Format) |> ignore
      | InterfaceNotRevealed(denv,ity,_) ->
          os.Append(InterfaceNotRevealedE().Format (NicePrint.minimalStringOfType denv ity)) |> ignore
      | NotAFunction(_,_,_,marg) ->
          if marg.StartColumn = 0 then 
            os.Append(NotAFunction1E().Format) |> ignore
          else
            os.Append(NotAFunction2E().Format) |> ignore
          
      | TyconBadArgs(_,tcref,d,_) -> 
          let exp = tcref.TyparsNoRange.Length
          if exp = 0 then
              os.Append(FSComp.SR.buildUnexpectedTypeArgs(fullDisplayTextOfTyconRef tcref, d)) |> ignore
          else
              os.Append(TyconBadArgsE().Format (fullDisplayTextOfTyconRef tcref) exp d) |> ignore
      | IndeterminateType(_) -> 
          os.Append(IndeterminateTypeE().Format) |> ignore
      | NameClash(nm,k1,nm1,_,k2,nm2,_) -> 
          if nm = nm1 && nm1 = nm2 && k1 = k2 then 
              os.Append(NameClash1E().Format k1 nm1) |> ignore
          else
              os.Append(NameClash2E().Format k1 nm1 nm k2 nm2) |> ignore
      | Duplicate(k,s,_)  -> 
          if k = "member" then 
              os.Append(Duplicate1E().Format (DecompileOpName s)) |> ignore
          else 
              os.Append(Duplicate2E().Format k (DecompileOpName s)) |> ignore
      | UndefinedName(_,k,id,_) -> 
          os.Append(k (DecompileOpName id.idText)) |> ignore
                
      | InternalUndefinedTyconItem(f,tcref,s) ->
          let _, errs = f((fullDisplayTextOfTyconRef tcref), s)
          os.Append(errs) |> ignore
      | InternalUndefinedItemRef(f,smr,ccuName,s) ->
          let _, errs = f(smr, ccuName, s)
          os.Append(errs) |> ignore
      | FieldNotMutable  _ -> 
          os.Append(FieldNotMutableE().Format) |> ignore
      | FieldsFromDifferentTypes (_,fref1,fref2,_) -> 
          os.Append(FieldsFromDifferentTypesE().Format fref1.FieldName fref2.FieldName) |> ignore
      | VarBoundTwice(id) ->  
          os.Append(VarBoundTwiceE().Format (DecompileOpName id.idText)) |> ignore
      | Recursion (denv,id,ty1,ty2,_) -> 
          let t1,t2,tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(RecursionE().Format (DecompileOpName id.idText) t1 t2 tpcs) |> ignore
      | InvalidRuntimeCoercion(denv,ty1,ty2,_) -> 
          let t1,t2,tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(InvalidRuntimeCoercionE().Format t1 t2 tpcs) |> ignore
      | IndeterminateRuntimeCoercion(denv,ty1,ty2,_) -> 
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(IndeterminateRuntimeCoercionE().Format t1 t2) |> ignore
      | IndeterminateStaticCoercion(denv,ty1,ty2,_) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(IndeterminateStaticCoercionE().Format t1 t2) |> ignore
      | StaticCoercionShouldUseBox(denv,ty1,ty2,_) ->
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(StaticCoercionShouldUseBoxE().Format t1 t2) |> ignore
      | TypeIsImplicitlyAbstract(_) -> 
          os.Append(TypeIsImplicitlyAbstractE().Format) |> ignore
      | NonRigidTypar(denv,tpnmOpt,typarRange,ty1,ty,_) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let _, (ty1,ty), _cxs = PrettyTypes.PrettifyTypes2 denv.g (ty1,ty)
          match tpnmOpt with 
          | None -> 
              os.Append(NonRigidTypar1E().Format (stringOfRange typarRange) (NicePrint.stringOfTy denv ty)) |> ignore
          | Some tpnm -> 
              match ty1 with 
              | TType_measure _ -> 
                os.Append(NonRigidTypar2E().Format tpnm  (NicePrint.stringOfTy denv ty)) |> ignore
              | _ -> 
                os.Append(NonRigidTypar3E().Format tpnm  (NicePrint.stringOfTy denv ty)) |> ignore
      | SyntaxError (ctxt,_) -> 
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
              | unknown ->           
                  System.Diagnostics.Debug.Assert(false,"unknown token tag")
                  let result = sprintf "%+A" unknown
                  System.Diagnostics.Debug.Assert(false, result)
                  result

          match ctxt.CurrentToken with 
          | None -> os.Append(UnexpectedEndOfInputE().Format) |> ignore
          | Some token -> 
              match (token |> Parser.tagOfToken |> Parser.tokenTagToTokenId), token with 
              | EndOfStructuredConstructToken,_ -> os.Append(OBlockEndSentenceE().Format) |> ignore
              | Parser.TOKEN_LEX_FAILURE, Parser.LEX_FAILURE str -> Printf.bprintf os "%s" str (* Fix bug://2431 *)
              | token,_ -> os.Append(UnexpectedE().Format (token |> tokenIdToText)) |> ignore

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
                  Printf.bprintf os ". (no 'in' context found: %+A)" (List.map (List.map Parser.prodIdxToNonTerminal) ctxt.ReducibleProductions);
        #else
              foundInContext |> ignore // suppress unused variable warning in RELEASE
        #endif
              let fix (s:string) = s.Replace(SR.GetString("FixKeyword"),"").Replace(SR.GetString("FixSymbol"),"").Replace(SR.GetString("FixReplace"),"")
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
      | RuntimeCoercionSourceSealed(denv,ty,_) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let _, ty, _cxs = PrettyTypes.PrettifyTypes1 denv.g ty
          if isTyparTy denv.g ty 
          then os.Append(RuntimeCoercionSourceSealed1E().Format (NicePrint.stringOfTy denv ty)) |> ignore
          else os.Append(RuntimeCoercionSourceSealed2E().Format (NicePrint.stringOfTy denv ty)) |> ignore
      | CoercionTargetSealed(denv,ty,_) -> 
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let _, ty, _cxs= PrettyTypes.PrettifyTypes1 denv.g ty
          os.Append(CoercionTargetSealedE().Format (NicePrint.stringOfTy denv ty)) |> ignore
      | UpcastUnnecessary(_) -> 
          os.Append(UpcastUnnecessaryE().Format) |> ignore
      | TypeTestUnnecessary(_) -> 
          os.Append(TypeTestUnnecessaryE().Format) |> ignore
      | QuotationTranslator.IgnoringPartOfQuotedTermWarning (msg,_) -> 
          Printf.bprintf os "%s" msg
      | OverrideDoesntOverride(denv,impl,minfoVirtOpt,g,amap,m) ->
          let sig1 = DispatchSlotChecking.FormatOverride denv impl
          begin match minfoVirtOpt with 
          | None -> 
              os.Append(OverrideDoesntOverride1E().Format sig1) |> ignore
          | Some minfoVirt -> 
              os.Append(OverrideDoesntOverride2E().Format sig1) |> ignore
              let sig2 = DispatchSlotChecking.FormatMethInfoSig g amap m denv minfoVirt
              if sig1 <> sig2 then 
                  os.Append(OverrideDoesntOverride3E().Format  sig2) |> ignore
          end
      | UnionCaseWrongArguments (_,n1,n2,_) ->
          os.Append(UnionCaseWrongArgumentsE().Format n2 n1) |> ignore
      | UnionPatternsBindDifferentNames _ -> 
          os.Append(UnionPatternsBindDifferentNamesE().Format) |> ignore
      | ValueNotContained (denv,mref,implVal,sigVal,f) ->
          let text1,text2 = NicePrint.minimalStringsOfTwoValues denv implVal sigVal
          os.Append(f((fullDisplayTextOfModRef mref), text1, text2)) |> ignore
      | ConstrNotContained (denv,v1,v2,f) ->
          os.Append(f((NicePrint.stringOfUnionCase denv v1), (NicePrint.stringOfUnionCase denv v2))) |> ignore
      | ExnconstrNotContained (denv,v1,v2,f) ->
          os.Append(f((NicePrint.stringOfExnDef denv v1), (NicePrint.stringOfExnDef denv v2))) |> ignore
      | FieldNotContained (denv,v1,v2,f) ->
          os.Append(f((NicePrint.stringOfRecdField denv v1), (NicePrint.stringOfRecdField denv v2))) |> ignore
      | RequiredButNotSpecified (_,mref,k,name,_) ->
          let nsb = new System.Text.StringBuilder()
          name nsb;
          os.Append(RequiredButNotSpecifiedE().Format (fullDisplayTextOfModRef mref) k (nsb.ToString())) |> ignore
      | UseOfAddressOfOperator _ -> 
          os.Append(UseOfAddressOfOperatorE().Format) |> ignore
      | DefensiveCopyWarning(s,_) -> os.Append(DefensiveCopyWarningE().Format s) |> ignore
      | DeprecatedThreadStaticBindingWarning(_) -> 
          os.Append(DeprecatedThreadStaticBindingWarningE().Format) |> ignore
      | FunctionValueUnexpected (denv,ty,_) ->
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let _, ty, _cxs = PrettyTypes.PrettifyTypes1 denv.g ty
          os.Append(FunctionValueUnexpectedE().Format (NicePrint.stringOfTy denv ty)) |> ignore
      | UnitTypeExpected (denv,ty,perhapsProp,_) ->
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let _, ty, _cxs = PrettyTypes.PrettifyTypes1 denv.g ty
          if perhapsProp then 
            os.Append(UnitTypeExpected2E().Format (NicePrint.stringOfTy denv ty)) |> ignore
          else
            os.Append(UnitTypeExpected1E().Format (NicePrint.stringOfTy denv ty)) |> ignore
      | RecursiveUseCheckedAtRuntime  _ -> 
          os.Append(RecursiveUseCheckedAtRuntimeE().Format) |> ignore
      | LetRecUnsound (_,[v],_) ->  
          os.Append(LetRecUnsound1E().Format v.DisplayName) |> ignore
      | LetRecUnsound (_,path,_) -> 
          let bos = new System.Text.StringBuilder()
          (path.Tail @ [path.Head]) |> List.iter (fun (v:ValRef) -> bos.Append(LetRecUnsoundInnerE().Format v.DisplayName) |> ignore) 
          os.Append(LetRecUnsound2E().Format (List.head path).DisplayName (bos.ToString())) |> ignore
      | LetRecEvaluatedOutOfOrder (_,_,_,_) -> 
          os.Append(LetRecEvaluatedOutOfOrderE().Format) |> ignore
      | LetRecCheckedAtRuntime _ -> 
          os.Append(LetRecCheckedAtRuntimeE().Format) |> ignore
      | SelfRefObjCtor(false,_) -> 
          os.Append(SelfRefObjCtor1E().Format) |> ignore
      | SelfRefObjCtor(true,_) -> 
          os.Append(SelfRefObjCtor2E().Format) |> ignore
      | VirtualAugmentationOnNullValuedType(_) ->
          os.Append(VirtualAugmentationOnNullValuedTypeE().Format) |> ignore
      | NonVirtualAugmentationOnNullValuedType(_) ->
          os.Append(NonVirtualAugmentationOnNullValuedTypeE().Format) |> ignore
      | NonUniqueInferredAbstractSlot(_,denv,bindnm,bvirt1,bvirt2,_) ->
          os.Append(NonUniqueInferredAbstractSlot1E().Format bindnm) |> ignore
          let ty1 = bvirt1.EnclosingType
          let ty2 = bvirt2.EnclosingType
          // REVIEW: consider if we need to show _cxs (the type parameter constrants)
          let t1, t2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
          os.Append(NonUniqueInferredAbstractSlot2E().Format) |> ignore
          if t1 <> t2 then 
              os.Append(NonUniqueInferredAbstractSlot3E().Format t1 t2) |> ignore
          os.Append(NonUniqueInferredAbstractSlot4E().Format) |> ignore
      | Error ((_,s),_) -> os.Append(s) |> ignore
      | NumberedError ((_,s),_) -> os.Append(s) |> ignore
      | InternalError (s,_) 
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
              System.Diagnostics.Debug.Assert(false,sprintf "Bug seen in compiler: %s" (exn.ToString()))
    #endif
      | FullAbstraction(s,_) -> os.Append(FullAbstractionE().Format s) |> ignore
      | WrappedError (exn,_) -> OutputExceptionR os exn
      | Patcompile.MatchIncomplete (isComp,cexOpt,_) -> 
          os.Append(MatchIncomplete1E().Format) |> ignore
          match cexOpt with 
          | None -> ()
          | Some (cex,false) ->  os.Append(MatchIncomplete2E().Format cex) |> ignore
          | Some (cex,true) ->  os.Append(MatchIncomplete3E().Format cex) |> ignore
          if isComp then 
              os.Append(MatchIncomplete4E().Format) |> ignore
      | Patcompile.RuleNeverMatched _ -> os.Append(RuleNeverMatchedE().Format) |> ignore
      | ValNotMutable _ -> os.Append(ValNotMutableE().Format) |> ignore
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
      | MissingFields(sl,_) -> os.Append(MissingFieldsE().Format (String.concat "," sl + ".")) |> ignore
      | ValueRestriction(denv,hassig,v,_,_) -> 
          let denv = { denv with showImperativeTyparAnnotations=true; }
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
      | ReservedKeyword (s,_) -> os.Append(ReservedKeywordE().Format s) |> ignore
      | IndentationProblem (s,_) -> os.Append(IndentationProblemE().Format s) |> ignore
      | OverrideInIntrinsicAugmentation(_) -> os.Append(OverrideInIntrinsicAugmentationE().Format) |> ignore
      | OverrideInExtrinsicAugmentation(_) -> os.Append(OverrideInExtrinsicAugmentationE().Format) |> ignore
      | IntfImplInIntrinsicAugmentation(_) -> os.Append(IntfImplInIntrinsicAugmentationE().Format) |> ignore
      | IntfImplInExtrinsicAugmentation(_) -> os.Append(IntfImplInExtrinsicAugmentationE().Format) |> ignore
      | UnresolvedReferenceError(assemblyname,_)
      | UnresolvedReferenceNoRange(assemblyname) ->
          os.Append(UnresolvedReferenceNoRangeE().Format assemblyname) |> ignore
      | UnresolvedPathReference(assemblyname,pathname,_) 
      | UnresolvedPathReferenceNoRange(assemblyname,pathname) ->
          os.Append(UnresolvedPathReferenceNoRangeE().Format pathname assemblyname) |> ignore
      | DeprecatedCommandLineOptionFull(fullText,_) ->
          os.Append(fullText) |> ignore
      | DeprecatedCommandLineOptionForHtmlDoc(optionName,_) ->
          os.Append(FSComp.SR.optsDCLOHtmlDoc(optionName)) |> ignore
      | DeprecatedCommandLineOptionSuggestAlternative(optionName,altOption,_) ->
          os.Append(FSComp.SR.optsDCLODeprecatedSuggestAlternative(optionName, altOption)) |> ignore
      | InternalCommandLineOption(optionName,_) ->
          os.Append(FSComp.SR.optsInternalNoDescription(optionName)) |> ignore
      | DeprecatedCommandLineOptionNoDescription(optionName,_) ->
          os.Append(FSComp.SR.optsDCLONoDescription(optionName)) |> ignore
      | HashIncludeNotAllowedInNonScript(_) ->
          os.Append(HashIncludeNotAllowedInNonScriptE().Format) |> ignore
      | HashReferenceNotAllowedInNonScript(_) ->
          os.Append(HashReferenceNotAllowedInNonScriptE().Format) |> ignore
      | HashDirectiveNotAllowedInNonScript(_) ->
          os.Append(HashDirectiveNotAllowedInNonScriptE().Format) |> ignore
      | FileNameNotResolved(filename,locations,_) -> 
          os.Append(FileNameNotResolvedE().Format filename locations) |> ignore
      | AssemblyNotResolved(originalName,_) ->
          os.Append(AssemblyNotResolvedE().Format originalName) |> ignore
      | IllegalFileNameChar(fileName,invalidChar) ->
          os.Append(FSComp.SR.buildUnexpectedFileNameCharacter(fileName,string invalidChar)|>snd) |> ignore
      | HashLoadedSourceHasIssues(warnings,errors,_) -> 
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
      | InvalidInternalsVisibleToAssemblyName(badName,fileNameOption) ->      
          match fileNameOption with      
          | Some file -> os.Append(InvalidInternalsVisibleToAssemblyName1E().Format badName file) |> ignore
          | None      -> os.Append(InvalidInternalsVisibleToAssemblyName2E().Format badName) |> ignore
      | LoadedSourceNotFoundIgnoring(filename,_) ->
          os.Append(LoadedSourceNotFoundIgnoringE().Format filename) |> ignore
      | MSBuildReferenceResolutionWarning(code,message,_) 
      | MSBuildReferenceResolutionError(code,message,_) -> 
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
              System.Diagnostics.Debug.Assert(false,sprintf "Bug seen in compiler: %s" (e.ToString()))
    #endif
    OutputExceptionR os (err.Exception)


// remove any newlines and tabs 
let OutputPhasedError (os:System.Text.StringBuilder) (err:PhasedError) (flattenErrors:bool) = 
    let buf = new System.Text.StringBuilder()

    OutputPhasedErrorR buf err
    let s = if flattenErrors then ErrorLogger.NormalizeErrorString (buf.ToString()) else buf.ToString()
    
    os.Append(s) |> ignore


type ErrorStyle = 
    | DefaultErrors 
    | EmacsErrors 
    | TestErrors 
    | VSErrors

let SanitizeFileName fileName implicitIncludeDir =
    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy filename
    //  - if you have a #line directive, e.g. 
    //        # 1000 "Line01.fs"
    //    then it also asserts.  But these are edge cases that can be fixed later, e.g. in bug 4651.
    //System.Diagnostics.Debug.Assert(System.IO.Path.IsPathRooted(fileName), sprintf "filename should be absolute: '%s'" fileName)
    try
        let fullPath = FileSystem.GetFullPathShim(fileName)
        let currentDir = implicitIncludeDir
        
        // if the file name is not rooted in the current directory, return the full path
        if not(fullPath.StartsWith(currentDir)) then
            fullPath
        // if the file name is rooted in the current directory, return the relative path
        else
            fullPath.Replace(currentDir+"\\","")
    with _ ->
        fileName

type ErrorLocation =
    {
        Range : range
        File : string
        TextRepresentation : string
        IsEmpty : bool
    }

type CanonicalInformation = 
    {
        ErrorNumber : int
        Subcategory : string
        TextRepresentation : string
    }

type DetailedIssueInfo = 
    {
        Location : ErrorLocation option
        Canonical : CanonicalInformation
        Message : string
    }

type ErrorOrWarning = 
    | Short of bool * string
    | Long of bool * DetailedIssueInfo

/// returns sequence that contains ErrorOrWarning for the given error + ErrorOrWarning for all related errors
let CollectErrorOrWarning (implicitIncludeDir,showFullPaths,flattenErrors,errorStyle,warn, err:PhasedError) = 
    let outputWhere (showFullPaths,errorStyle) m = 
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
                    let file = file.Replace("\\","/")
                    (sprintf "File \"%s\", line %d, characters %d-%d: " file m.StartLine m.StartColumn m.EndColumn), m, file

                  // We're adjusting the columns here to be 1-based - both for parity with C# and for MSBuild, which assumes 1-based columns for error output
                  | ErrorStyle.DefaultErrors -> 
                    let file = file.Replace('/',System.IO.Path.DirectorySeparatorChar)
                    let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) m.End
                    (sprintf "%s(%d,%d): " file m.StartLine m.StartColumn), m, file

                  // We may also want to change TestErrors to be 1-based
                  | ErrorStyle.TestErrors    -> 
                    let file = file.Replace("/","\\")
                    let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1) )
                    sprintf "%s(%d,%d-%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file

                  // Here, we want the complete range information so Project Systems can generate proper squiggles
                  | ErrorStyle.VSErrors      -> 
                        // Show prefix only for real files. Otherise, we just want a truncated error like:
                        //      parse error FS0031 : blah blah
                        if m<>range0 && m<>rangeStartup && m<>rangeCmdArgs then 
                            let file = file.Replace("/","\\")
                            let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1) )
                            sprintf "%s(%d,%d,%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file
                        else
                            "", m, file
            { Range = m; TextRepresentation = text; IsEmpty = false; File = file }

    match err.Exception with 
    | ReportedError _ -> 
        dprintf "Unexpected ReportedError"  (* this should actually never happen *)
        Seq.empty
    | StopProcessing -> 
        dprintf "Unexpected StopProcessing"  (* this should actually never happen *)
        Seq.empty
    | _ -> 
        let errors = ResizeArray()
        let report err =
            let OutputWhere(err) = 
                match RangeOfError err with 
                | Some m -> Some(outputWhere (showFullPaths,errorStyle) m)
                | None -> None

            let OutputCanonicalInformation(err:PhasedError,subcategory, errorNumber) = 
                let text = 
                    match errorStyle with
                    // Show the subcategory for --vserrors so that we can fish it out in Visual Studio and use it to determine error stickiness.
                    | ErrorStyle.VSErrors -> sprintf "%s %s FS%04d: " subcategory (if warn then "warning" else "error") errorNumber;
                    | _ -> sprintf "%s FS%04d: " (if warn then "warning" else "error") (GetErrorNumber err);
                {  ErrorNumber = errorNumber; Subcategory = subcategory; TextRepresentation = text}
        
            let mainError,relatedErrors = SplitRelatedErrors err
            let where = OutputWhere(mainError)
            let canonical = OutputCanonicalInformation(mainError,err.Subcategory(),GetErrorNumber mainError)
            let message = 
                let os = System.Text.StringBuilder()
                OutputPhasedError os mainError flattenErrors;
                os.ToString()
            
            let entry = { Location = where; Canonical = canonical; Message = message }
            
            errors.Add ( ErrorOrWarning.Long( not warn, entry ) )

            let OutputRelatedError(err) =
                match errorStyle with
                // Give a canonical string when --vserror.
                | ErrorStyle.VSErrors -> 
                    let relWhere = OutputWhere(mainError) // mainError?
                    let relCanonical = OutputCanonicalInformation(err, err.Subcategory(),GetErrorNumber mainError) // Use main error for code
                    let relMessage = 
                        let os = System.Text.StringBuilder()
                        OutputPhasedError os err flattenErrors
                        os.ToString()

                    let entry = { Location = relWhere; Canonical = relCanonical; Message = relMessage}
                    errors.Add( ErrorOrWarning.Long (not warn, entry) )

                | _ -> 
                    let os = System.Text.StringBuilder()
                    OutputPhasedError os err flattenErrors
                    errors.Add( ErrorOrWarning.Short((not warn), os.ToString()) )
        
            relatedErrors |> List.iter OutputRelatedError

        match err with
#if EXTENSIONTYPING
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
let rec OutputErrorOrWarning (implicitIncludeDir,showFullPaths,flattenErrors,errorStyle,warn) os (err:PhasedError) = 
    
    let errors = CollectErrorOrWarning (implicitIncludeDir,showFullPaths,flattenErrors,errorStyle,warn, err)
    for e in errors do
        Printf.bprintf os "\n"
        match e with
        | Short(_, txt) -> 
            os.Append txt |> ignore
        | Long(_, details) ->
            match details.Location with
            | Some l when not l.IsEmpty -> os.Append(l.TextRepresentation) |> ignore
            | _ -> ()
            os.Append( details.Canonical.TextRepresentation ) |> ignore
            os.Append( details.Message ) |> ignore
      
let OutputErrorOrWarningContext prefix fileLineFn os err =
    match RangeOfError err with
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
            Printf.bprintf os "%s%s\n"   prefix line;
            Printf.bprintf os "%s%s%s\n" prefix (String.make iA '-') (String.make iLen '^')



//----------------------------------------------------------------------------

let GetFSharpCoreLibraryName () = "FSharp.Core"

type internal TypeInThisAssembly = class end
let GetFSharpCoreReferenceUsedByCompiler() = 
    let fsCoreName = GetFSharpCoreLibraryName()
    typeof<TypeInThisAssembly>.Assembly.GetReferencedAssemblies()
    |> Array.pick (fun name ->
        if name.Name = fsCoreName then Some(name.ToString())
        else None
    )
let GetFsiLibraryName () = "FSharp.Compiler.Interactive.Settings"  

// This list is the default set of references for "non-project" files. 
//
// These DLLs are
//    (a) included in the environment used for all .fsx files (see service.fs)
//    (b) included in environment for files 'orphaned' from a project context
//            -- for orphaned files (files in VS without a project context)
//            -- for files given on a command line without --noframework set
let DefaultBasicReferencesForOutOfProjectSources = 
    [ yield "System"
      yield "System.Xml" 
      yield "System.Runtime.Remoting"
      yield "System.Runtime.Serialization.Formatters.Soap"
      yield "System.Data"
      yield "System.Drawing"
      
      // Don't reference System.Core for .NET 2.0 compilations.
      //
      // We only use a default reference to System.Core if one exists which we can load it into the compiler process.
      // Note: this is not a partiuclarly good technique as it relying on the environment the compiler is executing in
      // to determine the default references. However, System.Core will only fail to load on machines with only .NET 2.0,
      // in which case the compiler will also be running as a .NET 2.0 process.
      if (try System.Reflection.Assembly.Load "System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" |> ignore; true with _ -> false) then 
          yield "System.Core" 

      yield "System.Web"
      yield "System.Web.Services"
      yield "System.Windows.Forms" ]

// Extra implicit references for .NET 4.0
let DefaultBasicReferencesForOutOfProjectSources40 = 
    [ "System.Numerics" ]

// A set of assemblies to always consider to be system assemblies
let SystemAssemblies primaryAssemblyName = 
    [ yield primaryAssemblyName 
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
      yield "System.Observable"
      yield "System.Numerics"] 

// The set of references entered into the TcConfigBuilder for scripts prior to computing
// the load closure. 
//
// REVIEW: it isn't clear if there is any negative effect
// of leaving an assembly off this list.
let BasicReferencesForScriptLoadClosure = 
    ["mscorlib"; GetFSharpCoreLibraryName () ] @ // Need to resolve these explicitly so they will be found in the reference assemblies directory which is where the .xml files are.
    DefaultBasicReferencesForOutOfProjectSources @ 
    [ GetFsiLibraryName () ]

let (++) x s = x @ [s]



//----------------------------------------------------------------------------
// General file name resolver
//--------------------------------------------------------------------------

/// Will return None if the filename is not found.
let TryResolveFileUsingPaths(paths,m,name) =
    let () = 
        try FileSystem.IsPathRootedShim(name)  |> ignore 
        with :? System.ArgumentException as e -> error(Error(FSComp.SR.buildProblemWithFilename(name,e.Message),m))
    if FileSystem.IsPathRootedShim(name) && FileSystem.SafeExists name 
    then Some name 
    else
        let res = paths |> List.tryPick (fun path ->  
                    let n = Path.Combine (path, name)
                    if FileSystem.SafeExists n then  Some n 
                    else None)
        res                        

/// Will raise FileNameNotResolved if the filename was not found
let ResolveFileUsingPaths(paths,m,name) =
    match TryResolveFileUsingPaths(paths,m,name) with
    | Some(res) -> res
    | None ->
        let searchMessage = String.concat "\n " paths
        raise (FileNameNotResolved(name,searchMessage,m))            
            
let GetWarningNumber(m,s:string) =
    try 
        Some (int32 s)
    with err -> 
        warning(Error(FSComp.SR.buildInvalidWarningNumber(s),m));
        None

let ComputeMakePathAbsolute implicitIncludeDir (path : string) = 
    try  
        // remove any quotation marks from the path first
        let path = path.Replace("\"","")
        if not (FileSystem.IsPathRootedShim(path)) 
        then Path.Combine (implicitIncludeDir, path)
        else path 
    with 
        :? System.ArgumentException -> path  

//----------------------------------------------------------------------------
// Configuration
//--------------------------------------------------------------------------

type CompilerTarget = 
    | WinExe 
    | ConsoleExe 
    | Dll 
    | Module
    member x.IsExe = (match x with ConsoleExe | WinExe -> true | _ -> false)

type ResolveAssemblyReferenceMode = Speculative | ReportErrors

type VersionFlag = 
    | VersionString of string
    | VersionFile of string
    | VersionNone
    member x.GetVersionInfo(implicitIncludeDir) =
        let vstr = x.GetVersionString(implicitIncludeDir)
        try 
            IL.parseILVersion vstr
        with _ -> errorR(Error(FSComp.SR.buildInvalidVersionString(vstr),rangeStartup)) ; IL.parseILVersion "0.0.0.0"
        
    member x.GetVersionString(implicitIncludeDir) = 
         match x with 
         | VersionString s -> s
         | VersionFile s ->
             let s = if FileSystem.IsPathRootedShim(s) then s else Path.Combine(implicitIncludeDir,s)
             if not(FileSystem.SafeExists(s)) then 
                 errorR(Error(FSComp.SR.buildInvalidVersionFile(s),rangeStartup)) ; "0.0.0.0"
             else
                 use is = System.IO.File.OpenText s
                 is.ReadLine()
         | VersionNone -> "0.0.0.0"
     

type AssemblyReference = 
    | AssemblyReference of range * string 
    member x.Range = (let (AssemblyReference(m,_)) = x in m)
    member x.Text = (let (AssemblyReference(_,text)) = x in text)
    member x.SimpleAssemblyNameIs(name) = 
        (String.Compare(fileNameWithoutExtension x.Text, name, StringComparison.OrdinalIgnoreCase) = 0) ||
        (let text = x.Text.ToLowerInvariant()
         not (text.Contains "/") && not (text.Contains "\\") && not (text.Contains ".dll") && not (text.Contains ".exe") &&
           try let aname = System.Reflection.AssemblyName(x.Text) in aname.Name = name 
           with _ -> false) 
    override x.ToString() = sprintf "AssemblyReference(%s)" x.Text

type UnresolvedAssemblyReference = UnresolvedAssemblyReference of string * AssemblyReference list
#if EXTENSIONTYPING
type ResolvedExtensionReference = ResolvedExtensionReference of string * AssemblyReference list * Tainted<ITypeProvider> list
#endif

type ImportedBinary = 
    { FileName: string;
      RawMetadata: ILModuleDef; 
#if EXTENSIONTYPING
      ProviderGeneratedAssembly: System.Reflection.Assembly option
      IsProviderGenerated: bool;
      ProviderGeneratedStaticLinkMap : ProvidedAssemblyStaticLinkingMap option
#endif
      ILAssemblyRefs : ILAssemblyRef list;
      ILScopeRef: ILScopeRef }

type ImportedAssembly = 
    { ILScopeRef: ILScopeRef; 
      FSharpViewOfMetadata: CcuThunk;
      AssemblyAutoOpenAttributes: string list;
      AssemblyInternalsVisibleToAttributes: string list;
#if EXTENSIONTYPING
      IsProviderGenerated: bool
      mutable TypeProviders: Tainted<Microsoft.FSharp.Core.CompilerServices.ITypeProvider> list;
#endif
      FSharpOptimizationData : Microsoft.FSharp.Control.Lazy<Option<Opt.LazyModuleInfo>> }

type AvailableImportedAssembly =
    | ResolvedImportedAssembly of ImportedAssembly
    | UnresolvedImportedAssembly of string

// Helps to perform 2-step initialization of the system runtime
// Compiler heavily relies on ILGlobals structure that contains fundamental types.
// For mscorlib based profiles everything was easy - all fundamental types were located in one assembly so initialization sequence was simple
// - read mscorlib -> create ILGlobals (*) -> use ILGlobals to read remaining assemblies
// For .NETCore everything is not so obvious because fundamental types now reside in different assemblies and this makes initialization more tricky:
// - read system runtime -> create ILGlobals that is partially initialized (*) -> use ILGlobals to read remaining assemblies -> finish the initialization of ILGlobals using data from the previous step
// BeginLoadingSystemRuntime -> (*) EndLoadingSystemRuntime

type CcuLoadFailureAction = 
    | RaiseError
    | ReturnNone

type ISystemRuntimeCcuInitializer = 
    abstract BeginLoadingSystemRuntime : resolver : (AssemblyReference -> ImportedAssembly) * noDebug :bool -> ILGlobals * obj
    abstract EndLoadingSystemRuntime : state : obj * resolver : (CcuLoadFailureAction -> AssemblyReference -> ImportedAssembly option) -> ImportedAssembly

type NetCoreSystemRuntimeTraits(primaryAssembly) = 
    
    let valueOf name hole = 
        match hole with
        | Some assembly -> assembly
        | None -> failwithf "Internal compiler error: scope ref hole '%s' is not initialized" name

    let mutable systemReflection = None
    let mutable systemDiagnosticsDebug = None
    let mutable systemLinqExpressions = None
    let mutable systemCollections = None
    let mutable systemRuntimeInteropServices = None

    member this.FixupImportedAssemblies(systemReflectionRef, systemDiagnosticsDebugRef, systemLinqExpressionsRef, systemCollectionsRef, systemRuntimeInteropServicesRef) = 
        systemReflection        <- systemReflectionRef
        systemDiagnosticsDebug  <- systemDiagnosticsDebugRef
        systemLinqExpressions   <- systemLinqExpressionsRef
        systemCollections       <- systemCollectionsRef
        systemRuntimeInteropServices <- systemRuntimeInteropServicesRef

    interface IPrimaryAssemblyTraits with
        member this.ScopeRef = primaryAssembly
        member this.SystemReflectionScopeRef        = lazy ((valueOf "System.Reflection" systemReflection).FSharpViewOfMetadata.ILScopeRef)
        member this.TypedReferenceTypeScopeRef      = None
        member this.RuntimeArgumentHandleTypeScopeRef = None
        member this.SerializationInfoTypeScopeRef   = None
        member this.SecurityPermissionAttributeTypeScopeRef = None
        member this.SystemDiagnosticsDebugScopeRef  = lazy ((valueOf "System.Diagnostics.Debug" systemDiagnosticsDebug).FSharpViewOfMetadata.ILScopeRef)
        member this.SystemRuntimeInteropServicesScopeRef    = 
            lazy 
                match systemRuntimeInteropServices with 
                | Some assemblyRef ->  Some assemblyRef.FSharpViewOfMetadata.ILScopeRef
                | None -> None
        member this.IDispatchConstantAttributeScopeRef      = None
        member this.IUnknownConstantAttributeScopeRef       = None
        member this.ContextStaticAttributeScopeRef  = None
        member this.ThreadStaticAttributeScopeRef   = None
        member this.SystemLinqExpressionsScopeRef   = lazy ((valueOf "System.Linq.Expressions" systemLinqExpressions).FSharpViewOfMetadata.ILScopeRef)
        member this.SystemCollectionsScopeRef       = lazy ((valueOf "System.Collections" systemCollections).FSharpViewOfMetadata.ILScopeRef)
        member this.SpecialNameAttributeScopeRef    = None
        member this.NonSerializedAttributeScopeRef  = None
        member this.MarshalByRefObjectScopeRef      = None
        member this.ArgIteratorTypeScopeRef         = None

let getSystemRuntimeInitializer (primaryAssembly: PrimaryAssembly) (mkReference : string -> AssemblyReference) : ISystemRuntimeCcuInitializer = 
    let name = primaryAssembly.Name
    let primaryAssemblyReference = mkReference name

    match primaryAssembly with
    | Mscorlib ->
        {
            new ISystemRuntimeCcuInitializer with
                member this.BeginLoadingSystemRuntime(resolver, noData) = 
                    let mscorlibRef = resolver primaryAssemblyReference
                    let traits = (IL.mkMscorlibBasedTraits mscorlibRef.FSharpViewOfMetadata.ILScopeRef)
                    (mkILGlobals traits (Some name) noData), box mscorlibRef
                member this.EndLoadingSystemRuntime(state, _resolver) = 
                    unbox state
        }

    | DotNetCore ->
        let systemReflectionRef = mkReference "System.Reflection"
        let systemDiagnosticsDebugRef = mkReference "System.Diagnostics.Debug"
        let systemLinqExpressionsRef = mkReference "System.Linq.Expressions"
        let systemCollectionsRef = mkReference "System.Collections"
        let systemRuntimeInteropServicesRef = mkReference "System.Runtime.InteropServices"
        {
            new ISystemRuntimeCcuInitializer with
                member this.BeginLoadingSystemRuntime(resolver, noData) = 
                    let primaryAssembly = resolver primaryAssemblyReference
                    let traits = new NetCoreSystemRuntimeTraits(primaryAssembly.FSharpViewOfMetadata.ILScopeRef)
                    mkILGlobals traits (Some name) noData, box (primaryAssembly, traits)
                member this.EndLoadingSystemRuntime(state, resolver) = 
                    let (primaryAssembly : ImportedAssembly, traits : NetCoreSystemRuntimeTraits) = unbox state
                    // finish initialization of SystemRuntimeTraits
                    traits.FixupImportedAssemblies
                        (
                            systemReflectionRef             = resolver CcuLoadFailureAction.RaiseError systemReflectionRef,
                            systemDiagnosticsDebugRef       = resolver CcuLoadFailureAction.RaiseError systemDiagnosticsDebugRef,
                            systemRuntimeInteropServicesRef = resolver CcuLoadFailureAction.ReturnNone systemRuntimeInteropServicesRef,
                            systemLinqExpressionsRef        = resolver CcuLoadFailureAction.RaiseError systemLinqExpressionsRef,
                            systemCollectionsRef            = resolver CcuLoadFailureAction.RaiseError systemCollectionsRef
                        )
                    primaryAssembly
        }


type TcConfigBuilder =
    { mutable primaryAssembly : PrimaryAssembly;
      mutable autoResolveOpenDirectivesToDlls: bool;
      mutable noFeedback: bool;
      mutable stackReserveSize: int32 option;
      mutable implicitIncludeDir: string; (* normally "." *)
      mutable openBinariesInMemory: bool; (* false for command line, true for VS *)
      mutable openDebugInformationForLaterStaticLinking: bool; (* only for --standalone *)
      defaultFSharpBinariesDir: string;
      mutable compilingFslib: bool;
      mutable compilingFslib20: string option;
      mutable compilingFslib40: bool;
      mutable useIncrementalBuilder: bool;
      mutable includes: string list;
      mutable implicitOpens: string list;
      mutable useFsiAuxLib: bool;
      mutable framework: bool;
      mutable resolutionEnvironment : Microsoft.FSharp.Compiler.MSBuildResolver.ResolutionEnvironment
      mutable implicitlyResolveAssemblies: bool;
      mutable addVersionSpecificFrameworkReferences: bool;
      mutable light: bool option;
      mutable conditionalCompilationDefines: string list;
      mutable loadedSources: (range * string) list;
      mutable referencedDLLs : AssemblyReference list;
      mutable knownUnresolvedReferences : UnresolvedAssemblyReference list;
      optimizeForMemory: bool;
      mutable subsystemVersion : int * int
      mutable useHighEntropyVA : bool
      mutable inputCodePage: int option;
      mutable embedResources : string list;
      mutable globalWarnAsError: bool;
      mutable globalWarnLevel: int;
      mutable specificWarnOff: int list; 
      mutable specificWarnOn: int list; 
      mutable specificWarnAsError: int list 
      mutable specificWarnAsWarn : int list
      mutable mlCompatibility: bool;
      mutable checkOverflow: bool;
      mutable showReferenceResolutions:bool;
      mutable outputFile : string option;
      mutable resolutionFrameworkRegistryBase : string;
      mutable resolutionAssemblyFoldersSuffix : string; 
      mutable resolutionAssemblyFoldersConditions : string;    
      mutable platform : ILPlatform option;
      mutable prefer32Bit : bool;
      mutable useMonoResolution : bool
      mutable target : CompilerTarget
      mutable debuginfo : bool
      mutable testFlagEmitFeeFeeAs100001 : bool;
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
      mutable version : VersionFlag 
      mutable metadataVersion : string option
      mutable standalone : bool
      mutable extraStaticLinkRoots : string list 
      mutable noSignatureData : bool
      mutable onlyEssentialOptimizationData : bool
      mutable useOptimizationDataFile : bool
      mutable useSignatureDataFile : bool
      mutable jitTracking : bool
      mutable ignoreSymbolStoreSequencePoints : bool
      mutable internConstantStrings : bool
      mutable extraOptimizationIterations : int

      mutable win32res : string 
      mutable win32manifest : string
      mutable includewin32manifest : bool
      mutable linkResources : string list


      mutable showFullPaths : bool
      mutable errorStyle : ErrorStyle
      mutable validateTypeProviders: bool
      mutable utf8output : bool
      mutable flatErrors: bool

      mutable maxErrors : int
      mutable abortOnError : bool (* intended for fsi scripts that should exit on first error *)
      mutable baseAddress : int32 option
#if DEBUG
      mutable writeGeneratedILFiles : bool (* write il files? *)  
      mutable showOptimizationData : bool
#endif
      mutable showTerms     : bool (* show terms between passes? *)
      mutable writeTermsToFiles : bool (* show terms to files? *)
      mutable doDetuple     : bool (* run detuple pass? *)
      mutable doTLR         : bool (* run TLR     pass? *)
      mutable doFinalSimplify : bool (* do final simplification pass *)
      mutable optsOn        : bool (* optimizations are turned on *)
      mutable optSettings   : Opt.OptimizationSettings 
      mutable emitTailcalls : bool
      mutable lcid          : int option

      mutable productNameForBannerText : string
      /// show the MS (c) notice, e.g. with help or fsi? 
      mutable showBanner  : bool
        
      /// show times between passes? 
      mutable showTimes : bool
      mutable showLoadedAssemblies : bool
      mutable continueAfterParseFailure : bool
#if EXTENSIONTYPING
      /// show messages about extension type resolution?
      mutable showExtensionTypeMessages : bool
#endif

      /// pause between passes? 
      mutable pause : bool
      
      /// use reflection and indirect calls to call methods taking multidimensional generic arrays
      mutable indirectCallArrayMethods : bool 
      
      /// whenever possible, emit callvirt instead of call
      mutable alwaysCallVirt : bool

      /// if true, strip away data that would not be of use to end users, but is useful to us for debugging
      // REVIEW: "stripDebugData"?
      mutable noDebugData : bool

      /// if true, indicates all type checking and code generation is in the context of fsi.exe
      isInteractive : bool
      isInvalidationSupported : bool

      /// used to log sqm data
      mutable sqmSessionGuid : System.Guid option
      mutable sqmNumOfSourceFiles : int
      sqmSessionStartedTime : int64

      /// if true - every expression in quotations will be augmented with full debug info (filename, location in file)
      mutable emitDebugInfoInQuotations : bool

      /// When false FSI will lock referenced assemblies requiring process restart, false = disable Shadow Copy false (*default*)
      mutable shadowCopyReferences : bool
      }


    static member CreateNew (defaultFSharpBinariesDir,optimizeForMemory,implicitIncludeDir,isInteractive,isInvalidationSupported) =
        System.Diagnostics.Debug.Assert(FileSystem.IsPathRootedShim(implicitIncludeDir), sprintf "implicitIncludeDir should be absolute: '%s'" implicitIncludeDir)
        if (String.IsNullOrEmpty(defaultFSharpBinariesDir)) then 
            failwith "Expected a valid defaultFSharpBinariesDir"
        { primaryAssembly = PrimaryAssembly.Mscorlib; // defaut value, can be overridden using the command line switch
          light = None;
          noFeedback=false;
          stackReserveSize=None;
          conditionalCompilationDefines=[];
          implicitIncludeDir = implicitIncludeDir;
          autoResolveOpenDirectivesToDlls = false;
          openBinariesInMemory = false;
          openDebugInformationForLaterStaticLinking=false;
          defaultFSharpBinariesDir=defaultFSharpBinariesDir;
          compilingFslib=false;
          compilingFslib20=None;
          compilingFslib40=false;
          useIncrementalBuilder=false;
          useFsiAuxLib=false;
          implicitOpens=[];
          includes=[];
          resolutionEnvironment=MSBuildResolver.CompileTimeLike
          framework=true;
          implicitlyResolveAssemblies=true;
          addVersionSpecificFrameworkReferences=false;
          referencedDLLs = [];
          knownUnresolvedReferences = [];
          loadedSources = [];
          globalWarnAsError=false;
          globalWarnLevel=3;
          specificWarnOff=[]; 
          specificWarnOn=[]; 
          specificWarnAsError=[] 
          specificWarnAsWarn=[]
          embedResources = [];
          inputCodePage=None;
          optimizeForMemory=optimizeForMemory;
          subsystemVersion = 4,0 // per spec for 357994
          useHighEntropyVA = false
          mlCompatibility=false;
          checkOverflow=false;
          showReferenceResolutions=false;
          outputFile=None;
          resolutionFrameworkRegistryBase = "Software\Microsoft\.NetFramework";
          resolutionAssemblyFoldersSuffix = "AssemblyFoldersEx"; 
          resolutionAssemblyFoldersConditions = "";              
          platform = None;
          prefer32Bit = false;
          useMonoResolution = runningOnMono
          target = ConsoleExe
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
          version = VersionNone
          metadataVersion = None
          standalone = false
          extraStaticLinkRoots = []
          noSignatureData = false
          onlyEssentialOptimizationData = false
          useOptimizationDataFile = false
          useSignatureDataFile = false
          jitTracking = true
          ignoreSymbolStoreSequencePoints = false
          internConstantStrings = true
          extraOptimizationIterations = 0

          win32res = ""
          win32manifest = ""
          includewin32manifest = true
          linkResources = []
          showFullPaths =false
          errorStyle = ErrorStyle.DefaultErrors
#if COMPILED_AS_LANGUAGE_SERVICE_DLL
          validateTypeProviders = true
#else
          validateTypeProviders = false
#endif

          utf8output = false
          flatErrors = false

 #if DEBUG
          writeGeneratedILFiles       = false (* write il files? *)  
          showOptimizationData = false
 #endif
          showTerms     = false 
          writeTermsToFiles = false 
          
          doDetuple     = false 
          doTLR         = false 
          doFinalSimplify = false
          optsOn        = false 
          optSettings   = Opt.OptimizationSettings.Defaults
          emitTailcalls = true
          lcid = None
          // See bug 6071 for product banner spec
          productNameForBannerText = (FSComp.SR.buildProductName(FSharpEnvironment.DotNetBuildString))
          showBanner  = true 
          showTimes = false 
          showLoadedAssemblies = false
          continueAfterParseFailure = false
#if EXTENSIONTYPING
          showExtensionTypeMessages = false
#endif
          pause = false 
          indirectCallArrayMethods = false
          alwaysCallVirt = true
          noDebugData = false
          isInteractive = isInteractive
          isInvalidationSupported = isInvalidationSupported
          sqmSessionGuid = None
          sqmNumOfSourceFiles = 0
          sqmSessionStartedTime = System.DateTime.Now.Ticks
          emitDebugInfoInQuotations = false
          shadowCopyReferences = false
        }

    member tcConfigB.ResolveSourceFile(m,nm,pathLoadedFrom) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        ResolveFileUsingPaths(tcConfigB.includes @ [pathLoadedFrom],m,nm)

    /// Decide names of output file, pdb and assembly
    member tcConfigB.DecideNames sourceFiles =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)    
        if sourceFiles = [] then errorR(Error(FSComp.SR.buildNoInputsSpecified(),rangeCmdArgs));
        let ext() = match tcConfigB.target with Dll -> ".dll" | Module -> ".netmodule" | ConsoleExe | WinExe -> ".exe"
        let implFiles = sourceFiles |> List.filter (fun lower -> List.exists (Filename.checkSuffix (String.lowercase lower)) implSuffixes)
        let outfile = 
            match tcConfigB.outputFile, List.rev implFiles with 
            | None,[] -> "out" + ext()
            | None, h :: _  -> 
                let basic = fileNameOfPath h
                let modname = try Filename.chopExtension basic with _ -> basic
                modname+(ext())
            | Some f,_ -> f
        let assemblyName, assemblyNameIsInvalid = 
            let baseName = fileNameOfPath outfile
            let assemblyName = fileNameWithoutExtension baseName
            if not (Filename.checkSuffix (String.lowercase baseName) (ext())) then
                errorR(Error(FSComp.SR.buildMismatchOutputExtension(),rangeCmdArgs))
                assemblyName, true
            else
                assemblyName, false

        let pdbfile = 
            
            if tcConfigB.debuginfo then
              // assembly name is invalid, we've already reported the error so just skip pdb name checks
              if assemblyNameIsInvalid then None else
#if NO_PDB_WRITER
              Some (match tcConfigB.debugSymbolFile with None -> (Filename.chopExtension outfile)+ (if runningOnMono then ".mdb" else ".pdb") | Some f -> f)
#else
              Some (match tcConfigB.debugSymbolFile with 
                    | None -> Microsoft.FSharp.Compiler.AbstractIL.Internal.Support.getDebugFileName outfile
                    | Some _ when runningOnMono ->
                        // On Mono, the name of the debug file has to be "<assemblyname>.mdb" so specifying it explicitly is an error
                        warning(Error(FSComp.SR.ilwriteMDBFileNameCannotBeChangedWarning(),rangeCmdArgs)) ; ()
                        Microsoft.FSharp.Compiler.AbstractIL.Internal.Support.getDebugFileName outfile
                    | Some f -> f)   
#endif
            elif (tcConfigB.debugSymbolFile <> None) && (not (tcConfigB.debuginfo)) then
              error(Error(FSComp.SR.buildPdbRequiresDebug(),rangeStartup))  
            else None
        tcConfigB.outputFile <- Some(outfile)
        outfile,pdbfile,assemblyName

    member tcConfigB.TurnWarningOff(m,s:string) =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)    
        match GetWarningNumber(m,s) with 
        | None -> ()
        | Some n -> 
            // nowarn:62 turns on mlCompatibility, e.g. shows ML compat items in intellisense menus
            if n = 62 then tcConfigB.mlCompatibility <- true;
            tcConfigB.specificWarnOff <- ListSet.insert (=) n tcConfigB.specificWarnOff

    member tcConfigB.TurnWarningOn(m, s:string) =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)    
        match GetWarningNumber(m,s) with 
        | None -> ()
        | Some n -> 
            // warnon 62 turns on mlCompatibility, e.g. shows ML compat items in intellisense menus
            if n = 62 then tcConfigB.mlCompatibility <- false;
            tcConfigB.specificWarnOn <- ListSet.insert (=) n tcConfigB.specificWarnOn

    member tcConfigB.AddIncludePath (m,path,pathIncludedFrom) = 
        let absolutePath = ComputeMakePathAbsolute pathIncludedFrom path
        let ok = 
            let existsOpt = 
                try Some(Directory.Exists(absolutePath)) 
                with e -> warning(Error(FSComp.SR.buildInvalidSearchDirectory(path),m)); None
            match existsOpt with 
            | Some(exists) -> 
                if not exists then warning(Error(FSComp.SR.buildSearchDirectoryNotFound(absolutePath),m));         
                exists
            | None -> false
        if ok && not (List.mem absolutePath tcConfigB.includes) then 
           tcConfigB.includes <- tcConfigB.includes ++ absolutePath
           
    member tcConfigB.AddLoadedSource(m,path,pathLoadedFrom) =
        if Path.IsInvalidPath(path) then
            warning(Error(FSComp.SR.buildInvalidFilename(path),m))    
        else 
            let path = 
                match TryResolveFileUsingPaths(tcConfigB.includes @ [pathLoadedFrom],m,path) with 
                | Some(path) -> path
                | None ->
                    // File doesn't exist in the paths. Assume it will be in the load-ed from directory.
                    ComputeMakePathAbsolute pathLoadedFrom path
            if not (List.mem path (List.map snd tcConfigB.loadedSources)) then 
                tcConfigB.loadedSources <- tcConfigB.loadedSources ++ (m,path)
                

    member tcConfigB.AddEmbeddedResource filename =
        tcConfigB.embedResources <- tcConfigB.embedResources ++ filename

    member tcConfigB.AddReferencedAssemblyByPath (m,path) = 
        if Path.IsInvalidPath(path) then
            warning(Error(FSComp.SR.buildInvalidAssemblyName(path),m))
        elif not (List.mem (AssemblyReference(m,path)) tcConfigB.referencedDLLs) then // NOTE: We keep same paths if range is different.
             tcConfigB.referencedDLLs <- tcConfigB.referencedDLLs ++ AssemblyReference(m,path)
             
    member tcConfigB.RemoveReferencedAssemblyByPath (m,path) =
        tcConfigB.referencedDLLs <- List.filter (fun (ar : AssemblyReference) -> ar <> AssemblyReference(m,path)) tcConfigB.referencedDLLs
    
    static member SplitCommandLineResourceInfo ri = 
        if String.contains ri ',' then 
            let p = String.index ri ',' 
            let file = String.sub ri 0 p 
            let rest = String.sub ri (p+1) (String.length ri - p - 1) 
            if String.contains rest ',' then 
                let p = String.index rest ',' 
                let name = String.sub rest 0 p+".resources" 
                let pubpri = String.sub rest (p+1) (rest.Length - p - 1) 
                if pubpri = "public" then file,name,ILResourceAccess.Public 
                elif pubpri = "private" then file,name,ILResourceAccess.Private
                else error(Error(FSComp.SR.buildInvalidPrivacy(pubpri),rangeStartup))
            else 
                file,rest,ILResourceAccess.Public
        else 
            ri,fileNameOfPath ri,ILResourceAccess.Public 


let OpenILBinary(filename,optimizeForMemory,openBinariesInMemory,ilGlobalsOpt, pdbPathOption, primaryAssemblyName, noDebugData, shadowCopyReferences) = 
      let ilGlobals   = 
          // ILScopeRef.Local can be used only for primary assembly (mscorlib or System.Runtime) itself
          // Remaining assemblies should be opened using existing ilGlobals (so they can properly locate fundamental types)
          match ilGlobalsOpt with 
          | None -> mkILGlobals (IL.mkMscorlibBasedTraits ILScopeRef.Local) (Some primaryAssemblyName) noDebugData
          | Some ilGlobals -> ilGlobals

      let opts = { ILBinaryReader.mkDefault ilGlobals with                       
                      // fsc.exe does not uses optimizeForMemory (hence keeps MORE caches in AbstractIL)
                      // fsi.exe does use optimizeForMemory (hence keeps FEWER caches in AbstractIL), because its long running
                      // Visual Studio does use optimizeForMemory (hence keeps FEWER caches in AbstractIL), because its long running
                      ILBinaryReader.optimizeForMemory=optimizeForMemory;
                      ILBinaryReader.pdbPath = pdbPathOption; } 
                      
      // Visual Studio uses OpenILModuleReaderAfterReadingAllBytes for all DLLs to avoid having to dispose of any readers explicitly
      if openBinariesInMemory // && not syslib 
      then ILBinaryReader.OpenILModuleReaderAfterReadingAllBytes filename opts
      else
        let location =
          // In order to use memory mapped files on the shadow copied version of the Assembly, we `preload the assembly
          // We swallow all exceptions so that we do not change the exception contract of this API
          if shadowCopyReferences then 
            try
              System.Reflection.Assembly.ReflectionOnlyLoadFrom(filename).Location
            with e -> filename
          else
            filename
        ILBinaryReader.OpenILModuleReader location opts

#if DEBUG
[<System.Diagnostics.DebuggerDisplayAttribute("AssemblyResolution({resolvedPath})")>]
#endif
type AssemblyResolution = 
    { originalReference : AssemblyReference
      resolvedPath : string    
      resolvedFrom : ResolvedFrom
      fusionName : string
      redist : string 
      sysdir : bool 
      ilAssemblyRef : ILAssemblyRef option ref
    }
    member this.ILAssemblyRef = 
        match !this.ilAssemblyRef with 
        | Some(assref) -> assref
        | None ->
            let readerSettings : ILBinaryReader.ILReaderOptions = {pdbPath=None;ilGlobals = EcmaILGlobals;optimizeForMemory=false} // ??
            let reader = ILBinaryReader.OpenILModuleReader this.resolvedPath readerSettings
            try
                let assRef = mkRefToILAssembly reader.ILModuleDef.ManifestOfAssembly
                this.ilAssemblyRef := Some(assRef)
                assRef
            finally 
                ILBinaryReader.CloseILModuleReader reader


//----------------------------------------------------------------------------
// Names to match up refs and defs for assemblies and modules
//--------------------------------------------------------------------------

let GetNameOfILModule (m: ILModuleDef) = 
    match m.Manifest with 
    | Some manifest -> manifest.Name
    | None -> m.Name


let MakeScopeRefForIlModule (ilModule: ILModuleDef) = 
    match ilModule.Manifest with 
    | Some m -> ILScopeRef.Assembly (mkRefToILAssembly m)
    | None -> ILScopeRef.Module (mkRefToILModule ilModule)

let GetCustomAttributesOfIlModule (ilModule:ILModuleDef) = 
    (match ilModule.Manifest with Some m -> m.CustomAttrs | None -> ilModule.CustomAttrs).AsList 

let GetAutoOpenAttributes ilg ilModule = 
    ilModule |> GetCustomAttributesOfIlModule |> List.choose (TryFindAutoOpenAttr ilg)

let GetInternalsVisibleToAttributes ilg ilModule = 
    ilModule |> GetCustomAttributesOfIlModule |> List.choose (TryFindInternalsVisibleToAttr ilg)
    
//----------------------------------------------------------------------------
// TcConfig 
//--------------------------------------------------------------------------

[<Sealed>]
/// This type is immutable and must be kept as such. Do not extract or mutate the underlying data except by cloning it.
type TcConfig private (data : TcConfigBuilder,validate:bool) =

    // Validate the inputs - this helps ensure errors in options are shown in visual studio rather than only when built
    // However we only validate a minimal number of options at the moment
    do if validate then try data.version.GetVersionInfo(data.implicitIncludeDir) |> ignore with e -> errorR(e) 

    // clone the input builder to ensure nobody messes with it.
    let data = { data with pause = data.pause }

    let computeKnownDllReference(libraryName) = 
        let defaultCoreLibraryReference = AssemblyReference(range0,libraryName+".dll")
        let nameOfDll(AssemblyReference(m,filename) as r) = 
            let filename = ComputeMakePathAbsolute data.implicitIncludeDir filename
            if FileSystem.SafeExists(filename) then 
                r,Some(filename)
            else   
                // If the file doesn't exist, let reference resolution logic report the error later...
                defaultCoreLibraryReference, if m=rangeStartup then Some(filename) else None
        match data.referencedDLLs |> List.filter(fun assemblyReference -> assemblyReference.SimpleAssemblyNameIs libraryName) with
        | [r] -> nameOfDll r
        | [] -> 
            defaultCoreLibraryReference, None
        | r:: _ -> 
            // Recover by picking the first one.
            errorR(Error(FSComp.SR.buildMultipleReferencesNotAllowed(libraryName),rangeCmdArgs)) 
            nameOfDll(r)

    // Look for an explicit reference to mscorlib and use that to compute clrRoot and targetFrameworkVersion
    let primaryAssemblyReference, primaryAssemblyExplicitFilenameOpt = computeKnownDllReference(data.primaryAssembly.Name)
    let fslibReference,fslibExplicitFilenameOpt = 
        let (_, fileNameOpt) as res = computeKnownDllReference(GetFSharpCoreLibraryName())
        match fileNameOpt with
        | None -> 
            // if FSharp.Core was not provided explicitly - use version that was referenced by compiler
            AssemblyReference(range0, GetFSharpCoreReferenceUsedByCompiler()), None
        | _ -> res
    let primaryAssemblyCcuInitializer = getSystemRuntimeInitializer data.primaryAssembly (computeKnownDllReference >> fst)

    // If either mscorlib.dll/System.Runtime.dll or fsharp.core.dll are explicitly specified then we require the --noframework flag.
    // The reason is that some non-default frameworks may not have the default dlls. For example, Client profile does
    // not have System.Web.dll.
    do if ((primaryAssemblyExplicitFilenameOpt.IsSome || fslibExplicitFilenameOpt.IsSome) && data.framework) then
            error(Error(FSComp.SR.buildExplicitCoreLibRequiresNoFramework("--noframework"),rangeStartup))

    let clrRootValue, (mscorlibMajorVersion,targetFrameworkVersionValue), primaryAssemblyIsSilverlight = 
        match primaryAssemblyExplicitFilenameOpt with
        | Some(primaryAssemblyFilename) ->
            let filename = ComputeMakePathAbsolute data.implicitIncludeDir primaryAssemblyFilename
            try 
            
                let ilReader = OpenILBinary(filename,data.optimizeForMemory,data.openBinariesInMemory,None,None, data.primaryAssembly.Name, data.noDebugData, data.shadowCopyReferences)
                try 
                   let ilModule = ilReader.ILModuleDef
                 
                   match ilModule.ManifestOfAssembly.Version with 
                   | Some(v1,v2,v3,_) -> 
                       if v1 = 1us then 
                           warning(Error(FSComp.SR.buildRequiresCLI2(filename),rangeStartup))
                       let clrRoot = Some(Path.GetDirectoryName(Path.GetFullPath(filename)))

                       clrRoot, (int v1, sprintf "v%d.%d" v1 v2), (v1=5us && v2=0us && v3=5us) // SL5 mscorlib is 5.0.5.0
                   | _ -> 
                       failwith (FSComp.SR.buildCouldNotReadVersionInfoFromMscorlib())
                finally
                   ILBinaryReader.CloseILModuleReader ilReader
            with _ -> 
                error(Error(FSComp.SR.buildCannotReadAssembly(filename),rangeStartup))
        | _ ->
            None, MSBuildResolver.HighestInstalledNetFrameworkVersionMajorMinor(), false

    // Note: anycpu32bitpreferred can only be used with .Net version 4.5 and above
    // but now there is no way to discriminate between 4.0 and 4.5,
    // so here we minimally validate if .Net version >= 4 or not.
    do if data.prefer32Bit && mscorlibMajorVersion < 4 then 
        error(Error(FSComp.SR.invalidPlatformTargetForOldFramework(),rangeCmdArgs))        
    
    let systemAssemblies = SystemAssemblies data.primaryAssembly.Name

    // Check that the referenced version of FSharp.Core.dll matches the referenced version of mscorlib.dll 
    let checkFSharpBinaryCompatWithMscorlib filename (ilAssemblyRefs: ILAssemblyRef list) explicitFscoreVersionToCheckOpt m = 
        let isfslib = fileNameOfPath filename = GetFSharpCoreLibraryName() + ".dll"
        match ilAssemblyRefs |> List.tryFind (fun aref -> aref.Name = data.primaryAssembly.Name) with 
        | Some aref ->
            match aref.Version with
            | Some(v1,_,_,_) ->
                if isfslib && ((v1 < 4us) <> (mscorlibMajorVersion < 4)) then
                    // the versions mismatch, however they are allowed to mismatch in one case:
                    if primaryAssemblyIsSilverlight  && mscorlibMajorVersion=5   // SL5
                        && (match explicitFscoreVersionToCheckOpt with 
                            | Some(2us,3us,5us,_) // silverlight is supported for FSharp.Core 2.3.5.x and 3.47.x.y 
                            | Some(3us,47us,_,_) 
                            | None -> true        // the 'None' code path happens after explicit FSCore was already checked, from now on SL5 path is always excepted
                            | _ -> false) 
                    then
                        ()
                    else
                        error(Error(FSComp.SR.buildMscorLibAndFSharpCoreMismatch(filename),m))
                // If you're building an assembly that references another assembly built for a more recent
                // framework version, we want to raise a warning
                elif not(isfslib) && ((v1 = 4us) && (mscorlibMajorVersion < 4)) then
                    warning(Error(FSComp.SR.buildMscorlibAndReferencedAssemblyMismatch(filename),m))
                else
                    ()
            | _ -> ()
        | _ -> ()

    // Look for an explicit reference to FSharp.Core and use that to compute fsharpBinariesDir
    let fsharpBinariesDirValue = 
        match fslibExplicitFilenameOpt with
        | Some(fslibFilename) ->
            let filename = ComputeMakePathAbsolute data.implicitIncludeDir fslibFilename
            try 
                let ilReader = OpenILBinary(filename,data.optimizeForMemory,data.openBinariesInMemory,None,None, data.primaryAssembly.Name, data.noDebugData, data.shadowCopyReferences)
                try 
                   checkFSharpBinaryCompatWithMscorlib filename ilReader.ILAssemblyRefs ilReader.ILModuleDef.ManifestOfAssembly.Version rangeStartup;
                   let fslibRoot = Path.GetDirectoryName(FileSystem.GetFullPathShim(filename))
                   fslibRoot (* , sprintf "v%d.%d" v1 v2 *)
                finally
                   ILBinaryReader.CloseILModuleReader ilReader
            with _ -> 
                error(Error(FSComp.SR.buildCannotReadAssembly(filename),rangeStartup))
        | _ ->
            data.defaultFSharpBinariesDir

    member x.MscorlibMajorVersion = mscorlibMajorVersion
    member x.primaryAssembly = data.primaryAssembly
    member x.autoResolveOpenDirectivesToDlls = data.autoResolveOpenDirectivesToDlls
    member x.noFeedback = data.noFeedback
    member x.stackReserveSize = data.stackReserveSize
    member x.implicitIncludeDir = data.implicitIncludeDir
    member x.openBinariesInMemory = data.openBinariesInMemory
    member x.openDebugInformationForLaterStaticLinking = data.openDebugInformationForLaterStaticLinking
    member x.fsharpBinariesDir = fsharpBinariesDirValue
    member x.compilingFslib = data.compilingFslib
    member x.compilingFslib20 = data.compilingFslib20
    member x.compilingFslib40 = data.compilingFslib40
    member x.useIncrementalBuilder = data.useIncrementalBuilder
    member x.includes = data.includes
    member x.implicitOpens = data.implicitOpens
    member x.useFsiAuxLib = data.useFsiAuxLib
    member x.framework = data.framework
    member x.implicitlyResolveAssemblies = data.implicitlyResolveAssemblies
    member x.addVersionSpecificFrameworkReferences = data.addVersionSpecificFrameworkReferences
    member x.resolutionEnvironment = data.resolutionEnvironment
    member x.light = data.light
    member x.conditionalCompilationDefines = data.conditionalCompilationDefines
    member x.loadedSources = data.loadedSources
    member x.referencedDLLs = data.referencedDLLs
    member x.knownUnresolvedReferences = data.knownUnresolvedReferences
    member x.clrRoot = clrRootValue
    member x.optimizeForMemory = data.optimizeForMemory
    member x.subsystemVersion = data.subsystemVersion
    member x.useHighEntropyVA = data.useHighEntropyVA
    member x.inputCodePage = data.inputCodePage
    member x.embedResources  = data.embedResources
    member x.globalWarnAsError = data.globalWarnAsError
    member x.globalWarnLevel = data.globalWarnLevel
    member x.specificWarnOff = data. specificWarnOff
    member x.specificWarnOn = data. specificWarnOn
    member x.specificWarnAsError = data.specificWarnAsError
    member x.specificWarnAsWarn = data.specificWarnAsWarn
    member x.mlCompatibility = data.mlCompatibility
    member x.checkOverflow = data.checkOverflow
    member x.showReferenceResolutions = data.showReferenceResolutions
    member x.outputFile  = data.outputFile
    member x.resolutionFrameworkRegistryBase  = data.resolutionFrameworkRegistryBase
    member x.resolutionAssemblyFoldersSuffix  = data. resolutionAssemblyFoldersSuffix
    member x.resolutionAssemblyFoldersConditions  = data.  resolutionAssemblyFoldersConditions  
    member x.platform  = data.platform
    member x.prefer32Bit = data.prefer32Bit
    member x.useMonoResolution  = data.useMonoResolution
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
    member x.targetFrameworkVersionMajorMinor = targetFrameworkVersionValue
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
    member x.version  = data.version
    member x.metadataVersion = data.metadataVersion
    member x.standalone  = data.standalone
    member x.extraStaticLinkRoots  = data.extraStaticLinkRoots
    member x.noSignatureData  = data.noSignatureData
    member x.onlyEssentialOptimizationData  = data.onlyEssentialOptimizationData
    member x.useOptimizationDataFile  = data.useOptimizationDataFile
    member x.useSignatureDataFile = data.useSignatureDataFile
    member x.jitTracking  = data.jitTracking
    member x.ignoreSymbolStoreSequencePoints  = data.ignoreSymbolStoreSequencePoints
    member x.internConstantStrings  = data.internConstantStrings
    member x.extraOptimizationIterations  = data.extraOptimizationIterations
    member x.win32res  = data.win32res
    member x.win32manifest = data.win32manifest
    member x.includewin32manifest = data.includewin32manifest
    member x.linkResources  = data.linkResources
    member x.showFullPaths  = data.showFullPaths
    member x.errorStyle  = data.errorStyle
    member x.validateTypeProviders  = data.validateTypeProviders
    member x.utf8output  = data.utf8output
    member x.flatErrors = data.flatErrors
    member x.maxErrors  = data.maxErrors
    member x.baseAddress  = data.baseAddress
 #if DEBUG
    member x.writeGeneratedILFiles  = data.writeGeneratedILFiles
    member x.showOptimizationData  = data.showOptimizationData
#endif
    member x.showTerms      = data.showTerms
    member x.writeTermsToFiles  = data.writeTermsToFiles
    member x.doDetuple      = data.doDetuple
    member x.doTLR          = data.doTLR
    member x.doFinalSimplify = data.doFinalSimplify
    member x.optSettings    = data.optSettings
    member x.emitTailcalls = data.emitTailcalls
    member x.lcid           = data.lcid
    member x.optsOn         = data.optsOn
    member x.productNameForBannerText  = data.productNameForBannerText
    member x.showBanner   = data.showBanner
    member x.showTimes  = data.showTimes
    member x.showLoadedAssemblies = data.showLoadedAssemblies
    member x.continueAfterParseFailure = data.continueAfterParseFailure
#if EXTENSIONTYPING
    member x.showExtensionTypeMessages  = data.showExtensionTypeMessages    
#endif
    member x.pause  = data.pause
    member x.indirectCallArrayMethods = data.indirectCallArrayMethods
    member x.alwaysCallVirt = data.alwaysCallVirt
    member x.noDebugData = data.noDebugData
    member x.isInteractive = data.isInteractive
    member x.isInvalidationSupported = data.isInvalidationSupported
    member x.emitDebugInfoInQuotations = data.emitDebugInfoInQuotations
    member x.sqmSessionGuid = data.sqmSessionGuid
    member x.sqmNumOfSourceFiles = data.sqmNumOfSourceFiles
    member x.sqmSessionStartedTime = data.sqmSessionStartedTime
    member x.shadowCopyReferences = data.shadowCopyReferences

    static member Create(builder,validate) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        TcConfig(builder,validate)

    member tcConfig.CloneOfOriginalBuilder = 
        { data with conditionalCompilationDefines=data.conditionalCompilationDefines }

    member tcConfig.ComputeCanContainEntryPoint(sourceFiles:string list) = 
        let n = sourceFiles.Length in 
        sourceFiles |> List.mapi (fun i _ -> (i = n-1) && tcConfig.target.IsExe)
            
    // This call can fail if no CLR is found (this is the path to mscorlib)
    member tcConfig.ClrRoot = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        match tcConfig.clrRoot with 
        | Some x -> 
            [tcConfig.MakePathAbsolute x]
        | None -> 
            // When running on Mono we lead everyone to believe we're doing .NET 2.0 compilation 
            // by default. 
            if runningOnMono then 
                let mono10SysDir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() 
                assert(mono10SysDir.EndsWith("1.0",StringComparison.Ordinal));
                let mono20SysDir = Path.Combine(Path.GetDirectoryName mono10SysDir, "2.0")
                if Directory.Exists(mono20SysDir) then
                     [mono20SysDir]
                else 
                     [mono10SysDir]
            else                                
                try 
                    match tcConfig.resolutionEnvironment with
                    | MSBuildResolver.RuntimeLike ->
                        [System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()] 
                    | _ -> 
                        let frameworkRoot = MSBuildResolver.DotNetFrameworkReferenceAssembliesRootDirectory
                        let frameworkRootVersion = Path.Combine(frameworkRoot,tcConfig.targetFrameworkVersionMajorMinor)
                        [frameworkRootVersion]
                with e -> 
                    errorRecovery e range0; [] 

    member tcConfig.ComputeLightSyntaxInitialStatus filename = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        let lower = String.lowercase filename
        let lightOnByDefault = List.exists (Filename.checkSuffix lower) lightSyntaxDefaultExtensions
        if lightOnByDefault then (tcConfig.light <> Some(false)) else (tcConfig.light = Some(true) )

    member tcConfig.GetAvailableLoadedSources() =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        let resolveLoadedSource (m,path) =
            try
                if not(FileSystem.SafeExists(path)) then 
                    error(LoadedSourceNotFoundIgnoring(path,m))                         
                    None
                else Some(m,path)
            with e -> errorRecovery e m; None
        tcConfig.loadedSources 
        |> List.map resolveLoadedSource 
        |> List.filter Option.isSome 
        |> List.map Option.get                
        |> Seq.distinct
        |> Seq.toList        

    /// A closed set of assemblies where, for any subset S:
    ///    -  the TcImports object built for S (and thus the F# Compiler CCUs for the assemblies in S) 
    ///       is a resource that can be shared between any two IncrementalBuild objects that reference
    ///       precisely S
    ///
    /// Determined by looking at the set of assemblies in the framework assemblies directory, plus the 
    /// F# core library.
    ///
    /// Returning true may mean that the file is locked and/or placed into the
    /// 'framework' reference set that is potentially shared across multiple compilations.
    member tcConfig.IsSystemAssembly (filename:string) =  
        try 
            FileSystem.SafeExists filename && 
            ((tcConfig.ClrRoot |> List.exists (fun clrRoot -> clrRoot = Path.GetDirectoryName filename)) ||
             (systemAssemblies |> List.exists (fun sysFile -> sysFile = fileNameWithoutExtension filename)))
        with _ ->
            false    
        
    // This is not the complete set of search paths, it is just the set 
    // that is special to F# (as compared to MSBuild resolution)
    member tcConfig.SearchPathsForLibraryFiles = 
        [ yield! tcConfig.ClrRoot 
          yield! List.map (tcConfig.MakePathAbsolute) tcConfig.includes
          yield tcConfig.implicitIncludeDir 
          yield tcConfig.fsharpBinariesDir ]

    member tcConfig.MakePathAbsolute path = 
        let result = ComputeMakePathAbsolute tcConfig.implicitIncludeDir path
#if TRACK_DOWN_EXTRA_BACKSLASHES        
        System.Diagnostics.Debug.Assert(not(result.Contains(@"\\")), "tcConfig.MakePathAbsolute results in a non-canonical filename with extra backslashes: "+result)
#endif
        result
        
    member tcConfig.TryResolveLibWithDirectories (AssemblyReference (m,nm) as r) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        // Only want to resolve certain extensions (otherwise, 'System.Xml' is ambiguous).
        // MSBuild resolution is limitted to .exe and .dll so do the same here.
        let ext = System.IO.Path.GetExtension(nm)
        let isNetModule = String.Compare(ext,".netmodule",StringComparison.OrdinalIgnoreCase)=0 
        if String.Compare(ext,".dll",StringComparison.OrdinalIgnoreCase)=0 
           || String.Compare(ext,".exe",StringComparison.OrdinalIgnoreCase)=0 
           || isNetModule then 

            let resolved = TryResolveFileUsingPaths(tcConfig.SearchPathsForLibraryFiles,m,nm)
            match resolved with 
            | Some(resolved) -> 
                let sysdir = tcConfig.IsSystemAssembly resolved
                let fusionName = 
                    if isNetModule then ""
                    else 
                        try
                            let readerSettings : ILBinaryReader.ILReaderOptions = {pdbPath=None;ilGlobals = EcmaILGlobals;optimizeForMemory=false}
                            let reader = ILBinaryReader.OpenILModuleReader resolved readerSettings
                            try
                                let assRef = mkRefToILAssembly reader.ILModuleDef.ManifestOfAssembly
                                assRef.QualifiedName
                            finally 
                                ILBinaryReader.CloseILModuleReader reader
                        with e ->
                            ""
                Some
                    { originalReference = r;
                      resolvedPath = resolved;
                      resolvedFrom = Unknown;
                      fusionName = fusionName;
                      redist = null;
                      sysdir = sysdir;
                      ilAssemblyRef = ref None }
            | None -> None
        else None
                
    member tcConfig.ResolveLibWithDirectories ccuLoadFaulureAction (AssemblyReference (m,nm)) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        // test for both libraries and executables
        let ext = System.IO.Path.GetExtension(nm)
        let isExe = (String.Compare(ext,".exe",StringComparison.OrdinalIgnoreCase) = 0)
        let isDLL = (String.Compare(ext,".dll",StringComparison.OrdinalIgnoreCase) = 0)
        let isNetModule = (String.Compare(ext,".netmodule",StringComparison.OrdinalIgnoreCase) = 0)

        let nms = 
            if isExe || isDLL || isNetModule then
                [nm]
            else
                [nm+".dll";nm+".exe";nm+".netmodule"]

        match (List.tryPick (fun nm -> tcConfig.TryResolveLibWithDirectories(AssemblyReference(m,nm))) nms) with
        | Some(res) -> Some res
        | None ->
            match ccuLoadFaulureAction with
            | CcuLoadFailureAction.RaiseError ->
                let searchMessage = String.concat "\n " tcConfig.SearchPathsForLibraryFiles
                raise (FileNameNotResolved(nm,searchMessage,m))
            | CcuLoadFailureAction.ReturnNone -> None

    member tcConfig.ResolveSourceFile(m,nm,pathLoadedFrom) = 
        data.ResolveSourceFile(m,nm,pathLoadedFrom)

    member tcConfig.CheckFSharpBinary (filename,ilAssemblyRefs,m) = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
        checkFSharpBinaryCompatWithMscorlib filename ilAssemblyRefs None m

    // NOTE!! if mode=Speculative then this method must not report ANY warnings or errors through 'warning' or 'error'. Instead
    // it must return warnings and errors as data
    //
    // NOTE!! if mode=ReportErrors then this method must not raise exceptions. It must just report the errors and recover
    static member TryResolveLibsUsingMSBuildRules (tcConfig:TcConfig,originalReferences:AssemblyReference list, errorAndWarningRange:range, mode:ResolveAssemblyReferenceMode) : AssemblyResolution list * UnresolvedAssemblyReference list =
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
    
        if tcConfig.useMonoResolution then
            failwith "MSBuild resolution is not supported."
            
        if originalReferences=[] then [],[]
        else            
            // Group references by name with range values in the grouped value list.
            // In the grouped reference, store the index of the last use of the reference.
            let groupedReferences = 
                originalReferences
                |> List.mapi (fun index reference -> (index, reference))
                |> Seq.groupBy(fun (_, reference) -> reference.Text)
                |> Seq.map(fun (assemblyName,assemblyAndIndexGroup)->
                    let assemblyAndIndexGroup = assemblyAndIndexGroup |> List.ofSeq
                    let highestPosition = assemblyAndIndexGroup |> List.maxBy fst |> fst
                    let assemblyGroup = assemblyAndIndexGroup |> List.map snd
                    assemblyName, highestPosition, assemblyGroup)
                |> Array.ofSeq

            let logmessage showMessages  = 
                if showMessages && tcConfig.showReferenceResolutions then (fun (message:string)->dprintf "%s\n" message)
                else ignore
            let logwarning showMessages = 
                (fun code message->
                    if showMessages && mode = ReportErrors then 
                        match code with 
                        // These are warnings that mean 'not resolved' for some assembly.
                        // Note that we don't get to know the name of the assembly that couldn't be resolved.
                        // Ignore these and rely on the logic below to emit an error for each unresolved reference.
                        | "MSB3246" // Resolved file has a bad image, no metadata, or is otherwise inaccessible.
                        | "MSB3106"  
                            -> ()
                        | _ -> 
                            (if code = "MSB3245" then errorR else warning)
                                (MSBuildReferenceResolutionWarning(code,message,errorAndWarningRange)))
            let logerror showMessages = 
                (fun code message ->
                    if showMessages && mode = ReportErrors then 
                        errorR(MSBuildReferenceResolutionError(code,message,errorAndWarningRange)))

            let targetFrameworkMajorMinor = tcConfig.targetFrameworkVersionMajorMinor

#if DEBUG
            assert(MSBuildResolver.SupportedNetFrameworkVersions.Contains targetFrameworkMajorMinor) // Resolve is flexible, but pinning down targetFrameworkMajorMinor.
#endif

            let targetProcessorArchitecture = 
                    match tcConfig.platform with
                    | None -> "MSIL"
                    | Some(X86) -> "x86"
                    | Some(AMD64) -> "amd64"
                    | Some(IA64) -> "ia64"
            let outputDirectory = 
                match tcConfig.outputFile with 
                | Some(outputFile) -> tcConfig.MakePathAbsolute outputFile
                | None -> tcConfig.implicitIncludeDir
            let targetFrameworkDirectories =
                match tcConfig.clrRoot with
                | Some(clrRoot) -> [tcConfig.MakePathAbsolute clrRoot]
                | None -> []
                             
            // First, try to resolve everything as a file using simple resolution
            let resolvedAsFile = 
                groupedReferences 
                |>Array.map(fun (_filename,maxIndexOfReference,references)->
                                let assemblyResolution = references 
                                                         |> List.map tcConfig.TryResolveLibWithDirectories
                                                         |> List.filter Option.isSome
                                                         |> List.map Option.get
                                (maxIndexOfReference, assemblyResolution))  
                |> Array.filter(fun (_,refs)->refs|>List.isEmpty|>not)
                
                                       
            // Whatever is left, pass to MSBuild.
            let Resolve(references,showMessages) = 
                try 
                    MSBuildResolver.Resolve
                       (tcConfig.resolutionEnvironment,
                        references,
                        targetFrameworkMajorMinor,   // TargetFrameworkVersionMajorMinor
                        targetFrameworkDirectories,  // TargetFrameworkDirectories 
                        targetProcessorArchitecture, // TargetProcessorArchitecture
                        Path.GetDirectoryName(outputDirectory), // Output directory
                        tcConfig.fsharpBinariesDir, // FSharp binaries directory
                        tcConfig.includes, // Explicit include directories
                        tcConfig.implicitIncludeDir, // Implicit include directory (likely the project directory)
                        tcConfig.resolutionFrameworkRegistryBase, 
                        tcConfig.resolutionAssemblyFoldersSuffix, 
                        tcConfig.resolutionAssemblyFoldersConditions, 
                        logmessage showMessages, logwarning showMessages, logerror showMessages)
                with 
                    MSBuildResolver.ResolutionFailure -> error(Error(FSComp.SR.buildAssemblyResolutionFailed(),errorAndWarningRange))
            
            let toMsBuild = [|0..groupedReferences.Length-1|] 
                             |> Array.map(fun i->(p13 groupedReferences.[i]),(p23 groupedReferences.[i]),i) 
                             |> Array.filter (fun (_,i0,_)->resolvedAsFile|>Array.exists(fun (i1,_) -> i0=i1)|>not)
                             |> Array.map(fun (ref,_,i)->ref,string i)
            let resolutions = Resolve(toMsBuild,(*showMessages*)true)  

            // Map back to original assembly resolutions.
            let resolvedByMsbuild = 
                resolutions.resolvedFiles
                    |> Array.map(fun resolvedFile -> 
                                    let i = int resolvedFile.baggage
                                    let _,maxIndexOfReference,ms = groupedReferences.[i]
                                    let assemblyResolutions =
                                        ms|>List.map(fun originalReference ->
                                                    System.Diagnostics.Debug.Assert(FileSystem.IsPathRootedShim(resolvedFile.itemSpec), sprintf "msbuild-resolved path is not absolute: '%s'" resolvedFile.itemSpec)
                                                    let canonicalItemSpec = FileSystem.GetFullPathShim(resolvedFile.itemSpec)
                                                    {originalReference=originalReference; 
                                                     resolvedPath=canonicalItemSpec; 
                                                     resolvedFrom=resolvedFile.resolvedFrom;
                                                     fusionName=resolvedFile.fusionName
                                                     redist=resolvedFile.redist;
                                                     sysdir=tcConfig.IsSystemAssembly canonicalItemSpec;
                                                     ilAssemblyRef = ref None})
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
                    Resolve([|originalName,""|],(*showMessages*)false).resolvedFiles.Length<>0 
                    
            let unresolvedReferences =                     
                    groupedReferences 
                    //|> Array.filter(p13 >> IsNotFileOrIsAssembly)
                    |> Array.filter(p13 >> IsResolved >> not)   
                    |> List.ofArray                 

            // If mode=Speculative, then we haven't reported any errors.
            // We report the error condition by returning an empty list of resolutions
            if mode = Speculative && (List.length unresolvedReferences) > 0 then 
                [],(List.ofArray groupedReferences) |> List.map (fun (name, _, r) -> (name, r)) |> List.map UnresolvedAssemblyReference
            else 
                resultingResolutions,unresolvedReferences |> List.map (fun (name, _, r) -> (name, r)) |> List.map UnresolvedAssemblyReference    


    member tcConfig.PrimaryAssemblyDllReference() = primaryAssemblyReference
    member tcConfig.GetPrimaryAssemblyCcuInitializer() = primaryAssemblyCcuInitializer
    member tcConfig.CoreLibraryDllReference() = fslibReference
               

let warningMem n l = List.mem n l

let ReportWarning (globalWarnLevel : int) (specificWarnOff : int list) (specificWarnOn : int list) err = 
    let n = GetErrorNumber err
    warningOn err globalWarnLevel specificWarnOn && not (warningMem n specificWarnOff)

let ReportWarningAsError (globalWarnLevel : int) (specificWarnOff : int list) (specificWarnOn : int list) (specificWarnAsError : int list) (specificWarnAsWarn : int list) (globalWarnAsError : bool) err =
    (warningOn err globalWarnLevel specificWarnOn) &&
    not(warningMem (GetErrorNumber err) specificWarnAsWarn) &&
    ((globalWarnAsError && not (warningMem (GetErrorNumber err) specificWarnOff)) ||
     warningMem (GetErrorNumber err) specificWarnAsError)

//----------------------------------------------------------------------------
// Scoped #nowarn pragmas


let GetScopedPragmasForHashDirective hd = 
    [ match hd with 
      | ParsedHashDirective("nowarn",numbers,m) ->
          for s in numbers do
          match GetWarningNumber(m,s) with 
            | None -> ()
            | Some n -> yield ScopedPragma.WarningOff(m,n) 
      | _ -> () ]


let GetScopedPragmasForInput input = 

    match input with 
    | ParsedInput.SigFile (ParsedSigFileInput(_,_,pragmas,_,_)) -> pragmas
    | ParsedInput.ImplFile (ParsedImplFileInput(_,_,_,pragmas,_,_,_)) ->pragmas



/// Build an ErrorLogger that delegates to another ErrorLogger but filters warnings turned off by the given pragma declarations
//
// NOTE: we allow a flag to turn of strict file checking. This is because file names sometimes don't match due to use of 
// #line directives, e.g. for pars.fs/pars.fsy. In this case we just test by line number - in most cases this is sufficent
// because we install a filtering error handler on a file-by-file basis for parsing and type-checking.
// However this is indicative of a more systematic problem where source-line 
// sensitive operations (lexfilter and warning filtering) do not always
// interact well with #line directives.
type ErrorLoggerFilteringByScopedPragmas (checkFile,scopedPragmas,errorLogger:ErrorLogger) =
    inherit ErrorLogger("ErrorLoggerFilteringByScopedPragmas")
    let mutable scopedPragmas = scopedPragmas
    member x.ScopedPragmas with set v = scopedPragmas <- v
    override x.ErrorSinkImpl err = errorLogger.ErrorSink err
    override x.ErrorCount = errorLogger.ErrorCount
    override x.WarnSinkImpl err = 
        let report = 
            let warningNum = GetErrorNumber err
            match RangeOfError err with 
            | Some m -> 
                not (scopedPragmas |> List.exists (fun pragma ->
                    match pragma with 
                    | ScopedPragma.WarningOff(pragmaRange,warningNumFromPragma) -> 
                        warningNum = warningNumFromPragma && 
                        (not checkFile || m.FileIndex = pragmaRange.FileIndex) &&
                        Range.posGeq m.Start pragmaRange.Start))  
            | None -> true
        if report then errorLogger.WarnSink(err);
    override x.ErrorNumbers = errorLogger.ErrorNumbers
    override x.WarningNumbers = errorLogger.WarningNumbers

let GetErrorLoggerFilteringByScopedPragmas(checkFile,scopedPragmas,errorLogger) = 
    (ErrorLoggerFilteringByScopedPragmas(checkFile,scopedPragmas,errorLogger) :> ErrorLogger)

/// Build an ErrorLogger that delegates to another ErrorLogger but filters warnings turned off by the given pragma declarations
type DelayedErrorLogger(errorLogger:ErrorLogger) =
    inherit ErrorLogger("DelayedErrorLogger")
    let delayed = new ResizeArray<_>()
    override x.ErrorSinkImpl err = delayed.Add (err,true)
    override x.ErrorCount = delayed |> Seq.filter snd |> Seq.length
    override x.WarnSinkImpl err = delayed.Add(err,false)
    member x.CommitDelayedErrorsAndWarnings() = 
        // Eagerly grab all the errors and warnings from the mutable collection
        let errors = delayed |> Seq.toList
        // Now report them
        for (err,isError) in errors do
            if isError then errorLogger.ErrorSink err else errorLogger.WarnSink err


//----------------------------------------------------------------------------
// Parsing
//--------------------------------------------------------------------------


let CanonicalizeFilename filename = 
    let basic = fileNameOfPath filename
    String.capitalize (try Filename.chopExtension basic with _ -> basic)

let IsScript filename = 
    let lower = String.lowercase filename 
    scriptSuffixes |> List.exists (Filename.checkSuffix lower)
    
// Give a unique name to the different kinds of inputs. Used to correlate signature and implementation files
//   QualFileNameOfModuleName - files with a single module declaration or an anonymous module
let QualFileNameOfModuleName m filename modname = QualifiedNameOfFile(mkSynId m (textOfLid modname + (if IsScript filename then "$fsx" else "")))
let QualFileNameOfFilename m filename = QualifiedNameOfFile(mkSynId m (CanonicalizeFilename filename + (if IsScript filename then "$fsx" else "")))

// Interactive fragments
let QualFileNameOfUniquePath (m, p: string list) = QualifiedNameOfFile(mkSynId m (String.concat "_" p))

let QualFileNameOfSpecs filename specs = 
    match specs with 
    | [SynModuleOrNamespaceSig(modname,true,_,_,_,_,m)] -> QualFileNameOfModuleName m filename modname
    | _ -> QualFileNameOfFilename (rangeN filename 1) filename

let QualFileNameOfImpls filename specs = 
    match specs with 
    | [SynModuleOrNamespace(modname,true,_,_,_,_,m)] -> QualFileNameOfModuleName m filename modname
    | _ -> QualFileNameOfFilename (rangeN filename 1) filename

let PrepandPathToQualFileName x (QualifiedNameOfFile(q)) = QualFileNameOfUniquePath (q.idRange,pathOfLid x@[q.idText])
let PrepandPathToImpl x (SynModuleOrNamespace(p,c,d,e,f,g,h)) = SynModuleOrNamespace(x@p,c,d,e,f,g,h)
let PrepandPathToSpec x (SynModuleOrNamespaceSig(p,c,d,e,f,g,h)) = SynModuleOrNamespaceSig(x@p,c,d,e,f,g,h)

let PrependPathToInput x inp = 
    match inp with 
    | ParsedInput.ImplFile (ParsedImplFileInput(b,c,q,d,hd,impls,e)) -> ParsedInput.ImplFile (ParsedImplFileInput(b,c,PrepandPathToQualFileName x q,d,hd,List.map (PrepandPathToImpl x) impls,e))
    | ParsedInput.SigFile (ParsedSigFileInput(b,q,d,hd,specs)) -> ParsedInput.SigFile(ParsedSigFileInput(b,PrepandPathToQualFileName x q,d,hd,List.map (PrepandPathToSpec x) specs))

let ComputeAnonModuleName check defaultNamespace filename (m: range) = 
    let modname = CanonicalizeFilename filename
    if check && not (modname |> String.forall (fun c -> System.Char.IsLetterOrDigit(c) || c = '_')) then
          if not (filename.EndsWith("fsx",StringComparison.OrdinalIgnoreCase) || filename.EndsWith("fsscript",StringComparison.OrdinalIgnoreCase)) then
              warning(Error(FSComp.SR.buildImplicitModuleIsNotLegalIdentifier(modname,(fileNameOfPath filename)),m))
    let combined = 
      match defaultNamespace with 
      | None -> modname
      | Some ns -> textOfPath [ns;modname]

    let anonymousModuleNameRange  =
        let filename = m.FileName
        mkRange filename pos0 pos0
    pathToSynLid anonymousModuleNameRange (splitNamespace combined)

let PostParseModuleImpl (_i,defaultNamespace,isLastCompiland,filename,impl) = 
    match impl with 
    | ParsedImplFileFragment.NamedModule(SynModuleOrNamespace(lid,isModule,decls,xmlDoc,attribs,access,m)) -> 
        let lid = 
            match lid with 
            | [id] when isModule && id.idText = MangledGlobalName -> error(Error(FSComp.SR.buildInvalidModuleOrNamespaceName(),id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespace(lid,isModule,decls,xmlDoc,attribs,access,m)

    | ParsedImplFileFragment.AnonModule (defs,m)-> 
        if not isLastCompiland && not (doNotRequireNamespaceOrModuleSuffixes |> List.exists (Filename.checkSuffix (String.lowercase filename))) then 
            errorR(Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule(),trimRangeToLine m))
        let modname = ComputeAnonModuleName (nonNil defs) defaultNamespace filename (trimRangeToLine m)
        SynModuleOrNamespace(modname,true,defs,PreXmlDoc.Empty,[],None,m)

    | ParsedImplFileFragment.NamespaceFragment (lid,b,c,d,e,m)-> 
        let lid = 
            match lid with 
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespace(lid,b,c,d,e,None,m)

let PostParseModuleSpec (_i,defaultNamespace,isLastCompiland,filename,intf) = 
    match intf with 
    | ParsedSigFileFragment.NamedModule(SynModuleOrNamespaceSig(lid,isModule,decls,xmlDoc,attribs,access,m)) -> 
        let lid = 
            match lid with 
            | [id] when isModule && id.idText = MangledGlobalName -> error(Error(FSComp.SR.buildInvalidModuleOrNamespaceName(),id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespaceSig(lid,isModule,decls,xmlDoc,attribs,access,m)

    | ParsedSigFileFragment.AnonModule (defs,m) -> 
        if not isLastCompiland && not (doNotRequireNamespaceOrModuleSuffixes |> List.exists (Filename.checkSuffix (String.lowercase filename))) then 
            errorR(Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule(),m))
        let modname = ComputeAnonModuleName (nonNil defs) defaultNamespace filename (trimRangeToLine m)
        SynModuleOrNamespaceSig(modname,true,defs,PreXmlDoc.Empty,[],None,m)

    | ParsedSigFileFragment.NamespaceFragment (lid,b,c,d,e,m)-> 
        let lid = 
            match lid with 
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid
        SynModuleOrNamespaceSig(lid,b,c,d,e,None,m)



let PostParseModuleImpls (defaultNamespace,filename,isLastCompiland,ParsedImplFile(hashDirectives,impls)) = 
    match impls |> List.rev |> List.tryPick (function ParsedImplFileFragment.NamedModule(SynModuleOrNamespace(lid,_,_,_,_,_,_)) -> Some(lid) | _ -> None) with
    | Some lid when impls.Length > 1 -> 
        errorR(Error(FSComp.SR.buildMultipleToplevelModules(),rangeOfLid lid))
    | _ -> 
        ()
    let impls = impls |> List.mapi (fun i x -> PostParseModuleImpl (i, defaultNamespace, isLastCompiland, filename, x)) 
    let qualName = QualFileNameOfImpls filename impls
    let isScript = IsScript filename

    let scopedPragmas = 
        [ for (SynModuleOrNamespace(_,_,decls,_,_,_,_)) in impls do 
            for d in decls do
                match d with 
                | SynModuleDecl.HashDirective (hd,_) -> yield! GetScopedPragmasForHashDirective hd
                | _ -> () 
          for hd in hashDirectives do 
              yield! GetScopedPragmasForHashDirective hd ]
    ParsedInput.ImplFile(ParsedImplFileInput(filename,isScript,qualName,scopedPragmas,hashDirectives,impls,isLastCompiland))
  
let PostParseModuleSpecs (defaultNamespace,filename,isLastCompiland,ParsedSigFile(hashDirectives,specs)) = 
    match specs |> List.rev |> List.tryPick (function ParsedSigFileFragment.NamedModule(SynModuleOrNamespaceSig(lid,_,_,_,_,_,_)) -> Some(lid) | _ -> None) with
    | Some  lid when specs.Length > 1 -> 
        errorR(Error(FSComp.SR.buildMultipleToplevelModules(),rangeOfLid lid))
    | _ -> 
        ()
        
    let specs = specs |> List.mapi (fun i x -> PostParseModuleSpec(i,defaultNamespace,isLastCompiland,filename,x)) 
    let qualName = QualFileNameOfSpecs filename specs
    let scopedPragmas = 
        [ for (SynModuleOrNamespaceSig(_,_,decls,_,_,_,_)) in specs do 
            for d in decls do
                match d with 
                | SynModuleSigDecl.HashDirective(hd,_) -> yield! GetScopedPragmasForHashDirective hd
                | _ -> () 
          for hd in hashDirectives do 
              yield! GetScopedPragmasForHashDirective hd ]

    ParsedInput.SigFile(ParsedSigFileInput(filename,qualName,scopedPragmas,hashDirectives,specs))

let ParseInput (lexer,errorLogger:ErrorLogger,lexbuf:UnicodeLexing.Lexbuf,defaultNamespace,filename,isLastCompiland) = 
    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy filename
    //  - if you have a #line directive, e.g. 
    //        # 1000 "Line01.fs"
    //    then it also asserts.  But these are edge cases that can be fixed later, e.g. in bug 4651.
    //System.Diagnostics.Debug.Assert(System.IO.Path.IsPathRooted(filename), sprintf "should be absolute: '%s'" filename)
    let lower = String.lowercase filename 
    // Delay sending errors and warnings until after the file is parsed. This gives us a chance to scrape the
    // #nowarn declarations for the file
    let filteringErrorLogger = ErrorLoggerFilteringByScopedPragmas(false,[],errorLogger)
    let errorLogger = DelayedErrorLogger(filteringErrorLogger)
    use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
    use unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)
    try     
        let input = 
            if mlCompatSuffixes |> List.exists (Filename.checkSuffix lower)   then  
                mlCompatWarning (FSComp.SR.buildCompilingExtensionIsForML()) rangeStartup; 

            if implSuffixes |> List.exists (Filename.checkSuffix lower)   then  
                let impl = Parser.implementationFile lexer lexbuf 
                PostParseModuleImpls (defaultNamespace,filename,isLastCompiland,impl)
            elif sigSuffixes |> List.exists (Filename.checkSuffix lower)  then  
                let intfs = Parser.signatureFile lexer lexbuf 
                PostParseModuleSpecs (defaultNamespace,filename,isLastCompiland,intfs)
            else 
                errorLogger.Error(InternalError(FSComp.SR.buildUnknownFileSuffix(filename),Range.rangeStartup))
        filteringErrorLogger.ScopedPragmas <- GetScopedPragmasForInput input
        input
    finally
        // OK, now commit the errors, since the ScopedPragmas will (hopefully) have been scraped
        errorLogger.CommitDelayedErrorsAndWarnings()
    (* unwindEL, unwindBP dispose *)

//----------------------------------------------------------------------------
// parsing - ParseOneInputFile
// Filename is (ml/mli/fs/fsi source). Parse it to AST. 
//----------------------------------------------------------------------------
let ParseOneInputLexbuf (tcConfig:TcConfig,lexResourceManager,conditionalCompilationDefines,lexbuf,filename,isLastCompiland,errorLogger) =
    use unwindbuildphase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)
    try 
        let skip = true in (* don't report whitespace from lexer *)
        let lightSyntaxStatus = LightSyntaxStatus (tcConfig.ComputeLightSyntaxInitialStatus filename,true) 
        let lexargs = mkLexargs (filename,conditionalCompilationDefines@tcConfig.conditionalCompilationDefines,lightSyntaxStatus,lexResourceManager, ref [],errorLogger)
        let shortFilename = SanitizeFileName filename tcConfig.implicitIncludeDir 
        let input = 
            Lexhelp.usingLexbufForParsing (lexbuf,filename) (fun lexbuf ->
                if verbose then dprintn ("Parsing... "+shortFilename);
                let tokenizer = Lexfilter.LexFilter(lightSyntaxStatus, tcConfig.compilingFslib, Lexer.token lexargs skip, lexbuf)

                if tcConfig.tokenizeOnly then 
                    while true do 
                        printf "tokenize - getting one token from %s\n" shortFilename;
                        let t = tokenizer.Lexer lexbuf
                        printf "tokenize - got %s @ %a\n" (Parser.token_to_string t) outputRange lexbuf.LexemeRange;
                        (match t with Parser.EOF _ -> exit 0 | _ -> ());
                        if lexbuf.IsPastEndOfStream then  printf "!!! at end of stream\n"

                if tcConfig.testInteractionParser then 
                    while true do 
                        match (Parser.interaction tokenizer.Lexer lexbuf) with
                        | IDefns(l,m) -> dprintf "Parsed OK, got %d defs @ %a\n" l.Length outputRange m;
                        | IHash (_,m) -> dprintf "Parsed OK, got hash @ %a\n" outputRange m;
                    exit 0;

                let res = ParseInput(tokenizer.Lexer,errorLogger,lexbuf,None,filename,isLastCompiland)

                if tcConfig.reportNumDecls then 
                    let rec flattenSpecs specs = 
                          specs |> List.collect (function (SynModuleSigDecl.NestedModule (_,subDecls,_)) -> flattenSpecs subDecls | spec -> [spec])
                    let rec flattenDefns specs = 
                          specs |> List.collect (function (SynModuleDecl.NestedModule (_,subDecls,_,_)) -> flattenDefns subDecls | defn -> [defn])

                    let flattenModSpec (SynModuleOrNamespaceSig(_,_,decls,_,_,_,_)) = flattenSpecs decls
                    let flattenModImpl (SynModuleOrNamespace(_,_,decls,_,_,_,_)) = flattenDefns decls
                    match res with 
                    | ParsedInput.SigFile(ParsedSigFileInput(_,_,_,_,specs)) -> 
                        dprintf "parsing yielded %d specs" (List.collect flattenModSpec specs).Length
                    | ParsedInput.ImplFile(ParsedImplFileInput(_,_,_,_,_,impls,_)) -> 
                        dprintf "parsing yielded %d definitions" (List.collect flattenModImpl impls).Length
                res
            )
        if verbose then dprintn ("Parsed "+shortFilename);
        Some input 
    with e -> (* errorR(Failure("parse failed")); *) errorRecovery e rangeStartup; None 
            
            
let ParseOneInputFile (tcConfig:TcConfig,lexResourceManager,conditionalCompilationDefines,filename,isLastCompiland,errorLogger,retryLocked) =
    try 
       let lower = String.lowercase filename
       if List.exists (Filename.checkSuffix lower) (sigSuffixes@implSuffixes)  then  
            if not(FileSystem.SafeExists(filename)) then
                error(Error(FSComp.SR.buildCouldNotFindSourceFile(filename),rangeStartup))
            // bug 3155: if the file name is indirect, use a full path
            let lexbuf = UnicodeLexing.UnicodeFileAsLexbuf(filename,tcConfig.inputCodePage,retryLocked) 
            ParseOneInputLexbuf(tcConfig,lexResourceManager,conditionalCompilationDefines,lexbuf,filename,isLastCompiland,errorLogger)
       else error(Error(FSComp.SR.buildInvalidSourceFileExtension(SanitizeFileName filename tcConfig.implicitIncludeDir),rangeStartup))
    with e -> (* errorR(Failure("parse failed")); *) errorRecovery e rangeStartup; None 
     

[<Sealed>] 
type TcAssemblyResolutions(results : AssemblyResolution list, unresolved : UnresolvedAssemblyReference list) = 

    let originalReferenceToResolution = results |> List.map (fun r -> r.originalReference.Text,r) |> Map.ofList
    let resolvedPathToResolution      = results |> List.map (fun r -> r.resolvedPath,r) |> Map.ofList

    /// Add some resolutions to the map of resolution results.                
    member tcResolutions.AddResolutionResults(newResults) = TcAssemblyResolutions(newResults @ results, unresolved)
    /// Add some unresolved results.
    member tcResolutions.AddUnresolvedReferences(newUnresolved) = TcAssemblyResolutions(results, newUnresolved @ unresolved)

    /// Get information about referenced DLLs
    member tcResolutions.GetAssemblyResolutions() = results
    member tcResolutions.GetUnresolvedReferences() = unresolved
    member tcResolutions.TryFindByOriginalReference(assemblyReference:AssemblyReference) = originalReferenceToResolution.TryFind assemblyReference.Text
    member tcResolution.TryFindByExactILAssemblyRef assref = results |> List.tryFind (fun ar->ar.ILAssemblyRef = assref)
    member tcResolutions.TryFindByResolvedPath nm = resolvedPathToResolution.TryFind nm
    member tcResolutions.TryFindByOriginalReferenceText nm = originalReferenceToResolution.TryFind nm
        
    static member Resolve (tcConfig:TcConfig,assemblyList:AssemblyReference list, knownUnresolved:UnresolvedAssemblyReference list) : TcAssemblyResolutions =
        let resolved,unresolved = 
            if tcConfig.useMonoResolution then 
                assemblyList |> List.map ((tcConfig.ResolveLibWithDirectories CcuLoadFailureAction.RaiseError) >> Option.get), []
            else
                TcConfig.TryResolveLibsUsingMSBuildRules (tcConfig,assemblyList,rangeStartup,ReportErrors)
        TcAssemblyResolutions(resolved,unresolved @ knownUnresolved)                    


    static member GetAllDllReferences (tcConfig:TcConfig) =
        [ yield tcConfig.PrimaryAssemblyDllReference()
          if not tcConfig.compilingFslib then 
              yield tcConfig.CoreLibraryDllReference()

          if tcConfig.framework then 
              for s in DefaultBasicReferencesForOutOfProjectSources do 
                  yield AssemblyReference(rangeStartup,s+".dll")

          if tcConfig.framework || tcConfig.addVersionSpecificFrameworkReferences then 
              // For out-of-project context, then always reference some extra DLLs on .NET 4.0 
              if tcConfig.MscorlibMajorVersion >= 4 then 
                  for s in DefaultBasicReferencesForOutOfProjectSources40 do 
                      yield AssemblyReference(rangeStartup,s+".dll") 

          if tcConfig.useFsiAuxLib then 
              let name = Path.Combine(tcConfig.fsharpBinariesDir, GetFsiLibraryName()+".dll")
              yield AssemblyReference(rangeStartup,name) 
          yield! tcConfig.referencedDLLs ]

    static member SplitNonFoundationalResolutions (tcConfig:TcConfig) =
        let assemblyList = TcAssemblyResolutions.GetAllDllReferences tcConfig
        let resolutions = TcAssemblyResolutions.Resolve(tcConfig,assemblyList,tcConfig.knownUnresolvedReferences)
        let frameworkDLLs,nonFrameworkReferences = resolutions.GetAssemblyResolutions() |> List.partition (fun r -> r.sysdir) 
        let unresolved = resolutions.GetUnresolvedReferences()
#if TRACK_DOWN_EXTRA_BACKSLASHES        
        frameworkDLLs |> List.iter(fun x ->
            let path = x.resolvedPath 
            System.Diagnostics.Debug.Assert(not(path.Contains(@"\\")), "SplitNonFoundationalResolutions results in a non-canonical filename with extra backslashes: "+path)
            )
        nonFrameworkReferences |> List.iter(fun x ->
            let path = x.resolvedPath 
            System.Diagnostics.Debug.Assert(not(path.Contains(@"\\")), "SplitNonFoundationalResolutions results in a non-canonical filename with extra backslashes: "+path)
            )
#endif       
#if DEBUG
        let itFailed = ref false
        let addedText = "\nIf you want to debug this right now, attach a debugger, and put a breakpoint in 'build.fs' near the text '!itFailed', and you can re-step through the assembly resolution logic."
        unresolved 
        |> List.iter (fun (UnresolvedAssemblyReference(referenceText,_ranges)) ->
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
            let resolutions = TcAssemblyResolutions.Resolve(tcConfig,assemblyList,[])
            let _frameworkDLLs,_nonFrameworkReferences = resolutions.GetAssemblyResolutions() |> List.partition (fun r -> r.sysdir) 
            ()
#endif
        frameworkDLLs,nonFrameworkReferences,unresolved

    static member BuildFromPriorResolutions (tcConfig:TcConfig,resolutions,knownUnresolved) =
        let references = resolutions |> List.map (fun r -> r.originalReference)
        TcAssemblyResolutions.Resolve(tcConfig,references,knownUnresolved)
            

//----------------------------------------------------------------------------
// Typecheck and optimization environments on disk
//--------------------------------------------------------------------------
open Pickle

let IsSignatureDataResource         (r: ILResource) = String.hasPrefix r.Name FSharpSignatureDataResourceName
let IsOptimizationDataResource      (r: ILResource) = String.hasPrefix r.Name FSharpOptimizationDataResourceName
let GetSignatureDataResourceName    (r: ILResource) = String.dropPrefix (String.dropPrefix r.Name FSharpSignatureDataResourceName) "."
let GetOptimizationDataResourceName (r: ILResource) = String.dropPrefix (String.dropPrefix r.Name FSharpOptimizationDataResourceName) "."
let IsReflectedDefinitionsResource  (r: ILResource) = String.hasPrefix r.Name QuotationPickler.pickledDefinitionsResourceNameBase

type ILResource with 
    /// Get a function to read the bytes from a resource local to an assembly
    member r.GetByteReader(m) = 
        match r.Location with 
        | ILResourceLocation.Local b -> b
        | _-> error(InternalError("UnpickleFromResource",m))

let MakeILResource rname bytes = 
    { Name = rname;
      Location = ILResourceLocation.Local (fun () -> bytes);
      Access = ILResourceAccess.Public;
      CustomAttrs = emptyILCustomAttrs }

#if NO_COMPILER_BACKEND
#else
let PickleToResource file g scope rname p x = 
    { Name = rname;
      Location = (let bytes = pickleObjWithDanglingCcus file g scope p x in ILResourceLocation.Local (fun () -> bytes));
      Access = ILResourceAccess.Public;
      CustomAttrs = emptyILCustomAttrs }
#endif

let GetSignatureData (file, ilScopeRef, ilModule, byteReader) : PickledDataWithReferences<PickledModuleInfo> = 
    unpickleObjWithDanglingCcus file ilScopeRef ilModule unpickleModuleInfo (byteReader())

#if NO_COMPILER_BACKEND
#else
let WriteSignatureData (tcConfig:TcConfig,tcGlobals,exportRemapping,ccu:CcuThunk,file) : ILResource = 
    let mspec = ccu.Contents
#if DEBUG
    if !verboseStamps then 
        dprintf "Signature data before remap:\n%s\n" (Layout.showL (Layout.squashTo 192 (entityL mspec)));
        dprintf "---------------------- START OF APPLYING EXPORT REMAPPING TO SIGNATURE DATA------------\n";
#endif
    let mspec = ApplyExportRemappingToEntity tcGlobals exportRemapping mspec
#if DEBUG
    if !verboseStamps then 
        dprintf "---------------------- END OF APPLYING EXPORT REMAPPING TO SIGNATURE DATA------------\n";
        dprintf "Signature data after remap:\n%s\n" (Layout.showL (Layout.squashTo 192 (entityL mspec)));
#endif
    PickleToResource file tcGlobals ccu (FSharpSignatureDataResourceName+"."+ccu.AssemblyName) pickleModuleInfo 
        { mspec=mspec; 
          compileTimeWorkingDir=tcConfig.implicitIncludeDir;
          usesQuotations = ccu.UsesQuotations }
#endif // NO_COMPILER_BACKEND

let GetOptimizationData (file, ilScopeRef, ilModule, byteReader) = 
    unpickleObjWithDanglingCcus file ilScopeRef ilModule Opt.u_LazyModuleInfo (byteReader())

#if NO_COMPILER_BACKEND
#else
let WriteOptimizationData (tcGlobals, file, ccu,modulInfo) = 
    if verbose then  dprintf "Optimization data after remap:\n%s\n" (Layout.showL (Layout.squashTo 192 (Opt.moduleInfoL tcGlobals modulInfo)));
    PickleToResource file tcGlobals ccu (FSharpOptimizationDataResourceName+"."+ccu.AssemblyName) Opt.p_LazyModuleInfo modulInfo
#endif

//----------------------------------------------------------------------------
// Relink blobs of saved data by fixing up ccus.
//--------------------------------------------------------------------------

let availableToOptionalCcu = function
    | ResolvedCcu(ccu) -> Some(ccu)
    | UnresolvedCcu _ -> None


//----------------------------------------------------------------------------
// TcConfigProvider
//--------------------------------------------------------------------------

type TcConfigProvider = 
    | TcConfigProvider of (unit -> TcConfig)
    member x.Get() = (let (TcConfigProvider(f)) = x in f())
    static member Constant(tcConfig) = TcConfigProvider(fun () -> tcConfig)
    static member BasedOnMutableBuilder(tcConfigB) = TcConfigProvider(fun () -> TcConfig.Create(tcConfigB,validate=false))
    
    
//----------------------------------------------------------------------------
// TcImports
//--------------------------------------------------------------------------

          
/// Tables of imported assemblies.      
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
#if EXTENSIONTYPING
    let mutable generatedTypeRoots = new System.Collections.Generic.Dictionary<ILTypeRef, int * ProviderGeneratedType>()
#endif
    
    let CheckDisposed() =
        if disposed then assert false

    // REVIEW: Post-RTM, we should remove static dependencies over "expected" foundational CCUs, and 
    // search over all imported CCUs for each cached type
    static let ccuHasType (ccu : CcuThunk) (nsname : string list) (tname : string) =
        match (Some ccu.Contents, nsname) ||> List.fold (fun entityOpt n -> match entityOpt with None -> None | Some entity -> entity.ModuleOrNamespaceType.AllEntitiesByCompiledAndLogicalMangledNames.TryFind n) with
        | Some ns ->
                match Map.tryFind tname ns.ModuleOrNamespaceType.TypesByMangledName with
                | Some _ -> true
                | None -> false
        | None -> false
  
    member tcImports.SetBase(baseTcImports) =
        CheckDisposed()
        importsBase <- Some(baseTcImports)

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
        ccuInfos <- ccuInfos ++ ccuInfo;
        // Assembly Ref Resolution: remove this use of ccu.AssemblyName
        ccuTable <- NameMap.add (ccuInfo.FSharpViewOfMetadata.AssemblyName) ccuInfo ccuTable
    
    member tcImports.RegisterDll(dllInfo) =
        CheckDisposed()
        dllInfos <- dllInfos ++ dllInfo;
        dllTable <- NameMap.add (getNameOfScopeRef dllInfo.ILScopeRef) dllInfo dllTable

    member tcImports.GetDllInfos() = 
        CheckDisposed()
        match importsBase with 
        | Some(importsBase)-> importsBase.GetDllInfos() @ dllInfos
        | None -> dllInfos
        
    member tcImports.TryFindDllInfo (m,assemblyName,lookupOnly) =
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
            tcImports.ImplicitLoadIfAllowed(m,assemblyName,lookupOnly);
            look tcImports
    

    member tcImports.FindDllInfo (m,assemblyName) =
        match tcImports.TryFindDllInfo (m,assemblyName,lookupOnly=false) with 
        | Some res -> res
        | None -> error(Error(FSComp.SR.buildCouldNotResolveAssembly(assemblyName),m))

    member tcImports.GetImportedAssemblies() = 
        CheckDisposed()
        match importsBase with 
        | Some(importsBase)-> importsBase.GetImportedAssemblies() @ ccuInfos
        | None -> ccuInfos        
        
    member tcImports.GetCcusExcludingBase() = 
        CheckDisposed()
        ccuInfos |> List.map (fun x -> x.FSharpViewOfMetadata)        

    member tcImports.GetCcusInDeclOrder() =         
        CheckDisposed()
        List.map (fun x -> x.FSharpViewOfMetadata) (tcImports.GetImportedAssemblies())  
        
    // This is the main "assembly reference --> assembly" resolution routine. 
    member tcImports.FindCcuInfo (m,assemblyName,lookupOnly) = 
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
            tcImports.ImplicitLoadIfAllowed(m,assemblyName,lookupOnly);
            match look tcImports with 
            | Some res -> ResolvedImportedAssembly(res)
            | None -> UnresolvedImportedAssembly(assemblyName)
        

    member tcImports.FindCcu (m, assemblyName,lookupOnly) = 
        CheckDisposed()
        match tcImports.FindCcuInfo(m,assemblyName,lookupOnly) with
        | ResolvedImportedAssembly(importedAssembly) -> ResolvedCcu(importedAssembly.FSharpViewOfMetadata)
        | UnresolvedImportedAssembly(assemblyName) -> UnresolvedCcu(assemblyName)

    member tcImports.FindCcuFromAssemblyRef(m,assref:ILAssemblyRef) = 
        CheckDisposed()
        match tcImports.FindCcuInfo(m,assref.Name,lookupOnly=false) with
        | ResolvedImportedAssembly(importedAssembly) -> ResolvedCcu(importedAssembly.FSharpViewOfMetadata)
        | UnresolvedImportedAssembly _ -> UnresolvedCcu(assref.QualifiedName)


#if EXTENSIONTYPING
    member tcImports.GetProvidedAssemblyInfo(m, assembly: Tainted<ProvidedAssembly>) = 
        let anameOpt = assembly.PUntaint((fun assembly -> match assembly with null -> None | a -> Some (a.GetName())), m)
        match anameOpt with 
        | None -> false, None
        | Some aname -> 
        let ilShortAssemName = aname.Name
        match tcImports.FindCcu (m, ilShortAssemName, lookupOnly=true) with 
        | ResolvedCcu ccu -> 
            if ccu.IsProviderGenerated then 
                let dllinfo = tcImports.FindDllInfo(m,ilShortAssemName)
                true, dllinfo.ProviderGeneratedStaticLinkMap
            else
                false, None

        | UnresolvedCcu _ -> 
            let g = tcImports.GetTcGlobals()
            let ilScopeRef = ILScopeRef.Assembly (ILAssemblyRef.FromAssemblyName aname)
            let fileName = aname.Name + ".dll"
            let bytes = assembly.PApplyWithProvider((fun (assembly,provider) -> assembly.GetManifestModuleContents(provider)), m).PUntaint(id,m)
            let ilModule,ilAssemblyRefs = 
                let opts = { ILBinaryReader.mkDefault g.ilg with 
                                ILBinaryReader.optimizeForMemory=true
                                ILBinaryReader.pdbPath = None }                       
                let reader = ILBinaryReader.OpenILModuleReaderFromBytes fileName bytes opts
                reader.ILModuleDef, reader.ILAssemblyRefs

            let theActualAssembly = assembly.PUntaint((fun x -> x.Handle),m)
            let dllinfo = 
                { RawMetadata=ilModule; 
                  FileName=fileName;
                  ProviderGeneratedAssembly=Some theActualAssembly
                  IsProviderGenerated=true;
                  ProviderGeneratedStaticLinkMap= if g.isInteractive then None else Some (ProvidedAssemblyStaticLinkingMap.CreateNew())
                  ILScopeRef = ilScopeRef;
                  ILAssemblyRefs = ilAssemblyRefs }
            tcImports.RegisterDll(dllinfo);
            let ccuData = 
              { IsFSharp=false;
                UsesQuotations=false;
                InvalidateEvent=(new Event<_>()).Publish;
                IsProviderGenerated = true
                QualifiedName= Some (assembly.PUntaint((fun a -> a.FullName), m));
                Contents = NewCcuContents ilScopeRef m ilShortAssemName (NewEmptyModuleOrNamespaceType Namespace) ;
                ILScopeRef = ilScopeRef;
                Stamp = newStamp();
                SourceCodeDirectory = "";  
                FileName = Some fileName
                MemberSignatureEquality = (fun ty1 ty2 -> Tastops.typeEquivAux EraseAll g ty1 ty2)
                ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
                TypeForwarders = Map.empty }
                    
            let ccu = CcuThunk.Create(ilShortAssemName,ccuData)
            let ccuinfo = 
                { FSharpViewOfMetadata=ccu; 
                  ILScopeRef = ilScopeRef; 
                  AssemblyAutoOpenAttributes = [];
                  AssemblyInternalsVisibleToAttributes = [];
                  IsProviderGenerated = true;
                  TypeProviders=[];
                  FSharpOptimizationData = notlazy None }
            tcImports.RegisterCcu(ccuinfo);
            // Yes, it is generative
            true, dllinfo.ProviderGeneratedStaticLinkMap

    member tcImports.RecordGeneratedTypeRoot root = 
        // checking if given ProviderGeneratedType was already recorded before (probably for another set of static parameters) 
        let (ProviderGeneratedType(_, ilTyRef, _)) = root
        let index = 
            match generatedTypeRoots.TryGetValue ilTyRef with
            | true,(index, _) -> index
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
  
    override obj.ToString() = 
        sprintf "tcImports = \n    dllInfos=%A\n    dllTable=%A\n    ccuInfos=%A\n    ccuTable=%A\n    Base=%s\n"
            dllInfos
            dllTable
            ccuInfos
            ccuTable
            (match importsBase with None-> "None" | Some(importsBase) -> importsBase.ToString())
    
      
    // Note: the returned binary reader is associated with the tcImports, i.e. when the tcImports are closed 
    // then the reader is closed. 
    member tcImports.OpenILBinaryModule(filename,m) = 
      try
        CheckDisposed()
        let tcConfig = tcConfigP.Get()
        let pdbPathOption = 
            // We open the pdb file if one exists parallel to the binary we 
            // are reading, so that --standalone will preserve debug information. 
            if tcConfig.openDebugInformationForLaterStaticLinking then 
                let pdbDir = (try Filename.directoryName filename with _ -> ".") 
                let pdbFile = (try Filename.chopExtension filename with _ -> filename)+".pdb" 
                if FileSystem.SafeExists pdbFile then 
                    if verbose then dprintf "reading PDB file %s from directory %s\n" pdbFile pdbDir;
                    Some pdbDir
                else 
                    None 
            else   
                None

        let ilILBinaryReader = OpenILBinary(filename,tcConfig.optimizeForMemory,tcConfig.openBinariesInMemory,ilGlobalsOpt,pdbPathOption, tcConfig.primaryAssembly.Name, tcConfig.noDebugData, tcConfig.shadowCopyReferences)

        tcImports.AttachDisposeAction(fun _ -> ILBinaryReader.CloseILModuleReader ilILBinaryReader);
        ilILBinaryReader.ILModuleDef, ilILBinaryReader.ILAssemblyRefs
      with e ->
        error(Error(FSComp.SR.buildErrorOpeningBinaryFile(filename, e.Message),m))



    (* auxModTable is used for multi-module assemblies *)
    member tcImports.MkLoaderForMultiModuleIlAssemblies m =
        CheckDisposed()
        let auxModTable = HashMultiMap(10, HashIdentity.Structural)
        fun viewedScopeRef ->
        
            let tcConfig = tcConfigP.Get()
            match viewedScopeRef with
            | ILScopeRef.Module modref -> 
                let key = modref.Name
                if not (auxModTable.ContainsKey(key)) then
                    let resolution = tcConfig.ResolveLibWithDirectories CcuLoadFailureAction.RaiseError (AssemblyReference(m,key)) |> Option.get
                    let ilModule,_ = tcImports.OpenILBinaryModule(resolution.resolvedPath,m)
                    auxModTable.[key] <- ilModule
                auxModTable.[key] 

            | _ -> 
                error(InternalError("Unexpected ILScopeRef.Local or ILScopeRef.Assembly in exported type table",m))

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
                 member x.LoadAssembly (m, ilAssemblyRef) = 
                     tcImports.FindCcuFromAssemblyRef(m,ilAssemblyRef)
#if EXTENSIONTYPING
                 member x.GetProvidedAssemblyInfo (m,assembly) = tcImports.GetProvidedAssemblyInfo (m,assembly)
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

#if EXTENSIONTYPING
    member private tcImports.InjectProvidedNamespaceOrTypeIntoEntity 
            (typeProviderEnvironment, 
             tcConfig:TcConfig,
             m,entity:Entity,
             injectedNamspace,remainingNamespace,
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
                let cpath = CompPath(ILScopeRef.Local, injectedNamspace |> List.rev |> List.map (fun n -> (n,ModuleOrNamespaceKind.Namespace)) )
                let newNamespace = NewModuleOrNamespace (Some cpath) taccessPublic (ident(next,rangeStartup)) XmlDoc.Empty [] (notlazy (NewEmptyModuleOrNamespaceType Namespace)) 
                entity.ModuleOrNamespaceType.AddModuleOrNamespaceByMutation(newNamespace)
                tcImports.InjectProvidedNamespaceOrTypeIntoEntity (typeProviderEnvironment, tcConfig, m, newNamespace, next::injectedNamspace, rest, provider, st)
        | [] -> 
            match st with
            | Some st ->
                // Inject the wrapper type into the provider assembly.
                //
                // Generated types get properly injected into the provided (i.e. generated) assembly CCU in tc.fs

                let importProvidedType t = Import.ImportProvidedType (tcImports.GetImportMap()) m t
                let isSuppressRelocate = tcConfig.isInteractive || st.PUntaint((fun st -> st.IsSuppressRelocate),m) 
                let newEntity = Construct.NewProvidedTycon(typeProviderEnvironment, st, importProvidedType, isSuppressRelocate, m) 
                entity.ModuleOrNamespaceType.AddProvidedTypeEntity(newEntity)
            | None -> ()

            entity.Data.entity_tycon_repr <-
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
               (tpApprovals : ApprovalIO.TypeProviderApprovalStatus list, 
                displayPSTypeProviderSecurityDialogBlockingUI, 
                tcConfig:TcConfig, 
                fileNameOfRuntimeAssembly, 
                ilScopeRefOfRuntimeAssembly,
                runtimeAssemblyAttributes:ILAttribute list, 
                entityToInjectInto, invalidateCcu:Event<_>, m) = 

        let startingErrorCount = CompileThreadStatic.ErrorLogger.ErrorCount

        // Find assembly level TypeProviderAssemblyAttributes. These will point to the assemblies that 
        // have class which implement ITypeProvider and which have TypeProviderAttribute on them.
        let providerAssemblies = 
            runtimeAssemblyAttributes 
            |> List.choose (TryDecodeTypeProviderAssemblyAttr (defaultArg ilGlobalsOpt EcmaILGlobals))
            // If no design-time assembly is specified, use the runtime assembly
            |> List.map (function null -> Path.GetFileNameWithoutExtension fileNameOfRuntimeAssembly | s -> s)
            |> Set.ofList

        if providerAssemblies.Count > 0 then

            // Find the SystemRuntimeAssemblyVersion value to report in the TypeProviderConfig.
            let systemRuntimeAssemblyVersion = 
                let primaryAssemblyRef = tcConfig.PrimaryAssemblyDllReference()
                let resolution = tcConfig.ResolveLibWithDirectories CcuLoadFailureAction.RaiseError primaryAssemblyRef |> Option.get
                 // MSDN: this method causes the file to be opened and closed, but the assembly is not added to this domain
                let name = System.Reflection.AssemblyName.GetAssemblyName(resolution.resolvedPath)
                name.Version

            let typeProviderEnvironment = 
                 { resolutionFolder       = tcConfig.implicitIncludeDir
                   outputFile             = tcConfig.outputFile
                   showResolutionMessages = tcConfig.showExtensionTypeMessages 
                   referencedAssemblies   = [| for r in resolutions.GetAssemblyResolutions() -> r.resolvedPath |]
                   temporaryFolder        = Path.GetTempPath() }

            let providers = 
                [ for assemblyName in providerAssemblies do
                      yield ExtensionTyping.GetTypeProvidersOfAssembly(displayPSTypeProviderSecurityDialogBlockingUI, tcConfig.validateTypeProviders, tpApprovals, 
                                                                       fileNameOfRuntimeAssembly, ilScopeRefOfRuntimeAssembly, assemblyName, typeProviderEnvironment, 
                                                                       tcConfig.isInvalidationSupported, tcConfig.isInteractive, tcImports.SystemRuntimeContainsType, systemRuntimeAssemblyVersion, m) ]
            let wasApproved = providers |> List.forall (fun (ok,_) -> ok)
            let providers = providers |> List.map snd |> List.concat

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
                provider.PUntaint((fun tp -> tp.Invalidate.Add(fun _ -> invalidateCcu.Trigger ("The provider '" + fileNameOfRuntimeAssembly + "' reported a change"))), m)
                
            match providers with
            | [] -> 
                if wasApproved then
                    warning(Error(FSComp.SR.etHostingAssemblyFoundWithoutHosts(fileNameOfRuntimeAssembly,typeof<Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute>.FullName),m)); 
            | _ -> 

                if typeProviderEnvironment.showResolutionMessages then
                    dprintfn "Found extension type hosting hosting assembly '%s' with the following extensions:" fileNameOfRuntimeAssembly
                    providers |> List.iter(fun provider ->dprintfn " %s" (ExtensionTyping.DisplayNameOfTypeProvider(provider.TypeProvider, m)))
                    
                for provider in providers do 
                    try
                        // Inject an entity for the namespace, or if one already exists, then record this as a provider
                        // for that namespace.
                        let rec loop (providedNamespace: Tainted<IProvidedNamespace>) =
                            let path = ExtensionTyping.GetProvidedNamespaceAsPath(m,provider,providedNamespace.PUntaint((fun r -> r.NamespaceName), m))
                            tcImports.InjectProvidedNamespaceOrTypeIntoEntity (typeProviderEnvironment, tcConfig, m, entityToInjectInto, [],path, provider, None)

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

                        let providedNamespaces = provider.PApplyArray((fun r -> r.GetNamespaces()), "GetNamespaces", m)
                        for providedNamespace in providedNamespaces do
                            loop providedNamespace
                    with e -> 
                        errorRecovery e m

                if startingErrorCount<CompileThreadStatic.ErrorLogger.ErrorCount then
                    error(Error(FSComp.SR.etOneOrMoreErrorsSeenDuringExtensionTypeSetting(),m))  

            providers 
        else []
#endif

    /// Query information about types available in target system runtime library
    member tcImports.SystemRuntimeContainsType (typeName : string) : bool = 
        let ns, typeName = IL.splitILTypeName typeName
        let tcGlobals = tcImports.GetTcGlobals()
        ccuHasType tcGlobals.sysCcu ns typeName

    // Add a referenced assembly
    //
    // Retargetable assembly refs are required for binaries that must run 
    // against DLLs supported by multiple publishers. For example
    // Compact Framework binaries must use this. However it is not
    // clear when else it is required, e.g. for Mono.
    
    member tcImports.PrepareToImportReferencedIlDll tpApprovals m filename displayPSTypeProviderSecurityDialogBlockingUI (dllinfo:ImportedBinary) =
        CheckDisposed()
        let tcConfig = tcConfigP.Get()
        tcConfig.CheckFSharpBinary(filename,dllinfo.ILAssemblyRefs,m)
        let ilModule = dllinfo.RawMetadata
        let ilScopeRef = dllinfo.ILScopeRef
        let aref =   
            match ilScopeRef with 
            | ILScopeRef.Assembly aref -> aref 
            | _ -> error(InternalError("PrepareToImportReferencedIlDll: cannot reference .NET netmodules directly, reference the containing assembly instead",m))

        let nm = aref.Name
        if verbose then dprintn ("Converting IL assembly to F# data structures "+nm);
        let auxModuleLoader = tcImports.MkLoaderForMultiModuleIlAssemblies m
        let invalidateCcu = new Event<_>()
        let ccu = Import.ImportILAssembly(tcImports.GetImportMap,m,auxModuleLoader,ilScopeRef,tcConfig.implicitIncludeDir, Some filename,ilModule,invalidateCcu.Publish)
        
        let ilg = defaultArg ilGlobalsOpt EcmaILGlobals

        let ccuinfo = 
            { FSharpViewOfMetadata=ccu; 
              ILScopeRef = ilScopeRef; 
              AssemblyAutoOpenAttributes = GetAutoOpenAttributes ilg ilModule;
              AssemblyInternalsVisibleToAttributes = GetInternalsVisibleToAttributes ilg ilModule;
#if EXTENSIONTYPING
              IsProviderGenerated = false; 
              TypeProviders = [];
#endif
              FSharpOptimizationData = notlazy None }
        tcImports.RegisterCcu(ccuinfo);
        let phase2 () = 
#if EXTENSIONTYPING
            ccuinfo.TypeProviders <- tcImports.ImportTypeProviderExtensions (tpApprovals, displayPSTypeProviderSecurityDialogBlockingUI, tcConfig, filename, ilScopeRef, ilModule.ManifestOfAssembly.CustomAttrs.AsList, ccu.Contents, invalidateCcu, m)
#else
            // to prevent unused parameter warning
            ignore tpApprovals
            ignore displayPSTypeProviderSecurityDialogBlockingUI
#endif
            [ResolvedImportedAssembly(ccuinfo)]
        phase2

    member tcImports.PrepareToImportReferencedFSharpDll tpApprovals m filename displayPSTypeProviderSecurityDialogBlockingUI (dllinfo:ImportedBinary) =
        CheckDisposed()
        let tcConfig = tcConfigP.Get()
        tcConfig.CheckFSharpBinary(filename,dllinfo.ILAssemblyRefs,m)

        let ilModule = dllinfo.RawMetadata 
        let ilScopeRef = dllinfo.ILScopeRef 
        let ilShortAssemName = getNameOfScopeRef ilScopeRef 
        if verbose then dprintn ("Converting F# assembly to F# data structures "+(getNameOfScopeRef ilScopeRef));
        let attrs = GetCustomAttributesOfIlModule ilModule 
        assert (List.exists IsSignatureDataVersionAttr attrs);
        if verbose then dprintn ("Relinking interface info from F# assembly "+ilShortAssemName);
        let resources = ilModule.Resources.AsList 
        let externalSigAndOptData = ["FSharp.Core";"FSharp.LanguageService.Compiler"]
        if not(List.contains ilShortAssemName externalSigAndOptData) then 
            assert (List.exists IsSignatureDataResource resources);
        let optDataReaders = 
            resources 
            |> List.choose (fun r -> if IsOptimizationDataResource r then Some(GetOptimizationDataResourceName r,r.GetByteReader(m)) else None)


        let ccuRawDataAndInfos = 
            let sigDataReaders = 
                [ for iresource in resources  do
                    if IsSignatureDataResource  iresource then 
                        let ccuName = GetSignatureDataResourceName iresource 
                        yield (ccuName, iresource.GetByteReader(m)) ]
                        
            let sigDataReaders = 
                if List.contains ilShortAssemName externalSigAndOptData then 
                    let sigFileName = Path.ChangeExtension(filename, "sigdata")
                    if not sigDataReaders.IsEmpty then 
                        error(Error(FSComp.SR.buildDidNotExpectSigdataResource(),m));
                    if not (FileSystem.SafeExists sigFileName)  then 
                        error(Error(FSComp.SR.buildExpectedSigdataFile(), m));
                    [ (ilShortAssemName, (fun () -> FileSystem.ReadAllBytesShim sigFileName))]
                  else
                    sigDataReaders
            sigDataReaders 
            |> List.map (fun (ccuName, sigDataReader) -> 
                let data = GetSignatureData (filename, ilScopeRef, ilModule, sigDataReader)

                // Look for optimization data in a file 
                let optDataReaders = 
                    if List.contains ilShortAssemName externalSigAndOptData then 
                        let optDataFile = Path.ChangeExtension(filename, "optdata")
                        if not optDataReaders.IsEmpty then 
                            error(Error(FSComp.SR.buildDidNotExpectOptDataResource(),m));
                        if not (FileSystem.SafeExists optDataFile)  then 
                            error(Error(FSComp.SR.buildExpectedFileAlongSideFSharpCore(optDataFile),m));
                        [ (ilShortAssemName, (fun () -> FileSystem.ReadAllBytesShim optDataFile))]
                    else
                        optDataReaders

                let optDatas = Map.ofList optDataReaders

                let minfo : PickledModuleInfo = data.RawData 
                let mspec = minfo.mspec 

#if EXTENSIONTYPING
                let invalidateCcu = new Event<_>()
#endif

                // Adjust where the code for known F# libraries live relative to the installation of F#
                let codeDir = 
                    let dir = minfo.compileTimeWorkingDir
                    let knownLibraryLocation = @"src\fsharp\" // Help highlighting... " 
                    let knownLibarySuffixes = 
                        [ @"FSharp.Core";
                          @"FSharp.PowerPack"; 
                          @"FSharp.PowerPack.Linq"; 
                          @"FSharp.PowerPack.Metadata"  ]
                    match knownLibarySuffixes |> List.tryFind (fun x -> dir.EndsWith(knownLibraryLocation + x,StringComparison.OrdinalIgnoreCase)) with
                    | None -> 
                        dir
                    | Some libSuffix -> 
                        // add "..\lib\FSharp.Core" to the F# binaries directory
                        Path.Combine(Path.Combine(tcConfig.fsharpBinariesDir,@"..\lib"),libSuffix)

                let ccu = 
                   CcuThunk.Create(ccuName, { ILScopeRef=ilScopeRef
                                              Stamp = newStamp()
                                              FileName = Some filename 
                                              QualifiedName= Some(ilScopeRef.QualifiedName)
                                              SourceCodeDirectory = codeDir  (* note: in some cases we fix up this information later *)
                                              IsFSharp=true
                                              Contents = mspec 
#if EXTENSIONTYPING
                                              InvalidateEvent=invalidateCcu.Publish
                                              IsProviderGenerated = false
                                              ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
                                              UsesQuotations = minfo.usesQuotations
                                              MemberSignatureEquality= (fun ty1 ty2 -> Tastops.typeEquivAux EraseAll (tcImports.GetTcGlobals()) ty1 ty2)
                                              TypeForwarders = match ilModule.Manifest with | Some manifest -> ImportILAssemblyTypeForwarders(tcImports.GetImportMap,m,manifest.ExportedTypes) | None -> Map.empty })

                let optdata = 
                    lazy 
                        (match Map.tryFind ccuName optDatas  with 
                         | None -> 
                            if verbose then dprintf "*** no optimization data for CCU %s, was DLL compiled with --no-optimization-data??\n" ccuName 
                            None
                         | Some info -> 
                            let data = GetOptimizationData (filename, ilScopeRef, ilModule, info)
                            let res = data.OptionalFixup(fun nm -> availableToOptionalCcu(tcImports.FindCcu(m,nm,lookupOnly=false))) 
                            if verbose then dprintf "found optimization data for CCU %s\n" ccuName 
                            Some res)
                let ilg = defaultArg ilGlobalsOpt EcmaILGlobals
                let ccuinfo = 
                     { FSharpViewOfMetadata=ccu 
                       AssemblyAutoOpenAttributes = GetAutoOpenAttributes ilg ilModule
                       AssemblyInternalsVisibleToAttributes = GetInternalsVisibleToAttributes ilg ilModule
                       FSharpOptimizationData=optdata 
#if EXTENSIONTYPING
                       IsProviderGenerated = false
                       TypeProviders = []
#endif
                       ILScopeRef = ilScopeRef }  
                let phase2() = 
#if EXTENSIONTYPING
                     ccuinfo.TypeProviders <- tcImports.ImportTypeProviderExtensions (tpApprovals, displayPSTypeProviderSecurityDialogBlockingUI, tcConfig, filename, ilScopeRef, ilModule.ManifestOfAssembly.CustomAttrs.AsList, ccu.Contents, invalidateCcu, m)
#else
                     // to prevent unused parameter warning
                     ignore tpApprovals
                     ignore displayPSTypeProviderSecurityDialogBlockingUI

                     ()
#endif
                data,ccuinfo,phase2)
                     
        // Register all before relinking to cope with mutually-referential ccus 
        ccuRawDataAndInfos |> List.iter (p23 >> tcImports.RegisterCcu)
        let phase2 () = 
            (* Relink *)
            (* dprintf "Phase2: %s\n" filename; REMOVE DIAGNOSTICS *)
            ccuRawDataAndInfos |> List.iter (fun (data,_,_) -> data.OptionalFixup(fun nm -> availableToOptionalCcu(tcImports.FindCcu(m,nm,lookupOnly=false))) |> ignore);
#if EXTENSIONTYPING
            ccuRawDataAndInfos |> List.iter (fun (_,_,phase2) -> phase2())
#endif
            ccuRawDataAndInfos |> List.map p23 |> List.map ResolvedImportedAssembly  
        phase2
         

    member tcImports.RegisterAndPrepareToImportReferencedDll tpApprovals displayPSTypeProviderSecurityDialogBlockingUI (r:AssemblyResolution) : _*(unit -> AvailableImportedAssembly list)=
        CheckDisposed()
        let m = r.originalReference.Range
        let filename = r.resolvedPath
        let ilModule,ilAssemblyRefs = tcImports.OpenILBinaryModule(filename,m)

        let ilShortAssemName = GetNameOfILModule ilModule 
        if tcImports.IsAlreadyRegistered ilShortAssemName then 
            let dllinfo = tcImports.FindDllInfo(m,ilShortAssemName)
            let phase2() = [tcImports.FindCcuInfo(m,ilShortAssemName,lookupOnly=false)]
            dllinfo,phase2
        else 
            let ilScopeRef = MakeScopeRefForIlModule ilModule
            let dllinfo = {RawMetadata=ilModule 
                           FileName=filename
#if EXTENSIONTYPING
                           ProviderGeneratedAssembly=None
                           IsProviderGenerated=false
                           ProviderGeneratedStaticLinkMap = None
#endif
                           ILScopeRef = ilScopeRef
                           ILAssemblyRefs = ilAssemblyRefs }
            tcImports.RegisterDll(dllinfo)
            let attrs = GetCustomAttributesOfIlModule ilModule
            let ilg = defaultArg ilGlobalsOpt EcmaILGlobals
            let phase2 = 
                if (List.exists IsSignatureDataVersionAttr attrs) then 
                    if not (List.exists (IsMatchingSignatureDataVersionAttr ilg (IL.parseILVersion Internal.Utilities.FSharpEnvironment.FSharpBinaryMetadataFormatRevision)) attrs) then 
                      errorR(Error(FSComp.SR.buildDifferentVersionMustRecompile(filename),m))
                      tcImports.PrepareToImportReferencedIlDll tpApprovals m filename displayPSTypeProviderSecurityDialogBlockingUI dllinfo
                    else 
                      try
                        tcImports.PrepareToImportReferencedFSharpDll tpApprovals m filename displayPSTypeProviderSecurityDialogBlockingUI dllinfo
                      with e -> error(Error(FSComp.SR.buildErrorOpeningBinaryFile(filename, e.Message),m))
                else 
                    tcImports.PrepareToImportReferencedIlDll tpApprovals m filename displayPSTypeProviderSecurityDialogBlockingUI dllinfo
            dllinfo,phase2

    member tcImports.RegisterAndImportReferencedAssemblies (displayPSTypeProviderSecurityDialogBlockingUI, nms:AssemblyResolution list) =
        CheckDisposed()

#if EXTENSIONTYPING
        let tpApprovals = ExtensionTyping.ApprovalIO.ReadApprovalsFile(None)
#else
        let tpApprovals = []
#endif
        let dllinfos,phase2s = 
           nms |> List.map 
                    (fun nm ->
                        try
                            tcImports.RegisterAndPrepareToImportReferencedDll tpApprovals displayPSTypeProviderSecurityDialogBlockingUI nm
                        with e ->
                            error(Error(FSComp.SR.buildProblemReadingAssembly(nm.fusionName, e.Message),nm.originalReference.Range)))
               |> List.unzip
        let ccuinfos = (List.collect (fun phase2 -> phase2()) phase2s) 
        dllinfos,ccuinfos
      
    member tcImports.DoRegisterAndImportReferencedAssemblies(displayPSTypeProviderSecurityDialogBlockingUI,nms) = 
        CheckDisposed()
        tcImports.RegisterAndImportReferencedAssemblies(displayPSTypeProviderSecurityDialogBlockingUI,nms) |> ignore

    member tcImports.ImplicitLoadIfAllowed (m, assemblyName, lookupOnly) = 
        CheckDisposed()
        // If the user is asking for the default framework then also try to resolve other implicit assemblies as they are discovered.
        // Using this flag to mean 'allow implicit discover of assemblies'.
        let tcConfig = tcConfigP.Get()
        if not lookupOnly && tcConfig.implicitlyResolveAssemblies then 
            let tryFile speculativeFileName = 
                let foundFile = tcImports.TryResolveAssemblyReference (AssemblyReference (m, speculativeFileName), ResolveAssemblyReferenceMode.Speculative)
                match foundFile with 
                | OkResult (warns, res) -> 
                     ReportWarnings warns
                     tcImports.DoRegisterAndImportReferencedAssemblies(None,res)
                     true
                | ErrorResult (_warns, _err) -> 
                    // Throw away warnings and errors - this is speculative loading
                    false

            if tryFile (assemblyName + ".dll") then ()
            else tryFile (assemblyName + ".exe")  |> ignore

#if EXTENSIONTYPING
    member tcImports.TryFindProviderGeneratedAssemblyByName(assemblyName:string) :  System.Reflection.Assembly option = 
        // The assembly may not be in the resolutions, but may be in the load set including EST injected assemblies
        match tcImports.TryFindDllInfo (range0,assemblyName,lookupOnly=true) with 
        | Some res -> 
            // Provider-generated assemblies don't necessarily have an on-disk representation we can load.
            res.ProviderGeneratedAssembly 
        | _ -> None
#endif

    member tcImports.TryFindExistingFullyQualifiedPathFromAssemblyRef(assref:ILAssemblyRef) :  string option = 
        match resolutions.TryFindByExactILAssemblyRef assref with 
        | Some r -> Some r.resolvedPath
        | None -> None
        (*
                // The assembly may not be in the resolutions, but may be in the load set including EST injected assemblies
                let assemblyName = assref.Name
                match tcImports.TryFindDllInfo (range0,assemblyName,lookupOnly=true) with 
                | Some res -> 
#if EXTENSIONTYPING
                    // Provider-generated assemblies don't necessarily have an on-disk representation we can load.
                    if res.IsProviderGenerated then None else 
#endif
                    Some res.FileName
                | _ -> None
*)

    member tcImports.TryResolveAssemblyReference(assemblyReference:AssemblyReference,mode:ResolveAssemblyReferenceMode) : OperationResult<AssemblyResolution list> = 
        let tcConfig = tcConfigP.Get()
        // First try to lookup via the original reference text.
        match resolutions.TryFindByOriginalReference assemblyReference with
        | Some assemblyResolution -> 
            ResultD [assemblyResolution]
        | None ->
            // Next try to lookup up by the exact full resolved path.
            match resolutions.TryFindByResolvedPath assemblyReference.Text with 
            | Some assemblyResolution -> 
                ResultD [assemblyResolution]
            | None ->      
                                  
                if tcConfigP.Get().useMonoResolution then
                    ResultD [(tcConfig.ResolveLibWithDirectories CcuLoadFailureAction.RaiseError assemblyReference) |> Option.get]
                else 
                    // This is a previously unencounterd assembly. Resolve it and add it to the list.
                    // But don't cache resolution failures because the assembly may appear on the disk later.
                    let resolved,unresolved = TcConfig.TryResolveLibsUsingMSBuildRules(tcConfig,[ assemblyReference ],assemblyReference.Range,mode)
                    match resolved,unresolved with
                    | (assemblyResolution::_,_)  -> 
                        resolutions <- resolutions.AddResolutionResults resolved
                        ResultD [assemblyResolution]
                    | (_,_::_)  -> 
                        resolutions <- resolutions.AddUnresolvedReferences unresolved
                        ErrorD(AssemblyNotResolved(assemblyReference.Text,assemblyReference.Range))
                    | [],[] -> 
                        // Note, if mode=ResolveAssemblyReferenceMode.Speculative and the resolution failed then TryResolveLibsUsingMSBuildRules returns
                        // the empty list and we convert the failure into an AssemblyNotResolved here.
                        ErrorD(AssemblyNotResolved(assemblyReference.Text,assemblyReference.Range))
                        
     

    member tcImports.ResolveAssemblyReference(assemblyReference,mode) : AssemblyResolution list = 
        CommitOperationResult(tcImports.TryResolveAssemblyReference(assemblyReference,mode))

    // Note: This returns a TcImports object. However, framework TcImports are not currently disposed. The only reason
    // we dispose TcImports is because we need to dispose type providers, and type providers are never included in the framework DLL set.
    //
    // If this ever changes then callers may need to begin disposing the TcImports (though remember, not before all derived 
    // non-frameworkk TcImports built related to this framework TcImports are disposed).
    static member BuildFrameworkTcImports (tcConfigP:TcConfigProvider, frameworkDLLs, nonFrameworkDLLs) =

        let tcConfig = tcConfigP.Get()
        let tcResolutions = TcAssemblyResolutions.BuildFromPriorResolutions(tcConfig,frameworkDLLs,[])
        let tcAltResolutions = TcAssemblyResolutions.BuildFromPriorResolutions(tcConfig,nonFrameworkDLLs,[])

        // Note: TcImports are disposable - the caller owns this object and must dispose
        let frameworkTcImports = new TcImports(tcConfigP,tcResolutions,None,None) 
        let resolveAssembly loadFailureAction r = 
            // use existing resolutions before trying to search in known folders
            let resolution =
                match tcResolutions.TryFindByOriginalReference r with
                | Some r -> Some r
                | None -> 
                    match tcAltResolutions.TryFindByOriginalReference r with
                    | Some r -> Some r
                    | None -> tcConfig.ResolveLibWithDirectories loadFailureAction r
            match resolution with
            | Some resolution ->
                match frameworkTcImports.RegisterAndImportReferencedAssemblies(None, [resolution]) with
                | (_, [ResolvedImportedAssembly(ccu)]) -> Some ccu
                | _        -> 
                    match loadFailureAction with
                    | CcuLoadFailureAction.RaiseError -> error(InternalError("BuildFoundationalTcImports: no ccu for " + r.Text, rangeStartup))
                    | CcuLoadFailureAction.ReturnNone -> None
            | None -> None
        
        let ccuInitializer = tcConfig.GetPrimaryAssemblyCcuInitializer()
        let ilGlobals, state = ccuInitializer.BeginLoadingSystemRuntime((resolveAssembly CcuLoadFailureAction.RaiseError) >> Option.get, tcConfig.noDebugData)        
        frameworkTcImports.SetILGlobals ilGlobals
        let sysCcu = ccuInitializer.EndLoadingSystemRuntime(state, resolveAssembly)

        // Load the rest of the framework DLLs all at once (they may be mutually recursive)
        frameworkTcImports.DoRegisterAndImportReferencedAssemblies (None, tcResolutions.GetAssemblyResolutions())

        let fslibCcu = 
            if tcConfig.compilingFslib then 
                // When compiling FSharp.Core.dll, the fslibCcu reference to FSharp.Core.dll is a delayed ccu thunk fixed up during type checking
                CcuThunk.CreateDelayed(GetFSharpCoreLibraryName())
            else
                let fslibCcuInfo =
                    let coreLibraryReference = tcConfig.CoreLibraryDllReference()
                    //printfn "coreLibraryReference = %A" coreLibraryReference
                    
                    let resolvedAssemblyRef = 
                        match tcResolutions.TryFindByOriginalReference coreLibraryReference with
                        | Some resolution -> Some resolution
                        | _ -> 
                            // Are we using a "non-cannonical" FSharp.Core?
                            match tcAltResolutions.TryFindByOriginalReference coreLibraryReference with
                            | Some resolution -> Some resolution
                            | _ -> tcResolutions.TryFindByOriginalReferenceText (GetFSharpCoreLibraryName())  // was the ".dll" elided?
                    
                    match resolvedAssemblyRef with 
                    | Some coreLibraryResolution -> 
                        //printfn "coreLibraryResolution = '%s'" coreLibraryResolution.resolvedPath
                        match frameworkTcImports.RegisterAndImportReferencedAssemblies(None, [coreLibraryResolution]) with
                        | (_, [ResolvedImportedAssembly(fslibCcuInfo) ]) -> fslibCcuInfo
                        | _ -> 
                            error(InternalError("BuildFrameworkTcImports: no successful import of "+coreLibraryResolution.resolvedPath,coreLibraryResolution.originalReference.Range))
                    | None -> 
                        error(InternalError(sprintf "BuildFrameworkTcImports: no resolution of '%s'" coreLibraryReference.Text,rangeStartup))
                IlxSettings.ilxFsharpCoreLibAssemRef := 
                    (let scoref = fslibCcuInfo.ILScopeRef
                     match scoref with
                     | ILScopeRef.Assembly aref             -> Some aref
                     | ILScopeRef.Local | ILScopeRef.Module _ -> error(InternalError("not ILScopeRef.Assembly",rangeStartup)))
                fslibCcuInfo.FSharpViewOfMetadata            
                  
        // Search for a type
        let getTypeCcu nsname typeName =
            if ccuHasType sysCcu.FSharpViewOfMetadata nsname typeName  then 
                  sysCcu.FSharpViewOfMetadata
            else
                let search = 
                    seq { yield sysCcu.FSharpViewOfMetadata; 
                          yield! frameworkTcImports.GetCcusInDeclOrder() 
                          for dllName in SystemAssemblies tcConfig.primaryAssembly.Name do 
                            match frameworkTcImports.CcuTable.TryFind dllName with 
                            | Some sysCcu -> yield sysCcu.FSharpViewOfMetadata
                            | None -> () }
                    |> Seq.tryFind (fun ccu -> ccuHasType ccu nsname typeName)
                match search with 
                | Some x -> x
                | None -> fslibCcu
        
        // REVIEW: We use this in some places to work around bugs in the 2.0 runtime.
        // Silverlight 4.0 will have some of these fixes, but their version number is 2.0.5.0.
        // If we ever modify the compiler to run on Silverlight, we'll need to update this mechanism.
        let using40environment = 
            match ilGlobals.traits.ScopeRef.AssemblyRef.Version with 
            | Some (v1, _v2, _v3, _v4)  -> v1 >= 4us 
            | _ -> true

        // OK, now we have both mscorlib.dll and FSharp.Core.dll we can create TcGlobals
        let tcGlobals = mkTcGlobals(tcConfig.compilingFslib,sysCcu.FSharpViewOfMetadata,ilGlobals,fslibCcu,
                                    tcConfig.implicitIncludeDir,tcConfig.mlCompatibility,using40environment,tcConfig.indirectCallArrayMethods,
                                    tcConfig.isInteractive,getTypeCcu, tcConfig.emitDebugInfoInQuotations) 

#if DEBUG
        // the global_g reference cell is used only for debug printing
        global_g := Some tcGlobals
#endif
        // do this prior to parsing, since parsing IL assembly code may refer to mscorlib
#if NO_INLINE_IL_PARSER
        // inline IL not permitted by hostable compiler
#else
        Microsoft.FSharp.Compiler.AbstractIL.Internal.AsciiConstants.parseILGlobals := tcGlobals.ilg 
#endif
        frameworkTcImports.SetTcGlobals(tcGlobals)
        tcGlobals,frameworkTcImports

    member tcImports.ReportUnresolvedAssemblyReferences(knownUnresolved) =
        // Report that an assembly was not resolved.
        let reportAssemblyNotResolved(file,originalReferences:AssemblyReference list) = 
            originalReferences |> List.iter(fun originalReference -> errorR(AssemblyNotResolved(file,originalReference.Range)))
        knownUnresolved
        |> List.map (function UnresolvedAssemblyReference(file,originalReferences) -> file,originalReferences)
        |> List.iter reportAssemblyNotResolved
        
    // Note: This returns a TcImports object. TcImports are disposable - the caller owns the returned TcImports object 
    // and when hosted in Visual Studio or another long-running process must dispose this object. 
    static member BuildNonFrameworkTcImports (displayPSTypeProviderSecurityDialogBlockingUI : (string->unit) option, tcConfigP:TcConfigProvider, tcGlobals:TcGlobals, baseTcImports, nonFrameworkReferences, knownUnresolved) = 
        let tcConfig = tcConfigP.Get()
        let tcResolutions = TcAssemblyResolutions.BuildFromPriorResolutions(tcConfig,nonFrameworkReferences,knownUnresolved)
        let references = tcResolutions.GetAssemblyResolutions()
        let tcImports = new TcImports(tcConfigP,tcResolutions,Some baseTcImports, Some tcGlobals.ilg)
        tcImports.DoRegisterAndImportReferencedAssemblies(displayPSTypeProviderSecurityDialogBlockingUI, references)
        tcImports.ReportUnresolvedAssemblyReferences(knownUnresolved)
        tcImports
      
    // Note: This returns a TcImports object. TcImports are disposable - the caller owns the returned TcImports object 
    // and if hosted in Visual Studio or another long-running process must dispose this object. However this
    // function is currently only used from fsi.exe. If we move to a long-running hosted evaluation service API then
    // we should start disposing these objects.
    static member BuildTcImports(tcConfigP:TcConfigProvider) = 
        let tcConfig = tcConfigP.Get()
        //let foundationalTcImports,tcGlobals = TcImports.BuildFoundationalTcImports(tcConfigP)
        let frameworkDLLs,nonFrameworkReferences,knownUnresolved = TcAssemblyResolutions.SplitNonFoundationalResolutions(tcConfig)
        let tcGlobals,frameworkTcImports = TcImports.BuildFrameworkTcImports (tcConfigP,frameworkDLLs,nonFrameworkReferences)
        let tcImports = TcImports.BuildNonFrameworkTcImports(None, tcConfigP,tcGlobals,frameworkTcImports,nonFrameworkReferences,knownUnresolved)
        tcGlobals,tcImports
        
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

//----------------------------------------------------------------------------
// Add "#r" and "#I" declarations to the tcConfig
//--------------------------------------------------------------------------

// Add the reference and add the ccu to the type checking environment . Only used by F# Interactive
let RequireDLL (tcImports:TcImports) tcEnv m file = 
    let RequireResolved = function
        | ResolvedImportedAssembly(ccuinfo) -> ccuinfo
        | UnresolvedImportedAssembly(assemblyName) -> error(Error(FSComp.SR.buildCouldNotResolveAssemblyRequiredByFile(assemblyName,file),m))
    let resolutions = CommitOperationResult(tcImports.TryResolveAssemblyReference(AssemblyReference(m,file),ResolveAssemblyReferenceMode.ReportErrors))
    let dllinfos,ccuinfos = tcImports.RegisterAndImportReferencedAssemblies(None, resolutions)
    let ccuinfos = ccuinfos |> List.map RequireResolved
    let g = tcImports.GetTcGlobals()
    let amap = tcImports.GetImportMap()
    let tcEnv = ccuinfos |> List.fold (fun tcEnv ccuinfo -> Tc.AddCcuToTcEnv(g,amap,m,tcEnv,ccuinfo.FSharpViewOfMetadata,ccuinfo.AssemblyAutoOpenAttributes,false)) tcEnv 
    tcEnv,(dllinfos,ccuinfos)

       
       
let ProcessMetaCommandsFromInput 
     (nowarnF: 'state -> range * string -> 'state,
      dllRequireF: 'state -> range * string -> 'state,
      loadSourceF: 'state -> range * string -> unit) 
     (tcConfig:TcConfigBuilder) 
     inp 
     pathOfMetaCommandSource
     state0 =
     
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)

    let canHaveScriptMetaCommands = 
        match inp with 
        | ParsedInput.SigFile(_) ->  false
        | ParsedInput.ImplFile(ParsedImplFileInput(_,isScript,_,_,_,_,_)) -> isScript

    let ProcessMetaCommand state hash  =
        let mutable matchedm = range0
        try 
            match hash with 
            | ParsedHashDirective("I",args,m) ->
               if not canHaveScriptMetaCommands then 
                   errorR(HashIncludeNotAllowedInNonScript(m))
               match args with 
               | [path] -> 
                   matchedm<-m
                   tcConfig.AddIncludePath(m,path,pathOfMetaCommandSource)
                   state
               | _ -> 
                   errorR(Error(FSComp.SR.buildInvalidHashIDirective(),m))
                   state
            | ParsedHashDirective("nowarn",numbers,m) ->
               List.fold (fun state d -> nowarnF state (m,d)) state numbers
            | ParsedHashDirective(("reference" | "r"),args,m) -> 
               if not canHaveScriptMetaCommands then 
                   errorR(HashReferenceNotAllowedInNonScript(m))
               match args with 
               | [path] -> 
                   matchedm<-m
                   dllRequireF state (m,path)
               | _ -> 
                   errorR(Error(FSComp.SR.buildInvalidHashrDirective(),m))
                   state
            | ParsedHashDirective("load",args,m) -> 
               if not canHaveScriptMetaCommands then 
                   errorR(HashDirectiveNotAllowedInNonScript(m))
               match args with 
               | _ :: _ -> 
                  matchedm<-m
                  args |> List.iter (fun path -> loadSourceF state (m,path))
               | _ -> 
                  errorR(Error(FSComp.SR.buildInvalidHashloadDirective(),m))
               state
            | ParsedHashDirective("time",args,m) -> 
               if not canHaveScriptMetaCommands then 
                   errorR(HashDirectiveNotAllowedInNonScript(m))
               match args with 
               | [] -> 
                   ()
               | ["on" | "off"] -> 
                   ()
               | _ -> 
                   errorR(Error(FSComp.SR.buildInvalidHashtimeDirective(),m))
               state
               
            | _ -> 
               
            (* warning(Error("This meta-command has been ignored",m)); *) 
               state
        with e -> errorRecovery e matchedm; state

    let rec WarnOnIgnoredSpecDecls decls = 
        decls |> List.iter (fun d -> 
            match d with 
            | SynModuleSigDecl.HashDirective (_,m) -> warning(Error(FSComp.SR.buildDirectivesInModulesAreIgnored(),m)) 
            | SynModuleSigDecl.NestedModule (_,subDecls,_) -> WarnOnIgnoredSpecDecls subDecls
            | _ -> ())

    let rec WarnOnIgnoredImplDecls decls = 
        decls |> List.iter (fun d -> 
            match d with 
            | SynModuleDecl.HashDirective (_,m) -> warning(Error(FSComp.SR.buildDirectivesInModulesAreIgnored(),m)) 
            | SynModuleDecl.NestedModule (_,subDecls,_,_) -> WarnOnIgnoredImplDecls subDecls
            | _ -> ())

    let ProcessMetaCommandsFromModuleSpec state (SynModuleOrNamespaceSig(_,_,decls,_,_,_,_)) =
        List.fold (fun s d -> 
            match d with 
            | SynModuleSigDecl.HashDirective (h,_) -> ProcessMetaCommand s h
            | SynModuleSigDecl.NestedModule (_,subDecls,_) -> WarnOnIgnoredSpecDecls subDecls; s
            | _ -> s)
         state
         decls 

    let ProcessMetaCommandsFromModuleImpl state (SynModuleOrNamespace(_,_,decls,_,_,_,_)) =
        List.fold (fun s d -> 
            match d with 
            | SynModuleDecl.HashDirective (h,_) -> ProcessMetaCommand s h
            | SynModuleDecl.NestedModule (_,subDecls,_,_) -> WarnOnIgnoredImplDecls subDecls; s
            | _ -> s)
         state
         decls

    match inp with 
    | ParsedInput.SigFile(ParsedSigFileInput(_,_,_,hashDirectives,specs)) -> 
        let state = List.fold ProcessMetaCommand state0 hashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleSpec state specs
        state
    | ParsedInput.ImplFile(ParsedImplFileInput(_,_,_,_,hashDirectives,impls,_)) -> 
        let state = List.fold ProcessMetaCommand state0 hashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleImpl state impls
        state

let ApplyNoWarnsToTcConfig (tcConfig:TcConfig) (inp:ParsedInput,pathOfMetaCommandSource) = 
    // Clone
    let tcConfigB = tcConfig.CloneOfOriginalBuilder 
    let addNoWarn = fun () (m,s) -> tcConfigB.TurnWarningOff(m,s)
    let addReferencedAssemblyByPath = fun () (_m,_s) -> ()
    let addLoadedSource = fun () (_m,_s) -> ()
    ProcessMetaCommandsFromInput (addNoWarn, addReferencedAssemblyByPath, addLoadedSource) tcConfigB inp pathOfMetaCommandSource ()
    TcConfig.Create(tcConfigB,validate=false)

let ApplyMetaCommandsFromInputToTcConfig (tcConfig:TcConfig) (inp:ParsedInput,pathOfMetaCommandSource) = 
    // Clone
    let tcConfigB = tcConfig.CloneOfOriginalBuilder 
    let getWarningNumber = fun () _ -> () 
    let addReferencedAssemblyByPath = fun () (m,s) -> tcConfigB.AddReferencedAssemblyByPath(m,s)
    let addLoadedSource = fun () (m,s) -> tcConfigB.AddLoadedSource(m,s,pathOfMetaCommandSource)
    ProcessMetaCommandsFromInput (getWarningNumber, addReferencedAssemblyByPath, addLoadedSource) tcConfigB inp pathOfMetaCommandSource ()
    TcConfig.Create(tcConfigB,validate=false)

let GetAssemblyResolutionInformation(tcConfig : TcConfig) : AssemblyResolution list * UnresolvedAssemblyReference list =
    use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
    let assemblyList = TcAssemblyResolutions.GetAllDllReferences(tcConfig)
    let resolutions = TcAssemblyResolutions.Resolve(tcConfig,assemblyList,[])
    resolutions.GetAssemblyResolutions(),resolutions.GetUnresolvedReferences()
    
type LoadClosure = {
        /// The source files along with the ranges of the #load positions in each file.
        SourceFiles: (string * range list) list
        /// The resolved references along with the ranges of the #r positions in each file.
        References: (string * AssemblyResolution list) list
        /// The list of references that were not resolved during load closure. These may still be extension references.
        UnresolvedReferences : UnresolvedAssemblyReference list
        /// The list of all sources in the closure with inputs when available
        Inputs: (string * ParsedInput option) list
        /// The #nowarns
        NoWarns: (string * range list) list
        /// Errors seen while parsing root of closure
        RootErrors : PhasedError list
        /// Warnings seen while parsing root of closure
        RootWarnings : PhasedError list        
    }
    
[<RequireQualifiedAccess>]
type CodeContext =
    | Evaluation // in fsi.exe
    | Compilation  // in fsc.exe
    | Editing // in VS
    

module private ScriptPreprocessClosure = 
    open Internal.Utilities.Text.Lexing
    
    type private ClosureDirective = 
        | SourceFile of string * range * string // filename, range, source text
        | ClosedSourceFile of string * range * ParsedInput option * PhasedError list * PhasedError list * (string * range) list // filename, range, errors, warnings, nowarns
        
    type private Observed() =
        let seen = System.Collections.Generic.Dictionary<_,bool>()
        member ob.SetSeen(check) = 
            if not(seen.ContainsKey(check)) then 
                seen.Add(check,true)
        
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
            | CodeContext.Evaluation -> ["INTERACTIVE"]
            | CodeContext.Compilation -> ["COMPILED"]
            | CodeContext.Editing -> "EDITING" :: (if IsScript filename then ["INTERACTIVE"] else ["COMPILED"])
        let lexbuf = UnicodeLexing.StringAsLexbuf source 
        
        let isLastCompiland = IsScript filename // The root compiland is last in the list of compilands.
        ParseOneInputLexbuf (tcConfig,lexResourceManager,defines,lexbuf,filename,isLastCompiland,errorLogger) 
          
    /// Create a TcConfig for load closure starting from a single .fsx file
    let CreateScriptSourceTcConfig(filename:string,codeContext) =  
        let projectDir = Path.GetDirectoryName(filename)
        let isInteractive = (codeContext = CodeContext.Evaluation)
        let isInvalidationSupported = (codeContext = CodeContext.Editing)
        // always use primary assembly = mscorlib for scripts
        let tcConfigB = TcConfigBuilder.CreateNew(Internal.Utilities.FSharpEnvironment.BinFolderOfDefaultFSharpCompiler.Value, true (* optimize for memory *), projectDir, isInteractive, isInvalidationSupported) 
        BasicReferencesForScriptLoadClosure |> List.iter(fun f->tcConfigB.AddReferencedAssemblyByPath(range0,f)) // Add script references
        tcConfigB.resolutionEnvironment <-
            match codeContext with 
            | CodeContext.Editing -> MSBuildResolver.DesigntimeLike
            | CodeContext.Compilation | CodeContext.Evaluation -> MSBuildResolver.RuntimeLike
        tcConfigB.framework <- false 
        // Indicates that there are some references not in BasicReferencesForScriptLoadClosure which should
        // be added conditionally once the relevant version of mscorlib.dll has been detected.
        tcConfigB.addVersionSpecificFrameworkReferences <- true 
        tcConfigB.implicitlyResolveAssemblies <- false
        TcConfig.Create(tcConfigB,validate=true)
        
    let private SourceFileOfFilename(filename,m,inputCodePage:int option) : ClosureDirective list = 
        try
            let filename = FileSystem.SafeGetFullPath(filename)
            use stream = FileSystem.FileStreamReadShim filename
            use reader = 
                match inputCodePage with 
                | None -> new  StreamReader(stream,true)
                | Some n -> new  StreamReader(stream,Encoding.GetEncodingShim(n)) 
            let source = reader.ReadToEnd()
            [SourceFile(filename,m,source)]
        with e -> 
            errorRecovery e m 
            []
            
    let ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn (tcConfig:TcConfig) (inp:ParsedInput,pathOfMetaCommandSource) = 
        let tcConfigB = tcConfig.CloneOfOriginalBuilder 
        let nowarns = ref [] 
        let getWarningNumber = fun () (m,s) -> nowarns := (s,m) :: !nowarns
        let addReferencedAssemblyByPath = fun () (m,s) -> tcConfigB.AddReferencedAssemblyByPath(m,s)
        let addLoadedSource = fun () (m,s) -> tcConfigB.AddLoadedSource(m,s,pathOfMetaCommandSource)
        try 
            ProcessMetaCommandsFromInput (getWarningNumber, addReferencedAssemblyByPath, addLoadedSource) tcConfigB inp pathOfMetaCommandSource ()
        with ReportedError _ ->
            // Recover by using whatever did end up in the tcConfig
            ()
            
        try
            TcConfig.Create(tcConfigB,validate=false),nowarns
        with ReportedError _ ->
            // Recover by  using a default TcConfig.
            let tcConfigB = tcConfig.CloneOfOriginalBuilder 
            TcConfig.Create(tcConfigB,validate=false),nowarns
    
    let private FindClosureDirectives(closureDirectives,tcConfig:TcConfig,codeContext,lexResourceManager:Lexhelp.LexResourceManager) =
        let tcConfig = ref tcConfig
        
        let observedSources = Observed()
        let rec FindClosure (closureDirective:ClosureDirective) : ClosureDirective list = 
            match closureDirective with 
            | ClosedSourceFile _ as csf -> [csf]
            | SourceFile(filename,m,source) ->
                let filename = FileSystem.SafeGetFullPath(filename)
                if observedSources.HaveSeen(filename) then [] 
                else     
                    observedSources.SetSeen(filename)
                    
                    let errors = ref []
                    let warnings = ref [] 
                    let errorLogger = 
                         { new ErrorLogger("FindClosure") with 
                               member x.ErrorSinkImpl(e) = errors := e :: !errors
                               member x.WarnSinkImpl(e) = warnings := e :: !warnings
                               member x.ErrorCount = (!errors).Length }                        

                    use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
                    let pathOfMetaCommandSource = Path.GetDirectoryName(filename)
                    match ParseScriptText(filename,source,!tcConfig,codeContext,lexResourceManager,errorLogger) with 
                    | Some(input) ->                    
                        let tcConfigResult, noWarns = ApplyMetaCommandsFromInputToTcConfigAndGatherNoWarn !tcConfig (input,pathOfMetaCommandSource)
                        tcConfig := tcConfigResult
                        
                        let AddFileIfNotSeen(m,filename) = 
                            if observedSources.HaveSeen(filename) then []
                            else
                                if IsScript(filename) then SourceFileOfFilename(filename,m,tcConfigResult.inputCodePage)
                                else 
                                    observedSources.SetSeen(filename)
                                    [ClosedSourceFile(filename,m,None,[],[],[])] // Don't traverse into .fs leafs.
                        
                        let loadedSources = (!tcConfig).GetAvailableLoadedSources() |> List.rev |> List.map AddFileIfNotSeen |> List.concat
                        ClosedSourceFile(filename,m,Some(input),!errors,!warnings,!noWarns) :: loadedSources |> List.map FindClosure |> List.concat // Final closure is in reverse order. Keep the closed source at the top.
                    | None -> [ClosedSourceFile(filename,m,None,!errors,!warnings,[])]

        closureDirectives |> List.map FindClosure |> List.concat, !tcConfig
        
    /// Reduce the full directive closure into LoadClosure
    let private GetLoadClosure(rootFilename,closureDirectives,tcConfig,codeContext) = 
    
        // Mark the last file as isLastCompiland. closureDirectives is currently reversed.
        let closureDirectives =
            match closureDirectives with
            | ClosedSourceFile(filename,m,Some(ParsedInput.ImplFile(ParsedImplFileInput(name,isScript,qualNameOfFile,scopedPragmas,hashDirectives,implFileFlags,_))),errs,warns,nowarns)::rest -> 
                ClosedSourceFile(filename,m,Some(ParsedInput.ImplFile(ParsedImplFileInput(name,isScript,qualNameOfFile,scopedPragmas,hashDirectives,implFileFlags,true))),errs,warns,nowarns)::rest
            | x -> x

        // Get all source files.
        let sourceFiles = ref []
        let sourceInputs = ref []
        let globalNoWarns = ref []
        let ExtractOne = function
            | ClosedSourceFile(filename,m,input,_,_,noWarns) -> 
                let filename = FileSystem.SafeGetFullPath(filename)
                sourceFiles := (filename,m) :: !sourceFiles  
                globalNoWarns := (!globalNoWarns @ noWarns) 
                sourceInputs := (filename,input) :: !sourceInputs                 
            | _ -> failwith "Unexpected"
            
        closureDirectives |> List.iter ExtractOne // This unreverses the list of sources 
        
        // Resolve all references.
        let resolutionErrors = ref []
        let resolutionWarnings = ref [] 
        let errorLogger = 
            { new ErrorLogger("GetLoadClosure") with 
               member x.ErrorSinkImpl(e) = resolutionErrors := e :: !resolutionErrors
               member x.WarnSinkImpl(e) = resolutionWarnings := e :: !resolutionWarnings
               member x.ErrorCount = (!resolutionErrors).Length }      
        
        let references,unresolvedReferences = 
            use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger) 
            GetAssemblyResolutionInformation(tcConfig)
        let references =  references |> List.map (fun ar -> ar.resolvedPath,ar)
        
        // Root errors and warnings
        let rootErrors, rootWarnings = 
            match closureDirectives with
            | ClosedSourceFile(_,_,_,errors,warnings,_) :: _ -> errors @ !resolutionErrors, warnings @ !resolutionWarnings
            | _ -> [],[] // When no file existed.
        
        let isRootRange exn =
            match RangeOfError exn with
            | Some m -> 
                // Return true if the error was *not* from a #load-ed file.
                let isArgParameterWhileNotEditing = (codeContext <> CodeContext.Editing) && (m = range0 || m = rangeStartup || m = rangeCmdArgs)
                let isThisFileName = (0 = String.Compare(rootFilename, m.FileName, StringComparison.OrdinalIgnoreCase))
                isArgParameterWhileNotEditing || isThisFileName
            | None -> true
        
        // Filter out non-root errors and warnings
        let rootErrors = rootErrors |> List.filter isRootRange
        let rootWarnings = rootWarnings |> List.filter isRootRange
        
        let result = {SourceFiles = List.groupByFirst !sourceFiles
                      References = List.groupByFirst references
                      UnresolvedReferences = unresolvedReferences
                      Inputs = !sourceInputs
                      NoWarns = List.groupByFirst !globalNoWarns
                      RootErrors = rootErrors
                      RootWarnings = rootWarnings}       
        result
        
    /// Given source text, find the full load closure
    /// Used from service.fs, when editing a script file
    let GetFullClosureOfScriptSource(filename,source,codeContext,lexResourceManager:Lexhelp.LexResourceManager) = 
        let tcConfig = CreateScriptSourceTcConfig(filename,codeContext)
        let protoClosure = [SourceFile(filename,range0,source)]
        let finalClosure,tcConfig = FindClosureDirectives(protoClosure,tcConfig,codeContext,lexResourceManager)
        GetLoadClosure(filename,finalClosure,tcConfig,codeContext)
        
    /// Given source filename, find the full load closure
    /// Used from fsi.fs and fsc.fs, for #load and command line
    let GetFullClosureOfScriptFiles(tcConfig:TcConfig,files:(string*range) list,codeContext,_useDefaultScriptingReferences:bool,lexResourceManager:Lexhelp.LexResourceManager) = 
        let mainFile = fst (List.head files)
        let protoClosure = files |> List.map (fun (filename,m)->SourceFileOfFilename(filename,m,tcConfig.inputCodePage)) |> List.concat |> List.rev // Reverse to put them in the order they will be extracted later
        let finalClosure,tcConfig = FindClosureDirectives(protoClosure,tcConfig,codeContext,lexResourceManager)
        GetLoadClosure(mainFile,finalClosure,tcConfig,codeContext)        

type LoadClosure with
    // Used from service.fs, when editing a script file
    static member ComputeClosureOfSourceText(filename:string,source:string,codeContext,lexResourceManager:Lexhelp.LexResourceManager) : LoadClosure = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)
        ScriptPreprocessClosure.GetFullClosureOfScriptSource(filename,source,codeContext,lexResourceManager)

    /// Used from fsi.fs and fsc.fs, for #load and command line.
    /// The resulting references are then added to a TcConfig.
    static member ComputeClosureOfSourceFiles(tcConfig:TcConfig,files:(string*range) list,codeContext,useDefaultScriptingReferences:bool,lexResourceManager:Lexhelp.LexResourceManager) : LoadClosure = 
        use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)
        ScriptPreprocessClosure.GetFullClosureOfScriptFiles(tcConfig,files,codeContext,useDefaultScriptingReferences,lexResourceManager)
        
              

//----------------------------------------------------------------------------
// Build the initial type checking environment
//--------------------------------------------------------------------------

let implicitOpen tcGlobals amap m tcEnv p =
    if verbose then dprintf "opening %s\n" p 
    Tc.TcOpenDecl TcResultsSink.NoSink tcGlobals amap m m tcEnv (pathToSynLid m (splitNamespace p))

let GetInitialTypecheckerEnv (assemblyName:string option) (initm:range) (tcConfig:TcConfig) (tcImports:TcImports) tcGlobals  =    
    let initm = initm.StartRange
    if verbose then dprintf "--- building initial tcEnv\n"         
    let internalsAreVisibleHere (ccuinfo:ImportedAssembly) =
        match assemblyName with
        | None -> false
        | Some assemblyName ->
            let isTargetAssemblyName (visibleTo:string) =             
                try                    
                    System.Reflection.AssemblyName(visibleTo).Name = assemblyName                
                with e ->
                    warning(InvalidInternalsVisibleToAssemblyName(visibleTo,ccuinfo.FSharpViewOfMetadata.FileName))
                    false
            let internalsVisibleTos = ccuinfo.AssemblyInternalsVisibleToAttributes
            List.exists isTargetAssemblyName internalsVisibleTos
    let ccus = tcImports.GetImportedAssemblies() |> List.map (fun ccuinfo -> ccuinfo.FSharpViewOfMetadata,
                                                                             ccuinfo.AssemblyAutoOpenAttributes,
                                                                             ccuinfo |> internalsAreVisibleHere)    
    let amap = tcImports.GetImportMap()
    let tcEnv = Tc.CreateInitialTcEnv(tcGlobals,amap,initm,ccus) |> (fun tce ->
            if tcConfig.checkOverflow then
                List.fold (implicitOpen tcGlobals amap initm) tce [FSharpLib.CoreOperatorsCheckedName]
            else
                tce)
    if verbose then dprintf "--- opening implicit paths\n" 
    if verbose then dprintf "--- GetInitialTypecheckerEnv, top modules = %s\n" (String.concat ";" (NameMap.domainL tcEnv.NameEnv.eModulesAndNamespaces)) 
    if verbose then dprintf "<-- GetInitialTypecheckerEnv\n" 
    tcEnv

//----------------------------------------------------------------------------
// TYPECHECK
//--------------------------------------------------------------------------

(* The incremental state of type checking files *)
(* REVIEW: clean this up  *)

type RootSigs =  Zmap<QualifiedNameOfFile, ModuleOrNamespaceType>
type RootImpls = Zset<QualifiedNameOfFile >
type TypecheckerSigsAndImpls = RootSigsAndImpls of RootSigs * RootImpls * ModuleOrNamespaceType * ModuleOrNamespaceType

let qnameOrder = Order.orderBy (fun (q:QualifiedNameOfFile) -> q.Text)

type TcState = 
    { tcsCcu: CcuThunk
      tcsCcuType: ModuleOrNamespace
      tcsNiceNameGen: NiceNameGenerator
      tcsTcSigEnv: TcEnv
      tcsTcImplEnv: TcEnv
      (* The accumulated results of type checking for this assembly *)
      tcsRootSigsAndImpls : TypecheckerSigsAndImpls }
    member x.NiceNameGenerator = x.tcsNiceNameGen
    member x.TcEnvFromSignatures = x.tcsTcSigEnv
    member x.TcEnvFromImpls = x.tcsTcImplEnv
    member x.Ccu = x.tcsCcu
 
    member x.NextStateAfterIncrementalFragment(tcEnvAtEndOfLastInput) = 
        { x with tcsTcSigEnv = tcEnvAtEndOfLastInput
                 tcsTcImplEnv = tcEnvAtEndOfLastInput } 

 
let TypecheckInitialState(m,ccuName,tcConfig:TcConfig,tcGlobals,tcImports:TcImports,niceNameGen,tcEnv0) =
    ignore tcImports
    if verbose then dprintf "Typecheck (constructing initial state)....\n"
    // Create a ccu to hold all the results of compilation 
    let ccuType = NewCcuContents ILScopeRef.Local m ccuName (NewEmptyModuleOrNamespaceType Namespace)
    let ccu = 
      CcuThunk.Create(ccuName,{IsFSharp=true
                               UsesQuotations=false
#if EXTENSIONTYPING
                               InvalidateEvent=(new Event<_>()).Publish
                               IsProviderGenerated = false
                               ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
                               FileName=None 
                               Stamp = newStamp()
                               QualifiedName= None
                               SourceCodeDirectory = tcConfig.implicitIncludeDir 
                               ILScopeRef=ILScopeRef.Local
                               Contents=ccuType
                               MemberSignatureEquality= (Tastops.typeEquivAux EraseAll tcGlobals)
                               TypeForwarders=Map.empty })

    (* OK, is this is the F# library CCU then fix it up. *)
    if tcConfig.compilingFslib then 
        tcGlobals.fslibCcu.Fixup(ccu)
      
    let rootSigs = Zmap.empty qnameOrder
    let rootImpls = Zset.empty qnameOrder
    let allSigModulTyp = NewEmptyModuleOrNamespaceType Namespace
    let allImplementedSigModulTyp = NewEmptyModuleOrNamespaceType Namespace
    { tcsCcu= ccu
      tcsCcuType=ccuType
      tcsNiceNameGen=niceNameGen
      tcsTcSigEnv=tcEnv0
      tcsTcImplEnv=tcEnv0
      tcsRootSigsAndImpls = RootSigsAndImpls (rootSigs, rootImpls, allSigModulTyp, allImplementedSigModulTyp) }

let CheckSimulateException(tcConfig:TcConfig) = 
    match tcConfig.simulateException with
    | Some("tc-oom") -> raise(System.OutOfMemoryException())
    | Some("tc-an") -> raise(System.ArgumentNullException("simulated"))
    | Some("tc-invop") -> raise(System.InvalidOperationException())
    | Some("tc-av") -> raise(System.AccessViolationException())
    | Some("tc-aor") -> raise(System.ArgumentOutOfRangeException())
    | Some("tc-dv0") -> raise(System.DivideByZeroException())
    | Some("tc-nfn") -> raise(System.NotFiniteNumberException())
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


(* Typecheck a single file or interactive entry into F# Interactive *)
let TypecheckOneInputEventually
      (checkForErrors , tcConfig:TcConfig, tcImports:TcImports,  
       tcGlobals, prefixPathOpt, tcSink, tcState: TcState, inp: ParsedInput) =
  eventually {
   try 
      CheckSimulateException(tcConfig)
      let (RootSigsAndImpls(rootSigs,rootImpls,allSigModulTyp,allImplementedSigModulTyp)) = tcState.tcsRootSigsAndImpls
      let m = inp.Range
      let amap = tcImports.GetImportMap()
      let! (topAttrs, mimpls,tcEnvAtEnd,tcSigEnv,tcImplEnv,topSigsAndImpls,ccuType) = 
        eventually {
            match inp with 
            | ParsedInput.SigFile (ParsedSigFileInput(filename,qualNameOfFile, _,_,_) as file) ->
                
                // Check if we've seen this top module signature before. 
                if Zmap.mem qualNameOfFile rootSigs then 
                    errorR(Error(FSComp.SR.buildSignatureAlreadySpecified(qualNameOfFile.Text),m.StartRange))

                (* Check if the implementation came first in compilation order *)
                if Zset.contains qualNameOfFile rootImpls then 
                    errorR(Error(FSComp.SR.buildImplementationAlreadyGivenDetail(qualNameOfFile.Text),m))

                // Typecheck the signature file 
#if DEBUG
                if !verboseStamps then 
                    dprintf "---------------------- START CHECK %A ------------\n" filename
#else
                filename |> ignore
#endif
                let! (tcEnvAtEnd,tcEnv,smodulTypeRoot) = 
                    Tc.TypecheckOneSigFile (tcGlobals,tcState.tcsNiceNameGen,amap,tcState.tcsCcu,checkForErrors,tcConfig.conditionalCompilationDefines,tcSink) tcState.tcsTcSigEnv file

#if DEBUG
                if !verboseStamps then 
                    dprintf "Type-checked signature:\n%s\n" (Layout.showL (Layout.squashTo 192 (entityTypeL smodulTypeRoot)))
                    dprintf "---------------------- END CHECK %A ------------\n" filename
#endif

                let rootSigs = Zmap.add qualNameOfFile  smodulTypeRoot rootSigs

                // Open the prefixPath for fsi.exe 
                let tcEnv = 
                    match prefixPathOpt with 
                    | None -> tcEnv 
                    | Some prefixPath -> 
                        let m = qualNameOfFile.Range
                        TcOpenDecl tcSink tcGlobals amap m m tcEnv prefixPath

                let res = (EmptyTopAttrs, [],tcEnvAtEnd,tcEnv,tcState.tcsTcImplEnv,RootSigsAndImpls(rootSigs,rootImpls, allSigModulTyp, allImplementedSigModulTyp  ),tcState.tcsCcuType)
                return res

            | ParsedInput.ImplFile (ParsedImplFileInput(filename,_,qualNameOfFile,_,_,_,_) as file) ->
            
                // Check if we've got an interface for this fragment 
                let rootSigOpt = rootSigs.TryFind(qualNameOfFile)

                if verbose then dprintf "ParsedInput.ImplFile, nm = %s, qualNameOfFile = %s, ?rootSigOpt = %b\n" filename qualNameOfFile.Text (isSome rootSigOpt)

                // Check if we've already seen an implementation for this fragment 
                if Zset.contains qualNameOfFile rootImpls then 
                  errorR(Error(FSComp.SR.buildImplementationAlreadyGiven(qualNameOfFile.Text),m))

                let tcImplEnv = tcState.tcsTcImplEnv

#if DEBUG
                if !verboseStamps then 
                    dprintf "---------------------- START CHECK %A ------------\n" filename
#endif
                // Typecheck the implementation file 
                let! topAttrs,implFile,tcEnvAtEnd = 
                    Tc.TypecheckOneImplFile  (tcGlobals,tcState.tcsNiceNameGen,amap,tcState.tcsCcu,checkForErrors,tcConfig.conditionalCompilationDefines,tcSink) tcImplEnv rootSigOpt file

                let hadSig = isSome rootSigOpt
                let implFileSigType = SigTypeOfImplFile implFile

#if DEBUG
                if !verboseStamps then 
                    dprintf "Implementation signature:\n%s\n" (Layout.showL (Layout.squashTo 192 (entityTypeL implFileSigType)))
                    dprintf "---------------------- END CHECK %A ------------\n" filename
#endif

                if verbose then  dprintf "done TypecheckOneImplFile...\n"
                let rootImpls = Zset.add qualNameOfFile rootImpls
        
                // Only add it to the environment if it didn't have a signature 
                let m = qualNameOfFile.Range
                let tcImplEnv = Tc.AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcImplEnv implFileSigType
                let tcSigEnv = 
                    if hadSig then tcState.tcsTcSigEnv 
                    else Tc.AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcState.tcsTcSigEnv implFileSigType
                
                // Open the prefixPath for fsi.exe 
                let tcImplEnv = 
                    match prefixPathOpt with 
                    | None -> tcImplEnv 
                    | Some prefixPath -> 
                        TcOpenDecl tcSink tcGlobals amap m m tcImplEnv prefixPath

                let allImplementedSigModulTyp = combineModuleOrNamespaceTypeList [] m [implFileSigType; allImplementedSigModulTyp]

                // Add it to the CCU 
                let ccuType = 
                    // The signature must be reestablished. 
                    //   [CHECK: Why? This seriously degraded performance] 
                    NewCcuContents ILScopeRef.Local m tcState.tcsCcu.AssemblyName allImplementedSigModulTyp

                if verbose then  dprintf "done TypecheckOneInputEventually...\n"

                let topSigsAndImpls = RootSigsAndImpls(rootSigs,rootImpls,allSigModulTyp,allImplementedSigModulTyp)
                let res = (topAttrs,[implFile], tcEnvAtEnd, tcSigEnv, tcImplEnv,topSigsAndImpls,ccuType)
                return res }
     
      return (tcEnvAtEnd,topAttrs,mimpls),
             { tcState with 
                  tcsCcuType=ccuType
                  tcsTcSigEnv=tcSigEnv
                  tcsTcImplEnv=tcImplEnv
                  tcsRootSigsAndImpls = topSigsAndImpls }
   with e -> 
      errorRecovery e range0 
      return (tcState.TcEnvFromSignatures,EmptyTopAttrs,[]),tcState
 }

let TypecheckOneInput (checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt) tcState  inp =
    // 'use' ensures that the warning handler is restored at the end
    use unwindEL = PushErrorLoggerPhaseUntilUnwind(fun oldLogger -> GetErrorLoggerFilteringByScopedPragmas(false,GetScopedPragmasForInput(inp),oldLogger) )
    use unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.TypeCheck)
    TypecheckOneInputEventually (checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, TcResultsSink.NoSink, tcState, inp) |> Eventually.force

let TypecheckMultipleInputsFinish(results,tcState: TcState) =
    let tcEnvsAtEndFile,topAttrs,mimpls = List.unzip3 results
    
    let topAttrs = List.foldBack CombineTopAttrs topAttrs EmptyTopAttrs
    let mimpls = List.concat mimpls
    // This is the environment required by fsi.exe when incrementally adding definitions 
    let tcEnvAtEndOfLastFile = (match tcEnvsAtEndFile with h :: _ -> h | _ -> tcState.TcEnvFromSignatures)
    if verbose then  dprintf "done TypecheckMultipleInputs...\n"
    
    (tcEnvAtEndOfLastFile,topAttrs,mimpls),tcState

let TypecheckMultipleInputs(checkForErrors,tcConfig:TcConfig,tcImports,tcGlobals,prefixPathOpt,tcState,inputs) =
    let results,tcState =  List.mapFold (TypecheckOneInput (checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt)) tcState inputs
    TypecheckMultipleInputsFinish(results,tcState)

let TypecheckSingleInputAndFinishEventually(checkForErrors,tcConfig:TcConfig,tcImports,tcGlobals,prefixPathOpt,tcSink,tcState,input) =
    eventually {
        let! results,tcState =  TypecheckOneInputEventually(checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input)
        return TypecheckMultipleInputsFinish([results],tcState)
    }

let TypecheckClosedInputSetFinish(mimpls,tcState) =
    // Publish the latest contents to the CCU 
    tcState.tcsCcu.Deref.Contents <- tcState.tcsCcuType

    // Check all interfaces have implementations 
    let (RootSigsAndImpls(rootSigs,rootImpls,_,_)) = tcState.tcsRootSigsAndImpls
    rootSigs |> Zmap.iter (fun qualNameOfFile _ ->  
      if not (Zset.contains qualNameOfFile rootImpls) then 
        errorR(Error(FSComp.SR.buildSignatureWithoutImplementation(qualNameOfFile.Text), qualNameOfFile.Range)))
    if verbose then  dprintf "done TypecheckClosedInputSet...\n"
    let tassembly = TAssembly(mimpls)
    tcState, tassembly    
    
let TypecheckClosedInputSet(checkForErrors,tcConfig,tcImports,tcGlobals,prefixPathOpt,tcState,inputs) =
    // tcEnvAtEndOfLastFile is the environment required by fsi.exe when incrementally adding definitions 
    let (tcEnvAtEndOfLastFile,topAttrs,mimpls),tcState = TypecheckMultipleInputs (checkForErrors,tcConfig,tcImports,tcGlobals,prefixPathOpt,tcState,inputs)
    let tcState,tassembly = TypecheckClosedInputSetFinish (mimpls, tcState)
    tcState, topAttrs, tassembly, tcEnvAtEndOfLastFile

type OptionSwitch = 
    | On
    | Off

type OptionSpec = 
    | OptionClear of bool ref
    | OptionFloat of (float -> unit)
    | OptionInt of (int -> unit)
    | OptionSwitch of (OptionSwitch -> unit)
    | OptionIntList of (int -> unit)
    | OptionIntListSwitch of (int -> OptionSwitch -> unit)
    | OptionRest of (string -> unit)
    | OptionSet of bool ref
    | OptionString of (string -> unit)
    | OptionStringList of (string -> unit)
    | OptionStringListSwitch of (string -> OptionSwitch -> unit)
    | OptionUnit of (unit -> unit)
    | OptionHelp of (CompilerOptionBlock list -> unit)                      // like OptionUnit, but given the "options"
    | OptionGeneral of (string list -> bool) * (string list -> string list) // Applies? * (ApplyReturningResidualArgs)

and  CompilerOption      = CompilerOption of string * string * OptionSpec * Option<exn> * string option
and  CompilerOptionBlock = PublicOptions  of string * CompilerOption list | PrivateOptions of CompilerOption list
let blockOptions = function PublicOptions (_,opts) -> opts | PrivateOptions opts -> opts

let filterCompilerOptionBlock pred block =
  match block with
    | PublicOptions(heading,opts) -> PublicOptions(heading,List.filter pred opts)
    | PrivateOptions(opts)        -> PrivateOptions(List.filter pred opts)

let compilerOptionUsage (CompilerOption(s,tag,spec,_,_)) =
  let s = if s="--" then "" else s (* s="flag" for "--flag" options. s="--" for "--" option. Adjust printing here for "--" case. *)
  match spec with
    | (OptionUnit _ | OptionSet _ | OptionClear _ | OptionHelp _) -> sprintf "--%s" s 
    | OptionStringList _ -> sprintf "--%s:%s" s tag
    | OptionIntList _ -> sprintf "--%s:%s" s tag
    | OptionSwitch _ -> sprintf "--%s[+|-]" s 
    | OptionStringListSwitch _ -> sprintf "--%s[+|-]:%s" s tag
    | OptionIntListSwitch _ -> sprintf "--%s[+|-]:%s" s tag
    | OptionString _ -> sprintf "--%s:%s" s tag
    | OptionInt _ -> sprintf "--%s:%s" s tag
    | OptionFloat _ ->  sprintf "--%s:%s" s tag         
    | OptionRest _ -> sprintf "--%s ..." s
    | OptionGeneral _  -> if tag="" then sprintf "%s" s else sprintf "%s:%s" s tag (* still being decided *)

let printCompilerOption (CompilerOption(_s,_tag,_spec,_,help) as compilerOption) =
    let flagWidth = 30 // fixed width for printing of flags, e.g. --warnaserror:<warn;...>
    let defaultLineWidth = 80 // the fallback width
    let lineWidth = try System.Console.BufferWidth with e -> defaultLineWidth
    let lineWidth = if lineWidth=0 then defaultLineWidth else lineWidth (* Have seen BufferWidth=0 on Linux/Mono *)
    // Lines have this form: <flagWidth><space><description>
    //   flagWidth chars - for flags description or padding on continuation lines.
    //   single space    - space.
    //   description     - words upto but excluding the final character of the line.
    assert(flagWidth = 30)
    printf "%-30s" (compilerOptionUsage compilerOption)
    let printWord column (word:string) =
        // Have printed upto column.
        // Now print the next word including any preceeding whitespace.
        // Returns the column printed to (suited to folding).
        if column + 1 (*space*) + word.Length >= lineWidth then // NOTE: "equality" ensures final character of the line is never printed
          printfn "" (* newline *)
          assert(flagWidth = 30)
          printf  "%-30s %s" ""(*<--flags*) word
          flagWidth + 1 + word.Length
        else
          printf  " %s" word
          column + 1 + word.Length
    let words = match help with None -> [| |] | Some s -> s.Split [| ' ' |]
    let _finalColumn = Array.fold printWord flagWidth words
    printfn "" (* newline *)

let printPublicOptions (heading,opts) =
  if nonNil opts then
    printfn ""
    printfn ""      
    printfn "\t\t%s" heading
    List.iter printCompilerOption opts

let printCompilerOptionBlocks blocks =
  let equals x y = x=y
  let publicBlocks = List.choose (function PrivateOptions _ -> None | PublicOptions (heading,opts) -> Some (heading,opts)) blocks
  let consider doneHeadings (heading, _opts) =
    if Set.contains heading doneHeadings then
      doneHeadings
    else
      let headingOptions = List.filter (fst >> equals heading) publicBlocks |> List.map snd |> List.concat
      printPublicOptions (heading,headingOptions)
      Set.add heading doneHeadings
  List.fold consider Set.empty publicBlocks |> ignore<Set<string>>

(* For QA *)
let dumpCompilerOption prefix (CompilerOption(str, _, spec, _, _)) =
    printf "section='%-25s' ! option=%-30s kind=" prefix str
    match spec with
      | OptionUnit             _ -> printf "OptionUnit"
      | OptionSet              _ -> printf "OptionSet"
      | OptionClear            _ -> printf "OptionClear"
      | OptionHelp             _ -> printf "OptionHelp"
      | OptionStringList       _ -> printf "OptionStringList"
      | OptionIntList          _ -> printf "OptionIntList"
      | OptionSwitch           _ -> printf "OptionSwitch"
      | OptionStringListSwitch _ -> printf "OptionStringListSwitch"
      | OptionIntListSwitch    _ -> printf "OptionIntListSwitch"
      | OptionString           _ -> printf "OptionString"
      | OptionInt              _ -> printf "OptionInt"
      | OptionFloat            _ -> printf "OptionFloat"
      | OptionRest             _ -> printf "OptionRest"
      | OptionGeneral          _ -> printf "OptionGeneral"
    printf "\n"
let dumpCompilerOptionBlock = function
  | PublicOptions (heading,opts) -> List.iter (dumpCompilerOption heading)     opts
  | PrivateOptions opts          -> List.iter (dumpCompilerOption "NoSection") opts
let dumpCompilerOptionBlocks blocks = List.iter dumpCompilerOptionBlock blocks

let isSlashOpt (opt:string) = 
    opt.[0] = '/' && (opt.Length = 1 || not (opt.[1..].Contains "/"))

//----------------------------------------------------------------------------
// The argument parser is used by both the VS plug-in and the fsc.exe to
// parse the include file path and other front-end arguments.
//
// The language service uses this function too. It's important to continue
// processing flags even if an error is seen in one so that the best possible
// intellisense can be show.
//--------------------------------------------------------------------------
let ParseCompilerOptions (collectOtherArgument : string -> unit) (blocks: CompilerOptionBlock list) args =
  use unwindBuildPhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parameter)
  
  let specs : CompilerOption list = List.collect blockOptions blocks
          
  // returns a tuple - the option token, the option argument string
  let parseOption (s : string) = 
    // grab the option token
    let opts = s.Split([|':'|])
    let mutable opt = opts.[0]
    if opt = "" then
        ()
    // if it doesn't start with a '-' or '/', reject outright
    elif opt.[0] <> '-' && opt.[0] <> '/' then
      opt <- ""
    elif opt <> "--" then
      // is it an abbreviated or MSFT-style option?
      // if so, strip the first character and move on with your life
      if opt.Length = 2 || isSlashOpt opt then
        opt <- opt.[1 ..]
      // else, it should be a non-abbreviated option starting with "--"
      elif opt.Length > 3 && opt.StartsWith("--") then
        opt <- opt.[2 ..]
      else
        opt <- ""

    // get the argument string  
    let optArgs = if opts.Length > 1 then String.Join(":",opts.[1 ..]) else ""
    opt, optArgs
              
  let getOptionArg compilerOption (argString : string) =
    if argString = "" then
      errorR(Error(FSComp.SR.buildOptionRequiresParameter(compilerOptionUsage compilerOption),rangeCmdArgs)) 
    argString
    
  let getOptionArgList compilerOption (argString : string) =
    if argString = "" then
      errorR(Error(FSComp.SR.buildOptionRequiresParameter(compilerOptionUsage compilerOption),rangeCmdArgs)) 
      []
    else
      argString.Split([|',';';'|]) |> List.ofArray
  
  let getSwitchOpt (opt : string) =
    // if opt is a switch, strip the  '+' or '-'
    if opt <> "--" && opt.Length > 1 && (opt.EndsWith("+",StringComparison.Ordinal) || opt.EndsWith("-",StringComparison.Ordinal)) then
      opt.[0 .. opt.Length - 2]
    else
      opt
      
  let getSwitch (s: string) = 
    let s = (s.Split([|':'|])).[0]
    if s <> "--" && s.EndsWith("-",StringComparison.Ordinal) then Off else On

  let rec processArg args =    
    match args with 
    | [] -> ()
    | opt :: t ->  

        let optToken, argString = parseOption opt

        let reportDeprecatedOption errOpt =
          match errOpt with
          | Some(e) -> warning(e)
          | None -> ()

        let rec attempt l = 
          match l with 
          | (CompilerOption(s, _, OptionHelp f, d, _) :: _) when optToken = s  && argString = "" -> 
              reportDeprecatedOption d
              f blocks; t
          | (CompilerOption(s, _, OptionUnit f, d, _) :: _) when optToken = s  && argString = "" -> 
              reportDeprecatedOption d
              f (); t
          | (CompilerOption(s, _, OptionSwitch f, d, _) :: _) when getSwitchOpt(optToken) = s && argString = "" -> 
              reportDeprecatedOption d
              f (getSwitch opt); t
          | (CompilerOption(s, _, OptionSet f, d, _) :: _) when optToken = s && argString = "" -> 
              reportDeprecatedOption d
              f := true; t
          | (CompilerOption(s, _, OptionClear f, d, _) :: _) when optToken = s && argString = "" -> 
              reportDeprecatedOption d
              f := false; t
          | (CompilerOption(s, _, OptionString f, d, _) as compilerOption :: _) when optToken = s -> 
              reportDeprecatedOption d
              let oa = getOptionArg compilerOption argString
              if oa <> "" then
                  f (getOptionArg compilerOption oa)
              t 
          | (CompilerOption(s, _, OptionInt f, d, _) as compilerOption :: _) when optToken = s ->
              reportDeprecatedOption d
              let oa = getOptionArg compilerOption argString
              if oa <> "" then 
                  f (try int32 (oa) with _ -> 
                      errorR(Error(FSComp.SR.buildArgInvalidInt(getOptionArg compilerOption argString),rangeCmdArgs)); 0)
              t
          | (CompilerOption(s, _, OptionFloat f, d, _) as compilerOption :: _) when optToken = s -> 
              reportDeprecatedOption d
              let oa = getOptionArg compilerOption argString
              if oa <> "" then
                  f (try float (oa) with _ -> 
                      errorR(Error(FSComp.SR.buildArgInvalidFloat(getOptionArg compilerOption argString), rangeCmdArgs)); 0.0)
              t
          | (CompilerOption(s, _, OptionRest f, d, _) :: _) when optToken = s -> 
              reportDeprecatedOption d
              List.iter f t; []
          | (CompilerOption(s, _, OptionIntList f, d, _) as compilerOption :: _) when optToken = s ->
              reportDeprecatedOption d
              let al = getOptionArgList compilerOption argString
              if al <> [] then
                  List.iter (fun i -> f (try int32 i with _ -> errorR(Error(FSComp.SR.buildArgInvalidInt(i),rangeCmdArgs)); 0)) al ;
              t
          | (CompilerOption(s, _, OptionIntListSwitch f, d, _) as compilerOption :: _) when getSwitchOpt(optToken) = s -> 
              reportDeprecatedOption d
              let al = getOptionArgList compilerOption argString
              if al <> [] then
                  let switch = getSwitch(opt)
                  List.iter (fun i -> f (try int32 i with _ -> errorR(Error(FSComp.SR.buildArgInvalidInt(i),rangeCmdArgs)); 0) switch) al  
              t
              // here
          | (CompilerOption(s, _, OptionStringList f, d, _) as compilerOption :: _) when optToken = s -> 
              reportDeprecatedOption d
              let al = getOptionArgList compilerOption argString
              if al <> [] then
                  List.iter (fun s -> f s) (getOptionArgList compilerOption argString)
              t
          | (CompilerOption(s, _, OptionStringListSwitch f, d, _) as compilerOption :: _) when getSwitchOpt(optToken) = s -> 
              reportDeprecatedOption d
              let al = getOptionArgList compilerOption argString
              if al <> [] then
                  let switch = getSwitch(opt)
                  List.iter (fun s -> f s switch) (getOptionArgList compilerOption argString)
              t
          | (CompilerOption(_, _, OptionGeneral (pred,exec), d, _) :: _) when pred args -> 
              reportDeprecatedOption d
              let rest = exec args in rest // arguments taken, rest remaining
          | (_ :: more) -> attempt more 
          | [] -> 
              if opt.Length = 0 || opt.[0] = '-' || isSlashOpt opt
               then 
                  // want the whole opt token - delimiter and all
                  let unrecOpt = (opt.Split([|':'|]).[0])
                  errorR(Error(FSComp.SR.buildUnrecognizedOption(unrecOpt),rangeCmdArgs)) 
                  t
              else 
                 (collectOtherArgument opt; t)
        let rest = attempt specs 
        processArg rest
  
  let result = processArg args
  result

do()


