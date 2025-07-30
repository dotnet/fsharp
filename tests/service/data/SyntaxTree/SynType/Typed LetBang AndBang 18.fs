module Module

async {
    let! (x as y): int = asyncInt()
    and! (a as b): = asyncString()
    return x + b
}