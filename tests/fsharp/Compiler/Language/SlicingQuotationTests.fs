namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module SlicingQuotationTests =
    [<Test>]
    let ``Reverse slicing quotation on array with range return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = [|1;2;3|]
    let q = <@ xs.[0..^1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call(None, GetArraySlice, [ValueWithName([|1;2;3|], xs), NewUnionCase(Some, Value(0)), NewUnionCase(Some, Call(None, []`1.GetReverseIndex, [ValueWithName([|1;2;3|], xs), Value(0), Value(1)]))])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []


    [<Test>]
    let ``Reverse slicing quotation on array2d with ranges return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = array2D [ [1;2;3]; ]
    let q = <@ xs.[0..^1, ^1..^2] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (None, GetArraySlice2D, [ValueWithName ([[1; 2; 3]], xs), NewUnionCase (Some, Value (0)), NewUnionCase (Some, Call(None, [,]`1.GetReverseIndex, [ValueWithName([[1;2;3]],xs), Value(0), Value(1)])), NewUnionCase (Some, Call(None, [,]`1.GetReverseIndex, [ValueWithName([[1;2;3]], xs), Value(1), Value(1)])), NewUnionCase (Some, Call(None, [,]`1.GetReverseIndex, [ValueWithName([[1;2;3]], xs), Value(1), Value(2)]))])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Reverse slicing quotation on array2d with fixed index return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = array2D [ [1;2;3]; ]
    let q = <@ xs.[0..^1, ^1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (None, GetArraySlice2DFixed2, [ValueWithName ([[1; 2; 3]], xs), NewUnionCase (Some, Value (0)), NewUnionCase (Some, Call(None, [,]`1.GetReverseIndex,[ValueWithName([[1;2;3]], xs), Value(0), Value(1)])), Call(None, [,]`1.GetReverseIndex,[ValueWithName([[1;2;3]], xs), Value(1), Value(1)])])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Reverse indexing quotation on array2d return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = array2D [ [1;2;3]; ]
    let q = <@ xs.[^0, 1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call(None, GetArray2D, [ValueWithName([[1;2;3]], xs), Call(None, [,]`1.GetReverseIndex, [ValueWithName([[1;2;3]], xs), Value(0), Value(0)]), Value(1)])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Reverse slicing quotation on list with range return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = [1;2;3]
    let q = <@ xs.[^0..1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (Some (ValueWithName ([1; 2; 3], xs)), GetSlice, [NewUnionCase (Some, Call(Some(ValueWithName([1;2;3], xs)), GetReverseIndex, [Value(0), Value(0)])), NewUnionCase (Some, Value(1))])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []



    [<Test>]
    let ``Regular slicing quotation on array with star return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = [|1;2;3|]
    let q = <@ xs.[*] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call(None, GetArraySlice, [ValueWithName([|1;2;3|], xs), NewUnionCase(None), NewUnionCase(None)])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Regular slicing quotation on array with range return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = [|1;2;3|]
    let q = <@ xs.[0..1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call(None, GetArraySlice, [ValueWithName([|1;2;3|], xs), NewUnionCase(Some, Value(0)), NewUnionCase(Some, Value(1))])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []


    [<Test>]
    let ``Regular slicing quotation on array2d with ranges return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = array2D [ [1;2;3]; ]
    let q = <@ xs.[0..1, 1..2] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (None, GetArraySlice2D, [ValueWithName ([[1; 2; 3]], xs), NewUnionCase (Some, Value (0)), NewUnionCase (Some, Value (1)), NewUnionCase (Some, Value(1)), NewUnionCase (Some, Value(2))])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Regular slicing quotation on array2d with fixed index return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = array2D [ [1;2;3]; ]
    let q = <@ xs.[0..1, 1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (None, GetArraySlice2DFixed2, [ValueWithName ([[1; 2; 3]], xs), NewUnionCase (Some, Value (0)), NewUnionCase (Some, Value (1)), Value(1)])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Regular indexing quotation on array2d return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = array2D [ [1;2;3]; ]
    let q = <@ xs.[0, 1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call(None,GetArray2D,[ValueWithName([[1;2;3]],xs),Value(0),Value(1)])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Regular slicing quotation on list with star return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = [1;2;3]
    let q = <@ xs.[*] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (Some (ValueWithName ([1; 2; 3], xs)), GetSlice, [NewUnionCase (None), NewUnionCase (None)])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []

    [<Test>]
    let ``Regular slicing quotation on list with range return expected expression``() =
        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
open System
open System.Text.RegularExpressions
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns

let test() = 
    let xs = [1;2;3]
    let q = <@ xs.[0..1] @>
    
    let expectEqualWithoutWhitespace s1 s2 = 
        let a = Regex.Replace(s1, "\s", "") 
        let b = Regex.Replace(s2, "\s", "")
        if a <> b then failwithf "Expected '%s', but got\n'%s'" a b
        ()
    
    let expected = "Call (Some (ValueWithName ([1; 2; 3], xs)), GetSlice, [NewUnionCase (Some, Value(0)), NewUnionCase (Some, Value(1))])"
    expectEqualWithoutWhitespace expected (q.ToString())
    
test()
            """
            []


