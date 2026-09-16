Imports PDFSignerCommon

''' <summary>Proves the test project can load both product assemblies and see their Friend types.</summary>
<TestClass>
Public Class ProjectSmokeTests
    <TestMethod>
    Public Sub Common_enum_is_visible()
        Assert.AreEqual(2, CInt(RevocationType.OCSP))
    End Sub

    <TestMethod>
    Public Sub Runtime_friend_type_is_visible()
        Assert.IsNotNull(GetType(PDFSigner.SignatureRequest))
    End Sub
End Class
