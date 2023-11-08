' This interface will provide methods to login to Minecraft online server.
' SPDX-License-Identifier: WTFPL
Public Interface ILoginProvider

    ' Perform login process.
    ' Usually, this includes sending username & password to server.
    ' @param    Username                User's name or email.
    ' @param    Password                User's password.
    ' @param    ref AccessToken         User's access token, for downloading resources.
    ' @param    ref ClientToken         User's client token, for sensitive actions.
    ' @return   ReturnStatus.Success    when user is successfully logon.
    ' @return   Other                   on failure.
    Function PerformLogin(Username As String, Password As String, ByRef AccessToken As String, ByRef ClientToken As String) As ReturnStatus

    ' Perform refresh token process.
    ' @param    ref AccessToken         User's access token to refresh.
    ' @param    ref ClientToken         User's client token to refresh.
    ' @return   ReturnStatus.Success    when token is successfully refreshed.
    ' @return   Other                   on failure.
    Function PreformRefresh(ByRef AccessToken As String, ByRef ClientToken As String) As ReturnStatus

    ' Check whether Tokens are valid.
    ' @param    AccessToken             User's access token to refresh.
    ' @param    ClientToken             User's client token to refresh.
    ' @return   ReturnStatus.Success    when tokens are valid.
    ' @return   Other                   on failure.
    Function PreformValidate(AccessToken As String, ClientToken As String) As ReturnStatus

End Interface
