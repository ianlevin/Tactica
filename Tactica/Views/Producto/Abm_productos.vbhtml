@ModelType List(Of Producto)

@Code
    Dim productos = TryCast(ViewBag.Productos, List(Of Producto))
    Dim categorias = TryCast(ViewBag.Categorias, List(Of String))
End Code

<h2>Lista de Productos</h2>

<div class="mb-3">
    <input type="text" id="buscar_producto" class="form-control" placeholder="Buscar por nombre" onkeyup="BuscarProductos()" />
</div>

<div class="mb-3">
    <label for="categoria_filtro">Filtrar por categoría:</label>
    <select id="categoria_filtro" class="form-control" onchange="FiltrarPorCategoria()">
        <option value="">Seleccione una categoría</option>
        @For Each categoria In categorias
            @<option value = "@categoria" >@categoria</option>
        Next
    </select>
</div>

<div class="mb-3">
    <label for="precio_desde">Filtrar por precio:</label>
    <div class="d-flex">
        <input type="number" id="precio_desde" class="form-control me-2" placeholder="Desde" />
        <input type="number" id="precio_hasta" class="form-control me-2" placeholder="Hasta" />
        <button class="btn btn-primary" onclick="FiltrarPorPrecio()">Buscar</button>
    </div>
</div>
<button class="btn btn-success mb-3" onclick="NuevoProducto()">Agregar Producto</button>
<Table Class="table table-bordered">
    <thead>
        <tr>
            <th> ID</th>
            <th> Nombre</th>
            <th> Precio</th>
            <th> Categoria</th>
            <th> Acciones</th>
        </tr>
    </thead>
    <tbody id="tabla_productos">
        @For Each producto As Producto In productos
            @<tr id="tr_@producto.ID">
                <td id="@producto.ID">@producto.ID</td>
                <td id="nombre_@producto.ID">@producto.Nombre</td>
                <td id="precio_@producto.ID">@producto.Precio</td>
                <td id="categoria_@producto.ID">@producto.Categoria</td>
                <td>
                    <button type="submit" class="btn btn-danger" onclick="Eliminar(@producto.ID)">❌</button>
                    <button class="btn btn-primary" onclick="Editar(@producto.ID)">Editar</button>
                </td>
            </tr>
        Next
    </tbody>
</Table>

