module FSharp.Compiler.LanguageServer.Tests.IdentifierExtractionTests

open Xunit

// Extract the identifier parsing logic from the handler for testing
let getIdentifierAtPosition (text: string) (col: int) =
    if col < 0 || col >= text.Length then []
    else
        // If we're on whitespace or punctuation (except dots), return empty
        let charAtPos = text.[col]
        if System.Char.IsWhiteSpace(charAtPos) || 
           (System.Char.IsPunctuation(charAtPos) && charAtPos <> '.' && charAtPos <> '_') then
            []
        else
            let mutable start = col
            let mutable endPos = col
            
            // Move start backward while we have identifier characters or dots
            while start > 0 && (System.Char.IsLetterOrDigit(text.[start - 1]) || text.[start - 1] = '_' || text.[start - 1] = '.') do
                start <- start - 1
                
            // Move end forward while we have identifier characters or dots
            while endPos < text.Length && (System.Char.IsLetterOrDigit(text.[endPos]) || text.[endPos] = '_' || text.[endPos] = '.') do
                endPos <- endPos + 1
                
            if start = endPos then []
            else 
                let identifier = text.Substring(start, endPos - start)
                // Handle dotted identifiers like Module.Function
                // Remove empty parts and leading/trailing dots
                identifier.Split('.') 
                |> Array.toList 
                |> List.filter (fun s -> not (System.String.IsNullOrWhiteSpace(s)))

[<Fact>]
let ``Simple identifier extraction works`` () =
    let result = getIdentifierAtPosition "let x = 42" 4
    Assert.Equal(["x"], result)

[<Fact>]
let ``Function name extraction works`` () =
    let result = getIdentifierAtPosition "let func x = x + 1" 5
    Assert.Equal(["func"], result)

[<Fact>]
let ``Dotted identifier extraction works`` () =
    let result = getIdentifierAtPosition "Module.Function()" 10
    Assert.Equal(["Module"; "Function"], result)

[<Fact>]
let ``System namespace extraction works`` () =
    let result = getIdentifierAtPosition "System.String.Empty" 8
    Assert.Equal(["System"; "String"; "Empty"], result)

[<Fact>]
let ``Empty text returns empty list`` () =
    let result = getIdentifierAtPosition "" 0
    Assert.Equal([], result)

[<Fact>]
let ``Position beyond text length returns empty list`` () =
    let result = getIdentifierAtPosition "let x = 42" 100
    Assert.Equal([], result)

[<Fact>]
let ``Position on whitespace returns empty list`` () =
    let result = getIdentifierAtPosition "let x = 42" 3  // Position on space
    Assert.Equal([], result)

[<Fact>]
let ``Position on punctuation returns empty list`` () =
    let result = getIdentifierAtPosition "let x = 42" 6  // Position on '='
    Assert.Equal([], result)

[<Fact>]
let ``Negative position returns empty list`` () =
    let result = getIdentifierAtPosition "let x = 42" -1
    Assert.Equal([], result)