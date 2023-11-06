Option Explicit On
Option Strict On
' This file defines MainWindow interaction.
' SPDX-License-Identifier: WTFPL

Imports HDUIECMinecraftClientLauncher.My.Resources

Namespace Frontend

    Class MainWindow

        Private Async Function StartDownloadProcess(Component As String, Version As String) As Task
            If Await CommonValues.DownloadProvider.StartDownloadingProcessAsync(Component) = ReturnStatus.Success Then
                If CommonValues.DecompressionProvider.DecompressFile(Environment.CurrentDirectory + IO.Path.DirectorySeparatorChar + Component,
                                                                    Environment.CurrentDirectory + IO.Path.DirectorySeparatorChar,
                                                                    Backend.IDecompressionProvider.DecompressionMethod.OverwriteCurrentFiles) _
                                                                    = ReturnStatus.Success Then
                    CommonValues.NormalConfigurationProvider.SetConfiguration("ComponentVersion." + Component, " " + Version)
                Else
                End If
            Else
            End If
        End Function

        Private Async Sub Button_Click(sender As Object, e As RoutedEventArgs)
            Dim Version As String = ""
            Dim GotVersion As String = ""
            For Each I As String In CommonValues.GameComponents
                If CommonValues.DownloadProvider.GetLatestVersionOfComponent(I, Version) = ReturnStatus.Success Then
                    If CommonValues.NormalConfigurationProvider.GetConfiguration("ComponentVersion." + I, GotVersion) = ReturnStatus.Success Then
                        If GotVersion <> Version Then
                            Await StartDownloadProcess(I, Version)
                        End If
                    End If
                End If
            Next
        End Sub

        Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            Dim Value As String = ""
            CommonValues.RegistryConfigurationProvider.GetConfiguration("UserInformation.Username", Value)
            If Value.Equals("") Then
                Username.Content = LocalizationString.ClickToLogin
            Else
                Username.Content = Value
            End If
        End Sub
    End Class
End Namespace
