<!DOCTYPE html>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>@ViewBag.Title - Mi aplicación ASP.NET</title>
    @Styles.Render("~/Content/css")
    @Scripts.Render("~/bundles/modernizr")
</head>
<body>
    <div class="navbar">
        <div class="navbar-logo">
            <img src="/Content/logo.png" alt="Logo">
            <h1>Sistema de Gestión</h1>
        </div>

        <div class="navbar-menu">
            <a href="@Url.Action("AbmClientes", "Home")" class="active">ABM Clientes</a>
            <a href="@Url.Action("AbmProductos", "Home")">ABM Productos</a>
            <a href="@Url.Action("AbmVentas", "Home")">ABM Ventas</a>
        </div>

        <div class="navbar-toggle" onclick="toggleMenu()">
            <span></span>
            <span></span>
            <span></span>
        </div>
    </div>

    @RenderBody()


    @Scripts.Render("~/bundles/jquery")
    @Scripts.Render("~/bundles/bootstrap")
    @RenderSection("scripts", required:=False)
</body>
</html>
<script>function toggleMenu() {
        const menu = document.querySelector('.navbar-menu');
        menu.classList.toggle('show');
    }</script>