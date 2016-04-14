// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// API to the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range

[<Sealed>]
type internal FSharpNoteworthyParamInfoLocations =
    member LongId : string list
    member LongIdStartLocation : pos
    member LongIdEndLocation : pos
    member OpenParenLocation : pos
    /// locations of commas and close parenthesis (or, last char of last arg, if no final close parenthesis)
    member TupleEndLocations : pos[]  
    /// false if either this is a call without parens "f x" or the parser recovered as in "f(x,y"
    member IsThereACloseParen : bool   
    /// empty or a name if an actual named parameter; f(0,a=4,?b=None) would be [|null;"a";"b"|]
    member NamedParamNames : string[]  

    static member Find : pos * Ast.ParsedInput -> FSharpNoteworthyParamInfoLocations option

