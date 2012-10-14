Public Delegate Sub MemebersListChangedEventHandler()

Public Class MembersLoginLogoutClass
    Private Shared FInstance As MembersLoginLogoutClass = Nothing



    Public Shared Event MembersListChanged As MemebersListChangedEventHandler


    Private Shared _connectionsList As New Concurrent.ConcurrentBag(Of Users)

    Public Shared Property ConnectionsList As Concurrent.ConcurrentBag(Of Users)
        Get
            Return _connectionsList
        End Get
        Set(value As Concurrent.ConcurrentBag(Of Users))
            _connectionsList = value
        End Set
    End Property


    Protected Sub New()

    End Sub

    Public Shared Function Instance() As MembersLoginLogoutClass

        If (FInstance Is Nothing) Then
            FInstance = New MembersLoginLogoutClass
        End If

        Return FInstance
    End Function

    Public Function AddUser(ByVal nuser As Users) As Boolean
        Try
            Dim a = (From b In ConnectionsList
                    Where b.Name = nuser.Name And b.ConnectionId = nuser.ConnectionId
                    Select b).FirstOrDefault

            If a Is Nothing Then
                ConnectionsList.Add(nuser)
                Return True
            Else
                Return True
            End If
            RaiseEvent MembersListChanged()
        Catch
            Return False
        End Try

    End Function

    Public Function removeUser(ByVal nuser As Users) As Boolean
        ConnectionsList.TryTake(nuser)

        If ConnectionsList.Contains(nuser) Then
            Return False
        Else
            RaiseEvent MembersListChanged()
            Return True
        End If
    End Function

    Public Function GetAllUsers() As List(Of Users)
        Dim retlist As New List(Of Users)
        For Each a In ConnectionsList
            retlist.Add(a)
        Next
        Return retlist
    End Function
End Class
