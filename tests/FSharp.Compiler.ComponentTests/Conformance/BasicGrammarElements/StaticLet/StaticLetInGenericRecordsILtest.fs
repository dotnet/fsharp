module Test

open System

[<NoComparison>]
[<NoEquality>]
type MyRecord<'T> = 
    {
        X: 'T 
    }
    // Init per typar
    static let cachedVal =
        Console.WriteLine(typeof<'T>.Name)
        typeof<'T>.Name

    static member GetMyName() = cachedVal



Console.WriteLine(MyRecord<int>.GetMyName())
Console.WriteLine(MyRecord<string>.GetMyName())