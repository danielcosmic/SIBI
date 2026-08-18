# Manual de Usuario — Rol Administradora

**Sistema SIBI — Inventario de Bienes Institucionales, EIC-UCR**

Este manual es para personas con rol **Administradora**: el nivel de acceso más alto del sistema. Si tu cuenta tiene este rol, tienes control total sobre activos, usuarios, categorías, encargados y las decisiones finales de aprobación/baja.

---

## 1. ¿Qué puedes hacer con este rol?

La Administradora es la única que puede:

- Gestionar **usuarios del sistema** (crear cuentas, bloquear/desbloquear, restablecer contraseñas, eliminar).
- Gestionar **categorías** de activos (crear, editar, eliminar).
- **Eliminar definitivamente** un activo que lleva un año o más en estado Desecho.
- **Aprobar o rechazar** una solicitud de baja (desecho) de un activo.
- **Aprobar o rechazar** las solicitudes de cambio pendientes.
- **Eliminar encargados** del sistema.

Además de todo lo anterior, también puedes: ver el dashboard, consultar y buscar el inventario completo, ver el detalle de cualquier activo, ver el historial/auditoría completo, crear y editar activos directamente, importar activos masivamente desde Excel, gestionar encargados (crear/editar/reasignar) y cambiar la placa de un activo.

En resumen: eres el rol que **no depende de nadie más** para tomar decisiones dentro del sistema, y eres tú quien resuelve las solicitudes y bloqueos de las demás personas usuarias.

---

## 2. Iniciar sesión y primer acceso

1. Entra a la URL del sistema y escribe tu correo institucional (`@ucr.ac.cr`) y tu contraseña.
2. Si tu cuenta es nueva o alguien te restableció la contraseña, el sistema te dará una **contraseña temporal**. Al iniciar sesión con ella, el sistema te obligará automáticamente a cambiarla antes de dejarte usar cualquier otra pantalla.
3. La nueva contraseña debe tener **mínimo 6 caracteres, al menos una mayúscula, una minúscula y un número**.
4. Si escribes tu contraseña incorrectamente **3 veces seguidas**, tu propia cuenta quedará bloqueada. Si esto te pasa a ti, necesitas que **otra cuenta con este mismo nivel de acceso** te desbloquee desde la sección de Usuarios (ver punto 5). Por eso es buena práctica que exista más de una cuenta con acceso total activa al mismo tiempo.
5. Si olvidaste tu contraseña, usa la opción "¿Olvidaste tu contraseña?" en la pantalla de inicio de sesión: el sistema genera una contraseña temporal para tu cuenta (actualmente esta contraseña temporal debe ser comunicada manualmente por quien administra el servidor, ya que el envío automático por correo aún no está activo — coordina con el equipo técnico si esto te ocurre a ti).

---

## 3. Dashboard

Al iniciar sesión llegas al Dashboard, con tarjetas resumen: total de activos, activos en desecho, cantidad de categorías y encargados. Cada tarjeta es un atajo: haz clic para ir directo a esa sección (por ejemplo, la tarjeta "Desecho" te lleva al listado de activos pendientes de decisión).

---

## 4. Gestión de usuarios del sistema

Esta pantalla (menú "Usuarios") es **exclusiva de tu rol**. Aquí:

- **Crear un usuario nuevo**: indicas nombre, correo (debe terminar en `@ucr.ac.cr`, si no el sistema lo rechaza) y el rol que le vas a asignar (Administradora, GTI, JefaAdministrativa o Invitado). El sistema genera una contraseña temporal de 8 caracteres que debes comunicarle a la persona por un canal seguro (Teams, en persona, etc.) — **no se envía por correo automáticamente**. La persona deberá cambiarla en su primer inicio de sesión.
- **Desbloquear una cuenta**: si alguien falló 3 veces su contraseña, aparece bloqueada. Usa la opción de desbloquear: se le genera una contraseña temporal nueva y se resetea el contador de intentos fallidos.
- **Restablecer la contraseña de otra persona**: útil si alguien perdió su contraseña y no quiere/puede usar la recuperación por correo. No puedes usar esta opción sobre tu propia cuenta (por seguridad, el sistema te lo bloquea); si necesitas restablecer la tuya, pídeselo a otra cuenta con este mismo nivel de acceso o usa "¿Olvidaste tu contraseña?" desde el login.
- **Editar** nombre, rol o estado (activo/inactivo) de una cuenta.
- **Eliminar** una cuenta. No puedes eliminar tu propia cuenta, ni la cuenta institucional de soporte (`soporte.eic@ucr.ac.cr`) — esa cuenta está protegida para asegurar que siempre exista al menos un acceso administrador al sistema, y tampoco se le puede quitar el rol Administradora.

**Recomendación de buenas prácticas**: no dejes solo un usuario con acceso total activo en el sistema. Si esa única cuenta se bloquea o pierde acceso, nadie podrá desbloquearla desde dentro del sistema y se necesitará intervención directa en la base de datos por parte del equipo técnico.

---

## 5. Gestión de categorías

Las categorías (por ejemplo: "Computadoras", "Mobiliario", "Equipo de laboratorio") clasifican los activos y también dan forma a las estadísticas del Dashboard. Solo tú puedes:

- Crear una categoría nueva (nombre + un ícono/emoji representativo).
- Editar el nombre o ícono de una categoría existente.
- Eliminar una categoría — **solo si no tiene activos asociados**. Si intentas borrar una con activos, el sistema te lo va a impedir; primero hay que reclasificar esos activos a otra categoría.

---

