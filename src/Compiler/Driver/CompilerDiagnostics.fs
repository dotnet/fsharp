// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Contains logic to prepare, post-process, filter and emit compiler diagnsotics
module internal FSharp.Compiler.CompilerDiagnostics

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Text

open FSharp.Compiler.CheckPatterns
open Internal.Utilities.Library.Extras
open Internal.Utilities.Library
open Internal.Utilities.Text

open FSharp.Compiler
open FSharp.Compiler.AttributeChecking
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

type Exception with

    member exn.DiagnosticRange =
        match exn with
        | ErrorFromAddingConstraint (_, exn2, _) -> exn2.DiagnosticRange
#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange exn -> exn.DiagnosticRange
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
        | NotUpperCaseConstructorWithoutRQA m
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
        | PatternMatchCompilation.MatchNotAllowedForUnionCaseWithNoData m
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
        | :? System.Reflection.TargetInvocationException as e -> e.InnerException.DiagnosticRange
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
        | PatternMatchCompilation.MatchNotAllowedForUnionCaseWithNoData _
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
        | NotUpperCaseConstructorWithoutRQA _ -> 53
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

        // Strip TargetInvocationException wrappers
        | :? TargetInvocationException as e -> e.InnerException.DiagnosticNumber
        | WrappedError (e, _) -> e.DiagnosticNumber
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

type PhasedDiagnostic with

    member x.Range = x.Exception.DiagnosticRange

    member x.Number = x.Exception.DiagnosticNumber

    member x.WarningLevel =
        match x.Exception with
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

    member x.IsEnabled(severity, options) =
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
        | 3366 -> false //tcIndexNotationDeprecated - currently off by default
        | 3517 -> false // optFailedToInlineSuggestedValue - off by default
        | 3388 -> false // tcSubsumptionImplicitConversionUsed - off by default
        | 3389 -> false // tcBuiltInImplicitConversionUsed - off by default
        | 3390 -> false // xmlDocBadlyFormed - off by default
        | 3395 -> false // tcImplicitConversionUsedForMethodArg - off by default
        | _ ->
            (severity = FSharpDiagnosticSeverity.Info)
            || (severity = FSharpDiagnosticSeverity.Warning && level >= x.WarningLevel)

    /// Indicates if a diagnostic should be reported as an informational
    member x.ReportAsInfo(options, severity) =
        match severity with
        | FSharpDiagnosticSeverity.Error -> false
        | FSharpDiagnosticSeverity.Warning -> false
        | FSharpDiagnosticSeverity.Info -> x.IsEnabled(severity, options) && not (List.contains x.Number options.WarnOff)
        | FSharpDiagnosticSeverity.Hidden -> false

    /// Indicates if a diagnostic should be reported as a warning
    member x.ReportAsWarning(options, severity) =
        match severity with
        | FSharpDiagnosticSeverity.Error -> false

        | FSharpDiagnosticSeverity.Warning -> x.IsEnabled(severity, options) && not (List.contains x.Number options.WarnOff)

        // Informational become warning if explicitly on and not explicitly off
        | FSharpDiagnosticSeverity.Info ->
            let n = x.Number
            List.contains n options.WarnOn && not (List.contains n options.WarnOff)

        | FSharpDiagnosticSeverity.Hidden -> false

    /// Indicates if a diagnostic should be reported as an error
    member x.ReportAsError(options, severity) =

        match severity with
        | FSharpDiagnosticSeverity.Error -> true

        // Warnings become errors in some situations
        | FSharpDiagnosticSeverity.Warning ->
            let n = x.Number

            x.IsEnabled(severity, options)
            && not (List.contains n options.WarnAsWarn)
            && ((options.GlobalWarnAsError && not (List.contains n options.WarnOff))
                || List.contains n options.WarnAsError)

        // Informational become errors if explicitly WarnAsError
        | FSharpDiagnosticSeverity.Info -> List.contains x.Number options.WarnAsError

        | FSharpDiagnosticSeverity.Hidden -> false

[<AutoOpen>]
module OldStyleMessages =
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
    let MatchNotAllowedForUnionCaseWithNoDataE () = Message("MatchNotAllowedForUnionCaseWithNoData", "")
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

let (|InvalidArgument|_|) (exn: exn) =
    match exn with
    | :? ArgumentException as e -> Some e.Message
    | _ -> None

