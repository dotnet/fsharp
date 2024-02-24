// https://github.com/dotnet/fsharp/issues/12085
module NoTailcallToByrefsWithModReq =
    module ClassLibrary =

        // F# equivalent of `public delegate FieldType Getter<DeclaringType, FieldType>(in DeclaringType);`
        type Getter<'DeclaringType, 'FieldType> = delegate of inref<'DeclaringType> -> 'FieldType 

        type GetterWrapper<'DeclaringType, 'FieldType> (getter : Getter<'DeclaringType, 'FieldType>) =
            member _.Get (instance : 'DeclaringType) = getter.Invoke &instance
    
    open ClassLibrary
    type MyRecord = { Value: int[] }
    let App() =
        
        let wrapper = new GetterWrapper<MyRecord, int[]>(fun (record: inref<MyRecord>) -> record.Value)

        let record = { Value = [| 42 |] }
        let value = wrapper.Get(record)
        value.GetEnumerator()
    App()