// #Regression #Conformance #LexicalAnalysis 
#light

//<Expects id="FS0010" status="error" span="(18,1)">Incomplete structured construct at or before this point in binding</Expects>
//<Expects id="FS0516" status="error" span="(9,5)">End of file in comment begun at or before here</Expects>

let numbers = [1 .. 20]

let (*
asdf
asdf
asdf
asdf
asdf
asdf
asdf
asdf
