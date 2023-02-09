// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.CheckPatterns

open FSharp.Compiler.CheckBasics
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypedTree
open FSharp.Compiler.PatternMatchCompilation
open FSharp.Compiler.Syntax

/// Check a set of simple patterns, e.g. the declarations of parameters for an implicit constructor.
val TcSimplePatsOfUnknownType:
    cenv: TcFileState ->
    optionalArgsOK: bool ->
    checkConstraints: CheckConstraints ->
    env: TcEnv ->
    tpenv: UnscopedTyparEnv ->
    synSimplePats: SynSimplePats ->
        string list * TcPatLinearEnv

// Check a pattern, e.g. for a binding or a match clause
val TcPat:
    warnOnUpper: WarnOnUpperFlag ->
    cenv: TcFileState ->
    env: TcEnv ->
    valReprInfo: PrelimValReprInfo option ->
    vFlags: TcPatValFlags ->
    patEnv: TcPatLinearEnv ->
    ty: TType ->
    synPat: SynPat ->
        (TcPatPhase2Input -> Pattern) * TcPatLinearEnv

// Check a list of simple patterns, e.g. for the arguments of a function or a class constructor
val TcSimplePats:
    cenv: TcFileState ->
    optionalArgsOK: bool ->
    checkConstraints: CheckConstraints ->
    ty: TType ->
    env: TcEnv ->
    patEnv: TcPatLinearEnv ->
    synSimplePats: SynSimplePats ->
        string list * TcPatLinearEnv
