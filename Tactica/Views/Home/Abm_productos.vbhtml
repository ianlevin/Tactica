﻿@ModelType List(Of Producto)

@Code
    Dim productos = TryCast(ViewBag.Productos, List(Of Producto))
End Code

<h2>Lista de Productos</h2>
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
        const nombreProducto = document.getElementById('nombre_'+productoID);
        const precioProducto = document.getElementById('precio_'+productoID);
        const categoriaProducto = document.getElementById('categoria_'+productoID);

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
            url: '/Home/ActualizarProducto',
            data: { producto: nuevoProducto },
            success: function (response) {
                console.log("hola")
                resolve(response);
            },
            error: function (error) {
                console.log("hoala")
            }
        });

        nombreProducto.innerHTML = `${nuevoProducto.nombre}`;
        precioProducto.innerHTML = `${nuevoProducto.precio}`;
        categoriaProducto.innerHTML = `${nuevoProducto.categoria}`;

        const editarBoton = document.querySelector(`button[onclick="Editar(${productoID})"]`);
        editarBoton.textContent = "Editar";
        editarBoton.onclick = () => Editar(productoID);

    }

    function Eliminar(productoID) {
        $.ajax({
            type: 'POST',
            url: '/Home/EliminarProducto',
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
            url: '/Home/AgregarNuevoProducto',
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
                        <button type="submit" class="btn btn-danger" onclick="Eliminar(${productoID})">?</button>
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
</script>