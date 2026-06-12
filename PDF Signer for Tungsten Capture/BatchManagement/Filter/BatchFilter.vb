Imports System.ComponentModel
Imports System.Xml.Linq
Imports System.Linq
Imports PDFSigner.My.Resources
Imports PDFSignerCommon

Friend Class BatchFilter
    Public Property Enabled As Boolean
    Public Property Name As String
    Public Property Items As BindingList(Of BatchFilterConditionItem)
    Friend Property FilterXml As String

    Public Sub New()
        Items = New BindingList(Of BatchFilterConditionItem)
    End Sub

    Friend Function Validate() As String
        If Not Enabled Then Return String.Empty
        Dim ExistingConditions As New BindingList(Of BatchFilterConditionItem)

        Dim c As Integer = 1

        For Each i As BatchFilterConditionItem In Items
            If ExistingConditions.Any(Function(f) f.ConditionFilterBy = i.ConditionFilterBy AndAlso f.ConditionFilterBy <> FilterBy.BatchClass) Then
                Return String.Format(Messages.Filter_By_Criteria_Not_Unique, c)
            End If

            If String.IsNullOrEmpty(i.ConditionFilterBy) Then
                Return String.Format(Messages.Filter_By_Criteria_Not_Set, c)
            End If

            ExistingConditions.Add(i)

            If i.ConditionFilterBy = FilterBy.BatchField Then
                If String.IsNullOrEmpty(i.ConditionBatchField) Then
                    Return String.Format(Messages.Filter_Batch_Field_Not_Set, c, GUIText.Filter_Batch_Field)
                End If
            End If

            If String.IsNullOrEmpty(i.ConditionRelation) Then
                Return String.Format(Messages.Filter_Relation_Not_Set, c)
            End If

            If Not FilterRelation.GetSupportedFilterRelations(i.ConditionFilterBy).Contains(i.ConditionRelation) Then
                Return String.Format(Messages.Filter_Relation_Not_Supported, c)
            End If

            If i.ConditionValue Is Nothing OrElse String.IsNullOrEmpty(i.ConditionValue.ToString) Then
                Return String.Format(Messages.Filter_Value_Not_Set, c)
            End If

            c += 1
        Next

        Return String.Empty
    End Function

    Friend Sub LoadFromXml()
        Items.Clear()

        If String.IsNullOrEmpty(FilterXml) Then Return

        Dim dom As XDocument = XDocument.Parse(FilterXml)

        For Each filterCriteria As XElement In dom.Root.Elements("FilterCriteria")
            Dim bfci As New BatchFilterConditionItem With {
                .ConditionFilterBy = filterCriteria.Element("Name"),
                .ConditionBatchField = filterCriteria.Element("BatchField"),
                .ConditionRelation = filterCriteria.Element("Operator")
                }

            Select Case bfci.ConditionFilterBy
                Case FilterBy.BatchClass
                    bfci.ConditionValue = String.Join(";", filterCriteria.Element("ValueList").Elements("Value").Select(Function(x As XElement) x.Value))
                Case FilterBy.BatchCreationDateTime
                    Dim dt As Date
                    Date.TryParseExact(filterCriteria.Element("Value").Value, Constant.BatchFilterDateFormat, Nothing, Globalization.DateTimeStyles.None, dt)
                    bfci.ConditionValue = dt
                Case FilterBy.Priority, FilterBy.Status, FilterBy.HasError
                    Dim i As Integer
                    Integer.TryParse(filterCriteria.Element("Value").Value, i)
                    bfci.ConditionValue = i
                Case Else
                    bfci.ConditionValue = filterCriteria.Element("Value").Value
            End Select

            Items.Add(bfci)
        Next

        Enabled = Converter.StringToBoolean(dom.Root.Element("FilteringEnabled"))
    End Sub

    Friend Function ToXml() As String
        Dim dec As New XDeclaration("1.0", "utf-16", "yes")
        Dim dom As New XDocument(dec)

        Dim root As New XElement("FilterCriteriaList")

        For Each filterCriteria As BatchFilterConditionItem In Items
            If filterCriteria.ConditionFilterBy = FilterBy.BatchClass Then 'Batch classes must go to a vlaue list
                Dim fc As New XElement("FilterCriteria",
                                      New XElement("Name", filterCriteria.ConditionFilterBy),
                                      New XElement("BatchField", filterCriteria.ConditionBatchField),
                                      New XElement("Operator", filterCriteria.ConditionRelation),
                                      New XElement("ValueList"))

                Dim vl As XElement = fc.Element("ValueList")
                Dim batchClassList As New List(Of String)
                batchClassList.AddRange(filterCriteria.ConditionValue.ToString.Split(New Char() {";"}, StringSplitOptions.RemoveEmptyEntries))

                For Each bc As String In batchClassList
                    vl.Add(New XElement("Value", bc))
                Next

                root.Add(fc)
            Else
                Dim fc As New XElement("FilterCriteria",
                                      New XElement("Name", filterCriteria.ConditionFilterBy),
                                      New XElement("BatchField", filterCriteria.ConditionBatchField),
                                      New XElement("Operator", filterCriteria.ConditionRelation)
                                      )

                Dim fcValue As XElement

                'DateTime data types must be formatted according to XML date format
                If filterCriteria.ConditionFilterBy = FilterBy.BatchCreationDateTime AndAlso
                    TypeOf filterCriteria.ConditionValue Is Date Then
                    fcValue = New XElement("Value", CType(filterCriteria.ConditionValue, Date).ToString(Constant.BatchFilterDateFormat))
                Else
                    fcValue = New XElement("Value", filterCriteria.ConditionValue)
                End If

                fc.Add(fcValue)
                root.Add(fc)
            End If
        Next

        root.Add(New XElement("FilteringEnabled", Converter.BooleanToLiteralString(Enabled, False)))

        dom.Add(root)

        FilterXml = dom.ToString
        Return FilterXml
    End Function
End Class
