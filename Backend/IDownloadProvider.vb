' This interface will provide methods to download files.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On

Namespace Backend
    Public Interface IDownloadProvider

        ' Prepare download environment.
        ' Usually, this includes locating server & perform login process.
        ' @param    none
        ' @return   ReturnStatus.Success    when download environment is successfully initialized.
        ' @return   Other                   on failure.
        Function PrepareDownloadEnvironment() As ReturnStatus

        ' Finalize download environment.
        ' Usually, this includes logging out & closing connection.
        ' @param    none
        ' @return   ReturnStatus.Success    when download environment is successfully finalized.
        ' @return   Other                   on failure.
        Function FinalizeDownloadEnvironment() As ReturnStatus

        ' Return the latest version of specified component.
        ' On success, set the status value, and passes version via ByRef ComponentName.
        ' On failure, set the status value.
        ' @param    ComponentName           The name of specified component.
        ' @param    ByRef CurrentVersion    The output version.
        ' @return   ReturnStatus.Success    when download environment is successfully finalized.
        ' @return   Other                   on failure.
        Function GetLatestVersionOfComponent(ComponentName As String, ByRef CurrentVersion As String) As ReturnStatus

        ' Start downloading process the specified component.
        ' This is an async downloading process.
        ' @param    ComponentName           The name of specified component.
        ' @return   ReturnStatus.Success    when download environment is successfully finalized.
        ' @return   Other                   on failure.
        Function StartDownloadingProcessAsync(ComponentName As String) As Task(Of ReturnStatus)

        ' Get the current download progress, and total size of it.
        ' @param    ComponentName           The name of specified component.
        ' @param    ByRef DownloadedSize    The downloaded size so far.
        ' @param    ByRef TotalSize         The total size of current file.
        ' @return   ReturnStatus.Success    when download environment is successfully finalized.
        ' @return   Other                   on failure.
        Function GetDownloadProgress(ComponentName As String, ByRef DownloadedSize As Long, ByRef TotalSize As Long) As ReturnStatus

    End Interface
End Namespace
