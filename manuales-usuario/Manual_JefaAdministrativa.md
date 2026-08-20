# Manual de Usuario — Rol Jefa Administrativa

**Sistema SIBI — Inventario de Bienes Institucionales, EIC-UCR**

Este manual es para personas con rol **JefaAdministrativa**. Tu rol **no edita los activos existentes directamente**: en su lugar, **propones** cambios mediante solicitudes que deben ser revisadas y aprobadas antes de que se apliquen de verdad. Esto existe como control de calidad sobre la información del inventario.

---

## 1. ¿Qué puedes hacer con este rol?

- Ver el Dashboard, el inventario completo y el detalle de cada activo.
- Ver el historial/auditoría del sistema.
- Crear activos nuevos e importarlos masivamente (Excel o CSV).
- Exportar el inventario a Excel o PDF.
- Crear encargados nuevos (por ejemplo, al no encontrar uno existente al crear un activo).
- **Proponer cambios** sobre un activo existente (marca, modelo, número de serie, artículo, categoría, observaciones, ubicación, encargado o estado) mediante una **solicitud de cambio**, que queda pendiente hasta que sea revisada.
- **Solicitar el envío de un activo a Desecho** con un botón dedicado, independiente de la solicitud de cambio general.
- Ver el estado de tus propias solicitudes (pendientes, aprobadas o rechazadas) en la sección "Mis Solicitudes".
- Eliminar un activo que **tú acabas de crear**, dentro de los primeros 10 minutos (para corregir errores de captura sin esperar aprobación).
- Ver la lista de activos en Desecho (solo lectura).

## 2. Lo que NO puedes hacer

- **No puedes editar un activo directamente.** Cualquier cambio a un activo ya existente (excepto su creación) debe pasar por una solicitud de cambio.
- No puedes cambiar la placa de un activo.
- No puedes aprobar ni rechazar solicitudes de cambio (ni siquiera las tuyas); esa revisión la realiza otra persona con los permisos correspondientes.
- No puedes aprobar ni rechazar bajas de Desecho.
- No puedes gestionar categorías (crear/editar/eliminar). Si necesitas una categoría nueva, contacta a un administrador.
- No puedes editar, eliminar ni reasignar encargados — si necesitas corregir un encargado ya existente o reasignar activos en bloque, contacta a un administrador. (Sí puedes **crear** un encargado nuevo, ver punto 5).
- No tienes acceso a la sección de Usuarios del sistema.
- No recibes notificaciones push en tiempo real ni un contador automático de solicitudes: debes revisar "Mis Solicitudes" manualmente para ver si ya fueron resueltas.

---

## 3. Iniciar sesión y primer acceso

1. Ingresa con tu correo institucional (`@ucr.ac.cr`) y tu contraseña.
2. Si tu cuenta es nueva o alguien te restableció la contraseña, usarás una contraseña temporal que te habrán comunicado directamente (no llega por correo automático todavía). Al ingresar con ella, el sistema te va a obligar a cambiarla (pantalla "Cambiar Contraseña") antes de dejarte usar cualquier otra pantalla — solo debes escribir y confirmar la nueva.
3. La contraseña nueva debe cumplir 4 requisitos que la pantalla marca en verde a medida que los completas: mínimo 6 caracteres, una mayúscula, una minúscula y un número.
4. Si te equivocas de contraseña, el login muestra "Credenciales incorrectas" con un contador ("Intento X de 3"). Al tercer intento fallido tu cuenta se bloquea automáticamente. Deberás pedir que un administrador del sistema te la desbloquee.
5. El enlace "¿Olvidaste tu contraseña?" **no genera una clave nueva automáticamente**: es una pantalla informativa que indica escribir a soporte.eic@ucr.ac.cr o contactar a un administrador para que te restablezcan el acceso manualmente.
6. **Cambio de contraseña voluntario**: en cualquier momento (sin estar bloqueada ni con contraseña temporal) puedes abrir el menú de tu perfil, arriba a la derecha, y elegir "Cambiar contraseña". A diferencia del cambio forzado del primer ingreso, aquí el sistema sí te pide la contraseña actual, además de la nueva y su confirmación.

