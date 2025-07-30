module Module

async {
    let Even as x: int = 1
    let! Even as x: int = async { return 2 }
    return x
}