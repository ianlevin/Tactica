﻿Public Class HomeController
    Inherits System.Web.Mvc.Controller

    Function Index() As ActionResult
        Return View()
    End Function
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
    Function AbmProductos() As ActionResult
        ViewBag.Productos = BD.ObtenerProductos()
        Return View("Abm_productos")
    End Function
    Function ActualizarProducto(producto As Producto) As Boolean
        Return BD.ActualizarProducto(producto)
    End Function
    Function EliminarProducto(productoID As Integer) As Boolean
        Return BD.EliminarProducto(productoID)
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
