// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2467
//<Expects id="FS3565" span="(9,115-9,117)" status="error">Expecting type</Expects>

#light

type ProjectFlavoring = {
     Create:(*projectFilename:*)string->(*files:*)string list->(*references:*)string list->(*projReferences:*)string list->
            (*disabledWarnings:*)string list->(*versionFile:*)string->(*otherFlags:*)string->(*otherMSBuildStuff*)->unit
     AppendProjectExtension:string->string
     MakeHierarchy:string->string->string->IVsHierarchy
     AddFileToHierarchy:string->IVsHierarchy->unit
     Build:string->string
     Destroy:unit->unit
     ModifyConfiguration:string->unit
}
