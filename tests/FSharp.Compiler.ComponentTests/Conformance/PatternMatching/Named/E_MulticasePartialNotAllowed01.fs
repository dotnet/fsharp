// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:4621
// Multi case partial active patterns are not allowed


//<Expects span="(20,7-20,15)" status="error" id="FS0039">The pattern discriminator 'Sentence' is not defined</Expects>

let (|Sentence|Word|Punctuation|WhiteSpace|_|) (input : string) = 
    if input.Trim() = "" then 
        Some(WhiteSpace())
    elif input.IndexOf(" ") <> -1 then 
        Some(Sentence (input.Split([|" "|], System.StringSplitOptions.RemoveEmptyEntries)))
    else
        match input with
        | "." | ";" | "," | ":" | "?" | "!" -> Some(Punctuation(input.ToCharArray().[0]))
        | _ -> Some(Word (input))
        
let test str =
    match str with
    | Sentence words -> words.Length    // All words in sentence
    | Word word -> word.Length          // Characters in word
    | WhiteSpace -> 0 
    | Punctuation _ -> -1
    | _ -> failwith "Should not happen"
