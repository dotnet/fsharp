open System.Text.Json.Serialization

type internal ApplicationTenantJsonDerivedTypeAttribute() =
    inherit JsonDerivedTypeAttribute (typeof<ApplicationTenant>, "a")

and [<ApplicationTenantJsonDerivedType>] ApplicationTenant [<JsonConstructor>] (id, name, loginProvider, allowedDomains, authorizedTenants, properties) =
    member _.Id = ""