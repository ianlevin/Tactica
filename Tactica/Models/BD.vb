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
    Public Shared Function ObtenerVentasItems() As List(Of VentaItems)
        Dim ventasItems As New List(Of VentaItems)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT * FROM VentasItems"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim ventaItem As New VentaItems With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .IDVenta = Convert.ToInt32(reader("IDVenta")),
                    .IDProducto = Convert.ToInt32(reader("IDProducto")),
                    .PrecioUnitario = Convert.ToDouble(reader("PrecioUnitario")),
                    .Cantidad = Convert.ToInt32(reader("Cantidad")),
                    .PrecioTotal = Convert.ToDouble(reader("PrecioTotal"))
                }
                ventasItems.Add(ventaItem)
            End While

            reader.Close()
        End Using

        Return ventasItems
    End Function
    Public Shared Function ObtenerProductosPorVenta() As List(Of List(Of Producto))
        Dim productosPorVenta As New List(Of List(Of Producto))
        Dim idsVentas As New List(Of Integer)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT ID FROM ventas"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                idsVentas.Add(reader("ID"))
            End While

            reader.Close()

            Dim queryProductos As String = "select Productos.* from Productos left join ventasitems on Productos.ID = ventasitems.IDProducto left join ventas on ventasitems.IDVenta = ventas.ID where ventas.ID = @ID"
            Dim commandProductos As New SqlCommand(queryProductos, connection)

            For Each ID As Integer In idsVentas
                commandProductos.Parameters.Clear()
                commandProductos.Parameters.AddWithValue("@ID", ID)

                Dim readerProductos As SqlDataReader = commandProductos.ExecuteReader()
                Dim productosPorUnaVenta As New List(Of Producto)

                While readerProductos.Read()
                    Dim producto As New Producto() With {
                    .ID = Convert.ToInt32(readerProductos("ID")),
                    .Nombre = Convert.ToString(readerProductos("Nombre")),
                    .Precio = Convert.ToDecimal(readerProductos("Precio")),
                    .Categoria = readerProductos("Categoria").ToString()
                }
                    productosPorUnaVenta.Add(producto)
                End While
                readerProductos.Close()

                productosPorVenta.Add(productosPorUnaVenta)
            Next

        End Using

        Return productosPorVenta
    End Function
    Public Shared Function ObtenerCantidadProductosPorVenta() As List(Of List(Of Integer))
        Dim productosPorVenta As New List(Of List(Of Integer))
        Dim idsVentas As New List(Of Integer)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT ID FROM ventas"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                idsVentas.Add(reader("ID"))
            End While

            reader.Close()

            Dim queryProductos As String = "select ventasitems.Cantidad from ventasitems left join ventas on ventasitems.IDVenta = ventas.ID where ventas.ID = @ID"
            Dim commandCantidad As New SqlCommand(queryProductos, connection)

            For Each ID As Integer In idsVentas
                commandCantidad.Parameters.Clear()
                commandCantidad.Parameters.AddWithValue("@ID", ID)

                Dim readerProductos As SqlDataReader = commandCantidad.ExecuteReader()
                Dim cantidadPorUnaVenta As New List(Of Integer)

                While readerProductos.Read()
                    Dim cantidad As Integer = Convert.ToInt32(readerProductos("Cantidad"))
                    cantidadPorUnaVenta.Add(cantidad)
                End While
                readerProductos.Close()

                productosPorVenta.Add(cantidadPorUnaVenta)
            Next

        End Using

        Return productosPorVenta
    End Function
    Public Shared Function ObtenerVentas() As List(Of Venta)
        Dim ventas As New List(Of Venta)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT ventas.ID, ventas.IDCliente, ventas.Fecha, ventas.Total, Clientes.Cliente AS 'NombreCliente' FROM ventas LEFT JOIN clientes ON ventas.IDCliente = clientes.ID"
            Dim command As New SqlCommand(query, connection)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim venta As New Venta With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .IDCliente = Convert.ToInt32(reader("IDCliente")),
                    .Fecha = Convert.ToDateTime(reader("Fecha")),
                    .Total = Convert.ToInt32(reader("Total")),
                    .NombreCliente = reader("NombreCliente").ToString()
                }
                ventas.Add(venta)
            End While

            reader.Close()
        End Using

        Return ventas
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
    Public Shared Function ActualizarVentaItems(ventasItems As VentaItems) As Boolean

        Return True
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
    Public Shared Function BuscarPorCliente(busqueda As String) As List(Of Venta)
        Dim ventas As New List(Of Venta)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "SELECT ventas.ID, ventas.IDCliente, ventas.Fecha, ventas.Total, Clientes.Cliente AS 'NombreCliente' from ventas left join clientes on ventas.IDCliente = clientes.ID where clientes.Cliente like @busqueda"
            Dim command As New SqlCommand(query, connection)

            Dim busquedaConPorcentaje As String = "%" & busqueda & "%"
            command.Parameters.AddWithValue("@busqueda", busquedaConPorcentaje)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                Dim venta As New Venta With {
                    .ID = Convert.ToInt32(reader("ID")),
                    .IDCliente = Convert.ToInt32(reader("IDCliente")),
                    .Fecha = Convert.ToDateTime(reader("Fecha")),
                    .Total = Convert.ToInt32(reader("Total")),
                    .NombreCliente = reader("NombreCliente").ToString()
                }
                ventas.Add(venta)
            End While

            reader.Close()
        End Using
        Return ventas
    End Function
    Public Shared Function BuscarItemsPorCliente(busqueda As String) As List(Of VentaItems)
        Dim idsVentas As New List(Of Integer)
        Dim ventaItems As New List(Of VentaItems)

        Using connection As New SqlConnection(connectionString)
            Dim query As String = "select ventas.ID from ventas left join clientes on ventas.IDCliente = clientes.ID where clientes.Cliente like @busqueda"
            Dim command As New SqlCommand(query, connection)

            Dim busquedaConPorcentaje As String = "%" & busqueda & "%"
            command.Parameters.AddWithValue("@busqueda", busquedaConPorcentaje)

            connection.Open()
            Dim reader As SqlDataReader = command.ExecuteReader()

            While reader.Read()
                idsVentas.Add(Convert.ToInt32(reader("ID")))
            End While

            reader.Close()

            For Each ID As Integer In idsVentas
                Dim queryVentasItems As String = "select ventasitems.* from ventasitems where ventasitems.IDVenta = @ID"
                Using commandVentasItems As New SqlCommand(queryVentasItems, connection)
                    commandVentasItems.Parameters.Clear()
                    commandVentasItems.Parameters.AddWithValue("@ID", ID)

                    Dim readerVentasItems As SqlDataReader = commandVentasItems.ExecuteReader()

                    While readerVentasItems.Read()
                        Dim ventaItem As New VentaItems With {
                        .ID = Convert.ToInt32(readerVentasItems("ID")),
                        .IDVenta = Convert.ToInt32(readerVentasItems("IDVenta")),
                        .IDProducto = Convert.ToInt32(readerVentasItems("IDProducto")),
                        .PrecioUnitario = Convert.ToDouble(readerVentasItems("PrecioUnitario")),
                        .Cantidad = Convert.ToInt32(readerVentasItems("Cantidad")),
                        .PrecioTotal = Convert.ToDouble(readerVentasItems("PrecioTotal"))
                    }
                        ventaItems.Add(ventaItem)
                    End While

                    readerVentasItems.Close()
                End Using
            Next

        End Using
        Return ventaItems
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
