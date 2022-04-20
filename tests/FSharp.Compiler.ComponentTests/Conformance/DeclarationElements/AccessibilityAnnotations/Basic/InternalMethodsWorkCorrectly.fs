// #Conformance #DeclarationElements #Accessibility 
#light

module internal InternalModule =

    let internal initialString = "InitialValue"
    let internal deVowelStr x = String.map (fun c -> match c with 'a' | 'e' | 'i' | 'o' | 'u' | 'y' -> '_' | _ -> c) x

    type internal InternalClass (x : string) =
        let m_innards = x
        member internal this.InternalMethod_GetDevoweledString() = deVowelStr m_innards
        
module testModule =

    let startingValue = InternalModule.initialString

    // 'x' needs to be internal, otherwise public testModule will leak an internal type.
    let internal x = new InternalModule.InternalClass(startingValue)

    let result = x.InternalMethod_GetDevoweledString()
    
    if result <> InternalModule.deVowelStr startingValue then failwith "Failed: 1"

