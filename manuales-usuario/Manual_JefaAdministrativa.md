# Manual de Usuario — Rol Jefa Administrativa

**Sistema SIBI — Inventario de Bienes Institucionales, EIC-UCR**

Este manual es para personas con rol **JefaAdministrativa**. Tu rol **no edita los activos existentes directamente**: en su lugar, **propones** cambios mediante solicitudes que deben ser revisadas y aprobadas antes de que se apliquen de verdad. Esto existe como control de calidad sobre la información del inventario.

---

## 1. ¿Qué puedes hacer con este rol?

- Ver el Dashboard, el inventario completo y el detalle de cada activo.
- Ver el historial/auditoría del sistema.
- Crear activos nuevos e importarlos masivamente desde Excel.
- Crear encargados nuevos (por ejemplo, al no encontrar uno existente al crear un activo).
- **Proponer cambios** sobre un activo existente (marca, modelo, número de serie, artículo, categoría, observaciones, ubicación, encargado o estado) mediante una **solicitud de cambio**, que queda pendiente hasta que sea revisada.
- Ver el estado de tus propias solicitudes (pendientes, aprobadas o rechazadas) en la sección "Mis Solicitudes".
- Eliminar un activo que **tú acabas de crear**, dentro de los primeros 10 minutos (para corregir errores de captura sin esperar aprobación).

## 2. Lo que NO puedes hacer

- **No puedes editar un activo directamente.** Cualquier cambio a un activo ya existente (excepto su creación) debe pasar por una solicitud de cambio.
- No puedes cambiar la placa de un activo.
- No puedes aprobar ni rechazar solicitudes de cambio (ni siquiera las tuyas); esa revisión la realiza otra persona con los permisos correspondientes.
- No puedes aprobar ni rechazar bajas de Desecho.
- No puedes gestionar categorías (crear/editar/eliminar). Si necesitas una categoría nueva, contacta a un administrador.
- No puedes editar, eliminar ni reasignar encargados — si necesitas corregir un encargado ya existente o reasignar activos en bloque, contacta a un administrador. (Sí puedes **crear** un encargado nuevo, ver punto 5).
- No tienes acceso a la sección de Usuarios del sistema.

---

## 3. Iniciar sesión y primer acceso

1. Ingresa con tu correo institucional (`@ucr.ac.cr`) y tu contraseña.
2. Si tu cuenta es nueva o alguien te restableció la contraseña, usarás una contraseña temporal que te habrán comunicado directamente (no llega por correo automático todavía). Al ingresar con ella, el sistema te va a obligar a cambiarla antes de dejarte usar cualquier otra pantalla.
3. La contraseña nueva debe tener mínimo 6 caracteres, con al menos una mayúscula, una minúscula y un número.
4. Si te equivocas de contraseña 3 veces seguidas, tu cuenta se bloquea automáticamente. Deberás pedir que un administrador del sistema te la desbloquee.
5. Si olvidaste tu contraseña, usa la opción "¿Olvidaste tu contraseña?" en la pantalla de inicio de sesión.

---

## 4. Dashboard

Verás tarjetas resumen (total de activos, en desecho, categorías) y una tarjeta especial de **"Cambios Solicitados"** que te lleva directo a tus solicitudes propias, para que puedas revisar rápido su estado.

---

## 5. Crear un activo nuevo

Sí puedes dar de alta un activo nuevo en el inventario sin pasar por aprobación (la creación es directa): completa placa, tipo de placa, marca, modelo, número de serie, artículo, categoría, ubicación y encargado.

- Si el **encargado** que necesitas todavía no existe, puedes crearlo tú misma/o desde el mismo formulario (nombre y cargo/rol), sin salir del proceso de creación del activo.
- Si la **categoría** que necesitas todavía no existe, no puedes crearla tú: contacta a un administrador para que la cree primero.

Si detectas un error justo después de crear un activo (por ejemplo, escribiste mal la placa), puedes eliminarlo tú misma/o **dentro de los primeros 10 minutos**. Pasado ese tiempo, el sistema ya no te deja borrarlo directamente: tendrás que enviar una solicitud de cambio para corregirlo.

---

## 6. Cómo proponer un cambio sobre un activo existente

Este es el flujo central de tu rol:

1. Ve al inventario, busca el activo (por placa, marca, modelo, etc.) y ábrelo.
2. En vez de un botón de "Editar" que aplique cambios de inmediato, verás la opción de **solicitar un cambio**. Completa los campos que quieres modificar (marca, modelo, categoría, ubicación, encargado, estado, observaciones, etc.).
3. Al enviar la solicitud, queda en estado **Pendiente**. El sistema notifica automáticamente en tiempo real a quienes pueden revisarla.
4. El activo **no cambia todavía** — solo se actualizará si la solicitud es aprobada.
5. Si el activo ya tiene una solicitud tuya pendiente, el sistema te lo indicará al abrirlo, para evitar que envíes solicitudes duplicadas o contradictorias sobre el mismo bien.

### Seguimiento: "Mis Solicitudes"

En el menú "Mis Solicitudes" puedes ver todas las que has enviado, con su estado:

- **Pendiente**: todavía no ha sido revisada.
- **Aprobada**: los cambios ya se aplicaron al activo real.
- **Rechazada**: no se aplicaron. Si quien la revisó dejó un comentario, lo verás ahí — úsalo para corregir y, si corresponde, enviar una nueva solicitud con los datos correctos.

---

## 7. Consultar el inventario y el historial

Puedes buscar y filtrar el inventario completo (por texto, categoría o estado) y ver el detalle completo de cualquier activo, incluyendo su ubicación actual/anterior y su encargado actual/anterior. También tienes acceso al historial de auditoría completo del sistema, con filtros por persona usuaria, placa, tipo de acción y rango de fechas — útil para verificar si tu solicitud ya fue aplicada o para revisar el historial de un bien antes de proponer un cambio.

---

## 8. Importar activos desde Excel

Puedes usar la importación masiva de activos: descarga la plantilla, complétala sin tocar los encabezados, y súbela. El sistema valida cada fila y te indica cuáles se importaron con éxito y cuáles fallaron, con el motivo (por ejemplo, categoría o encargado no encontrado — en ese caso, contacta a un administrador para que existan antes de reintentar esa fila).

---

## 9. Limitaciones a tener en cuenta (resumen)

- Todo cambio sobre un activo **ya existente** pasa por un proceso de aprobación — planifica tiempo para eso, no es instantáneo.
- No puedes crear categorías nuevas (sí puedes crear encargados); si te falta una categoría para completar un formulario, pide que la creen antes de continuar.
- No tienes un menú "Encargados" propio para administrarlos en bloque — la creación de un encargado nuevo solo está disponible como paso rápido dentro del formulario de un activo.
- No puedes deshacer una solicitud ya enviada ni "cancelarla" tú misma/o una vez pendiente — si te equivocaste, contacta a quien la vaya a revisar para que la rechace, y luego envía una nueva correcta.
- La ventana para borrar un activo recién creado por ti es de solo 10 minutos.

---

## 10. ¿A quién acudir?

- Para que se apruebe o rechace una solicitud con urgencia, o para que se cree una categoría que necesitas, contacta directamente a un administrador del sistema.
- Para dudas sobre el uso del sistema, tu contraseña, o cualquier problema técnico, puedes escribir a **soporte.eic@ucr.ac.cr**.
