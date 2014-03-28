module Neg53

let r = 
    async {
        let! z = failwith ""
        let rec loop() =
            async {
                let x = _arg1 // this should give an error
                return ()
            }
        return! loop()
    }
 