---

## 4. Dashboard

Verás tarjetas resumen (total de activos, en desecho, categorías) y una tarjeta especial de **"Cambios Solicitados"** que te lleva directo a tus solicitudes propias, para que puedas revisar rápido su estado.

---

## 5. Paso a paso: cómo ingresar activos (uno por uno o en bloque)

Esta es la única parte de tu rol donde sí trabajas de forma directa, sin pasar por aprobación: dar de alta activos nuevos que todavía no existen en el inventario. (Corregir un activo ya existente sí requiere una solicitud — ver punto 6).

### 5.1 Ingresar un activo individual

1. Ve a "Inventario" y haz clic en **"Nuevo Activo"** (arriba a la derecha).
2. Completa el formulario, dividido en cuatro bloques:
   - **Identificación**: Placa* (máx. 8 caracteres), Tipo de Placa* (Institucional o Interno), Artículo* (máx. 20), N° Serial* (máx. 30), Marca* (máx. 30), Modelo* (máx. 20).
   - **Clasificación**: Categoría* — búscala en el selector. Si no existe la que necesitas, no puedes crearla desde aquí: contacta a un administrador para que la agregue primero.
   - **Ubicación y Responsable**: Ubicación Actual* (texto libre, máx. 30) y Encargado* — búscalo en el selector, o crea uno nuevo con "Nuevo encargado" (nombre y cargo/rol) sin salir del formulario.
   - **Observaciones** (opcional, hasta 200 caracteres).
3. Haz clic en **"Confirmar y Crear"**: se abre una pantalla de revisión con todos los datos agrupados. Si algo está mal, usa "Editar" para volver al formulario; si todo está correcto, haz clic en **"Confirmar y Crear"** de nuevo para guardarlo definitivamente.
4. El activo queda creado con estado "Activo" y aparece de inmediato en el inventario. Si detectas un error justo después (por ejemplo, la placa mal escrita), puedes eliminarlo tú misma/o con el botón "Eliminar" en su vista rápida — solo funciona **dentro de los primeros 10 minutos** desde la creación (verás la cuenta regresiva en el propio botón). Pasado ese tiempo, el sistema ya no te deja borrarlo directamente: tendrás que enviar una solicitud de cambio para corregirlo (ver punto 6).

### 5.2 Ingresar activos en bloque (Excel o CSV)

Es un asistente de 3 o 4 pasos (el paso de "Entidades" solo aparece si hace falta resolver algo):

1. En "Inventario", haz clic en **"Importar Excel"**. En el **paso 1 ("Preparar archivo")** verás la lista de categorías y encargados que ya existen en el sistema (útil para escribir los nombres exactos en tu archivo) y el botón **"Descargar plantilla (.xlsx)"**. Complétala con una fila por activo, **sin modificar los encabezados**, respetando el orden de columnas: `Placa, Tipo Placa, Artículo, Marca, Modelo, N° Serial, Categoría, Ubicación Actual, Encargado, Observaciones`. El campo Tipo Placa debe ser `Institucional` o `Interno`/`Interna`. Luego arrastra el archivo a la zona de carga o haz clic para seleccionarlo (acepta `.xlsx`, `.xls` o `.csv`).
2. Haz clic en **"Previsualizar"** para pasar al **paso 2 ("Vista previa")**: una tabla te muestra cada fila con sus datos y, si hay un problema, el motivo en rojo. Las categorías o encargados que el sistema no reconoce se marcan con una etiqueta amarilla "Nuevo", y los nombres de encargado que coinciden con más de una persona registrada se marcan "Duplicado". Si hay errores de datos (campos vacíos, etc.), corrige el archivo original y vuelve a subirlo desde el paso 1.
3. Si todas las filas son válidas y no hay entidades nuevas ni ambiguas, el botón dice directamente **"Importar N activos"** — haz clic y listo. Si el sistema detectó categorías/encargados nuevos o nombres duplicados, verás en cambio **"Siguiente →"**, que te lleva al **paso 3 ("Entidades")**, donde:
   - Apruebas o descartas cada encargado nuevo (checkbox) indicando su cargo/rol — esto sí funciona para tu rol.
   - **Cuidado con las categorías nuevas**: el asistente te deja marcar la casilla para aprobar una categoría que no existe, pero la creación de categorías está reservada a un administrador — si apruebas una categoría nueva, esa parte falla en silencio y las filas que dependen de ella terminarán con error "Categoría no encontrada" en el paso de resultados. Si tu archivo usa una categoría que no existe todavía, pide a un administrador que la cree antes de importar, en vez de aprobarla desde aquí.
   - Para nombres parecidos a uno ya existente, eliges si es "la misma" (usar la existente) o si en realidad es una nueva.
   - Para nombres duplicados dentro del propio archivo (la misma categoría o persona escrita de formas distintas), eliges cuál nombre usar para todas esas filas.
   - Para encargados con nombre ambiguo en el sistema (varias personas con el mismo nombre), eliges cuál de ellas es la correcta para esas filas.
