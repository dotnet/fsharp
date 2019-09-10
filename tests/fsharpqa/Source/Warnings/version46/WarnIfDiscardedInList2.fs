// #Warnings
//<Expects status="Warning" span="(15,19)" id="FS3222"></Expects>

// stupid things to make the sample compile
let div _ _ = 1
let subView _ _ = [1; 2]
let y = 1

// elmish view
let view model dispatch =
   [
        div [] [
           match y with
           | 1 -> yield! subView model dispatch
           | _ -> subView model dispatch
        ]
   ]

exit 0