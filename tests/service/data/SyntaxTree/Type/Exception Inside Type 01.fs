// Expected: Warning for exception inside type
module Module

type A = 
    | A
    exception MyException of string
