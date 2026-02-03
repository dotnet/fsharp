// #Conformance #DataExpressions #Query
// DevDiv:568559, incorrect error message for zip style operators
//<Expects status="error" span="(17,5-17,8)" id="FS3098">'zip' must come after a 'for' selection clause and be followed by the rest of the query\. Syntax: \.\.\. zip var in collection \.\.\.$</Expects>
//<Expects status="error" span="(23,5-23,10)" id="FS3097">Incorrect syntax for 'zip'\. Usage: zip var in collection\.$</Expects>
//<Expects status="error" span="(29,5-29,8)" id="FS3098">'zip' must come after a 'for' selection clause and be followed by the rest of the query\. Syntax: \.\.\. zip var in collection \.\.\.$</Expects>

type ListBuilder() =
    [<CustomOperation("zip", IsLikeZip = true)>]
    member x.Zip(l1, l2, f) = List.map2 f l1 l2
    member x.Yield(v) = [v]
    member x.For(l,f) = List.collect f l

let list = ListBuilder()

list {
    for x in [1;2;3] do
    zip y into ["a";"b";"c"]
    yield x
} |> ignore

list {
    for x in [1;2;3] do
    zip y
    yield x
} |> ignore

list {
    for x in [1;2;3] do
    zip y into
    yield x
} |> ignore