' This file defines avatar generation function.
' SPDX-License-Identifier: WTFPL

Option Explicit On
Option Strict On
Imports System.Drawing
Imports System.Net.Http

Namespace Common

    Public Enum SkinType
        Steve
        Alex
    End Enum

    Public Class AvatarGenerator

        Private Shared ReadOnly WebClient As New HttpClient

        Const HeadOffsetX As Integer = 8
        Const HeadOffsetY As Integer = 8
        Const HeadSizeX As Integer = 8
        Const HeadSizeY As Integer = 8
        Public Const AvatarFileName As String = "Avatar.png"

        ' Downloads and generates avatar file.
        ' @param    Uri                     The URI of player skin.
        ' @param    Type                    The skin type.
        ' @return   ReturnStatus.Success    when download environment is successfully initialized.
        ' @return   Other                   on failure.

        Public Shared Async Function DownloadAndGenerateAvatar(Uri As String) As Task(Of ReturnStatus)
            ' Step 1: download.
            Dim AvatarStream = Await WebClient.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead).Result.Content.ReadAsStreamAsync()
            ' Step 2: 16x upscaling 8px -> 128px, integer scaling.
            Dim Img = New Bitmap(AvatarStream)
            Dim Img128x = New Bitmap(128, 128)
            Dim PixelColor As Color
            For I = 0 To HeadSizeY - 1
                For J = 0 To HeadSizeX - 1
                    PixelColor = Img.GetPixel(I + HeadOffsetX, J + HeadOffsetY)
                    For K = 0 To (128 \ HeadSizeY) - 1
                        For L = 0 To (128 \ HeadSizeX) - 1
                            Img128x.SetPixel(I * (128 \ HeadSizeX) + L, J * (128 \ HeadSizeY) + K, PixelColor)
                        Next L
                    Next K
                Next J
            Next I
            ' Step 3: Fractional downscaling 128px -> 96px.
            Dim Img96x = New Bitmap(Img128x, New Size(96, 96))
            ' Step 4: Fill back to original 128x region to create padding.
            For I = 0 To 95
                For J = 0 To 95
                    Img128x.SetPixel(16 + J, 16 + I, Img96x.GetPixel(J, I))
                Next J
            Next I
            For I = 0 To 15
                For J = 0 To 127
                    Img128x.SetPixel(J, I, Color.Transparent)
                Next J
            Next I
            For I = 112 To 127
                For J = 0 To 127
                    Img128x.SetPixel(J, I, Color.Transparent)
                Next J
            Next I
            For I = 0 To 127
                For J = 0 To 15
                    Img128x.SetPixel(J, I, Color.Transparent)
                Next J
            Next I
            For I = 0 To 127
                For J = 112 To 127
                    Img128x.SetPixel(J, I, Color.Transparent)
                Next J
            Next I
            ' Step 5: Save.
            Img128x.Save(AvatarFileName)
            Img96x.Dispose()
            Img128x.Dispose()
            Img.Dispose()
            Return ReturnStatus.Success
        End Function
    End Class

End Namespace

