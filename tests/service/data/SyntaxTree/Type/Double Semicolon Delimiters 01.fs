// Testing: Double semicolon delimiters with invalid constructs
module Module

type A =
    type B = int;;
    module C = ();;
    exception D;;
