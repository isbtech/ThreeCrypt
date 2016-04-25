Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Org.BouncyCastle.Crypto
Imports Org.BouncyCastle.Security
Imports ThreeCrypt.Functions

Module Main
    ' A VB.Net Implementation of the ThreeFish Algorithm...
    ' ------
    ' The intention of this program is to encrypt a set group of files
    ' and preserve the folder structure. The encryption is using the 1024
    ' bit implementation of ThreeFish. To preserve minimal RAM overhead,
    ' this program writes as it encrypts in 1024 byte chunks.
    ' ------
    ' If you use or modify this please leave credits to it's origional creator.
    ' Happy encrypting :^)
    ' ~Killpot
    Dim _hashedPassword, _passwordPath, _folderPath, _outputPath As String
    Sub Main()
        Console.Title = "ThreeCrpyt"
        Console.CursorVisible = False
        Print("Welcome to ThreeCrypt, a highley secure VB.Net implementation of ThreeFish.", ConsoleColor.Cyan)
        Print("If you enjoy this tool, please leave feedback to encourage further development on the project.", ConsoleColor.Cyan)
        Print("Press any key to continue...") : Console.ReadKey(True)
RePass:
        Console.Clear()
        Print("Please select a path to save your encryption key to...", ConsoleColor.Cyan)

        Using sfd As New SaveFileDialog()
            sfd.Title = "Select the path to save your encryption key to..."
            sfd.Filter = "Data Files (*.dat)|*.dat|All Files (*.*)|*.*"

            If sfd.ShowDialog() = DialogResult.OK And sfd.CheckPathExists Then
                Print("Selected save path is: " + sfd.FileName, ConsoleColor.Cyan)
                _passwordPath = sfd.FileName
            Else
                Print("Path invalid, or no path included.")
                Print("Press any key to continue...") : Console.ReadKey(True)
                GoTo RePass
            End If
        End Using
        Dim sr As New SecureRandom()
        Dim kgp = New KeyGenerationParameters(sr, 1024)
        Dim kg As New CipherKeyGenerator()
        kg.Init(kgp)
        Dim key As Byte() = kg.GenerateKey()

        File.WriteAllBytes(_passwordPath, key)
        _hashedPassword = Convert.ToBase64String(key)

        Print("Base64 Encoded Password: " + _hashedPassword, ConsoleColor.Cyan)
        Print("Press any key to continue...") : Console.ReadKey(True)
RePath:
        Console.Clear()
        Print("Now please select the directory to encrypt...", ConsoleColor.Cyan)
        Using fbd As New FolderBrowserDialog()
            fbd.Description = "Select the directory you wish to encrypt..."
            If fbd.ShowDialog() = DialogResult.OK And My.Computer.FileSystem.DirectoryExists(fbd.SelectedPath) Then
                Print("Selected path is: " + fbd.SelectedPath, ConsoleColor.Cyan)
                _folderPath = fbd.SelectedPath
            Else
                Print("Path invalid, or no path inputed.")
                Print("Press any key to continue...") : Console.ReadKey(True)
                GoTo RePath
            End If
        End Using
        Print("Press any key to continue...") : Console.ReadKey(True)
ReSave:
        Console.Clear()
        Print("Please select where you would like to output the encrypted data to.", ConsoleColor.Cyan)
        Using sfd As New SaveFileDialog()
            sfd.Title = "Select the path to save the encrypted data to..."
            sfd.Filter = "Data Files (*.dat)|*.dat|All Files (*.*)|*.*"

            If sfd.ShowDialog() = DialogResult.OK And sfd.CheckPathExists Then
                Print("Selected save path is: " + sfd.FileName, ConsoleColor.Cyan)
                _outputPath = sfd.FileName
            Else
                Print("Path invalid, or no path included.")
                Print("Press any key to continue...") : Console.ReadKey(True)
                GoTo ReSave
            End If
        End Using
        Print("Press any key to continue...") : Console.ReadKey(True)
        Console.Clear()
        Print("All preparations done, ready to begin encrypting.", ConsoleColor.Cyan)
        Print("Press any key to continue...") : Console.ReadKey(True)
        Console.Clear()
        Print("Building file list...", ConsoleColor.Cyan)
        Dim fileList As List(Of String) = GetFilesRecursive(_folderPath)
        Print("Done.", ConsoleColor.Green)
        Dim stw As New Stopwatch
        stw.Start()
        Using sw As New StreamWriter(_outputPath, False)
            For Each filePath As String In fileList
                Print("Encrypting: " + New FileInfo(filePath).Name, ConsoleColor.Green)
                Dim relativepath As Byte() = Encoding.ASCII.GetBytes(filePath.Replace(_folderPath, Nothing).TrimStart("\"))
                sw.WriteLine(Convert.ToBase64String(relativepath) + ":")
                Dim fs As Stream = File.OpenRead(filePath)
                Dim inputBuffer As Byte() = New Byte(1023) {}
                Dim rl As Integer
                While (InlineAssignHelper(rl, fs.Read(inputBuffer, 0, 1024))) > 0
                    sw.WriteLine(PerformEncrypt(inputBuffer, key))
                End While
            Next
            sw.Close()
        End Using
        stw.Stop()
        Print("Encryption finished in: " + stw.Elapsed().ToString, ConsoleColor.Cyan)
        Print("Press any key to continue...") : Console.ReadKey(True)
reStub:
        Console.Clear()
        Print("Would you like to generate a decryption stub too? (y/n)", ConsoleColor.Cyan)
        Select Case Console.ReadKey().KeyChar.ToString()
            Case "y"
ReDS:
                Using sfd As New SaveFileDialog
                    sfd.Title = "Select the path to save the decryption stub to..."
                    sfd.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"

                    If sfd.ShowDialog() = DialogResult.OK And sfd.CheckPathExists Then
                        Print("Selected save path is: " + sfd.FileName, ConsoleColor.Cyan)
                        System.IO.File.WriteAllBytes(sfd.FileName, My.Resources.ThreeCryptStub)
                    Else
                        Print("Path invalid, or no path included.")
                        Print("Press any key to continue...") : Console.ReadKey(True)
                        GoTo ReDS
                    End If
                End Using
            Case "n"
                Exit Sub
            Case Else
                GoTo reStub
        End Select
    End Sub
End Module