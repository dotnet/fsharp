open Prelude

module TestBaseCall =
    type Incrementor(z) =
        abstract member Increment : int byref * int byref -> unit
        default this.Increment(i : int byref,j : int byref) =
            i <- i + z

    type Decrementor(z) =
        inherit Incrementor(z)
        override this.Increment(i, j) =
            base.Increment(&i, &j)

            i <- i - z