<script>
    function Editar(productoID) {
        const nombreProducto = document.getElementById('nombre_' + productoID);
        const precioProducto = document.getElementById('precio_' + productoID);
        const categoriaProducto = document.getElementById('categoria_' + productoID);

        nombreProducto.innerHTML = `<input type="text" value="${nombreProducto.textContent}" id="nombre_${productoID}" class="form-control">`;
        precioProducto.innerHTML = `<input type="text" value="${precioProducto.textContent}" id="precio_${productoID}" class="form-control">`;
        categoriaProducto.innerHTML = `<input type="text" value="${categoriaProducto.textContent}" id="categoria_${productoID}" class="form-control">`;

        const editarBoton = document.querySelector(`button[onclick="Editar(${productoID})"]`);
        editarBoton.textContent = "Guardar";
        editarBoton.onclick = () => Guardar(productoID);
    }

    function Guardar(productoID) {
        const nombreProducto = document.getElementById('nombre_' + productoID);
        const precioProducto = document.getElementById('precio_' + productoID);
        const categoriaProducto = document.getElementById('categoria_' + productoID);

        const nuevoProducto = {
            ID: productoID,
            nombre: nombreProducto.querySelector('input').value,
            precio: precioProducto.querySelector('input').value,
            categoria: categoriaProducto.querySelector('input').value
        };

        $.ajax({
            type: 'POST',
            url: '/Producto/ActualizarProducto',
            data: { producto: nuevoProducto },
            success: function (response) {
                nombreProducto.innerHTML = `${nuevoProducto.nombre}`;
                precioProducto.innerHTML = `${nuevoProducto.precio}`;
                categoriaProducto.innerHTML = `${nuevoProducto.categoria}`;

                const editarBoton = document.querySelector(`button[onclick="Editar(${productoID})"]`);
                editarBoton.textContent = "Editar";
                editarBoton.onclick = () => Editar(productoID);
            },
            error: function (error) {
                console.log("error")
            }
        });


    }

    function Eliminar(productoID) {
        $.ajax({
            type: 'POST',
            url: '/Producto/EliminarProducto',
            data: { productoID: productoID },
            success: function (response) {
                resolve(response);
            },
            error: function (error) {
                reject(error);
            }
        });
        const ProductoRow = document.getElementById('tr_' + productoID);
        ProductoRow.remove();

    }

    function NuevoProducto() {
        if (document.getElementById('tr_nuevo') == null) {
            const nuevaFila = `
                <tr id="tr_nuevo">
                    <td>-</td>
                    <td>
                        <input type="text" id="nombre_nuevo" class="form-control" placeholder="Nombre del Producto">
                    </td>
                    <td>
                        <input type="text" id="precio_nuevo" class="form-control" placeholder="Precio">
                    </td>
                    <td>
                        <input type="email" id="categoria_nuevo" class="form-control" placeholder="Categoria">
                    </td>
                    <td>
                        <button class="btn btn-primary" onclick="GuardarNuevoProducto()">Guardar</button>
                    </td>
                </tr>
            `;

            document.getElementById('tabla_productos').insertAdjacentHTML('beforeend', nuevaFila);
        }

    }

    function GuardarNuevoProducto() {
        const nombre = document.getElementById('nombre_nuevo').value.trim();
        const precio = document.getElementById('precio_nuevo').value.trim();
        const categoria = document.getElementById('categoria_nuevo').value.trim();

        if (!nombre || !precio || !categoria) {
            alert('Por favor, completa todos los campos antes de guardar.');
            return;
        }

        const nuevoProducto = {
            nombre: nombre,
            precio: precio,
            categoria: categoria
        };

        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Producto/AgregarNuevoProducto',
            data: { producto: nuevoProducto },
            success: function (response) {

                const productoID = response.ID;

                const nuevaFila = `
                <tr id="tr_${productoID}">
                    <td id="${productoID}">${productoID}</td>
                    <td id="nombre_${productoID}">${nuevoProducto.nombre}</td>
                    <td id="precio_${productoID}">${nuevoProducto.precio}</td>
                    <td id="categoria_${productoID}">${nuevoProducto.categoria}</td>
                    <td>
                        <button type="submit" class="btn btn-danger" onclick="Eliminar(${productoID})">❌</button>
                        <button class="btn btn-primary" onclick="Editar(${productoID})">Editar</button>
                    </td>
                </tr>
            `;

                document.getElementById('tr_nuevo').outerHTML = nuevaFila;
            },
            error: function (error) {
                alert('Error al guardar el Producto: ' + error.responseText);
            }
        });
    }

    function BuscarProductos() {
        const busqueda = document.getElementById('buscar_producto').value.trim();

        $.ajax({
            type: 'POST',
            url: '/Producto/BuscarProductos',
            dataType: 'JSON',
            data: { busqueda: busqueda },
            success: function (response) {
                const tablaProductos = document.getElementById('tabla_productos');
                tablaProductos.innerHTML = '';
                console.log(response)

                response.ID.forEach(function (producto) {
                    const nuevaFila = `
                <tr id="tr_${producto.ID}">
                    <td id="${producto.ID}">${producto.ID}</td>
                    <td id="nombre_${producto.ID}">${producto.Nombre}</td>
                    <td id="precio_${producto.ID}">${producto.Precio}</td>
                    <td id="categoria_${producto.ID}">${producto.Categoria}</td>
                    <td>
                        <button type="submit" class="btn btn-danger" onclick="Eliminar(${producto.ID})">❌</button>
                        <button class="btn btn-primary" onclick="Editar(${producto.ID})">Editar</button>
                    </td>
                </tr>
            `;
                    tablaProductos.insertAdjacentHTML('beforeend', nuevaFila);
                });
            },
            error: function (error) {
                console.error('Error al buscar productos: ', error);
            }
        });
    }

    function FiltrarPorPrecio() {
        const precioDesde = document.getElementById('precio_desde').value.trim();
        const precioHasta = document.getElementById('precio_hasta').value.trim();
        
        if (precioDesde === '' && precioHasta === '') {
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Producto/FiltrarPorPrecio', 
            dataType: 'JSON',
            data: {
                desde: precioDesde,
                hasta: precioHasta
            },
            success: function (response) {
                const tablaProductos = document.getElementById('tabla_productos');
                tablaProductos.innerHTML = ''; 

                response.ID.forEach(function (producto) {
                    const nuevaFila = `
                    <tr id="tr_${producto.ID}">
                        <td id="${producto.ID}">${producto.ID}</td>
                        <td id="nombre_${producto.ID}">${producto.Nombre}</td>
                        <td id="precio_${producto.ID}">${producto.Precio}</td>
                        <td id="categoria_${producto.ID}">${producto.Categoria}</td>
                        <td>
                            <button type="submit" class="btn btn-danger" onclick="Eliminar(${producto.ID})">❌</button>
                            <button class="btn btn-primary" onclick="Editar(${producto.ID})">Editar</button>
                        </td>
                    </tr>
                `;
                    tablaProductos.insertAdjacentHTML('beforeend', nuevaFila);
                });
            },
            error: function (error) {
                console.error('Error al filtrar productos por precio: ', error);
            }
        });
        document.getElementById('precio_desde').value = '';
        document.getElementById('precio_hasta').value = '';
    }

    function FiltrarPorCategoria() {
        const categoria = document.getElementById('categoria_filtro').value.trim();

        if (categoria === '') {
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/Producto/FiltrarPorCategoria', 
            dataType: 'JSON',
            data: { categoria: categoria },
            success: function (response) {
                const tablaProductos = document.getElementById('tabla_productos');
                tablaProductos.innerHTML = ''; 

                response.ID.forEach(function (producto) {
                    const nuevaFila = `
                    <tr id="tr_${producto.ID}">
                        <td id="${producto.ID}">${producto.ID}</td>
                        <td id="nombre_${producto.ID}">${producto.Nombre}</td>
                        <td id="precio_${producto.ID}">${producto.Precio}</td>
                        <td id="categoria_${producto.ID}">${producto.Categoria}</td>
                        <td>
                            <button type="submit" class="btn btn-danger" onclick="Eliminar(${producto.ID})">❌</button>
                            <button class="btn btn-primary" onclick="Editar(${producto.ID})">Editar</button>
                        </td>
                    </tr>
                `;
                    tablaProductos.insertAdjacentHTML('beforeend', nuevaFila);
                });
            },
            error: function (error) {
                console.error('Error al filtrar productos por categoría: ', error);
            }
        });
    }

</script>