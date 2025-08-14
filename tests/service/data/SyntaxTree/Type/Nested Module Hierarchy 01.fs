// Testing: Module inside type at deep nesting level
module Root

module Level1 =
    module Level2 =
        module Level3 =
            type TypeWithInvalidModule =
                module InvalidModule = 
                    let x = 1
