// #Regression #Conformance #ControlFlow #Exceptions 
//<Expects id="FS0413" status="error" span="(4,1)">Calls to 'reraise' may only occur directly in a handler of a try-with$</Expects>

reraise()

exit 1
