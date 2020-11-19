// #Conformance #TypeInference #ApplicationExpressions #ReqNOMT 
#light

// Infer multiple parameters as a tuple when expecting a single generic arg

type Fruit (shelfLife : int) =
    
     let mutable m_age = 0

     let expireEvent = new Event<int * int>()

     member this.Age x =
          m_age <- m_age + x
          if m_age > shelfLife then

              // This used to give...              
              //      ERROR: The member or object constructor 'Trigger' takes 1 argument(s) but is here given 2. 
              //      The required signature is 'member Event.Trigger : arg:'a -> unit'.
              // But now is inferred as the correct tuple argument.
              expireEvent.Trigger(m_age, shelfLife)
     
     member this.OnExpire = expireEvent.Publish

let apple = new Fruit(10)

apple.OnExpire.Add (fun (age, shelfLife) -> if age = 99 && shelfLife = 10 then exit 0)

apple.Age(99)

exit 1
