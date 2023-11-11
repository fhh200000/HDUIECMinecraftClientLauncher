' This file defines common values.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On

Imports HDUIECMinecraftClientLauncher.Backend
Imports HDUIECMinecraftClientLauncher.Common

Public Enum ReturnStatus
    Success
    GenericFailure
    InsufficientMemory
    FileNotFound
    NetworkError
    ReadonlyFileSystem
    InvalidConfigurationItem
    InvaildPassword
End Enum

Public Class CommonValues

    Public Shared DownloadProvider As IDownloadProvider
    Public Shared NormalConfigurationProvider As IConfigurationProvider
    Public Shared RegistryConfigurationProvider As IConfigurationProvider
    Public Shared DecompressionProvider As IDecompressionProvider
    Public Shared LoginProvider As ILoginProvider
    Public Shared GameComponents() As String = {"Mod", "Base", "Config", "Launcher", "Shaderpack", "JavaRuntime", "MainComponent"}

End Class
