﻿namespace Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

module ExtensionMethodTests =

    [<Fact>]
    let ``Extension method with toplevel attribute on type`` () =
        Fsx
            """
open System.Runtime.CompilerServices

[<Extension>]
type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1

let f (b:int) = b.PlusOne()
            """
        |> withLangVersion80
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Extension method without toplevel attribute on type`` () =
        Fsx
            """
open System.Runtime.CompilerServices

type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1

let f (b:int) = b.PlusOne()
            """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
        
    [<Fact>]
    let ``Extension method without toplevel attribute on type lang version 7`` () =
        Fsx
            """
open System.Runtime.CompilerServices

type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1

let f (b:int) = b.PlusOne()
            """
        |> withLangVersion70
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 8, Col 19, Line 8, Col 26, "The type 'Int32' does not define the field, constructor or member 'PlusOne'.")
        ]

    [<Fact>]
    let ``Extension method without toplevel attribute on recursive type`` () =
        Fsx
            """
open System.Runtime.CompilerServices

type Foo =
    class
    end
and Bar =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1

let f (b:int) = b.PlusOne()
            """
        |> withLangVersion80
        |> compile
        |> shouldSucceed
    
    [<Fact>]
    let ``F# CSharpStyleExtensionMethod consumed in C#`` () =
        let fsharp =
            FSharp
                """
module Hello

open System.Runtime.CompilerServices

type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1
"""
            |> withLangVersion80
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed
        
    [<Fact>]
    let ``F# lang version 7 CSharpStyleExtensionMethod consumed in C#`` () =
        let fsharp =
            FSharp
                """
module Hello

open System.Runtime.CompilerServices

type Foo =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1
"""
            |> withLangVersion70
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1061, Line 9, Col 25, Line 9, Col 32, "'int' does not contain a definition for 'PlusOne' and no accessible extension method 'PlusOne' accepting a first argument of type 'int' could be found (are you missing a using directive or an assembly reference?)")
        ]

    [<Fact>]
    let ``F# CSharpStyleExtensionMethod in recursive type consumed in C#`` () =
        let fsharp =
            FSharp
                """
module Hello

open System.Runtime.CompilerServices

type Foo =
    class
    end
and Bar =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1
"""
            |> withLangVersion80
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Bar;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed
    
    [<Fact>]
    let ``F# CSharpStyleExtensionMethod defined in top level module with attribute consumed in C#`` () =
        let fsharp =
            FSharp
                """
namespace Hello

open System.Runtime.CompilerServices

[<Extension>]
module Foo =
    [<Extension>]
    let PlusOne (a:int) : int = a + 1
"""
            |> withLangVersion80
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``F# CSharpStyleExtensionMethod defined in top level module without attribute consumed in C#`` () =
        let fsharp =
            FSharp
                """
namespace Hello

open System.Runtime.CompilerServices

module Foo =
    [<Extension>]
    let PlusOne (a:int) : int = a + 1
"""
            |> withLangVersion80
            |> withName "FSLib"

        let csharp =
            CSharp
                """
namespace Consumer
{
    using static Hello.Foo;

    public class Class1
    {
        public Class1()
        {
            var meh = 1.PlusOne();
        }
    }
}
"""
            |> withName "CSLib"
            |> withReferences [ fsharp ]

        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``Toplevel named module with Extension attribute and top level let binding with Extension attribute`` () =
        let fsharp =
            FSharp """
    [<System.Runtime.CompilerServices.Extension>]
    module Foo

    [<System.Runtime.CompilerServices.Extension>]
    let PlusOne (a:int) = a + 1
    """
           |> withLangVersion80
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """

            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``Toplevel named module without Extension attribute and top level let binding with Extension attribute`` () =
        let fsharp =
            FSharp """
    module Foo

    [<System.Runtime.CompilerServices.Extension>]
    let PlusOne (a:int) = a + 1
    """
           |> withLangVersion80
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """

            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``Recursive toplevel named module with Extension attribute and top level let binding with Extension attribute`` () =
        let fsharp =
            FSharp """
    [<System.Runtime.CompilerServices.Extension>]
    module rec Foo

    [<System.Runtime.CompilerServices.Extension>]
    let PlusOne (a:int) = a + 1
    """
           |> withLangVersion80
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """

            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed
    
    [<Fact>]
    let ``Recursive toplevel named module without Extension attribute and top level let binding with Extension attribute`` () =
        let fsharp =
            FSharp """
    module rec Foo

    [<System.Runtime.CompilerServices.Extension>]
    let PlusOne (a:int) = a + 1
    """
           |> withLangVersion80
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """

            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``Foobar `` () =
        let fsharp =
            FSharp """
module rec Foo

[<System.Runtime.CompilerServices.Extension>]
type Bar =
    [<System.Runtime.CompilerServices.Extension>]
    static member PlusOne (a:int) = a + 1
    """
           |> withLangVersion80
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo.Bar;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """
            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed
    
    [<Fact>]
    let ``Recursive named module with type with CSharp style extension can be consumed in CSharp`` () =
        let fsharp =
            FSharp """
module rec Foo

type Bar =
    [<System.Runtime.CompilerServices.Extension>]
    static member PlusOne (a:int) = a + 1
    """
            |> withLangVersion80
            |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo.Bar;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """
            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``CSharp style extension method in F# type backed by a signature`` () =
        let implementation =
            SourceCodeFileKind.Create(
                "Source.fs",
                """
module Foo

open System.Runtime.CompilerServices

type Bar =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1
"""
            )

        let fsharp =
            Fsi """
module Foo

open System.Runtime.CompilerServices

[<Class>]
type Bar =
    [<Extension>]
    static member PlusOne: a: int -> int
"""
           |> withLangVersion80
           |> withAdditionalSourceFile implementation
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo.Bar;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """
            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``CSharp style extension method in F# type backed by a signature in a recursive module`` () =
        let implementation =
            SourceCodeFileKind.Create(
                "Source.fs",
                """
module rec Foo

open System.Runtime.CompilerServices

type Bar =
    [<Extension>]
    static member PlusOne (a:int) : int = a + 1
"""
            )

        let fsharp =
            Fsi """
module rec Foo

open System.Runtime.CompilerServices

[<Class>]
type Bar =
    [<Extension>]
    static member PlusOne: a: int -> int
"""
           |> withLangVersion80
           |> withAdditionalSourceFile implementation
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo.Bar;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne();
            }
        }
    }
    """
            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed

    [<Fact>]
    let ``Multiple top level let binding with Extension attribute`` () =
        let fsharp =
            FSharp """
    module Foo

    [<System.Runtime.CompilerServices.Extension>]
    let PlusOne (a:int) = a + 1
    
    [<System.Runtime.CompilerServices.Extension>]
    let MinusOne (a:int) = a - 1
    """
           |> withLangVersion80
           |> withName "FSLib"
        
        let csharp =
            CSharp """
    namespace Consumer
    {
        using static Foo;

        public class Class1
        {
            public Class1()
            {
                var meh = 1.PlusOne().MinusOne();
            }
        }
    }
    """

            |> withName "CSLib"
            |> withReferences [ fsharp ]
        
        csharp |> compile |> shouldSucceed
