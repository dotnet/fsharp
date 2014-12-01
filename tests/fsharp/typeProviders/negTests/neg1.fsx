#r "provider.dll"

let s : string = FSharp.GoodProviderForNegativeTypeTests1.TheType.Foo

// This one exposes the types of a provider, one of which is an array
open FSharp.EvilProvider

// It is enough to name the type to expose the validation check
type Negative1 = FSharp.EvilProvider.IsArrayTypeRaisesException
type Negative2 = FSharp.EvilProvider.IsArrayTypeReturnsTrue
type Negative3 = FSharp.EvilProvider.TypeWhereNameRaisesException
type Negative4 = FSharp.EvilProvider.TypeWhereNameReturnsNull
type Negative5 = FSharp.EvilProvider.IsGenericTypeRaisesException
type Negative6 = FSharp.EvilProvider.IsGenericTypeReturnsTrue
type Negative7 = FSharp.EvilProvider.TypeWhereFullNameRaisesException
type Negative8 = FSharp.EvilProvider.TypeWhereFullNameReturnsNull
type Negative9 = FSharp.EvilProvider.TypeWhereNamespaceRaisesException
type Negative10 = FSharp.EvilProvider.TypeWhereNamespaceReturnsNull
type Negative11 = FSharp.EvilProvider.DeclaringTypeRaisesException
type Negative12 = FSharp.EvilProvider.TypeWhereGetMethodsRaisesException
type Negative13 = FSharp.EvilProvider.TypeWhereGetEventsRaisesException
type Negative14 = FSharp.EvilProvider.TypeWhereGetFieldsRaisesException
type Negative15 = FSharp.EvilProvider.TypeWhereGetPropertiesRaisesException
type Negative16 = FSharp.EvilProvider.TypeWhereGetNestedTypesRaisesException
type Negative17 = FSharp.EvilProvider.TypeWhereGetConstructorsRaisesException
//type Negative18 = FSharp.EvilProvider.TypeWhereGetInterfacesRaisesException
type Negative19 = FSharp.EvilProvider.TypeWhereGetMethodsReturnsNull
type Negative20 = FSharp.EvilProvider.TypeWhereGetEventsReturnsNull
type Negative21 = FSharp.EvilProvider.TypeWhereGetFieldsReturnsNull
type Negative22 = FSharp.EvilProvider.TypeWhereGetPropertiesReturnsNull
type Negative23 = FSharp.EvilProvider.TypeWhereGetNestedTypesReturnsNull
type Negative24 = FSharp.EvilProvider.TypeWhereGetConstructorsReturnsNull
type Negative25 = FSharp.EvilProvider.TypeWhereGetInterfacesReturnsNull


type Positive1 = FSharp.EvilProvider.TypeWhereGetGenericArgumentsRaisesException
type Positive2 = FSharp.EvilProvider.TypeWhereGetMembersRaisesException



module Int32 = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<true>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const true>.StaticProperty1 |> ignore


module SByte = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<'1'>.StaticProperty1 |> ignore

        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSByteParameter<const true>.StaticProperty1 |> ignore

module Int16 = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<true>.StaticProperty1 |> ignore

        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt16Parameter<const true>.StaticProperty1 |> ignore


module Int64 = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<true>.StaticProperty1 |> ignore

        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt64Parameter<const true>.StaticProperty1 |> ignore


module UInt64 = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<true>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt64Parameter<const true>.StaticProperty1 |> ignore


module UInt32 = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<true>.StaticProperty1 |> ignore

        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt32Parameter<const true>.StaticProperty1 |> ignore


module UInt16 = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<true>.StaticProperty1 |> ignore

        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticUInt16Parameter<const true>.StaticProperty1 |> ignore


module Byte = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<true>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticByteParameter<const true>.StaticProperty1 |> ignore


module Bool = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<1uy>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticBoolParameter<const 1>.StaticProperty1 |> ignore


module String = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<true>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<null>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const true>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<const null>.StaticProperty1 |> ignore

module Double = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<true>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<1uy>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticDoubleParameter<const true>.StaticProperty1 |> ignore



module Single = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<true>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<'1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<1uy>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const '1'>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticSingleParameter<const true>.StaticProperty1 |> ignore


module Char = 
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<"1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<true>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<1uy>.StaticProperty1 |> ignore
        
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1L>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1y>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1UL>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1uy>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1u>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1us>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1.0M>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1.0>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1.0f>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const "1">.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const 1>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticCharParameter<const true>.StaticProperty1 |> ignore

module ConstExpr =

        [<Literal>]
        let s = "abc"
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const s>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<s>.StaticProperty1 |> ignore
        type T1 = FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<const s>
        type T2 = FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<s>




module TooManyArgs =

        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<3,2>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<3,Count=2>.StaticProperty1 |> ignore
        FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticInt32Parameter<Count=3,Count=2>.StaticProperty1 |> ignore
        //FSharp.GoodProviderForNegativeStaticParameterTypeTests.HelloWorldTypeWithStaticStringParameter<s>.StaticProperty1 |> ignore

module NullLiteralNotAllowed =

    let v = (null : FSharp.HelloWorld.HelloWorldType) // should give a type error - this explicitly has AllowNullLiteralAttribute(false), so a null literal is not allowed
