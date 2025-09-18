// Expected: Warning for module in type augmentation
module Module

type A = 
    | A
    with
        module M = begin end
