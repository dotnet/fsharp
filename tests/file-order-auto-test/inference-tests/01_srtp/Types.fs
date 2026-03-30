module SrtpTest.Types

type Vector2D = {
    X: float
    Y: float
}
with
    static member (+) (a: Vector2D, b: Vector2D) = { X = a.X + b.X; Y = a.Y + b.Y }
    static member (*) (a: float, b: Vector2D) = { X = a * b.X; Y = a * b.Y }
    static member Zero = { X = 0.0; Y = 0.0 }
