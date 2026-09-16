Imports System.ComponentModel
Imports Kofax.Capture.AdminModule.InteropServices
Imports PDFSignerCommon

Friend Class ComboFiller
    Friend Sub FillComboWithIndexes(DocClass As DocumentClass, Combo As ComboBox)
        Dim indexes = New BindingList(Of String)

        For Each i As IndexField In DocClass.IndexFields
            indexes.Add(i.Name)
        Next

        Combo.DataSource = indexes
    End Sub

    Friend Sub FillComboWithMetadataTypes(Combo As ComboBox)
        Dim types = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = MetaDataType.Text, .DisplayText = "Text"},
            New ComboKeyValue() With {.Id = MetaDataType.XML, .DisplayText = "XML"},
            New ComboKeyValue() With {.Id = MetaDataType.DublinCoreXML, .DisplayText = "Dublin Core XML"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = types
    End Sub

    Friend Sub FillComboWithViewers(Combo As ComboBox)
        Dim types = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = DocumentViewerType.SimpleViewer, .DisplayText = "Simple viewer"},
            New ComboKeyValue() With {.Id = DocumentViewerType.AdvancedViewer, .DisplayText = "Advanced viewer"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = types
    End Sub

    Friend Sub FillComboWithCryptoProviders(Combo As ComboBox)
        Dim types = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = CryptoProviderType.PDFSigner, .DisplayText = "PDF Signer"},
            New ComboKeyValue() With {.Id = CryptoProviderType.PDFStreamer, .DisplayText = "PDF Streamer"},
            New ComboKeyValue() With {.Id = CryptoProviderType.MQFTP, .DisplayText = "MQFTP"},
            New ComboKeyValue() With {.Id = CryptoProviderType.MNBSigner, .DisplayText = "MNB Signer"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = types
    End Sub

    Friend Sub FillComboWithHashMethods(Combo As ComboBox)
        Dim methods = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = HashType.SHA1, .DisplayText = "SHA-1"},
            New ComboKeyValue() With {.Id = HashType.SHA224, .DisplayText = "SHA-224"},
            New ComboKeyValue() With {.Id = HashType.SHA256, .DisplayText = "SHA-256"},
            New ComboKeyValue() With {.Id = HashType.SHA384, .DisplayText = "SHA-384"},
            New ComboKeyValue() With {.Id = HashType.SHA512, .DisplayText = "SHA-512"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = methods
    End Sub

    Friend Sub FillComboWithRevocationMethods(Combo As ComboBox)
        Dim methods = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = RevocationType.None, .DisplayText = "None"},
            New ComboKeyValue() With {.Id = RevocationType.CRL, .DisplayText = "CRL"},
            New ComboKeyValue() With {.Id = RevocationType.OCSP, .DisplayText = "OCSP"},
            New ComboKeyValue() With {.Id = RevocationType.OCSPWithCRLFallback, .DisplayText = "OCSP (CRL fallback)"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = methods
    End Sub

    Friend Sub FillComboWithProxyAuthMethods(Combo As ComboBox)
        Dim methods = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = ProxyAuthenticationMethod.NoAuthentication, .DisplayText = "No authentication"},
            New ComboKeyValue() With {.Id = ProxyAuthenticationMethod.UserPassword, .DisplayText = "User/Password"},
            New ComboKeyValue() With {.Id = ProxyAuthenticationMethod.Digest, .DisplayText = "Digest"},
            New ComboKeyValue() With {.Id = ProxyAuthenticationMethod.NTLM, .DisplayText = "NTLM"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = methods
    End Sub

    ''' <summary>Fills a combo with the four PAdES baseline levels the built-in provider offers.</summary>
    Friend Sub FillComboWithPAdESLevels(Combo As ComboBox)
        Dim levels = New BindingList(Of ComboKeyValue) From {
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineB, .DisplayText = "PAdES B-B (signature only)"},
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineT, .DisplayText = "PAdES B-T (signature time stamp)"},
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineLT, .DisplayText = "PAdES B-LT (embedded revocation information)"},
            New ComboKeyValue() With {.Id = PAdESLevelType.BaselineLTA, .DisplayText = "PAdES B-LTA (archive document time stamp)"}
        }

        Combo.DisplayMember = NameOf(ComboKeyValue.DisplayText)
        Combo.ValueMember = NameOf(ComboKeyValue.Id)
        Combo.DataSource = levels
    End Sub
End Class