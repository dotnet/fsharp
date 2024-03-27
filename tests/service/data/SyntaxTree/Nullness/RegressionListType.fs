type List<'T> = 
    | ([])  
    | ( :: )  of Head: 'T * Tail: 'T list