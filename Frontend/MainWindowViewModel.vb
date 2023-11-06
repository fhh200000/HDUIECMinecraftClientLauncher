' This file defines databinding view model for Main Window.
' SPDX-License-Identifier: WTFPL

Imports System.ComponentModel
Imports System.Timers

Namespace Frontend

    Public Enum DownloadStatus
        NotStarted
        Downloading
        Extracting
        Finished
        Failed
    End Enum

    ' For Data binding
    Public Class DownloadItem
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(name As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
        End Sub

        Public Property ComponentName As String

        Private _DownloadedSize As Long
        Public Property DownloadedSize As Long
            Set(value As Long)
                _DownloadedSize = value
                OnPropertyChanged("DownloadedSize")
            End Set
            Get
                Return _DownloadedSize
            End Get
        End Property

        Public Property VersionString As String

        Public Property TotalSize As Long

        Private _Status As DownloadStatus
        Public Property Status As DownloadStatus
            Get
                Return _Status
            End Get
            Set(value As DownloadStatus)
                _Status = value
                OnPropertyChanged("Status")
            End Set
        End Property

        Private _DownloadedRatio As Double
        Public Property DownloadedRatio As Double
            Set(value As Double)
                _DownloadedRatio = value
                OnPropertyChanged("DownloadedRatio")
            End Set
            Get
                Return _DownloadedRatio
            End Get
        End Property

    End Class

    Public Class MainWindowViewModel
        Implements INotifyPropertyChanged
        ' DownloadItem container, for databinding.
        Public Property DownloadItems As New List(Of DownloadItem)
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(name As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
        End Sub

        Public Sub TimerRefreshDownloadingProgress(Source As Object, e As Timers.ElapsedEventArgs)
            Dim DownloadedSize As Long, TotalSize As Long
            For Each I In DownloadItems
                If I.Status <> DownloadStatus.Finished Then
                    CommonValues.DownloadProvider.GetDownloadProgress(I.ComponentName, DownloadedSize, TotalSize)
                    I.DownloadedSize = DownloadedSize
                    I.DownloadedRatio = I.DownloadedSize / I.TotalSize * 100
                End If
            Next
        End Sub

        Public Sub New()
            Dim Version As String = ""
            Dim GotVersion As String = ""
            Dim DownloadedSize As Long, TotalSize As Long
            For Each I As String In CommonValues.GameComponents
                If CommonValues.DownloadProvider.GetLatestVersionOfComponent(I, Version) = ReturnStatus.Success Then
                    If CommonValues.NormalConfigurationProvider.GetConfiguration("ComponentVersion." + I, GotVersion) = ReturnStatus.Success Then
                        CommonValues.DownloadProvider.GetDownloadProgress(I, DownloadedSize, TotalSize)
                        If GotVersion.Equals("") Then
                            GotVersion = "(N/A)"
                        End If
                        If Not GotVersion.Equals(Version) Then
                            Version = String.Format("{0} --> {1}", GotVersion, Version)
                        Else
                            DownloadedSize = TotalSize
                        End If
                        DownloadItems.Add(New DownloadItem With {
                            .DownloadedSize = DownloadedSize,
                            .ComponentName = I,
                            .TotalSize = TotalSize,
                            .VersionString = Version,
                            .Status = IIf(DownloadedSize = TotalSize, DownloadStatus.Finished, DownloadStatus.NotStarted),
                            .DownloadedRatio = IIf(DownloadedSize = TotalSize, 100, 0)
                        })
                    End If
                End If
            Next

            'Start Download process timer.
            Dim T As New Timer
            AddHandler T.Elapsed, New ElapsedEventHandler(AddressOf TimerRefreshDownloadingProgress)
            T.Interval = 2000
            T.AutoReset = True
            T.Enabled = True
        End Sub

    End Class

End Namespace