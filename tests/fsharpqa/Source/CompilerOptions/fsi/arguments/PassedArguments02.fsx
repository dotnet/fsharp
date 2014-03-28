// <Expects status=success>Args: \[\|"PassedArguments02\.fsx"; "Alpha"; "Beta Gamma"; "foo\.fs"\|]</Expects>

printfn "Args: %A" fsi.CommandLineArgs
#q;;