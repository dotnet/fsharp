module Module

async {
    let a: int, b: int =  1, 3
    let! c: int, d: int = async { return 1, 3 }
    return a + b + c + d
}