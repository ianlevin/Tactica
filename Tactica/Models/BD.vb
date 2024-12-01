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
                    .Nombre = reader("Nombre").ToString(),
                    .Precio = Convert.ToDouble(reader("Precio")),
                    .Categoria = reader("Categoria").ToString()
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
    Public Shared Function ActualizarProducto(producto As Producto) As Boolean

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "UPDATE Productos SET Nombre = @Nombre, Precio = @Precio, Categoria = @Categoria WHERE ID = @ID"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@Nombre", producto.Nombre)
            command.Parameters.AddWithValue("@Precio", producto.Precio)
            command.Parameters.AddWithValue("@Categoria", producto.Categoria)
            command.Parameters.AddWithValue("@ID", producto.ID)

            connection.Open()
            Dim rowsAffected As Integer = command.ExecuteNonQuery()
            connection.Close()
            Return rowsAffected > 0
        End Using
    End Function
    Public Shared Function EliminarProducto(productoID As Integer) As Boolean

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "DELETE FROM Productos WHERE ID = @ID"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@ID", productoID)

            connection.Open()
            Dim rowsAffected As Integer = command.ExecuteNonQuery()
            connection.Close()
            Return rowsAffected > 0
        End Using
    End Function
    Public Shared Function AgregarNuevoProducto(producto As Producto) As Integer

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "INSERT INTO Productos (Nombre, Precio, Categoria) VALUES (@Nombre, @Precio, @Categoria)"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@Nombre", producto.Nombre)
            command.Parameters.AddWithValue("@Precio", producto.Precio)
            command.Parameters.AddWithValue("@Categoria", producto.Categoria)

            connection.Open()
            command.ExecuteNonQuery()
            connection.Close()

            Dim queryID As String = "SELECT MAX(ID) FROM Productos"
            Dim commandID As New SqlCommand(queryID, connection)

            connection.Open()
            Dim clienteID As Integer = Convert.ToInt32(commandID.ExecuteScalar())
            connection.Close()

            Return clienteID
        End Using
    End Function
    Public Shared Function BuscarClientes(busqueda As String) As List(Of Cliente)
        Dim clientes As New List(Of Cliente)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM Clientes where Cliente like @busqueda"
            Dim command As New SqlCommand(query, connection)

            Dim busquedaConPorcentaje As String = "%" & busqueda & "%"
            command.Parameters.AddWithValue("@busqueda", busquedaConPorcentaje)

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
    Public Shared Function BuscarProductos(busqueda As String) As List(Of Producto)
        Dim productos As New List(Of Producto)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM Productos where Nombre like @busqueda"
            Dim command As New SqlCommand(query, connection)

            Dim busquedaConPorcentaje As String = "%" & busqueda & "%"
            command.Parameters.AddWithValue("@busqueda", busquedaConPorcentaje)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim producto As New Producto With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .Nombre = reader("Nombre").ToString(),
                    .Precio = Convert.ToDouble(reader("Precio")),
                    .Categoria = reader("Categoria").ToString()
                }
                productos.Add(producto)
            End While

            reader.Close()
        End Using
        Return productos
    End Function
    Public Shared Function ObtenerCategorias() As List(Of String)
        Dim categorias As New List(Of String)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT DISTINCT Categoria FROM Productos"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                categorias.Add(reader("Categoria").ToString())
            End While
            reader.Close()
        End Using

        Return categorias
    End Function
    Public Shared Function FiltrarPorCategoria(categoria As String) As List(Of Producto)
        Dim productos As New List(Of Producto)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM productos WHERE Categoria = @categoria"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@categoria", categoria)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim producto As New Producto With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .Nombre = reader("Nombre").ToString(),
                    .Precio = Convert.ToDouble(reader("Precio")),
                    .Categoria = reader("Categoria").ToString()
                }
                productos.Add(producto)
            End While

            reader.Close()
        End Using

        Return productos
    End Function
    Public Shared Function FiltrarPorPrecio(desde As Integer, hasta As Integer) As List(Of Producto)
        Dim productos As New List(Of Producto)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM productos WHERE Precio >= @desde AND Precio <= @hasta"
            Dim command As New SqlCommand(query, connection)

            command.Parameters.AddWithValue("@desde", desde)
            command.Parameters.AddWithValue("@hasta", hasta)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim producto As New Producto With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .Nombre = reader("Nombre").ToString(),
                    .Precio = Convert.ToDouble(reader("Precio")),
                    .Categoria = reader("Categoria").ToString()
                }
                productos.Add(producto)
            End While

            reader.Close()
        End Using

        Return productos
    End Function
End Class
