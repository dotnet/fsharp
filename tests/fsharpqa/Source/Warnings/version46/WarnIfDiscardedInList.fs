// #Warnings
//<Expects status="Warning" span="(11,8)" id="FS3221"></Expects>

let div _ _ = 1  
let subView _ _ = [1; 2]

// elmish view
let view model dispatch =
   [
       yield! subView model dispatch
       div [] []
   ]
    
exit 0