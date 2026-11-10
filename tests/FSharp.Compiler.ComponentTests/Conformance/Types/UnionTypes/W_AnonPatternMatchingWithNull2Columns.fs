// Anonymous Union Types
// Pattern matching on anonymous union types - two columns, with null

let decide (x: (int|string|null), y: (int|string|null)): int =
    match (x, y) with
        | :? string, _ -> 1
        | :? int, _ -> 2
        | null, :? int -> 3
        | null, :? string -> 4