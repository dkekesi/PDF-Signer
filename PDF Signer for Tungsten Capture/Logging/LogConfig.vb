Imports NLog
Imports NLog.Config
Imports NLog.Targets
Imports PDFSignerCommon
Imports System.IO

Friend Module LogConfig
    Friend Function Setup() As LoggingConfiguration
        Dim conf As New LoggingConfiguration

        Dim fileTarget As New FileTarget With
            {
                .ArchiveEvery = FileArchivePeriod.Month,
                .ArchiveSuffixFormat = "_{1:yyMM}",
                .FileName = Path.Combine(KofaxRegistry.KofaxLogFolder, "PDFSigner.txt"),
                .Encoding = Text.Encoding.UTF8,
                .Layout = "${date:format=yyyy.MM.dd. HH\:mm\:ss.fff} [${level:uppercase=true}] ${machinename} (${windows-identity}) - ${message}"
            }

        Dim wr As New Wrappers.AsyncTargetWrapper With
            {
                .Name = "AsyncWrapper",
                .WrappedTarget = fileTarget
            }

        conf.AddTarget("file", fileTarget)

        Dim rule = New LoggingRule("*", LogLevel.Trace, fileTarget)
        conf.LoggingRules.Add(rule)

        Return conf
    End Function
End Module
