module Module

async {
    let! (Union value): int option = asyncOption()
    and! (Union value2): int option = asyncOption()
    return value + value2
}