Option Strict On
Option Explicit On
' This interface will provide methods to access Windows registry.
' SPDX-License-Identifier: WTFPL

Imports Microsoft.Win32

Namespace Common

    Public Class RegistryConfigurationProvider
        Implements IConfigurationProvider

        Private Const RegistryPrefix As String = "HKEY_CURRENT_USER\SOFTWARE\HDUIECMinecraftClientLauncher"

        Public Function PrepareConfigurationEnvironment() As ReturnStatus Implements IConfigurationProvider.PrepareConfigurationEnvironment
            ' Registry doesn't to be prepared.
            Return ReturnStatus.Success
        End Function

        Public Function GetConfiguration(ConfigurationName As String, ByRef Value As String) As ReturnStatus Implements IConfigurationProvider.GetConfiguration
            Dim SplitNames() As String = ConfigurationName.Split("."c)
            Value = Convert.ToString(Registry.GetValue(RegistryPrefix + "\" + SplitNames(0), SplitNames(1), ""))
            Return ReturnStatus.Success
        End Function

        Public Function SetConfiguration(ConfigurationName As String, Value As String) As ReturnStatus Implements IConfigurationProvider.SetConfiguration
            Dim SplitNames() As String = ConfigurationName.Split("."c)
            Registry.SetValue(RegistryPrefix + "\" + SplitNames(0), SplitNames(1), Value, RegistryValueKind.String)
            Return ReturnStatus.Success
        End Function

    End Class

End Namespace