open Prelude

// Test a simple ref  argument
module CompareExchangeTests = 
    let mutable x = 3
    let v =  System.Threading.Interlocked.CompareExchange(&x, 4, 3)
    check "cweweoiwekla" v 3
    let v2 =  System.Threading.Interlocked.CompareExchange(&x, 5, 3)
    check "cweweoiweklb" v2 4