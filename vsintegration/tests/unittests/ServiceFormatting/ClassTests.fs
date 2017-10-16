// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.ClassTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``class signatures``() =
    formatSourceString true """
module Heap

type Heap<'T when 'T : comparison> =
    class
    new : capacity:int -> Heap<'T>
    member Clear : unit -> unit
    member ExtractMin : unit -> 'T
    member Insert : k:'T -> unit
    member IsEmpty : unit -> bool
    member PeekMin : unit -> 'T
    override ToString : unit -> string
    member Count : int
    end""" config
    |> prepend newline
    |> should equal """
module Heap

type Heap<'T when 'T : comparison> =
    class
        new : capacity:int -> Heap<'T>
        member Clear : unit -> unit
        member ExtractMin : unit -> 'T
        member Insert : k:'T -> unit
        member IsEmpty : unit -> bool
        member PeekMin : unit -> 'T
        override ToString : unit -> string
        member Count : int
    end
"""

[<Test>]
let ``type constraints complex``() =
    formatSourceString false """
type Class4<'T when 'T : (static member staticMethod1 : unit -> 'T) > =
    class end

type Class5<'T when 'T : (member Method1 : 'T -> int)> =
    class end

type Class6<'T when 'T : (member Property1 : int)> =
    class end

type Class7<'T when 'T : (new : unit -> 'T)>() =
   member val Field = new 'T()
    """ config
    |> prepend newline
    |> should equal """
type Class4<'T when 'T : (static member staticMethod1 : unit -> 'T)> =
    class
    end

type Class5<'T when 'T : (member Method1 : 'T -> int)> =
    class
    end

type Class6<'T when 'T : (member Property1 : int)> =
    class
    end

type Class7<'T when 'T : (new : unit -> 'T)>() =
    member val Field = new 'T()
"""

[<Test>]
let ``abstract classes``() =
    formatSourceString false """
[<AbstractClass>]
type Shape2D(x0 : float, y0 : float) =
    let mutable x, y = x0, y0
    let mutable rotAngle = 0.0

    member this.CenterX with get() = x and set xval = x <- xval
    member this.CenterY with get() = y and set yval = y <- yval

    abstract Area : float with get
    abstract Perimeter : float  with get
    abstract Name : string with get

    member this.Move dx dy =
       x <- x + dx
       y <- y + dy

    abstract member Rotate: float -> unit
    default this.Rotate(angle) = rotAngle <- rotAngle + angle
    """ config
    |> prepend newline
    |> should equal """
[<AbstractClass>]
type Shape2D(x0 : float, y0 : float) =
    let mutable x, y = x0, y0
    let mutable rotAngle = 0.0
    
    member this.CenterX 
        with get () = x
        and set xval = x <- xval
    
    member this.CenterY 
        with get () = y
        and set yval = y <- yval
    
    abstract Area : float
    abstract Perimeter : float
    abstract Name : string
    
    member this.Move dx dy =
        x <- x + dx
        y <- y + dy
    
    abstract Rotate : float -> unit
    override this.Rotate(angle) = rotAngle <- rotAngle + angle
"""

[<Test>]
let ``class declaration``() =
    formatSourceString false """
type BaseClass = class
    val string1 : string
    new(str) = { string1 = str }
    new() = { string1 = "" }
end

type DerivedClass =
    inherit BaseClass
    val string2 : string
    new (str1, str2) = { inherit BaseClass(str1); string2 = str2 }
    new (str2) = { inherit BaseClass(); string2 = str2 }""" config
    |> prepend newline
    |> should equal """
type BaseClass =
    class
        val string1 : string
        new(str) = { string1 = str }
        new() = { string1 = "" }
    end

type DerivedClass =
    inherit BaseClass
    val string2 : string
    new(str1, str2) = { inherit BaseClass(str1); string2 = str2 }
    new(str2) = { inherit BaseClass(); string2 = str2 }
"""

