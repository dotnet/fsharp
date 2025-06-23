// #Regression #Conformance #LexicalAnalysis 
// Related to FSHARP1.0:4287

// Test the __SOURCE_FILE__ directive

if System.IO.Path.GetFileName(__SOURCE_FILE__) <> "SourceFile01.fs" then ignore 1

ignore 0
