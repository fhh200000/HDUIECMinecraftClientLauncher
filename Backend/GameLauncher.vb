' This class launches game by generating Windows batches.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On

Namespace Backend

    Public Class GameLauncher
        Public Shared Async Sub LaunchGame()
            Dim Pros As New Process()
            Dim Value As String = ""
            Pros.StartInfo.FileName = Environment.CurrentDirectory + "\\.minecraft\\commandline.bat"
            Pros.StartInfo.WorkingDirectory = Environment.CurrentDirectory
            Pros.StartInfo.EnvironmentVariables.Add("BASEDIR", Environment.CurrentDirectory)
            CommonValues.RegistryConfigurationProvider.GetConfiguration("UserInformation.UserId", Value)
            Pros.StartInfo.EnvironmentVariables.Add("USERID", Value)
            CommonValues.RegistryConfigurationProvider.GetConfiguration("UserInformation.Username", Value)
            Pros.StartInfo.EnvironmentVariables("PLAYERNAME") = Value
            Pros.StartInfo.EnvironmentVariables.Add("LAUNCHERBRAND", "HDUIECMinecraftClientLauncher")
            Pros.StartInfo.EnvironmentVariables.Add("LAUNCHERVERSION", "2023")
            CommonValues.NormalConfigurationProvider.GetConfiguration("Yggdrasil.Uri", Value)
            Pros.StartInfo.EnvironmentVariables.Add("AUTHURI", Value)
            CommonValues.RegistryConfigurationProvider.GetConfiguration("Login.ClientToken", Value)
            Pros.StartInfo.EnvironmentVariables.Add("CLIENTTOKEN", Value)
            CommonValues.RegistryConfigurationProvider.GetConfiguration("Login.AccessToken", Value)
            Pros.StartInfo.EnvironmentVariables.Add("ACCESSTOKEN", Value)
            Pros.StartInfo.UseShellExecute = False
            Await Task.FromResult(Pros.Start())
        End Sub
    End Class
End Namespace
