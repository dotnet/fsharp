// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
[<NUnit.Framework.TestFixture>]
module Tests.ServiceAnalysis.DocCommentIdParser

open NUnit.Framework
open Microsoft.VisualStudio.FSharp.Editor


[<Test>]
let ``Test DocCommentId parser``() =
    let testData = dict [
        "T:N.X.Nested", 0;
        "M:N.X.#ctor", 0;
        "M:N.X.#ctor(System.Int32)", 0;
        "M:N.X.f", 0;
        "M:N.X.bb(System.String,System.Int32@)", 0;
        "M:N.X.gg(System.Int16[],System.Int32[0:,0:])", 0;
        "M:N.X.op_Addition(N.X,N.X)", 0;
        "M:N.X.op_Explicit(N.X)~System.Int32", 0;
        "M:N.GenericMethod.WithNestedType``1(N.GenericType{``0}.NestedType)", 0;
        "M:N.GenericMethod.WithIntOfNestedType``1(N.GenericType{System.Int32}.NestedType)", 0;
        "M:N.X.N#IX{N#KVP{System#String,System#Int32}}#IXA(N.KVP{System.String,System.Int32})", 0;
        "E:N.X.d", 0;
        "F:N.X.q", 0;
        "P:N.X.prop", 0;
    ]

    for pair in testData do
        let docId = pair.Key
        let expected = pair.Value
        let actual = FSharpCrossLanguageSymbolNavigationService.DocCommentIdToPath(docId)
        printfn $"{docId} = {expected} = %A{actual}"