' This interface will implement methods to extract/decompress ZIP files.
' SPDX-License-Identifier: WTFPL

Option Explicit On
Option Strict On
Imports System.IO
Imports System.IO.Compression

Namespace Backend

    Public Class ZipDecompressionProvider
        Implements IDecompressionProvider

        Public Function DecompressFile(FilePath As String, DestinationDir As String,
                                       Method As IDecompressionProvider.DecompressionMethod) As ReturnStatus _
                                       Implements IDecompressionProvider.DecompressFile
            If Method = IDecompressionProvider.DecompressionMethod.RemoveFilesBeforeDecompression Then
                Try
                    Dim FileListFile = File.OpenRead(DestinationDir + Path.DirectorySeparatorChar _
                                                      + ".minecraft" + Path.DirectorySeparatorChar + Path.GetFileName(FilePath) + ".FileList")
                    Dim FileListStreamReader As New StreamReader(FileListFile)
                    Dim FileListLine As String = FileListStreamReader.ReadLine()
                    Do While FileListLine <> Nothing
                        Try
                            File.Delete(FileListLine)
                        Catch
                        End Try
                        FileListLine = FileListStreamReader.ReadLine()
                    Loop
                    FileListStreamReader.Close()
                    FileListFile.Close()
                Catch
                    ' It is possible that the older version DOES NOT contains a file list. Proceed anyway.
                End Try
            End If
            Try
                Dim Archive = ZipFile.OpenRead(FilePath)
                For Each Entry As ZipArchiveEntry In Archive.Entries
                    If Entry.FullName.EndsWith("/") Or Entry.FullName.EndsWith("\\") Then
                        Dim EntryFullName As String = Entry.FullName.Replace("/"c, "\"c)
                        Dim EntryFullpath As String = Path.Combine(DestinationDir, EntryFullName)
                        If (Not Directory.Exists(EntryFullpath)) Then
                            Directory.CreateDirectory(EntryFullpath)
                        End If
                    Else
                        Dim EntryFullName As String = Entry.FullName.Replace("/"c, "\"c)
                        Entry.ExtractToFile(Path.Combine(DestinationDir, EntryFullName), True)
                    End If
                Next
                Archive.Dispose()
            Catch
                Return ReturnStatus.GenericFailure
            End Try
            Return ReturnStatus.Success
        End Function

    End Class
End Namespace