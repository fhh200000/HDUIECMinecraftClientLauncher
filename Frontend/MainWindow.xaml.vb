' This file defines MainWindow interaction.
' SPDX-License-Identifier: WTFPL
Namespace Frontend
    Class MainWindow
        Private Sub button_Click(sender As Object, e As RoutedEventArgs)
            Dim Version As String = ""
            For Each I As String In CommonValues.GameComponents
                CommonValues.DownloadProvider.GetLatestVersionOfComponent(I, Version)
            Next

        End Sub
    End Class
End Namespace
