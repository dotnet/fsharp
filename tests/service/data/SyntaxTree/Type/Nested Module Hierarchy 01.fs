// Expected: Warning for module inside type at deep nesting level
module Root

module Level1 =
    type ValidType1 = int
    
    module Level2 =
        type ValidType2 = string
        
        module Level3 =
            type TypeWithInvalidModule =
                | A
                | B
                module InvalidModule = 
                    let x = 1
            
            type ValidType3 = float
