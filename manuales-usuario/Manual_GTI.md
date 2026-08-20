# Manual de Usuario — Rol GTI

**Sistema SIBI — Inventario de Bienes Institucionales, EIC-UCR**

Este manual es para personas con rol **GTI** (Gestión de Tecnologías de Información). Este rol tiene permisos operativos amplios sobre el inventario: puedes crear, editar, importar y organizar activos y encargados, y resolver las solicitudes de cambio pendientes. No tienes acceso a la gestión de usuarios del sistema ni a la decisión final de eliminar activos o categorías.

---

## 1. ¿Qué puedes hacer con este rol?

- Ver el Dashboard, el inventario completo, el detalle de cualquier activo y el historial/auditoría.
- Crear activos, editarlos directamente (incluyendo cambiar su estado: Activo / Mantenimiento / Desecho) e importar activos masivamente (Excel o CSV).
- Exportar el inventario a Excel o PDF.
- Cambiar la placa de un activo.
- Enviar un activo a Desecho con un solo clic desde su vista rápida, sin pasar por el formulario completo de edición.
- Eliminar un activo recién creado, dentro de los primeros 10 minutos.
- Crear y editar encargados, y reasignar en bloque los activos de uno a otro.
- **Aprobar o rechazar** las solicitudes de cambio pendientes, incluidas las solicitudes de envío a Desecho.
- Recibir notificaciones en tiempo real (ícono de campana) cuando llega una solicitud nueva, y ver un contador de solicitudes pendientes en el menú.
- Ver (pero no crear/editar/eliminar) las categorías de activos; sí puedes hacer clic en una para ver sus activos.

## 2. Lo que NO puedes hacer

- No puedes crear, editar ni eliminar **categorías**. Si necesitas una categoría nueva o una corrección, contacta a un administrador.
- No puedes **eliminar** un encargado (sí puedes crearlo, editarlo y reasignarle o quitarle activos). Si un encargado ya no debería existir, reasigna primero todos sus activos y luego contacta a un administrador para que lo elimine.
- No puedes aprobar ni rechazar una **baja definitiva** (Desecho). Tú sí puedes enviar un activo a Desecho (directamente o mediante edición), pero la decisión final de eliminarlo permanentemente o restaurarlo requiere contactar a un administrador.
- No puedes eliminar un activo definitivamente (requiere que haya pasado al menos un año en Desecho, y contactar a un administrador). La opción de "eliminar recientes" solo te sirve durante los primeros 10 minutos tras crear un activo.
- No tienes acceso a la sección de **Usuarios** del sistema (crear cuentas, desbloquear, restablecer contraseñas de otras personas). Para gestiones sobre cuentas, contacta a un administrador.
- No puedes **crear** solicitudes de cambio; en tu rol, los cambios sobre activos existentes se aplican directamente mediante edición, sin pasar por ese flujo de aprobación.

---

## 3. Iniciar sesión y primer acceso

1. Ingresa con tu correo institucional (`@ucr.ac.cr`) y tu contraseña.
2. Si tu cuenta es nueva o te restablecieron la contraseña, recibirás una contraseña temporal (comunicada directamente por un administrador, no por correo automático). Al iniciar sesión con ella, el sistema te pedirá cambiarla (pantalla "Cambiar Contraseña") antes de dejarte usar cualquier otra pantalla — ahí solo escribes y confirmas la nueva.
3. La contraseña nueva debe cumplir 4 requisitos que la pantalla marca en verde a medida que los completas: mínimo 6 caracteres, una mayúscula, una minúscula y un número.
4. Si te equivocas de contraseña, el login te muestra "Credenciales incorrectas" con un contador ("Intento X de 3") y opciones para reintentar o cambiar de cuenta. Al tercer intento fallido tu cuenta queda bloqueada automáticamente. En ese caso necesitas que un administrador del sistema te la desbloquee — contacta a quien administra las cuentas en tu equipo, o escribe a **soporte.eic@ucr.ac.cr** si no sabes a quién recurrir.
5. El enlace "¿Olvidaste tu contraseña?" **no genera una clave nueva automáticamente**: te lleva a una pantalla que solo indica escribir a soporte.eic@ucr.ac.cr o contactar a un administrador para que te la restablezcan manualmente.
6. **Cambio de contraseña voluntario**: en cualquier momento (sin estar bloqueado ni con contraseña temporal) puedes abrir el menú de tu perfil, arriba a la derecha, y elegir "Cambiar contraseña". A diferencia del cambio forzado del primer ingreso, aquí el sistema sí te pide la contraseña actual, además de la nueva y su confirmación.

---

## 4. Dashboard

