@ModelType List(Of Venta)

@Code
    Dim ventas = TryCast(ViewBag.Ventas, List(Of Venta))
    Dim productos = TryCast(ViewBag.Productos, List(Of Producto))
    Dim ventasItems = TryCast(ViewBag.VentasItems, List(Of VentaItems))
End Code

<h2>Lista de Ventas</h2>

<div class="row mb-3">
    <div class="col-md-3">
        <label for="filtro_fecha" class="form-label">Fecha de Venta</label>
        <input type="date" id="filtro_fecha" class="form-control" />
        <button class="btn btn-primary mt-2" onclick="FiltrarPorFecha()">Buscar por Fecha</button>
    </div>
    <div class="col-md-6">
        <label for="filtro_precio_desde" class="form-label">Rango de Precios</label>
        <div class="input-group">
            <input type="number" id="filtro_precio_desde" class="form-control" placeholder="Desde" />
            <input type="number" id="filtro_precio_hasta" class="form-control" placeholder="Hasta" />
        </div>
        <button class="btn btn-primary mt-2" onclick="FiltrarPorPrecio()">Buscar por Rango de Precios</button>
    </div>
</div>

<input type="text" id="buscar_venta" class="form-control mb-3" placeholder="Buscar por nombre de cliente" onkeyup="BuscarPorNombre()" />
<button class="btn btn-success mb-3" onclick="Nuevoventa()">Agregar Venta</button>

<table class="table table-bordered">
    <thead>
        <tr>
            <th>ID</th>
            <th>Nombre del Cliente</th>
            <th>Fecha de la Venta</th>
            <th>Productos</th>
            <th>Total</th>
            <th>Acciones</th>
        </tr>
    </thead>
    <tbody id="tabla_ventas">
        @For Each venta As Venta In ventas
            @<tr id="tr_@venta.ID">
                <td id="@venta.ID">@venta.ID</td>
                <td id="nombre_@venta.ID">@venta.NombreCliente</td>
                <td id="fecha_@venta.ID">@venta.Fecha</td>
                <td id="productos_@venta.ID">
                    @For Each ventaItem As VentaItems In ventasItems.Where(Function(v) v.IDVenta = venta.ID)
                        Dim productoNombre = productos.FirstOrDefault(Function(p) p.ID = ventaItem.IDProducto)?.Nombre
                        @<span id="producto_@ventaItem.IDProducto">@productoNombre</span>
                        @<span id="cantidad_@ventaItem.IDProducto">x @ventaItem.Cantidad</span>@<br />
                                            Next
                </td>
                <td id="total_@venta.ID">@venta.Total</td>
                <td>
                    <button type="button" class="btn btn-danger" onclick="Eliminar(@venta.ID)">❌</button>
                    <button class="btn btn-primary" onclick="Editar(@venta.ID)">Editar</button>
                </td>
            </tr>
        Next
    </tbody>
</table>


