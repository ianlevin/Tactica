Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function
    Function ActualizarCliente(cliente As Cliente) As Boolean
        Return BD.ActualizarCliente(cliente)
    End Function
    Function ActualizarVentaItems(ventaItems As VentaItems) As Boolean
        Return BD.ActualizarVentaItems(ventaItems)
    End Function
    Function EliminarCliente(clienteID As Integer) As Boolean
        Return BD.EliminarCliente(clienteID)
    End Function
    Public Function AgregarNuevoCliente(cliente As Cliente) As JsonResult
        Return Json(New With {.ID = BD.AgregarNuevoCliente(cliente)})
    End Function
    Function BuscarClientes(busqueda As String) As JsonResult
        Return Json(New With {.ID = BD.BuscarClientes(busqueda)})
    End Function
    Function BuscarPorCliente(busqueda As String) As JsonResult
        Dim ventas = BD.BuscarPorCliente(busqueda)
        Dim ventasItems = BD.BuscarItemsPorCliente(busqueda)
        Dim productos = BD.ObtenerProductos()
        Return Json(New With {
        .ventas = ventas,
        .ventaItems = ventasItems,
        .productos = productos
    }, JsonRequestBehavior.AllowGet)
    End Function
    Function BuscarProductos(busqueda As String) As JsonResult
        Return Json(New With {.ID = BD.BuscarProductos(busqueda)})
    End Function
    Function AbmClientes() As ActionResult
        ViewBag.Clientes = BD.ObtenerClientes()
        Return View("Abm_clientes")
    End Function
    Function AbmProductos() As ActionResult
        ViewBag.Productos = BD.ObtenerProductos()
        ViewBag.Categorias = BD.ObtenerCategorias()
        Return View("Abm_productos")
    End Function
    Function AbmVentas() As ActionResult
        ViewBag.Ventas = BD.ObtenerVentas()
        ViewBag.Productos = BD.ObtenerProductos()
        ViewBag.VentasItems = BD.ObtenerVentasItems()
        Return View("Abm_ventas")
    End Function
    Function ActualizarProducto(producto As Producto) As Boolean
        Return BD.ActualizarProducto(producto)
    End Function
    Function EliminarProducto(productoID As Integer) As Boolean
        Return BD.EliminarProducto(productoID)
    End Function
    Function FiltrarPorCategoria(categoria As String) As JsonResult
        Return Json(New With {.ID = BD.FiltrarPorCategoria(categoria)})
    End Function
    Function FiltrarPorPrecio(desde As Integer, hasta As Integer) As JsonResult
        Return Json(New With {.ID = BD.FiltrarPorPrecio(desde, hasta)})
    End Function
    Public Function AgregarNuevoProducto(producto As Producto) As JsonResult
        Return Json(New With {.ID = BD.AgregarNuevoProducto(producto)})
    End Function

    Function About() As ActionResult
        ViewData("Message") = "Your application description page."

        Return View()
    End Function
    Function Contact() As ActionResult
        ViewData("Message") = "Your contact page."

        Return View()
    End Function
End Class
