// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Contains logic to prepare, post-process, filter and emit compiler diagnostics
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
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CheckIncrementalClasses
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
let showAssertForUnexpectedException = ref true
#endif

exception HashIncludeNotAllowedInNonScript of range

exception HashReferenceNotAllowedInNonScript of range

exception HashLoadedSourceHasIssues of informationals: exn list * warnings: exn list * errors: exn list * range

exception HashLoadedScriptConsideredSource of range

exception HashDirectiveNotAllowedInNonScript of range

exception DeprecatedCommandLineOptionFull of string * range

exception DeprecatedCommandLineOptionForHtmlDoc of string * range

exception DeprecatedCommandLineOptionSuggestAlternative of string * string * range

exception DeprecatedCommandLineOptionNoDescription of string * range

exception InternalCommandLineOption of string * range

type Exception with

    member exn.DiagnosticRange =
        match exn with
        | DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer(range = m) -> Some m
        | ArgumentsInSigAndImplMismatch(_, implArg) -> Some implArg.idRange
        | ErrorFromAddingConstraint(_, exn2, _) -> exn2.DiagnosticRange
#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange exn -> exn.DiagnosticRange
        | TypeProviders.ProvidedTypeResolution(m, _)
#endif
        | ReservedKeyword(_, m)
        | IndentationProblem(_, m)
        | ErrorFromAddingTypeEquation(_, _, _, _, _, m)
        | ErrorFromApplyingDefault(_, _, _, _, _, m)
        | ErrorsFromAddingSubsumptionConstraint(_, _, _, _, _, _, m)
        | FunctionExpected(_, _, m)
        | BakedInMemberConstraintName(_, m)
        | StandardOperatorRedefinitionWarning(_, m)
        | BadEventTransformation m
        | ParameterlessStructCtor m
        | FieldNotMutable(_, _, m)
        | Recursion(_, _, _, _, m)
        | InvalidRuntimeCoercion(_, _, _, m)
        | IndeterminateRuntimeCoercion(_, _, _, m)
        | IndeterminateStaticCoercion(_, _, _, m)
        | StaticCoercionShouldUseBox(_, _, _, m)
        | CoercionTargetSealed(_, _, m)
        | UpcastUnnecessary m
        | QuotationTranslator.IgnoringPartOfQuotedTermWarning(_, m)

        | TypeTestUnnecessary m
        | RuntimeCoercionSourceSealed(_, _, m)
        | OverrideDoesntOverride(_, _, _, _, _, m)
        | UnionPatternsBindDifferentNames m
        | UnionCaseWrongArguments(_, _, _, m)
        | TypeIsImplicitlyAbstract m
        | RequiredButNotSpecified(_, _, _, _, m)
        | FunctionValueUnexpected(_, _, m)
        | UnitTypeExpected(_, _, m)
        | UnitTypeExpectedWithEquality(_, _, m)
        | UnitTypeExpectedWithPossiblePropertySetter(_, _, _, _, m)
        | UnitTypeExpectedWithPossibleAssignment(_, _, _, _, m)
        | UseOfAddressOfOperator m
        | DeprecatedThreadStaticBindingWarning m
        | NonUniqueInferredAbstractSlot(_, _, _, _, _, m)
        | DefensiveCopyWarning(_, m)
        | LetRecCheckedAtRuntime m
        | UpperCaseIdentifierInPattern m
        | NotUpperCaseConstructor m
        | NotUpperCaseConstructorWithoutRQA m
        | RecursiveUseCheckedAtRuntime(_, _, m)
        | LetRecEvaluatedOutOfOrder(_, _, _, m)
        | DiagnosticWithText(_, _, m)
        | DiagnosticWithSuggestions(_, _, m, _, _)
        | DiagnosticEnabledWithLanguageFeature(_, _, m, _)
        | SyntaxError(_, m)
        | InternalError(_, m)
        | InternalException(_, _, m)
        | InterfaceNotRevealed(_, _, m)
        | WrappedError(_, m)
        | PatternMatchCompilation.MatchIncomplete(_, _, m)
        | PatternMatchCompilation.MatchIncompleteForLoopHint(PatternMatchCompilation.MatchIncomplete(_, _, m))
        | PatternMatchCompilation.EnumMatchIncomplete(_, _, m)
        | PatternMatchCompilation.RuleNeverMatched m
        | ValNotMutable(_, _, m)
        | ValNotLocal(_, _, m)
        | MissingFields(_, m)
        | OverrideInIntrinsicAugmentation m
        | IntfImplInIntrinsicAugmentation m
        | OverrideInExtrinsicAugmentation m
        | IntfImplInExtrinsicAugmentation m
        | ValueRestriction(_, _, _, _, m)
        | LetRecUnsound(_, _, m)
        | ObsoleteDiagnostic(_, _, _, _, m)
        | Experimental(range = m)
        | PossibleUnverifiableCode m
        | UserCompilerMessage(_, _, m)
        | Deprecated(_, m)
        | LibraryUseOnly m
        | FieldsFromDifferentTypes(_, _, _, m)
        | IndeterminateType m
        | InvalidAttributeTargetForLanguageElement(_, _, m)
        | TyconBadArgs(_, _, _, m) -> Some m

        | FieldNotContained(_, _, _, _, _, arf, _, _) -> Some arf.Range
        | ValueNotContained(_, _, _, _, aval, _, _) -> Some aval.Range
        | UnionCaseNotContained(_, _, _, aval, _, _) -> Some aval.Id.idRange
        | FSharpExceptionNotContained(_, _, aexnc, _, _) -> Some aexnc.Range

        | VarBoundTwice id
        | UndefinedName(_, _, id, _) -> Some id.idRange

        | Duplicate(_, _, m)
        | NameClash(_, _, _, m, _, _, _)
        | UnresolvedOverloading(_, _, _, m)
        | UnresolvedConversionOperator(_, _, _, m)
        | VirtualAugmentationOnNullValuedType m
        | NonVirtualAugmentationOnNullValuedType m
        | NonRigidTypar(_, _, _, _, _, m)
        | ConstraintSolverTupleDiffLengths(_, _, _, _, m, _)
        | ConstraintSolverInfiniteTypes(_, _, _, _, m, _)
        | ConstraintSolverMissingConstraint(_, _, _, m, _)
        | ConstraintSolverNullnessWarningEquivWithTypes(_, _, _, _, _, m, _)
        | ConstraintSolverNullnessWarningWithTypes(_, _, _, _, _, m, _)
        | ConstraintSolverNullnessWarningWithType(_, _, _, m, _)
        | ConstraintSolverNullnessWarningOnDotAccess(_, _, _, _, m, _)
        | ConstraintSolverNullnessWarning(_, m, _)
        | ConstraintSolverTypesNotInEqualityRelation(_, _, _, m, _, _)
        | ConstraintSolverError(_, m, _)
        | ConstraintSolverTypesNotInSubsumptionRelation(_, _, _, m, _)
        | SelfRefObjCtor(_, m) -> Some m

        | NotAFunction(_, _, mfun, _) -> Some mfun

        | NotAFunctionButIndexer(_, _, _, mfun, _) -> Some mfun

        | IllegalFileNameChar _ -> Some rangeCmdArgs

        | UnresolvedReferenceError(_, m)
        | UnresolvedPathReference(_, _, m)
        | DeprecatedCommandLineOptionFull(_, m)
        | DeprecatedCommandLineOptionForHtmlDoc(_, m)
        | DeprecatedCommandLineOptionSuggestAlternative(_, _, m)
        | DeprecatedCommandLineOptionNoDescription(_, m)
        | InternalCommandLineOption(_, m)
        | HashIncludeNotAllowedInNonScript m
        | HashReferenceNotAllowedInNonScript m
        | HashDirectiveNotAllowedInNonScript m
        | FileNameNotResolved(_, _, m)
        | LoadedSourceNotFoundIgnoring(_, m)
        | MSBuildReferenceResolutionWarning(_, _, m)
        | MSBuildReferenceResolutionError(_, _, m)
        | AssemblyNotResolved(_, m)
        | HashLoadedSourceHasIssues(_, _, _, m)
        | HashLoadedScriptConsideredSource m
        | NoConstructorsAvailableForType(_, _, m) -> Some m

        // Strip TargetInvocationException wrappers
        | :? TargetInvocationException as e when isNotNull e.InnerException -> (!!e.InnerException).DiagnosticRange
#if !NO_TYPEPROVIDERS
        | :? TypeProviderError as e -> e.Range |> Some
#endif
        | _ -> None

    member exn.DiagnosticNumber =
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
        | PatternMatchCompilation.MatchIncompleteForLoopHint _ -> 25
        | PatternMatchCompilation.RuleNeverMatched _ -> 26

        | ValNotMutable _ -> 27
        | ValNotLocal _ -> 28
        | MissingFields _ -> 29
        | ValueRestriction _ -> 30
        | LetRecUnsound _ -> 31
        | FieldsFromDifferentTypes _ -> 32
        | TyconBadArgs _ -> 33
        | FieldNotContained(kind = TypeMismatchSource.NullnessOnlyMismatch) -> 3261
        | ValueNotContained(kind = TypeMismatchSource.NullnessOnlyMismatch) -> 3261
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
        | ObsoleteDiagnostic(isError = false) -> 44
        | ReservedKeyword _ -> 46
        | SelfRefObjCtor _ -> 47
        | VirtualAugmentationOnNullValuedType _ -> 48
        | UpperCaseIdentifierInPattern _ -> 49
        | InterfaceNotRevealed _ -> 50
        | UseOfAddressOfOperator _ -> 51
        | DefensiveCopyWarning _ -> 52
        | NotUpperCaseConstructor _ -> 53
        | NotUpperCaseConstructorWithoutRQA _ -> 53
        | TypeIsImplicitlyAbstract _ -> 54
        // 55 cannot be reused
        | DeprecatedThreadStaticBindingWarning _ -> 56
        | Experimental _ -> 57
        | IndentationProblem _ -> 58
        | CoercionTargetSealed _ -> 59
        | OverrideInIntrinsicAugmentation _ -> 60
        | NonVirtualAugmentationOnNullValuedType _ -> 61
        | UserCompilerMessage(_, n, _) -> n
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
        | ObsoleteDiagnostic(isError = true) -> 101
#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange _
        | TypeProviders.ProvidedTypeResolution _ -> 103
#endif
        | PatternMatchCompilation.EnumMatchIncomplete _ -> 104
        | Failure _ -> 192
        | DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer _ -> 318
        | NoConstructorsAvailableForType _ -> 1133
        | ArgumentsInSigAndImplMismatch _ -> 3218

        // Strip TargetInvocationException wrappers
        | :? TargetInvocationException as e when isNotNull e.InnerException -> (!!e.InnerException).DiagnosticNumber
        | WrappedError(e, _) -> e.DiagnosticNumber
        | DiagnosticWithText(n, _, _) -> n
        | DiagnosticWithSuggestions(n, _, _, _, _) -> n
        | DiagnosticEnabledWithLanguageFeature(n, _, _, _) -> n
        | IllegalFileNameChar(fileName, invalidChar) -> fst (FSComp.SR.buildUnexpectedFileNameCharacter (fileName, string invalidChar))
#if !NO_TYPEPROVIDERS
        | :? TypeProviderError as e -> e.Number
#endif
        | ErrorsFromAddingSubsumptionConstraint(_, _, _, _, _, ContextInfo.DowncastUsedInsteadOfUpcast _, _) ->
            fst (FSComp.SR.considerUpcast ("", ""))
        | ConstraintSolverNullnessWarningEquivWithTypes _ -> 3261
        | ConstraintSolverNullnessWarningWithTypes _ -> 3261
        | ConstraintSolverNullnessWarningWithType _ -> 3261
        | ConstraintSolverNullnessWarningOnDotAccess _ -> 3261
        | ConstraintSolverNullnessWarning _ -> 3261
        | InvalidAttributeTargetForLanguageElement _ -> 842
        | _ -> 193

