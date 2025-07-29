module Module

async {
    let! (x, y): int * int = asyncInt()
    and! (x: int, y: int) = asyncInt()
    return () 
}
