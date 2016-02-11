' Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System
Imports Microsoft.Win32
Imports Marshal = System.Runtime.InteropServices.Marshal

Namespace Microsoft.VisualStudio.Editors.Interop


' Users can make their classes implement this interface to get access to all
' the constants in the Win32 API easily.
    <System.Runtime.InteropServices.ComVisible(False)> _
Friend Class win
    Friend Const CLSCTX_INPROC_SERVER As Integer = &H1
    Friend Const CLSCTX_INPROC_HANDLER As Integer = &H2
    Friend Const CLSCTX_LOCAL_SERVER As Integer = &H4
    Friend Const CLSCTX_INPROC_SERVER16 As Integer = &H8
    Friend Const CLSCTX_REMOTE_SERVER As Integer = &H10
    Friend Const CLSCTX_INPROC_HANDLER16 As Integer = &H20
    Friend Const CLSCTX_INPROC_SERVERX86 As Integer = &H40
    Friend Const CLSCTX_INPROC_HANDLERX86 As Integer = &H80
    Friend Const CLSCTX_ESERVER_HANDLER As Integer = &H100
    Friend Const CLSCTX_RESERVED As Integer = &H200
    Friend Const CLSCTX_NO_CODE_DOWNLOAD As Integer = &H400
    Friend Const DISPID_UNKNOWN As Integer = -1
    Friend Const DISP_E_MEMBERNOTFOUND As Integer = &H80020003
    Friend Const DLGC_WANTTAB As Integer = &H2
    Friend Const EM_UNDO As Integer = &HC7
    Friend Const FNERR_BUFFERTOOSMALL As Integer = &H3003
    Friend Const [FALSE] As Integer = 0
    Friend Const FACILITY_WIN32 As Integer = 7
    Friend Const GW_CHILD As UInteger = 5
    Friend Const HDI_TEXT As Integer = &H2
    Friend Const HDI_FORMAT As Integer = &H4
    Friend Const HDI_IMAGE As Integer = &H20
    Friend Const HDF_STRING As Integer = &H4000
    Friend Const HDF_BITMAP_ON_RIGHT As Integer = &H1000
    Friend Const HDF_IMAGE As Integer = &H800
    Friend Const HDM_SETITEMW As Integer = &H1200 + 12
    Friend Const HDM_SETIMAGELIST As Integer = &H1200 + 8
    Friend Const LVM_EDITLABELA As Integer = &H1000 + 23
    Friend Const LVM_EDITLABELW As Integer = &H1000 + 118
    Friend Const LVM_GETHEADER As Integer = &H1000 + 31
    Friend Const MAX_PATH As Integer = 260
    Friend Const OLE_E_PROMPTSAVECANCELLED As Integer = &H8004000C
    Friend Const QS_KEY As Integer = &H1
    Friend Const QS_MOUSEMOVE As Integer = &H2
    Friend Const QS_MOUSEBUTTON As Integer = &H4
    Friend Const QS_POSTMESSAGE As Integer = &H8
    Friend Const QS_TIMER As Integer = &H10
    Friend Const QS_PAINT As Integer = &H20
    Friend Const QS_SENDMESSAGE As Integer = &H40
    Friend Const QS_HOTKEY As Integer = &H80
    Friend Const QS_ALLPOSTMESSAGE As Integer = &H100
    Friend Const QS_MOUSE As Integer = QS_MOUSEMOVE Or QS_MOUSEBUTTON
    Friend Const QS_INPUT As Integer = QS_MOUSE Or QS_KEY
    Friend Const QS_ALLEVENTS As Integer = QS_INPUT Or QS_POSTMESSAGE Or QS_TIMER Or QS_PAINT Or QS_HOTKEY
    Friend Const QS_ALLINPUT As Integer = QS_INPUT Or QS_POSTMESSAGE Or QS_TIMER Or QS_PAINT Or QS_HOTKEY Or QS_SENDMESSAGE
    Friend Const SPI_GETSCREENREADER As Integer = 70
    Friend Const TVM_SETITEMA As Integer = &H1100 + 13
    Friend Const WAVE_FORMAT_PCM As Integer = &H1
    Friend Const WAVE_FORMAT_ADPCM As Integer = &H2
    Friend Const WAVE_FORMAT_IEEE_FLOAT As Integer = &H3
    Friend Const WM_SETFOCUS As Integer = &H7
    Friend Const WM_SYSCOLORCHANGE As Integer = &H15
    Friend Const WM_SETTINGCHANGE As Integer = &H1A
    Friend Const WM_HELP As Integer = &H53
    Friend Const WM_CONTEXTMENU As Integer = &H7B
    Friend Const WM_GETDLGCODE As Integer = &H87
    Friend Const WM_KEYDOWN As Integer = &H100
    Friend Const WM_KEYUP As Integer = &H101
    Friend Const WM_CHAR As Integer = &H102
    Friend Const WM_SYSCHAR As Integer = &H106
    Friend Const WM_RBUTTONDOWN As Integer = &H204
    Friend Const WM_RBUTTONUP As Integer = &H205
    Friend Const WM_PASTE As Integer = &H302
    Friend Const WM_PALETTECHANGED As Integer = &H311
    Friend Const WM_USER As Integer = &H400
End Class 'win

End Namespace
