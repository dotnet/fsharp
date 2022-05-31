// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Contains logic to prepare, post-process, filter and emit compiler diagnsotics
module internal FSharp.Compiler.CompilerDiagnostics

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Text

open Internal.Utilities.Library.Extras
open Internal.Utilities.Library
open Internal.Utilities.Text

open FSharp.Compiler
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticMessage
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Infos
open FSharp.Compiler.IO
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.MethodCalls
open FSharp.Compiler.MethodOverrides
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.SignatureConformance
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps

#if DEBUG
[<AutoOpen>]
module internal CompilerService =
    let showAssertForUnexpectedException = ref true
#endif

/// This exception is an old-style way of reporting a diagnostic
exception HashIncludeNotAllowedInNonScript of range

/// This exception is an old-style way of reporting a diagnostic
exception HashReferenceNotAllowedInNonScript of range

/// This exception is an old-style way of reporting a diagnostic
exception HashLoadedSourceHasIssues of informationals: exn list * warnings: exn list * errors: exn list * range

/// This exception is an old-style way of reporting a diagnostic
exception HashLoadedScriptConsideredSource of range

/// This exception is an old-style way of reporting a diagnostic
exception HashDirectiveNotAllowedInNonScript of range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionFull of string * range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionForHtmlDoc of string * range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionSuggestAlternative of string * string * range

/// This exception is an old-style way of reporting a diagnostic
exception DeprecatedCommandLineOptionNoDescription of string * range

/// This exception is an old-style way of reporting a diagnostic
exception InternalCommandLineOption of string * range

let GetRangeOfDiagnostic (diagnostic: PhasedDiagnostic) =
    let rec RangeFromException exn =
        match exn with
        | ErrorFromAddingConstraint (_, exn2, _) -> RangeFromException exn2
#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange exn -> RangeFromException exn
        | TypeProviders.ProvidedTypeResolution (m, _)
#endif
        | ReservedKeyword (_, m)
        | IndentationProblem (_, m)
        | ErrorFromAddingTypeEquation (_, _, _, _, _, m)
        | ErrorFromApplyingDefault (_, _, _, _, _, m)
        | ErrorsFromAddingSubsumptionConstraint (_, _, _, _, _, _, m)
        | FunctionExpected (_, _, m)
        | BakedInMemberConstraintName (_, m)
        | StandardOperatorRedefinitionWarning (_, m)
        | BadEventTransformation m
        | ParameterlessStructCtor m
        | FieldNotMutable (_, _, m)
        | Recursion (_, _, _, _, m)
        | InvalidRuntimeCoercion (_, _, _, m)
        | IndeterminateRuntimeCoercion (_, _, _, m)
        | IndeterminateStaticCoercion (_, _, _, m)
        | StaticCoercionShouldUseBox (_, _, _, m)
        | CoercionTargetSealed (_, _, m)
        | UpcastUnnecessary m
        | QuotationTranslator.IgnoringPartOfQuotedTermWarning (_, m)

        | TypeTestUnnecessary m
        | RuntimeCoercionSourceSealed (_, _, m)
        | OverrideDoesntOverride (_, _, _, _, _, m)
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
        | DeprecatedThreadStaticBindingWarning m
        | NonUniqueInferredAbstractSlot (_, _, _, _, _, m)
        | DefensiveCopyWarning (_, m)
        | LetRecCheckedAtRuntime m
        | UpperCaseIdentifierInPattern m
        | NotUpperCaseConstructor m
        | RecursiveUseCheckedAtRuntime (_, _, m)
        | LetRecEvaluatedOutOfOrder (_, _, _, m)
        | DiagnosticWithText (_, _, m)
        | DiagnosticWithSuggestions (_, _, m, _, _)
        | SyntaxError (_, m)
        | InternalError (_, m)
        | InterfaceNotRevealed (_, _, m)
        | WrappedError (_, m)
        | PatternMatchCompilation.MatchIncomplete (_, _, m)
        | PatternMatchCompilation.EnumMatchIncomplete (_, _, m)
        | PatternMatchCompilation.RuleNeverMatched m
        | ValNotMutable (_, _, m)
        | ValNotLocal (_, _, m)
        | MissingFields (_, m)
        | OverrideInIntrinsicAugmentation m
        | IntfImplInIntrinsicAugmentation m
        | OverrideInExtrinsicAugmentation m
        | IntfImplInExtrinsicAugmentation m
        | ValueRestriction (_, _, _, _, _, m)
        | LetRecUnsound (_, _, m)
        | ObsoleteError (_, m)
        | ObsoleteWarning (_, m)
        | Experimental (_, m)
        | PossibleUnverifiableCode m
        | UserCompilerMessage (_, _, m)
        | Deprecated (_, m)
        | LibraryUseOnly m
        | FieldsFromDifferentTypes (_, _, _, m)
        | IndeterminateType m
        | TyconBadArgs (_, _, _, m) -> Some m

        | FieldNotContained (_, _, _, arf, _, _) -> Some arf.Range
        | ValueNotContained (_, _, _, aval, _, _) -> Some aval.Range
        | UnionCaseNotContained (_, _, _, aval, _, _) -> Some aval.Id.idRange
        | FSharpExceptionNotContained (_, _, aexnc, _, _) -> Some aexnc.Range

        | VarBoundTwice id
        | UndefinedName (_, _, id, _) -> Some id.idRange

        | Duplicate (_, _, m)
        | NameClash (_, _, _, m, _, _, _)
        | UnresolvedOverloading (_, _, _, m)
        | UnresolvedConversionOperator (_, _, _, m)
        | VirtualAugmentationOnNullValuedType m
        | NonVirtualAugmentationOnNullValuedType m
        | NonRigidTypar (_, _, _, _, _, m)
        | ConstraintSolverTupleDiffLengths (_, _, _, m, _)
        | ConstraintSolverInfiniteTypes (_, _, _, _, m, _)
        | ConstraintSolverMissingConstraint (_, _, _, m, _)
        | ConstraintSolverTypesNotInEqualityRelation (_, _, _, m, _, _)
        | ConstraintSolverError (_, m, _)
        | ConstraintSolverTypesNotInSubsumptionRelation (_, _, _, m, _)
        | SelfRefObjCtor (_, m) -> Some m

        | NotAFunction (_, _, mfun, _) -> Some mfun

        | NotAFunctionButIndexer (_, _, _, mfun, _, _) -> Some mfun

        | IllegalFileNameChar _ -> Some rangeCmdArgs

        | UnresolvedReferenceError (_, m)
        | UnresolvedPathReference (_, _, m)
        | DeprecatedCommandLineOptionFull (_, m)
        | DeprecatedCommandLineOptionForHtmlDoc (_, m)
        | DeprecatedCommandLineOptionSuggestAlternative (_, _, m)
        | DeprecatedCommandLineOptionNoDescription (_, m)
        | InternalCommandLineOption (_, m)
        | HashIncludeNotAllowedInNonScript m
        | HashReferenceNotAllowedInNonScript m
        | HashDirectiveNotAllowedInNonScript m
        | FileNameNotResolved (_, _, m)
        | LoadedSourceNotFoundIgnoring (_, m)
        | MSBuildReferenceResolutionWarning (_, _, m)
        | MSBuildReferenceResolutionError (_, _, m)
        | AssemblyNotResolved (_, m)
        | HashLoadedSourceHasIssues (_, _, _, m)
        | HashLoadedScriptConsideredSource m -> Some m
        // Strip TargetInvocationException wrappers
        | :? System.Reflection.TargetInvocationException as e -> RangeFromException e.InnerException
#if !NO_TYPEPROVIDERS
        | :? TypeProviderError as e -> e.Range |> Some
#endif

        | _ -> None

    RangeFromException diagnostic.Exception

let GetDiagnosticNumber (diagnostic: PhasedDiagnostic) =
    let rec GetFromException (exn: exn) =
        match exn with
        // DO NOT CHANGE THESE NUMBERS
        | ErrorFromAddingTypeEquation _ -> 1
        | FunctionExpected _ -> 2
        | NotAFunctionButIndexer _ -> 3217
        | NotAFunction _ -> 3
        | FieldNotMutable _ -> 5
        | Recursion _ -> 6
        | InvalidRuntimeCoercion _ -> 7
        | IndeterminateRuntimeCoercion _ -> 8
        | PossibleUnverifiableCode _ -> 9
        | SyntaxError _ -> 10
        // 11 cannot be reused
        // 12 cannot be reused
        | IndeterminateStaticCoercion _ -> 13
        | StaticCoercionShouldUseBox _ -> 14
        // 15 cannot be reused
        | RuntimeCoercionSourceSealed _ -> 16
        | OverrideDoesntOverride _ -> 17
        | UnionPatternsBindDifferentNames _ -> 18
        | UnionCaseWrongArguments _ -> 19
        | UnitTypeExpected _ -> 20
        | UnitTypeExpectedWithEquality _ -> 20
        | UnitTypeExpectedWithPossiblePropertySetter _ -> 20
        | UnitTypeExpectedWithPossibleAssignment _ -> 20
        | RecursiveUseCheckedAtRuntime _ -> 21
        | LetRecEvaluatedOutOfOrder _ -> 22
        | NameClash _ -> 23
        // 24 cannot be reused
        | PatternMatchCompilation.MatchIncomplete _ -> 25
        | PatternMatchCompilation.RuleNeverMatched _ -> 26
        | ValNotMutable _ -> 27
        | ValNotLocal _ -> 28
        | MissingFields _ -> 29
        | ValueRestriction _ -> 30
        | LetRecUnsound _ -> 31
        | FieldsFromDifferentTypes _ -> 32
        | TyconBadArgs _ -> 33
        | ValueNotContained _ -> 34
        | Deprecated _ -> 35
        | UnionCaseNotContained _ -> 36
        | Duplicate _ -> 37
        | VarBoundTwice _ -> 38
        | UndefinedName _ -> 39
        | LetRecCheckedAtRuntime _ -> 40
        | UnresolvedOverloading _ -> 41
        | LibraryUseOnly _ -> 42
        | ErrorFromAddingConstraint _ -> 43
        | ObsoleteWarning _ -> 44
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
        | FSharpExceptionNotContained _ -> 63
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
#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange _
        | TypeProviders.ProvidedTypeResolution _ -> 103
#endif
        | PatternMatchCompilation.EnumMatchIncomplete _ -> 104
        // DO NOT CHANGE THE NUMBERS

        // Strip TargetInvocationException wrappers
        | :? System.Reflection.TargetInvocationException as e -> GetFromException e.InnerException

        | WrappedError (e, _) -> GetFromException e

        | DiagnosticWithText (n, _, _) -> n
        | DiagnosticWithSuggestions (n, _, _, _, _) -> n
        | Failure _ -> 192
        | IllegalFileNameChar (fileName, invalidChar) -> fst (FSComp.SR.buildUnexpectedFileNameCharacter (fileName, string invalidChar))
#if !NO_TYPEPROVIDERS
        | :? TypeProviderError as e -> e.Number
#endif
        | ErrorsFromAddingSubsumptionConstraint (_, _, _, _, _, ContextInfo.DowncastUsedInsteadOfUpcast _, _) ->
            fst (FSComp.SR.considerUpcast ("", ""))
        | _ -> 193

    GetFromException diagnostic.Exception

let GetWarningLevel diagnostic =
    match diagnostic.Exception with
    // Level 5 warnings
    | RecursiveUseCheckedAtRuntime _
    | LetRecEvaluatedOutOfOrder _
    | DefensiveCopyWarning _ -> 5

    | DiagnosticWithText (n, _, _)
    | DiagnosticWithSuggestions (n, _, _, _, _) ->
        // 1178, tcNoComparisonNeeded1, "The struct, record or union type '%s' is not structurally comparable because the type parameter %s does not satisfy the 'comparison' constraint..."
        // 1178, tcNoComparisonNeeded2, "The struct, record or union type '%s' is not structurally comparable because the type '%s' does not satisfy the 'comparison' constraint...."
        // 1178, tcNoEqualityNeeded1, "The struct, record or union type '%s' does not support structural equality because the type parameter %s does not satisfy the 'equality' constraint..."
        // 1178, tcNoEqualityNeeded2, "The struct, record or union type '%s' does not support structural equality because the type '%s' does not satisfy the 'equality' constraint...."
        if (n = 1178) then 5 else 2
    // Level 2
    | _ -> 2

let IsWarningOrInfoEnabled (diagnostic, severity) n level specificWarnOn =
    List.contains n specificWarnOn
    ||
    // Some specific warnings/informational are never on by default, i.e. unused variable warnings
    match n with
    | 1182 -> false // chkUnusedValue - off by default
    | 3180 -> false // abImplicitHeapAllocation - off by default
    | 3186 -> false // pickleMissingDefinition - off by default
    | 3366 -> false //tcIndexNotationDeprecated - currently off by default
    | 3517 -> false // optFailedToInlineSuggestedValue - off by default
    | 3388 -> false // tcSubsumptionImplicitConversionUsed - off by default
    | 3389 -> false // tcBuiltInImplicitConversionUsed - off by default
    | 3390 -> false // xmlDocBadlyFormed - off by default
    | 3395 -> false // tcImplicitConversionUsedForMethodArg - off by default
    | _ ->
        (severity = FSharpDiagnosticSeverity.Info)
        || (severity = FSharpDiagnosticSeverity.Warning
            && level >= GetWarningLevel diagnostic)

