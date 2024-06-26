module MyTestModule

let iCanProduceNullSometimes (arg:'a) : 'a =
    let mutable cache = null

    if (System.DateTime.Now.Hour = 7) then
        cache <- arg

    if (System.DateTime.Now.Hour = 7) then
        null
    else
        arg

let iPatternMatchOnArg arg =
    match arg with
    | null -> "null"
    | _ -> "not null"

let iAcceptNullPartiallyInferedFromUnderscore(arg: _ | null) = 0

let iAcceptNullPartiallyInferedFromNamedTypar(arg: 'a | null) = 0

let iAcceptNullWithNullAnnotation(arg: 'a | null when 'a: not struct) =
    if isNull arg then
        1
    else
        0

let iAcceptNullExplicitAnnotation(arg: 'T when 'T:null) =
    if isNull arg then
        1
    else
        0


let fullyInferedTestCase arg1 arg2 = 
    System.Console.Write(iAcceptNullPartiallyInferedFromUnderscore arg1)
    let maybeNull = iCanProduceNullSometimes arg2
    maybeNull

let structShouldBeAllowedHere arg = 
    let boxed : obj | null = box arg
    iAcceptNullPartiallyInferedFromUnderscore boxed