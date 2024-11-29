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
    Public Shared Function ActualizarCliente(cliente As Cliente) As Boolean

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "UPDATE Clientes SET Cliente = @Cliente, Telefono = @Telefono, Correo = @Correo WHERE ID = @ID"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@Cliente", cliente.Cliente)
            command.Parameters.AddWithValue("@Telefono", cliente.Telefono)
            command.Parameters.AddWithValue("@Correo", cliente.Correo)
            command.Parameters.AddWithValue("@ID", cliente.ID)

            connection.Open()
            Dim rowsAffected As Integer = command.ExecuteNonQuery()
            connection.Close()
            Return rowsAffected > 0
        End Using
    End Function
    Public Shared Function EliminarCliente(clienteID As Integer) As Boolean

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM Clientes WHERE ID = @ID"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@ID", clienteID)

            connection.Open()
            Dim rowsAffected As Integer = command.ExecuteNonQuery()
            connection.Close()
            Return rowsAffected > 0
        End Using
    End Function
    Public Shared Function AgregarNuevoCliente(cliente As Cliente) As Integer

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO Clientes (Cliente, Telefono, Correo) VALUES (@Cliente, @Telefono, @Correo)"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@Cliente", cliente.Cliente)
            command.Parameters.AddWithValue("@Telefono", cliente.Telefono)
            command.Parameters.AddWithValue("@Correo", cliente.Correo)

            connection.Open()
            command.ExecuteNonQuery()
            connection.Close()

            Dim queryID As String = "SELECT MAX(ID) FROM Clientes"
            Dim commandID As New SqlCommand(queryID, connection)

            connection.Open()
            Dim clienteID As Integer = Convert.ToInt32(commandID.ExecuteScalar())
            connection.Close()

            Return clienteID
        End Using
    End Function
End Class