let SplitRelatedDiagnostics (diagnostic: PhasedDiagnostic) : PhasedDiagnostic * PhasedDiagnostic list =
    let ToPhased exn =
        {
            Exception = exn
            Phase = diagnostic.Phase
        }

    let rec SplitRelatedException exn =
        match exn with
        | ErrorFromAddingTypeEquation (g, denv, ty1, ty2, exn2, m) ->
            let diag2, related = SplitRelatedException exn2
            ErrorFromAddingTypeEquation(g, denv, ty1, ty2, diag2.Exception, m) |> ToPhased, related
        | ErrorFromApplyingDefault (g, denv, tp, defaultType, exn2, m) ->
            let diag2, related = SplitRelatedException exn2

            ErrorFromApplyingDefault(g, denv, tp, defaultType, diag2.Exception, m)
            |> ToPhased,
            related
        | ErrorsFromAddingSubsumptionConstraint (g, denv, ty1, ty2, exn2, contextInfo, m) ->
            let diag2, related = SplitRelatedException exn2

            ErrorsFromAddingSubsumptionConstraint(g, denv, ty1, ty2, diag2.Exception, contextInfo, m)
            |> ToPhased,
            related
        | ErrorFromAddingConstraint (x, exn2, m) ->
            let diag2, related = SplitRelatedException exn2
            ErrorFromAddingConstraint(x, diag2.Exception, m) |> ToPhased, related
        | WrappedError (exn2, m) ->
            let diag2, related = SplitRelatedException exn2
            WrappedError(diag2.Exception, m) |> ToPhased, related
        // Strip TargetInvocationException wrappers
        | :? TargetInvocationException as exn -> SplitRelatedException exn.InnerException
        | _ -> ToPhased exn, []

    SplitRelatedException diagnostic.Exception

let Message (name, format) = DeclareResourceString(name, format)

do FSComp.SR.RunStartupValidation()
let SeeAlsoE () = Message("SeeAlso", "%s")
let ConstraintSolverTupleDiffLengthsE () = Message("ConstraintSolverTupleDiffLengths", "%d%d")
let ConstraintSolverInfiniteTypesE () = Message("ConstraintSolverInfiniteTypes", "%s%s")
let ConstraintSolverMissingConstraintE () = Message("ConstraintSolverMissingConstraint", "%s")
let ConstraintSolverTypesNotInEqualityRelation1E () = Message("ConstraintSolverTypesNotInEqualityRelation1", "%s%s")
let ConstraintSolverTypesNotInEqualityRelation2E () = Message("ConstraintSolverTypesNotInEqualityRelation2", "%s%s")
let ConstraintSolverTypesNotInSubsumptionRelationE () = Message("ConstraintSolverTypesNotInSubsumptionRelation", "%s%s%s")
let ErrorFromAddingTypeEquation1E () = Message("ErrorFromAddingTypeEquation1", "%s%s%s")
let ErrorFromAddingTypeEquation2E () = Message("ErrorFromAddingTypeEquation2", "%s%s%s")
let ErrorFromApplyingDefault1E () = Message("ErrorFromApplyingDefault1", "%s")
let ErrorFromApplyingDefault2E () = Message("ErrorFromApplyingDefault2", "")
let ErrorsFromAddingSubsumptionConstraintE () = Message("ErrorsFromAddingSubsumptionConstraint", "%s%s%s")
let UpperCaseIdentifierInPatternE () = Message("UpperCaseIdentifierInPattern", "")
let NotUpperCaseConstructorE () = Message("NotUpperCaseConstructor", "")
let FunctionExpectedE () = Message("FunctionExpected", "")
let BakedInMemberConstraintNameE () = Message("BakedInMemberConstraintName", "%s")
let BadEventTransformationE () = Message("BadEventTransformation", "")
let ParameterlessStructCtorE () = Message("ParameterlessStructCtor", "")
let InterfaceNotRevealedE () = Message("InterfaceNotRevealed", "%s")
let TyconBadArgsE () = Message("TyconBadArgs", "%s%d%d")
let IndeterminateTypeE () = Message("IndeterminateType", "")
let NameClash1E () = Message("NameClash1", "%s%s")
let NameClash2E () = Message("NameClash2", "%s%s%s%s%s")
let Duplicate1E () = Message("Duplicate1", "%s")
let Duplicate2E () = Message("Duplicate2", "%s%s")
let UndefinedName2E () = Message("UndefinedName2", "")
let FieldNotMutableE () = Message("FieldNotMutable", "")
let FieldsFromDifferentTypesE () = Message("FieldsFromDifferentTypes", "%s%s")
let VarBoundTwiceE () = Message("VarBoundTwice", "%s")
let RecursionE () = Message("Recursion", "%s%s%s%s")
let InvalidRuntimeCoercionE () = Message("InvalidRuntimeCoercion", "%s%s%s")
let IndeterminateRuntimeCoercionE () = Message("IndeterminateRuntimeCoercion", "%s%s")
let IndeterminateStaticCoercionE () = Message("IndeterminateStaticCoercion", "%s%s")
let StaticCoercionShouldUseBoxE () = Message("StaticCoercionShouldUseBox", "%s%s")
let TypeIsImplicitlyAbstractE () = Message("TypeIsImplicitlyAbstract", "")
let NonRigidTypar1E () = Message("NonRigidTypar1", "%s%s")
let NonRigidTypar2E () = Message("NonRigidTypar2", "%s%s")
let NonRigidTypar3E () = Message("NonRigidTypar3", "%s%s")
let OBlockEndSentenceE () = Message("BlockEndSentence", "")
let UnexpectedEndOfInputE () = Message("UnexpectedEndOfInput", "")
let UnexpectedE () = Message("Unexpected", "%s")
let NONTERM_interactionE () = Message("NONTERM.interaction", "")
let NONTERM_hashDirectiveE () = Message("NONTERM.hashDirective", "")
let NONTERM_fieldDeclE () = Message("NONTERM.fieldDecl", "")
let NONTERM_unionCaseReprE () = Message("NONTERM.unionCaseRepr", "")
let NONTERM_localBindingE () = Message("NONTERM.localBinding", "")
let NONTERM_hardwhiteLetBindingsE () = Message("NONTERM.hardwhiteLetBindings", "")
let NONTERM_classDefnMemberE () = Message("NONTERM.classDefnMember", "")
let NONTERM_defnBindingsE () = Message("NONTERM.defnBindings", "")
let NONTERM_classMemberSpfnE () = Message("NONTERM.classMemberSpfn", "")
let NONTERM_valSpfnE () = Message("NONTERM.valSpfn", "")
let NONTERM_tyconSpfnE () = Message("NONTERM.tyconSpfn", "")
let NONTERM_anonLambdaExprE () = Message("NONTERM.anonLambdaExpr", "")
let NONTERM_attrUnionCaseDeclE () = Message("NONTERM.attrUnionCaseDecl", "")
let NONTERM_cPrototypeE () = Message("NONTERM.cPrototype", "")
let NONTERM_objectImplementationMembersE () = Message("NONTERM.objectImplementationMembers", "")
let NONTERM_ifExprCasesE () = Message("NONTERM.ifExprCases", "")
let NONTERM_openDeclE () = Message("NONTERM.openDecl", "")
let NONTERM_fileModuleSpecE () = Message("NONTERM.fileModuleSpec", "")
let NONTERM_patternClausesE () = Message("NONTERM.patternClauses", "")
let NONTERM_beginEndExprE () = Message("NONTERM.beginEndExpr", "")
let NONTERM_recdExprE () = Message("NONTERM.recdExpr", "")
let NONTERM_tyconDefnE () = Message("NONTERM.tyconDefn", "")
let NONTERM_exconCoreE () = Message("NONTERM.exconCore", "")
let NONTERM_typeNameInfoE () = Message("NONTERM.typeNameInfo", "")
let NONTERM_attributeListE () = Message("NONTERM.attributeList", "")
let NONTERM_quoteExprE () = Message("NONTERM.quoteExpr", "")
let NONTERM_typeConstraintE () = Message("NONTERM.typeConstraint", "")
let NONTERM_Category_ImplementationFileE () = Message("NONTERM.Category.ImplementationFile", "")
let NONTERM_Category_DefinitionE () = Message("NONTERM.Category.Definition", "")
let NONTERM_Category_SignatureFileE () = Message("NONTERM.Category.SignatureFile", "")
let NONTERM_Category_PatternE () = Message("NONTERM.Category.Pattern", "")
let NONTERM_Category_ExprE () = Message("NONTERM.Category.Expr", "")
let NONTERM_Category_TypeE () = Message("NONTERM.Category.Type", "")
let NONTERM_typeArgsActualE () = Message("NONTERM.typeArgsActual", "")
let TokenName1E () = Message("TokenName1", "%s")
let TokenName1TokenName2E () = Message("TokenName1TokenName2", "%s%s")
let TokenName1TokenName2TokenName3E () = Message("TokenName1TokenName2TokenName3", "%s%s%s")
let RuntimeCoercionSourceSealed1E () = Message("RuntimeCoercionSourceSealed1", "%s")
let RuntimeCoercionSourceSealed2E () = Message("RuntimeCoercionSourceSealed2", "%s")
let CoercionTargetSealedE () = Message("CoercionTargetSealed", "%s")
let UpcastUnnecessaryE () = Message("UpcastUnnecessary", "")
let TypeTestUnnecessaryE () = Message("TypeTestUnnecessary", "")
let OverrideDoesntOverride1E () = Message("OverrideDoesntOverride1", "%s")
let OverrideDoesntOverride2E () = Message("OverrideDoesntOverride2", "%s")
let OverrideDoesntOverride3E () = Message("OverrideDoesntOverride3", "%s")
let OverrideDoesntOverride4E () = Message("OverrideDoesntOverride4", "%s")
let UnionCaseWrongArgumentsE () = Message("UnionCaseWrongArguments", "%d%d")
let UnionPatternsBindDifferentNamesE () = Message("UnionPatternsBindDifferentNames", "")
let RequiredButNotSpecifiedE () = Message("RequiredButNotSpecified", "%s%s%s")
let UseOfAddressOfOperatorE () = Message("UseOfAddressOfOperator", "")
let DefensiveCopyWarningE () = Message("DefensiveCopyWarning", "%s")
let DeprecatedThreadStaticBindingWarningE () = Message("DeprecatedThreadStaticBindingWarning", "")
let FunctionValueUnexpectedE () = Message("FunctionValueUnexpected", "%s")
let UnitTypeExpectedE () = Message("UnitTypeExpected", "%s")
let UnitTypeExpectedWithEqualityE () = Message("UnitTypeExpectedWithEquality", "%s")
let UnitTypeExpectedWithPossiblePropertySetterE () = Message("UnitTypeExpectedWithPossiblePropertySetter", "%s%s%s")
let UnitTypeExpectedWithPossibleAssignmentE () = Message("UnitTypeExpectedWithPossibleAssignment", "%s%s")
let UnitTypeExpectedWithPossibleAssignmentToMutableE () = Message("UnitTypeExpectedWithPossibleAssignmentToMutable", "%s%s")
let RecursiveUseCheckedAtRuntimeE () = Message("RecursiveUseCheckedAtRuntime", "")
let LetRecUnsound1E () = Message("LetRecUnsound1", "%s")
let LetRecUnsound2E () = Message("LetRecUnsound2", "%s%s")
let LetRecUnsoundInnerE () = Message("LetRecUnsoundInner", "%s")
let LetRecEvaluatedOutOfOrderE () = Message("LetRecEvaluatedOutOfOrder", "")
let LetRecCheckedAtRuntimeE () = Message("LetRecCheckedAtRuntime", "")
let SelfRefObjCtor1E () = Message("SelfRefObjCtor1", "")
let SelfRefObjCtor2E () = Message("SelfRefObjCtor2", "")
let VirtualAugmentationOnNullValuedTypeE () = Message("VirtualAugmentationOnNullValuedType", "")
let NonVirtualAugmentationOnNullValuedTypeE () = Message("NonVirtualAugmentationOnNullValuedType", "")
let NonUniqueInferredAbstractSlot1E () = Message("NonUniqueInferredAbstractSlot1", "%s")
let NonUniqueInferredAbstractSlot2E () = Message("NonUniqueInferredAbstractSlot2", "")
let NonUniqueInferredAbstractSlot3E () = Message("NonUniqueInferredAbstractSlot3", "%s%s")
let NonUniqueInferredAbstractSlot4E () = Message("NonUniqueInferredAbstractSlot4", "")
let Failure3E () = Message("Failure3", "%s")
let Failure4E () = Message("Failure4", "%s")
let MatchIncomplete1E () = Message("MatchIncomplete1", "")
let MatchIncomplete2E () = Message("MatchIncomplete2", "%s")
let MatchIncomplete3E () = Message("MatchIncomplete3", "%s")
let MatchIncomplete4E () = Message("MatchIncomplete4", "")
let RuleNeverMatchedE () = Message("RuleNeverMatched", "")
let EnumMatchIncomplete1E () = Message("EnumMatchIncomplete1", "")
let ValNotMutableE () = Message("ValNotMutable", "%s")
let ValNotLocalE () = Message("ValNotLocal", "")
let Obsolete1E () = Message("Obsolete1", "")
let Obsolete2E () = Message("Obsolete2", "%s")
let ExperimentalE () = Message("Experimental", "%s")
let PossibleUnverifiableCodeE () = Message("PossibleUnverifiableCode", "")
let DeprecatedE () = Message("Deprecated", "%s")
let LibraryUseOnlyE () = Message("LibraryUseOnly", "")
let MissingFieldsE () = Message("MissingFields", "%s")
let ValueRestriction1E () = Message("ValueRestriction1", "%s%s%s")
let ValueRestriction2E () = Message("ValueRestriction2", "%s%s%s")
let ValueRestriction3E () = Message("ValueRestriction3", "%s")
let ValueRestriction4E () = Message("ValueRestriction4", "%s%s%s")
let ValueRestriction5E () = Message("ValueRestriction5", "%s%s%s")
let RecoverableParseErrorE () = Message("RecoverableParseError", "")
let ReservedKeywordE () = Message("ReservedKeyword", "%s")
let IndentationProblemE () = Message("IndentationProblem", "%s")
let OverrideInIntrinsicAugmentationE () = Message("OverrideInIntrinsicAugmentation", "")
let OverrideInExtrinsicAugmentationE () = Message("OverrideInExtrinsicAugmentation", "")
let IntfImplInIntrinsicAugmentationE () = Message("IntfImplInIntrinsicAugmentation", "")
let IntfImplInExtrinsicAugmentationE () = Message("IntfImplInExtrinsicAugmentation", "")
let UnresolvedReferenceNoRangeE () = Message("UnresolvedReferenceNoRange", "%s")
let UnresolvedPathReferenceNoRangeE () = Message("UnresolvedPathReferenceNoRange", "%s%s")
let HashIncludeNotAllowedInNonScriptE () = Message("HashIncludeNotAllowedInNonScript", "")
let HashReferenceNotAllowedInNonScriptE () = Message("HashReferenceNotAllowedInNonScript", "")
let HashDirectiveNotAllowedInNonScriptE () = Message("HashDirectiveNotAllowedInNonScript", "")
let FileNameNotResolvedE () = Message("FileNameNotResolved", "%s%s")
let AssemblyNotResolvedE () = Message("AssemblyNotResolved", "%s")
let HashLoadedSourceHasIssues0E () = Message("HashLoadedSourceHasIssues0", "")
let HashLoadedSourceHasIssues1E () = Message("HashLoadedSourceHasIssues1", "")
let HashLoadedSourceHasIssues2E () = Message("HashLoadedSourceHasIssues2", "")
let HashLoadedScriptConsideredSourceE () = Message("HashLoadedScriptConsideredSource", "")
let InvalidInternalsVisibleToAssemblyName1E () = Message("InvalidInternalsVisibleToAssemblyName1", "%s%s")
let InvalidInternalsVisibleToAssemblyName2E () = Message("InvalidInternalsVisibleToAssemblyName2", "%s")
let LoadedSourceNotFoundIgnoringE () = Message("LoadedSourceNotFoundIgnoring", "%s")
let MSBuildReferenceResolutionErrorE () = Message("MSBuildReferenceResolutionError", "%s%s")
let TargetInvocationExceptionWrapperE () = Message("TargetInvocationExceptionWrapper", "%s")

