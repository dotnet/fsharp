module Module

async {
    let! res: int = async { return 1 }
    and! res2: int = async { return 2 }
    return res
}