type PhasedDiagnostic with

    member x.Range = x.Exception.DiagnosticRange

    member x.Number = x.Exception.DiagnosticNumber

    member x.WarningLevel =
        match x.Exception with
        // Level 5 warnings
        | RecursiveUseCheckedAtRuntime _
        | LetRecEvaluatedOutOfOrder _
        | DefensiveCopyWarning _ -> 5

        | DiagnosticWithText(n, _, _)
        | DiagnosticEnabledWithLanguageFeature(n, _, _, _)
        | DiagnosticWithSuggestions(n, _, _, _, _) ->
            // 1178, tcNoComparisonNeeded1, "The struct, record or union type '%s' is not structurally comparable because the type parameter %s does not satisfy the 'comparison' constraint..."
            // 1178, tcNoComparisonNeeded2, "The struct, record or union type '%s' is not structurally comparable because the type '%s' does not satisfy the 'comparison' constraint...."
            // 1178, tcNoEqualityNeeded1, "The struct, record or union type '%s' does not support structural equality because the type parameter %s does not satisfy the 'equality' constraint..."
            // 1178, tcNoEqualityNeeded2, "The struct, record or union type '%s' does not support structural equality because the type '%s' does not satisfy the 'equality' constraint...."
            if (n = 1178) then 5 else 2
        // Level 2
        | _ -> 2

    member private x.IsEnabled(severity, options) =
        let level = options.WarnLevel
        let specificWarnOn = options.WarnOn
        let n = x.Number

        List.contains n specificWarnOn
        ||
        // Some specific warnings/informational are never on by default, i.e. unused variable warnings
        match n with
        | 1182 -> false // chkUnusedValue - off by default
        | 3180 -> false // abImplicitHeapAllocation - off by default
        | 3186 -> false // pickleMissingDefinition - off by default
        | 3366 -> false // tcIndexNotationDeprecated - currently off by default
        | 3517 -> false // optFailedToInlineSuggestedValue - off by default
        | 3388 -> false // tcSubsumptionImplicitConversionUsed - off by default
        | 3389 -> false // tcBuiltInImplicitConversionUsed - off by default
        | 3390 -> false // xmlDocBadlyFormed - off by default
        | 3395 -> false // tcImplicitConversionUsedForMethodArg - off by default
        | 3559 -> false // typrelNeverRefinedAwayFromTop - off by default
        | 3560 -> false // tcCopyAndUpdateRecordChangesAllFields - off by default
        | 3575 -> false // tcMoreConcreteTiebreakerUsed - off by default
        | 3576 -> false // tcGenericOverloadBypassed - off by default
        | 3579 -> false // alwaysUseTypedStringInterpolation - off by default
        | 3582 -> false // infoIfFunctionShadowsUnionCase - off by default
        | 3570 -> false // tcAmbiguousDiscardDotLambda - off by default
        | 3878 -> false // tcAttributeIsNotValidForUnionCaseWithFields - off by default
        | 3905 -> false // tcRecordTypeDefinitionSpreadFieldShadowsSpreadField - off by default
        | 3906 -> false // tcRecordExplicitFieldShadowsSpreadField - off by default
        | 3907 -> false // tcRecordExprSpreadFieldShadowsSpreadField - off by default
        | _ ->
            match x.Exception with
            | DiagnosticEnabledWithLanguageFeature(_, _, _, enabled) -> enabled
            | _ ->
                (severity = FSharpDiagnosticSeverity.Info && level > 0)
                || (severity = FSharpDiagnosticSeverity.Warning && level >= x.WarningLevel)

    member x.AdjustSeverity(options) =
        let severity = x.Severity
        let n = x.Number

        let localWarnon () = WarnScopes.IsWarnon options n x.Range

        let localNowarn () = WarnScopes.IsNowarn options n x.Range

        let warnOff () =
            List.contains n options.WarnOff && not (localWarnon ()) || localNowarn ()

        match severity with
        | FSharpDiagnosticSeverity.Error -> FSharpDiagnosticSeverity.Error
        | FSharpDiagnosticSeverity.Warning when
            x.IsEnabled(severity, options)
            && ((options.GlobalWarnAsError && not (warnOff ()))
                || List.contains n options.WarnAsError && not (localNowarn ()))
            && not (List.contains n options.WarnAsWarn)
            ->
            FSharpDiagnosticSeverity.Error
        | FSharpDiagnosticSeverity.Info when List.contains n options.WarnAsError && not (localNowarn ()) -> FSharpDiagnosticSeverity.Error
        | FSharpDiagnosticSeverity.Warning when x.IsEnabled(severity, options) && not (warnOff ()) -> FSharpDiagnosticSeverity.Warning
        | FSharpDiagnosticSeverity.Warning when localWarnon () -> FSharpDiagnosticSeverity.Warning
        | FSharpDiagnosticSeverity.Info when List.contains n options.WarnOn && not (warnOff ()) -> FSharpDiagnosticSeverity.Warning
        | FSharpDiagnosticSeverity.Info when x.IsEnabled(severity, options) && not (warnOff ()) -> FSharpDiagnosticSeverity.Info
        | _ -> FSharpDiagnosticSeverity.Hidden

[<AutoOpen>]
module OldStyleMessages =
    let Message (name, format) = DeclareResourceString(name, format)

    do FSComp.SR.RunStartupValidation()
    let SeeAlsoE () = Message("SeeAlso", "%s")
    let ConstraintSolverTupleDiffLengthsE () = Message("ConstraintSolverTupleDiffLengths", "%d%d")
    let ConstraintSolverInfiniteTypesE () = Message("ConstraintSolverInfiniteTypes", "%s%s")
    let ConstraintSolverMissingConstraintE () = Message("ConstraintSolverMissingConstraint", "%s")
    let ConstraintSolverNullnessWarningEquivWithTypesE () = Message("ConstraintSolverNullnessWarningEquivWithTypes", "%s")
    let ConstraintSolverNullnessWarningWithTypesE () = Message("ConstraintSolverNullnessWarningWithTypes", "%s%s")
    let ConstraintSolverNullnessWarningWithTypeE () = Message("ConstraintSolverNullnessWarningWithType", "%s")
    let ConstraintSolverNullnessWarningOnDotAccessE () = Message("ConstraintSolverNullnessWarningOnDotAccess", "%s%s")

    let ConstraintSolverNullnessWarningOnDotAccessWithBindingE () =
        Message("ConstraintSolverNullnessWarningOnDotAccessWithBinding", "%s%s%s")

    let ConstraintSolverNullnessWarningE () = Message("ConstraintSolverNullnessWarning", "%s")
    let ConstraintSolverTypesNotInEqualityRelation1E () = Message("ConstraintSolverTypesNotInEqualityRelation1", "%s%s")
    let ConstraintSolverTypesNotInEqualityRelation2E () = Message("ConstraintSolverTypesNotInEqualityRelation2", "%s%s")
    let ConstraintSolverTypesNotInSubsumptionRelationE () = Message("ConstraintSolverTypesNotInSubsumptionRelation", "%s%s%s")
    let ErrorFromAddingTypeEquation1E () = Message("ErrorFromAddingTypeEquation1", "%s%s%s")
    let ErrorFromAddingTypeEquation1TupleE () = Message("ErrorFromAddingTypeEquation1Tuple", "%s%s%s")
    let ErrorFromAddingTypeEquation2E () = Message("ErrorFromAddingTypeEquation2", "%s%s%s")
    let ErrorFromAddingTypeEquation2TupleE () = Message("ErrorFromAddingTypeEquation2Tuple", "%s%s%s")
    let ErrorFromAddingTypeEquationTuplesE () = Message("ErrorFromAddingTypeEquationTuples", "%d%s%d%s%s")
    let ErrorFromApplyingDefault1E () = Message("ErrorFromApplyingDefault1", "%s")
    let ErrorFromApplyingDefault2E () = Message("ErrorFromApplyingDefault2", "")
    let ErrorsFromAddingSubsumptionConstraintE () = Message("ErrorsFromAddingSubsumptionConstraint", "%s%s%s")
    let UpperCaseIdentifierInPatternE () = Message("UpperCaseIdentifierInPattern", "")
    let NotUpperCaseConstructorE () = Message("NotUpperCaseConstructor", "")
    let NotUpperCaseConstructorWithoutRQAE () = Message("NotUpperCaseConstructorWithoutRQA", "")
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
    let NONTERM_classMemberSpfnGetSetElementsE () = Message("NONTERM.classMemberSpfnGetSetElements", "")
    let NONTERM_autoPropsDefnDeclE () = Message("NONTERM.autoPropsDefnDecl", "")
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
    let OverrideShouldBeStatic () = Message("OverrideShouldBeStatic", "")
    let OverrideShouldBeInstance () = Message("OverrideShouldBeInstance", "")
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
    let MatchIncompleteForLoopE () = Message("MatchIncompleteForLoop", "")
    let RuleNeverMatchedE () = Message("RuleNeverMatched", "")
    let EnumMatchIncomplete1E () = Message("EnumMatchIncomplete1", "")
    let ValNotMutableE () = Message("ValNotMutable", "%s")
    let ValNotMutableParameterE () = Message("ValNotMutableParameter", "%s%s%s")
    let ValNotLocalE () = Message("ValNotLocal", "")
    let Obsolete1E () = Message("Obsolete1", "")
    let Obsolete2E () = Message("Obsolete2", "%s")
    let Experimental1E () = Message("Experimental1", "")
    let Experimental2E () = Message("Experimental2", "%s")
    let Experimental3E () = Message("Experimental3", "")
    let PossibleUnverifiableCodeE () = Message("PossibleUnverifiableCode", "")
    let DeprecatedE () = Message("Deprecated", "%s")
    let LibraryUseOnlyE () = Message("LibraryUseOnly", "")
    let MissingFieldsE () = Message("MissingFields", "%s")
    let ValueRestrictionFunctionE () = Message("ValueRestrictionFunction", "%s%s%s")
    let ValueRestrictionE () = Message("ValueRestriction", "%s%s%s")
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
    let ArgumentsInSigAndImplMismatchE () = Message("ArgumentsInSigAndImplMismatch", "%s%s")

    let DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferE () =
        Message("DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer", "%s%s%s%s")

    let InvalidAttributeTargetForLanguageElement1E () = Message("InvalidAttributeTargetForLanguageElement1", "%s%s")
    let InvalidAttributeTargetForLanguageElement2E () = Message("InvalidAttributeTargetForLanguageElement2", "")

    let NoConstructorsAvailableForTypeE () = Message("NoConstructorsAvailableForType", "%s")

#if DEBUG
let mutable showParserStackOnParseError = false
#endif

[<return: Struct>]
let (|InvalidArgument|_|) (exn: exn) =
    match exn with
    | :? ArgumentException as e -> ValueSome e.Message
    | _ -> ValueNone

/// Classifies a name that failed to resolve. It stands for nothing, so it is not an entity of unknown
/// kind but a name of its own kind.
let richTextOfUnresolvedName name =
    RichText.mkUnresolvedName (ConvertValLogicalNameToDisplayNameCore name)

/// Classifies a name that does resolve but whose kind is not known here, e.g. one offered as a
/// suggestion in place of a name that did not resolve
let richTextOfNameOfUnknownKind name =
    RichText.mkUnknownEntity (ConvertValLogicalNameToDisplayNameCore name)

let OutputNameSuggestions (os: RichTextBuilder) suggestNames suggestionsF idText =
    if suggestNames then
        let buffer = DiagnosticResolutionHints.SuggestionBuffer idText

        if not buffer.Disabled then
            suggestionsF buffer.Add

            if not buffer.IsEmpty then
                os.Append " "
                os.Append(FSComp.SR.undefinedNameSuggestionsIntro ())

                for value in buffer do
                    os.Append(RichText.mkLineBreak Environment.NewLine)
                    os.Append "   "
                    os.Append(richTextOfNameOfUnknownKind value)

let OutputTypesNotInEqualityRelationContextInfo contextInfo (ty1: RichText) (ty2: RichText) m (os: RichTextBuilder) fallback =
    match contextInfo with
    | ContextInfo.IfExpression range when equals range m -> os.Append(FSComp.SR.ifExpression (ty1, ty2))
    | ContextInfo.CollectionElement(isArray, range) when equals range m ->
        if isArray then
            os.Append(FSComp.SR.arrayElementHasWrongType (ty1, ty2))
        else
            os.Append(FSComp.SR.listElementHasWrongType (ty1, ty2))
    | ContextInfo.OmittedElseBranch range when equals range m -> os.Append(FSComp.SR.missingElseBranch (ty2))
    | ContextInfo.ElseBranchResult range when equals range m -> os.Append(FSComp.SR.elseBranchHasWrongType (ty1, ty2))
    | ContextInfo.FollowingPatternMatchClause range when equals range m ->
        os.Append(FSComp.SR.followingPatternMatchClauseHasWrongType (ty1, ty2))
    | ContextInfo.PatternMatchGuard range when equals range m -> os.Append(FSComp.SR.patternMatchGuardIsNotBool (ty2))
    | contextInfo -> fallback contextInfo