4. Haz clic en **"Confirmar e importar"**: llegas al **paso 4 ("Resultados")**, con el conteo de activos creados, placas que ya existían (omitidas) y filas con error (con el detalle de cada una). Los activos creados ya están disponibles en el inventario al cerrar el asistente.

También tienes los botones "Excel"/"PDF" en Inventario para exportar el listado filtrado.

---

## 6. Cómo proponer un cambio sobre un activo existente

Este es el flujo central de tu rol:

1. Ve al inventario, busca el activo (por placa, marca, modelo, etc.) y ábrelo (clic sobre la fila abre una vista rápida).
2. En la vista rápida verás dos botones específicos para tu rol: **"Solicitar Cambio"** (ámbar) para proponer modificaciones a sus datos, y **"Solicitar Desecho"** (rojo, ver más abajo) si lo que necesitas es enviarlo a Desecho. Ninguno de los dos aparece si el activo ya está en Desecho o si ya tienes una solicitud pendiente sobre él.
3. En "Solicitar Cambio" completas los campos que quieres modificar (marca, modelo, categoría, ubicación, encargado, estado, observaciones, etc.).
4. Al enviar la solicitud, queda en estado **Pendiente**. El sistema notifica automáticamente en tiempo real a quienes pueden revisarla (tú no recibes confirmación push, solo queda registrada).
5. El activo **no cambia todavía** — solo se actualizará si la solicitud es aprobada.
6. Si el activo ya tiene una solicitud tuya pendiente, el sistema te lo indicará al abrirlo, para evitar que envíes solicitudes duplicadas o contradictorias sobre el mismo bien.

### Solicitar el envío a Desecho

Si lo único que necesitas es que un activo pase a estado Desecho (sin proponer ningún otro cambio), usa el botón dedicado **"Solicitar Desecho"** en vez de "Solicitar Cambio". Genera una solicitud simplificada que solo contiene el cambio de estado; en "Mis Solicitudes" se muestra con una etiqueta y borde rojo distintivos, y con una tarjeta "Activo a desechar" en lugar de la comparación de campos. Sigue el mismo flujo de aprobación: el activo solo pasa a Desecho si quien la revisa la aprueba.

### Seguimiento: "Mis Solicitudes"

En el menú "Mis Solicitudes" puedes ver todas las que has enviado, con su estado:

- **Pendiente**: todavía no ha sido revisada.
- **Aprobada**: los cambios ya se aplicaron al activo real.
- **Rechazada**: no se aplicaron. Si quien la revisó dejó un comentario, lo verás ahí — úsalo para corregir y, si corresponde, enviar una nueva solicitud con los datos correctos.

Como no recibes notificaciones push, esta es la pantalla que debes revisar manualmente para saber si ya resolvieron tus solicitudes.

---

## 7. Consultar el inventario, Desecho y el historial

