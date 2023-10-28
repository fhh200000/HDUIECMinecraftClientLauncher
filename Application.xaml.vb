' This file initializes other components.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On
Imports System.IO
Imports HDUIECMinecraftClientLauncher.Backend
Imports HDUIECMinecraftClientLauncher.Common

Class Application

    ' Initialize components.
    Public Sub App_Startup(Sender As Object, e As StartupEventArgs)
        CommonValues.ConfigurationProvider = New IniConfigurationProvider
        If CommonValues.ConfigurationProvider.PrepareConfigurationEnvironment() <> ReturnStatus.Success Then
            Throw New IOException("Configuration Provider initialization failed!")
        End If
        CommonValues.DownloadProvider = New SynoDownloadProvider
        If CommonValues.DownloadProvider.PrepareDownloadEnvironment() <> ReturnStatus.Success Then
            Throw New IOException("Download Provider initialization failed!")
        End If
    End Sub
    ' Finalize components.
    Public Sub App_Exit(Sender As Object, e As ExitEventArgs)
        CommonValues.DownloadProvider.FinalizeDownloadEnvironment()
    End Sub
End Class
