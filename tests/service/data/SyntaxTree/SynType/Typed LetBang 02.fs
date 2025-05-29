module Module

async {
    let! (res: int) = async { return 1 }
    return res
}
