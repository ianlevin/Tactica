@ModelType List(Of Cliente)

@Code
    Dim clientes = TryCast(ViewBag.Clientes, List(Of Cliente))
End Code

<h2>Lista de Clientes</h2>

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
    <tbody>
        @For Each cliente As Cliente In clientes
            @<tr>
                <td id="@cliente.ID">@cliente.ID</td>
                <td id="@cliente.ID+cliente">@cliente.Cliente</td>
                <td id="@cliente.ID+telefono">@cliente.Telefono</td>
                <td id="@cliente.ID+correo">@cliente.Correo</td>
                <td>
                    <form action='@Url.Action("Eliminar")' method="post" style="display:inline;">
                        <input type="hidden" name="id" value="@cliente.ID" />
                        <button type="submit" class="btn btn-danger" onclick="return confirm('¿Está seguro de eliminar este cliente?');">❌</button>
                    </form>
                    <button class="btn btn-primary" onclick="Editar(@cliente.ID)">Editar</button>
                </td>
            </tr>
        Next
    </tbody>
</Table>
<script>
    function Editar(clienteID) {
    // Obtén la fila correspondiente al cliente
    const fila = document.querySelector(`#${clienteID}`).parentElement;

    // Obtén las celdas que se quieren editar
    const clienteCelda = fila.querySelector(`#${clienteID}+cliente`);
    const telefonoCelda = fila.querySelector(`#${clienteID}+telefono`);
    const correoCelda = fila.querySelector(`#${clienteID}+correo`);

    // Guarda los valores actuales en caso de cancelación
    const clienteOriginal = clienteCelda.textContent.trim();
    const telefonoOriginal = telefonoCelda.textContent.trim();
    const correoOriginal = correoCelda.textContent.trim();

    // Convierte las celdas en campos de entrada
    clienteCelda.innerHTML = `<input type="text" value="${clienteOriginal}" class="form-control" id="input-cliente-${clienteID}" />`;
    telefonoCelda.innerHTML = `<input type="text" value="${telefonoOriginal}" class="form-control" id="input-telefono-${clienteID}" />`;
    correoCelda.innerHTML = `<input type="text" value="${correoOriginal}" class="form-control" id="input-correo-${clienteID}" />`;

    // Cambia el botón "Editar" a un botón de confirmación (✅)
    const botonesCelda = fila.lastElementChild;
    botonesCelda.innerHTML = `
        <button class="btn btn-success" onclick="Guardar(${clienteID})">✅</button>
        <button class="btn btn-secondary" onclick="Cancelar(${clienteID}, '${clienteOriginal}', '${telefonoOriginal}', '${correoOriginal}')">❌</button>
    `;
}

function Guardar(clienteID) {
    // Obtén los valores de los campos de entrada
    const cliente = document.getElementById(`input-cliente-${clienteID}`).value.trim();
    const telefono = document.getElementById(`input-telefono-${clienteID}`).value.trim();
    const correo = document.getElementById(`input-correo-${clienteID}`).value.trim();

    // Envía los datos al controlador usando fetch
    fetch('/Clientes/Editar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            ID: clienteID,
            Cliente: cliente,
            Telefono: telefono,
            Correo: correo
        })
    })
    .then(response => {
        if (response.ok) {
            alert('Cliente actualizado con éxito');
            location.reload(); // Recarga la página para reflejar los cambios
        } else {
            alert('Error al actualizar el cliente');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        alert('Error al actualizar el cliente');
    });
}

function Cancelar(clienteID, clienteOriginal, telefonoOriginal, correoOriginal) {
    // Restaura los valores originales en las celdas
    const fila = document.querySelector(`#${clienteID}`).parentElement;

    fila.querySelector(`#${clienteID}+cliente`).textContent = clienteOriginal;
    fila.querySelector(`#${clienteID}+telefono`).textContent = telefonoOriginal;
    fila.querySelector(`#${clienteID}+correo`).textContent = correoOriginal;

    // Restaura el botón de "Editar"
    const botonesCelda = fila.lastElementChild;
    botonesCelda.innerHTML = `
        <form action='@Url.Action("Eliminar")' method="post" style="display:inline;">
            <input type="hidden" name="id" value="${clienteID}" />
            <button type="submit" class="btn btn-danger" onclick="return confirm('¿Está seguro de eliminar este cliente?');">❌</button>
        </form>
        <button class="btn btn-primary" onclick="Editar(${clienteID})">Editar</button>
    `;
}

</script>