let OutputNameSuggestions (os: StringBuilder) suggestNames suggestionsF idText =
    if suggestNames then
        let buffer = DiagnosticResolutionHints.SuggestionBuffer idText

        if not buffer.Disabled then
            suggestionsF buffer.Add

            if not buffer.IsEmpty then
                os.AppendString " "
                os.AppendString(FSComp.SR.undefinedNameSuggestionsIntro ())

                for value in buffer do
                    os.AppendLine() |> ignore
                    os.AppendString "   "
                    os.AppendString(ConvertValLogicalNameToDisplayNameCore value)

type Exception with

    member exn.Output(os: StringBuilder, suggestNames) =

        match exn with
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
            e.Output(os, suggestNames)

        | ErrorFromAddingTypeEquation(error = ConstraintSolverTypesNotInSubsumptionRelation _ as e) -> e.Output(os, suggestNames)

        | ErrorFromAddingTypeEquation(error = ConstraintSolverError _ as e) -> e.Output(os, suggestNames)

        | ErrorFromAddingTypeEquation (g, denv, ty1, ty2, e, _) ->
            if not (typeEquiv g ty1 ty2) then
                let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2

                if ty1 <> ty2 + tpcs then
                    os.AppendString(ErrorFromAddingTypeEquation2E().Format ty1 ty2 tpcs)

            e.Output(os, suggestNames)

        | ErrorFromApplyingDefault (_, denv, _, defaultType, e, _) ->
            let defaultType = NicePrint.minimalStringOfType denv defaultType
            os.AppendString(ErrorFromApplyingDefault1E().Format defaultType)
            e.Output(os, suggestNames)
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
                        e.Output(os, suggestNames)
                else
                    e.Output(os, suggestNames)

        | UpperCaseIdentifierInPattern _ -> os.AppendString(UpperCaseIdentifierInPatternE().Format)

        | NotUpperCaseConstructor _ -> os.AppendString(NotUpperCaseConstructorE().Format)

        | NotUpperCaseConstructorWithoutRQA _ -> os.AppendString(NotUpperCaseConstructorWithoutRQAE().Format)

        | ErrorFromAddingConstraint (_, e, _) -> e.Output(os, suggestNames)

#if !NO_TYPEPROVIDERS
        | TypeProviders.ProvidedTypeResolutionNoRange e

        | TypeProviders.ProvidedTypeResolution (_, e) -> e.Output(os, suggestNames)

        | :? TypeProviderError as e -> os.AppendString(e.ContextualErrorMessage)
