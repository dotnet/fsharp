module FsiOrder.Consumer

let describe (s: FsiOrder.Types.Shape) =
    match s with
    | FsiOrder.Types.Circle _ -> "circle"
    | FsiOrder.Types.Square _ -> "square"

let totalArea shapes =
    shapes |> List.sumBy FsiOrder.Types.area
