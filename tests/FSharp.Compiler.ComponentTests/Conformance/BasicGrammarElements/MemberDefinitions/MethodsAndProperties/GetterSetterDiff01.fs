// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Verify if parameters to getter and setter are different

let mutable a, b, c = 0, false, ""

type Foo() =

    member this.Item with get (x : int, y : bool) = 
                                        a <- x; b <- y; 42
                       and  set (z : string, y : bool, x : int) (arg : float) = 
                                        a <- x; b <- y; c <- z; 
                                        if arg <> 12.34 then failwith "Failed: 1"

if (a, b, c) <> (0, false, "") then failwith "Failed: 2"

let t = new Foo()
if t.[1, true] <> 42 then failwith "Failed: 3"

if (a, b, c) <> (1, true, "") then failwith "Failed: 4"

t.["#", false, 2] <- 12.34

if (a, b, c) <> (2, false, "#") then failwith "Failed: 5"
