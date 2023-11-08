Option Explicit On
Option Strict On
' This file defines MainWindow interaction.
' SPDX-License-Identifier: WTFPL

Imports System.IO
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
                    For Each I In CType(DataContext, MainWindowViewModel).DownloadItems
                        If I.ComponentName = Component Then
                            I.VersionString = Version
                        End If
                    Next
                    File.Delete(Environment.CurrentDirectory + Path.DirectorySeparatorChar + Component)
                Else
                End If
            Else
            End If
        End Function

        Private Sub StartGame(sender As Object, e As RoutedEventArgs)

        End Sub
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            DataContext = New MainWindowViewModel()
        End Sub
        Private Async Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            Dim Value As String = ""
            CommonValues.RegistryConfigurationProvider.GetConfiguration("UserInformation.Username", Value)
            If Value.Equals("") Then
                Username.Content = LocalizationString.ClickToLogin
            Else
                Username.Content = Value
            End If
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

        Private Sub PerformLogin(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub CancelLogin(sender As Object, e As RoutedEventArgs)
            LoginPrompt.Visibility = Visibility.Hidden
        End Sub

        Private Sub ShowLoginDialog(sender As Object, e As RoutedEventArgs)
            LoginPrompt.Visibility = Visibility.Visible
        End Sub

    End Class

End Namespace
