// Ensures that we get an error when warnings as error specified



namespace FSharp.Conformance.DeclarationElements.CustomAttributes.InheritedAttribute

open System

[<AttributeUsage(AttributeTargets.All, Inherited=false, AllowMultiple=true)>]
type Author(name: string) = class end

[<Author("P. Ackerman")>]
type FirstClass() = class end