#if DEBUG
let mutable showParserStackOnParseError = false
#endif

let getErrorString key = SR.GetString key

let (|InvalidArgument|_|) (exn: exn) =
    match exn with
    | :? ArgumentException as e -> Some e.Message
    | _ -> None

let OutputPhasedErrorR (os: StringBuilder) (diagnostic: PhasedDiagnostic) (canSuggestNames: bool) =

    let suggestNames suggestionsF idText =
        if canSuggestNames then
            let buffer = DiagnosticResolutionHints.SuggestionBuffer idText

            if not buffer.Disabled then
                suggestionsF buffer.Add

                if not buffer.IsEmpty then
                    os.AppendString " "
                    os.AppendString(FSComp.SR.undefinedNameSuggestionsIntro ())

                    for value in buffer do
                        os.AppendLine() |> ignore
                        os.AppendString "   "
                        os.AppendString(DecompileOpName value)

    let rec OutputExceptionR (os: StringBuilder) error =

        match error with
        | ConstraintSolverTupleDiffLengths (_, tl1, tl2, m, m2) ->
            os.AppendString(ConstraintSolverTupleDiffLengthsE().Format tl1.Length tl2.Length)

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverInfiniteTypes (denv, contextInfo, ty1, ty2, m, m2) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(ConstraintSolverInfiniteTypesE().Format ty1 ty2)

            match contextInfo with
            | ContextInfo.ReturnInComputationExpression -> os.AppendString(" " + FSComp.SR.returnUsedInsteadOfReturnBang ())
            | ContextInfo.YieldInComputationExpression -> os.AppendString(" " + FSComp.SR.yieldUsedInsteadOfYieldBang ())
            | _ -> ()

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverMissingConstraint (denv, tpr, tpc, m, m2) ->
            os.AppendString(
                ConstraintSolverMissingConstraintE()
                    .Format(NicePrint.stringOfTyparConstraint denv (tpr, tpc))
            )

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverTypesNotInEqualityRelation (denv, (TType_measure _ as ty1), (TType_measure _ as ty2), m, m2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

            os.AppendString(ConstraintSolverTypesNotInEqualityRelation1E().Format ty1 ty2)

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverTypesNotInEqualityRelation (denv, ty1, ty2, m, m2, contextInfo) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

            match contextInfo with
            | ContextInfo.IfExpression range when equals range m -> os.AppendString(FSComp.SR.ifExpression (ty1, ty2))
            | ContextInfo.CollectionElement (isArray, range) when equals range m ->
                if isArray then
                    os.AppendString(FSComp.SR.arrayElementHasWrongType (ty1, ty2))
                else
                    os.AppendString(FSComp.SR.listElementHasWrongType (ty1, ty2))
            | ContextInfo.OmittedElseBranch range when equals range m -> os.AppendString(FSComp.SR.missingElseBranch (ty2))
            | ContextInfo.ElseBranchResult range when equals range m -> os.AppendString(FSComp.SR.elseBranchHasWrongType (ty1, ty2))
            | ContextInfo.FollowingPatternMatchClause range when equals range m ->
                os.AppendString(FSComp.SR.followingPatternMatchClauseHasWrongType (ty1, ty2))
            | ContextInfo.PatternMatchGuard range when equals range m -> os.AppendString(FSComp.SR.patternMatchGuardIsNotBool (ty2))
            | _ -> os.AppendString(ConstraintSolverTypesNotInEqualityRelation2E().Format ty1 ty2)

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverTypesNotInSubsumptionRelation (denv, ty1, ty2, m, m2) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(ConstraintSolverTypesNotInSubsumptionRelationE().Format ty2 ty1 cxs)

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m2))

        | ConstraintSolverError (msg, m, m2) ->
            os.AppendString msg

            if m.StartLine <> m2.StartLine then
                os.AppendString(SeeAlsoE().Format(stringOfRange m2))

        | ErrorFromAddingTypeEquation (g, denv, ty1, ty2, ConstraintSolverTypesNotInEqualityRelation (_, ty1b, ty2b, m, _, contextInfo), _) when
            typeEquiv g ty1 ty1b && typeEquiv g ty2 ty2b
            ->
            let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

            match contextInfo with
            | ContextInfo.IfExpression range when equals range m -> os.AppendString(FSComp.SR.ifExpression (ty1, ty2))

            | ContextInfo.CollectionElement (isArray, range) when equals range m ->
                if isArray then
                    os.AppendString(FSComp.SR.arrayElementHasWrongType (ty1, ty2))
                else
                    os.AppendString(FSComp.SR.listElementHasWrongType (ty1, ty2))

            | ContextInfo.OmittedElseBranch range when equals range m -> os.AppendString(FSComp.SR.missingElseBranch (ty2))

            | ContextInfo.ElseBranchResult range when equals range m -> os.AppendString(FSComp.SR.elseBranchHasWrongType (ty1, ty2))

            | ContextInfo.FollowingPatternMatchClause range when equals range m ->
                os.AppendString(FSComp.SR.followingPatternMatchClauseHasWrongType (ty1, ty2))

            | ContextInfo.PatternMatchGuard range when equals range m -> os.AppendString(FSComp.SR.patternMatchGuardIsNotBool (ty2))

            | ContextInfo.TupleInRecordFields ->
                os.AppendString(ErrorFromAddingTypeEquation1E().Format ty2 ty1 tpcs)
                os.AppendString(Environment.NewLine + FSComp.SR.commaInsteadOfSemicolonInRecord ())

            | _ when ty2 = "bool" && ty1.EndsWithOrdinal(" ref") ->
                os.AppendString(ErrorFromAddingTypeEquation1E().Format ty2 ty1 tpcs)
                os.AppendString(Environment.NewLine + FSComp.SR.derefInsteadOfNot ())

            | _ -> os.AppendString(ErrorFromAddingTypeEquation1E().Format ty2 ty1 tpcs)

        | ErrorFromAddingTypeEquation (_, _, _, _, (ConstraintSolverTypesNotInEqualityRelation (_, _, _, _, _, contextInfo) as e), _) when
            (match contextInfo with
             | ContextInfo.NoContext -> false
             | _ -> true)
            ->
            OutputExceptionR os e

        | ErrorFromAddingTypeEquation (_,
                                       _,
                                       _,
                                       _,
                                       (ConstraintSolverTypesNotInSubsumptionRelation _
                                       | ConstraintSolverError _ as e),
                                       _) -> OutputExceptionR os e

        | ErrorFromAddingTypeEquation (g, denv, ty1, ty2, e, _) ->
            if not (typeEquiv g ty1 ty2) then
                let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

                if ty1 <> ty2 + tpcs then
                    os.AppendString(ErrorFromAddingTypeEquation2E().Format ty1 ty2 tpcs)

            OutputExceptionR os e

        | ErrorFromApplyingDefault (_, denv, _, defaultType, e, _) ->
            let defaultType = NicePrint.minimalStringOfType denv defaultType
            os.AppendString(ErrorFromApplyingDefault1E().Format defaultType)
            OutputExceptionR os e
            os.AppendString(ErrorFromApplyingDefault2E().Format)

        | ErrorsFromAddingSubsumptionConstraint (g, denv, ty1, ty2, e, contextInfo, _) ->
            match contextInfo with
            | ContextInfo.DowncastUsedInsteadOfUpcast isOperator ->
                let ty1, ty2, _ = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

                if isOperator then
                    os.AppendString(FSComp.SR.considerUpcastOperator (ty1, ty2) |> snd)
                else
                    os.AppendString(FSComp.SR.considerUpcast (ty1, ty2) |> snd)
            | _ ->
                if not (typeEquiv g ty1 ty2) then
                    let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

                    if ty1 <> (ty2 + tpcs) then
                        os.AppendString(ErrorsFromAddingSubsumptionConstraintE().Format ty2 ty1 tpcs)
                    else
                        OutputExceptionR os e
                else
                    OutputExceptionR os e

        | UpperCaseIdentifierInPattern _ -> os.AppendString(UpperCaseIdentifierInPatternE().Format)

        | NotUpperCaseConstructor _ -> os.AppendString(NotUpperCaseConstructorE().Format)

        | ErrorFromAddingConstraint (_, e, _) -> OutputExceptionR os e

#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange e

        | TypeProviders.ProvidedTypeResolution (_, e) -> OutputExceptionR os e

        | :? TypeProviderError as e -> os.AppendString(e.ContextualErrorMessage)
