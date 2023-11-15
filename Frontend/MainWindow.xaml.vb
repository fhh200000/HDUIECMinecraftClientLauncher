Option Explicit On
Option Strict On
' This file defines MainWindow interaction.
' SPDX-License-Identifier: WTFPL

Imports System.IO
Imports HDUIECMinecraftClientLauncher.Backend
Imports HDUIECMinecraftClientLauncher.Common
Imports HDUIECMinecraftClientLauncher.My.Resources

Namespace Frontend

    Class MainWindow

        Dim LoggedIn As Boolean
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
            If LoggedIn Then
                GameLauncher.LaunchGame()
            End If
        End Sub
        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            DataContext = New MainWindowViewModel()
        End Sub
        Private Async Function UpdateAvatar() As Task
            Dim SkinUri As String = ""
            Dim SkinType As SkinType = SkinType.Steve
            Dim PlayerId As String = ""
            CommonValues.RegistryConfigurationProvider.GetConfiguration("UserInformation.UserId", PlayerId)
            Dim Status = CommonValues.LoginProvider.GetPlayerSkinInformation(PlayerId, SkinUri, SkinType)
            If Status = ReturnStatus.Success Then
                Status = Await AvatarGenerator.DownloadAndGenerateAvatar(SkinUri)
                If Status = ReturnStatus.Success Then
                    Dim AvatarStream As New MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + Path.DirectorySeparatorChar + AvatarGenerator.AvatarFileName))
                    Dim AvatarImage As New BitmapImage
                    AvatarImage.BeginInit()
                    AvatarImage.CacheOption = BitmapCacheOption.OnLoad
                    AvatarImage.StreamSource = AvatarStream
                    AvatarImage.EndInit()
                    AvatarImage.Freeze()
                    Avatar.Source = AvatarImage
                End If
            End If
        End Function
        Private Async Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
            Dim ClientToken As String = ""
            Dim AccessToken As String = ""
            CommonValues.RegistryConfigurationProvider.GetConfiguration("Login.ClientToken", ClientToken)
            CommonValues.RegistryConfigurationProvider.GetConfiguration("Login.AccessToken", AccessToken)
            If (Not ClientToken.Equals("")) And (Not AccessToken.Equals("")) Then
                Dim Result = CommonValues.LoginProvider.PreformRefresh(AccessToken, ClientToken)
                If Result = ReturnStatus.Success Then
                    CommonValues.RegistryConfigurationProvider.SetConfiguration("Login.AccessToken", AccessToken)
                    If File.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + AvatarGenerator.AvatarFileName) Then
                        Dim AvatarStream As New MemoryStream(File.ReadAllBytes(Environment.CurrentDirectory + Path.DirectorySeparatorChar + AvatarGenerator.AvatarFileName))
                        Dim AvatarImage As New BitmapImage
                        AvatarImage.BeginInit()
                        AvatarImage.CacheOption = BitmapCacheOption.OnLoad
                        AvatarImage.StreamSource = AvatarStream
                        AvatarImage.EndInit()
                        AvatarImage.Freeze()
                        Avatar.Source = AvatarImage
                    Else
                        Await UpdateAvatar()
                    End If
                    LoggedIn = True
                Else
                    CommonValues.RegistryConfigurationProvider.SetConfiguration("UserInformation.Username", "")
                End If
            End If
            Dim Value As String = ""
            CommonValues.RegistryConfigurationProvider.GetConfiguration("UserInformation.Username", Value)
            If Value = Nothing Or "".Equals(Value) Then
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
            StartButton.IsEnabled = True
        End Sub

        Private Async Sub PerformLogin(sender As Object, e As RoutedEventArgs)
            Dim ClientToken As String = ""
            Dim AccessToken As String = ""
            Dim PlayerId As String = ""
            Dim PlayerName As String = ""
            CommonValues.RegistryConfigurationProvider.GetConfiguration("Login.ClientToken", ClientToken)
            If ClientToken = "" Then
                ClientToken = Guid.NewGuid().ToString
            End If
            CommonValues.RegistryConfigurationProvider.SetConfiguration("Login.ClientToken", ClientToken)
            Dim Status = CommonValues.LoginProvider.PerformLogin(UserNameInput.Text, PasswordInput.Password, AccessToken,
                                                                    ClientToken, PlayerName, PlayerId)
            If Status = ReturnStatus.Success Then
                CommonValues.RegistryConfigurationProvider.SetConfiguration("UserInformation.Username", PlayerName)
                CommonValues.RegistryConfigurationProvider.SetConfiguration("UserInformation.UserId", PlayerId)
                CommonValues.RegistryConfigurationProvider.SetConfiguration("Login.AccessToken", AccessToken)
                Username.Content = PlayerName
                LoginPrompt.Visibility = Visibility.Hidden
            ElseIf Status = ReturnStatus.InvaildPassword Then
                MsgBox(LocalizationString.InvalidPassword, vbExclamation)
            ElseIf Status = ReturnStatus.NoCharacter Then
                MsgBox(LocalizationString.NoCharacter, vbExclamation)
            End If
            LoggedIn = True
            Await UpdateAvatar()
        End Sub

        Private Sub CancelLogin(sender As Object, e As RoutedEventArgs)
            LoginPrompt.Visibility = Visibility.Hidden
        End Sub

        Private Sub ShowLoginDialog(sender As Object, e As RoutedEventArgs)
            LoginPrompt.Visibility = Visibility.Visible
        End Sub

    End Class

End Namespace
