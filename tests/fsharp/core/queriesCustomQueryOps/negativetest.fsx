module NegativeTests

type DoNothingBuilder() = 
    member this.Yield(v) = [v]
    member this.Delay f = f
    member this.Run(f) = f()
    member this.Zero() = []
    member this.For(e, b) = List.collect b e
    member this.TryWith(b, h) = 
        try b()
        with e -> h e
    member this.TryFinally(b, c) = try b() finally c()
    [<CustomOperation("select")>]
    member this.Select(v, [<ProjectionParameter>]f) = List.map f v 
         
let builder = DoNothingBuilder()

// if\then\else are not allowed in query expressions
query {
    if true then 
        yield 1 
    else 
        yield 2
}

// try\with are not allowed in query expressions
query {
    try
        yield 1
    with _ -> yield 2
}

// try\finally are not allowed in query expressions
query {
    try 
        yield 1
    finally
        printfn "1"
}

// custom operators are not allowed in if\then\else
builder {
    if true then
        select 1
    else
        select 2
}

// custom operators are not allowed in try\with - 'try'
builder {
    try
        select 1
    with _ -> yield 1
}

// custom operators are not allowed in try\with - 'with'
builder {
    try
        yield 1
    with _ -> select 1
}

// custom operators are not allowed in try\finally
builder {
    try
     select 1
    finally  ()
}