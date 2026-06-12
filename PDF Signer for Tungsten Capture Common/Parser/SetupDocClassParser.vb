Imports Kofax.Capture.SDK.Data

Public Module SetupDocClassParser
    Dim _SetupDocClassDict As Dictionary(Of String, IACDataElement)

    Public Sub ResetSetupDocClassData()
        _SetupDocClassDict = New Dictionary(Of String, IACDataElement)
    End Sub

    Public Function GetSetupDocClassByFormTypeName(SetupRootElement As IACDataElement, FormTypeName As String) As IACDataElement
        Dim retryCounter As Integer = 5
        Dim retryDelay = 1000

        If _SetupDocClassDict.ContainsKey(FormTypeName) Then
            Return _SetupDocClassDict(FormTypeName)
        Else
            Dim SetupFormTypesElement As IACDataElement
            Dim SetupFormType As IACDataElement
            Dim SetupDocClassesElement As IACDataElement
            Dim SetupDocClasses As IACDataElementCollection

            For i As Integer = 1 To retryCounter

                SetupDocClassesElement = SetupRootElement.FindChildElementByName("DocumentClasses")
                SetupDocClasses = SetupDocClassesElement.FindChildElementsByName("DocumentClass")

                For Each SetupDocClass As IACDataElement In SetupDocClasses
                    SetupFormTypesElement = SetupDocClass.FindChildElementByName("FormTypes")
                    SetupFormType = SetupFormTypesElement.FindChildElementByAttribute("FormType", "Name", FormTypeName)

                    If SetupFormType IsNot Nothing Then
                        _SetupDocClassDict.Add(FormTypeName, SetupDocClass)
                        Return SetupDocClass
                    End If
                Next

                If i < retryCounter Then
                    Threading.Thread.Sleep(retryDelay)
                End If
            Next
        End If

        Return Nothing
    End Function

End Module