## 6. Inventario: crear, editar, importar y cambiar placa

Tienes acceso completo a la pantalla de Inventario:

- **Crear un activo nuevo**: placa, tipo de placa (Institucional o Interno), marca, modelo, número de serie, artículo, categoría, ubicación y encargado responsable. Puedes crear una categoría o un encargado nuevo desde el mismo formulario si no existe todavía.
- **Editar un activo**: puedes cambiar cualquier campo, incluyendo su estado (Activo / Mantenimiento / Desecho) y reasignar ubicación/encargado. Si cambias estado a Desecho, el sistema registra automáticamente la fecha; si lo devuelves a Activo, esa fecha se limpia.
- **Cambiar la placa** de un activo (por ejemplo, si se identificó un error de captura o se reetiquetó físicamente el bien). El sistema conserva todo el historial y las solicitudes asociadas al pasar a la nueva placa.
- **Importar activos masivamente desde Excel**: descarga la plantilla, complétala sin modificar los encabezados, y súbela. El sistema valida fila por fila y te muestra cuáles se importaron con éxito y cuáles fallaron (con el motivo exacto: categoría no encontrada, encargado ambiguo, placa duplicada, etc.).
- **Deshacer una creación reciente**: si acabas de crear un activo (o alguien más lo hizo) y notas un error grave, puedes eliminarlo directamente **solo dentro de los primeros 10 minutos** desde su creación. Pasado ese tiempo, ya no se puede borrar así — hay que editarlo o, si corresponde, pasarlo por el flujo de Desecho.

---

## 7. Desecho: aprobar o rechazar bajas definitivas

Esta es una de tus responsabilidades exclusivas. Cuando un activo pasa a estado "Desecho" (lo marca cualquier persona con permiso de edición), aparece en la pantalla de Desecho con el conteo de días que lleva ahí. Tú decides:

- **Rechazar la baja**: el activo vuelve a estado Activo, como si nunca se hubiera propuesto desechar.
- **Aprobar la eliminación definitiva**: **solo disponible cuando el activo lleva 365 días o más en Desecho**. Esta acción es irreversible: borra el activo y todo lo asociado a esa placa (historial, solicitudes de cambio) de forma permanente. Úsala con mucho cuidado — no hay manera de deshacerla desde el sistema.

Este período de espera de un año es una salvaguarda intencional del sistema para evitar bajas apresuradas.

---

## 8. Solicitudes de cambio: aprobar o rechazar

Cuando alguien sin permiso de edición directa sobre un activo quiere corregir o actualizar sus datos, lo hace mediante una **solicitud de cambio**, que queda pendiente hasta que se resuelve. Tú eres quien las resuelve:

1. En el menú "Solicitudes" ves todas las pendientes, con un comparativo de los valores actuales del activo contra los valores propuestos.
2. **Aprobar**: aplica automáticamente todos los cambios propuestos al activo real (incluyendo, si corresponde, un cambio de ubicación/encargado) y queda registrado en el historial.
3. **Rechazar**: puedes agregar un comentario explicando por qué (por ejemplo, para que la persona solicitante sepa qué corregir). El activo no cambia.

Recibirás una notificación en tiempo real (ícono de campana) cada vez que llegue una solicitud nueva, sin necesidad de recargar la página.

---

## 9. Encargados

Los encargados son las personas responsables de cada bien (no necesariamente tienen cuenta en el sistema). Puedes:

- Crear, editar y ver el listado de encargados con la cantidad de activos que tiene asignados cada uno.
- **Reasignar en bloque** varios activos de un encargado a otro (útil cuando alguien deja su puesto o cambia de área).
- **Eliminar** un encargado — solo si no tiene activos asignados actualmente. Si tiene, primero debes reasignarlos.

---

## 10. Historial / auditoría

En "Historial" puedes consultar **todos** los movimientos registrados en el sistema: creación de activos, cambios de ubicación, de encargado, de estado, de placa, aprobaciones y rechazos de solicitudes o bajas. Puedes filtrar por persona usuaria, placa de activo, tipo de acción y rango de fechas. Es tu herramienta principal para auditar qué pasó con un bien y quién hizo cada cambio.

---

## 11. Limitaciones a tener en cuenta

- No puedes eliminar tu propia cuenta de usuario, ni restablecer tu propia contraseña desde la sección de Usuarios (por seguridad).
- No puedes modificar el rol de la cuenta institucional `soporte.eic@ucr.ac.cr` ni eliminarla — está protegida para que el sistema nunca se quede sin una cuenta administradora funcional.
- La eliminación definitiva de un activo en Desecho requiere sí o sí que hayan pasado 365 días desde que se marcó como Desecho; el sistema no permite saltarse este plazo aunque seas Administradora.
- Las acciones de eliminación definitiva (activo, encargado con activos, categoría con activos) son irreversibles o están bloqueadas por el sistema hasta que se resuelvan sus dependencias — revisa bien antes de confirmar.
- El envío de contraseñas temporales por correo **no está activo todavía**; debes comunicarlas tú mismo/a por un medio seguro a la persona usuaria.

---

## 12. ¿A quién acudir?

Como Administradora, **tú eres el punto de contacto principal** para las demás personas usuarias del sistema (creación de cuentas, desbloqueos, restablecimiento de contraseñas, dudas sobre categorías o permisos).

Si el problema es **técnico** (el sistema no carga, error del servidor, la base de datos parece inconsistente, necesitas un cambio de configuración o de infraestructura), ese tipo de incidencias debe escalarse al equipo de desarrollo/soporte técnico del sistema, fuera del propio SIBI.
