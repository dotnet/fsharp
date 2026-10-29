// Anonymous Union Types
// Pattern matching on anonymous union types - two columns

let decide (x: (int|bool), y: (int|bool)): int =
    match (x, y) with
        | :? bool, _ -> 1
        | _, :? int  -> 2