Puedes buscar y filtrar el inventario completo (por texto, categoría o estado) y ver el detalle completo de cualquier activo, incluyendo su ubicación actual/anterior y su encargado actual/anterior. También puedes ver la lista de activos en Desecho (con su barra de progreso hacia los 365 días y quién los envió, cuando esa información esté disponible), aunque no puedes actuar sobre ella. Tienes acceso al historial de auditoría completo del sistema, con filtros por persona usuaria, tipo de acción (Creación, Cambio de Ubicación, Cambio de Encargado, Cambio de Estado, Cambio de Placa, Aprobación, Eliminación, Rechazo, Solicitud de Cambio, Solicitud Aprobada, Solicitud Rechazada) y rango de fechas — útil para verificar si tu solicitud ya fue aplicada o para revisar el historial de un bien antes de proponer un cambio.

---

## 8. Limitaciones a tener en cuenta (resumen)

- Todo cambio sobre un activo **ya existente** pasa por un proceso de aprobación — planifica tiempo para eso, no es instantáneo.
- No puedes crear categorías nuevas (sí puedes crear encargados); si te falta una categoría para completar un formulario, pide que la creen antes de continuar.
- No tienes un menú "Encargados" propio para administrarlos en bloque — la creación de un encargado nuevo solo está disponible como paso rápido dentro del formulario de un activo.
- No puedes deshacer una solicitud ya enviada ni "cancelarla" tú misma/o una vez pendiente — si te equivocaste, contacta a quien la vaya a revisar para que la rechace, y luego envía una nueva correcta.
- La ventana para borrar un activo recién creado por ti es de solo 10 minutos.
- No hay aviso automático cuando resuelven tus solicitudes: revisa "Mis Solicitudes" periódicamente.

---

## 9. Preguntas frecuentes y errores comunes

**"No encuentro el botón para editar un activo."**
Es correcto: tu rol no tiene edición directa. En la vista rápida del activo busca los botones **"Solicitar Cambio"** o **"Solicitar Desecho"** — son tu forma de proponer modificaciones.

**"Envié una solicitud y no aparece el botón para enviar otra sobre el mismo activo."**
El sistema no permite tener dos solicitudes pendientes a la vez sobre el mismo activo, para evitar propuestas contradictorias. Espera a que la primera se resuelva (revisa "Mis Solicitudes") antes de enviar otra.

**"Mi solicitud lleva días en 'Pendiente' y no sé si alguien la va a revisar."**
No recibes ninguna notificación automática cuando la revisan: tienes que consultar "Mis Solicitudes" tú misma/o. Si es urgente, contacta directamente a un administrador para pedir que la revisen.

**"Mi solicitud fue rechazada, ¿qué hago?"**
Revisa el comentario que haya dejado quien la rechazó (aparece en "Mis Solicitudes", debajo de la solicitud resuelta) — normalmente explica qué corregir. Corrige los datos y envía una solicitud nueva; la rechazada no se puede reabrir ni editar.

**"Quiero crear un activo pero la categoría que necesito no aparece en la lista."**
No puedes crear categorías nuevas desde tu rol. Contacta a un administrador para que la agregue antes de continuar; el encargado, en cambio, sí puedes crearlo tú mismo/a desde el mismo formulario.

**"Al importar el Excel me sale 'Categoría no encontrada' o 'Encargado no encontrado'."**
El nombre escrito en la plantilla debe coincidir exactamente con una categoría o encargado ya existente. Si es una categoría que falta, pide que la creen; si es un encargado, corrige el nombre o créalo tú mismo/a antes de reintentar esa fila.

**"Ya no puedo borrar un activo que creé por error."**
Solo puedes eliminarlo directamente **dentro de los primeros 10 minutos** desde su creación (verás una cuenta regresiva en el botón). Pasado ese tiempo, tu única vía es enviar una solicitud de cambio para corregirlo.

**"No veo el botón 'Solicitar Cambio' en un activo."**
No aparece si el activo ya está en estado Desecho, o si ya tienes una solicitud pendiente sobre él.

---

## 10. ¿A quién acudir?

- Para que se apruebe o rechace una solicitud con urgencia, o para que se cree una categoría que necesitas, contacta directamente a un administrador del sistema.
- Para dudas sobre el uso del sistema, tu contraseña, o cualquier problema técnico, puedes escribir a **soporte.eic@ucr.ac.cr**.
