Imports System.IO
Imports System.Text
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Crypto.Modes
Imports Org.BouncyCastle.Utilities.Encoders
Imports Org.BouncyCastle.Crypto.Paddings
Imports Org.BouncyCastle.Crypto.Engines
Imports Org.BouncyCastle.Crypto.Parameters

Public Class Functions
    Public Shared Sub Print(ByVal message As String, Optional ByVal color As ConsoleColor = ConsoleColor.Gray)
        Console.ForegroundColor = color
        Console.WriteLine(message)
        Console.ForegroundColor = ConsoleColor.Gray
    End Sub

    Public Shared Function GetFilesRecursive(ByVal initial As String) As List(Of String)
        Dim result As New List(Of String)
        Dim stack As New Stack(Of String)
        stack.Push(initial)
        Do While (stack.Count > 0)
            Dim dir As String = stack.Pop
            Try
                result.AddRange(Directory.GetFiles(dir, "*.*"))
                Dim directoryName As String
                For Each directoryName In Directory.GetDirectories(dir)
                    stack.Push(directoryName)
                Next
            Catch ex As Exception
            End Try
        Loop
        Return result
    End Function

    Public Shared Function GetInput() As String
        Console.CursorVisible = True
        Dim retVal As String = Console.ReadLine()
        Console.CursorVisible = False
        Return retVal
    End Function

    Public Shared Function PerformEncrypt(inByte As Byte(), key As Byte()) As String
        Dim outStr As New MemoryStream()
        Dim cipher As New PaddedBufferedBlockCipher(New CbcBlockCipher(New ThreefishEngine(1024)))

        cipher.Init(True, New KeyParameter(key))

        Dim inBlockSize As Integer = 1024
        Dim outBlockSize As Integer = cipher.GetOutputSize(inBlockSize)

        Dim outblock As Byte() = New Byte(outBlockSize - 1) {}

        Dim outl As Integer
        outl = cipher.ProcessBytes(inByte, 0, inBlockSize, outblock, 0)

        If outl > 0 Then
            Hex.Encode(outblock, 0, outl, outStr)
        End If

        Try
            outl = cipher.DoFinal(outblock, 0)
            If outl > 0 Then
                Hex.Encode(outblock, 0, outl, outStr)
            End If

        Catch generatedExceptionName As CryptoException
            MsgBox(generatedExceptionName.Message)
        End Try
        Return Convert.ToBase64String(outStr.ToArray())
    End Function

    Public Shared Function PerformDecrypt(inStr As String, key As Byte()) As Byte()
        Dim outMs As New MemoryStream()
        Dim cipher As New PaddedBufferedBlockCipher(New CbcBlockCipher(New ThreefishEngine(1024)))
        cipher.Init(False, New KeyParameter(key))

        Dim outL As Integer
        Dim inblock, outblock As Byte()

        inblock = Hex.Decode(Convert.FromBase64String(inStr))
        outblock = New Byte(cipher.GetOutputSize(inblock.Length) - 1) {}

        outL = cipher.ProcessBytes(inblock, 0, inblock.Length, outblock, 0)

        If outL > 0 Then
            outMs.Write(outblock, 0, outL)
        End If

        Try
            outL = cipher.DoFinal(outblock, 0)
            If outL > 0 Then
                outMs.Write(outblock, 0, outL)
            End If

        Catch generatedExceptionName As CryptoException
            MsgBox(generatedExceptionName.Message)
        End Try
        Return outMs.ToArray()
    End Function

    Public Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function
End Class