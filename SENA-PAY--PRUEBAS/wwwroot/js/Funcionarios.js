// ════════════════════════════════════════════════════════════════
//  SENA-PAY · Panel Funcionario · JavaScript
//  Arquitectura: Fetch + JSON → FuncionariosController
// ════════════════════════════════════════════════════════════════

let usuarios = [];

// ── Sidebar ──────────────────────────────────────────────────
function openSidebar() {
    document.getElementById('sidebar').classList.add('open');
    document.getElementById('sidebarOverlay').classList.add('active');
}
function closeSidebar() {
    document.getElementById('sidebar').classList.remove('open');
    document.getElementById('sidebarOverlay').classList.remove('active');
}

function showSection(sec) {
    document.getElementById('section-usuarios').style.display = 'none';
    document.getElementById('sec-bloquear').style.display = 'none';
    if (sec === 'usuarios') document.getElementById('section-usuarios').style.display = 'block';
    else if (sec === 'sec-bloquear') document.getElementById('sec-bloquear').style.display = 'block';
    document.querySelectorAll('.nav-link-item').forEach(el => el.classList.remove('active'));
    event.currentTarget.classList.add('active');
    document.getElementById('breadcrumb-current').textContent =
        sec === 'usuarios' ? 'Gestión de Usuarios' :
            sec === 'sec-bloquear' ? 'Reportes / Bloquear Documento' : 'Configuración';
    closeSidebar();
}

// ── Utils ─────────────────────────────────────────────────────
function soloNumeros(e) { const c = e.which || e.keyCode; return (c >= 48 && c <= 57); }
function escHtml(str) {
    return String(str ?? '')
        .replace(/&/g, '&amp;').replace(/</g, '&lt;')
        .replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

// Obtiene el token antifalsificación del DOM
function getAntiforgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}

// ── Toast ─────────────────────────────────────────────────────
let toastTimer;
function showToast(type, title, msg) {
    const panel = document.getElementById('toastPanel');
    document.getElementById('toastTitle').textContent = title;
    document.getElementById('toastMsg').textContent = msg;
    document.getElementById('toastIcon').className = `toast-icon ${type}`;
    document.getElementById('toastIco').className = type === 'success'
        ? 'bi bi-check2-circle' : 'bi bi-x-circle';
    panel.classList.add('show');
    clearTimeout(toastTimer);
    toastTimer = setTimeout(hideToast, 4500);
}
function hideToast() { document.getElementById('toastPanel').classList.remove('show'); }

// ── Modal: abrir / cerrar ─────────────────────────────────────

/**
 * Abre el modal en modo AGREGAR.
 * Limpia todos los campos y restaura el estado visual inicial.
 */
function abrirModalAgregar() {
    limpiarModal();

    // Título e ícono en modo agregar
    document.getElementById('modalTitulo').textContent = 'Agregar Usuario';
    document.getElementById('modalSub').textContent = 'Completa los datos del nuevo usuario';
    document.getElementById('modalIcono').className = 'modal-icon';
    document.getElementById('textoGuardar').textContent = 'Guardar usuario';
    document.getElementById('btnGuardarModal').className = 'btn-modal-save';

    // Mostrar campos exclusivos de creación
    document.getElementById('modal-grupo-rol').style.display = '';
    document.getElementById('modal-grupo-documento').style.display = '';

    abrirModal();
}

/**
 * Abre el modal en modo EDITAR precargando los datos del usuario.
 * @param {number} id - IdUsuario a editar
 */
async function abrirModalEditar(id) {
    limpiarModal();

    // Título e ícono en modo editar
    document.getElementById('modalTitulo').textContent = 'Modificar Usuario';
    document.getElementById('modalSub').textContent = 'Actualiza los datos del usuario seleccionado';
    document.getElementById('modalIcono').className = 'modal-icon edit-mode';
    document.getElementById('textoGuardar').textContent = 'Guardar cambios';
    document.getElementById('btnGuardarModal').className = 'btn-modal-save edit-mode';

    // Ocultar campos que no se editan (rol y documento son inmutables)
    document.getElementById('modal-grupo-rol').style.display = 'none';
    document.getElementById('modal-grupo-documento').style.display = 'none';

    abrirModal();

    // Cargar datos desde el backend
    try {
        const res = await fetch(`/Funcionarios/ObtenerUsuario?id=${id}`);
        if (!res.ok) throw new Error('N o se pudo cargar el usuario.');
        const u = await res.json();

        document.getElementById('modal-idUsuario').value = u.idUsuario;
        document.getElementById('modal-nombre').value = u.nombre;
        document.getElementById('modal-correo').value = u.correo;
        document.getElementById('modal-telefono').value = u.telefono;
        document.getElementById('modal-saldo').value = u.saldo;
    } catch (err) {
        cerrarModal();
        showToast('error', 'Error', err.message ?? 'Problema de conexión.');
    }
}

