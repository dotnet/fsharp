// This no longer gives a warning because implicit yields are now allowed


let div _ _ = 1  
let subView _ _ = [1; 2]

// elmish view
let view model dispatch =
   [
       yield! subView model dispatch
       div [] []
   ]
    
exit 0