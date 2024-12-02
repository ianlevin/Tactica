@ModelType List(Of Cliente)

@Code
    Dim clientes = TryCast(ViewBag.Clientes, List(Of Cliente))
End Code


<h2>Lista de Clientes</h2>
<input type="text" id="buscar_cliente" class="form-control mb-3" placeholder="Buscar por nombre" onkeyup="BuscarClientes()" />
<button class="btn btn-success mb-3" onclick="NuevoCliente()">Agregar Cliente</button>
<Table Class="table table-bordered">
    <thead>
        <tr>
            <th> ID</th>
            <th> Cliente</th>
            <th> Teléfono</th>
            <th> Correo</th>
            <th> Acciones</th> 
        </tr>
    </thead>
    <tbody id="tabla_clientes">
        @For Each cliente As Cliente In clientes
            @<tr id="tr_@cliente.ID">
                <td id="@cliente.ID">@cliente.ID</td>
                <td id="nombre_@cliente.ID">@cliente.Cliente</td>
                <td id="telefono_@cliente.ID">@cliente.Telefono</td>
                <td id="correo_@cliente.ID">@cliente.Correo</td>
                <td>
                    <button type="submit" class="btn btn-danger" onclick="Eliminar(@cliente.ID)">❌</button>
                    <button class="btn btn-primary" onclick="Editar(@cliente.ID)">Editar</button>
                </td>
            </tr>
        Next
    </tbody>
</Table>
<script>
    function Editar(clienteID) {
        const nombreCliente = document.getElementById('nombre_'+clienteID);
        const telefonoCLiente = document.getElementById('telefono_'+clienteID);
        const correoCliente = document.getElementById('correo_'+clienteID);

        nombreCliente.innerHTML = `<input type="text" value="${nombreCliente.textContent}" id="nombre_${clienteID}" class="form-control">`;
        telefonoCLiente.innerHTML = `<input type="text" value="${telefonoCLiente.textContent}" id="telefono_${clienteID}" class="form-control">`;
        correoCliente.innerHTML = `<input type="text" value="${correoCliente.textContent}" id="correo_${clienteID}" class="form-control">`;

        const editarBoton = document.querySelector(`button[onclick="Editar(${clienteID})"]`);
        editarBoton.textContent = "Guardar";
        editarBoton.onclick = () => Guardar(clienteID);
}

    function Guardar(clienteID) {
        const nombreCliente = document.getElementById('nombre_' + clienteID);
        const telefonoCLiente = document.getElementById('telefono_' + clienteID);
        const correoCliente = document.getElementById('correo_' + clienteID);

        const nuevoCliente = {
            ID: clienteID,
            Cliente: nombreCliente.querySelector('input').value,
            Telefono: telefonoCLiente.querySelector('input').value,
            Correo: correoCliente.querySelector('input').value
        };

        $.ajax({
            type: 'POST',
            url: '/Cliente/ActualizarCliente',
            data: { cliente: nuevoCliente },
            success: function (response) {
                console.log("hola")
                resolve(response);
            },
            error: function (error) {
                console.log("hoala")
            }
        });

        nombreCliente.innerHTML = `${nuevoCliente.Cliente}`;
        telefonoCLiente.innerHTML = `${nuevoCliente.Telefono}`;
        correoCliente.innerHTML = `${nuevoCliente.Correo}`;

        const editarBoton = document.querySelector(`button[onclick="Editar(${clienteID})"]`);
        editarBoton.textContent = "Editar";
        editarBoton.onclick = () => Editar(clienteID);

    }

    function Eliminar(clienteID) {
        $.ajax({
            type: 'POST',
            url: '/Cliente/EliminarCliente',
            data: { clienteID: clienteID },
            success: function (response) {
                resolve(response);
            },
            error: function (error) {
                reject(error);
            }
        });
        const clienteRow = document.getElementById('tr_' + clienteID);
        clienteRow.remove();

    }

    function NuevoCliente() {
        if (document.getElementById('tr_nuevo') == null) {
            const nuevaFila = `
                <tr id="tr_nuevo">
                    <td>-</td>
                    <td>
                        <input type="text" id="nombre_nuevo" class="form-control" placeholder="Nombre del Cliente">
                    </td>
                    <td>
                        <input type="text" id="telefono_nuevo" class="form-control" placeholder="Teléfono">
                    </td>
                    <td>
                        <input type="email" id="correo_nuevo" class="form-control" placeholder="Correo Electrónico">
                    </td>
                    <td>
                        <button class="btn btn-primary" onclick="GuardarNuevoCliente()">Guardar</button>
                    </td>
                </tr>
            `;

            document.getElementById('tabla_clientes').insertAdjacentHTML('beforeend', nuevaFila);
        }
        
    }

    function GuardarNuevoCliente() {
        const nombre = document.getElementById('nombre_nuevo').value.trim();
        const telefono = document.getElementById('telefono_nuevo').value.trim();
        const correo = document.getElementById('correo_nuevo').value.trim();

        if (!nombre || !telefono || !correo) {
            alert('Por favor, completa todos los campos antes de guardar.');
            return;
        }

        const nuevoCliente = {
            Cliente: nombre,
            Telefono: telefono,
            Correo: correo
        };

        $.ajax({
            type: 'POST',
            dataType: 'JSON',
            url: '/Cliente/AgregarNuevoCliente', 
            data: { cliente: nuevoCliente },
            success: function (response) {
                
                const clienteID = response.ID;

                const nuevaFila = `
                <tr id="tr_${clienteID}">
                    <td id="${clienteID}">${clienteID}</td>
                    <td id="nombre_${clienteID}">${nuevoCliente.Cliente}</td>
                    <td id="telefono_${clienteID}">${nuevoCliente.Telefono}</td>
                    <td id="correo_${clienteID}">${nuevoCliente.Correo}</td>
                    <td>
                        <button type="submit" class="btn btn-danger" onclick="Eliminar(${clienteID})">❌</button>
                        <button class="btn btn-primary" onclick="Editar(${clienteID})">Editar</button>
                    </td>
                </tr>
            `;

                document.getElementById('tr_nuevo').outerHTML = nuevaFila;
            },
            error: function (error) {
                alert('Error al guardar el cliente: ' + error.responseText);
            }
        });
    }

    function BuscarClientes() {
        const busqueda = document.getElementById('buscar_cliente').value.trim();

        $.ajax({
            type: 'POST',
            url: '/Cliente/BuscarClientes',
            dataType: 'JSON',
            data: { busqueda: busqueda },
            success: function (response) {
                const tablaClientes = document.getElementById('tabla_clientes');
                tablaClientes.innerHTML = '';
                console.log(response)

                response.ID.forEach(function (cliente) {
                    const nuevaFila = `
                    <tr id="tr_${cliente.ID}">
                        <td id="${cliente.ID}">${cliente.ID}</td>
                        <td id="nombre_${cliente.ID}">${cliente.Cliente}</td>
                        <td id="telefono_${cliente.ID}">${cliente.Telefono}</td>
                        <td id="correo_${cliente.ID}">${cliente.Correo}</td>
                        <td>
                            <button type="submit" class="btn btn-danger" onclick="Eliminar(${cliente.ID})">❌</button>
                            <button class="btn btn-primary" onclick="Editar(${cliente.ID})">Editar</button>
                        </td>
                    </tr>
                `;
                    tablaClientes.insertAdjacentHTML('beforeend', nuevaFila);
                });
            },
            error: function (error) {
                console.error('Error al buscar clientes: ', error);
            }
        });
    }
</script>