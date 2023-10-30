' This interface will provide methods to extract/decompress files.
' SPDX-License-Identifier: WTFPL

Option Explicit On
Option Strict On

Namespace Backend
    Public Interface IDecompressionProvider

        Enum DecompressionMethod
            OverwriteCurrentFiles
            RemoveFilesBeforeDecompression
        End Enum

        ' Decompress files.
        ' Different implementations shall have their own algorithms to fulfill.
        ' @param    FilePath                Full path of the input file.
        ' @param    DestinationDir          Full path of the destination directory.
        '                                   This directory is guaranteed to exist.
        '                                   Implementations are assumed to put files DIRECTLY INSIDE this directory.
        ' @param    Method                  The decompression method.
        ' @return   ReturnStatus.Success    when the compressed file is successfully decompressed.
        ' @return   Other                   on failure.
        Function DecompressFile(FilePath As String, DestinationDir As String, Method As DecompressionMethod) As ReturnStatus

    End Interface
End Namespace
