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
<button class="btn btn-success mb-3" onclick="NuevaVenta()">Agregar Venta</button>

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
                        @<span id="producto_@ventaItem.IDProducto" data-id-venta-item="@ventaItem.ID">@productoNombre</span>
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
    function Eliminar(ventaId) {
        $.ajax({
            type: 'POST',
            url: '/Venta/EliminarVenta',
            data: { ventaId: ventaId },
            success: function (response) {
                resolve(response);
            },
            error: function (error) {
                reject(error);
            }
        });
        const clienteRow = document.getElementById('tr_' + ventaId);
        clienteRow.remove();
    }

    function Editar(ventaId) {
    const fila = document.getElementById(`tr_${ventaId}`);

    const productosVenta = Array.from(fila.querySelectorAll(`#productos_${ventaId} span[id^='producto_']`)).map(el => ({
        id: el.id.split('_')[1],
        nombre: el.textContent.trim(),
        cantidad: el.nextElementSibling.textContent.trim().replace('x ', ''),
        idVentaItem: el.dataset.idVentaItem || 0
    }));

    const productosDisponibles = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(productos));

    let productosHTML = "";
    productosVenta.forEach(producto => {
        productosHTML += `
            <div id="producto_editar_${producto.id}" class="mb-2 d-flex align-items-center" data-id-venta-item="${producto.idVentaItem}">
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


        $.ajax({
            type: 'POST',
            url: '/Venta/EliminarProductoVenta',
            data: {ventaId:ventaId,productoId:productoId},
            success: function (response) {
                const productoDiv = document.getElementById(`producto_editar_${productoId}`);
                productoDiv.parentNode.removeChild(productoDiv);
            },
            error: function () {
                alert("Ocurrió un error al guardar los cambios.");
            }
        });
    }

    function Guardar(ventaId) {
        var productosDisponibles = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(productos));
        const fila = document.getElementById(`tr_${ventaId}`);

        const productos = Array.from(fila.querySelectorAll(`#productos_${ventaId} div[id^='producto_editar_']`)).map(el => {
            const idProducto = el.id.split('_')[2];
            const cantidad = el.querySelector("input").value;

            const idVentaItem = el.dataset.idVentaItem || 0;
            const nombreProducto = el.querySelector(".producto-nombre")?.textContent.trim();

            const producto = productosDisponibles.find(p => p.ID == idProducto);
            const precioUnitario = producto ? producto.Precio : 0;

            return {
                IDVenta: ventaId,
                IDProducto: idProducto,
                ID: parseInt(idVentaItem, 10),
                Cantidad: cantidad,
                Nombre: nombreProducto,
                PrecioUnitario: precioUnitario,
                PrecioTotal: precioUnitario * cantidad
            };
        });

        const total = parseFloat(document.getElementById(`total_editar_${ventaId}`).value);

        const ventaItems = productos.map(p => ({
            IDVenta: p.IDVenta,
            IDProducto: p.IDProducto,
            ID: p.ID,
            Cantidad: parseInt(p.Cantidad, 10),
            PrecioUnitario: p.PrecioUnitario,
            PrecioTotal: p.Cantidad * p.PrecioUnitario
        }));

        console.log(ventaItems);
        $.ajax({
            type: 'POST',
            url: '/Venta/ActualizarVentaItems',
            contentType: 'application/json',
            data: JSON.stringify({ ventaItems: ventaItems }),
            success: function (response) {
                window.location.href = '/Venta/AbmVentas';
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
            const fechaVentaTexto = row.querySelector(`#fecha_${row.id.split('_')[1]}`)?.textContent.trim();

            if (fechaVentaTexto) {
                const [dia, mes, anio] = fechaVentaTexto.split(" ")[0].split("/");
                const fechaVenta = `${anio}-${mes}-${dia}`;

                if (fechaVenta === fecha) {
                    row.style.display = "";
                } else {
                    row.style.display = "none";
                }
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
            url: '/Venta/BuscarPorCliente',
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

    function NuevaVenta() {
    const tabla = document.getElementById('tabla_ventas');
    const nuevaFilaId = `nueva_venta`;

    const productosDisponibles = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(productos));
    const clientesDisponibles = [];

    $.ajax({
        type: 'POST',
        dataType: 'JSON',
        url: '/Venta/ObtenerClientes',
        success: function (response) {
            if (response && Array.isArray(response.clientes)) {
                clientesDisponibles.push(...response.clientes);
            }

            const filaHTML = `
                <tr id="${nuevaFilaId}">
                    <td>0</td>
                    <td>
                        <select id="nuevo_cliente_${nuevaFilaId}" class="form-select">
                            <option value="">Seleccione un cliente</option>
                            ${clientesDisponibles.map(cliente => `<option value="${cliente.ID}">${cliente.Cliente}</option>`).join('')}
                        </select>
                    </td>
                    <td>${new Date().toLocaleDateString()}</td>
                    <td id="productos_${nuevaFilaId}">
                        <div class="d-flex align-items-center mb-2">
                            <select id="nuevo_producto_${nuevaFilaId}" class="form-select me-2">
                                <option value="">Seleccione un producto</option>
                                ${productosDisponibles.map(producto => `<option value="${producto.ID}">${producto.Nombre}</option>`).join('')}
                            </select>
                            <input type="number" class="form-control me-2" id="cantidad_${nuevaFilaId}" placeholder="Cantidad" style="width: 80px;" />
                            <button class="btn btn-success btn-sm" onclick="AgregarProducto('${nuevaFilaId}')">Agregar Producto</button>
                        </div>
                    </td>
                    <td id="total_${nuevaFilaId}">0</td>
                    <td>
                        <button class="btn btn-success btn-sm me-2" onclick="GuardarNuevaVenta('${nuevaFilaId}')">Guardar</button>
                        <button class="btn btn-secondary btn-sm" onclick="CancelarNuevaVenta('${nuevaFilaId}')">Cancelar</button>
                    </td>
                </tr>
            `;
            tabla.insertAdjacentHTML('beforeend', filaHTML);
        },
        error: function () {
            alert("Error al cargar los clientes disponibles.");
        }
    });
}

    function AgregarProducto(filaId) {
        const productoId = document.getElementById(`nuevo_producto_${filaId}`).value;
        const cantidad = parseInt(document.getElementById(`cantidad_${filaId}`).value, 10);
        const productoNombre = document.getElementById(`nuevo_producto_${filaId}`).selectedOptions[0].text;

        if (!productoId || isNaN(cantidad) || cantidad <= 0) {
            alert("Seleccione un producto y una cantidad válida.");
            return;
        }

        const productosDisponibles = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(productos));
        const producto = productosDisponibles.find(prod => prod.ID == productoId);

        const productosContainer = document.getElementById(`productos_${filaId}`);
        const totalField = document.getElementById(`total_${filaId}`);
        const totalActual = parseFloat(totalField.textContent) || 0;

        const subtotal = producto.Precio * cantidad;
        totalField.textContent = (totalActual + subtotal).toFixed(2);

        productosContainer.innerHTML += `
            <div id="producto_editar_${productoId}_${filaId}">
                <span>${productoNombre}</span>
                <input type="number" class="form-control me-2" value="${cantidad}" style="width: 80px;" disabled />
                <button class="btn btn-danger btn-sm" onclick="EliminarProducto('${filaId}', ${productoId}, ${subtotal})">❌</button>
            </div>
        `;
    }

    function EliminarProducto(filaId, productoId, subtotal) {
        const productoDiv = document.getElementById(`producto_editar_${productoId}_${filaId}`);
        productoDiv.remove();

        const totalField = document.getElementById(`total_${filaId}`);
        const totalActual = parseFloat(totalField.textContent) || 0;
        totalField.textContent = (totalActual - subtotal).toFixed(2);
    }

    function GuardarNuevaVenta(filaId) {
    const fila = document.getElementById(filaId);
    const clienteId = fila.querySelector(`#nuevo_cliente_${filaId}`).value;
    const productosDisponibles = @Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(productos));

    const productos = Array.from(fila.querySelectorAll(`#productos_${filaId} div`)).map(el => {
        const productoId = el.id.split('_')[2];
        const cantidad = parseInt(el.querySelector('input').value, 10);
        const producto = productosDisponibles.find(prod => prod.ID == productoId);
        const precioUnitario = producto ? producto.Precio : 0;
        const precioTotal = precioUnitario * cantidad;

        return {
            IDVenta: 0,
            IDProducto: parseInt(productoId, 10),
            Cantidad: cantidad,
            PrecioUnitario: precioUnitario,
            PrecioTotal: precioTotal,
        };
    });

    if (!clienteId) {
        alert("Seleccione un cliente.");
        return;
    }

    if (productos.length === 0) {
        alert("Agregue al menos un producto.");
        return;
    }
        let total = 0
        for (var i = 1; i < productos.length; i++) {
            total += productos[i].PrecioTotal
        }

    const nuevaVenta = {
        ID: 0,
        IDCliente: clienteId,
        Fecha: new Date().toISOString(),
        Total: total,
    };
    console.log(nuevaVenta, productos);

    $.ajax({
        type: 'POST',
        url: '/Venta/AgregarNuevaVenta',
        contentType: 'application/json',
        data: JSON.stringify({ nuevaVenta: nuevaVenta, ventaItems: productos }),
        success: function () {
            window.location.reload();
        },
        error: function () {
            alert("Error al guardar la nueva venta.");
        }
    });
}


    function CancelarNuevaVenta(filaId) {
        const fila = document.getElementById(filaId);
        fila.remove();
    }





</script>