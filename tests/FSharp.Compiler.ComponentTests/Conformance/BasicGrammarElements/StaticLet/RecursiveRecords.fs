module rec Test

type Chicken = 
    {Eggs : Egg list}
    static do printfn "Chicken init"
    static let firstEggEver = 
        printfn "creating firstEggEver"
        {Mother = {Eggs = []}}

    static member FirstEgg = firstEggEver

type Egg = 
    {Mother : Chicken}
    static do printfn "Egg init"

type Omelette = 
    {Egg : Egg}
    static do printfn "Omelette init"



let o = {Egg = {Mother = {Eggs = [{Mother = {Eggs = [Chicken.FirstEgg]}}]}}}

printfn "%i" (o.Egg.Mother.Eggs |> List.length)


