module FsiOrder.Types

type Shape =
    | Circle of radius: float
    | Square of side: float

val area: Shape -> float