#endif

        | UnresolvedOverloading (denv, callerArgs, failure, m) ->

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

        | InterfaceNotRevealed (denv, intfTy, _) ->
            os.AppendString(InterfaceNotRevealedE().Format(NicePrint.minimalStringOfType denv intfTy))

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
                os.AppendString(Duplicate1E().Format(ConvertValLogicalNameToDisplayNameCore s))
            else
                os.AppendString(Duplicate2E().Format k (ConvertValLogicalNameToDisplayNameCore s))

        | UndefinedName (_, k, id, suggestionsF) ->
            os.AppendString(k (ConvertValLogicalNameToDisplayNameCore id.idText))
            OutputNameSuggestions os suggestNames suggestionsF id.idText

        | InternalUndefinedItemRef (f, smr, ccuName, s) ->
            let _, errs = f (smr, ccuName, s)
            os.AppendString errs

        | FieldNotMutable _ -> os.AppendString(FieldNotMutableE().Format)

        | FieldsFromDifferentTypes (_, fref1, fref2, _) ->
            os.AppendString(FieldsFromDifferentTypesE().Format fref1.FieldName fref2.FieldName)

        | VarBoundTwice id -> os.AppendString(VarBoundTwiceE().Format(ConvertValLogicalNameToDisplayNameCore id.idText))

        | Recursion (denv, id, ty1, ty2, _) ->
            let ty1, ty2, tpcs = NicePrint.minimalStringsOfTwoTypes denv ty1 ty2
            os.AppendString(RecursionE().Format (ConvertValLogicalNameToDisplayNameCore id.idText) ty1 ty2 tpcs)

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
                | Parser.TOKEN_DOT_DOT_HAT -> SR.GetString("Parser.TOKEN.DOT.DOT")
                | Parser.TOKEN_QUOTE -> SR.GetString("Parser.TOKEN.QUOTE")
                | Parser.TOKEN_STAR -> SR.GetString("Parser.TOKEN.STAR")
                | Parser.TOKEN_HIGH_PRECEDENCE_TYAPP -> SR.GetString("Parser.TOKEN.HIGH.PRECEDENCE.TYAPP")
                | Parser.TOKEN_COLON -> SR.GetString("Parser.TOKEN.COLON")
                | Parser.TOKEN_COLON_EQUALS -> SR.GetString("Parser.TOKEN.COLON.EQUALS")
                | Parser.TOKEN_LARROW -> SR.GetString("Parser.TOKEN.LARROW")
                | Parser.TOKEN_EQUALS -> SR.GetString("Parser.TOKEN.EQUALS")
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
                | Parser.TOKEN_RQUOTE_DOT _
                | Parser.TOKEN_RQUOTE -> SR.GetString("Parser.TOKEN.RQUOTE")
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
                | Parser.TOKEN_LET _
                | Parser.TOKEN_OLET _ -> SR.GetString("Parser.TOKEN.OLET")
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
                | Parser.TOKEN_HASH_LIGHT
                | Parser.TOKEN_HASH_LINE
                | Parser.TOKEN_HASH_IF
                | Parser.TOKEN_HASH_ELSE
                | Parser.TOKEN_HASH_ENDIF -> SR.GetString("Parser.TOKEN.HASH.ENDIF")
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
                | unknown ->
                    Debug.Assert(false, "unknown token tag")
                    let result = sprintf "%+A" unknown
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
                            | Some ty when isUnitTy g ty -> true
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
            os.AppendString(ConvertValLogicalNameToDisplayNameCore s)
            OutputNameSuggestions os suggestNames suggestionF idText

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

        | WrappedError (e, _) -> e.Output(os, suggestNames)

        | PatternMatchCompilation.MatchIncomplete (isComp, cexOpt, _) ->
            os.AppendString(MatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some (cex, false) -> os.AppendString(MatchIncomplete2E().Format cex)
            | Some (cex, true) -> os.AppendString(MatchIncomplete3E().Format cex)

            if isComp then
                os.AppendString(MatchIncomplete4E().Format)

        | PatternMatchCompilation.EnumMatchIncomplete (isComp, cexOpt, _) ->
            os.AppendString(EnumMatchIncomplete1E().Format)

            match cexOpt with
            | None -> ()
            | Some (cex, false) -> os.AppendString(MatchIncomplete2E().Format cex)
            | Some (cex, true) -> os.AppendString(MatchIncomplete3E().Format cex)

            if isComp then
                os.AppendString(MatchIncomplete4E().Format)

        | PatternMatchCompilation.RuleNeverMatched _ -> os.AppendString(RuleNeverMatchedE().Format)
        
        | PatternMatchCompilation.MatchNotAllowedForUnionCaseWithNoData _ ->
            os.AppendString(MatchNotAllowedForUnionCaseWithNoDataE().Format)

        | ValNotMutable (_, vref, _) -> os.AppendString(ValNotMutableE().Format(vref.DisplayName))

        | ValNotLocal _ -> os.AppendString(ValNotLocalE().Format)

        | ObsoleteError (s, _)

        | ObsoleteWarning (s, _) ->
            os.AppendString(Obsolete1E().Format)

            if s <> "" then
                os.AppendString(Obsolete2E().Format s)

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

            match warnings, errors with
            | _, e :: _ ->
                os.AppendString(HashLoadedSourceHasIssues2E().Format)
                e.Output(os, suggestNames)
            | e :: _, _ ->
                os.AppendString(HashLoadedSourceHasIssues1E().Format)
                e.Output(os, suggestNames)
            | [], [] ->
                os.AppendString(HashLoadedSourceHasIssues0E().Format)
                infos.Head.Output(os, suggestNames)

        | HashLoadedScriptConsideredSource _ -> os.AppendString(HashLoadedScriptConsideredSourceE().Format)

        | InvalidInternalsVisibleToAssemblyName (badName, fileNameOption) ->
            match fileNameOption with
            | Some file -> os.AppendString(InvalidInternalsVisibleToAssemblyName1E().Format badName file)
            | None -> os.AppendString(InvalidInternalsVisibleToAssemblyName2E().Format badName)

        | LoadedSourceNotFoundIgnoring (fileName, _) -> os.AppendString(LoadedSourceNotFoundIgnoringE().Format fileName)

        | MSBuildReferenceResolutionWarning (code, message, _)

        | MSBuildReferenceResolutionError (code, message, _) -> os.AppendString(MSBuildReferenceResolutionErrorE().Format message code)

        // Strip TargetInvocationException wrappers
        | :? TargetInvocationException as exn -> exn.InnerException.Output(os, suggestNames)

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

/// Eagerly format a PhasedDiagnostic to a DiagnosticWithText
type PhasedDiagnostic with

    // remove any newlines and tabs
    member x.OutputCore(os: StringBuilder, flattenErrors: bool, suggestNames: bool) =
        let buf = StringBuilder()

        x.Exception.Output(buf, suggestNames)

        let text =
            if flattenErrors then
                NormalizeErrorString(buf.ToString())
            else
                buf.ToString()

        os.AppendString text

    member x.FormatCore(flattenErrors: bool, suggestNames: bool) =
        let os = StringBuilder()
        x.OutputCore(os, flattenErrors, suggestNames)
        os.ToString()

    member x.EagerlyFormatCore(suggestNames: bool) =
        match x.Range with
        | Some m ->
            let buf = StringBuilder()
            x.Exception.Output(buf, suggestNames)
            let message = buf.ToString()
            let exn = DiagnosticWithText(x.Number, message, m)
            { Exception = exn; Phase = x.Phase }
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
    }

