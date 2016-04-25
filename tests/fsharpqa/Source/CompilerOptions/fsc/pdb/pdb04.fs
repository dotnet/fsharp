// #Regression #NoMT #CompilerOptions #NoMono 
// Regression test for FSharp1.0:3204 - Compiler throws exception when trying to use option "-g --pdb" on a locked file (or any non-accessible file)
//<Expects id="FS1136" status="error">Unexpected error creating debug information file '.+pdb04\.exe'</Expects>

exit 1
