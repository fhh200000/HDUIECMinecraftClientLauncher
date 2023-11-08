' This class implements Yggdrasil methods to login to Minecraft online server.
' SPDX-License-Identifier: WTFPL
Public Class YggdrasilLoginProvider
    Implements ILoginProvider

    Public Function PerformLogin(Username As String, Password As String, ByRef AccessToken As String, ByRef ClientToken As String) As ReturnStatus Implements ILoginProvider.PerformLogin
        Throw New NotImplementedException()
    End Function

    Public Function PreformRefresh(ByRef AccessToken As String, ByRef ClientToken As String) As ReturnStatus Implements ILoginProvider.PreformRefresh
        Throw New NotImplementedException()
    End Function

    Public Function PreformValidate(AccessToken As String, ClientToken As String) As ReturnStatus Implements ILoginProvider.PreformValidate
        Throw New NotImplementedException()
    End Function

End Class
