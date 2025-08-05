module Module

async {
    let! (Union value) = asyncOption()
    and! (Union value2) = asyncOption()
    return value + value2
}