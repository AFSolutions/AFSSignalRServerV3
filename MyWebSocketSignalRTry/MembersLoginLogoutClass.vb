Public Delegate Sub MemebersListChangedEventHandler()

Public Class MembersLoginLogoutClass
    Private Shared FInstance As MembersLoginLogoutClass = Nothing



    Public Shared Event MembersListChanged As MemebersListChangedEventHandler


    Private Shared _connectionsList As New Concurrent.ConcurrentBag(Of Users)
    Private _removelist As New Concurrent.ConcurrentBag(Of Users)


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
        Dim tuser As Users = nuser

        Dim taken = ConnectionsList.TryTake(tuser)

        If taken Then
            RaiseEvent MembersListChanged()
            Return True
        Else
            _removelist.Add(tuser)
            RaiseEvent MembersListChanged()
            Return True
        End If
    End Function

    Public Function GetAllUsers() As List(Of Users)
        Dim retlist As New List(Of Users)
        For Each a In ConnectionsList
            If _removelist.Count = 0 Then
                retlist.Add(a)
                Continue For
            End If

            Dim g = From b In _removelist
                    Where b.ConnectionId = a.ConnectionId And b.Name = a.Name
                    Select b

            If g.Count = 0 Then
                retlist.Add(a)
            End If
        Next
        Return retlist
    End Function
End Class
