Imports NLog
Imports PDFSigner.My.Resources

Module Program
    Private _logger As Logger

    Public Sub Main(args As String())
        Dim currentDomain As AppDomain = AppDomain.CurrentDomain
        AddHandler currentDomain.UnhandledException, AddressOf UnhandledException

        LogManager.Configuration = LogConfig.Setup
        _logger = LogManager.GetCurrentClassLogger()

        Dim f As New FrmPdfSigner(args)
        f.ShowDialog()
    End Sub

    Friend Sub UnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        _logger.Fatal(Messages.Unhandled_Exception, e.ExceptionObject.ToString)
        MsgBox(e.ExceptionObject.Message, MsgBoxStyle.Critical, Resources.MessageBoxTitle)
    End Sub

End Module
