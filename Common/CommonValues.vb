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
End Enum

Public Class CommonValues

    Public Shared DownloadProvider As IDownloadProvider
    Public Shared ConfigurationProvider As IConfigurationProvider
    Public Shared GameComponents() As String = {"Mod", "Base", "Config", "Launcher", "Shaderpack", "JavaRuntime", "MainComponent"}

End Class