[<RequireQualifiedAccess>]
type FormattedDiagnostic =
    | Short of FSharpDiagnosticSeverity * string
    | Long of FSharpDiagnosticSeverity * FormattedDiagnosticDetailedInfo

let FormatDiagnosticLocation (tcConfig: TcConfig) m : FormattedDiagnosticLocation =
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
                    not (equals m range0)
                    && not (equals m rangeStartup)
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
                | DiagnosticStyle.VisualStudio -> sprintf "%s %s FS%04d: " subcategory message errorNumber
                | _ -> sprintf "%s FS%04d: " message errorNumber

            let canonical: FormattedDiagnosticCanonicalInformation =
                {
                    ErrorNumber = errorNumber
                    Subcategory = subcategory
                    TextRepresentation = text
                }

            let message = diagnostic.FormatCore(tcConfig.flatErrors, suggestNames)

            let entry: FormattedDiagnosticDetailedInfo =
                {
                    Location = where
                    Canonical = canonical
                    Message = message
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
            | FormattedDiagnostic.Short (_, txt) -> buf.AppendString txt |> ignore
            | FormattedDiagnostic.Long (_, details) ->
                match details.Location with
                | Some l when not l.IsEmpty -> buf.AppendString l.TextRepresentation
                | _ -> ()

                buf.AppendString details.Canonical.TextRepresentation
                buf.AppendString details.Message

    member diagnostic.OutputContext(buf, prefix, fileLineFunction) =
        match diagnostic.Range with
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
                Printf.bprintf buf "%s%s\n" prefix line
                Printf.bprintf buf "%s%s%s\n" prefix (String.make iA '-') (String.make iLen '^')

    member diagnostic.WriteWithContext(os, prefix, fileLineFunction, tcConfig, severity) =
        writeViaBuffer os (fun buf ->
            diagnostic.OutputContext(buf, prefix, fileLineFunction)
            diagnostic.Output(buf, tcConfig, severity))

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

    override _.DiagnosticSink(diagnostic: PhasedDiagnostic, severity) =
        if severity = FSharpDiagnosticSeverity.Error then
            diagnosticsLogger.DiagnosticSink(diagnostic, severity)
        else
            let report =
                let warningNum = diagnostic.Number

                match diagnostic.Range with
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
                if diagnostic.ReportAsError(diagnosticOptions, severity) then
                    diagnosticsLogger.DiagnosticSink(diagnostic, FSharpDiagnosticSeverity.Error)
                elif diagnostic.ReportAsWarning(diagnosticOptions, severity) then
                    diagnosticsLogger.DiagnosticSink(diagnostic, FSharpDiagnosticSeverity.Warning)
                elif diagnostic.ReportAsInfo(diagnosticOptions, severity) then
                    diagnosticsLogger.DiagnosticSink(diagnostic, severity)

    override _.ErrorCount = diagnosticsLogger.ErrorCount

let GetDiagnosticsLoggerFilteringByScopedPragmas (checkFile, scopedPragmas, diagnosticOptions, diagnosticsLogger) =
    DiagnosticsLoggerFilteringByScopedPragmas(checkFile, scopedPragmas, diagnosticOptions, diagnosticsLogger) :> DiagnosticsLogger