function abrirModal() {
    document.getElementById('modalUsuario').classList.add('open');
    document.body.style.overflow = 'hidden'; // evita scroll detrás del modal
}

function cerrarModal() {
    document.getElementById('modalUsuario').classList.remove('open');
    document.body.style.overflow = '';
}

// Cierra el modal solo si el clic fue sobre el backdrop (no en el box)
function cerrarModalSiBackdrop(e) {
    if (e.target === document.getElementById('modalUsuario')) cerrarModal();
}

// Limpia todos los campos del modal y sus errores
function limpiarModal() {
    document.getElementById('modal-idUsuario').value = '';
    document.getElementById('modal-nombre').value = '';
    document.getElementById('modal-rol').value = '';
    document.getElementById('modal-correo').value = '';
    document.getElementById('modal-telefono').value = '';
    document.getElementById('modal-documento').value = '';
    document.getElementById('modal-saldo').value = '0';

    ['merr-nombre', 'merr-rol', 'merr-correo', 'merr-telefono', 'merr-documento', 'merr-saldo']
        .forEach(id => document.getElementById(id).style.display = 'none');
}

function modalLimpiarError(id) {
    document.getElementById(id).style.display = 'none';
}

// ── Modal: validación ─────────────────────────────────────────

/**
 * Valida los campos visibles del modal.
 * @param {boolean} esEdicion - true si es edición, false si es creación
 */
function validarModal(esEdicion) {
    const nombre = document.getElementById('modal-nombre').value.trim();
    const correo = document.getElementById('modal-correo').value.trim();
    const telefono = document.getElementById('modal-telefono').value.trim();
    const saldo = parseFloat(document.getElementById('modal-saldo').value);
    let ok = true;

    if (!nombre) { document.getElementById('merr-nombre').style.display = 'flex'; ok = false; }
    if (!correo || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(correo)) { document.getElementById('merr-correo').style.display = 'flex'; ok = false; }
    if (!telefono || telefono.length < 7) { document.getElementById('merr-telefono').style.display = 'flex'; ok = false; }
    if (isNaN(saldo) || saldo < 0) { document.getElementById('merr-saldo').style.display = 'flex'; ok = false; }

    if (!esEdicion) {
        const rol = document.getElementById('modal-rol').value;
        const doc = document.getElementById('modal-documento').value.trim();
        if (!rol) { document.getElementById('merr-rol').style.display = 'flex'; ok = false; }
        if (!doc || !/^\d+$/.test(doc)) { document.getElementById('merr-documento').style.display = 'flex'; ok = false; }
    }

    return ok;
}

// ── Modal: submit (crear o editar) ────────────────────────────

/**
 * Detecta si es creación o edición por la presencia de idUsuario,
 * construye el payload JSON y lo envía al endpoint correspondiente.
 */
async function submitModal() {
    const idUsuario = document.getElementById('modal-idUsuario').value;
    const esEdicion = idUsuario !== '';

    if (!validarModal(esEdicion)) return;

    // Estado del botón: cargando
    const btn = document.getElementById('btnGuardarModal');
    const spinner = document.getElementById('spinnerModal');
    const texto = document.getElementById('textoGuardar');
    btn.disabled = true;
    spinner.style.display = 'block';
    texto.textContent = 'Guardando...';

    // Construir payload JSON
    const payload = {
        idUsuario: esEdicion ? parseInt(idUsuario) : 0,
        nombre: document.getElementById('modal-nombre').value.trim(),
        correo: document.getElementById('modal-correo').value.trim(),
        telefono: document.getElementById('modal-telefono').value.trim(),
        saldo: parseFloat(document.getElementById('modal-saldo').value),
        // Solo se envían en creación; el controlador los ignora en edición
        documento: esEdicion ? 0 : parseInt(document.getElementById('modal-documento').value),
        idRol: esEdicion ? 0 : parseInt(document.getElementById('modal-rol').value)
    };

    // El endpoint se elige según el modo
    const url = esEdicion
        ? '/Funcionarios/EditarUsuarioJson'
        : '/Funcionarios/AgregarUsuarioJson';

    try {
        const res = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': getAntiforgeryToken()
            },
            body: JSON.stringify(payload)
        });

        const data = await res.json();

        if (data.ok) {
            cerrarModal();
            showToast('success', esEdicion ? '¡Actualizado!' : '¡Usuario creado!', data.msg);
            await cargarUsuariosIniciales(); // refresca la tabla sin recargar la página
        } else {
            showToast('error', 'Error', data.msg);
        }
    } catch {
        showToast('error', 'Error', 'Problema de conexión con el servidor.');
    } finally {
        btn.disabled = false;
        spinner.style.display = 'none';
        texto.textContent = esEdicion ? 'Guardar cambios' : 'Guardar usuario';
    }
}

