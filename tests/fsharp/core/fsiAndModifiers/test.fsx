#r "TestLibrary.dll"

let x = TestType.Get(100)
if x = 100 then System.IO.File.WriteAllText("test.ok", "")