module Module
let! x:int = async { return 1 }
let! (y:int) = async { return 2 }