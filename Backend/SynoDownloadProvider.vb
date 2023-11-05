' This interface will implement Synology REST API methods to download files.
' Thanks HZ for sharing his Synology!
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On
Imports System.Net.Http
Imports System.Web
Imports System.Text.Json
Imports System.Text.RegularExpressions
Imports System.IO

Namespace Backend
    Public Class SynoDownloadProvider
        Implements IDownloadProvider

        Private Const LoginUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.API.Auth&version=6&method=login&account={1}&passwd={2}&session=FileStation&format=cookie"
        Private Const FileDownloadUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.FileStation.Download&version=2&method=download&path={1}&mode=download"
        Private Const FileListUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list&folder_path=%22{1}%22&additional=%5B%22size%22%5D"
        Private Const LogoutUriTemplate As String =
            "{0}/webapi/entry.cgi?api=SYNO.API.Auth&version=6&method=logout"

        Const BufferCache As Integer = 16384 '16K buffer size

        Private BaseUri As String
        Private Username As String
        Private Password As String
        Private RemoteDir As String
        Private ReadOnly Versions As New Dictionary(Of String, String)
        Private ReadOnly LiteralPaths As New Dictionary(Of String, String)
        Private ReadOnly Sizes As New Dictionary(Of String, Long)
        Private ReadOnly DownloadedSizes As New Dictionary(Of String, Long)

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
            If Not CommonValues.GameComponents.Contains(ComponentName) Then
                Return ReturnStatus.InvalidConfigurationItem
            End If
            If Versions.Count = 0 Then
                ' Refresh.
                CommonValues.ConfigurationProvider.GetConfiguration("Synology.RemoteDir", RemoteDir)
                Dim Result = WebClient.GetAsync(String.Format(FileListUriTemplate, BaseUri, HttpUtility.UrlEncode(RemoteDir)))
                Result.Wait()
                If Result.Result.StatusCode <> 200 Then
                    Return ReturnStatus.NetworkError
                End If

                ' Workaround for Synology DSM bug: quoted content type.
                Result.Result.Content.Headers.ContentType.CharSet = Result.Result.Content.Headers.ContentType.CharSet.Replace("""", "")
                Dim ResultMessage = Result.Result.Content.ReadAsStringAsync()
                ResultMessage.Wait()

                Dim DataArray As JsonElement = JsonDocument.Parse(ResultMessage.Result).RootElement.GetProperty("data").GetProperty("files")
                Dim Template As New Regex("\b(?<Name>[\w\d]+)-(?<Version>[\w\d]+)\.")
                For Each Data As JsonElement In DataArray.EnumerateArray
                    Dim Mth As Match = Template.Match(Data.GetProperty("name").GetString())
                    If Mth.Groups("Name").Value.Length <> 0 Then
                        Versions.Add(Mth.Groups("Name").Value, Mth.Groups("Version").Value)
                        LiteralPaths.Add(Mth.Groups("Name").Value, Data.GetProperty("path").GetString())
                        Sizes.Add(Mth.Groups("Name").Value, Data.GetProperty("additional").GetProperty("size").GetInt64)
                        DownloadedSizes.Add(Mth.Groups("Name").Value, 0)
                    End If
                Next
            End If

            If Not Versions.ContainsKey(ComponentName) Then
                Return ReturnStatus.FileNotFound
            End If
            CurrentVersion = Versions(ComponentName)
            Return ReturnStatus.Success
        End Function

        Public Async Function StartDownloadingProcessAsync(ComponentName As String) As Task(Of ReturnStatus) _
                                                            Implements IDownloadProvider.StartDownloadingProcessAsync
            Dim Fs As FileStream
            If Not Sizes.ContainsKey(ComponentName) Then
                Return ReturnStatus.InvalidConfigurationItem
            End If
            Try
                Fs = New FileStream(ComponentName, FileMode.Create, FileAccess.ReadWrite)
            Catch
                Return ReturnStatus.ReadonlyFileSystem
            End Try
            Dim ReadBuffer(BufferCache) As Byte
            Dim Result = Await WebClient.GetAsync(String.Format(FileDownloadUriTemplate, BaseUri, HttpUtility.UrlEncode(LiteralPaths(ComponentName))), HttpCompletionOption.ResponseHeadersRead)
            If Result.StatusCode <> 200 Then
                Return ReturnStatus.NetworkError
            End If
            Dim ResultFileStream = Await Result.Content.ReadAsStreamAsync()
FillBuffer:
            Dim Size As Integer = Await ResultFileStream.ReadAsync(ReadBuffer, 0, BufferCache)
            Fs.Write(ReadBuffer, 0, Size)
            DownloadedSizes(ComponentName) += Size
            If Size <> 0 Then
                GoTo FillBuffer
            End If
            Fs.Flush()
            Fs.Close()
            Return ReturnStatus.Success

        End Function

        Public Function GetDownloadProgress(ComponentName As String, ByRef DownloadedSize As Long, ByRef TotalSize As Long) As ReturnStatus _
                                            Implements IDownloadProvider.GetDownloadProgress
            If Not Sizes.ContainsKey(ComponentName) Then
                Return ReturnStatus.InvalidConfigurationItem
            End If
            DownloadedSize = DownloadedSizes(ComponentName)
            TotalSize = Sizes(ComponentName)
            Return ReturnStatus.Success
        End Function
    End Class
End Namespace
