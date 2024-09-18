// Test the case where a2.fs is in the same namespace as a1.fs, and so the namespace is implicitly opened
#load "a1.fs"
#load "a2.fs"

let os = System.IO.File.CreateText "test.ok" in os.Close() 
