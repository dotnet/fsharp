// #Regression #Diagnostics 
// Regression test for DEV10:903949
// Passing a resx to the compiler should result in a deprecation warning
//<Expects status="warning" id="FS2023">Passing a \.resx file \(W_PassingResxToCompilerIsDeprecated01\.resx\) as a source file to the compiler is deprecated\. Use resgen\.exe to transform the \.resx file into a \.resources file to pass as a --resource option\. If you are using MSBuild, this can be done via an <EmbeddedResource> item in the \.fsproj project file\.$</Expects>
