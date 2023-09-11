module GreaterColonOperator =
    let (>:) e1 e2 = if e1 then e1 else false
    
    let (>:) (>:) e1 e2 = if e1 then e1 else false

    type Vector() =
        static member (>:) = false
        
    type Vector1 =
        static member (>:) (>:) = false

    type Vector3() =
        static member (>:) (>:) = false
