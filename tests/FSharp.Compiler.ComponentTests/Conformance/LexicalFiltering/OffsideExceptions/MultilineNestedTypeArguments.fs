// #Regression #Conformance #LexFilter #Exceptions
// https://github.com/dotnet/fsharp/issues/15171
// The closing '>' of a nested, multiline type-argument list may align with the column of the
// opening type name (here the inner 'Foo'); it must not be treated as a new sequence-block item.

open System

type Bar = class end
type Foo<'a> = class end

type Terminal =
    abstract onKey:
        IEvent<
            Foo<
                Bar * int
            >
        > with get, set
