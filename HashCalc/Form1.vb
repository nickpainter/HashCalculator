Imports System.Threading
Imports System.Security.Cryptography
Imports System.IO

Public Class Form1

    'Supported Hash functions:
    'MD5
    'SHA1
    'SHA256
    'SHA384
    'SHA512

    Private Sub Form1_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        cmbHashType.SelectedIndex = 0
        Me.AllowDrop = True
        Me.progBar.Maximum = 100
        Me.progBar.Style = ProgressBarStyle.Continuous
    End Sub


    'UI Methods

    Private Sub btnCalculate_Click(sender As System.Object, e As System.EventArgs) Handles btnCalculate.Click
        lblResult.Visible = False
        If Trim(txtFilePath.Text) <> String.Empty Then
            CalculateHash()
            'Moved the calculation logic to a background thread so this method
            'no longerneeds to be performed in the background
            'Dim tCalcHash As Thread = New Thread(AddressOf CalculateHash)
            'tCalcHash.IsBackground = True
            'tCalcHash.Start()
        Else
            ChangeForeColor(lblResult, Color.Black)
            ChangeText(lblResult, "Enter file path.")
            ChangeVisibility(lblResult, True)
        End If

    End Sub

    Private Sub btnCompare_Click(sender As System.Object, e As System.EventArgs) Handles btnCompare.Click
        Dim tCheckHash As Thread = New Thread(AddressOf CheckHash)
        tCheckHash.IsBackground = True
        tCheckHash.Start()
    End Sub

    Private Sub CheckHash()
        ChangeVisibility(lblResult, False)
        If Trim(txtExistingHash.Text) <> Nothing And Trim(txtCalculatedHash.Text) <> Nothing Then

            If CheckHash(txtExistingHash.Text, txtCalculatedHash.Text) Then
                ChangeForeColor(lblResult, Color.Green)
                ChangeText(lblResult, "Match")
                ChangeVisibility(lblResult, True)
            Else
                ChangeForeColor(lblResult, Color.Red)
                ChangeText(lblResult, "Mismatch")
                ChangeVisibility(lblResult, True)
            End If
        Else
            ChangeForeColor(lblResult, Color.Red)
            ChangeText(lblResult, "Enter existing hash.")
            ChangeVisibility(lblResult, True)
        End If
    End Sub

    Private Sub btnBrowse_Click(sender As System.Object, e As System.EventArgs) Handles btnBrowse.Click
        Dim sFileName As String
        sFileName = SelectFile()
        ChangeText(txtFilePath, sFileName)
    End Sub

    Private Function SelectFile() As String
        Dim sFileName As String
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Open File Dialog"
        fd.InitialDirectory = "C:\"
        fd.Filter = "All files (*.*)|*.*|All files (*.*)|*.*"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True

        If fd.ShowDialog() = DialogResult.OK Then
            sFileName = fd.FileName
        Else
            sFileName = String.Empty
        End If
        Return sFileName
    End Function


    Private Sub CalculateHash()
        Dim filePath As String = Trim(txtFilePath.Text)
        Dim calculatedHash As String = ""
        'Dim hashCalc As New clsHashCalculator

        'Dim sTest As String = GetSelectedValue(cmbHashType)
        Dim ha As System.Security.Cryptography.HashAlgorithm

        Select Case GetSelectedValue(cmbHashType)
            Case "MD5"
                ha = New MD5Cng()
            Case "SHA1"
                ha = New SHA1CryptoServiceProvider
            Case "SHA256"
                ha = New SHA256CryptoServiceProvider
            Case "SHA384"
                ha = New SHA384CryptoServiceProvider
            Case "SHA512"
                ha = New SHA512CryptoServiceProvider
            Case Else
                Exit Sub
        End Select

        Dim trd As New Thread(Sub()
                                  Dim stream As New ProgressStream(filePath, Me.progBar)
                                  Dim hash() As Byte = ha.ComputeHash(stream)
                                  calculatedHash = ByteArrayToString(hash)
                                  ChangeText(txtCalculatedHash, calculatedHash.ToUpper)
                              End Sub)
        trd.IsBackground = True
        trd.Start()

    End Sub

    Public Function CheckHash(referenceHash As String, calculatedHash As String)
        'compares the reference hash to the calculated hash.  If the two are the same returns true.  Else returns false.
        If referenceHash = calculatedHash Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim sb As New System.Text.StringBuilder(arrInput.Length * 2)
        For i As Integer = 0 To arrInput.Length - 1
            sb.Append(arrInput(i).ToString("X2"))

        Next
        Return sb.ToString().ToLower
    End Function


    Public Class ProgressStream
        Inherits FileStream

        Property pb As ProgressBar

        Public Event ProgressChanged(sender As ProgressStream, progress As Integer)

        Public Sub New(fileName As String)
            MyBase.New(fileName, FileMode.Open, FileAccess.Read)
        End Sub

        'Alternative new method with progressbar parameter that can be used
        'to update progressbar on the main UI thread
        Public Sub New(fileName As String, pbar As ProgressBar)
            MyBase.New(fileName, FileMode.Open, FileAccess.Read)
            pb = pbar
        End Sub

        Public ReadOnly Property Progress() As Integer
            Get
                Return CInt(Me.Position / Me.Length * 100)
            End Get
        End Property

        Public Overrides Function Read(array() As Byte, offset As Integer, count As Integer) As Integer
            Read = MyBase.Read(array, offset, count)
            RaiseEvent ProgressChanged(Me, Me.Progress)
        End Function

        Private Sub ProgressStream_ProgressChanged(sender As ProgressStream, progress As Integer) Handles Me.ProgressChanged
            'Update UI progress bar with a current indication of progress
            If pb.InvokeRequired Then
                pb.Invoke(Sub()
                              pb.Value = progress
                              pb.Update()
                              'Application.DoEvents()
                          End Sub)
            End If
        End Sub
    End Class


    'Multithreading Methods
    Private Delegate Function ReadComboDelegate(c As ComboBox) As String

    Private Function ReadComboBox(c As ComboBox) As String
        If c.InvokeRequired Then
            Dim del As New ReadComboDelegate(AddressOf Me.ReadComboBox)
            Return DirectCast(c.Invoke(del, c), String)
        Else
            Return c.Text
        End If

    End Function

    Private Delegate Function GetSelectedValueDelegate(ByVal ctrl As ComboBox) As String
    Private Function GetSelectedValue(ByVal ctrl As ComboBox) As String
        If ctrl.InvokeRequired Then
            Dim del As New GetSelectedValueDelegate(AddressOf Me.GetSelectedValue)
            Return DirectCast(ctrl.Invoke(del, ctrl), String)
        Else
            Return ctrl.Text
        End If



        'Dim sTest = ""
        'If ctrl.InvokeRequired Then
        '    ctrl.Invoke(New GetSelectedValueDelegate(AddressOf GetSelectedValue), New Object() {ctrl})
        '    sTest = ctrl.SelectedItem.ToString()
        '    ctrl.EndUpdate()
        'Else

        'End If
        ''Return ctrl.SelectedValue
        'Return sTest
        ''Return sTest
    End Function

    'Delegate Function GetSelectedValueDelegate(ByVal ctrl As ComboBox)
    'Private Function GetSelectedValue(ByVal ctrl As ComboBox)
    '    If ctrl.InvokeRequired Then
    '        ctrl.Invoke(New GetSelectedValueDelegate(AddressOf GetSelectedValue), New Object() {ctrl})
    '        Return Nothing 'ctrl.SelectedValue.ToString()
    '    End If
    '    Return ctrl.SelectedValue.ToString()
    'End Function

    Delegate Sub ChangeTextDelegate(ByVal ctrl As Control, ByVal str As String)
    Private Sub ChangeText(ByVal ctrl As Control, ByVal str As String)
        If ctrl.InvokeRequired Then
            ctrl.Invoke(New ChangeTextDelegate(AddressOf ChangeText), New Object() {ctrl, str})
            Return
        End If
        ctrl.Text = str
    End Sub

    'Delegate Sub AddTextDelegate(ByVal ctrl As Control, ByVal str As String)
    'Private Sub AddText(ByVal ctrl As Control, ByVal str As String)
    '    If ctrl.InvokeRequired Then
    '        ctrl.Invoke(New AddTextDelegate(AddressOf AddText), New Object() {ctrl, str})
    '        Return
    '    End If
    '    ctrl.Text = ctrl.Text + str
    'End Sub

    Delegate Sub ChangeVisibilityDelegate(ByVal ctrl As Control, ByVal bool As Boolean)
    Private Sub ChangeVisibility(ByVal ctrl As Control, ByVal bool As Boolean)
        If ctrl.InvokeRequired Then
            ctrl.Invoke(New ChangeVisibilityDelegate(AddressOf ChangeVisibility), New Object() {ctrl, bool})
            Return
        End If
        ctrl.Visible = bool
    End Sub

    Delegate Sub ChangeForeColorDelegate(ByVal ctrl As Control, ByVal color As Color) 'Changes the forecolor of the textbox control
    Private Sub ChangeForeColor(ByVal ctrl As Control, ByVal color As Color)
        If ctrl.InvokeRequired Then
            ctrl.Invoke(New ChangeForeColorDelegate(AddressOf ChangeForeColor), New Object() {ctrl, color})
            Return
        End If
        ctrl.ForeColor = color
    End Sub

    Private Sub txtExistingHash_TextChanged(sender As Object, e As System.EventArgs) Handles txtExistingHash.TextChanged
        lblResult.Visible = False
    End Sub

    Private Sub Form1_DragDrop(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop) 'Get the file path of the dropped item
        ChangeText(txtFilePath, files(0))
    End Sub

    Private Sub Form1_DragEnter(sender As Object, e As System.Windows.Forms.DragEventArgs) Handles Me.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub





End Class
