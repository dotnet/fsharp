// #Conformance #DeclarationElements #Events #ReqNOMT 
#light

// Sanity check events based on the new library changes

type Fruit (shelfLife : int) =
    
     let mutable m_age = 0

     let expireEvent = new Event<int * int>()

     member this.Age x =
          m_age <- m_age + x
          if m_age > shelfLife then
              expireEvent.Trigger((m_age, shelfLife))
     
     member this.OnExpire = expireEvent.Publish


let mutable x = -1
let mutable y = -1

let apple = new Fruit(10)
apple.OnExpire.Add(fun (age, shelfLife) -> x <- age; y <- shelfLife)

// Verify x and y are still at their initialized values
if x <> -1 || y <> -1 then failwith "Failed: 1"

// Age the apple, which will call our event handler
apple.Age 12

// Verify x and y have been updated
if x <> 12 || y <> 10 then failwith "Failed: 2"

// Now attach a second event handler, but filtered.

let pear = new Fruit(50)
pear.OnExpire.Add(fun (age, shelfLife) -> x <- age; y <- shelfLife)

let mutable a = 0
let mutable b = 0

pear.OnExpire 
|> Event.map (fun (age, shelfLife) -> (-age, -shelfLife))          // Invert values
|> Event.filter (fun (age, shelfLife) -> age < -80)                // Ignore all apples > 100 old
|> Event.add (fun (age, shelfLife) -> a <- age; b <- shelfLife) // When triggered set a and bs values.


// Age the apple, which will call our event handler
pear.Age 75

// Verify x and y have been updated
if x <> 75 || y <> 50 then failwith "Failed: 3"
// Verify a and b are unchanged
if a <> 0 || b <> 0 then failwith "Failed: 4"

// Age again, verify our second handler has fired
pear.Age(25)

if x <> 100 || y <> 50 then failwith "Failed: 5"
// Verify a and b are unchanged
if a <> -100 || b <> -50 then failwith "Failed: 6"
