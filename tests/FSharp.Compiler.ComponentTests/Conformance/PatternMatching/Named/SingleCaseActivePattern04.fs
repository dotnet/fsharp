// Credit to https://www.devjoy.com/blog/active-patterns/single-total/
open System

let (|IsPalindrome|) (str: string) =
    str = String(str.ToCharArray() |> Array.rev)

let (|UpperCaseCount|) (str: string) =
    str.ToCharArray()
    |> Array.filter (fun c -> c.ToString() = c.ToString().ToUpper())
    |> Array.length

let (|LowerCaseCount|) (str: string) =
    str.ToCharArray()
    |> Array.filter (fun c -> c.ToString() = c.ToString().ToLower())
    |> Array.length

let (|SpecialCharacterCount|) (str: string) =
    let specialCharacters = "!£$%^"
    str.ToCharArray()
    |> Array.filter (fun c -> specialCharacters.Contains(c.ToString()))
    |> Array.length

let (|IsValid|) (str: string) =
    match str with
    | UpperCaseCount 0 -> (false, "Must have at least 1 upper case character")
    | LowerCaseCount 0 -> (false, "Must have at least 1 lower case character")
    | SpecialCharacterCount 0 -> (false, "Must have at least 1 of !£$%^")
    | IsPalindrome true -> (false, "A palindrome for a password? What are you thinking?")
    | UpperCaseCount u & LowerCaseCount l & SpecialCharacterCount s -> 
        (true, sprintf "Not a Palindrome, %d upper, %d lower, %d special." u l s)

let (IsValid result) = "Able was I ere I saw Elba"
let (IsPalindrome result2) = "Able was I ere I saw Elba"
let (UpperCaseCount result3) = "Able was I ere I saw Elba"
let (LowerCaseCount result4) = "Able was I ere I saw Elba"
let (SpecialCharacterCount result5) = "Able was I ere I saw Elba"
