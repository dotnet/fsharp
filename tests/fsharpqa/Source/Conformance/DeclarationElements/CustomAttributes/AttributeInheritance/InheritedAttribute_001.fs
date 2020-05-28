// Ensures that we get an error when warnings as error specified

//<Expects id="FS3242" span="(12,3)" status="error">This type does not inherit Attribute, it will not work correctly with other .NET languages.</Expects>

namespace FSharp.Conformance.DeclaratioElements.CustomAttributes.InheritedAttribute

open System

[<AttributeUsage(AttributeTargets.All, Inherited=false, AllowMultiple=true)>]
type Author(name: string) = class end

[<Author("P. Ackerman")>]
type FirstClass() = class end
