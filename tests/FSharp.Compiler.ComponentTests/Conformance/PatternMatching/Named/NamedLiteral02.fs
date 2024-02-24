// #Conformance #PatternMatching #ActivePatterns 
#light

// Verify [<Literal>] values can be used with active patterns

[<Literal>]
let OneHundred = 100

let (|ToInt|) (input : string) = System.Int32.Parse(input)

// Match the result of the active pattern against a literal value.
let test1() =
    match " 101 " with
    | ToInt OneHundred 
        ->  // If it didn't match the literal, this would capture new
            // value 'OneHundred' and this would fire.
            exit 1
    | _ -> ()
    
    match " 100 " with
    | ToInt OneHundred
        -> ()
    | _ -> exit 1
    
    ()
    
// Run the test
test1()


// Verify literals (and norma values) can be specified as parameters
let (|ConcatedWith|) (x : string) (y : string) (input : string) =
    input + x + y

[<Literal>]
let BarLiteral = "bar"

[<Literal>]
let BazLiteral = "baz"    
    
let test2() =
    let barValue = "bar"
    let bazValue = "baz"
    
    match "foo" with
    | ConcatedWith barValue BazLiteral "foobarbaz" -> ()
    | ConcatedWith barValue BazLiteral result -> printfn "%s" result; exit 1
    
    match "foo" with
    | ConcatedWith BarLiteral bazValue "foobarbaz" -> ()
    | ConcatedWith barValue BazLiteral result -> printfn "%s" result; exit 1
    
    ()

// Run the etest
test2()

exit 0
