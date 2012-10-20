Public Class InMemoryRepository
    Private Shared _connectedUsers As ICollection(Of Users)
    Private Shared _instance As InMemoryRepository = Nothing
    Private Shared ReadOnly max_random As Integer = 3

    Public Shared Function GetInstance() As InMemoryRepository
        If _instance Is Nothing Then
            _instance = New InMemoryRepository()
        End If
        Return _instance
    End Function

#Region "Private methods"

    Private Sub New()
        _connectedUsers = New List(Of Users)()
    End Sub

#End Region

#Region "Repository methods"

    Public ReadOnly Property Users() As IQueryable(Of Users)
        Get
            Return _connectedUsers.AsQueryable()
        End Get
    End Property

    Public Sub Add(user As Users)
        _connectedUsers.Add(user)
    End Sub

    Public Sub Remove(user As Users)
        _connectedUsers.Remove(user)
    End Sub


#End Region
End Class