<script>
    function Editar(ventaId) {
    
    const fila = document.getElementById(`tr_${ventaId}`);

    const productosVenta = Array.from(fila.querySelectorAll(`#productos_${ventaId} span[id^='producto_']`)).map(el => ({
        id: el.id.split('_')[1],
        nombre: el.textContent.trim(),
        cantidad: el.nextElementSibling.textContent.trim().replace('x ', '')
    }));

    const productosDisponibles = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(productos));

    let productosHTML = "";
    productosVenta.forEach(producto => {
        productosHTML += `
            <div id="producto_editar_${producto.id}" class="mb-2 d-flex align-items-center">
                <input type="text" class="form-control me-2" id="cantidad_${producto.id}" value="${producto.cantidad}" style="width: 60px;" />
                <span class="me-2">${producto.nombre}</span>
                <button class="btn btn-danger btn-sm" onclick="EliminarProducto(${ventaId}, ${producto.id})">❌</button>
            </div>
        `;
    });

    const productosSeleccionables = productosDisponibles.filter(p => !productosVenta.some(v => v.id == p.ID));
    let dropdownHTML = `<div class="d-flex align-items-center mb-2">`;
    dropdownHTML += `<select id="nuevo_producto_${ventaId}" class="form-select me-2" style="width: auto;">`;
    dropdownHTML += `<option value="">Seleccione un producto</option>`;
    productosSeleccionables.forEach(producto => {
        dropdownHTML += `<option value="${producto.ID}">${producto.Nombre}</option>`;
    });
    dropdownHTML += `</select>`;
    dropdownHTML += `<button class="btn btn-success btn-sm" onclick="AgregarProducto(${ventaId})">Agregar Producto</button>`;
    dropdownHTML += `</div>`;

    fila.querySelector(`#productos_${ventaId}`).innerHTML = `
        ${productosHTML}
        ${dropdownHTML}
    `;

    const total = fila.querySelector(`#total_${ventaId}`).textContent.trim();
    fila.querySelector(`#total_${ventaId}`).innerHTML = `<input type="text" class="form-control" id="total_editar_${ventaId}" value="${total}" style="width: 100px;" />`;

    const acciones = fila.querySelector(`td:last-child`);
    acciones.innerHTML = `
        <button class="btn btn-success btn-sm me-2" onclick="Guardar(${ventaId})">Guardar</button>
        <button class="btn btn-secondary btn-sm" onclick="Cancelar(${ventaId})">Cancelar</button>
    `;
}

    function AgregarProducto(ventaId) {
        const nuevoProductoId = document.getElementById(`nuevo_producto_${ventaId}`).value;
        const nuevoProductoNombre = document.getElementById(`nuevo_producto_${ventaId}`).selectedOptions[0].text;
        if (!nuevoProductoId) {
            alert("Seleccione un producto válido.");
            return;
        }

        const productosContainer = document.getElementById(`productos_${ventaId}`);
        productosContainer.innerHTML += `
        <div id="producto_editar_${nuevoProductoId}">
            <input type="text" class="form-control mb-1" id="cantidad_${nuevoProductoId}" value="1" />
            <button class="btn btn-danger" onclick="EliminarProducto(${ventaId}, ${nuevoProductoId})">❌</button>
            ${nuevoProductoNombre}
        </div>
    `;
    }

    function EliminarProducto(ventaId, productoId) {
        const productoDiv = document.getElementById(`producto_editar_${productoId}`);
        productoDiv.parentNode.removeChild(productoDiv);
    }

    function Guardar(ventaId) {
    const fila = document.getElementById(`tr_${ventaId}`);

    // Obtener los productos editados
    const productos = Array.from(fila.querySelectorAll(`#productos_${ventaId} div[id^='producto_editar_']`)).map(el => {
        const idProducto = el.id.split('_')[2];
        const cantidad = el.querySelector("input").value;

        // Obtener el nombre del producto directamente del DOM
        const nombreProducto = el.querySelector(".producto-nombre")?.textContent.trim();

        return {
            IDVenta: ventaId,
            IDProducto: idProducto,
            Cantidad: cantidad,
            Nombre: nombreProducto,
            PrecioUnitario: 0,
            PrecioTotal: 0
        };
    });

    const total = parseFloat(document.getElementById(`total_editar_${ventaId}`).value);

    // Crear la lista de VentaItems
    const ventaItems = productos.map(p => ({
        IDVenta: p.IDVenta,
        IDProducto: p.IDProducto,
        Cantidad: parseInt(p.Cantidad, 10),
        PrecioUnitario: p.PrecioUnitario,
        PrecioTotal: p.Cantidad * p.PrecioUnitario
    }));

    console.log(ventaItems); // Verificar qué se está enviando

    // Enviar los datos al controlador mediante AJAX
    $.ajax({
        type: 'POST',
        url: '/Home/ActualizarVentaItems', // Asegúrate de que esta ruta sea correcta
        contentType: 'application/json', // Importante: especifica el tipo de contenido como JSON
        data: JSON.stringify({ ventaItems: ventaItems }), // Serializa los datos como JSON
        success: function (response) {
            // Actualizar el DOM con los datos guardados
            let productosHTML = productos.map(p => `
                <span id="producto_${p.IDProducto}">${p.Nombre}</span>
                <span id="cantidad_${p.IDProducto}">x ${p.Cantidad}</span><br>
            `).join('');

            fila.querySelector(`#productos_${ventaId}`).innerHTML = productosHTML;
            fila.querySelector(`#total_${ventaId}`).textContent = total.toFixed(2);

            // Restaurar los botones de acciones
            const acciones = fila.querySelector(`td:last-child`);
            acciones.innerHTML = `
                <button class="btn btn-primary btn-sm" onclick="Editar(${ventaId})">Editar</button>
                <button class="btn btn-danger btn-sm" onclick="Eliminar(${ventaId})">❌</button>
            `;
        },
        error: function () {
            alert("Ocurrió un error al guardar los cambios.");
        }
    });
}

    function FiltrarPorFecha() {
        const fecha = document.getElementById("filtro_fecha").value;

        if (!fecha) {
            alert("Por favor, selecciona una fecha.");
            return;
        }

        document.querySelectorAll("#tabla_ventas tr").forEach(row => {
            const fechaVenta = row.querySelector(`#fecha_${row.id.split('_')[1]}`)?.textContent.trim();

            const fechaFiltrada = new Date(fecha).toISOString().split("T")[0];

            if (fechaVenta === fechaFiltrada) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    }

    function FiltrarPorPrecio() {
        const precioDesde = parseFloat(document.getElementById("filtro_precio_desde").value || 0);
        const precioHasta = parseFloat(document.getElementById("filtro_precio_hasta").value || Infinity);

        if (precioDesde > precioHasta) {
            alert("El precio mínimo no puede ser mayor que el máximo.");
            return;
        }

        document.querySelectorAll("#tabla_ventas tr").forEach(row => {
            const totalVenta = parseFloat(row.querySelector(`#total_${row.id.split('_')[1]}`)?.textContent.trim() || 0);

            if (totalVenta >= precioDesde && totalVenta <= precioHasta) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }
        });
    }
    function BuscarPorNombre() {
        const busqueda = document.getElementById('buscar_venta').value.trim();

        $.ajax({
            type: 'POST',
            url: '/Home/BuscarPorCliente',
            dataType: 'JSON',
            data: { busqueda: busqueda },
            success: function (response) {
                if (response && Array.isArray(response.ventas)) {
                    const arrayVentas = response.ventas;
                    const arrayVentaItems = response.ventaItems;
                    const arrayProductos = response.productos;
                    const tablaVentas = document.getElementById('tabla_ventas');
                    tablaVentas.innerHTML = ''; 

                    arrayVentas.forEach(function (venta) {
                        const ventaItemsDeEstaVenta = arrayVentaItems.filter(item => item.IDVenta === venta.ID);

                        let fila = `<tr id="tr_${venta.ID}">
                        <td id="${venta.ID}">${venta.ID}</td>
                        <td id="nombre_${venta.ID}">${venta.NombreCliente}</td>
                        <td id="fecha_${venta.ID}">${formatDate(venta.Fecha)}</td>
                        <td id="productos_${venta.ID}">`;

                        ventaItemsDeEstaVenta.forEach(function (ventaItem) {
                            const producto = arrayProductos.find(p => p.ID === ventaItem.IDProducto);
                            const productoNombre = producto ? producto.Nombre : "Producto desconocido"; 

                            fila += `<span id="producto_${ventaItem.IDProducto}">${productoNombre}</span>
                        <span id="cantidad_${ventaItem.IDProducto}">x ${ventaItem.Cantidad}</span><br />`;
                        });

                        fila += `</td>
                        <td id="total_${venta.ID}">${venta.Total}</td>
                        <td>
                            <button type="button" class="btn btn-danger" onclick="Eliminar(${venta.ID})">❌</button>
                            <button class="btn btn-primary" onclick="Editar(${venta.ID})">Editar</button>
                        </td>
                    </tr>`;

                        tablaVentas.insertAdjacentHTML('beforeend', fila);
                    });
                } else {
                    console.error("La respuesta no contiene ventas válidas o está mal formada.");
                }
            },
            error: function (error) {
                console.error('Error al buscar ventas: ', error);
            }
        });
    }

    function formatDate(dateString) {
        let date;

        if (dateString && dateString.startsWith("/Date(") && dateString.endsWith(")/")) {
            const timestamp = dateString.slice(6, -2); 
            date = new Date(parseInt(timestamp)); 
        } else {
            date = new Date(dateString);
        }


        if (isNaN(date)) {
            return "Fecha inválida"; 
        }

        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    }






</script>