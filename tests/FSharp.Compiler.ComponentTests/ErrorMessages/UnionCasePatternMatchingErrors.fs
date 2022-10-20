namespace FSharp.Compiler.ComponentTests.ErrorMessages
module UnionCasePatternMatchingErrors = 

    open Xunit
    open FSharp.Test.Compiler

    [<Fact>]
    let ``Union matching error - Incomplete union fields`` () =
        FSharp """
    module Tests
    type U =
        | B of  f1:int list * {|X:string|} * f3:U * f4: (int * System.String)

    let x : U = failwith ""
    let myVal = 
        match x with
        | B  -> 42"""
        |> typecheck
        |> shouldFail   
        |> withSingleDiagnostic (Error 727, Line 9, Col 7, Line 9, Col 8,
                                    "This union case expects 4 arguments in tupled form, but was given 0. The missing field arguments may be any of:
    \tf1: int list
    \t{| X: string |}
    \tf3: U
    \tf4: (int * System.String)")

    [<Fact>]
    let ``Union matching error - Named args - Name used twice`` () =
        FSharp """
    module Tests
    type U =    
        | B of field: int * int
    let x : U = failwith ""
    let myVal = 
        match x with
        | B (field = x; field = z) -> let y = x + z + 1 in ()"""
        |> typecheck
        |> shouldFail   
        |> withSingleDiagnostic (Error 3175, Line 8, Col 21, Line 8, Col 26, "Union case/exception field 'field' cannot be used more than once.")

    [<Fact>]
    let ``Union matching error - Multiple tupled args`` () =
        FSharp """
    module Tests
    type U =
        | B of field: int * int

    let x : U = failwith ""
    let myVal = 
        match x with
        | B x z -> let y = x + z + 1 in ()"""
        |> typecheck
        |> shouldFail   
        |> withSingleDiagnostic (Error 727, Line 9, Col 7, Line 9, Col 12, "This union case expects 2 arguments in tupled form, but was given 0. The missing field arguments may be any of:
    \tfield: int
    \tint")

    [<Fact>]
    let ``Union matching error - Missing field`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int

    let myVal = 
        match A with
        | A -> 15
        | B (x, _) -> 16"""
        |> typecheck
        |> shouldFail   
        |> withSingleDiagnostic (Error 727, Line 10, Col 7, Line 10, Col 15, "This union case expects 3 arguments in tupled form, but was given 2. The missing field arguments may be any of:
    \tint")
      
    [<Fact>]
    let ``Union Pattern discard not allowed for union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type X = X

    let x: X = X

    let myVal =
        match x with
        | X _ -> ()"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3548, Line 9, Col 7, Line 9, Col 10, "Pattern discard is not allowed for union case that takes no data.")
    
      
    [<Fact>]
    let ``Union Pattern discard allowed for union case that takes no data with Lang version 7`` () =
         FSharp """
    module Tests
    type X = X

    let x: X = X

    let myVal =
        match x with
        | X _ -> ()"""
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed
    
    [<Fact>]
    let ``Union function Pattern discard allowed for union case that takes no data with Lang version 7`` () =
         FSharp """
    module Tests
    type X = X

    let x: X = X

    let myVal =
        function
        | X _ -> ()"""
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed
    
    [<Fact>]
    let ``Union function Pattern discard not allowed for union case that takes no data with Lang version preview`` () =
         FSharp """
    module Tests
    type X = X

    let x: X = X

    let myVal =
        function
        | X _ -> ()"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3548, Line 9, Col 7, Line 9, Col 10, "Pattern discard is not allowed for union case that takes no data.")
    
    [<Fact>]
    let ``Pattern discard not allowed for union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int
        | C of int * int * int
    
    let a : U = A

    let myVal = 
        match a with
        | A _ -> 15
        | B (x, _, _) -> 16
        | C _ -> 17"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3548, Line 12, Col 7, Line 12, Col 10, "Pattern discard is not allowed for union case that takes no data.")

    [<Fact>]
    let ``Pattern function discard not allowed for union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int
        | C of int * int * int
    
    let a : U = A

    let myVal = 
        function
        | A _ -> 15
        | B (x, _, _) -> 16
        | C _ -> 17"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3548, Line 12, Col 7, Line 12, Col 10, "Pattern discard is not allowed for union case that takes no data.")
    
    [<Fact>]
    let ``Pattern discard allowed for union case that takes no data with Lang version 7`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int
        | C of int * int * int
    
    let a : U = A

    let myVal = 
        match a with
        | A _ -> 15
        | B (x, _, _) -> 16
        | C _ -> 17"""
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed
 
    [<Fact>]
    let ``Grouped Pattern discard not allowed for union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int
        | C of int * int * int
    
    let a : U = A

    let myVal = 
        match a with
        | A _
        | B _
        | C _ -> 17"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3548, Line 12, Col 7, Line 12, Col 10, "Pattern discard is not allowed for union case that takes no data.")

    [<Fact>]
    let ``Multiple pattern discards not allowed for union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int
        | C of int * int * int
    
    type V =
        | D
    
    let a : U = A
    let d : V = D
    
    let myVal = 
        match a, d with
        | A _, D -> 15
        | B (x, _, _), D _ -> 16
        | C _, _ -> 17"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3548, Line 16, Col 7, Line 16, Col 10, "Pattern discard is not allowed for union case that takes no data.")
            (Warning 3548, Line 17, Col 20, Line 17, Col 23, "Pattern discard is not allowed for union case that takes no data.")
        ]
    
    [<Fact>]
    let ``Multiple function pattern discards is not allowed for union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type U =
        | A
        | B of int * int * int
        | C of int * int * int
    
    type V =
        | D
    
    let a : U = A
    let d : V = D
    
    let myVal = 
        function
        | A _, D -> 15
        | B (x, _, _), D _ -> 16
        | C _, _ -> 17"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3548, Line 16, Col 7, Line 16, Col 10, "Pattern discard is not allowed for union case that takes no data.")
            (Warning 3548, Line 17, Col 20, Line 17, Col 23, "Pattern discard is not allowed for union case that takes no data.")
        ]
    
    [<Fact>]
    let ``Pattern discard allowed for single-case unions when using them as a deconstruct syntax in functions  with Lang 7`` () =
         FSharp """
    module Tests
    type MyWrapper = A

    let myDiscardedArgFunc(A _) = 5+5"""
        |> withLangVersion70
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Pattern named not allowed for single-case unions when using them as a deconstruct syntax in functions  with Lang 7`` () =
         FSharp """
    module Tests
    type MyWrapper = A

    let myFunc(A a) = 5+5"""
        |> withLangVersion70
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 725, Line 5, Col 12, Line 5, Col 15, "This union case does not take arguments")
        ]

    [<Fact>]
    let ``Pattern discard or named are not allowed for single-case union case that takes no data with Lang preview`` () =
         FSharp """
    module Tests
    type MyWrapper = A

    let myFunc(A a) = 5+5
    let myDiscardedArgFunc(A _) = 5+5"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 3548, Line 5, Col 12, Line 5, Col 15, "Pattern discard is not allowed for union case that takes no data.")
            (Warning 3548, Line 6, Col 24, Line 6, Col 27, "Pattern discard is not allowed for union case that takes no data.")
        ]