type Exception with

    member exn.Output(os: RichTextBuilder, suggestNames) =

        let typeEquationMessage g ty2 normalE tupleE = if isAnyTupleTy g ty2 then tupleE else normalE

        match exn with
        // TODO: this is now unused...?
        | ConstraintSolverTupleDiffLengths(_, _, tl1, tl2, m, m2) ->
            os.Append(ConstraintSolverTupleDiffLengthsE().Format tl1.Length tl2.Length)

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverInfiniteTypes(denv, contextInfo, ty1, ty2, m, m2) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(ConstraintSolverInfiniteTypesE(), ty1, ty2)

            match contextInfo with
            | ContextInfo.ReturnInComputationExpression -> os.Append(" " + FSComp.SR.returnUsedInsteadOfReturnBang ())
            | ContextInfo.YieldInComputationExpression -> os.Append(" " + FSComp.SR.yieldUsedInsteadOfYieldBang ())
            | _ -> ()

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverNullnessWarningEquivWithTypes(denv, ty1, ty2, _nullness1, _nullness2, m, m2) ->

            // Turn on nullness annotations for messages about nullness
            let denv =
                { denv with
                    showNullnessAnnotations = Some true
                }

            let t1, _t2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

            os.Append(ConstraintSolverNullnessWarningEquivWithTypesE(), t1)

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverNullnessWarningWithTypes(denv, ty1, ty2, _nullness1, _nullness2, m, m2) ->

            // Turn on nullness annotations for messages about nullness
            let denv =
                { denv with
                    showNullnessAnnotations = Some true
                }

            let t1, t2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

            os.Append(ConstraintSolverNullnessWarningWithTypesE(), t1, t2)

            if m.StartLine <> m2.StartLine || m.EndLine <> m2.EndLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverNullnessWarningWithType(denv, ty, _, m, m2) ->

            // Turn on nullness annotations for messages about nullness
            let denv =
                { denv with
                    showNullnessAnnotations = Some true
                }

            os.Append(ConstraintSolverNullnessWarningWithTypeE(), NicePrint.minimalRichTextOfType denv ty)

            if m.StartLine <> m2.StartLine || m.EndLine <> m2.EndLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverNullnessWarningOnDotAccess(denv, objTy, memberName, bindingName, m, m2) ->
            let tyText = NicePrint.minimalRichTextOfTypeWithNullness denv objTy

            match bindingName with
            | Some name ->
                os.Append(
                    ConstraintSolverNullnessWarningOnDotAccessWithBindingE(),
                    RichText.mkMember memberName,
                    RichText.mkLocal name,
                    tyText
                )
            | None -> os.Append(ConstraintSolverNullnessWarningOnDotAccessE(), RichText.mkMember memberName, tyText)

            if m.StartLine <> m2.StartLine || m.EndLine <> m2.EndLine then
                os.Append(SeeAlsoE().Format(stringOfRange m2))
            else
                os.Append(".")

        | ConstraintSolverNullnessWarning(msg, m, m2) ->
            os.Append(ConstraintSolverNullnessWarningE(), msg)

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m2))

        | ConstraintSolverMissingConstraint(denv, tpr, tpc, m, m2) ->
            os.Append(ConstraintSolverMissingConstraintE(), NicePrint.richTextOfTyparConstraint denv (tpr, tpc))

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverTypesNotInEqualityRelation(denv, ty1, ty2, m, m2, contextInfo) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1Text, ty2Text, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

            match ty1, ty2 with
            | TType_measure _, TType_measure _ -> os.Append(ConstraintSolverTypesNotInEqualityRelation1E(), ty1Text, ty2Text)
            | _ ->
                OutputTypesNotInEqualityRelationContextInfo contextInfo ty1Text ty2Text m os (fun _ ->
                    os.Append(ConstraintSolverTypesNotInEqualityRelation2E(), ty1Text, ty2Text))

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m))

        | ConstraintSolverTypesNotInSubsumptionRelation(denv, ty1, ty2, m, m2) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(ConstraintSolverTypesNotInSubsumptionRelationE(), ty2, ty1, cxs)

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m2))

        | ConstraintSolverError(msg, m, m2) ->
            os.Append msg

            if m.StartLine <> m2.StartLine then
                os.Append(SeeAlsoE().Format(stringOfRange m2))

        | ErrorFromAddingTypeEquation(g, denv, ty1, ty2, ConstraintSolverTypesNotInEqualityRelation(_, ty1b, ty2b, m, _, contextInfo), _) when
            typeEquiv g ty1 ty1b && typeEquiv g ty2 ty2b
            ->
            let typeEquation1E =
                typeEquationMessage g ty2 ErrorFromAddingTypeEquation1E ErrorFromAddingTypeEquation1TupleE

            let ty1, ty2, tpcs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

            OutputTypesNotInEqualityRelationContextInfo contextInfo ty1 ty2 m os (fun contextInfo ->
                match contextInfo with
                | ContextInfo.TupleInRecordFields ->
                    os.Append(typeEquation1E (), ty2, ty1, tpcs)
                    os.Append(Environment.NewLine + FSComp.SR.commaInsteadOfSemicolonInRecord ())
                | _ when ty2.Text = "bool" && ty1.Text.EndsWithOrdinal(" ref") ->
                    os.Append(typeEquation1E (), ty2, ty1, tpcs)
                    os.Append(Environment.NewLine + FSComp.SR.derefInsteadOfNot ())
                | _ -> os.Append(typeEquation1E (), ty2, ty1, tpcs))

        | ErrorFromAddingTypeEquation(_, _, _, _, (ConstraintSolverTypesNotInEqualityRelation(_, _, _, _, _, contextInfo) as e), _) when
            (match contextInfo with
             | ContextInfo.NoContext -> false
             | ContextInfo.NullnessCheckOfCapturedArg _ -> false
             | ContextInfo.MemberAccessOnNullable _ -> false
             | _ -> true)
            ->
            e.Output(os, suggestNames)

        | ErrorFromAddingTypeEquation(error = ConstraintSolverTypesNotInSubsumptionRelation _ as e) -> e.Output(os, suggestNames)

        | ErrorFromAddingTypeEquation(error = ConstraintSolverError _ as e) -> e.Output(os, suggestNames)

        | ErrorFromAddingTypeEquation(_g, denv, ty1, ty2, ConstraintSolverTupleDiffLengths(_, contextInfo, tl1, tl2, m1, m2), m) ->
            let ty1, ty2, tpcs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

            let tupleLengthsMessage (message: int * RichText * int * RichText -> RichText) = message (tl1.Length, ty1, tl2.Length, ty2)

            if ty1.Text <> ty2.Text + tpcs.Text then
                match contextInfo with
                | ContextInfo.IfExpression range when equals range m -> os.Append(tupleLengthsMessage FSComp.SR.ifExpressionTuple)
                | ContextInfo.ElseBranchResult range when equals range m ->
                    os.Append(tupleLengthsMessage FSComp.SR.elseBranchHasWrongTypeTuple)
                | ContextInfo.FollowingPatternMatchClause range when equals range m ->
                    os.Append(tupleLengthsMessage FSComp.SR.followingPatternMatchClauseHasWrongTypeTuple)
                | ContextInfo.CollectionElement(isArray, range) when equals range m ->
                    if isArray then
                        os.Append(tupleLengthsMessage FSComp.SR.arrayElementHasWrongTypeTuple)
                    else
                        os.Append(tupleLengthsMessage FSComp.SR.listElementHasWrongTypeTuple)
                | _ ->
                    os.Append(fun rich ->
                        ErrorFromAddingTypeEquationTuplesE().Format tl1.Length (rich ty1) tl2.Length (rich ty2) (rich tpcs))
            else
                os.Append(ConstraintSolverTupleDiffLengthsE().Format tl1.Length tl2.Length)

                if m1.StartLine <> m2.StartLine then
                    os.Append(SeeAlsoE().Format(stringOfRange m1))

        | ErrorFromAddingTypeEquation(g, denv, ty1, ty2, e, _) ->
            let typeEquation2E =
                typeEquationMessage g ty2 ErrorFromAddingTypeEquation2E ErrorFromAddingTypeEquation2TupleE

            let e =
                if not (typeEquiv g ty1 ty2) then
                    let ty1, ty2, tpcs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

                    if ty1.Text <> ty2.Text + tpcs.Text then
                        os.Append(typeEquation2E (), ty1, ty2, tpcs)

                    e

                else
                    // Fix for https://github.com/dotnet/fsharp/issues/18905
                    // If ty1 = ty2 after the type solving, then ty2 holds an actual type.
                    // The order of expected and actual types in ConstraintSolverTypesNotInEqualityRelation can be arbitrary
                    // due to type solving logic.
                    // If ty1 = ty2 = ty2b, it means ty2b is also an actual type, and it needs to be swapped with ty1b
                    // to be correctly used in the type mismatch error message based on ConstraintSolverTypesNotInEqualityRelation
                    match e with
                    | ConstraintSolverTypesNotInEqualityRelation(env, ty1b, ty2b, m, m2, contextInfo) when typeEquiv g ty2 ty2b ->
                        ConstraintSolverTypesNotInEqualityRelation(env, ty2b, ty1b, m, m2, contextInfo)
                    | _ -> e

            e.Output(os, suggestNames)

        | ErrorFromApplyingDefault(_, denv, _, defaultType, e, _) ->
            os.Append(ErrorFromApplyingDefault1E(), NicePrint.minimalRichTextOfType denv defaultType)
            e.Output(os, suggestNames)
            os.Append(ErrorFromApplyingDefault2E().Format)

        | ErrorsFromAddingSubsumptionConstraint(g, denv, ty1, ty2, e, contextInfo, _) ->
            match contextInfo with
            | ContextInfo.DowncastUsedInsteadOfUpcast isOperator ->
                let ty1, ty2, _ = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

                if isOperator then
                    os.Append(snd (FSComp.SR.considerUpcastOperator (ty1, ty2)))
                else
                    os.Append(snd (FSComp.SR.considerUpcast (ty1, ty2)))
            | _ ->
                if not (typeEquiv g ty1 ty2) then
                    let ty1, ty2, tpcs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

                    if ty1.Text <> ty2.Text + tpcs.Text then
                        os.Append(ErrorsFromAddingSubsumptionConstraintE(), ty2, ty1, tpcs)
                    else
                        e.Output(os, suggestNames)
                else
                    e.Output(os, suggestNames)

        | UpperCaseIdentifierInPattern _ -> os.Append(UpperCaseIdentifierInPatternE().Format)

        | NotUpperCaseConstructor _ -> os.Append(NotUpperCaseConstructorE().Format)

        | NotUpperCaseConstructorWithoutRQA _ -> os.Append(NotUpperCaseConstructorWithoutRQAE().Format)

        | ErrorFromAddingConstraint(_, e, _) -> e.Output(os, suggestNames)

#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange e

        | TypeProviders.ProvidedTypeResolution(_, e) -> e.Output(os, suggestNames)

        | :? TypeProviderError as e -> os.Append(e.ContextualErrorRichMessage)
