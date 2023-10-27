' This interface will provide methods to access configuration files.
' SPDX-License-Identifier: WTFPL

Namespace Common
    Public Interface IConfigurationProvider

        ' Prepare configuration values.
        ' Usually, this includes loading & parsing configuration files.
        ' @param    none
        ' @return   ReturnStatus.Success    when configuration files are loaded successfully.
        ' @return   Other                   on failure.
        Function PrepareConfigurationEnvironment() As ReturnStatus

        ' Get configuration value.
        ' @param    ConfigurationName       name of the configuration.
        ' @param    ByRef Value             value of the configuration.
        ' @return   ReturnStatus.Success    when configuration is read successfully.
        ' @return   Other                   on failure.
        Function GetConfiguration(ConfigurationName As String, ByRef Value As String) As ReturnStatus

        ' Set configuration value.
        ' The saved data is considered to be flushed instantly into storage.
        ' @param    ConfigurationName       name of the configuration.
        ' @param    Value                   value of the configuration.
        ' @return   ReturnStatus.Success    when configuration is read successfully.
        ' @return   Other                   on failure.
        Function SetConfiguration(ConfigurationName As String, Value As String) As ReturnStatus

    End Interface
End Namespace

