namespace Language

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

    [<Fact>]
    let ``Abbreviated CSharp type with extensions`` () =
        let csharp =
            CSharp """
namespace CSharpLib {

    public interface I
    {
        public int P { get; }
    }

    public static class Ext
    {
        public static void M(this I i)
        {
        }
    }
}
    """
            |> withName "CSLib"
        
        let fsharp =
            FSharp """
module Module

open CSharpLib

module M =
    type Ext2 = CSharpLib.Ext
 
    let f (i: I) =
        i.M()
"""
           |> withLangVersion80
           |> withName "FSLib"
           |> withReferences [ csharp ]
        
        fsharp |> compile |> shouldSucceed
        
    [<Fact>]
    let ``F# CSharpStyleExtensionMethod consumed in F#`` () =
        let producer =
            FSharp
                """
namespace Producer

open System.Runtime.CompilerServices

type WidgetBuilder<'msg, 'marker>() = class end

type IMarkerOne = interface end

// Commenting out [<Extension>] breaks
//[<Extension>]
type WidgetBuilderExtensions =
    [<Extension>]
    static member inline one(this: WidgetBuilder<'msg, #IMarkerOne>) = this
"""
            |> withLangVersion80
            |> withName "FSLibProducer"

        let fsharp2 =
            FSharp
                """
namespace Consumer

open Producer

module FSLibConsumer =   
    let x = WidgetBuilder<int, IMarkerOne>().one()
"""
            |> withName "FSLibConsumer"
            |> withReferences [ producer ]

        fsharp2 |> compile |> shouldSucceed

    [<Fact>]
    let ``Static extension members for types with same simple name but different namespaces should error`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        static member CompiledStaticExtension() = ()

    type Task with
        static member CompiledStaticExtension() = ()
            """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3356, Line 11, Col 23, Line 11, Col 46, "Extension members extending types with the same simple name 'Task' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
        ]

    [<Fact>]
    let ``Static extension members for types with same simple name in different modules should succeed`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions1 =
    type System.Threading.Tasks.Task with
        static member CompiledStaticExtension() = ()

module CompiledExtensions2 =
    type Task with
        static member CompiledStaticExtension() = ()
            """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Static extension members with nested module in between should error`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    // First extension for System.Threading.Tasks.Task
    type System.Threading.Tasks.Task with
        static member Extension1() = ()

    // Nested module - this is fine, shouldn't interfere with duplicate check
    module Nested =
        let someValue = 42
        type OtherType = { X: int }

    // Some other definition
    let someBinding = 10

    // Second extension for local Task type - this should clash with the first
    type Task with
        static member Extension2() = ()
            """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3356, Line 21, Col 23, Line 21, Col 33, "Extension members extending types with the same simple name 'Task' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
        ]

    [<Fact>]
    let ``Instance extension members for types with same simple name should succeed`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        member _.InstanceExtension() = ()

    type Task with
        member _.InstanceExtension() = ()
            """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Extension members on generic types with same simple name should error`` () =
        Fsx
            """
module MyModule

// Define a local List type different from System.Collections.Generic.List
type List<'T> = { Items: 'T array }

module Extensions =
    type System.Collections.Generic.List<'T> with
        static member Count(lst: System.Collections.Generic.List<'T>) = lst.Count

    type List<'T> with
        static member Count(lst: List<'T>) = lst.Items.Length
            """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3356, Line 12, Col 23, Line 12, Col 28, "Extension members extending types with the same simple name 'List`1' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
        ]

    [<Fact>]
    let ``Extension members with different member names but same type simple name should error`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        static member FirstExtension() = ()

    type Task with
        static member DifferentName() = ()  // Different member name, but still same simple type name
            """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3356, Line 11, Col 23, Line 11, Col 36, "Extension members extending types with the same simple name 'Task' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
        ]

    [<Fact>]
    let ``Extensions defined in nested modules should succeed - separate IL containers`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module OuterModule =
    module NestedModule1 =
        type System.Threading.Tasks.Task with
            static member Extension1() = ()

    module NestedModule2 =
        type Task with
            static member Extension2() = ()
            """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Multiple extension members for same duplicate type should error per member`` () =
        // Note: The current implementation reports an error per extension member (not per type)
        // because each Val has its own range. This is more informative as it shows all problematic locations.
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        static member Extension1() = ()
        static member Extension2() = ()

    type Task with
        static member Extension3() = ()
        static member Extension4() = ()
            """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3356, Line 9, Col 23, Line 9, Col 33, "Extension members extending types with the same simple name 'Task' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
            (Error 3356, Line 12, Col 23, Line 12, Col 33, "Extension members extending types with the same simple name 'Task' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
            (Error 3356, Line 13, Col 23, Line 13, Col 33, "Extension members extending types with the same simple name 'Task' but different fully qualified names cannot be defined in the same module. Consider defining these extensions in separate modules.")
        ]

    [<Fact>]
    let ``Instance inline extension members on builder types with same simple name should succeed`` () =
        Fsx
            """
namespace NsA
type BuilderBase() = class end

namespace NsB
type BuilderBase() = class end

namespace Extensions
module M =
    type NsA.BuilderBase with
        member inline this.Bind(x: int, f) = f x
        member inline this.ReturnFrom(x: int) = x

    type NsB.BuilderBase with
        member inline this.Source(x: int) = x
        member inline this.Bind(x: string, f) = f x
            """
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Mixed static and instance extension members - only static should error`` () =
        Fsx
            """
module Compiled

type Task = { F: int }

module CompiledExtensions =
    type System.Threading.Tasks.Task with
        static member StaticExt() = ()

    type Task with
        member _.InstanceExt() = ()
            """
        |> compile
        |> shouldSucceed