#endif

        | UnresolvedOverloading (denv, callerArgs, failure, m) ->

            // extract eventual information (return type and type parameters)
            // from ConstraintTraitInfo
            let knownReturnType, genericParameterTypes =
                match failure with
                | NoOverloadsFound(cx = Some cx)
                | PossibleCandidates(cx = Some cx) -> cx.ReturnType, cx.ArgumentTypes
                | _ -> None, []

            // prepare message parts (known arguments, known return type, known generic parameters)
            let argsMessage, returnType, genericParametersMessage =

                let retTy =
                    knownReturnType |> Option.defaultValue (TType_var(Typar.NewUnlinked(), 0uy))

                let argRepr =
                    callerArgs.ArgumentNamesAndTypes
                    |> List.map (fun (name, tTy) ->
                        tTy,
                        {
                            ArgReprInfo.Name = name |> Option.map (fun name -> Ident(name, range.Zero))
                            ArgReprInfo.Attribs = []
                        })

                let argsL, retTyL, genParamTysL =
                    NicePrint.prettyLayoutsOfUnresolvedOverloading denv argRepr retTy genericParameterTypes

                match callerArgs.ArgumentNamesAndTypes with
                | [] -> None, LayoutRender.showL retTyL, LayoutRender.showL genParamTysL
                | items ->
                    let args = LayoutRender.showL argsL

                    let prefixMessage =
                        match items with
                        | [ _ ] -> FSComp.SR.csNoOverloadsFoundArgumentsPrefixSingular
                        | _ -> FSComp.SR.csNoOverloadsFoundArgumentsPrefixPlural

                    Some(prefixMessage args), LayoutRender.showL retTyL, LayoutRender.showL genParamTysL

            let knownReturnType =
                match knownReturnType with
                | None -> None
                | Some _ -> Some(FSComp.SR.csNoOverloadsFoundReturnType returnType)

            let genericParametersMessage =
                match genericParameterTypes with
                | [] -> None
                | [ _ ] -> Some(FSComp.SR.csNoOverloadsFoundTypeParametersPrefixSingular genericParametersMessage)
                | _ -> Some(FSComp.SR.csNoOverloadsFoundTypeParametersPrefixPlural genericParametersMessage)

            let overloadMethodInfo displayEnv m (x: OverloadInformation) =
                let paramInfo =
                    match x.error with
                    | :? ArgDoesNotMatchError as x ->
                        let nameOrOneBasedIndexMessage =
                            x.calledArg.NameOpt
                            |> Option.map (fun n -> FSComp.SR.csOverloadCandidateNamedArgumentTypeMismatch n.idText)
                            |> Option.defaultValue (
                                FSComp.SR.csOverloadCandidateIndexedArgumentTypeMismatch ((vsnd x.calledArg.Position) + 1)
                            ) //snd

                        sprintf " // %s" nameOrOneBasedIndexMessage
                    | _ -> ""

                (NicePrint.stringOfMethInfo x.infoReader m displayEnv x.methodSlot.Method)
                + paramInfo

            let nl = Environment.NewLine

            let formatOverloads (overloads: OverloadInformation list) =
                overloads
                |> List.map (overloadMethodInfo denv m)
                |> List.sort
                |> List.map FSComp.SR.formatDashItem
                |> String.concat nl

            // assemble final message composing the parts
            let msg =
                let optionalParts =
                    [ knownReturnType; genericParametersMessage; argsMessage ]
                    |> List.choose id
                    |> String.concat (nl + nl)
                    |> function
                        | "" -> nl
                        | result -> nl + nl + result + nl + nl

                match failure with
                | NoOverloadsFound (methodName, overloads, _) ->
                    FSComp.SR.csNoOverloadsFound methodName
                    + optionalParts
                    + (FSComp.SR.csAvailableOverloads (formatOverloads overloads))
                | PossibleCandidates (methodName, [], _) -> FSComp.SR.csMethodIsOverloaded methodName
                | PossibleCandidates (methodName, overloads, _) ->
                    FSComp.SR.csMethodIsOverloaded methodName
                    + optionalParts
                    + FSComp.SR.csCandidates (formatOverloads overloads)

            os.AppendString msg

        | UnresolvedConversionOperator (denv, fromTy, toTy, _) ->
            let ty1, ty2, _tpcs = NicePrint.minimalStringsOfTwoTypes denv fromTy toTy
            os.AppendString(FSComp.SR.csTypeDoesNotSupportConversion (ty1, ty2))

        | FunctionExpected _ -> os.AppendString(FunctionExpectedE().Format)

        | BakedInMemberConstraintName (nm, _) -> os.AppendString(BakedInMemberConstraintNameE().Format nm)

        | StandardOperatorRedefinitionWarning (msg, _) -> os.AppendString msg

        | BadEventTransformation _ -> os.AppendString(BadEventTransformationE().Format)

        | ParameterlessStructCtor _ -> os.AppendString(ParameterlessStructCtorE().Format)

        | InterfaceNotRevealed (denv, ity, _) -> os.AppendString(InterfaceNotRevealedE().Format(NicePrint.minimalStringOfType denv ity))

        | NotAFunctionButIndexer (_, _, name, _, _, old) ->
            if old then
                match name with
                | Some name -> os.AppendString(FSComp.SR.notAFunctionButMaybeIndexerWithName name)
                | _ -> os.AppendString(FSComp.SR.notAFunctionButMaybeIndexer ())
            else
                match name with
                | Some name -> os.AppendString(FSComp.SR.notAFunctionButMaybeIndexerWithName2 name)
                | _ -> os.AppendString(FSComp.SR.notAFunctionButMaybeIndexer2 ())

        | NotAFunction (_, _, _, marg) ->
            if marg.StartColumn = 0 then
                os.AppendString(FSComp.SR.notAFunctionButMaybeDeclaration ())
            else
                os.AppendString(FSComp.SR.notAFunction ())

        | TyconBadArgs (_, tcref, d, _) ->
            let exp = tcref.TyparsNoRange.Length

            if exp = 0 then
                os.AppendString(FSComp.SR.buildUnexpectedTypeArgs (fullDisplayTextOfTyconRef tcref, d))
            else
                os.AppendString(TyconBadArgsE().Format (fullDisplayTextOfTyconRef tcref) exp d)

        | IndeterminateType _ -> os.AppendString(IndeterminateTypeE().Format)

        | NameClash (nm, k1, nm1, _, k2, nm2, _) ->
            if nm = nm1 && nm1 = nm2 && k1 = k2 then
                os.AppendString(NameClash1E().Format k1 nm1)
            else
                os.AppendString(NameClash2E().Format k1 nm1 nm k2 nm2)

        | Duplicate (k, s, _) ->
            if k = "member" then
                os.AppendString(Duplicate1E().Format(DecompileOpName s))
            else
                os.AppendString(Duplicate2E().Format k (DecompileOpName s))

        | UndefinedName (_, k, id, suggestionsF) ->
            os.AppendString(k (DecompileOpName id.idText))
            suggestNames suggestionsF id.idText

        | InternalUndefinedItemRef (f, smr, ccuName, s) ->
            let _, errs = f (smr, ccuName, s)
            os.AppendString errs

        | FieldNotMutable _ -> os.AppendString(FieldNotMutableE().Format)

        | FieldsFromDifferentTypes (_, fref1, fref2, _) ->
            os.AppendString(FieldsFromDifferentTypesE().Format fref1.FieldName fref2.FieldName)

        | VarBoundTwice id -> os.AppendString(VarBoundTwiceE().Format(DecompileOpName id.idText))

        | Recursion (denv, id, ty1, ty2, _) ->
            let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(RecursionE().Format (DecompileOpName id.idText) ty1 ty2 tpcs)

        | InvalidRuntimeCoercion (denv, ty1, ty2, _) ->
            let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(InvalidRuntimeCoercionE().Format ty1 ty2 tpcs)

        | IndeterminateRuntimeCoercion (denv, ty1, ty2, _) ->
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(IndeterminateRuntimeCoercionE().Format ty1 ty2)

        | IndeterminateStaticCoercion (denv, ty1, ty2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(IndeterminateStaticCoercionE().Format ty1 ty2)

        | StaticCoercionShouldUseBox (denv, ty1, ty2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(StaticCoercionShouldUseBoxE().Format ty1 ty2)

        | TypeIsImplicitlyAbstract _ -> os.AppendString(TypeIsImplicitlyAbstractE().Format)

        | NonRigidTypar (denv, tpnmOpt, typarRange, ty1, ty2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let (ty1, ty2), _cxs = PrettyTypes.PrettifyTypePair denv.g (ty1, ty2)

            match tpnmOpt with
            | None -> os.AppendString(NonRigidTypar1E().Format (stringOfRange typarRange) (NicePrint.stringOfTy denv ty2))
            | Some tpnm ->
                match ty1 with
                | TType_measure _ -> os.AppendString(NonRigidTypar2E().Format tpnm (NicePrint.stringOfTy denv ty2))
                | _ -> os.AppendString(NonRigidTypar3E().Format tpnm (NicePrint.stringOfTy denv ty2))

        | SyntaxError (ctxt, _) ->
            let ctxt = unbox<Parsing.ParseErrorContext<Parser.token>> (ctxt)

            let (|EndOfStructuredConstructToken|_|) token =
                match token with
                | Parser.TOKEN_ODECLEND
                | Parser.TOKEN_OBLOCKSEP
                | Parser.TOKEN_OEND
                | Parser.TOKEN_ORIGHT_BLOCK_END
                | Parser.TOKEN_OBLOCKEND
                | Parser.TOKEN_OBLOCKEND_COMING_SOON
                | Parser.TOKEN_OBLOCKEND_IS_HERE -> Some()
                | _ -> None

            let tokenIdToText tid =
                match tid with
                | Parser.TOKEN_IDENT -> getErrorString ("Parser.TOKEN.IDENT")
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
                | Parser.TOKEN_NATIVEINT -> getErrorString ("Parser.TOKEN.INT")
                | Parser.TOKEN_IEEE32
                | Parser.TOKEN_IEEE64 -> getErrorString ("Parser.TOKEN.FLOAT")
                | Parser.TOKEN_DECIMAL -> getErrorString ("Parser.TOKEN.DECIMAL")
                | Parser.TOKEN_CHAR -> getErrorString ("Parser.TOKEN.CHAR")

                | Parser.TOKEN_BASE -> getErrorString ("Parser.TOKEN.BASE")
                | Parser.TOKEN_LPAREN_STAR_RPAREN -> getErrorString ("Parser.TOKEN.LPAREN.STAR.RPAREN")
                | Parser.TOKEN_DOLLAR -> getErrorString ("Parser.TOKEN.DOLLAR")
                | Parser.TOKEN_INFIX_STAR_STAR_OP -> getErrorString ("Parser.TOKEN.INFIX.STAR.STAR.OP")
                | Parser.TOKEN_INFIX_COMPARE_OP -> getErrorString ("Parser.TOKEN.INFIX.COMPARE.OP")
                | Parser.TOKEN_COLON_GREATER -> getErrorString ("Parser.TOKEN.COLON.GREATER")
                | Parser.TOKEN_COLON_COLON -> getErrorString ("Parser.TOKEN.COLON.COLON")
                | Parser.TOKEN_PERCENT_OP -> getErrorString ("Parser.TOKEN.PERCENT.OP")
                | Parser.TOKEN_INFIX_AT_HAT_OP -> getErrorString ("Parser.TOKEN.INFIX.AT.HAT.OP")
                | Parser.TOKEN_INFIX_BAR_OP -> getErrorString ("Parser.TOKEN.INFIX.BAR.OP")
                | Parser.TOKEN_PLUS_MINUS_OP -> getErrorString ("Parser.TOKEN.PLUS.MINUS.OP")
                | Parser.TOKEN_PREFIX_OP -> getErrorString ("Parser.TOKEN.PREFIX.OP")
                | Parser.TOKEN_COLON_QMARK_GREATER -> getErrorString ("Parser.TOKEN.COLON.QMARK.GREATER")
                | Parser.TOKEN_INFIX_STAR_DIV_MOD_OP -> getErrorString ("Parser.TOKEN.INFIX.STAR.DIV.MOD.OP")
                | Parser.TOKEN_INFIX_AMP_OP -> getErrorString ("Parser.TOKEN.INFIX.AMP.OP")
                | Parser.TOKEN_AMP -> getErrorString ("Parser.TOKEN.AMP")
                | Parser.TOKEN_AMP_AMP -> getErrorString ("Parser.TOKEN.AMP.AMP")
                | Parser.TOKEN_BAR_BAR -> getErrorString ("Parser.TOKEN.BAR.BAR")
                | Parser.TOKEN_LESS -> getErrorString ("Parser.TOKEN.LESS")
                | Parser.TOKEN_GREATER -> getErrorString ("Parser.TOKEN.GREATER")
                | Parser.TOKEN_QMARK -> getErrorString ("Parser.TOKEN.QMARK")
                | Parser.TOKEN_QMARK_QMARK -> getErrorString ("Parser.TOKEN.QMARK.QMARK")
                | Parser.TOKEN_COLON_QMARK -> getErrorString ("Parser.TOKEN.COLON.QMARK")
                | Parser.TOKEN_INT32_DOT_DOT -> getErrorString ("Parser.TOKEN.INT32.DOT.DOT")
                | Parser.TOKEN_DOT_DOT -> getErrorString ("Parser.TOKEN.DOT.DOT")
                | Parser.TOKEN_DOT_DOT_HAT -> getErrorString ("Parser.TOKEN.DOT.DOT")
                | Parser.TOKEN_QUOTE -> getErrorString ("Parser.TOKEN.QUOTE")
                | Parser.TOKEN_STAR -> getErrorString ("Parser.TOKEN.STAR")
                | Parser.TOKEN_HIGH_PRECEDENCE_TYAPP -> getErrorString ("Parser.TOKEN.HIGH.PRECEDENCE.TYAPP")
                | Parser.TOKEN_COLON -> getErrorString ("Parser.TOKEN.COLON")
                | Parser.TOKEN_COLON_EQUALS -> getErrorString ("Parser.TOKEN.COLON.EQUALS")
                | Parser.TOKEN_LARROW -> getErrorString ("Parser.TOKEN.LARROW")
                | Parser.TOKEN_EQUALS -> getErrorString ("Parser.TOKEN.EQUALS")
                | Parser.TOKEN_GREATER_BAR_RBRACK -> getErrorString ("Parser.TOKEN.GREATER.BAR.RBRACK")
                | Parser.TOKEN_MINUS -> getErrorString ("Parser.TOKEN.MINUS")
                | Parser.TOKEN_ADJACENT_PREFIX_OP -> getErrorString ("Parser.TOKEN.ADJACENT.PREFIX.OP")
                | Parser.TOKEN_FUNKY_OPERATOR_NAME -> getErrorString ("Parser.TOKEN.FUNKY.OPERATOR.NAME")
                | Parser.TOKEN_COMMA -> getErrorString ("Parser.TOKEN.COMMA")
                | Parser.TOKEN_DOT -> getErrorString ("Parser.TOKEN.DOT")
                | Parser.TOKEN_BAR -> getErrorString ("Parser.TOKEN.BAR")
                | Parser.TOKEN_HASH -> getErrorString ("Parser.TOKEN.HASH")
                | Parser.TOKEN_UNDERSCORE -> getErrorString ("Parser.TOKEN.UNDERSCORE")
                | Parser.TOKEN_SEMICOLON -> getErrorString ("Parser.TOKEN.SEMICOLON")
                | Parser.TOKEN_SEMICOLON_SEMICOLON -> getErrorString ("Parser.TOKEN.SEMICOLON.SEMICOLON")
                | Parser.TOKEN_LPAREN -> getErrorString ("Parser.TOKEN.LPAREN")
                | Parser.TOKEN_RPAREN
                | Parser.TOKEN_RPAREN_COMING_SOON
                | Parser.TOKEN_RPAREN_IS_HERE -> getErrorString ("Parser.TOKEN.RPAREN")
                | Parser.TOKEN_LQUOTE -> getErrorString ("Parser.TOKEN.LQUOTE")
                | Parser.TOKEN_LBRACK -> getErrorString ("Parser.TOKEN.LBRACK")
                | Parser.TOKEN_LBRACE_BAR -> getErrorString ("Parser.TOKEN.LBRACE.BAR")
                | Parser.TOKEN_LBRACK_BAR -> getErrorString ("Parser.TOKEN.LBRACK.BAR")
                | Parser.TOKEN_LBRACK_LESS -> getErrorString ("Parser.TOKEN.LBRACK.LESS")
                | Parser.TOKEN_LBRACE -> getErrorString ("Parser.TOKEN.LBRACE")
                | Parser.TOKEN_BAR_RBRACK -> getErrorString ("Parser.TOKEN.BAR.RBRACK")
                | Parser.TOKEN_BAR_RBRACE -> getErrorString ("Parser.TOKEN.BAR.RBRACE")
                | Parser.TOKEN_GREATER_RBRACK -> getErrorString ("Parser.TOKEN.GREATER.RBRACK")
                | Parser.TOKEN_RQUOTE_DOT _
                | Parser.TOKEN_RQUOTE -> getErrorString ("Parser.TOKEN.RQUOTE")
                | Parser.TOKEN_RBRACK -> getErrorString ("Parser.TOKEN.RBRACK")
                | Parser.TOKEN_RBRACE
                | Parser.TOKEN_RBRACE_COMING_SOON
                | Parser.TOKEN_RBRACE_IS_HERE -> getErrorString ("Parser.TOKEN.RBRACE")
                | Parser.TOKEN_PUBLIC -> getErrorString ("Parser.TOKEN.PUBLIC")
                | Parser.TOKEN_PRIVATE -> getErrorString ("Parser.TOKEN.PRIVATE")
                | Parser.TOKEN_INTERNAL -> getErrorString ("Parser.TOKEN.INTERNAL")
                | Parser.TOKEN_CONSTRAINT -> getErrorString ("Parser.TOKEN.CONSTRAINT")
                | Parser.TOKEN_INSTANCE -> getErrorString ("Parser.TOKEN.INSTANCE")
                | Parser.TOKEN_DELEGATE -> getErrorString ("Parser.TOKEN.DELEGATE")
                | Parser.TOKEN_INHERIT -> getErrorString ("Parser.TOKEN.INHERIT")
                | Parser.TOKEN_CONSTRUCTOR -> getErrorString ("Parser.TOKEN.CONSTRUCTOR")
                | Parser.TOKEN_DEFAULT -> getErrorString ("Parser.TOKEN.DEFAULT")
                | Parser.TOKEN_OVERRIDE -> getErrorString ("Parser.TOKEN.OVERRIDE")
                | Parser.TOKEN_ABSTRACT -> getErrorString ("Parser.TOKEN.ABSTRACT")
                | Parser.TOKEN_CLASS -> getErrorString ("Parser.TOKEN.CLASS")
                | Parser.TOKEN_MEMBER -> getErrorString ("Parser.TOKEN.MEMBER")
                | Parser.TOKEN_STATIC -> getErrorString ("Parser.TOKEN.STATIC")
                | Parser.TOKEN_NAMESPACE -> getErrorString ("Parser.TOKEN.NAMESPACE")
                | Parser.TOKEN_OBLOCKBEGIN -> getErrorString ("Parser.TOKEN.OBLOCKBEGIN")
                | EndOfStructuredConstructToken -> getErrorString ("Parser.TOKEN.OBLOCKEND")
                | Parser.TOKEN_THEN
                | Parser.TOKEN_OTHEN -> getErrorString ("Parser.TOKEN.OTHEN")
                | Parser.TOKEN_ELSE
                | Parser.TOKEN_OELSE -> getErrorString ("Parser.TOKEN.OELSE")
                | Parser.TOKEN_LET _
                | Parser.TOKEN_OLET _ -> getErrorString ("Parser.TOKEN.OLET")
                | Parser.TOKEN_OBINDER
                | Parser.TOKEN_BINDER -> getErrorString ("Parser.TOKEN.BINDER")
                | Parser.TOKEN_OAND_BANG
                | Parser.TOKEN_AND_BANG -> getErrorString ("Parser.TOKEN.AND.BANG")
                | Parser.TOKEN_ODO -> getErrorString ("Parser.TOKEN.ODO")
                | Parser.TOKEN_OWITH -> getErrorString ("Parser.TOKEN.OWITH")
                | Parser.TOKEN_OFUNCTION -> getErrorString ("Parser.TOKEN.OFUNCTION")
                | Parser.TOKEN_OFUN -> getErrorString ("Parser.TOKEN.OFUN")
                | Parser.TOKEN_ORESET -> getErrorString ("Parser.TOKEN.ORESET")
                | Parser.TOKEN_ODUMMY -> getErrorString ("Parser.TOKEN.ODUMMY")
                | Parser.TOKEN_DO_BANG
                | Parser.TOKEN_ODO_BANG -> getErrorString ("Parser.TOKEN.ODO.BANG")
                | Parser.TOKEN_YIELD -> getErrorString ("Parser.TOKEN.YIELD")
                | Parser.TOKEN_YIELD_BANG -> getErrorString ("Parser.TOKEN.YIELD.BANG")
                | Parser.TOKEN_OINTERFACE_MEMBER -> getErrorString ("Parser.TOKEN.OINTERFACE.MEMBER")
                | Parser.TOKEN_ELIF -> getErrorString ("Parser.TOKEN.ELIF")
                | Parser.TOKEN_RARROW -> getErrorString ("Parser.TOKEN.RARROW")
                | Parser.TOKEN_SIG -> getErrorString ("Parser.TOKEN.SIG")
                | Parser.TOKEN_STRUCT -> getErrorString ("Parser.TOKEN.STRUCT")
                | Parser.TOKEN_UPCAST -> getErrorString ("Parser.TOKEN.UPCAST")
                | Parser.TOKEN_DOWNCAST -> getErrorString ("Parser.TOKEN.DOWNCAST")
                | Parser.TOKEN_NULL -> getErrorString ("Parser.TOKEN.NULL")
                | Parser.TOKEN_RESERVED -> getErrorString ("Parser.TOKEN.RESERVED")
                | Parser.TOKEN_MODULE
                | Parser.TOKEN_MODULE_COMING_SOON
                | Parser.TOKEN_MODULE_IS_HERE -> getErrorString ("Parser.TOKEN.MODULE")
                | Parser.TOKEN_AND -> getErrorString ("Parser.TOKEN.AND")
                | Parser.TOKEN_AS -> getErrorString ("Parser.TOKEN.AS")
                | Parser.TOKEN_ASSERT -> getErrorString ("Parser.TOKEN.ASSERT")
                | Parser.TOKEN_OASSERT -> getErrorString ("Parser.TOKEN.ASSERT")
                | Parser.TOKEN_ASR -> getErrorString ("Parser.TOKEN.ASR")
                | Parser.TOKEN_DOWNTO -> getErrorString ("Parser.TOKEN.DOWNTO")
                | Parser.TOKEN_EXCEPTION -> getErrorString ("Parser.TOKEN.EXCEPTION")
                | Parser.TOKEN_FALSE -> getErrorString ("Parser.TOKEN.FALSE")
                | Parser.TOKEN_FOR -> getErrorString ("Parser.TOKEN.FOR")
                | Parser.TOKEN_FUN -> getErrorString ("Parser.TOKEN.FUN")
                | Parser.TOKEN_FUNCTION -> getErrorString ("Parser.TOKEN.FUNCTION")
                | Parser.TOKEN_FINALLY -> getErrorString ("Parser.TOKEN.FINALLY")
                | Parser.TOKEN_LAZY -> getErrorString ("Parser.TOKEN.LAZY")
                | Parser.TOKEN_OLAZY -> getErrorString ("Parser.TOKEN.LAZY")
                | Parser.TOKEN_MATCH -> getErrorString ("Parser.TOKEN.MATCH")
                | Parser.TOKEN_MATCH_BANG -> getErrorString ("Parser.TOKEN.MATCH.BANG")
                | Parser.TOKEN_MUTABLE -> getErrorString ("Parser.TOKEN.MUTABLE")
                | Parser.TOKEN_NEW -> getErrorString ("Parser.TOKEN.NEW")
                | Parser.TOKEN_OF -> getErrorString ("Parser.TOKEN.OF")
                | Parser.TOKEN_OPEN -> getErrorString ("Parser.TOKEN.OPEN")
                | Parser.TOKEN_OR -> getErrorString ("Parser.TOKEN.OR")
                | Parser.TOKEN_VOID -> getErrorString ("Parser.TOKEN.VOID")
                | Parser.TOKEN_EXTERN -> getErrorString ("Parser.TOKEN.EXTERN")
                | Parser.TOKEN_INTERFACE -> getErrorString ("Parser.TOKEN.INTERFACE")
                | Parser.TOKEN_REC -> getErrorString ("Parser.TOKEN.REC")
                | Parser.TOKEN_TO -> getErrorString ("Parser.TOKEN.TO")
                | Parser.TOKEN_TRUE -> getErrorString ("Parser.TOKEN.TRUE")
                | Parser.TOKEN_TRY -> getErrorString ("Parser.TOKEN.TRY")
                | Parser.TOKEN_TYPE
                | Parser.TOKEN_TYPE_COMING_SOON
                | Parser.TOKEN_TYPE_IS_HERE -> getErrorString ("Parser.TOKEN.TYPE")
                | Parser.TOKEN_VAL -> getErrorString ("Parser.TOKEN.VAL")
                | Parser.TOKEN_INLINE -> getErrorString ("Parser.TOKEN.INLINE")
                | Parser.TOKEN_WHEN -> getErrorString ("Parser.TOKEN.WHEN")
                | Parser.TOKEN_WHILE -> getErrorString ("Parser.TOKEN.WHILE")
                | Parser.TOKEN_WITH -> getErrorString ("Parser.TOKEN.WITH")
                | Parser.TOKEN_IF -> getErrorString ("Parser.TOKEN.IF")
                | Parser.TOKEN_DO -> getErrorString ("Parser.TOKEN.DO")
                | Parser.TOKEN_GLOBAL -> getErrorString ("Parser.TOKEN.GLOBAL")
                | Parser.TOKEN_DONE -> getErrorString ("Parser.TOKEN.DONE")
                | Parser.TOKEN_IN
                | Parser.TOKEN_JOIN_IN -> getErrorString ("Parser.TOKEN.IN")
                | Parser.TOKEN_HIGH_PRECEDENCE_PAREN_APP -> getErrorString ("Parser.TOKEN.HIGH.PRECEDENCE.PAREN.APP")
                | Parser.TOKEN_HIGH_PRECEDENCE_BRACK_APP -> getErrorString ("Parser.TOKEN.HIGH.PRECEDENCE.BRACK.APP")
                | Parser.TOKEN_BEGIN -> getErrorString ("Parser.TOKEN.BEGIN")
                | Parser.TOKEN_END -> getErrorString ("Parser.TOKEN.END")
                | Parser.TOKEN_HASH_LIGHT
                | Parser.TOKEN_HASH_LINE
                | Parser.TOKEN_HASH_IF
                | Parser.TOKEN_HASH_ELSE
                | Parser.TOKEN_HASH_ENDIF -> getErrorString ("Parser.TOKEN.HASH.ENDIF")
                | Parser.TOKEN_INACTIVECODE -> getErrorString ("Parser.TOKEN.INACTIVECODE")
                | Parser.TOKEN_LEX_FAILURE -> getErrorString ("Parser.TOKEN.LEX.FAILURE")
                | Parser.TOKEN_WHITESPACE -> getErrorString ("Parser.TOKEN.WHITESPACE")
                | Parser.TOKEN_COMMENT -> getErrorString ("Parser.TOKEN.COMMENT")
                | Parser.TOKEN_LINE_COMMENT -> getErrorString ("Parser.TOKEN.LINE.COMMENT")
                | Parser.TOKEN_STRING_TEXT -> getErrorString ("Parser.TOKEN.STRING.TEXT")
                | Parser.TOKEN_BYTEARRAY -> getErrorString ("Parser.TOKEN.BYTEARRAY")
                | Parser.TOKEN_STRING -> getErrorString ("Parser.TOKEN.STRING")
                | Parser.TOKEN_KEYWORD_STRING -> getErrorString ("Parser.TOKEN.KEYWORD_STRING")
                | Parser.TOKEN_EOF -> getErrorString ("Parser.TOKEN.EOF")
                | Parser.TOKEN_CONST -> getErrorString ("Parser.TOKEN.CONST")
                | Parser.TOKEN_FIXED -> getErrorString ("Parser.TOKEN.FIXED")
                | Parser.TOKEN_INTERP_STRING_BEGIN_END -> getErrorString ("Parser.TOKEN.INTERP.STRING.BEGIN.END")
                | Parser.TOKEN_INTERP_STRING_BEGIN_PART -> getErrorString ("Parser.TOKEN.INTERP.STRING.BEGIN.PART")
                | Parser.TOKEN_INTERP_STRING_PART -> getErrorString ("Parser.TOKEN.INTERP.STRING.PART")
                | Parser.TOKEN_INTERP_STRING_END -> getErrorString ("Parser.TOKEN.INTERP.STRING.END")
                | unknown ->
                    Debug.Assert(false, "unknown token tag")
                    let result = sprintf "%+A" unknown
                    Debug.Assert(false, result)
                    result

#if DEBUG
            if showParserStackOnParseError then
                printfn "parser stack:"

                for rps in ctxt.ReducibleProductions do
                    printfn "   ----"
                    //printfn "   state %d" state
                    for rp in rps do
                        printfn "       non-terminal %+A (idx %d): ... " (Parser.prodIdxToNonTerminal rp) rp
#endif

            match ctxt.CurrentToken with
            | None -> os.AppendString(UnexpectedEndOfInputE().Format)
            | Some token ->
                let tokenId = token |> Parser.tagOfToken |> Parser.tokenTagToTokenId

                match tokenId, token with
                | EndOfStructuredConstructToken, _ -> os.AppendString(OBlockEndSentenceE().Format)
                | Parser.TOKEN_LEX_FAILURE, Parser.LEX_FAILURE str -> os.AppendString str
                | token, _ -> os.AppendString(UnexpectedE().Format(token |> tokenIdToText))

                // Search for a state producing a single recognized non-terminal in the states on the stack
                let foundInContext =

                    // Merge a bunch of expression non terminals
                    let (|NONTERM_Category_Expr|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_argExpr
                        | Parser.NONTERM_minusExpr
                        | Parser.NONTERM_parenExpr
                        | Parser.NONTERM_atomicExpr
                        | Parser.NONTERM_appExpr
                        | Parser.NONTERM_tupleExpr
                        | Parser.NONTERM_declExpr
                        | Parser.NONTERM_braceExpr
                        | Parser.NONTERM_braceBarExpr
                        | Parser.NONTERM_typedSequentialExprBlock
                        | Parser.NONTERM_interactiveExpr -> Some()
                        | _ -> None

                    // Merge a bunch of pattern non terminals
                    let (|NONTERM_Category_Pattern|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_constrPattern
                        | Parser.NONTERM_parenPattern
                        | Parser.NONTERM_atomicPattern -> Some()
                        | _ -> None

                    // Merge a bunch of if/then/else non terminals
                    let (|NONTERM_Category_IfThenElse|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_ifExprThen
                        | Parser.NONTERM_ifExprElifs
                        | Parser.NONTERM_ifExprCases -> Some()
                        | _ -> None

                    // Merge a bunch of non terminals
                    let (|NONTERM_Category_SignatureFile|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_signatureFile
                        | Parser.NONTERM_moduleSpfn
                        | Parser.NONTERM_moduleSpfns -> Some()
                        | _ -> None

                    let (|NONTERM_Category_ImplementationFile|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_implementationFile
                        | Parser.NONTERM_fileNamespaceImpl
                        | Parser.NONTERM_fileNamespaceImpls -> Some()
                        | _ -> None

                    let (|NONTERM_Category_Definition|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_fileModuleImpl
                        | Parser.NONTERM_moduleDefn
                        | Parser.NONTERM_interactiveDefns
                        | Parser.NONTERM_moduleDefns
                        | Parser.NONTERM_moduleDefnsOrExpr -> Some()
                        | _ -> None

                    let (|NONTERM_Category_Type|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_typ
                        | Parser.NONTERM_tupleType -> Some()
                        | _ -> None

                    let (|NONTERM_Category_Interaction|_|) nonTerminal =
                        match nonTerminal with
                        | Parser.NONTERM_interactiveItemsTerminator
                        | Parser.NONTERM_interaction
                        | Parser.NONTERM__startinteraction -> Some()
                        | _ -> None

                    // Canonicalize the categories and check for a unique category
                    ctxt.ReducibleProductions
                    |> List.exists (fun prods ->
                        let prodIds =
                            prods
                            |> List.map Parser.prodIdxToNonTerminal
                            |> List.map (fun nonTerminal ->
                                match nonTerminal with
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
                            |> Set.toList

                        match prodIds with
                        | [ Parser.NONTERM_interaction ] ->
                            os.AppendString(NONTERM_interactionE().Format)
                            true
                        | [ Parser.NONTERM_hashDirective ] ->
                            os.AppendString(NONTERM_hashDirectiveE().Format)
                            true
                        | [ Parser.NONTERM_fieldDecl ] ->
                            os.AppendString(NONTERM_fieldDeclE().Format)
                            true
                        | [ Parser.NONTERM_unionCaseRepr ] ->
                            os.AppendString(NONTERM_unionCaseReprE().Format)
                            true
                        | [ Parser.NONTERM_localBinding ] ->
                            os.AppendString(NONTERM_localBindingE().Format)
                            true
                        | [ Parser.NONTERM_hardwhiteLetBindings ] ->
                            os.AppendString(NONTERM_hardwhiteLetBindingsE().Format)
                            true
                        | [ Parser.NONTERM_classDefnMember ] ->
                            os.AppendString(NONTERM_classDefnMemberE().Format)
                            true
                        | [ Parser.NONTERM_defnBindings ] ->
                            os.AppendString(NONTERM_defnBindingsE().Format)
                            true
                        | [ Parser.NONTERM_classMemberSpfn ] ->
                            os.AppendString(NONTERM_classMemberSpfnE().Format)
                            true
                        | [ Parser.NONTERM_valSpfn ] ->
                            os.AppendString(NONTERM_valSpfnE().Format)
                            true
                        | [ Parser.NONTERM_tyconSpfn ] ->
                            os.AppendString(NONTERM_tyconSpfnE().Format)
                            true
                        | [ Parser.NONTERM_anonLambdaExpr ] ->
                            os.AppendString(NONTERM_anonLambdaExprE().Format)
                            true
                        | [ Parser.NONTERM_attrUnionCaseDecl ] ->
                            os.AppendString(NONTERM_attrUnionCaseDeclE().Format)
                            true
                        | [ Parser.NONTERM_cPrototype ] ->
                            os.AppendString(NONTERM_cPrototypeE().Format)
                            true
                        | [ Parser.NONTERM_objExpr | Parser.NONTERM_objectImplementationMembers ] ->
                            os.AppendString(NONTERM_objectImplementationMembersE().Format)
                            true
                        | [ Parser.NONTERM_ifExprThen | Parser.NONTERM_ifExprElifs | Parser.NONTERM_ifExprCases ] ->
                            os.AppendString(NONTERM_ifExprCasesE().Format)
                            true
                        | [ Parser.NONTERM_openDecl ] ->
                            os.AppendString(NONTERM_openDeclE().Format)
                            true
                        | [ Parser.NONTERM_fileModuleSpec ] ->
                            os.AppendString(NONTERM_fileModuleSpecE().Format)
                            true
                        | [ Parser.NONTERM_patternClauses ] ->
                            os.AppendString(NONTERM_patternClausesE().Format)
                            true
                        | [ Parser.NONTERM_beginEndExpr ] ->
                            os.AppendString(NONTERM_beginEndExprE().Format)
                            true
                        | [ Parser.NONTERM_recdExpr ] ->
                            os.AppendString(NONTERM_recdExprE().Format)
                            true
                        | [ Parser.NONTERM_tyconDefn ] ->
                            os.AppendString(NONTERM_tyconDefnE().Format)
                            true
                        | [ Parser.NONTERM_exconCore ] ->
                            os.AppendString(NONTERM_exconCoreE().Format)
                            true
                        | [ Parser.NONTERM_typeNameInfo ] ->
                            os.AppendString(NONTERM_typeNameInfoE().Format)
                            true
                        | [ Parser.NONTERM_attributeList ] ->
                            os.AppendString(NONTERM_attributeListE().Format)
                            true
                        | [ Parser.NONTERM_quoteExpr ] ->
                            os.AppendString(NONTERM_quoteExprE().Format)
                            true
                        | [ Parser.NONTERM_typeConstraint ] ->
                            os.AppendString(NONTERM_typeConstraintE().Format)
                            true
                        | [ NONTERM_Category_ImplementationFile ] ->
                            os.AppendString(NONTERM_Category_ImplementationFileE().Format)
                            true
                        | [ NONTERM_Category_Definition ] ->
                            os.AppendString(NONTERM_Category_DefinitionE().Format)
                            true
                        | [ NONTERM_Category_SignatureFile ] ->
                            os.AppendString(NONTERM_Category_SignatureFileE().Format)
                            true
                        | [ NONTERM_Category_Pattern ] ->
                            os.AppendString(NONTERM_Category_PatternE().Format)
                            true
                        | [ NONTERM_Category_Expr ] ->
                            os.AppendString(NONTERM_Category_ExprE().Format)
                            true
                        | [ NONTERM_Category_Type ] ->
                            os.AppendString(NONTERM_Category_TypeE().Format)
                            true
                        | [ Parser.NONTERM_typeArgsActual ] ->
                            os.AppendString(NONTERM_typeArgsActualE().Format)
                            true
                        | _ -> false)

#if DEBUG
                if not foundInContext then
                    Printf.bprintf
                        os
                        ". (no 'in' context found: %+A)"
                        (List.mapSquared Parser.prodIdxToNonTerminal ctxt.ReducibleProductions)
#else
                foundInContext |> ignore // suppress unused variable warning in RELEASE
#endif
                let fix (s: string) =
                    s
                        .Replace(SR.GetString("FixKeyword"), "")
                        .Replace(SR.GetString("FixSymbol"), "")
                        .Replace(SR.GetString("FixReplace"), "")

                let tokenNames =
                    ctxt.ShiftTokens
                    |> List.map Parser.tokenTagToTokenId
                    |> List.filter (function
                        | Parser.TOKEN_error
                        | Parser.TOKEN_EOF -> false
                        | _ -> true)
                    |> List.map tokenIdToText
                    |> Set.ofList
                    |> Set.toList

                match tokenNames with
                | [ tokenName1 ] -> os.AppendString(TokenName1E().Format(fix tokenName1))
                | [ tokenName1; tokenName2 ] -> os.AppendString(TokenName1TokenName2E().Format (fix tokenName1) (fix tokenName2))
                | [ tokenName1; tokenName2; tokenName3 ] ->
                    os.AppendString(TokenName1TokenName2TokenName3E().Format (fix tokenName1) (fix tokenName2) (fix tokenName3))
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

        | RuntimeCoercionSourceSealed (denv, ty, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty

            if isTyparTy denv.g ty then
                os.AppendString(RuntimeCoercionSourceSealed1E().Format(NicePrint.stringOfTy denv ty))
            else
                os.AppendString(RuntimeCoercionSourceSealed2E().Format(NicePrint.stringOfTy denv ty))

        | CoercionTargetSealed (denv, ty, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            os.AppendString(CoercionTargetSealedE().Format(NicePrint.stringOfTy denv ty))

        | UpcastUnnecessary _ -> os.AppendString(UpcastUnnecessaryE().Format)

        | TypeTestUnnecessary _ -> os.AppendString(TypeTestUnnecessaryE().Format)

        | QuotationTranslator.IgnoringPartOfQuotedTermWarning (msg, _) -> Printf.bprintf os "%s" msg

        | OverrideDoesntOverride (denv, impl, minfoVirtOpt, g, amap, m) ->
            let sig1 = DispatchSlotChecking.FormatOverride denv impl

            match minfoVirtOpt with
            | None -> os.AppendString(OverrideDoesntOverride1E().Format sig1)
            | Some minfoVirt ->
                // https://github.com/dotnet/fsharp/issues/35
                // Improve error message when attempting to override generic return type with unit:
                // we need to check if unit was used as a type argument
                let hasUnitTType_app (types: TType list) =
                    types
                    |> List.exists (function
                        | TType_app (maybeUnit, [], _) ->
                            match maybeUnit.TypeAbbrev with
                            | Some ttype when isUnitTy g ttype -> true
                            | _ -> false
                        | _ -> false)

                match minfoVirt.ApparentEnclosingType with
                | TType_app (tycon, tyargs, _) when tycon.IsFSharpInterfaceTycon && hasUnitTType_app tyargs ->
                    // match abstract member with 'unit' passed as generic argument
                    os.AppendString(OverrideDoesntOverride4E().Format sig1)
                | _ ->
                    os.AppendString(OverrideDoesntOverride2E().Format sig1)
                    let sig2 = DispatchSlotChecking.FormatMethInfoSig g amap m denv minfoVirt

                    if sig1 <> sig2 then
                        os.AppendString(OverrideDoesntOverride3E().Format sig2)

        | UnionCaseWrongArguments (_, n1, n2, _) -> os.AppendString(UnionCaseWrongArgumentsE().Format n2 n1)

        | UnionPatternsBindDifferentNames _ -> os.AppendString(UnionPatternsBindDifferentNamesE().Format)

        | ValueNotContained (denv, infoReader, mref, implVal, sigVal, f) ->
            let text1, text2 =
                NicePrint.minimalStringsOfTwoValues denv infoReader (mkLocalValRef implVal) (mkLocalValRef sigVal)

            os.AppendString(f ((fullDisplayTextOfModRef mref), text1, text2))

        | UnionCaseNotContained (denv, infoReader, enclosingTycon, v1, v2, f) ->
            let enclosingTcref = mkLocalEntityRef enclosingTycon

            os.AppendString(
                f (
                    (NicePrint.stringOfUnionCase denv infoReader enclosingTcref v1),
                    (NicePrint.stringOfUnionCase denv infoReader enclosingTcref v2)
                )
            )

        | FSharpExceptionNotContained (denv, infoReader, v1, v2, f) ->
            os.AppendString(
                f (
                    (NicePrint.stringOfExnDef denv infoReader (mkLocalEntityRef v1)),
                    (NicePrint.stringOfExnDef denv infoReader (mkLocalEntityRef v2))
                )
            )

        | FieldNotContained (denv, infoReader, enclosingTycon, v1, v2, f) ->
            let enclosingTcref = mkLocalEntityRef enclosingTycon

            os.AppendString(
                f (
                    (NicePrint.stringOfRecdField denv infoReader enclosingTcref v1),
                    (NicePrint.stringOfRecdField denv infoReader enclosingTcref v2)
                )
            )

        | RequiredButNotSpecified (_, mref, k, name, _) ->
            let nsb = StringBuilder()
            name nsb
            os.AppendString(RequiredButNotSpecifiedE().Format (fullDisplayTextOfModRef mref) k (nsb.ToString()))

        | UseOfAddressOfOperator _ -> os.AppendString(UseOfAddressOfOperatorE().Format)

        | DefensiveCopyWarning (s, _) -> os.AppendString(DefensiveCopyWarningE().Format s)

        | DeprecatedThreadStaticBindingWarning _ -> os.AppendString(DeprecatedThreadStaticBindingWarningE().Format)

        | FunctionValueUnexpected (denv, ty, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            let errorText = FunctionValueUnexpectedE().Format(NicePrint.stringOfTy denv ty)
            os.AppendString errorText

        | UnitTypeExpected (denv, ty, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            let warningText = UnitTypeExpectedE().Format(NicePrint.stringOfTy denv ty)
            os.AppendString warningText

        | UnitTypeExpectedWithEquality (denv, ty, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty

            let warningText =
                UnitTypeExpectedWithEqualityE().Format(NicePrint.stringOfTy denv ty)

            os.AppendString warningText

        | UnitTypeExpectedWithPossiblePropertySetter (denv, ty, bindingName, propertyName, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty

            let warningText =
                UnitTypeExpectedWithPossiblePropertySetterE().Format (NicePrint.stringOfTy denv ty) bindingName propertyName

            os.AppendString warningText

        | UnitTypeExpectedWithPossibleAssignment (denv, ty, isAlreadyMutable, bindingName, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty

            let warningText =
                if isAlreadyMutable then
                    UnitTypeExpectedWithPossibleAssignmentToMutableE().Format (NicePrint.stringOfTy denv ty) bindingName
                else
                    UnitTypeExpectedWithPossibleAssignmentE().Format (NicePrint.stringOfTy denv ty) bindingName

            os.AppendString warningText

        | RecursiveUseCheckedAtRuntime _ -> os.AppendString(RecursiveUseCheckedAtRuntimeE().Format)

        | LetRecUnsound (_, [ v ], _) -> os.AppendString(LetRecUnsound1E().Format v.DisplayName)

        | LetRecUnsound (_, path, _) ->
            let bos = StringBuilder()

            (path.Tail @ [ path.Head ])
            |> List.iter (fun (v: ValRef) -> bos.AppendString(LetRecUnsoundInnerE().Format v.DisplayName))

            os.AppendString(LetRecUnsound2E().Format (List.head path).DisplayName (bos.ToString()))

        | LetRecEvaluatedOutOfOrder (_, _, _, _) -> os.AppendString(LetRecEvaluatedOutOfOrderE().Format)

        | LetRecCheckedAtRuntime _ -> os.AppendString(LetRecCheckedAtRuntimeE().Format)

        | SelfRefObjCtor (false, _) -> os.AppendString(SelfRefObjCtor1E().Format)

        | SelfRefObjCtor (true, _) -> os.AppendString(SelfRefObjCtor2E().Format)

        | VirtualAugmentationOnNullValuedType _ -> os.AppendString(VirtualAugmentationOnNullValuedTypeE().Format)

        | NonVirtualAugmentationOnNullValuedType _ -> os.AppendString(NonVirtualAugmentationOnNullValuedTypeE().Format)

        | NonUniqueInferredAbstractSlot (_, denv, bindnm, bvirt1, bvirt2, _) ->
            os.AppendString(NonUniqueInferredAbstractSlot1E().Format bindnm)
            let ty1 = bvirt1.ApparentEnclosingType
            let ty2 = bvirt2.ApparentEnclosingType
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(NonUniqueInferredAbstractSlot2E().Format)

            if ty1 <> ty2 then
                os.AppendString(NonUniqueInferredAbstractSlot3E().Format ty1 ty2)

            os.AppendString(NonUniqueInferredAbstractSlot4E().Format)

        | DiagnosticWithText (_, s, _) -> os.AppendString s

        | DiagnosticWithSuggestions (_, s, _, idText, suggestionF) ->
            os.AppendString(DecompileOpName s)
            suggestNames suggestionF idText

        | InternalError (s, _)

        | InvalidArgument s

        | Failure s as exn ->
            ignore exn // use the argument, even in non DEBUG
            let f1 = SR.GetString("Failure1")
            let f2 = SR.GetString("Failure2")

            match s with
            | f when f = f1 -> os.AppendString(Failure3E().Format s)
            | f when f = f2 -> os.AppendString(Failure3E().Format s)
            | _ -> os.AppendString(Failure4E().Format s)
#if DEBUG
            Printf.bprintf os "\nStack Trace\n%s\n" (exn.ToString())
            Debug.Assert(false, sprintf "Unexpected exception seen in compiler: %s\n%s" s (exn.ToString()))
#endif

        | WrappedError (exn, _) -> OutputExceptionR os exn

        | PatternMatchCompilation.MatchIncomplete (isComp, cexOpt, _) ->
            os.AppendString(MatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some (cex, false) -> os.AppendString(MatchIncomplete2E().Format cex)
            | Some (cex, true) -> os.AppendString(MatchIncomplete3E().Format cex)

            if isComp then os.AppendString(MatchIncomplete4E().Format)

        | PatternMatchCompilation.EnumMatchIncomplete (isComp, cexOpt, _) ->
            os.AppendString(EnumMatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some (cex, false) -> os.AppendString(MatchIncomplete2E().Format cex)
            | Some (cex, true) -> os.AppendString(MatchIncomplete3E().Format cex)

            if isComp then os.AppendString(MatchIncomplete4E().Format)

        | PatternMatchCompilation.RuleNeverMatched _ -> os.AppendString(RuleNeverMatchedE().Format)

        | ValNotMutable (_, valRef, _) -> os.AppendString(ValNotMutableE().Format(valRef.DisplayName))

        | ValNotLocal _ -> os.AppendString(ValNotLocalE().Format)

        | ObsoleteError (s, _)

        | ObsoleteWarning (s, _) ->
            os.AppendString(Obsolete1E().Format)
            if s <> "" then os.AppendString(Obsolete2E().Format s)

        | Experimental (s, _) -> os.AppendString(ExperimentalE().Format s)

        | PossibleUnverifiableCode _ -> os.AppendString(PossibleUnverifiableCodeE().Format)

        | UserCompilerMessage (msg, _, _) -> os.AppendString msg

        | Deprecated (s, _) -> os.AppendString(DeprecatedE().Format s)

        | LibraryUseOnly _ -> os.AppendString(LibraryUseOnlyE().Format)

        | MissingFields (sl, _) -> os.AppendString(MissingFieldsE().Format(String.concat "," sl + "."))

        | ValueRestriction (denv, infoReader, hasSig, v, _, _) ->
            let denv =
                { denv with
                    showInferenceTyparAnnotations = true
                }

            let tau = v.TauType

            if hasSig then
                if isFunTy denv.g tau && (arityOfVal v).HasNoArgs then
                    let msg =
                        ValueRestriction1E().Format
                            v.DisplayName
                            (NicePrint.stringOfQualifiedValOrMember denv infoReader (mkLocalValRef v))
                            v.DisplayName

                    os.AppendString msg
                else
                    let msg =
                        ValueRestriction2E().Format
                            v.DisplayName
                            (NicePrint.stringOfQualifiedValOrMember denv infoReader (mkLocalValRef v))
                            v.DisplayName

                    os.AppendString msg
            else
                match v.MemberInfo with
                | Some membInfo when
                    (match membInfo.MemberFlags.MemberKind with
                     | SynMemberKind.PropertyGet
                     | SynMemberKind.PropertySet
                     | SynMemberKind.Constructor -> true // can't infer extra polymorphism
                     // can infer extra polymorphism
                     | _ -> false)
                    ->
                    let msg =
                        ValueRestriction3E()
                            .Format(NicePrint.stringOfQualifiedValOrMember denv infoReader (mkLocalValRef v))

                    os.AppendString msg
                | _ ->
                    if isFunTy denv.g tau && (arityOfVal v).HasNoArgs then
                        let msg =
                            ValueRestriction4E().Format
                                v.DisplayName
                                (NicePrint.stringOfQualifiedValOrMember denv infoReader (mkLocalValRef v))
                                v.DisplayName

                        os.AppendString msg
                    else
                        let msg =
                            ValueRestriction5E().Format
                                v.DisplayName
                                (NicePrint.stringOfQualifiedValOrMember denv infoReader (mkLocalValRef v))
                                v.DisplayName

                        os.AppendString msg

        | Parsing.RecoverableParseError -> os.AppendString(RecoverableParseErrorE().Format)

        | ReservedKeyword (s, _) -> os.AppendString(ReservedKeywordE().Format s)

        | IndentationProblem (s, _) -> os.AppendString(IndentationProblemE().Format s)

        | OverrideInIntrinsicAugmentation _ -> os.AppendString(OverrideInIntrinsicAugmentationE().Format)

        | OverrideInExtrinsicAugmentation _ -> os.AppendString(OverrideInExtrinsicAugmentationE().Format)

        | IntfImplInIntrinsicAugmentation _ -> os.AppendString(IntfImplInIntrinsicAugmentationE().Format)

        | IntfImplInExtrinsicAugmentation _ -> os.AppendString(IntfImplInExtrinsicAugmentationE().Format)

        | UnresolvedReferenceError (assemblyName, _)
        | UnresolvedReferenceNoRange assemblyName -> os.AppendString(UnresolvedReferenceNoRangeE().Format assemblyName)

        | UnresolvedPathReference (assemblyName, pathname, _)

        | UnresolvedPathReferenceNoRange (assemblyName, pathname) ->
            os.AppendString(UnresolvedPathReferenceNoRangeE().Format pathname assemblyName)

        | DeprecatedCommandLineOptionFull (fullText, _) -> os.AppendString fullText

        | DeprecatedCommandLineOptionForHtmlDoc (optionName, _) -> os.AppendString(FSComp.SR.optsDCLOHtmlDoc optionName)

        | DeprecatedCommandLineOptionSuggestAlternative (optionName, altOption, _) ->
            os.AppendString(FSComp.SR.optsDCLODeprecatedSuggestAlternative (optionName, altOption))

        | InternalCommandLineOption (optionName, _) -> os.AppendString(FSComp.SR.optsInternalNoDescription optionName)

        | DeprecatedCommandLineOptionNoDescription (optionName, _) -> os.AppendString(FSComp.SR.optsDCLONoDescription optionName)

        | HashIncludeNotAllowedInNonScript _ -> os.AppendString(HashIncludeNotAllowedInNonScriptE().Format)

        | HashReferenceNotAllowedInNonScript _ -> os.AppendString(HashReferenceNotAllowedInNonScriptE().Format)

        | HashDirectiveNotAllowedInNonScript _ -> os.AppendString(HashDirectiveNotAllowedInNonScriptE().Format)

        | FileNameNotResolved (fileName, locations, _) -> os.AppendString(FileNameNotResolvedE().Format fileName locations)

        | AssemblyNotResolved (originalName, _) -> os.AppendString(AssemblyNotResolvedE().Format originalName)

        | IllegalFileNameChar (fileName, invalidChar) ->
            os.AppendString(FSComp.SR.buildUnexpectedFileNameCharacter (fileName, string invalidChar) |> snd)

        | HashLoadedSourceHasIssues (infos, warnings, errors, _) ->
            let Emit (l: exn list) = OutputExceptionR os (List.head l)

            if isNil warnings && isNil errors then
                os.AppendString(HashLoadedSourceHasIssues0E().Format)
                Emit infos
            elif isNil errors then
                os.AppendString(HashLoadedSourceHasIssues1E().Format)
                Emit warnings
            else
                os.AppendString(HashLoadedSourceHasIssues2E().Format)
                Emit errors

        | HashLoadedScriptConsideredSource _ -> os.AppendString(HashLoadedScriptConsideredSourceE().Format)

        | InvalidInternalsVisibleToAssemblyName (badName, fileNameOption) ->
            match fileNameOption with
            | Some file -> os.AppendString(InvalidInternalsVisibleToAssemblyName1E().Format badName file)
            | None -> os.AppendString(InvalidInternalsVisibleToAssemblyName2E().Format badName)

        | LoadedSourceNotFoundIgnoring (fileName, _) -> os.AppendString(LoadedSourceNotFoundIgnoringE().Format fileName)

        | MSBuildReferenceResolutionWarning (code, message, _)

        | MSBuildReferenceResolutionError (code, message, _) -> os.AppendString(MSBuildReferenceResolutionErrorE().Format message code)

        // Strip TargetInvocationException wrappers
        | :? System.Reflection.TargetInvocationException as exn -> OutputExceptionR os exn.InnerException

        | :? FileNotFoundException as exn -> Printf.bprintf os "%s" exn.Message

        | :? DirectoryNotFoundException as exn -> Printf.bprintf os "%s" exn.Message

        | :? ArgumentException as exn -> Printf.bprintf os "%s" exn.Message

        | :? NotSupportedException as exn -> Printf.bprintf os "%s" exn.Message

        | :? IOException as exn -> Printf.bprintf os "%s" exn.Message

        | :? UnauthorizedAccessException as exn -> Printf.bprintf os "%s" exn.Message

        | exn ->
            os.AppendString(TargetInvocationExceptionWrapperE().Format exn.Message)
#if DEBUG
            Printf.bprintf os "\nStack Trace\n%s\n" (exn.ToString())

            if showAssertForUnexpectedException.Value then
                Debug.Assert(false, sprintf "Unknown exception seen in compiler: %s" (exn.ToString()))
#endif

    OutputExceptionR os diagnostic.Exception

// remove any newlines and tabs
let OutputPhasedDiagnostic (os: StringBuilder) (diagnostic: PhasedDiagnostic) (flattenErrors: bool) (suggestNames: bool) =
    let buf = StringBuilder()

    OutputPhasedErrorR buf diagnostic suggestNames

    let text =
        if flattenErrors then
            NormalizeErrorString(buf.ToString())
        else
            buf.ToString()

    os.AppendString text

let SanitizeFileName fileName implicitIncludeDir =
    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy file name
    //  - if you have a #line directive, e.g.
    //        # 1000 "Line01.fs"
    //    then it also asserts. But these are edge cases that can be fixed later, e.g. in bug 4651.
    try
        let fullPath = FileSystem.GetFullPathShim fileName
        let currentDir = implicitIncludeDir

        // if the file name is not rooted in the current directory, return the full path
        if not (fullPath.StartsWithOrdinal currentDir) then
            fullPath
        // if the file name is rooted in the current directory, return the relative path
        else
            fullPath.Replace(currentDir + "\\", "")
    with _ ->
        fileName

[<RequireQualifiedAccess>]
type FormattedDiagnosticLocation =
    {
        Range: range
        File: string
        TextRepresentation: string
        IsEmpty: bool
    }

[<RequireQualifiedAccess>]
type FormattedDiagnosticCanonicalInformation =
    {
        ErrorNumber: int
        Subcategory: string
        TextRepresentation: string
    }

[<RequireQualifiedAccess>]
type FormattedDiagnosticDetailedInfo =
    {
        Location: FormattedDiagnosticLocation option
        Canonical: FormattedDiagnosticCanonicalInformation
        Message: string
    }

[<RequireQualifiedAccess>]
type FormattedDiagnostic =
    | Short of FSharpDiagnosticSeverity * string
    | Long of FSharpDiagnosticSeverity * FormattedDiagnosticDetailedInfo

/// returns sequence that contains Diagnostic for the given error + Diagnostic for all related errors
let CollectFormattedDiagnostics
    (
        implicitIncludeDir,
        showFullPaths,
        flattenErrors,
        diagnosticStyle,
        severity: FSharpDiagnosticSeverity,
        diagnostic: PhasedDiagnostic,
        suggestNames: bool
    ) =
    let outputWhere (showFullPaths, diagnosticStyle) m : FormattedDiagnosticLocation =
        if equals m rangeStartup || equals m rangeCmdArgs then
            {
                Range = m
                TextRepresentation = ""
                IsEmpty = true
                File = ""
            }
        else
            let file = m.FileName

            let file =
                if showFullPaths then
                    FileSystem.GetFullFilePathInDirectoryShim implicitIncludeDir file
                else
                    SanitizeFileName file implicitIncludeDir

            let text, m, file =
                match diagnosticStyle with
                | DiagnosticStyle.Emacs ->
                    let file = file.Replace("\\", "/")
                    (sprintf "File \"%s\", line %d, characters %d-%d: " file m.StartLine m.StartColumn m.EndColumn), m, file

                // We're adjusting the columns here to be 1-based - both for parity with C# and for MSBuild, which assumes 1-based columns for error output
                | DiagnosticStyle.Default ->
                    let file = file.Replace('/', Path.DirectorySeparatorChar)
                    let m = mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) m.End
                    (sprintf "%s(%d,%d): " file m.StartLine m.StartColumn), m, file

                // We may also want to change Test to be 1-based
                | DiagnosticStyle.Test ->
                    let file = file.Replace("/", "\\")

                    let m =
                        mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1))

                    sprintf "%s(%d,%d-%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file

                | DiagnosticStyle.Gcc ->
                    let file = file.Replace('/', Path.DirectorySeparatorChar)

                    let m =
                        mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1))

                    sprintf "%s:%d:%d: " file m.StartLine m.StartColumn, m, file

                // Here, we want the complete range information so Project Systems can generate proper squiggles
                | DiagnosticStyle.VisualStudio ->
                    // Show prefix only for real files. Otherwise, we just want a truncated error like:
                    //      parse error FS0031: blah blah
                    if
                        not (equals m range0) && not (equals m rangeStartup)
                        && not (equals m rangeCmdArgs)
                    then
                        let file = file.Replace("/", "\\")

                        let m =
                            mkRange m.FileName (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1))

                        sprintf "%s(%d,%d,%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file
                    else
                        "", m, file

            {
                Range = m
                TextRepresentation = text
                IsEmpty = false
                File = file
            }

    match diagnostic.Exception with
    | ReportedError _ ->
        assert ("" = "Unexpected ReportedError") //  this should never happen
        [||]
    | StopProcessing ->
        assert ("" = "Unexpected StopProcessing") // this should never happen
        [||]
    | _ ->
        let errors = ResizeArray()

        let report diagnostic =
            let OutputWhere diagnostic =
                match GetRangeOfDiagnostic diagnostic with
                | Some m -> Some(outputWhere (showFullPaths, diagnosticStyle) m)
                | None -> None

            let OutputCanonicalInformation (subcategory, errorNumber) : FormattedDiagnosticCanonicalInformation =
                let message =
                    match severity with
                    | FSharpDiagnosticSeverity.Error -> "error"
                    | FSharpDiagnosticSeverity.Warning -> "warning"
                    | FSharpDiagnosticSeverity.Info
                    | FSharpDiagnosticSeverity.Hidden -> "info"

                let text =
                    match diagnosticStyle with
                    // Show the subcategory for --vserrors so that we can fish it out in Visual Studio and use it to determine error stickiness.
                    | DiagnosticStyle.VisualStudio -> sprintf "%s %s FS%04d: " subcategory message errorNumber
                    | _ -> sprintf "%s FS%04d: " message errorNumber

                {
                    ErrorNumber = errorNumber
                    Subcategory = subcategory
                    TextRepresentation = text
                }

            let mainError, relatedErrors = SplitRelatedDiagnostics diagnostic
            let where = OutputWhere mainError

            let canonical =
                OutputCanonicalInformation(diagnostic.Subcategory(), GetDiagnosticNumber mainError)

            let message =
                let os = StringBuilder()
                OutputPhasedDiagnostic os mainError flattenErrors suggestNames
                os.ToString()

            let entry: FormattedDiagnosticDetailedInfo =
                {
                    Location = where
                    Canonical = canonical
                    Message = message
                }

            errors.Add(FormattedDiagnostic.Long(severity, entry))

            let OutputRelatedError (diagnostic: PhasedDiagnostic) =
                match diagnosticStyle with
                // Give a canonical string when --vserror.
                | DiagnosticStyle.VisualStudio ->
                    let relWhere = OutputWhere mainError // mainError?

                    let relCanonical =
                        OutputCanonicalInformation(diagnostic.Subcategory(), GetDiagnosticNumber mainError) // Use main error for code

                    let relMessage =
                        let os = StringBuilder()
                        OutputPhasedDiagnostic os diagnostic flattenErrors suggestNames
                        os.ToString()

                    let entry: FormattedDiagnosticDetailedInfo =
                        {
                            Location = relWhere
                            Canonical = relCanonical
                            Message = relMessage
                        }

                    errors.Add(FormattedDiagnostic.Long(severity, entry))

                | _ ->
                    let os = StringBuilder()
                    OutputPhasedDiagnostic os diagnostic flattenErrors suggestNames
                    errors.Add(FormattedDiagnostic.Short(severity, os.ToString()))

            relatedErrors |> List.iter OutputRelatedError

        match diagnostic with
#if !NO_TYPEPROVIDERS
        | {
              Exception = :? TypeProviderError as tpe
          } ->
            tpe.Iter(fun exn ->
                let newErr = { diagnostic with Exception = exn }
                report newErr)
#endif
        | x -> report x

        errors.ToArray()

/// used by fsc.exe and fsi.exe, but not by VS
/// prints error and related errors to the specified StringBuilder
let rec OutputDiagnostic (implicitIncludeDir, showFullPaths, flattenErrors, diagnosticStyle, severity) os (diagnostic: PhasedDiagnostic) =

    // 'true' for "canSuggestNames" is passed last here because we want to report suggestions in fsc.exe and fsi.exe, just not in regular IDE usage.
    let errors =
        CollectFormattedDiagnostics(implicitIncludeDir, showFullPaths, flattenErrors, diagnosticStyle, severity, diagnostic, true)

    for e in errors do
        Printf.bprintf os "\n"

        match e with
        | FormattedDiagnostic.Short (_, txt) -> os.AppendString txt |> ignore
        | FormattedDiagnostic.Long (_, details) ->
            match details.Location with
            | Some l when not l.IsEmpty -> os.AppendString l.TextRepresentation
            | _ -> ()

            os.AppendString details.Canonical.TextRepresentation
            os.AppendString details.Message

let OutputDiagnosticContext prefix fileLineFunction os diagnostic =
    match GetRangeOfDiagnostic diagnostic with
    | None -> ()
    | Some m ->
        let fileName = m.FileName
        let lineA = m.StartLine
        let lineB = m.EndLine
        let line = fileLineFunction fileName lineA

        if line <> "" then
            let iA = m.StartColumn
            let iB = m.EndColumn
            let iLen = if lineA = lineB then max (iB - iA) 1 else 1
            Printf.bprintf os "%s%s\n" prefix line
            Printf.bprintf os "%s%s%s\n" prefix (String.make iA '-') (String.make iLen '^')

let ReportDiagnosticAsInfo options (diagnostic, severity) =
    match severity with
    | FSharpDiagnosticSeverity.Error -> false
    | FSharpDiagnosticSeverity.Warning -> false
    | FSharpDiagnosticSeverity.Info ->
        let n = GetDiagnosticNumber diagnostic

        IsWarningOrInfoEnabled (diagnostic, severity) n options.WarnLevel options.WarnOn
        && not (List.contains n options.WarnOff)
    | FSharpDiagnosticSeverity.Hidden -> false

let ReportDiagnosticAsWarning options (diagnostic, severity) =
    match severity with
    | FSharpDiagnosticSeverity.Error -> false
    | FSharpDiagnosticSeverity.Warning ->
        let n = GetDiagnosticNumber diagnostic

        IsWarningOrInfoEnabled (diagnostic, severity) n options.WarnLevel options.WarnOn
        && not (List.contains n options.WarnOff)
    // Informational become warning if explicitly on and not explicitly off
    | FSharpDiagnosticSeverity.Info ->
        let n = GetDiagnosticNumber diagnostic
        List.contains n options.WarnOn && not (List.contains n options.WarnOff)
    | FSharpDiagnosticSeverity.Hidden -> false

let ReportDiagnosticAsError options (diagnostic, severity) =
    match severity with
    | FSharpDiagnosticSeverity.Error -> true
    // Warnings become errors in some situations
    | FSharpDiagnosticSeverity.Warning ->
        let n = GetDiagnosticNumber diagnostic

        IsWarningOrInfoEnabled (diagnostic, severity) n options.WarnLevel options.WarnOn
        && not (List.contains n options.WarnAsWarn)
        && ((options.GlobalWarnAsError && not (List.contains n options.WarnOff))
            || List.contains n options.WarnAsError)
    // Informational become errors if explicitly WarnAsError
    | FSharpDiagnosticSeverity.Info ->
        let n = GetDiagnosticNumber diagnostic
        List.contains n options.WarnAsError
    | FSharpDiagnosticSeverity.Hidden -> false

//----------------------------------------------------------------------------
// Scoped #nowarn pragmas

/// Build an DiagnosticsLogger that delegates to another DiagnosticsLogger but filters warnings turned off by the given pragma declarations
//
// NOTE: we allow a flag to turn of strict file checking. This is because file names sometimes don't match due to use of
// #line directives, e.g. for pars.fs/pars.fsy. In this case we just test by line number - in most cases this is sufficient
// because we install a filtering error handler on a file-by-file basis for parsing and type-checking.
// However this is indicative of a more systematic problem where source-line
// sensitive operations (lexfilter and warning filtering) do not always
// interact well with #line directives.
type DiagnosticsLoggerFilteringByScopedPragmas
    (
        checkFile,
        scopedPragmas,
        diagnosticOptions: FSharpDiagnosticOptions,
        diagnosticsLogger: DiagnosticsLogger
    ) =
    inherit DiagnosticsLogger("DiagnosticsLoggerFilteringByScopedPragmas")

    override _.DiagnosticSink(diagnostic, severity) =
        if severity = FSharpDiagnosticSeverity.Error then
            diagnosticsLogger.DiagnosticSink(diagnostic, severity)
        else
            let report =
                let warningNum = GetDiagnosticNumber diagnostic

                match GetRangeOfDiagnostic diagnostic with
                | Some m ->
                    scopedPragmas
                    |> List.exists (fun pragma ->
                        let (ScopedPragma.WarningOff (pragmaRange, warningNumFromPragma)) = pragma

                        warningNum = warningNumFromPragma
                        && (not checkFile || m.FileIndex = pragmaRange.FileIndex)
                        && posGeq m.Start pragmaRange.Start)
                    |> not
                | None -> true

            if report then
                if ReportDiagnosticAsError diagnosticOptions (diagnostic, severity) then
                    diagnosticsLogger.DiagnosticSink(diagnostic, FSharpDiagnosticSeverity.Error)
                elif ReportDiagnosticAsWarning diagnosticOptions (diagnostic, severity) then
                    diagnosticsLogger.DiagnosticSink(diagnostic, FSharpDiagnosticSeverity.Warning)
                elif ReportDiagnosticAsInfo diagnosticOptions (diagnostic, severity) then
                    diagnosticsLogger.DiagnosticSink(diagnostic, severity)

    override _.ErrorCount = diagnosticsLogger.ErrorCount

let GetDiagnosticsLoggerFilteringByScopedPragmas (checkFile, scopedPragmas, diagnosticOptions, diagnosticsLogger) =
    DiagnosticsLoggerFilteringByScopedPragmas(checkFile, scopedPragmas, diagnosticOptions, diagnosticsLogger) :> DiagnosticsLogger
