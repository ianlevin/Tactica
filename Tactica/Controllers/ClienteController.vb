Public Class ClienteController
    Inherits System.Web.Mvc.Controller
    Function ActualizarCliente(cliente As Cliente) As Boolean
        Return BD.ActualizarCliente(cliente)
    End Function
    Function EliminarCliente(clienteID As Integer) As Boolean
        Return BD.EliminarCliente(clienteID)
    End Function
    Public Function AgregarNuevoCliente(cliente As Cliente) As JsonResult
        Return Json(New With {.ID = BD.AgregarNuevoCliente(cliente)})
    End Function
    Function AbmClientes() As ActionResult
        ViewBag.Clientes = BD.ObtenerClientes()
        Return View("Abm_clientes")
    End Function
    Function BuscarClientes(busqueda As String) As JsonResult
        Return Json(New With {.ID = BD.BuscarClientes(busqueda)})
    End Function
End Class