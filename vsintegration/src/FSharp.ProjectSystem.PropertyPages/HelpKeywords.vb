' Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

'
' All help keywords for anything inside this assembly should be defined here,
'   so that it is easier for UE to find them.
'


Option Explicit On
Option Strict On
Option Compare Binary



'****************************************************
'*****  Property Page Help IDs
'****************************************************

Namespace Microsoft.VisualStudio.Editors.Common

    Friend NotInheritable Class HelpKeywords

        Friend Const FBProjPropDebug As String = "fs.ProjectPropertiesDebug"
        Friend Const FSProjPropApplication As String = "fs.ProjectPropertiesApplication"
        Friend Const FSProjPropBuild As String = "fs.ProjectPropertiesBuild"
        Friend Const FSProjPropBuildEvents As String = "fs.ProjectPropertiesBuildEvents"
        Friend Const FSProjPropBuildEventsBuilder As String = "fs.ProjectPropertiesBuildEventsBuilder"
        Friend Const FSProjPropReference As String = "fs.ProjectPropertiesReferencePaths"
        Friend Const FSProjPropReferencePaths As String = "fs.ProjectPropertiesReferencePaths"
    End Class

End Namespace



'****************************************************
'*****  Resource Editor Help IDs
'****************************************************

Namespace FSharpPropPages.Microsoft.VisualStudio.Editors.ResourceEditor

    Friend NotInheritable Class HelpIDs

        'General errors
        Public Const Err_CantFindResourceFile As String = "msvse_resedit.Err.CantFindResourceFile"
        Public Const Err_LoadingResource As String = "msvse_resedit.Err.LoadingResource"
        Public Const Err_NameBlank As String = "msvse_resedit.Err.NameBlank"
        Public Const Err_InvalidName As String = "msvse_resedit.Err.InvalidName"
        Public Const Err_DuplicateName As String = "msvse_resedit.Err.DuplicateName"
        Public Const Err_UnexpectedResourceType As String = "msvse_resedit.Err.UnexpectedResourceType"
        Public Const Err_CantCreateNewResource As String = "msvse_resedit.Err.CantCreateNewResource"
        Public Const Err_CantPlay As String = "msvse_resedit.Err.CantPlay"
        Public Const Err_CantConvertFromString As String = "msvse_resedit.Err.CantConvertFromString"
        Public Const Err_EditFormResx As String = "msvse_resedit.Err.EditFormResx"
        Public Const Err_CantAddFileToDeviceProject As String = "msvse_resedit.Err.CantAddFileToDeviceProject"
        Public Const Err_TypeIsNotSupported As String = "msvse_resedit.Err.TypeIsNotSupported"
        Public Const Err_CantSaveBadResourceItem As String = "msvse_resedit.Err.CantSaveBadResourceItem "
        Public Const Err_MaxFilesLimitation As String = "msvse_resedit.Err.MaxFilesLimitation"

        'Task list errors
        Public Const Task_BadLink As String = "msvse_resedit.tasklist.BadLink"
        Public Const Task_CantInstantiate As String = "msvse_resedit.tasklist.CantInstantiate"
        Public Const Task_NonrecommendedName As String = "msvse_resedit.tasklist.NonrecommendedName"
        Public Const Task_CantChangeCustomToolOrNamespace As String = "msvse_resedit.tasklist.CantChangeCustomToolOrNamespace"


        'Dialogs
        Public Const Dlg_OpenEmbedded As String = "msvse_resedit.dlg.OpenEmbedded"
        Public Const Dlg_QueryName As String = "msvse_resedit.dlg.QueryName"
        Public Const Dlg_OpenFileWarning As String = "msvse_resedit.dlg.OpenFileWarning"
    End Class

End Namespace



'****************************************************
'*****  Settings Designer Help IDs
'****************************************************

