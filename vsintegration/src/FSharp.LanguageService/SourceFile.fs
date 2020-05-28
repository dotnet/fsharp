// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//------- DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS ---------------

//------- DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS ---------------

namespace FSharp.Compiler.VsLanguageService
open System
open System.IO
open FSharp.Compiler

/// Methods for dealing with F# sources files.
module (*internal*) SourceFile =

    /// Source file extensions
    let private compilableExtensions = Build.sigSuffixes @ Build.implSuffixes @ Build.scriptSuffixes
    
    /// Single file projects extensions
    let private singleFileProjectExtensions = Build.scriptSuffixes

    /// Whether or not this file is compilable    
    let IsCompilable file =
        let ext = Path.GetExtension(file)
        compilableExtensions |> List.exists(fun e->0 = String.Compare(e,ext,StringComparison.OrdinalIgnoreCase))
        
    /// Whether or not this file should be a single-file project
    let MustBeSingleFileProject file =
        let ext = Path.GetExtension(file)
        singleFileProjectExtensions |> List.exists(fun e-> 0 = String.Compare(e,ext,StringComparison.OrdinalIgnoreCase))            
