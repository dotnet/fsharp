module FsiOrder.Types

type Shape =
    | Circle of radius: float
    | Square of side: float

let area s =
    match s with
    | Circle r -> System.Math.PI * r * r
    | Square s -> s * s
