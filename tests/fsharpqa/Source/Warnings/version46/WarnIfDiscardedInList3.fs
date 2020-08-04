// #Warnings
//<Expects status="Warning" span="(15,19)" id="FS0020"></Expects>

// stupid things to make the sample compile
let div _ _ = 1  
let subView _ _ = true
let y = 1

// elmish view
let view model dispatch =
   [   
        div [] [
           match y with
           | 1 -> ()
           | _ -> subView model dispatch
        ]
   ]

exit 0