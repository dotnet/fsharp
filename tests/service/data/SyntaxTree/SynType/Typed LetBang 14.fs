module Module

async {
    let! (x as y): = asyncInt()
    and! (a as b): string = asyncString()
    return x + b
}