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
                    Directory.Delete(DestinationDir, True)
                    Directory.CreateDirectory(DestinationDir)
                Catch
                    Return ReturnStatus.ReadonlyFileSystem
                End Try
            End If
            Try
                ZipFile.ExtractToDirectory(FilePath, DestinationDir)
            Catch
                Return ReturnStatus.GenericFailure
            End Try
            Return ReturnStatus.Success
        End Function

    End Class
End Namespace