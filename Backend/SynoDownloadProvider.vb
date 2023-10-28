' This interface will implement Synology REST API methods to download files.
' Thanks HZ for sharing his Synology!
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On
Imports System.Net.Http
Imports System.Web

Namespace Backend
    Public Class SynoDownloadProvider
        Implements IDownloadProvider

        Private Const LoginUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.API.Auth&version=6&method=login&account={1}&passwd={2}&session=FileStation&format=cookie"
        Private Const FileDownloadUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.FileStation.Download&version=2&method=download&path={1}&mode=download"
        Private Const LogoutUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.API.Auth&version=6&method=logout"

        Private BaseUri As String
        Private Username As String
        Private Password As String

        Private WebClient As HttpClient

        Public Function PrepareDownloadEnvironment() As ReturnStatus _
                                                    Implements IDownloadProvider.PrepareDownloadEnvironment
            Dim Status As ReturnStatus
            Status = CommonValues.ConfigurationProvider.GetConfiguration("Synology.BaseUri", BaseUri)
            If Status <> ReturnStatus.Success Then
                Return Status
            End If
            Status = CommonValues.ConfigurationProvider.GetConfiguration("Synology.Username", Username)
            If Status <> ReturnStatus.Success Then
                Return Status
            End If
            Status = CommonValues.ConfigurationProvider.GetConfiguration("Synology.Password", Password)
            If Status <> ReturnStatus.Success Then
                Return Status
            End If
            WebClient = New HttpClient()
            ' Do login
            Try
                Dim Result = WebClient.GetAsync(String.Format(LoginUriTemplate, BaseUri, HttpUtility.UrlEncode(Username),
                                                              HttpUtility.UrlEncode(Password)))
                Result.Wait()
                If Result.Result.StatusCode <> 200 Then
                    Status = ReturnStatus.NetworkError
                End If
            Catch
                Return ReturnStatus.NetworkError
            End Try
            Return Status
        End Function

        Public Function FinalizeDownloadEnvironment() As ReturnStatus _
                                                    Implements IDownloadProvider.FinalizeDownloadEnvironment
            Dim Status As ReturnStatus
            ' Do logout
            Try
                Dim Result = WebClient.GetAsync(String.Format(LogoutUriTemplate, BaseUri))
                Result.Wait()
                If Result.Result.StatusCode <> 200 Then
                    Status = ReturnStatus.NetworkError
                End If
            Catch
                Return ReturnStatus.NetworkError
            End Try
            WebClient.Dispose()
            Return Status
        End Function

        Public Function GetLatestVersionOfComponent(ComponentName As String, ByRef CurrentVersion As String) As ReturnStatus _
                                                    Implements IDownloadProvider.GetLatestVersionOfComponent
            Throw New NotImplementedException()
        End Function

        Public Function StartDownloadingProcess(ComponentName As String, ByRef DownloadPercent As Double,
                                                ByRef DownloadSize As Double, ByRef TotalSize As Long) As ReturnStatus _
                                                Implements IDownloadProvider.StartDownloadingProcess
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
