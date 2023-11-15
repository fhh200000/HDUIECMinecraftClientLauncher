' This interface will provide methods to login to Minecraft online server.
' SPDX-License-Identifier: WTFPL

Option Strict On
Option Explicit On

Namespace Backend
    Public Interface ILoginProvider

        ' Perform login process.
        ' Usually, this includes sending username & password to server.
        ' @param    Username                User's name or email.
        ' @param    Password                User's password.
        ' @param    ref AccessToken         User's access token, for downloading resources.
        ' @param    ClientToken             User's client token, for sensitive actions.
        ' @param    ref CharacterName       User's character name.
        ' @param    ref CharacterId         User's character ID.
        ' @return   ReturnStatus.Success    when user is successfully logon.
        ' @return   Other                   on failure.
        Function PerformLogin(Username As String, Password As String, ByRef AccessToken As String,
                                ClientToken As String, ByRef PlayerName As String, ByRef PlayerId As String) As ReturnStatus

        ' Perform refresh token process.
        ' @param    ref AccessToken         User's access token to refresh.
        ' @param    ref ClientToken         User's client token to refresh.
        ' @param    UserId                  Character ID to bind.
        ' @param    UserName                Character Name to bind.
        ' @return   ReturnStatus.Success    when token is successfully refreshed.
        ' @return   Other                   on failure.
        Function PreformRefresh(ByRef AccessToken As String, ByRef ClientToken As String,
                                       Optional UserId As String = "", Optional Username As String = "") As ReturnStatus

        ' Check whether Tokens are valid.
        ' @param    AccessToken             User's access token to refresh.
        ' @param    ClientToken             User's client token to refresh.
        ' @return   ReturnStatus.Success    when tokens are valid.
        ' @return   Other                   on failure.
        Function PreformValidate(AccessToken As String, ClientToken As String) As ReturnStatus

        ' Get player's Skin Uri, as well as skin type.
        ' @param    CharacterId             User's character ID.
        ' @param    ref CharacterSkinUri    URI of user's skin.
        ' @param    ref CharacterSkinType   Type of user's skin (i.e., Steve or Alex)
        ' @return   ReturnStatus.Success    when skin information is successfully retrieved.
        ' @return   Other                   on failure.
        Function GetPlayerSkinInformation(CharacterId As String, ByRef CharacterSkinUri As String,
                                          ByRef CharacterSkinType As Common.SkinType) As ReturnStatus

    End Interface
End Namespace