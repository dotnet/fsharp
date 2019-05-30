// #Conformance #TypesAndModules #Unions 
// Constructing DUs with named fields
//<Expects status="success"></Expects>
type Lunch = 
  | Sandwich of meat : string * pickles : bool * layers : int
  | Soup of Ounces : float * string
  | Salad of int * string
  | Burrito
  | Burger of ``Patty Size`` : int * ``Cheese Type`` : string

let mutable s = Sandwich("beef", true, 3)
s <- Sandwich("beef", true, layers = 3)
s <- Sandwich("beef", pickles = true, layers = 3)
s <- Sandwich(meat = "beef", pickles = true, layers = 3)

s <- Soup(8., "pea")

s <- Salad(6, "Caesar")

s <- Burrito

s <- Burger(8, "Cheddar")
s <- Burger(``Patty Size`` = 12, ``Cheese Type`` = "Pepper jack")

// multi-case DUs do not have member name clash restrictions
type MyDU =
    | Case1 of Val1 : int * Val2 : string
    | Case2 of int * float
    | Case3 of int
    member this.Val1 = 5
    member this.Item1 = ""
    member this.Item = 4.