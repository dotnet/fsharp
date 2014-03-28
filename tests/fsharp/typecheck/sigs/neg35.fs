namespace global

module PrefixOperatorsNegative = 
    let (~<<) x = x // now rejected
    let (~++) x = x // now rejected
    let (~!) x = x // now rejected
    let (~%%%%%%) x = x // now rejected

    let x1 : int = id ~<< 2 // now rejected
    let x2 : int = id ~++ 2 // now rejected
    let x3 : int = id ~+2 // now rejected
    let x4 : int = id ~!2 // now rejected
