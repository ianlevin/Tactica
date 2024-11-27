Imports System.Data.SqlClient

Public Class BD
    Private Shared ReadOnly connectionString As String = "Server=localhost;Uid=sa;Pwd=sasa;MultipleActiveResultSets=True;Timeout=120;Database=pruebademo;"

    Public Shared Function ObtenerClientes() As List(Of Cliente)
        Dim clientes As New List(Of Cliente)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM clientes"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim cliente As New Cliente With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .Cliente = reader("Cliente").ToString(),
                    .Telefono = reader("Telefono").ToString(),
                    .Correo = reader("Correo").ToString()
                }
                clientes.Add(cliente)
            End While

            reader.Close()
        End Using

        Return clientes
    End Function
    Public Shared Function ObtenerProductos() As List(Of Producto)
        Dim productos As New List(Of Producto)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM productos"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim producto As New Producto With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .Nombre = reader("Cliente").ToString(),
                    .Precio = Convert.ToDouble(reader("ID")),
                    .Categoria = reader("Correo").ToString()
                }
                productos.Add(producto)
            End While

            reader.Close()
        End Using

        Return productos
    End Function
End Class
