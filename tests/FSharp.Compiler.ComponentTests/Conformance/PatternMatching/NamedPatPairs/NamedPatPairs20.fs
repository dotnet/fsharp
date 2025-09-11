type U = A of x : (int * int) * y : int

// These are the same
let (A (x = _, _, y = _)) = A ((1, 2), 3)
let (A (x = (_, _), y = _)) = A ((1, 2), 3)