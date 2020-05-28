module Pos30

module Test0 = 
    System.Console.WriteLine(format="{0}", arg = (Array.ofSeq <| seq {
        yield box "hello"
    }))

module Test0b = 
    System.Console.WriteLine(seq {
        yield 1 
    })

module Test1 = 
    System.Console.WriteLine(format="{0}", arg = [| 
        "hello"
    |])
    System.Console.WriteLine([|
        "hello"
    |])

module Test2 = 
    System.Console.WriteLine(format="{0}", arg = Array.ofList [ 
        "hello"
    ])
    System.Console.WriteLine([
        "hello"
    ])

module Test3 = 
    System.Console.WriteLine(format="{0}", arg = [| 
    |])
    System.Console.WriteLine([|
    |])


module Test4 = 
    System.Console.WriteLine(format="{0}", arg = Array.ofList [ 
    ])
    System.Console.WriteLine([
    ])

module Test5 = 
    System.Console.WriteLine(format="{0}", arg = [|
        yield box "hello"
        yield box "hello"
        yield box "hello"
        yield box "hello"
    |])

module Test6 = 
    System.Console.WriteLine(format="{0}", arg = Array.ofList [ 
        yield box "hello"
        yield box "hello"
        yield box "hello"
        yield box "hello"
        yield box "hello"
    ])


module Test7 = 
    System.Console.WriteLine [| 
        yield "hello"
        yield "hello"
        yield "hello"
        yield "hello"
        yield "hello"
    |]


    