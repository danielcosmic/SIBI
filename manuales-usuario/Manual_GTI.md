# Manual de Usuario — Rol GTI

**Sistema SIBI — Inventario de Bienes Institucionales, EIC-UCR**

Este manual es para personas con rol **GTI** (Gestión de Tecnologías de Información). Este rol tiene permisos operativos amplios sobre el inventario: puedes crear, editar, importar y organizar activos y encargados, y resolver las solicitudes de cambio pendientes. No tienes acceso a la gestión de usuarios del sistema ni a la decisión final de eliminar activos o categorías.

---

## 1. ¿Qué puedes hacer con este rol?

- Ver el Dashboard, el inventario completo, el detalle de cualquier activo y el historial/auditoría.
- Crear activos, editarlos directamente (incluyendo cambiar su estado: Activo / Mantenimiento / Desecho) e importar activos masivamente desde Excel.
- Cambiar la placa de un activo.
- Eliminar un activo recién creado, dentro de los primeros 10 minutos.
- Crear y editar encargados, y reasignar activos de un encargado a otro.
- **Aprobar o rechazar** las solicitudes de cambio pendientes.
- Ver (pero no crear/editar/eliminar) las categorías de activos.

## 2. Lo que NO puedes hacer

- No puedes crear, editar ni eliminar **categorías**. Si necesitas una categoría nueva o una corrección, contacta a un administrador.
- No puedes **eliminar** un encargado (sí puedes crearlo, editarlo y reasignarle o quitarle activos). Si un encargado ya no debería existir, reasigna primero todos sus activos y luego contacta a un administrador para que lo elimine.
- No puedes aprobar ni rechazar una **baja definitiva** (Desecho). Tú sí puedes marcar un activo como Desecho al editarlo, pero la decisión final de eliminarlo o restaurarlo requiere contactar a un administrador.
- No puedes eliminar un activo definitivamente (requiere que haya pasado al menos un año en Desecho, y contactar a un administrador). La opción de "eliminar recientes" solo te sirve durante los primeros 10 minutos tras crear un activo.
- No tienes acceso a la sección de **Usuarios** del sistema (crear cuentas, desbloquear, restablecer contraseñas de otras personas). Para gestiones sobre cuentas, contacta a un administrador.
- No puedes **crear** solicitudes de cambio; en tu rol, los cambios sobre activos existentes se aplican directamente mediante edición, sin pasar por ese flujo de aprobación.

---

## 3. Iniciar sesión y primer acceso

1. Ingresa con tu correo institucional (`@ucr.ac.cr`) y tu contraseña.
2. Si tu cuenta es nueva o te restablecieron la contraseña, recibirás una contraseña temporal (comunicada directamente por un administrador, no por correo automático). Al iniciar sesión con ella, el sistema te pedirá cambiarla antes de dejarte usar cualquier otra pantalla.
3. La contraseña nueva debe tener mínimo 6 caracteres, con al menos una mayúscula, una minúscula y un número.
4. Si te equivocas de contraseña 3 veces seguidas, tu cuenta queda bloqueada automáticamente. En ese caso necesitas que un administrador del sistema te la desbloquee — contacta a quien administra las cuentas en tu equipo, o escribe a **soporte.eic@ucr.ac.cr** si no sabes a quién recurrir.
5. Si olvidaste tu contraseña, usa "¿Olvidaste tu contraseña?" en la pantalla de login.

---

## 4. Dashboard

Verás tarjetas resumen: total de activos, activos en desecho, categorías y una tarjeta de **Encargados** (visible para tu rol) que te lleva directo a esa sección. Cada tarjeta es un atajo de navegación.

---

## 5. Inventario: tu trabajo principal

