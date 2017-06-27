module TypeProvidersBug.Test

open FSharp.Configuration

type Configuration = YamlConfig<YamlText = "Foo: foo">

let config = Configuration()
