module Module

async {
    let! [| first; second |]: int array = asyncArray()
    and! head :: tail: string list = asyncList()
    return first
}