Namespace FSharpPropPages.Microsoft.VisualStudio.Editors.SettingsDesigner

    Friend NotInheritable Class HelpIDs

        'General errors
        Public Const Err_LoadingSettingsFile As String = "msvse_settingsdesigner.Err.LoadingSettingsFile"
        Public Const Err_LoadingAppConfigFile As String = "msvse_settingsdesigner.Err.LoadingAppConfigFile"
        Public Const Err_SavingAppConfigFile As String = "msvse_settingsdesigner.Err.SavingAppConfigFile"
        Public Const Err_NameBlank As String = "msvse_settingsdesigner.Err.NameBlank"
        Public Const Err_InvalidName As String = "msvse_settingsdesigner.Err.InvalidName"
        Public Const Err_DuplicateName As String = "msvse_settingsdesigner.Err.DuplicateName"
        Public Const Err_FormatValue As String = "msvse_settingsdesigner.Err.FormatValue"
        Public Const Err_ViewCode As String = "msvse_settingsdesigner.Err.ViewCode"

        ' Synchronize user config
        Public Const Err_SynchronizeUserConfig As String = "msvse_settingsdesigner.SynchronizeUserConfig"

        'Dialogs
        Public Const Dlg_PickType As String = "msvse_settingsdesigner.dlg.PickType"

        ' Help keyword for description link in settings designer
        Public Const SettingsDesignerDescription As String = "ApplicationSettingsOverview"


        'My.Settings help keyword (generated into the .settings.designer.vb file in VB)
        Public Const MySettingsHelpKeyword As String = "My.Settings"


        ' Can't create this guy!
        Private Sub New()
        End Sub
    End Class

End Namespace



'****************************************************
'*****  Indigo Design-Time Tools Help IDs
'****************************************************

Namespace FSharpPropPages.Microsoft.VisualStudio.Editors.WCF

    Friend NotInheritable Class HelpIDs

        'General errors
        Public Const ReferenceGroup_InvalidNamespace As String = "msvse_wcf.Err.ReferenceGroup_InvalidNamespace"
        Public Const AddSvcRefDlg_ErrorOnOK As String = "msvse_wcf.Err.ErrorOnOK"
        Public Const AddSvcRefDlg_NothingSelectedOnGo As String = "msvse_wcf.Err.AddSvcRefDlg_NothingSelectedOnGo"
        Public Const ReferenceGroup_NamespaceConflictsOther As String = "msvse_wcf.Err.ReferenceGroup_NamespaceConflictsOther"
        Public Const ReferenceGroup_NamespaceCantBeEmpty As String = "msvse_wcf.Err.ReferenceGroup_NamespaceCantBeEmpty"
        Public Const ReferenceGroup_InvalidName As String = "msvse_wcf.Err.ReferenceGroup_InvalidName"
        Public Const ReferenceGroup_NameCantBeEmpty As String = "msvse_wcf.Err.ReferenceGroup_NameCantBeEmpty"
        Public Const ReferenceGroup_NameConflictsOther As String = "msvse_wcf.Err.ReferenceGroup_NameConflictsOther"
        Public Const ReferenceGroup_NameCantStartEndWithSpace As String = "msvse_wcf.Err.ReferenceGroup_NameCantStartEndWithSpace"
        Public Const ReferenceGroup_NotSupportedInCurrentProject As String = "msvse_wcf.Err.ReferenceGroup_NotSupportedInCurrentProject"
        Public Const ReferenceGroup_FailedToGetReferencedAssembliesFromProject As String = "msvse_wcf.Err.ReferenceGroup_FailedToGetReferencedAssembliesFromProject"
        Public Const ReferenceGroup_SvcMapFileIsMissing As String = "msvse_wcf.Err.ReferenceGroup_SvcMapFileIsMissing"
        Public Const ReferenceGroup_SvcMapFileBadFormat As String = "msvse_wcf.Err.ReferenceGroup_SvcMapFileBadFormat"

        'Dialogs
        Public Const Dlg_AddServiceReference As String = "msvse_wcf.dlg.AddServiceReference"
        Public Const Dlg_ConfigureServiceReference As String = "msvse_wcf.dlg.ConfigureServiceReference"

        ' Configuration 
        Public Const Configuration_ConfigurationErrorsException As String = "msvse_wcf.cfg.ConfigurationErrorsException"

    End Class

End Namespace

'****************************************************
'*****  My Extensibility Design-Time Tools HelpIDs
'****************************************************
Namespace Microsoft.VisualStudio.Editors.MyExtensibility
    Friend NotInheritable Class HelpIDs
        Public Const Dlg_AddMyNamespaceExtensions As String = "vb.AddingMyExtensions"
        Public Const VBProjPropMyExtensions As String = "vb.ProjectPropertiesMyExtensions"

        Private Sub New()
        End Sub
    End Class
End Namespace
