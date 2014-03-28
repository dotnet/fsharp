// Dev11:210820, ensuring that the C# attributes to get line numbers and directories work correctly across F# boundaries

let c = new ClassLibrary1.Class1()
c.DoStuff()

let f() =
    c.TraceMessage("from F#", int __LINE__, __SOURCE_DIRECTORY__ + __SOURCE_FILE__)
f()
