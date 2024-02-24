module MyTests

let printerFunc = _.ToString()

let processString (x:string) = x |> _.Length

type MyRecord = {ThisIsFieldOfMyRecord : string}

let extractProp = _.ThisIsFieldOfMyRecord