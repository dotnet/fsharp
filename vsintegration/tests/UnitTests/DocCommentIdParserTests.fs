// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
[<NUnit.Framework.TestFixture>]
module Tests.ServiceAnalysis.DocCommentIdParser

open NUnit.Framework
open Microsoft.VisualStudio.FSharp.Editor




[<Test>]
let ``Test DocCommentId parser``() =
    let testData = dict [
        "T:N.X.Nested", DocCommentId.Type ["N"; "X"; "Nested"];
        "M:N.X.#ctor", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "``.ctor``"; GenericParameters = 0 }, SymbolMemberType.Constructor);
        "M:N.X.#ctor(System.Int32)", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "``.ctor``"; GenericParameters = 0 }, SymbolMemberType.Constructor);
        "M:N.X.f", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "f"; GenericParameters = 0 }, SymbolMemberType.Method);
        "M:N.X.bb(System.String,System.Int32@)", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "bb"; GenericParameters = 0 }, SymbolMemberType.Method);
        "M:N.X.gg(System.Int16[],System.Int32[0:,0:])", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "gg"; GenericParameters = 0 }, SymbolMemberType.Method);
        "M:N.X.op_Addition(N.X,N.X)", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "op_Addition"; GenericParameters = 0 }, SymbolMemberType.Method);
        "M:N.X.op_Explicit(N.X)~System.Int32", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "op_Explicit"; GenericParameters = 0 }, SymbolMemberType.Method);
        "M:N.GenericMethod.WithNestedType``1(N.GenericType{``0}.NestedType)", DocCommentId.Member ({ EntityPath = ["N"; "GenericMethod"]; MemberOrValName = "WithNestedType"; GenericParameters = 1 }, SymbolMemberType.Method);
        "M:N.GenericMethod.WithIntOfNestedType``1(N.GenericType{System.Int32}.NestedType)", DocCommentId.Member ({ EntityPath = ["N"; "GenericMethod"]; MemberOrValName = "WithIntOfNestedType"; GenericParameters = 1 }, SymbolMemberType.Method);
        "E:N.X.d", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "d"; GenericParameters = 0 }, SymbolMemberType.Event);
        "F:N.X.q", DocCommentId.Field { EntityPath = ["N"; "X"]; MemberOrValName = "q"; GenericParameters = 0 };
        "P:N.X.prop", DocCommentId.Member ({ EntityPath = ["N"; "X"]; MemberOrValName = "prop"; GenericParameters = 0 }, SymbolMemberType.Property);
    ]

    let mutable res = ""
    for pair in testData do
        let docId = pair.Key
        let expected = pair.Value
        let actual = FSharpCrossLanguageSymbolNavigationService.DocCommentIdToPath(docId)
        if actual <> expected then
            res <- res + $"DocumentId: {docId}; Expected = %A{expected} = Actual = %A{actual}\n"

    if res <> "" then
        failwith res
    ()