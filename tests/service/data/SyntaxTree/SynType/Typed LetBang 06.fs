module Module
let! _:int = async { return 1 }
let! (_:int) = async { return 2 }