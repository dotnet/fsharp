// #Regression #Conformance #LexicalAnalysis 
#light

//<Expects id="FS0010" status="error">Unexpected keyword 'type' in binding</Expects>
//<Expects id="FS0010" status="error">Unexpected keyword 'class' in binding. Expected '=' or other token.</Expects>

let type = 2

let f class = 5

exit 1