// ── Carga inicial ─────────────────────────────────────────────

async function cargarUsuariosIniciales() {
    try {
        const res = await fetch('/Funcionarios/ObtenerUsuarios');
        const data = await res.json();
        if (Array.isArray(data)) { usuarios = data; renderTabla(); }
    } catch (err) {
        console.warn('[SenaPay] No se cargaron usuarios:', err.message);
        actualizarEstadisticas();
    }
}
// ── Render tabla ──────────────────────────────────────────────
function renderTabla() {
    const tbody = document.getElementById('tbodyUsuarios');
    const emptyRow = document.getElementById('emptyRow');

    tbody.querySelectorAll('tr:not(#emptyRow)').forEach(r => r.remove());

    if (usuarios.length === 0) {
        emptyRow.style.display = '';
        actualizarEstadisticas();
        return;
    }
    emptyRow.style.display = 'none';

    const badgeClass = { Aprendiz: 'badge-aprendiz', AdminTienda: 'badge-tienda', Funcionario: 'badge-funcionario'};
    const badgeIcon = { Aprendiz: 'bi-person-badge-fill', AdminTienda: 'bi-shop', Funcionario: 'bi-briefcase-fill'};

    usuarios.forEach(u => {

        // Badge de estado
        const estadoBadge = u.activo
            ? `<span style="
                display:inline-flex; align-items:center; gap:5px;
                background:rgba(34,197,94,.12); color:#22c55e;
                padding:4px 10px; border-radius:20px; font-size:.72rem; font-weight:700;">
                <i class="bi bi-circle-fill" style="font-size:.45rem;"></i> Activo
               </span>`
            : `<span style="
                display:inline-flex; align-items:center; gap:5px;
                background:rgba(100,116,139,.12); color:#94a3b8;
                padding:4px 10px; border-radius:20px; font-size:.72rem; font-weight:700;">
                <i class="bi bi-circle" style="font-size:.45rem;"></i> Inactivo
               </span>`;

        // Botón activar/desactivar
        const estadoBtn = u.activo
            ? `<button class="btn-action" style="margin-left:6px; background:rgba(234,179,8,.1);
               color:#ca8a04; border:1px solid rgba(234,179,8,.2);"
               onclick="cambiarEstado(${u.idUsuario}, false, '${escHtml(u.nombre)}')">
               <i class="bi bi-pause-circle-fill"></i> Desactivar
               </button>`
            : `<button class="btn-action" style="margin-left:6px; background:rgba(34,197,94,.1);
               color:#22c55e; border:1px solid rgba(34,197,94,.2);"
               onclick="cambiarEstado(${u.idUsuario}, true, '${escHtml(u.nombre)}')">
               <i class="bi bi-play-circle-fill"></i> Activar
               </button>`;

        const tr = document.createElement('tr');
        if (!u.activo) tr.style.opacity = '0.5';
        tr.dataset.id = u.idUsuario;
        // ✅ Reemplazar por esto — con columna Estado y sin Eliminar
        tr.innerHTML = `
              <td style="font-weight:600;">${escHtml(u.nombre)}</td>
              <td style="font-family:'Outfit',sans-serif;">${escHtml(u.documento)}</td>
              <td>
              <span class="role-badge ${badgeClass[u.rol] || ''}">
              <i class="bi ${badgeIcon[u.rol] || 'bi-person'}"></i>
              ${escHtml(u.rol)}
              </span>
              </td>
              <td style="font-family:'Outfit',sans-serif; font-weight:700;">
              $${Number(u.saldo).toLocaleString('es-CO')}
              </td>
              <td>${estadoBadge}</td>
              <td style="text-align:right;">
              <button class="btn-action btn-edit"
              onclick="abrirModalEditar(${u.idUsuario})">
              <i class="bi bi-pencil-fill"></i> Editar
              </button>
              ${estadoBtn}
              </td>`;
        tbody.appendChild(tr);
    });

    actualizarEstadisticas();
}

