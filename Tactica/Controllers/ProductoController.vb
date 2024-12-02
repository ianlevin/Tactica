Public Class ProductoController
    Inherits System.Web.Mvc.Controller
    Function BuscarProductos(busqueda As String) As JsonResult
        Return Json(New With {.ID = BD.BuscarProductos(busqueda)})
    End Function
    Function AbmProductos() As ActionResult
        ViewBag.Productos = BD.ObtenerProductos()
        ViewBag.Categorias = BD.ObtenerCategorias()
        Return View("Abm_productos")
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
End Class