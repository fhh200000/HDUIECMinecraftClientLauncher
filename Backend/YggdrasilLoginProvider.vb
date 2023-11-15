' This class implements Yggdrasil methods to login to Minecraft online server.
' SPDX-License-Identifier: WTFPL

Option Explicit On
Option Strict On
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports HDUIECMinecraftClientLauncher.Common

Namespace Backend
    Public Class YggdrasilLoginProvider
        Implements ILoginProvider

        Private Const LoginUri As String = "{0}/authserver/authenticate"
        ' lang=json,strict
        Private Const LoginTemplate As String =
    "{{
        ""username"":""{0}"",
        ""password"":""{1}"",
        ""clientToken"":""{2}"",
        ""requestUser"":true,
        ""agent"":{{
            ""name"":""Minecraft"",
            ""version"":1
        }}
    }}"
        Private Const RefreshUri As String = "{0}/authserver/refresh"
        ' lang=json,strict
        Private Const RefreshTemplate As String =
    "{{
        ""accessToken"":""{0}"",
        ""clientToken"":""{1}"",
        ""requestUser"":true
    }}"
        Private Const SkinUri As String = "{0}/sessionserver/session/minecraft/profile/{1}?unsigned=true"

        Private ReadOnly WebClient As HttpClient
        Private ReadOnly BaseUri As String

        Public Function PerformLogin(Username As String, Password As String, ByRef AccessToken As String,
                                ClientToken As String, ByRef PlayerName As String, ByRef PlayerId As String) As ReturnStatus _
                                    Implements ILoginProvider.PerformLogin
            Dim Login = WebClient.PostAsync(String.Format(LoginUri, BaseUri), New StringContent(
                                            String.Format(LoginTemplate, Username, Password, ClientToken), Encoding.UTF8, "application/json"))
            Login.Wait()
            If Login.Result.StatusCode = Net.HttpStatusCode.OK Then
                ' Success
                Dim ResultTask = Login.Result.Content.ReadAsStringAsync()
                ResultTask.Wait()
                Dim Result = JsonDocument.Parse(ResultTask.Result)
                AccessToken = Result.RootElement.GetProperty("accessToken").ToString()
                Dim PlayerInfo = Result.RootElement.GetProperty("selectedProfile")
                PlayerId = PlayerInfo.GetProperty("id").ToString()
                PlayerName = PlayerInfo.GetProperty("name").ToString()
                Return ReturnStatus.Success
            ElseIf Login.Result.StatusCode = Net.HttpStatusCode.Forbidden Then
                Return ReturnStatus.InvaildPassword
            End If
            Return ReturnStatus.NetworkError
        End Function

        Public Function PreformRefresh(ByRef AccessToken As String, ByRef ClientToken As String) As ReturnStatus _
                                    Implements ILoginProvider.PreformRefresh
            Dim Refresh = WebClient.PostAsync(String.Format(RefreshUri, BaseUri), New StringContent(
                                            String.Format(RefreshTemplate, AccessToken, ClientToken), Encoding.UTF8, "application/json"))
            Refresh.Wait()
            If Refresh.Result.StatusCode = Net.HttpStatusCode.OK Then
                ' Success
                Dim ResultTask = Refresh.Result.Content.ReadAsStringAsync()
                ResultTask.Wait()
                Dim Result = JsonDocument.Parse(ResultTask.Result)
                AccessToken = Result.RootElement.GetProperty("accessToken").ToString()
                Return ReturnStatus.Success
            End If
            Return ReturnStatus.NetworkError
        End Function

        Public Function PreformValidate(AccessToken As String, ClientToken As String) As ReturnStatus _
                                    Implements ILoginProvider.PreformValidate
            ' No need to validate at this time.
            Throw New NotImplementedException()
        End Function

        Public Sub New()
            WebClient = New HttpClient
            Dim Value As String = ""
            If CommonValues.NormalConfigurationProvider.GetConfiguration("Yggdrasil.Uri", Value) = ReturnStatus.Success Then
                BaseUri = Value
            End If
        End Sub

        Public Function GetPlayerSkinInformation(CharacterId As String, ByRef CharacterSkinUri As String,
                                                 ByRef CharacterSkinType As SkinType) As ReturnStatus _
                                                 Implements ILoginProvider.GetPlayerSkinInformation
            Dim Skin = WebClient.GetAsync(String.Format(SkinUri, BaseUri, CharacterId))
            Skin.Wait()
            If Skin.Result.StatusCode = Net.HttpStatusCode.OK Then
                Dim ResultTask = Skin.Result.Content.ReadAsStringAsync()
                ResultTask.Wait()
                Dim Result = JsonDocument.Parse(ResultTask.Result)
                Dim PropertyArray = Result.RootElement.GetProperty("properties")
                For Each I As JsonElement In PropertyArray.EnumerateArray
                    If I.GetProperty("name").ToString().Equals("textures") Then
                        Dim DecodedInfo = JsonDocument.Parse(Encoding.Default.GetString(Convert.FromBase64String(I.GetProperty("value").ToString())))
                        Dim SkinNode As JsonElement = DecodedInfo.RootElement.GetProperty("textures").GetProperty("SKIN")
                        CharacterSkinUri = SkinNode.GetProperty("url").ToString()
                        Dim JsonNodeTry As JsonElement
                        If SkinNode.TryGetProperty("metadata", JsonNodeTry) Then
                            If SkinNode.TryGetProperty("model", JsonNodeTry) Then
                                If "slim".Equals(JsonNodeTry.ToString()) Then
                                    CharacterSkinType = SkinType.Alex
                                Else
                                    CharacterSkinType = SkinType.Steve
                                End If
                            End If
                        End If
                        GoTo Processed
                    End If
                Next
Processed:
                Return ReturnStatus.Success
            End If
            Return ReturnStatus.NetworkError
        End Function
    End Class
End Namespace
