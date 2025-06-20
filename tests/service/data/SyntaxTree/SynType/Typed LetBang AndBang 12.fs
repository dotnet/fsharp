module Module

async {
    let! x as y = asyncInt()
    and! a as b = asyncString()
    return x + b
}