' This file initializes other components.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On
Imports HDUIECMinecraftClientLauncher.Backend
Imports HDUIECMinecraftClientLauncher.Common
Imports HDUIECMinecraftClientLauncher.My.Resources

Class Application

    ' Initialize components.
    Public Sub App_Startup(Sender As Object, e As StartupEventArgs)
        CommonValues.NormalConfigurationProvider = New IniConfigurationProvider
        If CommonValues.NormalConfigurationProvider.PrepareConfigurationEnvironment() <> ReturnStatus.Success Then
            MsgBox("Configuration Provider initialization failed!", vbCritical)
            End
        End If

        ' TODO: Detect runtime OS information
        ' On OSes without registry support, RegistryConfigurationProvider is the same as NormalConfigurationProvider.
        CommonValues.RegistryConfigurationProvider = New RegistryConfigurationProvider

        CommonValues.DownloadProvider = New SynoDownloadProvider
        If CommonValues.DownloadProvider.PrepareDownloadEnvironment() <> ReturnStatus.Success Then
            'HZ said the availability of server cannot be guranteed.
            MsgBox(LocalizationString.NoInternet, vbExclamation)
        End If
        CommonValues.DecompressionProvider = New ZipDecompressionProvider
        CommonValues.LoginProvider = New YggdrasilLoginProvider
    End Sub
    ' Finalize components.
    Public Sub App_Exit(Sender As Object, e As ExitEventArgs)
        CommonValues.DownloadProvider.FinalizeDownloadEnvironment()
    End Sub
End Class
