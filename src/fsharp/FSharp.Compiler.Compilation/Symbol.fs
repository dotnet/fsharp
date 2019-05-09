namespace FSharp.Compiler.Compilation

open System.Collections.Immutable
open FSharp.Compiler.Range

type TypeConstraintFlags =
    | Nullable = 0
    | Equality = 1
    | Struct = 2
    | Unmanaged = 3

type TypeConstraint =
    {
        Flags: TypeConstraintFlags
        // TODO: add more
    }

type TypeParameter =
    {
        Name: string
        Constraints: ImmutableArray<TypeConstraint>
        Range: range
    }

type ParameterSymbol =
    {
        Name: string option
        Type: uint32
    }

type MemberMethodSymbol =
    {
        Name: string
        TypeParameters: ImmutableArray<TypeParameter>
        Parameters: ImmutableArray<ParameterSymbol>
        ReturnType: uint32
        IsInstance: bool
        DeclaringType: uint32
        Accessibility: ImmutableArray<uint32> 
    }

type AssemblyOrModuleSymbol =
    {
        Name: string
    }

type NamedTypeSymbol =
    {
        QualifiedName: string
        AssemblyOrModule: uint32
        TypeParameters: ImmutableArray<TypeParameter>
        MemberMethods: ImmutableArray<uint32>
        Accessibility: ImmutableArray<uint32> 
    }

type TypeAbbreviationSymbol =
    {
        QualifiedName: string
        AssemblyOrModule: uint32
        TypeParameters: ImmutableArray<TypeParameter>
        Type: uint32
        Accessibility: ImmutableArray<uint32> 
    }

type TypeSymbol =
    | Unresolved
    | Abbreviation of TypeAbbreviationSymbol
    | Array 
    | Pointer
    | Named of NamedTypeSymbol

type Symbol =
    | Assembly
    | Event
    | Field
    | Label
    | Local
    | MemberMethod of MemberMethodSymbol
    | Function
    | Namespace
    | Module 
    | Parameter of ParameterSymbol 
    | Property 