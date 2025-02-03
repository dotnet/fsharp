module Test

// https://github.com/dotnet/fsharp/issues/18165

type FooBar =
    { xyz : string }
    static let staticLet = 1

let doThing (foo : FooBar) =
    let bar = { foo with xyz = foo.xyz }
    let baz = { bar with xyz = bar.xyz }
    printf "%O" baz

doThing { xyz = "" }