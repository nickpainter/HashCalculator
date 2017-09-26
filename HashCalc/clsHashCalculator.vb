'Compute the hash values of a file
'clsHashCalculator
'Use:

'Dim hashCalc As New clsHashCalculator
'	hashCalc.ComputeSHA256Hash(fileToHash)

'Supported Hash functions:
'MD5
'SHA1
'SHA256
'SHA384
'SHA512

Imports System.Security.Cryptography
Imports System.IO

Public Class clsHashCalculator

    'Public Function ComputeMD5Hash(filePath As String)
    '    'calculates the MD5 hash of a given file
    '    Dim s As New MD5CryptoServiceProvider
    '    Dim filebytes() As Byte = IO.File.ReadAllBytes(filePath)
    '    Dim hash() As Byte = s.ComputeHash(filebytes)
    '    Dim calculatedHash As String = ByteArrayToString(hash)
    '    Return calculatedHash
    'End Function
    


    Property hashAlgoritm As HashAlgorithm

    Public Sub New(ha As System.Security.Cryptography.HashAlgorithm)


        'System.Security.Cryptography.HashAlgorithm.Create(h
        'If ha = "MD5" Then
        '    hashAlgoritm = System.Security.Cryptography.MD5CryptoServiceProvider.Create()
        'End If
    End Sub


    Public Function ComputeMD5Hash(filePath As String)
        Dim ha As New MD5CryptoServiceProvider
        Dim stream As System.IO.Stream = IO.File.Open(filePath, FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)
        Dim hash() As Byte = ha.ComputeHash(stream)
        Dim calculatedHash As String = ByteArrayToString(hash)
        Return calculatedHash
    End Function

    'Use the managed classes for better platform compatibility, but use the non-managed hash alrorithms for modern platforms in order to achieve better performance
    Public Function ComputeSHA1Hash(filePath As String)

        Dim hashAlgorithm As New SHA1CryptoServiceProvider
        Dim stream As System.IO.Stream = IO.File.Open(filePath, FileMode.Open, FileAccess.Read, IO.FileShare.Read)
        Dim hash() As Byte = hashAlgorithm.ComputeHash(stream)
        Dim calculatedHash As String = ByteArrayToString(hash)
        Return calculatedHash
    End Function

    'Public Function ComputeSHA1Hash(filePath As String)
    '    'calculates the SHA1 hash of a given file
    '    Dim s As New SHA1Managed
    '    Dim filebytes() As Byte = IO.File.ReadAllBytes(filePath)
    '    Dim hash() As Byte = s.ComputeHash(filebytes)
    '    Dim calculatedHash As String = ByteArrayToString(hash)
    '    Return calculatedHash
    'End Function


    Public Function ComputeSHA256Hash(filePath As String)
        'calculates the SHA256 hash of a given file
        Dim s As New SHA256Managed
        Dim filebytes() As Byte = IO.File.ReadAllBytes(filePath)
        Dim hash() As Byte = s.ComputeHash(filebytes)
        Dim calculatedHash As String = ByteArrayToString(hash)
        Return calculatedHash
    End Function

    Public Function ComputeSHA384Hash(filePath As String)
        'calculates the SHA384 hash of a given file
        Dim s As New SHA384Managed
        Dim filebytes() As Byte = IO.File.ReadAllBytes(filePath)
        Dim hash() As Byte = s.ComputeHash(filebytes)
        Dim calculatedHash As String = ByteArrayToString(hash)
        Return calculatedHash
    End Function

    Public Function ComputeSHA512Hash(filePath As String)
        'calculates the SHA512 hash of a given file
        Dim s As New SHA512Managed
        Dim filebytes() As Byte = IO.File.ReadAllBytes(filePath)
        Dim hash() As Byte = s.ComputeHash(filebytes)
        Dim calculatedHash As String = ByteArrayToString(hash)
        Return calculatedHash
    End Function

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
End Class