Verás tarjetas resumen: total de activos, activos en desecho, categorías y una tarjeta de **Encargados** (visible para tu rol) que te lleva directo a esa sección. Cada tarjeta es un atajo de navegación.

---

## 5. Inventario: tu trabajo principal

- **Crear un activo**: completa placa, tipo de placa, marca, modelo, número de serie, artículo, categoría, ubicación y encargado. Si el encargado o la categoría todavía no existen, puedes crearlos sin salir del formulario.
- **Editar un activo**: puedes modificar cualquier dato, incluido el estado. Si cambias el estado a "Desecho", el sistema anota la fecha automáticamente y el activo pasará a la cola de revisión para su baja definitiva.
- **Enviar a Desecho con un clic**: al abrir la vista rápida de un activo (que no esté ya en Desecho) verás un botón rojo "Enviar a Desecho" que aplica el cambio de inmediato tras confirmar, sin necesidad de abrir el formulario completo de edición.
- **Cambiar de ubicación o encargado**: el sistema conserva automáticamente cuál era la ubicación/encargado anterior, así que no necesitas anotarlo en ningún otro lado — queda visible en el detalle del activo y en el historial.
- **Cambiar la placa**: útil si se detecta un error de captura o el bien fue reetiquetado físicamente. Todo el historial y solicitudes de ese activo se conservan bajo la placa nueva.
- **Importar activos**: descarga la plantilla (no cambies los encabezados de las columnas), complétala y súbela — acepta archivos `.xlsx`, `.xls` o `.csv`. El sistema te dirá fila por fila si se importó correctamente o por qué falló (por ejemplo: "Categoría 'X' no encontrada" o "Encargado 'Y' coincide con varios encargados en el sistema" — en ese caso, corrige el nombre para que sea único o pide que se ajuste el encargado desde la pantalla correspondiente).
- **Exportar**: los botones "Excel" y "PDF" en la parte superior del inventario generan un archivo con los activos que cumplan los filtros de búsqueda que tengas activos en ese momento.
- **Deshacer una creación reciente**: si tú (o alguien más) cometió un error grave al crear un activo, puedes borrarlo directamente **solo si pasaron menos de 10 minutos** desde su creación (el botón muestra una cuenta regresiva). Pasado ese tiempo, corrígelo editándolo, o si ya no debe existir, pásalo a Desecho para que sea evaluado.

---

## 6. Desecho

Puedes **ver** la lista de activos en estado Desecho, cuántos días llevan ahí (con una barra de progreso hacia los 365 días) y quién los envió o solicitó (nota "Responsable del desecho" cuando esa información está disponible). También puedes exportar la lista a PDF con el botón "Imprimir PDF". Lo que **no puedes** es aprobar ni rechazar su eliminación definitiva. Si consideras que un activo en Desecho debería restaurarse o eliminarse, contacta a un administrador del sistema.

---

## 7. Solicitudes de cambio: tu rol como aprobador

Cuando alguien sin permiso de edición directa quiere proponer un cambio sobre un activo, lo hace mediante una **solicitud de cambio**, que tú puedes resolver:

1. En el menú "Solicitudes" verás todas las pendientes, con los valores actuales del activo junto a los valores propuestos. Las solicitudes que son específicamente para enviar un activo a Desecho se distinguen con un borde y una etiqueta roja, y muestran una tarjeta simplificada en lugar de la comparación campo a campo, ya que no proponen otros cambios.
2. **Aprobar**: aplica los cambios propuestos automáticamente al activo real. Si la solicitud implica cambio de ubicación o encargado, el sistema también actualiza eso y conserva el valor anterior en el historial. Si la solicitud es de envío a Desecho, el sistema fija correctamente la fecha de desecho al aprobarla, arrancando desde ahí el conteo de 365 días.
3. **Rechazar**: puedes escribir un comentario explicando el motivo, para que la persona solicitante sepa qué corregir y pueda volver a enviarla si corresponde.

Recibirás una notificación en tiempo real (ícono de campana) apenas llegue una solicitud nueva, sin necesidad de recargar la página, y verás un contador junto al ítem "Solicitudes" del menú lateral con la cantidad pendiente.

---

## 8. Encargados

- Puedes **crear** (botón "Nuevo Encargado") y **editar** encargados (nombre y rol/cargo).
- Puedes ver cuántos activos tiene asignados cada encargado y consultar el detalle de esos activos en un panel desplegable.
- Puedes **reasignar en bloque**: dentro del modal "Editar Encargado" hay una tabla de sus activos con casillas de selección; al marcar uno o varios aparece un selector "Reasignar a..." y un botón "Aplicar" para moverlos de una vez a otro encargado.
- **No puedes eliminar** un encargado del sistema — si un encargado ya no debería existir, primero reasigna todos sus activos y luego contacta a un administrador para que lo elimine.