[<Test>]
let ``classes and implicit constructors``() =
    formatSourceString false """
    type MyClass2(dataIn) as self =
       let data = dataIn
       do self.PrintMessage()
       member this.PrintMessage() =
           printf "Creating MyClass2 with Data %d" data""" config
    |> prepend newline
    |> should equal """
type MyClass2(dataIn) as self =
    let data = dataIn
    do self.PrintMessage()
    member this.PrintMessage() = printf "Creating MyClass2 with Data %d" data
"""

[<Test>]
let ``classes and private implicit constructors``() =
    formatSourceString false """
    type MyClass2 private (dataIn) as self =
       let data = dataIn
       do self.PrintMessage()
       member this.PrintMessage() =
           printf "Creating MyClass2 with Data %d" data""" config
    |> prepend newline
    |> should equal """
type MyClass2 private (dataIn) as self =
    let data = dataIn
    do self.PrintMessage()
    member this.PrintMessage() = printf "Creating MyClass2 with Data %d" data
"""

[<Test>]
let ``recursive classes``() =
    formatSourceString false """
type Folder(pathIn: string) =
  let path = pathIn
  let filenameArray : string array = System.IO.Directory.GetFiles(path)
  member this.FileArray = Array.map (fun elem -> new File(elem, this)) filenameArray

and File(filename: string, containingFolder: Folder) = 
   member __.Name = filename
   member __.ContainingFolder = containingFolder""" config
    |> prepend newline
    |> should equal """
type Folder(pathIn : string) =
    let path = pathIn
    let filenameArray : string array = System.IO.Directory.GetFiles(path)
    member this.FileArray =
        Array.map (fun elem -> new File(elem, this)) filenameArray

and File(filename : string, containingFolder : Folder) =
    member __.Name = filename
    member __.ContainingFolder = containingFolder
"""

[<Test>]
let ``classes and inheritance``() =
    formatSourceString false """
type MyClassBase2(x: int) =
   let mutable z = x * x
   do for i in 1..z do printf "%d " i

type MyClassDerived2(y: int) =
   inherit MyClassBase2(y * 2)
   do for i in 1..y do printf "%d " i""" config
    |> prepend newline
    |> should equal """
type MyClassBase2(x : int) =
    let mutable z = x * x
    do 
        for i in 1..z do
            printf "%d " i

type MyClassDerived2(y : int) =
    inherit MyClassBase2(y * 2)
    do 
        for i in 1..y do
            printf "%d " i
"""

[<Test>]
let ``should keep parens in class definition in the right place``() =
    formatSourceString false """type DGMLClass() = class   
    let mutable currentState = System.String.Empty
    end
    """ config
    |> should equal """type DGMLClass() =
    class
        let mutable currentState = System.String.Empty
    end
"""

[<Test>]
let ``should keep parens in class inheritance in the right place``() =
    formatSourceString false """type StateMachine(makeAsync) as this = class
    inherit DGMLClass()

    let functions = System.Collections.Generic.Dictionary<string, IState>()
    end
    """ config
    |> should equal """type StateMachine(makeAsync) as this =
    class
        inherit DGMLClass()
        let functions = System.Collections.Generic.Dictionary<string, IState>()
    end
"""

[<Test>]
let ``should keep type annotations on auto properties``() =
    formatSourceString false """type Document(id : string, library : string, name : string option) = 
    member val ID = id
    member val Library = library
    member val Name = name with get, set
    member val LibraryID : string option = None with get, set
"""  config
    |> should equal """type Document(id : string, library : string, name : string option) =
    member val ID = id
    member val Library = library
    member val Name = name with get, set
    member val LibraryID : string option = None with get, set
"""

[<Test>]
let ``should work on static auto properties``() =
    formatSourceString false """type A() =
    static member val LastSchema = "" with get, set
"""  config
    |> should equal """type A() =
    static member val LastSchema = "" with get, set
"""
