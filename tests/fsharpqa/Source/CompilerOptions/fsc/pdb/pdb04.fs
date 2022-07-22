// #Regression #NoMT #CompilerOptions #NoMono 
// Regression test for FSharp1.0:3204 - Compiler throws exception when trying to use option "-g --pdb" on a locked file (or any non-accessible file)
//<Expects id="FS1503" status="error">The pdb output file name cannot match the build output filename use --pdb:filename.pdb</Expects>
exit 1