---

## 9. Historial / auditoría

Puedes consultar el historial completo de movimientos del sistema, en formato de línea de tiempo. Filtros disponibles: **Tipo de Acción** (Creación, Cambio de Ubicación, Cambio de Encargado, Cambio de Estado, Cambio de Placa, Aprobación, Eliminación, Rechazo, Solicitud de Cambio, Solicitud Aprobada, Solicitud Rechazada), nombre de usuario, y rango de fechas. Al hacer clic en un registro se abre el detalle del activo relacionado. Es útil para verificar qué pasó con un activo antes de tomar una decisión sobre él.

---

## 10. Categorías

Puedes **ver** el listado de categorías y hacer clic sobre cualquiera para desplegar un panel con los activos que contiene (con su propio buscador), pero no puedes crear, editar ni eliminar ninguna. Si necesitas una categoría nueva o notas un error en una existente, contacta a un administrador para que la ajuste.

---

## 11. Limitaciones a tener en cuenta

- No tienes acceso a la sección de Usuarios del sistema.
- No puedes tomar la decisión final sobre categorías ni sobre eliminación de encargados o de activos en Desecho.
- La ventana para "deshacer" la creación de un activo es corta (10 minutos); pasado ese tiempo debes usar edición normal o el flujo de Desecho.
- No puedes crear solicitudes de cambio; los cambios sobre activos existentes se aplican directamente mediante edición.

---

## 12. Preguntas frecuentes y errores comunes

**"No veo el botón para aprobar la eliminación definitiva de un activo en Desecho."**
Es correcto: solo un administrador tiene esa opción. Tú puedes ver la lista de Desecho y exportarla, pero la aprobación/rechazo final no está en tu rol.

**"Al crear un activo me dice que la placa ya existe."**
Mensaje: *"Ya existe un activo con esa placa."* Verifica el número de placa; si el activo ya está registrado, edítalo en vez de crear uno nuevo, o usa "Cambiar placa" si lo que corresponde es corregir el número.

**"Al importar el Excel me sale 'Categoría no encontrada' o 'Encargado no encontrado'."**
El nombre escrito en la plantilla debe coincidir con una categoría o encargado que ya exista en el sistema. Corrige el nombre en el archivo y vuelve a subir esa fila, o crea primero la categoría/encargado que falta (si es un encargado, tú mismo puedes crearlo desde el formulario de un activo).

**"'El nombre coincide con varios encargados en el sistema.'"**
Ocurre cuando hay más de un encargado con el mismo nombre. En la plantilla de importación, especifica el encargado exacto seleccionándolo desde el sistema en vez de escribir solo el nombre, o pide que se ajusten los nombres duplicados para diferenciarlos.

**"Ya no puedo borrar un activo que creé por error."**
La opción de "eliminar reciente" solo funciona **dentro de los primeros 10 minutos** desde la creación (verás la cuenta regresiva en el botón mientras esté disponible). Pasado ese tiempo, corrígelo editándolo o, si ya no debe existir, pásalo a Desecho.

**"No me llegan notificaciones de solicitudes nuevas."**
Las notificaciones en tiempo real dependen de mantener la pestaña del navegador abierta con sesión activa; si cerraste sesión y volviste a entrar, la conexión se reinicia sola. Si el problema persiste, revisa el contador de solicitudes pendientes junto al menú "Solicitudes" (ese número se actualiza al navegar, independientemente de la notificación push) o refresca la página.

**"Cambié la placa de un activo y no encuentro su historial anterior."**
No se pierde: al cambiar la placa, el sistema traslada todo el historial y las solicitudes hacia la placa nueva. Busca el activo por la placa nueva en Inventario o Historial.

**"Quiero eliminar una categoría o un encargado y no me deja."**
Ninguna de las dos acciones está disponible para tu rol — contacta a un administrador. Si es un encargado con activos asignados, primero tendrías que reasignarlos (eso sí puedes hacerlo tú) antes de que un administrador lo elimine.

---

## 13. ¿A quién acudir?

- Para dudas sobre tu cuenta (contraseña bloqueada, cambio de rol, acceso), contacta a un administrador del sistema dentro de tu equipo.
- Para dudas sobre el uso del sistema, decisiones que no puedes tomar (categorías, bajas definitivas, eliminación de encargados) o cualquier problema técnico que encuentres, puedes escribir a **soporte.eic@ucr.ac.cr**.
