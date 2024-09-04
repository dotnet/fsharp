module MyTestModule

let justInt = 42
let maybeString : string | null = null
let maybeListOfMaybeString : List<_> | null = [maybeString]

let giveMeA () =
    {| A = maybeString; B = maybeListOfMaybeString; C = justInt|}

let giveMeB () =
    {| A = justInt; B = justInt; C = justInt|}

let giveMeC () =
    {| A = maybeString; B = maybeString; C = maybeString|}

let threeHappyStrings () : string array = [|giveMeA().ToString();giveMeB().ToString();giveMeC().ToString()|]