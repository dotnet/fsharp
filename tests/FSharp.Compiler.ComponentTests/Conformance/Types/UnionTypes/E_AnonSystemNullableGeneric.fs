// Anonymous Union Types
// Generic instantiation must not allow System.Nullable in an anonymous union

let length<'T> (x: ('T|string)) =
    match x with
    | :? 'T -> 0
    | :? string as text -> text.Length

length<System.Nullable<int>> (System.Nullable<int>()) |> ignore
