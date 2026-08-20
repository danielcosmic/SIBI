-- Usuario administrador principal (soporte.eic@ucr.ac.cr)
-- Contraseña fija, solo modificable directamente en base de datos.
-- EsContrasenaTemporal = 0 — nunca se le pedirá cambiarla desde la app.

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'soporte.eic@ucr.ac.cr')
BEGIN
    INSERT INTO Usuario (Nombre, Correo, Contrasena, Permisos, EsContrasenaTemporal, IntentosFallidos, Activo)
    VALUES (
        'Administradora EIC',
        'soporte.eic@ucr.ac.cr',
        '$2a$11$1Y1hjw9k.4l32F8Qx2r6zuD/zBHJ.DoReOR7X9zcm1ZSR6XDbPOY2',
        'Administradora',
        0,
        0,
        1
    );
    PRINT 'Usuario administrador creado.';
END
ELSE
BEGIN
    PRINT 'Usuario administrador ya existe, omitiendo.';
END
