Imports System.IO
Imports System.Xml.Serialization

Public Class RoomType
    Public Property Code As String
    Public Property Description As String
    Public Property ForSearch As Boolean
End Class

Public Class RoomTypes
    Public Shared Function GetRoomTypes() As List(Of RoomType)
        Dim roomTypes = New List(Of RoomType)

        Try
            Dim reader = Xml.XmlReader.Create(AppDomain.CurrentDomain.RelativeSearchPath + "\\NEMO_ROOM_TYPES.xml")
            Dim xmlSerializer = New XmlSerializer(GetType(List(Of RoomType)))

            roomTypes = CType(xmlSerializer.Deserialize(reader), List(Of RoomType))

            reader.Close()
        Catch ex As Exception
        End Try

        Return roomTypes
    End Function
End Class