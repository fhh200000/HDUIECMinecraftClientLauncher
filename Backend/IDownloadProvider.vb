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
        ' This is a blocking downloading process. Inside this function, an async process is assumed.
        ' @param    ComponentName           The name of specified component.
        ' @param    ByRef DownloadPercent   The current download percent, may be updated by databinding.
        ' @param    ByRef DownloadSize      The current download size, may be updated by databinding.
        ' @param    ByRef TotalSize         The total download size, may be updated by databinding.
        ' @return   ReturnStatus.Success    when download environment is successfully finalized.
        ' @return   Other                   on failure.
        Function StartDownloadingProcess(ComponentName As String, ByRef DownloadPercent As Double,
                                         ByRef DownloadSize As Double, ByRef TotalSize As Long) As ReturnStatus

    End Interface
End Namespace
