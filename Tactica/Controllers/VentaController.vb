Public Class VentaController
    Inherits System.Web.Mvc.Controller
    Function ActualizarVentaItems(ventaItems As List(Of VentaItems)) As ActionResult
        BD.ActualizarVentaItems(ventaItems)
        Return RedirectToAction("AbmVentas")
    End Function
    Function AbmVentas() As ActionResult
        ViewBag.Ventas = BD.ObtenerVentas()
        ViewBag.Productos = BD.ObtenerProductos()
        ViewBag.VentasItems = BD.ObtenerVentasItems()
        Return View("Abm_ventas")
    End Function
    Public Function AgregarNuevaVenta(nuevaVenta As Venta, ventaItems As List(Of VentaItems)) As Boolean
        Return BD.AgregarNuevaVenta(nuevaVenta, ventaItems)
    End Function
    Function EliminarVenta(ventaId As Integer) As Boolean
        Return BD.EliminarVenta(ventaId)
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
    Public Function ObtenerClientes() As JsonResult
        Return Json(New With {.clientes = BD.ObtenerClientes()})
    End Function
    Function EliminarProductoVenta(ventaId As Integer, productoId As Integer) As Boolean
        Return BD.EliminarProductoVenta(ventaId, productoId)
    End Function
End Class