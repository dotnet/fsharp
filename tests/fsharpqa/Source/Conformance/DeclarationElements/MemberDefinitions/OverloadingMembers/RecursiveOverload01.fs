// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Originally from https://gist.github.com/gusty/b6fac370bff36a665d75
type FoldArgs<'t> = FoldArgs of ('t -> 't -> 't)

let inline foldArgs f (x:'t) (y:'t) :'rest = (FoldArgs f $ Unchecked.defaultof<'rest>) x y

type FoldArgs<'t> with
    static member inline ($) (FoldArgs f, _:'t-> 'rest) = fun (a:'t) -> f a >> foldArgs f
    static member        ($) (FoldArgs f, _:'t        ) = f

let test1() =
    let x:int     = foldArgs (+) 2 3 
    let y:int     = foldArgs (+) 2 3 4
    let z:int     = foldArgs (+) 2 3 4 5
    let d:decimal = foldArgs (+) 2M 3M 4M
    let e:string  = foldArgs (+) "h" "e" "l" "l" "o"
    let f:float   = foldArgs (+) 2. 3. 4.

    let mult3Numbers a b c = a * b * c
    let res2 = mult3Numbers 3 (foldArgs (+) 3 4) (foldArgs (+) 2 2 3 3)
    ()

// Run the test
test1() 

exit 0