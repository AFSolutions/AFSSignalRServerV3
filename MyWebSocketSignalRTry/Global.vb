Imports SignalR.Hubs
Imports AFSSignalRServer.Global_asax
Imports System.Runtime.Serialization
REM ready version v2.8
<HubName("wsHub")>
Public Class wsHub
    Inherits Hub

    Sub New()
        MyBase.New()
        Dim fffg = MembersLoginLogoutClass.Instance
        AddHandler fffg.MembersListChanged, AddressOf MembersChangeing
    End Sub

    Private Sub MembersChangeing()
        Dim fffg = MembersLoginLogoutClass.Instance
        RemoveHandler fffg.MembersListChanged, AddressOf MembersChangeing
        Me.GetUsersHelper()
    End Sub


    Public Sub Login(ByVal Conid As String, ByVal name As String)
        Try
            Dim ffg As MembersLoginLogoutClass = MembersLoginLogoutClass.Instance

            Dim nUser As New Users With {.ConnectionId = Conid, .Name = name}

            If ffg.AddUser(nUser) Then
                Clients.clientUserLoggedIn(nUser)
            Else
                Caller.clientIsConnected(False)
            End If


            Clients.clientGetUsers(ffg.GetAllUsers)
        Catch ex As Exception
            Caller.clientIsConnected(False)
            Debug.WriteLine(ex.Message.ToString & vbCrLf & ex.Source.ToString & vbCrLf & ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub GetUsersHelper(Optional ByVal ee As Users = Nothing)
        Dim ffg = MembersLoginLogoutClass.Instance
        Debug.WriteLine("alluserslist: " & ffg.GetAllUsers.Count)
        Dim nlist As New List(Of Users)
        For Each a In ffg.GetAllUsers
            If ee Is Nothing Then
                nlist.Add(a)
            ElseIf ee IsNot Nothing Then
                If ee.ConnectionId = a.ConnectionId And ee.Name = a.Name Then
                    Continue For
                End If
            End If
        Next

        Clients.clientGetUsers(nlist) 'sJSON)
    End Sub

    Public Sub GetUsers()
        Try
            Me.GetUsersHelper()
        Catch ex As Exception
            Caller.clientOnErrorOccured(ex.Message.ToString)
        End Try
    End Sub

    Public Sub ConnectedUsersCount()
        Try

            'Caller.clientConnectedUsersCount(ConnectionsList.Count)
        Catch ex As Exception
            Caller.clientOnErrorOccured(ex.Message.ToString)
        End Try
    End Sub

    Public Sub LogOut(ByVal name As String, ByVal connid As String)
        Try

            Dim ffg As MembersLoginLogoutClass = MembersLoginLogoutClass.Instance
            Dim user = (New Users With {.Name = name, .ConnectionId = connid})
            ffg.removeUser(user)

            For Each a In ffg.GetAllUsers
                If a.ConnectionId <> connid Then
                    Clients(a.ConnectionId).clientUserLoggedOut(user)
                End If
            Next
            
            Clients.clientGetUsers(ffg.GetAllUsers)
        Catch ex As Exception
            Caller.clientOnErrorOccured(ex.Message.ToString)
        End Try
    End Sub


    Public Sub BroadCastMessage(ByVal conid As String, ByVal message As String)
        Clients.clientGotMessage(conid, message)
    End Sub

    Public Sub SendMessage(ByVal ToId As String, ByVal mesage As String)
        Dim ffg = MembersLoginLogoutClass.Instance
        Clients(ToId).clientGotSeperateMessage((From a In ffg.GetAllUsers
                                               Where a.ConnectionId = Context.ConnectionId
                                               Select a).FirstOrDefault, mesage)
    End Sub


    Public Sub UserWritesMessage(ByVal conid As String)
        Clients(conid).clientPartnerWritesMessage(Context.ConnectionId)
    End Sub

  


    'Public Sub StreamData(ByVal toid As String, ByVal filename As String, ByVal arrb As Byte())
    '    Clients(toid).clientGetStream(filename, arrb)
    'End Sub

    REM for filelenght < 2,5 megabytes
#Region "simple files getting"
    Public Sub SendFiletoUser(ByVal FileGetter As String, ByVal ServerFrom As String, ByVal filename As String, ByVal bytearr As Byte())
        Clients(FileGetter).clientGetStream(ServerFrom, filename, bytearr)
    End Sub


    'Public Sub SendFiletoUser(ByVal file As SimpleFileGetterType)
    '    Clients(file.FileGetter).clientGetStream(file.ServerFrom, file.Filename, file.ByteArr)
    'End Sub
#End Region

#Region "recieve getting files okay..."
    Public Sub AskForFileRecieve(ByVal server As String, ByVal SendTo As String, ByVal filename As String)
        'Clients(client).clientAskForFileRecieve(Context.ConnectionId, filename)
        Clients(SendTo).clientAskForFileRecieve(server, filename)
    End Sub

    Public Sub AskForMultipleFileRecieve(ByVal server As String, ByVal SendTo As String, ByVal listoffiles As List(Of String))
        Clients(SendTo).clientAskForMultipleFileRecieve(server, listoffiles)
    End Sub

    Public Sub YesNoFileRecieve(ByVal Answerer As String, ByVal ServerSendTo As String, ByVal filename As String, ByVal yesno As Boolean)
        Clients(ServerSendTo).clientYesNoFileRecieve(Answerer, filename, yesno)
    End Sub
#End Region

#Region "multiparted sending files"

    'Public Sub InitMultiPartedFileSending(ByVal toid As String, ByVal filename As String, ByVal numberofParts As String, ByVal FileFullLength As String)
    '    Clients(toid).clientInitMultiPartedFileSend(Context.ConnectionId, filename, numberofParts, FileFullLength)
    'End Sub

    'Public Sub SendMultiPartedSegment(ByVal toid As String, ByVal filename As String, ByVal partnb As String, ByVal segment As Byte())
    '    Clients(toid).clientMultiPartSegmentSent(Context.ConnectionId, filename, partnb, segment)
    'End Sub

    'Public Sub MultiPartedFileSegmentSending(ByVal toid As String, ByVal filename As String, ByVal partnumber As String)
    '    Clients(toid).clientSendSegments(Context.ConnectionId, filename, partnumber)
    'End Sub

    'Public Sub MultiPartedFileSegmentGotten(ByVal toid As String, ByVal filename As String, ByVal partnumber As String)
    '    Clients(toid).clientSendNextSegment(Context.ConnectionId, filename, partnumber)
    'End Sub

    'Public Sub MultiPartedFileSenT(ByVal toid As String, ByVal filename As String)
    '    Clients(toid).MultiPartedFileSentReady(filename)
    'End Sub

    Public Sub InitMultiPartedFileSending(ByVal Getter As String, ByVal Sender As String, ByVal filename As String, ByVal numberofParts As String, ByVal FileFullLength As String)
        Clients(Getter).clientInitMultiPartedFileSend(Sender, filename, numberofParts, FileFullLength)
    End Sub

    Public Sub SendMultiPartedSegment(ByVal Reciever As String, ByVal server As String, ByVal filename As String, ByVal partnb As String, ByVal segment As Byte())
        Clients(Reciever).clientMultiPartSegmentSent(server, Reciever, filename, partnb, segment)
    End Sub

    Public Sub MultiPartedFileSegmentSending(ByVal Server As String, ByVal Reciever As String, ByVal filename As String, ByVal partnumber As String)
        Clients(Server).clientSendSegments(Reciever, filename, partnumber)
    End Sub

    Public Sub MultiPartedFileSegmentGotten(ByVal toid As String, ByVal filename As String, ByVal partnumber As String)
        Clients(toid).clientSendNextSegment(Context.ConnectionId, filename, partnumber)
    End Sub

    Public Sub MultiPartedFileSenT(ByVal server As String, ByVal reciever As String, ByVal filename As String)
        Clients(reciever).MultiPartedFileSentReady(filename)
    End Sub
#End Region

End Class


<DataContract>
Public Class Users
    <DataMember>
    Public Property Name As String
    <DataMember>
    Public Property ConnectionId As String
End Class