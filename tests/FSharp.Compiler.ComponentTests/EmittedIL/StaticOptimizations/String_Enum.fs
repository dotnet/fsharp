open System

module String =
    type CharEnum = Char = 'a'
    type SByteEnum = SByte = 1y
    type Int16Enum = Int16 = 1s
    type Int32Enum = Int32 = 1
    type Int64Enum = Int64 = 1L

    type ByteEnum = Byte = 1uy
    type UInt16Enum = UInt16 = 1us
    type UInt32Enum = UInt32 = 1u
    type UInt64Enum = UInt64 = 1UL

    let ``string<CharEnum>`` (enum : CharEnum) = string enum
    let ``string<SByteEnum>`` (enum : SByteEnum) = string enum
    let ``string<Int16Enum>`` (enum : Int16Enum) = string enum
    let ``string<Int32Enum>`` (enum : Int32Enum) = string enum
    let ``string<Int64Enum>`` (enum : Int64Enum) = string enum

    let ``string<ByteEnum>`` (enum : ByteEnum) = string enum
    let ``string<UInt16Enum>`` (enum : UInt16Enum) = string enum
    let ``string<UInt32Enum>`` (enum : UInt32Enum) = string enum
    let ``string<UInt64Enum>`` (enum : UInt64Enum) = string enum

    let ``string<#Enum>`` (enum : #Enum) = string enum
    let ``string<'T :> Enum>`` (enum : 'T :> Enum) = string enum

    let ``string<'T when 'T : enum<'U>>`` (enum : 'T when 'T : enum<'U>) = string enum
    let ``string<'T when 'T : enum<int>>`` (enum : 'T when 'T : enum<int>) = string enum

    let ``string Unchecked.defaultof<System.Enum>`` () = string Unchecked.defaultof<System.Enum>
