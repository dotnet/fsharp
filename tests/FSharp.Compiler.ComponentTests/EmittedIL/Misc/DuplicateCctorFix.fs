// #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for duplicate .cctor issue: https://github.com/dotnet/fsharp/issues/18767
// This test verifies that discriminated unions with generic statics and nullary cases 
// generate only one .cctor method instead of failing with "duplicate entry '.cctor' in method table"

type TestUnion<'T when 'T: comparison> =
    | A of 'T
    | B of string  
    | C // nullary case that triggers union erasure .cctor for constant field initialization
    
    // Static member that triggers incremental class .cctor generation
    static member val StaticProperty = "test" with get, set
    
    // Another static member to ensure .cctor has meaningful initialization
    static member CompareStuff x y = compare x y