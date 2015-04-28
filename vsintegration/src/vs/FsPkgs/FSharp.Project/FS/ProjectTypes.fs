namespace Microsoft.VisualStudio.FSharp.ProjectSystem

open System

[<AutoOpen>]
module ProjectTypes = 
    [<RequireQualifiedAccess>]
    type MSBuildProjectItem = 
        | Reference
        | ComReference
        | ComFileReference
        | NativeReference
        | ProjectReference
        | Compile
        | EmbeddedResource
        | Content
        | None
        | Folder
        | Item of string
    
//    and MSBuildProjectItemComReference = 
//        { Guid: System.Guid option;
//          VersionMajor: int option;
//          VersionMinor: int option;
//          Lcid: string option;
//          Isolated: bool option;
//          WrapperTool: string option }
    
    type MSBuildProjectItemWrapper<'a> = 
        { Item : 'a;
          Include: Uri;
          Link : Uri option;
          Content : MSBuildProjectItem }
    
    [<RequireQualifiedAccess>]
    type ProjectTreeNode<'T> = 
        | Folder of string
        | Item of string * 'T