- **Crear un activo**: completa placa, tipo de placa, marca, modelo, número de serie, artículo, categoría, ubicación y encargado. Si el encargado o la categoría todavía no existen, puedes crearlos sin salir del formulario.
- **Editar un activo**: puedes modificar cualquier dato, incluido el estado. Si cambias el estado a "Desecho", el sistema anota la fecha automáticamente y el activo pasará a la cola de revisión para su baja definitiva.
- **Cambiar de ubicación o encargado**: el sistema conserva automáticamente cuál era la ubicación/encargado anterior, así que no necesitas anotarlo en ningún otro lado — queda visible en el detalle del activo y en el historial.
- **Cambiar la placa**: útil si se detecta un error de captura o el bien fue reetiquetado físicamente. Todo el historial y solicitudes de ese activo se conservan bajo la placa nueva.
- **Importar desde Excel**: descarga la plantilla (no cambies los encabezados de las columnas), complétala y súbela. El sistema te dirá fila por fila si se importó correctamente o por qué falló (por ejemplo: "Categoría 'X' no encontrada" o "Encargado 'Y' coincide con varios encargados en el sistema" — en ese caso, corrige el nombre para que sea único o pide que se ajuste el encargado desde la pantalla correspondiente).
- **Deshacer una creación reciente**: si tú (o alguien más) cometió un error grave al crear un activo, puedes borrarlo directamente **solo si pasaron menos de 10 minutos** desde su creación. Pasado ese tiempo, corrígelo editándolo, o si ya no debe existir, pásalo a Desecho para que sea evaluado.

---

## 6. Desecho

Puedes **ver** la lista de activos en estado Desecho y cuántos días llevan ahí, pero **no puedes aprobar ni rechazar** su eliminación definitiva. Si consideras que un activo en Desecho debería restaurarse o eliminarse, contacta a un administrador del sistema.

---

## 7. Solicitudes de cambio: tu rol como aprobador

Cuando alguien sin permiso de edición directa quiere proponer un cambio sobre un activo, lo hace mediante una **solicitud de cambio**, que tú puedes resolver:

1. En el menú "Solicitudes" verás todas las pendientes, con los valores actuales del activo junto a los valores propuestos.
2. **Aprobar**: aplica los cambios propuestos automáticamente al activo real. Si la solicitud implica cambio de ubicación o encargado, el sistema también actualiza eso y conserva el valor anterior en el historial.
3. **Rechazar**: puedes escribir un comentario explicando el motivo, para que la persona solicitante sepa qué corregir y pueda volver a enviarla si corresponde.

Recibirás una notificación en tiempo real (icono de campana) apenas llegue una solicitud nueva, sin necesidad de recargar la página.

---

## 8. Encargados

- Puedes **crear** y **editar** encargados (nombre y rol/cargo).
- Puedes ver cuántos activos tiene asignados cada encargado y consultar el detalle de esos activos.
- Puedes **reasignar en bloque** varios activos de un encargado a otro (por ejemplo, cuando alguien deja su puesto).
- **No puedes eliminar** un encargado del sistema — si un encargado ya no debería existir, primero reasigna todos sus activos y luego contacta a un administrador para que lo elimine.

---

## 9. Historial / auditoría

Puedes consultar el historial completo de movimientos del sistema (creaciones, cambios de ubicación, encargado, estado, placa, aprobaciones/rechazos), filtrando por persona, placa, tipo de acción o rango de fechas. Es útil para verificar qué pasó con un activo antes de tomar una decisión sobre él.

---

## 10. Categorías

Puedes **ver** el listado de categorías (por ejemplo al elegir una para un activo nuevo), pero no puedes crear, editar ni eliminar ninguna. Si necesitas una categoría nueva o notas un error en una existente, contacta a un administrador para que la ajuste.

---

## 11. Limitaciones a tener en cuenta

- No tienes acceso a la sección de Usuarios del sistema.
- No puedes tomar la decisión final sobre categorías ni sobre eliminación de encargados o de activos en Desecho.
- La ventana para "deshacer" la creación de un activo es corta (10 minutos); pasado ese tiempo debes usar edición normal o el flujo de Desecho.
- No puedes crear solicitudes de cambio; los cambios sobre activos existentes se aplican directamente mediante edición.

---

## 12. ¿A quién acudir?

- Para dudas sobre tu cuenta (contraseña bloqueada, cambio de rol, acceso), contacta a un administrador del sistema dentro de tu equipo.
- Para dudas sobre el uso del sistema, decisiones que no puedes tomar (categorías, bajas definitivas, eliminación de encargados) o cualquier problema técnico que encuentres, puedes escribir a **soporte.eic@ucr.ac.cr**.