#endif

        | UnresolvedOverloading(denv, callerArgs, failure, m) ->

            let g = denv.g
            // extract eventual information (return type and type parameters)
            // from ConstraintTraitInfo
            let knownReturnType, genericParameterTypes =
                match failure with
                | NoOverloadsFound(cx = Some cx)
                | PossibleCandidates(cx = Some cx) -> Some(cx.GetReturnType(g)), cx.GetCompiledArgumentTypes()
                | _ -> None, []

            // prepare message parts (known arguments, known return type, known generic parameters)
            let argsMessage, returnType, genericParametersMessage =

                let retTy =
                    knownReturnType
                    |> Option.defaultValue (TType.TType_var(Typar.NewUnlinked(), KnownAmbivalentToNull))

                let argRepr =
                    callerArgs.ArgumentNamesAndTypes
                    |> List.map (fun (name, tTy) ->
                        tTy,
                        {
                            ArgReprInfo.Name = name |> Option.map (fun name -> Ident(name, range0))
                            ArgReprInfo.Attribs = WellKnownValAttribs.Empty
                            ArgReprInfo.OtherRange = None
                        })

                let argsL, retTyL, genParamTysL =
                    NicePrint.prettyLayoutsOfUnresolvedOverloading denv argRepr retTy genericParameterTypes

                match callerArgs.ArgumentNamesAndTypes with
                | [] -> None, LayoutRender.toRichText retTyL, LayoutRender.toRichText genParamTysL
                | items ->
                    let args = LayoutRender.toRichText argsL

                    let prefixMessage: RichText -> RichText =
                        match items with
                        | [ _ ] -> FSComp.SR.csNoOverloadsFoundArgumentsPrefixSingular
                        | _ -> FSComp.SR.csNoOverloadsFoundArgumentsPrefixPlural

                    Some(prefixMessage args), LayoutRender.toRichText retTyL, LayoutRender.toRichText genParamTysL

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
                            |> Option.map (fun n -> FSComp.SR.csOverloadCandidateNamedArgumentTypeMismatch (RichText.mkParameter n.idText))
                            |> Option.defaultValue (
                                RichText.mkText (FSComp.SR.csOverloadCandidateIndexedArgumentTypeMismatch ((vsnd x.calledArg.Position) + 1))
                            ) //snd

                        RichText.append (RichText.mkText " // ") nameOrOneBasedIndexMessage
                    | _ -> RichText.empty

                RichText.append (NicePrint.richTextOfMethInfoForOverloadError x.infoReader m displayEnv x.methodSlot.Method) paramInfo

            let nl = Environment.NewLine

            let formatOverloads (overloads: OverloadInformation list) =
                overloads
                |> List.map (overloadMethodInfo denv m)
                |> List.sortBy (fun overload -> overload.Text)
                |> List.map FSComp.SR.formatDashItem
                |> RichText.concatWith (RichText.mkText nl)

            // assemble final message composing the parts
            let msg =
                let optionalParts =
                    let result =
                        [ knownReturnType; genericParametersMessage; argsMessage ]
                        |> List.choose id
                        |> RichText.concatWith (RichText.mkText (nl + nl))

                    if result.IsEmpty then
                        RichText.mkText nl
                    else
                        RichText.concat [ RichText.mkText (nl + nl); result; RichText.mkText (nl + nl) ]

                match failure with
                | NoOverloadsFound(methodName, overloads, _) ->
                    RichText.concat
                        [
                            FSComp.SR.csNoOverloadsFound (RichText.mkMethod methodName)
                            optionalParts
                            FSComp.SR.csAvailableOverloads (formatOverloads overloads)
                        ]
                | PossibleCandidates(methodName, [], _, _) -> FSComp.SR.csMethodIsOverloaded (RichText.mkMethod methodName)
                | PossibleCandidates(methodName, overloads, _, incomparableInfo) ->
                    let baseMessage =
                        RichText.concat
                            [
                                FSComp.SR.csMethodIsOverloaded (RichText.mkMethod methodName)
                                optionalParts
                                FSComp.SR.csCandidates (formatOverloads overloads)
                            ]

                    match incomparableInfo with
                    | Some info ->
                        let formatPositions positions =
                            match positions with
                            | [ p ] -> FSComp.SR.csConcretenessPosition p
                            | _ ->
                                positions
                                |> List.map string
                                |> String.concat ", "
                                |> FSComp.SR.csConcretenessPositions

                        let line1 =
                            FSComp.SR.formatDashItem (
                                FSComp.SR.csConcretenessMoreConcreteAt (info.Method1Signature, formatPositions info.Method1BetterPositions)
                            )

                        let line2 =
                            FSComp.SR.formatDashItem (
                                FSComp.SR.csConcretenessMoreConcreteAt (info.Method2Signature, formatPositions info.Method2BetterPositions)
                            )

                        RichText.concat
                            [
                                baseMessage
                                RichText.mkText nl
                                RichText.mkText (FSComp.SR.csIncomparableConcreteness (line1 + nl + line2))
                            ]
                    | None -> baseMessage

            os.Append msg

        | UnresolvedConversionOperator(denv, fromTy, toTy, _) ->
            let ty1, ty2, _tpcs = NicePrint.minimalRichTextsOfTwoTypes denv fromTy toTy
            os.Append(FSComp.SR.csTypeDoesNotSupportConversion (ty1, ty2))

        | FunctionExpected _ -> os.Append(FunctionExpectedE().Format)

        | BakedInMemberConstraintName(nm, _) -> os.Append(BakedInMemberConstraintNameE(), RichText.mkMember nm)

        | StandardOperatorRedefinitionWarning(msg, _) -> os.Append msg

        | BadEventTransformation _ -> os.Append(BadEventTransformationE().Format)

        | ParameterlessStructCtor _ -> os.Append(ParameterlessStructCtorE().Format)

        | InterfaceNotRevealed(denv, intfTy, _) -> os.Append(InterfaceNotRevealedE(), NicePrint.minimalRichTextOfType denv intfTy)

        | NotAFunctionButIndexer(_, _, name, _, _) ->
            match name with
            | Some name -> os.Append(FSComp.SR.notAFunctionButMaybeIndexerWithName2 (RichText.mkLocal name))
            | _ -> os.Append(FSComp.SR.notAFunctionButMaybeIndexer2 ())

        | NotAFunction(denv, ty, _, marg) ->
            if marg.StartColumn = 0 then
                os.Append(FSComp.SR.notAFunctionButMaybeDeclaration ())
            elif isTyparTy denv.g ty then
                os.Append(FSComp.SR.notAFunction ())
            else
                os.Append(FSComp.SR.notAFunctionWithType (NicePrint.prettyRichTextOfTy denv ty))

        | TyconBadArgs(_, tcref, d, _) ->
            let exp = tcref.Typars.Length

            if exp = 0 then
                os.Append(FSComp.SR.buildUnexpectedTypeArgs (richTextOfQualifiedTyconRef tcref, d))
            else
                os.Append(fun rich -> TyconBadArgsE().Format (rich (richTextOfQualifiedTyconRef tcref)) exp d)

        | IndeterminateType _ -> os.Append(IndeterminateTypeE().Format)

        | NameClash(nm, k1, nm1, _, k2, nm2, _) ->
            if nm = nm1 && nm1 = nm2 && k1 = k2 then
                os.Append(NameClash1E(), RichText.mkText k1, richTextOfNameOfUnknownKind nm1)
            else
                os.Append(fun rich ->
                    NameClash2E().Format
                        k1
                        (rich (richTextOfNameOfUnknownKind nm1))
                        (rich (richTextOfNameOfUnknownKind nm))
                        k2
                        (rich (richTextOfNameOfUnknownKind nm2)))

        | Duplicate(k, s, _) ->
            if k = "member" then
                os.Append(Duplicate1E(), RichText.mkMember (ConvertValLogicalNameToDisplayNameCore s))
            else
                os.Append(Duplicate2E(), RichText.mkText k, richTextOfNameOfUnknownKind s)

        | UndefinedName(_, k, id, suggestionsF) ->
            os.Append(k (richTextOfUnresolvedName id.idText))
            OutputNameSuggestions os suggestNames suggestionsF id.idText

        | InternalUndefinedItemRef(f, smr, ccuName, s) ->
            let _, errs = f (smr, ccuName, s)
            os.Append errs

        | FieldNotMutable _ -> os.Append(FieldNotMutableE().Format)

        | FieldsFromDifferentTypes(_, fref1, fref2, _) ->
            os.Append(FieldsFromDifferentTypesE(), RichText.mkRecordField fref1.FieldName, RichText.mkRecordField fref2.FieldName)

        | VarBoundTwice id -> os.Append(VarBoundTwiceE(), RichText.mkLocal (ConvertValLogicalNameToDisplayNameCore id.idText))

        | Recursion(denv, id, ty1, ty2, _) ->
            let ty1, ty2, tpcs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2

            let name = RichText.mkFunction (ConvertValLogicalNameToDisplayNameCore id.idText)

            os.Append(RecursionE(), name, ty1, ty2, tpcs)

        | InvalidRuntimeCoercion(denv, ty1, ty2, _) ->
            let ty1, ty2, tpcs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(InvalidRuntimeCoercionE(), ty1, ty2, tpcs)

        | IndeterminateRuntimeCoercion(denv, ty1, ty2, _) ->
            let ty1, ty2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(IndeterminateRuntimeCoercionE(), ty1, ty2)

        | IndeterminateStaticCoercion(denv, ty1, ty2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(IndeterminateStaticCoercionE(), ty1, ty2)

        | StaticCoercionShouldUseBox(denv, ty1, ty2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(StaticCoercionShouldUseBoxE(), ty1, ty2)

        | TypeIsImplicitlyAbstract _ -> os.Append(TypeIsImplicitlyAbstractE().Format)

        | NonRigidTypar(denv, tpnmOpt, typarRange, ty1, ty2, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let (ty1, ty2), _cxs = PrettyTypes.PrettifyTypePair denv.g (ty1, ty2)

            let ty2 = NicePrint.richTextOfTy denv ty2

            match tpnmOpt with
            | None -> os.Append(NonRigidTypar1E(), RichText.mkText (stringOfRange typarRange), ty2)
            | Some tpnm ->
                let tpnm = RichText.mkTypeParameter tpnm

                match ty1 with
                | TType_measure _ -> os.Append(NonRigidTypar2E(), tpnm, ty2)
                | _ -> os.Append(NonRigidTypar3E(), tpnm, ty2)

        | SyntaxError(ctxt, _) ->
            let ctxt = unbox<Parsing.ParseErrorContext<Parser.token>> ctxt

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
                | Parser.TOKEN_IDENT -> SR.GetString("Parser.TOKEN.IDENT")
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
                | Parser.TOKEN_NATIVEINT -> SR.GetString("Parser.TOKEN.INT")
                | Parser.TOKEN_IEEE32
                | Parser.TOKEN_IEEE64 -> SR.GetString("Parser.TOKEN.FLOAT")
                | Parser.TOKEN_DECIMAL -> SR.GetString("Parser.TOKEN.DECIMAL")
                | Parser.TOKEN_CHAR -> SR.GetString("Parser.TOKEN.CHAR")

                | Parser.TOKEN_BASE -> SR.GetString("Parser.TOKEN.BASE")
                | Parser.TOKEN_LPAREN_STAR_RPAREN -> SR.GetString("Parser.TOKEN.LPAREN.STAR.RPAREN")
                | Parser.TOKEN_DOLLAR -> SR.GetString("Parser.TOKEN.DOLLAR")
                | Parser.TOKEN_INFIX_STAR_STAR_OP -> SR.GetString("Parser.TOKEN.INFIX.STAR.STAR.OP")
                | Parser.TOKEN_INFIX_COMPARE_OP -> SR.GetString("Parser.TOKEN.INFIX.COMPARE.OP")
                | Parser.TOKEN_COLON_GREATER -> SR.GetString("Parser.TOKEN.COLON.GREATER")
                | Parser.TOKEN_COLON_COLON -> SR.GetString("Parser.TOKEN.COLON.COLON")
                | Parser.TOKEN_PERCENT_OP -> SR.GetString("Parser.TOKEN.PERCENT.OP")
                | Parser.TOKEN_INFIX_AT_HAT_OP -> SR.GetString("Parser.TOKEN.INFIX.AT.HAT.OP")
                | Parser.TOKEN_INFIX_BAR_OP -> SR.GetString("Parser.TOKEN.INFIX.BAR.OP")
                | Parser.TOKEN_PLUS_MINUS_OP -> SR.GetString("Parser.TOKEN.PLUS.MINUS.OP")
                | Parser.TOKEN_PREFIX_OP -> SR.GetString("Parser.TOKEN.PREFIX.OP")
                | Parser.TOKEN_COLON_QMARK_GREATER -> SR.GetString("Parser.TOKEN.COLON.QMARK.GREATER")
                | Parser.TOKEN_INFIX_STAR_DIV_MOD_OP -> SR.GetString("Parser.TOKEN.INFIX.STAR.DIV.MOD.OP")
                | Parser.TOKEN_INFIX_AMP_OP -> SR.GetString("Parser.TOKEN.INFIX.AMP.OP")
                | Parser.TOKEN_AMP -> SR.GetString("Parser.TOKEN.AMP")
                | Parser.TOKEN_AMP_AMP -> SR.GetString("Parser.TOKEN.AMP.AMP")
                | Parser.TOKEN_BAR_BAR -> SR.GetString("Parser.TOKEN.BAR.BAR")
                | Parser.TOKEN_LESS -> SR.GetString("Parser.TOKEN.LESS")
                | Parser.TOKEN_GREATER -> SR.GetString("Parser.TOKEN.GREATER")
                | Parser.TOKEN_QMARK -> SR.GetString("Parser.TOKEN.QMARK")
                | Parser.TOKEN_QMARK_QMARK -> SR.GetString("Parser.TOKEN.QMARK.QMARK")
                | Parser.TOKEN_COLON_QMARK -> SR.GetString("Parser.TOKEN.COLON.QMARK")
                | Parser.TOKEN_INT32_DOT_DOT -> SR.GetString("Parser.TOKEN.INT32.DOT.DOT")
                | Parser.TOKEN_DOT_DOT -> SR.GetString("Parser.TOKEN.DOT.DOT")
                | Parser.TOKEN_DOT_DOT_HAT -> SR.GetString("Parser.TOKEN.DOT.DOT.HAT")
                | Parser.TOKEN_DOT_DOT_DOT -> SR.GetString("Parser.TOKEN.DOT.DOT.DOT")
                | Parser.TOKEN_QUOTE -> SR.GetString("Parser.TOKEN.QUOTE")
                | Parser.TOKEN_STAR -> SR.GetString("Parser.TOKEN.STAR")
                | Parser.TOKEN_HIGH_PRECEDENCE_TYAPP -> SR.GetString("Parser.TOKEN.HIGH.PRECEDENCE.TYAPP")
                | Parser.TOKEN_COLON -> SR.GetString("Parser.TOKEN.COLON")
                | Parser.TOKEN_COLON_EQUALS -> SR.GetString("Parser.TOKEN.COLON.EQUALS")
                | Parser.TOKEN_LARROW -> SR.GetString("Parser.TOKEN.LARROW")
                | Parser.TOKEN_EQUALS -> SR.GetString("Parser.TOKEN.EQUALS")
                | Parser.TOKEN_GREATER_BAR_RBRACE -> SR.GetString("Parser.TOKEN.GREATER.BAR.RBRACE")
                | Parser.TOKEN_GREATER_BAR_RBRACK -> SR.GetString("Parser.TOKEN.GREATER.BAR.RBRACK")
                | Parser.TOKEN_MINUS -> SR.GetString("Parser.TOKEN.MINUS")
                | Parser.TOKEN_ADJACENT_PREFIX_OP -> SR.GetString("Parser.TOKEN.ADJACENT.PREFIX.OP")
                | Parser.TOKEN_FUNKY_OPERATOR_NAME -> SR.GetString("Parser.TOKEN.FUNKY.OPERATOR.NAME")
                | Parser.TOKEN_COMMA -> SR.GetString("Parser.TOKEN.COMMA")
                | Parser.TOKEN_DOT -> SR.GetString("Parser.TOKEN.DOT")
                | Parser.TOKEN_BAR -> SR.GetString("Parser.TOKEN.BAR")
                | Parser.TOKEN_HASH -> SR.GetString("Parser.TOKEN.HASH")
                | Parser.TOKEN_UNDERSCORE -> SR.GetString("Parser.TOKEN.UNDERSCORE")
                | Parser.TOKEN_SEMICOLON -> SR.GetString("Parser.TOKEN.SEMICOLON")
                | Parser.TOKEN_SEMICOLON_SEMICOLON -> SR.GetString("Parser.TOKEN.SEMICOLON.SEMICOLON")
                | Parser.TOKEN_LPAREN -> SR.GetString("Parser.TOKEN.LPAREN")
                | Parser.TOKEN_RPAREN
                | Parser.TOKEN_RPAREN_COMING_SOON
                | Parser.TOKEN_RPAREN_IS_HERE -> SR.GetString("Parser.TOKEN.RPAREN")
                | Parser.TOKEN_LQUOTE -> SR.GetString("Parser.TOKEN.LQUOTE")
                | Parser.TOKEN_LBRACK -> SR.GetString("Parser.TOKEN.LBRACK")
                | Parser.TOKEN_LBRACE_BAR -> SR.GetString("Parser.TOKEN.LBRACE.BAR")
                | Parser.TOKEN_LBRACK_BAR -> SR.GetString("Parser.TOKEN.LBRACK.BAR")
                | Parser.TOKEN_LBRACK_LESS -> SR.GetString("Parser.TOKEN.LBRACK.LESS")
                | Parser.TOKEN_LBRACE -> SR.GetString("Parser.TOKEN.LBRACE")
                | Parser.TOKEN_BAR_RBRACK -> SR.GetString("Parser.TOKEN.BAR.RBRACK")
                | Parser.TOKEN_BAR_RBRACE -> SR.GetString("Parser.TOKEN.BAR.RBRACE")
                | Parser.TOKEN_GREATER_RBRACK -> SR.GetString("Parser.TOKEN.GREATER.RBRACK")
                | Parser.TOKEN_RQUOTE_DOT
                | Parser.TOKEN_RQUOTE -> SR.GetString("Parser.TOKEN.RQUOTE")
                | Parser.TOKEN_RQUOTE_BAR_RBRACE -> SR.GetString("Parser.TOKEN.RQUOTE.BAR.RBRACE")
                | Parser.TOKEN_RBRACK -> SR.GetString("Parser.TOKEN.RBRACK")
                | Parser.TOKEN_RBRACE
                | Parser.TOKEN_RBRACE_COMING_SOON
                | Parser.TOKEN_RBRACE_IS_HERE -> SR.GetString("Parser.TOKEN.RBRACE")
                | Parser.TOKEN_PUBLIC -> SR.GetString("Parser.TOKEN.PUBLIC")
                | Parser.TOKEN_PRIVATE -> SR.GetString("Parser.TOKEN.PRIVATE")
                | Parser.TOKEN_INTERNAL -> SR.GetString("Parser.TOKEN.INTERNAL")
                | Parser.TOKEN_CONSTRAINT -> SR.GetString("Parser.TOKEN.CONSTRAINT")
                | Parser.TOKEN_INSTANCE -> SR.GetString("Parser.TOKEN.INSTANCE")
                | Parser.TOKEN_DELEGATE -> SR.GetString("Parser.TOKEN.DELEGATE")
                | Parser.TOKEN_INHERIT -> SR.GetString("Parser.TOKEN.INHERIT")
                | Parser.TOKEN_CONSTRUCTOR -> SR.GetString("Parser.TOKEN.CONSTRUCTOR")
                | Parser.TOKEN_DEFAULT -> SR.GetString("Parser.TOKEN.DEFAULT")
                | Parser.TOKEN_OVERRIDE -> SR.GetString("Parser.TOKEN.OVERRIDE")
                | Parser.TOKEN_ABSTRACT -> SR.GetString("Parser.TOKEN.ABSTRACT")
                | Parser.TOKEN_CLASS -> SR.GetString("Parser.TOKEN.CLASS")
                | Parser.TOKEN_MEMBER -> SR.GetString("Parser.TOKEN.MEMBER")
                | Parser.TOKEN_STATIC -> SR.GetString("Parser.TOKEN.STATIC")
                | Parser.TOKEN_NAMESPACE -> SR.GetString("Parser.TOKEN.NAMESPACE")
                | Parser.TOKEN_OBLOCKBEGIN -> SR.GetString("Parser.TOKEN.OBLOCKBEGIN")
                | EndOfStructuredConstructToken -> SR.GetString("Parser.TOKEN.OBLOCKEND")
                | Parser.TOKEN_THEN
                | Parser.TOKEN_OTHEN -> SR.GetString("Parser.TOKEN.OTHEN")
                | Parser.TOKEN_ELSE
                | Parser.TOKEN_OELSE -> SR.GetString("Parser.TOKEN.OELSE")
                | Parser.TOKEN_LET
                | Parser.TOKEN_OLET -> SR.GetString("Parser.TOKEN.OLET")
                | Parser.TOKEN_OBINDER
                | Parser.TOKEN_BINDER -> SR.GetString("Parser.TOKEN.BINDER")
                | Parser.TOKEN_OAND_BANG
                | Parser.TOKEN_AND_BANG -> SR.GetString("Parser.TOKEN.AND.BANG")
                | Parser.TOKEN_ODO -> SR.GetString("Parser.TOKEN.ODO")
                | Parser.TOKEN_OWITH -> SR.GetString("Parser.TOKEN.OWITH")
                | Parser.TOKEN_OFUNCTION -> SR.GetString("Parser.TOKEN.OFUNCTION")
                | Parser.TOKEN_OFUN -> SR.GetString("Parser.TOKEN.OFUN")
                | Parser.TOKEN_ORESET -> SR.GetString("Parser.TOKEN.ORESET")
                | Parser.TOKEN_ODUMMY -> SR.GetString("Parser.TOKEN.ODUMMY")
                | Parser.TOKEN_DO_BANG
                | Parser.TOKEN_ODO_BANG -> SR.GetString("Parser.TOKEN.ODO.BANG")
                | Parser.TOKEN_YIELD -> SR.GetString("Parser.TOKEN.YIELD")
                | Parser.TOKEN_YIELD_BANG -> SR.GetString("Parser.TOKEN.YIELD.BANG")
                | Parser.TOKEN_OINTERFACE_MEMBER -> SR.GetString("Parser.TOKEN.OINTERFACE.MEMBER")
                | Parser.TOKEN_ELIF -> SR.GetString("Parser.TOKEN.ELIF")
                | Parser.TOKEN_RARROW -> SR.GetString("Parser.TOKEN.RARROW")
                | Parser.TOKEN_SIG -> SR.GetString("Parser.TOKEN.SIG")
                | Parser.TOKEN_STRUCT -> SR.GetString("Parser.TOKEN.STRUCT")
                | Parser.TOKEN_UPCAST -> SR.GetString("Parser.TOKEN.UPCAST")
                | Parser.TOKEN_DOWNCAST -> SR.GetString("Parser.TOKEN.DOWNCAST")
                | Parser.TOKEN_NULL -> SR.GetString("Parser.TOKEN.NULL")
                | Parser.TOKEN_RESERVED -> SR.GetString("Parser.TOKEN.RESERVED")
                | Parser.TOKEN_MODULE
                | Parser.TOKEN_MODULE_COMING_SOON
                | Parser.TOKEN_MODULE_IS_HERE -> SR.GetString("Parser.TOKEN.MODULE")
                | Parser.TOKEN_AND -> SR.GetString("Parser.TOKEN.AND")
                | Parser.TOKEN_AS -> SR.GetString("Parser.TOKEN.AS")
                | Parser.TOKEN_ASSERT -> SR.GetString("Parser.TOKEN.ASSERT")
                | Parser.TOKEN_OASSERT -> SR.GetString("Parser.TOKEN.ASSERT")
                | Parser.TOKEN_ASR -> SR.GetString("Parser.TOKEN.ASR")
                | Parser.TOKEN_DOWNTO -> SR.GetString("Parser.TOKEN.DOWNTO")
                | Parser.TOKEN_EXCEPTION -> SR.GetString("Parser.TOKEN.EXCEPTION")
                | Parser.TOKEN_FALSE -> SR.GetString("Parser.TOKEN.FALSE")
                | Parser.TOKEN_FOR -> SR.GetString("Parser.TOKEN.FOR")
                | Parser.TOKEN_FUN -> SR.GetString("Parser.TOKEN.FUN")
                | Parser.TOKEN_FUNCTION -> SR.GetString("Parser.TOKEN.FUNCTION")
                | Parser.TOKEN_FINALLY -> SR.GetString("Parser.TOKEN.FINALLY")
                | Parser.TOKEN_LAZY -> SR.GetString("Parser.TOKEN.LAZY")
                | Parser.TOKEN_OLAZY -> SR.GetString("Parser.TOKEN.LAZY")
                | Parser.TOKEN_MATCH -> SR.GetString("Parser.TOKEN.MATCH")
                | Parser.TOKEN_MATCH_BANG -> SR.GetString("Parser.TOKEN.MATCH.BANG")
                | Parser.TOKEN_MUTABLE -> SR.GetString("Parser.TOKEN.MUTABLE")
                | Parser.TOKEN_NEW -> SR.GetString("Parser.TOKEN.NEW")
                | Parser.TOKEN_OF -> SR.GetString("Parser.TOKEN.OF")
                | Parser.TOKEN_OPEN -> SR.GetString("Parser.TOKEN.OPEN")
                | Parser.TOKEN_OR -> SR.GetString("Parser.TOKEN.OR")
                | Parser.TOKEN_VOID -> SR.GetString("Parser.TOKEN.VOID")
                | Parser.TOKEN_EXTERN -> SR.GetString("Parser.TOKEN.EXTERN")
                | Parser.TOKEN_INTERFACE -> SR.GetString("Parser.TOKEN.INTERFACE")
                | Parser.TOKEN_REC -> SR.GetString("Parser.TOKEN.REC")
                | Parser.TOKEN_TO -> SR.GetString("Parser.TOKEN.TO")
                | Parser.TOKEN_TRUE -> SR.GetString("Parser.TOKEN.TRUE")
                | Parser.TOKEN_TRY -> SR.GetString("Parser.TOKEN.TRY")
                | Parser.TOKEN_TYPE
                | Parser.TOKEN_TYPE_COMING_SOON
                | Parser.TOKEN_TYPE_IS_HERE -> SR.GetString("Parser.TOKEN.TYPE")
                | Parser.TOKEN_VAL -> SR.GetString("Parser.TOKEN.VAL")
                | Parser.TOKEN_INLINE -> SR.GetString("Parser.TOKEN.INLINE")
                | Parser.TOKEN_WHEN -> SR.GetString("Parser.TOKEN.WHEN")
                | Parser.TOKEN_WHILE -> SR.GetString("Parser.TOKEN.WHILE")
                | Parser.TOKEN_WHILE_BANG -> SR.GetString("Parser.TOKEN.WHILE.BANG")
                | Parser.TOKEN_WITH -> SR.GetString("Parser.TOKEN.WITH")
                | Parser.TOKEN_IF -> SR.GetString("Parser.TOKEN.IF")
                | Parser.TOKEN_DO -> SR.GetString("Parser.TOKEN.DO")
                | Parser.TOKEN_GLOBAL -> SR.GetString("Parser.TOKEN.GLOBAL")
                | Parser.TOKEN_DONE -> SR.GetString("Parser.TOKEN.DONE")
                | Parser.TOKEN_IN
                | Parser.TOKEN_JOIN_IN -> SR.GetString("Parser.TOKEN.IN")
                | Parser.TOKEN_HIGH_PRECEDENCE_PAREN_APP -> SR.GetString("Parser.TOKEN.HIGH.PRECEDENCE.PAREN.APP")
                | Parser.TOKEN_HIGH_PRECEDENCE_BRACK_APP -> SR.GetString("Parser.TOKEN.HIGH.PRECEDENCE.BRACK.APP")
                | Parser.TOKEN_BEGIN -> SR.GetString("Parser.TOKEN.BEGIN")
                | Parser.TOKEN_END -> SR.GetString("Parser.TOKEN.END")
                | Parser.TOKEN_HASH_LINE
                | Parser.TOKEN_HASH_IF
                | Parser.TOKEN_HASH_ELSE
                | Parser.TOKEN_HASH_ENDIF
                | Parser.TOKEN_HASH_ELIF -> SR.GetString("Parser.TOKEN.HASH.ENDIF")
                | Parser.TOKEN_INACTIVECODE -> SR.GetString("Parser.TOKEN.INACTIVECODE")
                | Parser.TOKEN_LEX_FAILURE -> SR.GetString("Parser.TOKEN.LEX.FAILURE")
                | Parser.TOKEN_WHITESPACE -> SR.GetString("Parser.TOKEN.WHITESPACE")
                | Parser.TOKEN_COMMENT -> SR.GetString("Parser.TOKEN.COMMENT")
                | Parser.TOKEN_LINE_COMMENT -> SR.GetString("Parser.TOKEN.LINE.COMMENT")
                | Parser.TOKEN_STRING_TEXT -> SR.GetString("Parser.TOKEN.STRING.TEXT")
                | Parser.TOKEN_BYTEARRAY -> SR.GetString("Parser.TOKEN.BYTEARRAY")
                | Parser.TOKEN_STRING -> SR.GetString("Parser.TOKEN.STRING")
                | Parser.TOKEN_KEYWORD_STRING -> SR.GetString("Parser.TOKEN.KEYWORD_STRING")
                | Parser.TOKEN_EOF -> SR.GetString("Parser.TOKEN.EOF")
                | Parser.TOKEN_CONST -> SR.GetString("Parser.TOKEN.CONST")
                | Parser.TOKEN_FIXED -> SR.GetString("Parser.TOKEN.FIXED")
                | Parser.TOKEN_INTERP_STRING_BEGIN_END -> SR.GetString("Parser.TOKEN.INTERP.STRING.BEGIN.END")
                | Parser.TOKEN_INTERP_STRING_BEGIN_PART -> SR.GetString("Parser.TOKEN.INTERP.STRING.BEGIN.PART")
                | Parser.TOKEN_INTERP_STRING_PART -> SR.GetString("Parser.TOKEN.INTERP.STRING.PART")
                | Parser.TOKEN_INTERP_STRING_END -> SR.GetString("Parser.TOKEN.INTERP.STRING.END")
                | Parser.TOKEN_BAR_JUST_BEFORE_NULL -> SR.GetString("Parser.TOKEN.BAR_JUST_BEFORE_NULL")
                | unknown ->
                    let result = sprintf "unknown token tag %+A" unknown
                    Debug.Assert(false, result)
                    result

#if DEBUG
            if showParserStackOnParseError then
                printfn "parser stack:"

                let rps =
                    ctxt.ReducibleProductions
                    |> List.map (fun rps -> rps |> List.map (fun rp -> rp, Parser.prodIdxToNonTerminal rp))

                for rps in rps do
                    printfn "   ----"
                    //printfn "   state %d" state
                    for rp, nonTerminalId in rps do
                        printfn $"       non-terminal %+A{nonTerminalId} (idx {rp}): ... "
#endif

            match ctxt.CurrentToken with
            | None -> os.Append(UnexpectedEndOfInputE().Format)
            | Some token ->
                let tokenId = token |> Parser.tagOfToken |> Parser.tokenTagToTokenId

                match tokenId, token with
                | EndOfStructuredConstructToken, _ -> os.Append(OBlockEndSentenceE().Format)
                | Parser.TOKEN_LEX_FAILURE, Parser.LEX_FAILURE str -> os.Append str
                | token, _ -> os.Append(UnexpectedE().Format(token |> tokenIdToText))

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
                            |> List.map (
                                Parser.prodIdxToNonTerminal
                                >> fun nonTerminal ->
                                    match nonTerminal with
                                    | NONTERM_Category_Type -> Parser.NONTERM_typ
                                    | NONTERM_Category_Expr -> Parser.NONTERM_declExpr
                                    | NONTERM_Category_Pattern -> Parser.NONTERM_atomicPattern
                                    | NONTERM_Category_IfThenElse -> Parser.NONTERM_ifExprThen
                                    | NONTERM_Category_SignatureFile -> Parser.NONTERM_signatureFile
                                    | NONTERM_Category_ImplementationFile -> Parser.NONTERM_implementationFile
                                    | NONTERM_Category_Definition -> Parser.NONTERM_moduleDefn
                                    | NONTERM_Category_Interaction -> Parser.NONTERM_interaction
                                    | nt -> nt
                            )
                            |> Set.ofList
                            |> Set.toList

                        match prodIds with
                        | [ Parser.NONTERM_interaction ] ->
                            os.Append(NONTERM_interactionE().Format)
                            true
                        | [ Parser.NONTERM_hashDirective ] ->
                            os.Append(NONTERM_hashDirectiveE().Format)
                            true
                        | [ Parser.NONTERM_fieldDecl ] ->
                            os.Append(NONTERM_fieldDeclE().Format)
                            true
                        | [ Parser.NONTERM_unionCaseRepr ] ->
                            os.Append(NONTERM_unionCaseReprE().Format)
                            true
                        | [ Parser.NONTERM_localBinding ] ->
                            os.Append(NONTERM_localBindingE().Format)
                            true
                        | [ Parser.NONTERM_hardwhiteLetBindings ] ->
                            os.Append(NONTERM_hardwhiteLetBindingsE().Format)
                            true
                        | [ Parser.NONTERM_classDefnMember ] ->
                            os.Append(NONTERM_classDefnMemberE().Format)
                            true
                        | [ Parser.NONTERM_defnBindings ] ->
                            os.Append(NONTERM_defnBindingsE().Format)
                            true
                        | [ Parser.NONTERM_classMemberSpfn ] ->
                            os.Append(NONTERM_classMemberSpfnE().Format)
                            true
                        | [ Parser.NONTERM_classMemberSpfnGetSetElements ] ->
                            os.Append(NONTERM_classMemberSpfnGetSetElementsE().Format)
                            true
                        | [ Parser.NONTERM_autoPropsDefnDecl ] ->
                            os.Append(NONTERM_autoPropsDefnDeclE().Format)
                            true
                        | [ Parser.NONTERM_valSpfn ] ->
                            os.Append(NONTERM_valSpfnE().Format)
                            true
                        | [ Parser.NONTERM_tyconSpfn ] ->
                            os.Append(NONTERM_tyconSpfnE().Format)
                            true
                        | [ Parser.NONTERM_anonLambdaExpr ] ->
                            os.Append(NONTERM_anonLambdaExprE().Format)
                            true
                        | [ Parser.NONTERM_attrUnionCaseDecl ] ->
                            os.Append(NONTERM_attrUnionCaseDeclE().Format)
                            true
                        | [ Parser.NONTERM_cPrototype ] ->
                            os.Append(NONTERM_cPrototypeE().Format)
                            true
                        | [ Parser.NONTERM_objExpr | Parser.NONTERM_objectImplementationMembers ] ->
                            os.Append(NONTERM_objectImplementationMembersE().Format)
                            true
                        | [ Parser.NONTERM_ifExprThen | Parser.NONTERM_ifExprElifs | Parser.NONTERM_ifExprCases ] ->
                            os.Append(NONTERM_ifExprCasesE().Format)
                            true
                        | [ Parser.NONTERM_openDecl ] ->
                            os.Append(NONTERM_openDeclE().Format)
                            true
                        | [ Parser.NONTERM_fileModuleSpec ] ->
                            os.Append(NONTERM_fileModuleSpecE().Format)
                            true
                        | [ Parser.NONTERM_patternClauses ] ->
                            os.Append(NONTERM_patternClausesE().Format)
                            true
                        | [ Parser.NONTERM_beginEndExpr ] ->
                            os.Append(NONTERM_beginEndExprE().Format)
                            true
                        | [ Parser.NONTERM_recdExpr ] ->
                            os.Append(NONTERM_recdExprE().Format)
                            true
                        | [ Parser.NONTERM_tyconDefn ] ->
                            os.Append(NONTERM_tyconDefnE().Format)
                            true
                        | [ Parser.NONTERM_exconCore ] ->
                            os.Append(NONTERM_exconCoreE().Format)
                            true
                        | [ Parser.NONTERM_typeNameInfo ] ->
                            os.Append(NONTERM_typeNameInfoE().Format)
                            true
                        | [ Parser.NONTERM_attributeList ] ->
                            os.Append(NONTERM_attributeListE().Format)
                            true
                        | [ Parser.NONTERM_quoteExpr ] ->
                            os.Append(NONTERM_quoteExprE().Format)
                            true
                        | [ Parser.NONTERM_typeConstraint ] ->
                            os.Append(NONTERM_typeConstraintE().Format)
                            true
                        | [ NONTERM_Category_ImplementationFile ] ->
                            os.Append(NONTERM_Category_ImplementationFileE().Format)
                            true
                        | [ NONTERM_Category_Definition ] ->
                            os.Append(NONTERM_Category_DefinitionE().Format)
                            true
                        | [ NONTERM_Category_SignatureFile ] ->
                            os.Append(NONTERM_Category_SignatureFileE().Format)
                            true
                        | [ NONTERM_Category_Pattern ] ->
                            os.Append(NONTERM_Category_PatternE().Format)
                            true
                        | [ NONTERM_Category_Expr ] ->
                            os.Append(NONTERM_Category_ExprE().Format)
                            true
                        | [ NONTERM_Category_Type ] ->
                            os.Append(NONTERM_Category_TypeE().Format)
                            true
                        | [ Parser.NONTERM_typeArgsActual ] ->
                            os.Append(NONTERM_typeArgsActualE().Format)
                            true
                        | _ -> false)

#if DEBUG
                if not foundInContext then
                    os.Append(
                        sprintf ". (no 'in' context found: %+A)" (List.mapSquared Parser.prodIdxToNonTerminal ctxt.ReducibleProductions)
                    )
#else
                foundInContext |> ignore // suppress unused variable warning in RELEASE
#endif
                // tokenIdToText describes a token as a keyword, as a symbol, or by a category such as
                // 'identifier'. The message drops that wording, so it is what tells us how to classify
                // what is left of it.
                let fix (s: string) =
                    let keyword = SR.GetString("FixKeyword")
                    let symbol = SR.GetString("FixSymbol")

                    let tag =
                        if s.Contains keyword then TextTag.Keyword
                        elif s.Contains symbol then TextTag.Punctuation
                        else TextTag.Text

                    s.Replace(keyword, "").Replace(symbol, "").Replace(SR.GetString("FixReplace"), "")
                    |> RichText.ofTag tag

                let tokenNames =
                    ctxt.ShiftTokens
                    |> List.map Parser.tokenTagToTokenId
                    |> List.filter (function
                        | Parser.TOKEN_error
                        | Parser.TOKEN_OBLOCKSEP
                        | Parser.TOKEN_EOF -> false
                        | _ -> true)
                    |> List.map tokenIdToText
                    |> Set.ofList
                    |> Set.toList

                match tokenNames with
                | [ tokenName1 ] -> os.Append(TokenName1E(), fix tokenName1)
                | [ tokenName1; tokenName2 ] -> os.Append(TokenName1TokenName2E(), fix tokenName1, fix tokenName2)
                | [ tokenName1; tokenName2; tokenName3 ] ->
                    os.Append(TokenName1TokenName2TokenName3E(), fix tokenName1, fix tokenName2, fix tokenName3)
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

            if isTyparTy denv.g ty then
                os.Append(RuntimeCoercionSourceSealed1E(), NicePrint.richTextOfTy denv ty)
            else
                os.Append(RuntimeCoercionSourceSealed2E(), NicePrint.richTextOfTy denv ty)

        | CoercionTargetSealed(denv, ty, _) ->
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            os.Append(CoercionTargetSealedE(), NicePrint.richTextOfTy denv ty)

        | UpcastUnnecessary _ -> os.Append(UpcastUnnecessaryE().Format)

        | TypeTestUnnecessary _ -> os.Append(TypeTestUnnecessaryE().Format)

        | QuotationTranslator.IgnoringPartOfQuotedTermWarning(msg, _) -> os.Append msg

        | OverrideDoesntOverride(denv, impl, minfoVirtOpt, g, amap, m) ->
            let sig1 = DispatchSlotChecking.FormatOverride denv impl

            match minfoVirtOpt with
            | None -> os.Append(OverrideDoesntOverride1E(), sig1)
            | Some minfoVirt ->
                // https://github.com/dotnet/fsharp/issues/35
                // Improve error message when attempting to override generic return type with unit:
                // we need to check if unit was used as a type argument
                let hasUnitTType_app (types: TType list) =
                    types
                    |> List.exists (function
                        | TType_app(maybeUnit, [], _) ->
                            match maybeUnit.TypeAbbrev with
                            | Some ty when isUnitTy g ty -> true
                            | _ -> false
                        | _ -> false)

                match minfoVirt.ApparentEnclosingType with
                | TType_app(tycon, tyargs, _) when tycon.IsFSharpInterfaceTycon && hasUnitTType_app tyargs ->
                    // match abstract member with 'unit' passed as generic argument
                    os.Append(OverrideDoesntOverride4E(), sig1)
                | _ ->
                    os.Append(OverrideDoesntOverride2E(), sig1)
                    let sig2 = DispatchSlotChecking.FormatMethInfoSig g amap m denv minfoVirt

                    if sig1 <> sig2 then
                        os.Append(OverrideDoesntOverride3E(), sig2)

                    // If implementation and required slot doesn't have same "instance-ness", then tell user that.
                    if impl.IsInstance <> minfoVirt.IsInstance then
                        // Required slot is instance, meaning implementation is static, tell user that we expect instance.
                        if minfoVirt.IsInstance then
                            os.Append(OverrideShouldBeStatic().Format)
                        else
                            os.Append(OverrideShouldBeInstance().Format)

        | UnionCaseWrongArguments(_, n1, n2, _) -> os.Append(UnionCaseWrongArgumentsE().Format n2 n1)

        | UnionPatternsBindDifferentNames _ -> os.Append(UnionPatternsBindDifferentNamesE().Format)

        | ValueNotContained(_, denv, infoReader, mref, implVal, sigVal, f) ->
            let text1, text2 =
                NicePrint.minimalRichTextsOfTwoValues denv infoReader (mkLocalValRef implVal) (mkLocalValRef sigVal)

            os.Append(f (richTextOfQualifiedModRef mref, text1, text2))

        | UnionCaseNotContained(denv, infoReader, enclosingTycon, v1, v2, f) ->
            let enclosingTcref = mkLocalEntityRef enclosingTycon

            os.Append(
                f (
                    (NicePrint.richTextOfUnionCase denv infoReader enclosingTcref v1),
                    (NicePrint.richTextOfUnionCase denv infoReader enclosingTcref v2)
                )
            )

        | FSharpExceptionNotContained(denv, infoReader, v1, v2, f) ->
            os.Append(
                f (
                    (NicePrint.richTextOfExnDef denv infoReader (mkLocalEntityRef v1)),
                    (NicePrint.richTextOfExnDef denv infoReader (mkLocalEntityRef v2))
                )
            )

        | FieldNotContained(_, denv, infoReader, enclosingTycon, _, v1, v2, f) ->
            let enclosingTcref = mkLocalEntityRef enclosingTycon

            os.Append(
                f (
                    (NicePrint.richTextOfRecdField denv infoReader enclosingTcref v1),
                    (NicePrint.richTextOfRecdField denv infoReader enclosingTcref v2)
                )
            )

        | RequiredButNotSpecified(_, mref, k, name, _) ->
            let nsb = RichTextBuilder()
            name nsb

            os.Append(RequiredButNotSpecifiedE(), richTextOfQualifiedModRef mref, RichText.mkText k, nsb.ToRichText())

        | UseOfAddressOfOperator _ -> os.Append(UseOfAddressOfOperatorE().Format)

        | DefensiveCopyWarning(s, _) -> os.Append(DefensiveCopyWarningE().Format s)

        | DeprecatedThreadStaticBindingWarning _ -> os.Append(DeprecatedThreadStaticBindingWarningE().Format)

        | FunctionValueUnexpected(denv, ty, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            os.Append(FunctionValueUnexpectedE(), NicePrint.richTextOfTy denv ty)

        | UnitTypeExpected(denv, ty, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            os.Append(UnitTypeExpectedE(), NicePrint.richTextOfTy denv ty)

        | UnitTypeExpectedWithEquality(denv, ty, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            os.Append(UnitTypeExpectedWithEqualityE(), NicePrint.richTextOfTy denv ty)

        | UnitTypeExpectedWithPossiblePropertySetter(denv, ty, bindingName, propertyName, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            let ty = NicePrint.richTextOfTy denv ty

            os.Append(UnitTypeExpectedWithPossiblePropertySetterE(), ty, RichText.mkLocal bindingName, RichText.mkProperty propertyName)

        | UnitTypeExpectedWithPossibleAssignment(denv, ty, isAlreadyMutable, bindingName, _) ->
            let ty, _cxs = PrettyTypes.PrettifyType denv.g ty
            let ty = NicePrint.richTextOfTy denv ty

            let bindingName = RichText.mkLocal bindingName

            if isAlreadyMutable then
                os.Append(UnitTypeExpectedWithPossibleAssignmentToMutableE(), ty, bindingName)
            else
                os.Append(UnitTypeExpectedWithPossibleAssignmentE(), ty, bindingName)

        | RecursiveUseCheckedAtRuntime _ -> os.Append(RecursiveUseCheckedAtRuntimeE().Format)

        | LetRecUnsound(denv, [ v ], _) -> os.Append(LetRecUnsound1E(), richTextOfValName denv.g v.Deref)

        | LetRecUnsound(denv, path, _) ->
            let bos = RichTextBuilder()

            (path.Tail @ [ path.Head ])
            |> List.iter (fun (v: ValRef) -> bos.Append(LetRecUnsoundInnerE(), richTextOfValName denv.g v.Deref))

            os.Append(LetRecUnsound2E(), richTextOfValName denv.g (List.head path).Deref, bos.ToRichText())

        | LetRecEvaluatedOutOfOrder _ -> os.Append(LetRecEvaluatedOutOfOrderE().Format)

        | LetRecCheckedAtRuntime _ -> os.Append(LetRecCheckedAtRuntimeE().Format)

        | SelfRefObjCtor(false, _) -> os.Append(SelfRefObjCtor1E().Format)

        | SelfRefObjCtor(true, _) -> os.Append(SelfRefObjCtor2E().Format)

        | VirtualAugmentationOnNullValuedType _ -> os.Append(VirtualAugmentationOnNullValuedTypeE().Format)

        | NonVirtualAugmentationOnNullValuedType _ -> os.Append(NonVirtualAugmentationOnNullValuedTypeE().Format)

        | NonUniqueInferredAbstractSlot(_, denv, bindnm, bvirt1, bvirt2, _) ->
            os.Append(NonUniqueInferredAbstractSlot1E(), RichText.mkMember bindnm)
            let ty1 = bvirt1.ApparentEnclosingType
            let ty2 = bvirt2.ApparentEnclosingType
            // REVIEW: consider if we need to show _cxs (the type parameter constraints)
            let ty1, ty2, _cxs = NicePrint.minimalRichTextsOfTwoTypes denv ty1 ty2
            os.Append(NonUniqueInferredAbstractSlot2E().Format)

            if ty1 <> ty2 then
                os.Append(NonUniqueInferredAbstractSlot3E(), ty1, ty2)

            os.Append(NonUniqueInferredAbstractSlot4E().Format)

        | DiagnosticWithText(_, s, _)
        | DiagnosticEnabledWithLanguageFeature(_, s, _, _) -> os.Append s

        | DiagnosticWithSuggestions(_, s, _, idText, suggestionF) ->
            os.Append s
            OutputNameSuggestions os suggestNames suggestionF idText

        | InternalError(s, _)
        | InternalException(_, s, _)
        | InvalidArgument s
        | Failure s as exn ->
            ignore exn // use the argument, even in non DEBUG
            let f1 = SR.GetString("Failure1")
            let f2 = SR.GetString("Failure2")

            match s with
            | f when f = f1 -> os.Append(Failure3E().Format s)
            | f when f = f2 -> os.Append(Failure3E().Format s)
            | _ -> os.Append(Failure4E().Format s)
#if DEBUG
            os.Append(sprintf "\nStack Trace\n%s\n" (exn.ToString()))
            Debug.Assert(false, sprintf "Unexpected exception seen in compiler: %s\n%s" s (exn.ToString()))
#endif

        | WrappedError(e, _) -> e.Output(os, suggestNames)

        | PatternMatchCompilation.MatchIncomplete(isComp, cexOpt, _) ->
            os.Append(MatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some(cex, false) -> os.Append(MatchIncomplete2E(), cex)
            | Some(cex, true) -> os.Append(MatchIncomplete3E(), cex)

            if isComp then
                os.Append(MatchIncomplete4E().Format)

        | PatternMatchCompilation.MatchIncompleteForLoopHint(PatternMatchCompilation.MatchIncomplete(isComp, cexOpt, _)) ->
            os.Append(MatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some(cex, false) -> os.Append(MatchIncomplete2E(), cex)
            | Some(cex, true) -> os.Append(MatchIncomplete3E(), cex)

            os.Append(MatchIncompleteForLoopE().Format)

            if isComp then
                os.Append(MatchIncomplete4E().Format)

        | PatternMatchCompilation.EnumMatchIncomplete(isComp, cexOpt, _) ->
            os.Append(EnumMatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some(cex, false) -> os.Append(MatchIncomplete2E(), cex)
            | Some(cex, true) -> os.Append(MatchIncomplete3E(), cex)

            if isComp then
                os.Append(MatchIncomplete4E().Format)

        | PatternMatchCompilation.RuleNeverMatched _ -> os.Append(RuleNeverMatchedE().Format)

        | ValNotMutable(_, vref, _) ->
            let name = vref.DisplayName

            let msg =
                if vref.Deref.IsParameter then
                    ValNotMutableParameterE().Format name name name
                else
                    ValNotMutableE().Format name

            os.Append msg

        | ValNotLocal _ -> os.Append(ValNotLocalE().Format)

        | ObsoleteDiagnostic(message = message) ->
            os.Append(Obsolete1E().Format)

            match message with
            | Some message when not message.IsEmpty -> os.Append(Obsolete2E(), message)
            | _ -> ()

        | Experimental(message = message) ->
            os.Append(Experimental1E().Format)

            match message with
            | Some message when message <> "" -> os.Append(Experimental2E().Format message)
            | _ -> ()

            os.Append(Experimental3E().Format)

        | PossibleUnverifiableCode _ -> os.Append(PossibleUnverifiableCodeE().Format)

        | UserCompilerMessage(msg, _, _) -> os.Append msg

        | Deprecated(s, _) -> os.Append(DeprecatedE(), s)

        | LibraryUseOnly _ -> os.Append(LibraryUseOnlyE().Format)

        | MissingFields(sl, _) -> os.Append(MissingFieldsE().Format(String.concat "," sl + "."))

        | ValueRestriction(denv, infoReader, v, _, _) ->
            let denv =
                { denv with
                    showInferenceTyparAnnotations = true
                }

            let tau = v.TauType

            let name = richTextOfValName denv.g v

            let signature =
                NicePrint.richTextOfQualifiedValOrMember denv infoReader (mkLocalValRef v)

            if isFunTy denv.g tau && (arityOfVal v).HasNoArgs then
                os.Append(ValueRestrictionFunctionE(), name, signature, name)
            else
                os.Append(ValueRestrictionE(), name, signature, name)

        | Parsing.RecoverableParseError -> os.Append(RecoverableParseErrorE().Format)

        | ReservedKeyword(s, _) -> os.Append(ReservedKeywordE(), s)

        | IndentationProblem(s, _) -> os.Append(IndentationProblemE().Format s)

        | OverrideInIntrinsicAugmentation _ -> os.Append(OverrideInIntrinsicAugmentationE().Format)

        | OverrideInExtrinsicAugmentation _ -> os.Append(OverrideInExtrinsicAugmentationE().Format)

        | IntfImplInIntrinsicAugmentation _ -> os.Append(IntfImplInIntrinsicAugmentationE().Format)

        | IntfImplInExtrinsicAugmentation _ -> os.Append(IntfImplInExtrinsicAugmentationE().Format)

        | UnresolvedReferenceError(assemblyName, _)
        | UnresolvedReferenceNoRange assemblyName -> os.Append(UnresolvedReferenceNoRangeE().Format assemblyName)

        | UnresolvedPathReference(assemblyName, pathname, _)

        | UnresolvedPathReferenceNoRange(assemblyName, pathname) ->
            os.Append(UnresolvedPathReferenceNoRangeE().Format pathname assemblyName)

        | DeprecatedCommandLineOptionFull(fullText, _) -> os.Append fullText

        | DeprecatedCommandLineOptionForHtmlDoc(optionName, _) -> os.Append(FSComp.SR.optsDCLOHtmlDoc optionName)

        | DeprecatedCommandLineOptionSuggestAlternative(optionName, altOption, _) ->
            os.Append(FSComp.SR.optsDCLODeprecatedSuggestAlternative (optionName, altOption))

        | InternalCommandLineOption(optionName, _) -> os.Append(FSComp.SR.optsInternalNoDescription optionName)

        | DeprecatedCommandLineOptionNoDescription(optionName, _) -> os.Append(FSComp.SR.optsDCLONoDescription optionName)

        | HashIncludeNotAllowedInNonScript _ -> os.Append(HashIncludeNotAllowedInNonScriptE().Format)

        | HashReferenceNotAllowedInNonScript _ -> os.Append(HashReferenceNotAllowedInNonScriptE().Format)

        | HashDirectiveNotAllowedInNonScript _ -> os.Append(HashDirectiveNotAllowedInNonScriptE().Format)

        | FileNameNotResolved(fileName, locations, _) -> os.Append(FileNameNotResolvedE().Format fileName locations)

        | AssemblyNotResolved(originalName, _) -> os.Append(AssemblyNotResolvedE().Format originalName)

        | IllegalFileNameChar(fileName, invalidChar) ->
            os.Append(FSComp.SR.buildUnexpectedFileNameCharacter (fileName, string invalidChar) |> snd)

        | HashLoadedSourceHasIssues(infos, warnings, errors, _) ->

            match warnings, errors with
            | _, e :: _ ->
                os.Append(HashLoadedSourceHasIssues2E().Format)
                e.Output(os, suggestNames)
            | e :: _, _ ->
                os.Append(HashLoadedSourceHasIssues1E().Format)
                e.Output(os, suggestNames)
            | [], [] ->
                os.Append(HashLoadedSourceHasIssues0E().Format)
                infos.Head.Output(os, suggestNames)

        | HashLoadedScriptConsideredSource _ -> os.Append(HashLoadedScriptConsideredSourceE().Format)

        | InvalidInternalsVisibleToAssemblyName(badName, fileNameOption) ->
            match fileNameOption with
            | Some file -> os.Append(InvalidInternalsVisibleToAssemblyName1E().Format badName file)
            | None -> os.Append(InvalidInternalsVisibleToAssemblyName2E().Format badName)

        | LoadedSourceNotFoundIgnoring(fileName, _) -> os.Append(LoadedSourceNotFoundIgnoringE().Format fileName)

        | MSBuildReferenceResolutionWarning(code, message, _)

        | MSBuildReferenceResolutionError(code, message, _) -> os.Append(MSBuildReferenceResolutionErrorE().Format message code)

        | ArgumentsInSigAndImplMismatch(sigArg, implArg) ->
            os.Append(ArgumentsInSigAndImplMismatchE(), RichText.mkParameter sigArg.idText, RichText.mkParameter implArg.idText)

        | DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer(denv, implTycon, _sigTycon, implTypeAbbrev, sigTypeAbbrev, _m) ->
            let s1, s2, _ =
                NicePrint.minimalRichTextsOfTwoTypes denv implTypeAbbrev sigTypeAbbrev

            os.Append(
                DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferE(),
                RichText.mkText (implTycon.TypeOrMeasureKind.ToString()),
                richTextOfEntity implTycon,
                s1,
                s2
            )

        | InvalidAttributeTargetForLanguageElement(elementTargets, allowedTargets, _m) ->
            if Array.isEmpty elementTargets then
                os.Append(InvalidAttributeTargetForLanguageElement2E().Format)
            else
                let elementTargets = String.concat ", " elementTargets
                let allowedTargets = allowedTargets |> String.concat ", "
                os.Append(InvalidAttributeTargetForLanguageElement1E().Format elementTargets allowedTargets)

        | NoConstructorsAvailableForType(t, denv, _) -> os.Append(NoConstructorsAvailableForTypeE(), NicePrint.minimalRichTextOfType denv t)

        // Strip TargetInvocationException wrappers
        | :? TargetInvocationException as e when isNotNull e.InnerException -> (!!e.InnerException).Output(os, suggestNames)

        | :? FileNotFoundException as exn -> os.Append exn.Message

        | :? DirectoryNotFoundException as exn -> os.Append exn.Message

        | :? ArgumentException as exn -> os.Append exn.Message

        | :? NotSupportedException as exn -> os.Append exn.Message

        | :? IOException as exn -> os.Append exn.Message

        | :? UnauthorizedAccessException as exn -> os.Append exn.Message

        | :? InvalidOperationException as exn when exn.Message.Contains "ControlledExecution.Run" -> os.Append exn.Message

        | exn ->
            os.Append(TargetInvocationExceptionWrapperE().Format exn.Message)
#if DEBUG
            os.Append(sprintf "\nStack Trace\n%s\n" (exn.ToString()))

            if showAssertForUnexpectedException.Value then
                Debug.Assert(false, sprintf "Unknown exception seen in compiler: %s" (exn.ToString()))
#endif

/// Eagerly format a PhasedDiagnostic to a DiagnosticWithText
type PhasedDiagnostic with

    // remove any newlines and tabs
    member x.FormatRichCore(flattenErrors: bool, suggestNames: bool) =
        let buf = RichTextBuilder()

        x.Exception.Output(buf, suggestNames)

        let text = buf.ToRichText()

        if flattenErrors then NormalizeErrorRichText text else text

    member x.FormatCore(flattenErrors: bool, suggestNames: bool) = x.FormatRichCore(flattenErrors, suggestNames).Text

    member x.EagerlyFormatCore(suggestNames: bool) =
        match x.Range with
        | Some m ->
            let message = x.FormatRichCore(false, suggestNames)
            let exn = DiagnosticWithText(x.Number, message, m)
            { x with Exception = exn }
        | None -> x

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
        Context: string option
        DiagnosticStyle: DiagnosticStyle
    }

[<RequireQualifiedAccess>]
type FormattedDiagnostic =
    | Short of FSharpDiagnosticSeverity * string
    | Long of FSharpDiagnosticSeverity * FormattedDiagnosticDetailedInfo

let FormatDiagnosticLocation (tcConfig: TcConfig) (m: Range) : FormattedDiagnosticLocation =
    if Range.equals m rangeStartup || Range.equals m rangeCmdArgs then
        {
            Range = m
            TextRepresentation = ""
            IsEmpty = true
            File = ""
        }
    else
        let m = m.ApplyLineDirectives()
        let file = m.FileName

        let file =
            if tcConfig.showFullPaths then
                FileSystem.GetFullFilePathInDirectoryShim tcConfig.implicitIncludeDir file
            else
                SanitizeFileName file tcConfig.implicitIncludeDir

        let text, m, file =
            match tcConfig.diagnosticStyle with
            | DiagnosticStyle.Emacs ->
                let file = file.Replace("\\", "/")
                (sprintf "File \"%s\", line %d, characters %d-%d: " file m.StartLine m.StartColumn m.EndColumn), m, file

            // We're adjusting the columns here to be 1-based - both for parity with C# and for MSBuild, which assumes 1-based columns for error output
            | DiagnosticStyle.Default ->
                let file = file.Replace('/', Path.DirectorySeparatorChar)
                let m = withStart (mkPos m.StartLine (m.StartColumn + 1)) m
                (sprintf "%s(%d,%d): " file m.StartLine m.StartColumn), m, file

            // We may also want to change Test to be 1-based
            | DiagnosticStyle.Test ->
                let file = file.Replace("/", "\\")

                let m =
                    withStartEnd (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1)) m

                sprintf "%s(%d,%d-%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file

            | DiagnosticStyle.Gcc ->
                let file = file.Replace('/', Path.DirectorySeparatorChar)

                let m =
                    withStartEnd (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1)) m

                sprintf "%s:%d:%d: " file m.StartLine m.StartColumn, m, file

            // Here, we want the complete range information so Project Systems can generate proper squiggles
            | DiagnosticStyle.VisualStudio ->
                // Show prefix only for real files. Otherwise, we just want a truncated error like:
                //      parse error FS0031: blah blah
                if
                    not (equals m range0)
                    && not (equals m rangeStartup)
                    && not (equals m rangeCmdArgs)
                then
                    let file = file.Replace("/", "\\")

                    let m =
                        withStartEnd (mkPos m.StartLine (m.StartColumn + 1)) (mkPos m.EndLine (m.EndColumn + 1)) m

                    sprintf "%s(%d,%d,%d,%d): " file m.StartLine m.StartColumn m.EndLine m.EndColumn, m, file
                else
                    "", m, file
            | DiagnosticStyle.Rich ->
                let file = file.Replace('/', Path.DirectorySeparatorChar)
                let m = withStart (mkPos m.StartLine (m.StartColumn + 1)) m
                (sprintf "\n  --> %s (%d,%d)" file m.StartLine m.StartColumn), m, file

        {
            Range = m
            TextRepresentation = text
            IsEmpty = false
            File = file
        }

/// returns sequence that contains Diagnostic for the given error + Diagnostic for all related errors
let CollectFormattedDiagnostics (tcConfig: TcConfig, severity: FSharpDiagnosticSeverity, diagnostic: PhasedDiagnostic, suggestNames: bool) =

    match diagnostic.Exception with
    | ReportedError _ ->
        assert ("" = "Unexpected ReportedError") //  this should never happen
        [||]
    | StopProcessing ->
        assert ("" = "Unexpected StopProcessing") // this should never happen
        [||]
    | _ ->
        let errors = ResizeArray()

        let report (diagnostic: PhasedDiagnostic) =
            let where =
                match diagnostic.Range with
                | Some m -> FormatDiagnosticLocation tcConfig m |> Some
                | None -> None

            let subcategory = diagnostic.Subcategory()
            let errorNumber = diagnostic.Number

            let message =
                match severity with
                | FSharpDiagnosticSeverity.Error -> "error"
                | FSharpDiagnosticSeverity.Warning -> "warning"
                | FSharpDiagnosticSeverity.Info
                | FSharpDiagnosticSeverity.Hidden -> "info"

            let text =
                match tcConfig.diagnosticStyle with
                // Show the subcategory for --vserrors so that we can fish it out in Visual Studio and use it to determine error stickiness.
                | DiagnosticStyle.Emacs
                | DiagnosticStyle.Gcc
                | DiagnosticStyle.Default
                | DiagnosticStyle.Test -> sprintf "%s FS%04d: " message errorNumber
                | DiagnosticStyle.VisualStudio -> sprintf "%s %s FS%04d: " subcategory message errorNumber
                | DiagnosticStyle.Rich -> sprintf "%s FS%04d: " message errorNumber

            let canonical: FormattedDiagnosticCanonicalInformation =
                {
                    ErrorNumber = errorNumber
                    Subcategory = subcategory
                    TextRepresentation = text
                }

            let message =
                match tcConfig.diagnosticStyle with
                | DiagnosticStyle.Emacs
                | DiagnosticStyle.Gcc
                | DiagnosticStyle.Default
                | DiagnosticStyle.Test
                | DiagnosticStyle.Rich
                | DiagnosticStyle.VisualStudio -> diagnostic.FormatCore(tcConfig.flatErrors, suggestNames)

            let context =
                match tcConfig.diagnosticStyle with
                | DiagnosticStyle.Emacs
                | DiagnosticStyle.Gcc
                | DiagnosticStyle.Default
                | DiagnosticStyle.Test
                | DiagnosticStyle.VisualStudio -> None
                | DiagnosticStyle.Rich ->
                    match diagnostic.Range with
                    | Some m ->
                        let m = m.ApplyLineDirectives()

                        let content =
                            m.FileName
                            |> FileSystem.GetFullFilePathInDirectoryShim tcConfig.implicitIncludeDir
                            |> File.ReadAllLines

                        if m.StartLine = m.EndLine then
                            $"\n  {m.StartLine} | {content[m.StartLine - 1]}\n"
                            + $"""{String.make (m.StartColumn + 6) ' '}{String.make (m.EndColumn - m.StartColumn) '^'}"""
                            |> Some
                        else
                            content
                            |> fun lines -> Array.sub lines (m.StartLine - 1) (m.EndLine - m.StartLine - 1)
                            |> Array.fold
                                (fun (context, lineNumber) line -> (context + $"\n{lineNumber} | {line}", lineNumber + 1))
                                ("", m.StartLine)
                            |> fst
                            |> Some
                    | None -> None

            let entry: FormattedDiagnosticDetailedInfo =
                {
                    Location = where
                    Context = context
                    Canonical = canonical
                    Message = message
                    DiagnosticStyle = tcConfig.diagnosticStyle
                }

            errors.Add(FormattedDiagnostic.Long(severity, entry))

        match diagnostic.Exception with
#if !NO_TYPEPROVIDERS
        | :? TypeProviderError as tpe -> tpe.Iter(fun exn -> report { diagnostic with Exception = exn })
#endif
        | _ -> report diagnostic

        errors.ToArray()

type PhasedDiagnostic with

    /// used by fsc.exe and fsi.exe, but not by VS
    /// prints error and related errors to the specified StringBuilder
    member diagnostic.Output(buf, tcConfig: TcConfig, severity) =

        // 'true' for "canSuggestNames" is passed last here because we want to report suggestions in fsc.exe and fsi.exe, just not in regular IDE usage.
        let diagnostics = CollectFormattedDiagnostics(tcConfig, severity, diagnostic, true)

        for e in diagnostics do
            Printf.bprintf buf "\n"

            match e with
            | FormattedDiagnostic.Short(_, txt) -> buf.AppendString txt
            | FormattedDiagnostic.Long(_, details) ->
                match details.DiagnosticStyle with
                | DiagnosticStyle.Emacs
                | DiagnosticStyle.Gcc
                | DiagnosticStyle.Test
                | DiagnosticStyle.VisualStudio
                | DiagnosticStyle.Default ->
                    match details.Location with
                    | Some l when not l.IsEmpty ->
                        buf.AppendString l.TextRepresentation

                        if details.Context.IsSome then
                            buf.AppendString details.Context.Value
                    | _ -> ()

                    buf.AppendString details.Canonical.TextRepresentation
                    buf.AppendString details.Message
                | DiagnosticStyle.Rich ->
                    buf.AppendString details.Canonical.TextRepresentation
                    buf.AppendString details.Message

                    match details.Location with
                    | Some l when not l.IsEmpty ->
                        buf.AppendString l.TextRepresentation

                        if details.Context.IsSome then
                            buf.AppendString details.Context.Value
                    | _ -> ()

    member diagnostic.OutputContext(buf, prefix, fileLineFunction) =
        match diagnostic.Range with
        | None -> ()
        | Some m ->
            let m = m.ApplyLineDirectives()
            let fileName = m.FileName
            let lineA = m.StartLine
            let lineB = m.EndLine
            let line = fileLineFunction fileName lineA

            if line <> "" then
                let iA = m.StartColumn
                let iB = m.EndColumn
                let iLen = if lineA = lineB then max (iB - iA) 1 else 1
                Printf.bprintf buf "%s%s\n" prefix line
                Printf.bprintf buf "%s%s%s\n" prefix (String.make iA '-') (String.make iLen '^')

    member diagnostic.WriteWithContext(os, prefix, fileLineFunction, tcConfig, severity) =
        writeViaBuffer os (fun buf ->
            diagnostic.OutputContext(buf, prefix, fileLineFunction)
            diagnostic.Output(buf, tcConfig, severity))

/// Build an DiagnosticsLogger that delegates to another DiagnosticsLogger but filters warnings
type DiagnosticsLoggerFilteringByScopedNowarn(diagnosticOptions: FSharpDiagnosticOptions, diagnosticsLogger: DiagnosticsLogger) =
    inherit DiagnosticsLogger("DiagnosticsLoggerFilteringByScopedNowarn")

    let mutable realErrorPresent = false

    override _.DiagnosticSink(diagnostic: PhasedDiagnostic) =

        if diagnostic.Severity = FSharpDiagnosticSeverity.Error then
            realErrorPresent <- true
            diagnosticsLogger.DiagnosticSink(diagnostic)
        else
            match diagnostic.AdjustSeverity(diagnosticOptions) with
            | FSharpDiagnosticSeverity.Hidden -> ()
            | s -> diagnosticsLogger.DiagnosticSink({ diagnostic with Severity = s })

    override _.ErrorCount = diagnosticsLogger.ErrorCount

    override _.CheckForRealErrorsIgnoringWarnings = realErrorPresent

let GetDiagnosticsLoggerFilteringByScopedNowarn (diagnosticOptions, diagnosticsLogger) =
    DiagnosticsLoggerFilteringByScopedNowarn(diagnosticOptions, diagnosticsLogger) :> DiagnosticsLogger
