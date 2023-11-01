' This interface will implement INI File structure methods to get/set configurations.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Common
    Public Class IniConfigurationProvider
        Implements IConfigurationProvider

        Private ConfigurationFilePath As String = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Config.ini"

        ' Thank you Win32 API!

        <DllImport("kernel32", CharSet:=CharSet.Unicode)>
        Private Shared Function WritePrivateProfileString(Section As String, Key As String, Value As String, FilePath As String) As Long
        End Function

        <DllImport("kernel32", CharSet:=CharSet.Unicode)>
        Private Shared Function GetPrivateProfileString(Section As String, Key As String, DefaultValue As String,
                                                        RetVal As StringBuilder, Size As Integer, FilePath As String) As Integer
        End Function

        Public Function PrepareConfigurationEnvironment() As ReturnStatus _
                                                        Implements IConfigurationProvider.PrepareConfigurationEnvironment
            If Not File.Exists(ConfigurationFilePath) Then
                Try
                    File.WriteAllText(ConfigurationFilePath, My.Resources.ApplicationResources.DefaultIniConfiguration)
                Catch
                    Return ReturnStatus.ReadonlyFileSystem
                End Try
            End If
            Return ReturnStatus.Success
        End Function

        Public Function GetConfiguration(ConfigurationName As String, ByRef Value As String) As ReturnStatus _
                                            Implements IConfigurationProvider.GetConfiguration
            Dim SplitNames() As String = ConfigurationName.Split("."c)
            Dim Temp As New StringBuilder(255)
            GetPrivateProfileString(SplitNames(0), SplitNames(1), "", Temp, 255, ConfigurationFilePath)
            Value = Temp.ToString
            Return ReturnStatus.Success
        End Function

        Public Function SetConfiguration(ConfigurationName As String, Value As String) As ReturnStatus _
                                            Implements IConfigurationProvider.SetConfiguration
            Dim SplitNames() As String = ConfigurationName.Split("."c)
            If WritePrivateProfileString(SplitNames(0), SplitNames(1), Value, ConfigurationFilePath) = 0 Then
                Return ReturnStatus.InvalidConfigurationItem
            End If
            Return ReturnStatus.Success
        End Function
    End Class
End Namespace