async function cambiarEstado(id, activar, nombre) {
    const accion = activar ? 'activar' : 'desactivar';
    const icono = activar ? 'bi-play-circle-fill' : 'bi-pause-circle-fill';
    const color = activar ? '#22c55e' : '#ca8a04';

    const result = await Swal.fire({
        title: `¿${activar ? 'Activar' : 'Desactivar'} usuario?`,
        html: `Estás a punto de <strong>${accion}</strong> a <strong>${escHtml(nombre)}</strong>.`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: `<i class="bi ${icono}"></i> Sí, ${accion}`,
        cancelButtonText: 'Cancelar',
        confirmButtonColor: color,
        background: '#13131f',
        color: '#e2e8f0',
        customClass: {
            popup: 'swal-popup-dark',
            confirmButton: 'swal-btn-confirm',
            cancelButton: 'swal-btn-cancel'
        },
        buttonsStyling: false
    });

    if (!result.isConfirmed) return;

    try {
        const body = new URLSearchParams({
            idUsuario: id,
            activar: activar,
            __RequestVerificationToken: getAntiforgeryToken()
        });

        const res = await fetch('/Funcionarios/CambiarEstadoUsuario', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: body.toString()
        });

        const data = await res.json();

        if (data.ok) {
            // Actualizar en memoria sin recargar toda la lista
            const u = usuarios.find(u => u.idUsuario === id);
            if (u) u.activo = activar;
            renderTabla();
            showToast('success', activar ? '¡Activado!' : '¡Desactivado!', data.msg);
        } else {
            showToast('error', 'Error', data.msg);
        }
    } catch {
        showToast('error', 'Error', 'Problema de conexión.');
    }
}

function actualizarEstadisticas() {
    const activos = usuarios.filter(u => u.activo);          // ← solo activos
    const total = activos.length;
    const aprend = activos.filter(u => u.rol === 'Aprendiz').length;
    const tienda = activos.filter(u => u.rol === 'AdminTienda').length;
    const saldo = activos.reduce((a, u) => a + (parseFloat(u.saldo) || 0), 0);

    document.getElementById('stat-total').textContent = total;
    document.getElementById('stat-aprendiz').textContent = aprend;
    document.getElementById('stat-tienda').textContent = tienda;
    document.getElementById('stat-saldo').textContent = '$' + saldo.toLocaleString('es-CO');
    document.getElementById('badge-usuarios').textContent = total;
}


// ── Sección Bloquear (sin cambios) ────────────────────────────
function clearBlkError(id) { const el = document.getElementById(id); if (el) el.style.display = 'none'; }

function goToConfirmStep() {
    const doc = document.getElementById('blk-doc').value.trim();
    const motivo = document.getElementById('blk-motivo').value;
    let ok = true;
    if (!doc) { document.getElementById('err-blk-doc').style.display = 'flex'; ok = false; }
    if (!motivo) { document.getElementById('err-blk-motivo').style.display = 'flex'; ok = false; }
    if (!ok) return;
    document.getElementById('blk-form-step').style.display = 'none';
    document.getElementById('blk-confirm-step').style.display = 'block';
    document.getElementById('blk-confirm-text').value = '';
    document.getElementById('blk-final-btn').disabled = true;
}

function checkConfirmWord() {
    document.getElementById('blk-final-btn').disabled =
        (document.getElementById('blk-confirm-text').value !== 'BLOQUEAR');
}

function executeBlock(e) {
    e.preventDefault();
    const btn = document.getElementById('blk-final-btn');
    const text = document.getElementById('blk-final-text');
    btn.disabled = true;
    text.textContent = 'Bloqueando...';
    setTimeout(() => {
        showToast('success', '¡Cuenta bloqueada!', 'Todos los pagos han sido suspendidos.');
        cancelBlock(e);
    }, 1200);
}

function cancelBlock(e) {
    e.preventDefault();
    document.getElementById('blk-form-step').style.display = 'block';
    document.getElementById('blk-confirm-step').style.display = 'none';
    document.getElementById('blk-doc').value = '';
    document.getElementById('blk-motivo').value = '';
    document.getElementById('blk-confirm-text').value = '';
    document.getElementById('blk-final-btn').disabled = true;
}

// ── Tecla Escape cierra el modal ──────────────────────────────
document.addEventListener('keydown', e => {
    if (e.key === 'Escape') cerrarModal();
});

// ── Init ──────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    actualizarEstadisticas();
    cargarUsuariosIniciales();
});


async function cargarUsuariosIniciales() {
    try {
        const res = await fetch('/Funcionarios/ObtenerUsuarios');
        const data = await res.json();

        // ← DIAGNÓSTICO TEMPORAL
        console.log('Primer usuario:', JSON.stringify(data[0]));
        console.log('activo (minúscula):', data[0]?.activo);
        console.log('Activo (mayúscula):', data[0]?.Activo);

        if (Array.isArray(data)) { usuarios = data; renderTabla(); }
    } catch (err) {
        console.warn('[SenaPay] No se cargaron usuarios:', err.message);
        actualizarEstadisticas();
    }
}