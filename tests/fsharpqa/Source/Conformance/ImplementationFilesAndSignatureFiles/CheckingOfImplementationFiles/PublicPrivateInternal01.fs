// #Conformance #SignatureFiles 
#light

module Island

type Plant =
    | Tree   of string
    | Bush   of Plant
    | Grass  of int
    | Flower of float * float
    | Weed

type internal InternalBuilding =
    {
        Name    : string
        Purpose : string[]
    }

let vegetation = [Tree("Oak"); Tree("Apple")]

let internal getInternalBuildings () =
    new Set<InternalBuilding>( [| { Name = "Hut"; Purpose = [| "Sleeping"; "Storing fish" |] } |])

let private topSecretInstallations = [| |] : InternalBuilding[]
