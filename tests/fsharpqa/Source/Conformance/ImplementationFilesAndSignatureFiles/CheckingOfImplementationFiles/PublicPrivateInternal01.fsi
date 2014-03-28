// #Conformance #SignatureFiles 
#light

// Test FSI checking for public, private, and internal 'stuff'

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

val vegetation : Plant list

val internal getInternalBuildings : unit -> Set<InternalBuilding>

val private topSecretInstallations : InternalBuilding[]
