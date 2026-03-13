let assertEqual a b = if a <> b then failwithf "not equal: %A and %A" a b
try System.ArgumentException.ThrowIfNullOrWhiteSpace(Seq.init 50 (fun _ -> " ")
  (* comment *) 
  |> String.concat " ")
with :? System.ArgumentException as ex -> 
  assertEqual true (ex.Message.Contains("(Parameter 'Seq.init 50 (fun _ -> \" \")
  (* comment *) 
  |> String.concat \" \""))
  

try System.ArgumentException.ThrowIfNullOrWhiteSpace(argument = (Seq.init 11 (fun _ -> " ")
  (* comment *) 
  |> String.concat " "))
with :? System.ArgumentException as ex -> 
  assertEqual true (ex.Message.Contains("(Parameter '(Seq.init 11 (fun _ -> \" \")
  (* comment *) 
  |> String.concat \" \")"))