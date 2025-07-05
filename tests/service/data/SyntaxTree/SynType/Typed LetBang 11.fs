module Module
async {
    let! (Union value): int option = asyncOption()
    return value 
}
