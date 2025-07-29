module Module

async {
    let! (x, y): int * int = asyncInt()
    and! (x, y): int * int = asyncInt()
    return () 
}
