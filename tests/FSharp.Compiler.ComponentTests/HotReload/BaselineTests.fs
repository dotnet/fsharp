namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.Collections.Generic
open System.Reflection

open Xunit

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryWriter
open FSharp.Compiler.HotReloadBaseline
open FSharp.Compiler.IlxGen
open FSharp.Reflection
open FSharp.Test
open FSharp.Test.Compiler
open Internal.Utilities

[<Collection(nameof NotThreadSafeResourceCollection)>]
module BaselineTests =

    let private mkSimpleMethodBody instrs =
        let code = nonBranchingInstrsToCode instrs
        mkMethodBody (false, [], 8, code, None, None)

    let private createSampleModule () =
        let ilg = PrimaryAssemblyILGlobals
        let intType = ilg.typ_Int32
        let objectType = ilg.typ_Object

        let staticMethod =
            mkILNonGenericStaticMethod (
                "GetValue",
                ILMemberAccess.Public,
                [ mkILParamNamed ("input", intType) ],
                mkILReturn intType,
                mkSimpleMethodBody
                    [ AI_ldc(DT_I4, ILConst.I4 1)
                      I_ret ]
            )

        let baseGetter =
            mkILNonGenericInstanceMethod (
                "get_Data",
                ILMemberAccess.Public,
                [],
                mkILReturn intType,
                mkSimpleMethodBody
                    [ AI_ldc(DT_I4, ILConst.I4 2)
                      I_ret ]
            )

        let getter =
            baseGetter.With(attributes = (baseGetter.Attributes ||| MethodAttributes.SpecialName ||| MethodAttributes.HideBySig))

        let baseSetter =
            mkILNonGenericInstanceMethod (
                "set_Data",
                ILMemberAccess.Public,
                [ mkILParamNamed ("value", intType) ],
                mkILReturn ILType.Void,
                mkSimpleMethodBody
                    [ I_ret ]
            )

        let setter =
            baseSetter.With(attributes = (baseSetter.Attributes ||| MethodAttributes.SpecialName ||| MethodAttributes.HideBySig))

        let baseAdd =
            mkILNonGenericInstanceMethod (
                "add_OnChanged",
                ILMemberAccess.Public,
                [ mkILParamNamed ("handler", objectType) ],
                mkILReturn ILType.Void,
                mkSimpleMethodBody
                    [ I_ret ]
            )

        let addHandler =
            baseAdd.With(attributes = (baseAdd.Attributes ||| MethodAttributes.SpecialName ||| MethodAttributes.RTSpecialName))

        let baseRemove =
            mkILNonGenericInstanceMethod (
                "remove_OnChanged",
                ILMemberAccess.Public,
                [ mkILParamNamed ("handler", objectType) ],
                mkILReturn ILType.Void,
                mkSimpleMethodBody
                    [ I_ret ]
            )

        let removeHandler =
            baseRemove.With(attributes = (baseRemove.Attributes ||| MethodAttributes.SpecialName ||| MethodAttributes.RTSpecialName))

        let fieldDef = mkILInstanceField ("valueBackingField", intType, None, ILMemberAccess.Private)

        let typeRef = mkILTyRef (ILScopeRef.Local, "Sample.Container")

        let propertyDef =
            ILPropertyDef(
                "Data",
                PropertyAttributes.None,
                Some(mkILMethRef (typeRef, ILCallingConv.Instance, "set_Data", 0, [ intType ], ILType.Void)),
                Some(mkILMethRef (typeRef, ILCallingConv.Instance, "get_Data", 0, [], intType)),
                ILThisConvention.Instance,
                intType,
                None,
                [],
                emptyILCustomAttrs
            )

        let eventDef =
            ILEventDef(
                Some objectType,
                "OnChanged",
                EventAttributes.None,
                mkILMethRef (typeRef, ILCallingConv.Instance, "add_OnChanged", 0, [ objectType ], ILType.Void),
                mkILMethRef (typeRef, ILCallingConv.Instance, "remove_OnChanged", 0, [ objectType ], ILType.Void),
                None,
                [],
                emptyILCustomAttrs
            )

        let typeDef =
            mkILSimpleClass
                ilg
                (
                    "Sample.Container",
                    ILTypeDefAccess.Public,
                    mkILMethods [ staticMethod; getter; setter; addHandler; removeHandler ],
                    mkILFields [ fieldDef ],
                    emptyILTypeDefs,
                    mkILProperties [ propertyDef ],
                    mkILEvents [ eventDef ],
                    emptyILCustomAttrs,
                    ILTypeInit.BeforeField
                )

        mkILSimpleModule
            "SampleAssembly"
            "SampleModule"
            true
            (4, 0)
            false
            (mkILTypeDefs [ typeDef ])
            None
            None
            0
            (mkILExportedTypes [])
            "v4.0.30319"

    let private sampleBaselineArtifacts () =
        let ilModule = createSampleModule ()

        let metadataSnapshot : MetadataSnapshot =
            { HeapSizes =
                { StringHeapSize = 128
                  UserStringHeapSize = 64
                  BlobHeapSize = 256
                  GuidHeapSize = 16 }
              TableRowCounts = Array.create 64 0
              GuidHeapStart = 0 }

        let typeTokenMap = dict [ "Sample.Container", 0x02000001 ]

        let methodTokenMap =
            dict [
                "Sample.Container",
                dict [
                    "GetValue", 0x06000001
                    "get_Data", 0x06000002
                    "set_Data", 0x06000003
                    "add_OnChanged", 0x06000004
                    "remove_OnChanged", 0x06000005
                ]
            ]

        let fieldTokenMap = dict [ "Sample.Container", dict [ "valueBackingField", 0x04000001 ] ]

        let propertyTokenMap = dict [ "Sample.Container", dict [ "Data", 0x17000001 ] ]

        let eventTokenMap = dict [ "Sample.Container", dict [ "OnChanged", 0x14000001 ] ]

        let tokenMappings : ILTokenMappings =
            { TypeDefTokenMap = (fun (_enc, tdef) -> typeTokenMap[tdef.Name])
              FieldDefTokenMap = (fun (_enc, tdef) field -> fieldTokenMap[tdef.Name][field.Name])
              MethodDefTokenMap = (fun (_enc, tdef) mdef -> methodTokenMap[tdef.Name][mdef.Name])
              PropertyTokenMap = (fun (_enc, tdef) prop -> propertyTokenMap[tdef.Name][prop.Name])
              EventTokenMap = (fun (_enc, tdef) ev -> eventTokenMap[tdef.Name][ev.Name]) }

        ilModule, tokenMappings, metadataSnapshot

    let private emitBaseline () =
        let ilModule, tokenMappings, metadataSnapshot = sampleBaselineArtifacts ()
        let moduleId = System.Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")
        create ilModule tokenMappings metadataSnapshot moduleId None

    [<Fact>]
    let ``baseline captures method semantics entries`` () =
        let baseline = emitBaseline ()
        let ilg = PrimaryAssemblyILGlobals

        let propertyGetterKey =
            { MethodDefinitionKey.DeclaringType = "Sample.Container"
              Name = "get_Data"
              GenericArity = 0
              ParameterTypes = []
              ReturnType = ilg.typ_Int32 }

        let propertySetterKey =
            { MethodDefinitionKey.DeclaringType = "Sample.Container"
              Name = "set_Data"
              GenericArity = 0
              ParameterTypes = [ ilg.typ_Int32 ]
              ReturnType = ILType.Void }

        let eventAdderKey =
            { MethodDefinitionKey.DeclaringType = "Sample.Container"
              Name = "add_OnChanged"
              GenericArity = 0
              ParameterTypes = [ ilg.typ_Object ]
              ReturnType = ILType.Void }

        let eventRemoverKey =
            { eventAdderKey with
                Name = "remove_OnChanged"
                ParameterTypes = [ ilg.typ_Object ] }

        let getterSemantics = baseline.MethodSemanticsEntries[propertyGetterKey] |> List.exactlyOne
        Assert.Equal(2, getterSemantics.RowId)
        Assert.Equal(MethodSemanticsAttributes.Getter, getterSemantics.Attributes)

        match getterSemantics.Association with
        | MethodSemanticsAssociation.PropertyAssociation(_, rowId) -> Assert.Equal(1, rowId)
        | _ -> failwith "Expected property association for getter."

        let setterSemantics = baseline.MethodSemanticsEntries[propertySetterKey] |> List.exactlyOne
        Assert.Equal(1, setterSemantics.RowId)
        Assert.Equal(MethodSemanticsAttributes.Setter, setterSemantics.Attributes)

        match setterSemantics.Association with
        | MethodSemanticsAssociation.PropertyAssociation(_, rowId) -> Assert.Equal(1, rowId)
        | _ -> failwith "Expected property association for setter."

        let adderSemantics = baseline.MethodSemanticsEntries[eventAdderKey] |> List.exactlyOne
        Assert.Equal(3, adderSemantics.RowId)
        Assert.Equal(MethodSemanticsAttributes.Adder, adderSemantics.Attributes)

        match adderSemantics.Association with
        | MethodSemanticsAssociation.EventAssociation(_, rowId) -> Assert.Equal(1, rowId)
        | _ -> failwith "Expected event association for adder."

        let removerSemantics = baseline.MethodSemanticsEntries[eventRemoverKey] |> List.exactlyOne
        Assert.Equal(4, removerSemantics.RowId)
        Assert.Equal(MethodSemanticsAttributes.Remover, removerSemantics.Attributes)

        match removerSemantics.Association with
        | MethodSemanticsAssociation.EventAssociation(_, rowId) -> Assert.Equal(1, rowId)
        | _ -> failwith "Expected event association for remover."

    let private createDummySnapshot () =
        let snapshotType = typeof<IlxGenEnvSnapshot>
        let fields =
            FSharpType.GetRecordFields(
                snapshotType,
                BindingFlags.NonPublic
                ||| BindingFlags.Public
            )

        let values =
            fields
            |> Array.map (fun field ->
                if field.PropertyType.IsValueType then
                    Activator.CreateInstance(field.PropertyType)
                else
                    null)

        FSharpValue.MakeRecord(snapshotType, values, true) :?> IlxGenEnvSnapshot

    [<Fact>]
    let ``baseline tokens are stable across emissions`` () =
        let first = emitBaseline ()
        let second = emitBaseline ()

        Assert.Equal<Map<string, int>>(first.TypeTokens, second.TypeTokens)
        Assert.Equal<Map<MethodDefinitionKey, int>>(first.MethodTokens, second.MethodTokens)
        Assert.Equal<Map<FieldDefinitionKey, int>>(first.FieldTokens, second.FieldTokens)
        Assert.Equal<Map<PropertyDefinitionKey, int>>(first.PropertyTokens, second.PropertyTokens)
        Assert.Equal<Map<EventDefinitionKey, int>>(first.EventTokens, second.EventTokens)

    [<Fact>]
    let ``baseline captures expected members`` () =
        let baseline = emitBaseline ()
        let ilg = PrimaryAssemblyILGlobals

        Assert.True(Map.containsKey "Sample.Container" baseline.TypeTokens)

        let methodKey =
            { DeclaringType = "Sample.Container"
              Name = "GetValue"
              GenericArity = 0
              ParameterTypes = [ ilg.typ_Int32 ]
              ReturnType = ilg.typ_Int32 }

        Assert.True(Map.containsKey methodKey baseline.MethodTokens)

        let fieldKey =
            { DeclaringType = "Sample.Container"
              Name = "valueBackingField"
              FieldType = ilg.typ_Int32 }

        Assert.True(Map.containsKey fieldKey baseline.FieldTokens)

        let propertyKey =
            { DeclaringType = "Sample.Container"
              Name = "Data"
              PropertyType = ilg.typ_Int32
              IndexParameterTypes = [] }

        Assert.True(Map.containsKey propertyKey baseline.PropertyTokens)

        let eventKey =
            { DeclaringType = "Sample.Container"
              Name = "OnChanged"
              EventType = Some ilg.typ_Object }

        Assert.True(Map.containsKey eventKey baseline.EventTokens)

    [<Fact>]
    let ``metadata snapshot captures heap lengths`` () =
        let baseline = emitBaseline ()
        let heaps = baseline.Metadata.HeapSizes

        Assert.True(heaps.StringHeapSize > 0)
        Assert.True(heaps.BlobHeapSize >= 0)
        Assert.Equal(64, baseline.Metadata.TableRowCounts.Length)
        Assert.True(baseline.Metadata.GuidHeapStart >= 0)

    [<Fact>]
    let ``createWithEnvironment retains snapshot reference`` () =
        let ilModule, tokenMappings, metadataSnapshot = sampleBaselineArtifacts ()
        let snapshot = createDummySnapshot ()

        let moduleId = System.Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")
        let baseline = createWithEnvironment ilModule tokenMappings metadataSnapshot snapshot moduleId None

        Assert.True(baseline.IlxGenEnvironment.IsSome)
        Assert.True(obj.ReferenceEquals(snapshot, baseline.IlxGenEnvironment.Value))

    [<Fact>]
    let ``compile with hot reload flag captures baseline`` () =
        let service = global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance
        service.EndSession()

        FSharp """
module Sample

let mutable state = 1
"""
        |> withOptions [ "--langversion:preview"; "--debug+"; "--optimize-"; "--enable:hotreloaddeltas" ]
        |> compile
        |> shouldSucceed
        |> ignore

        match service.TryGetBaseline() with
        | ValueSome baseline ->
            Assert.True(baseline.IlxGenEnvironment.IsSome)
            service.EndSession()
        | ValueNone ->
            Assert.True(false, "Expected hot reload baseline to be captured.")
