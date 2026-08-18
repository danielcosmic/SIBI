-- Usuario administrador inicial
-- Generar el hash con: cd tools/HashGenerator && dotnet run "TuContrasena"
-- Pegar el resultado ($2a$11$...) en el campo Contrasena de abajo
-- EsContrasenaTemporal = 1 obliga a cambiarla en el primer inicio de sesión

IF NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'soporte.eic@ucr.ac.cr')
BEGIN
    INSERT INTO Usuario (Nombre, Correo, Contrasena, Permisos, EsContrasenaTemporal, IntentosFallidos, Activo)
    VALUES (
        'Administradora EIC',
        'soporte.eic@ucr.ac.cr',
        'REEMPLAZAR_CON_EL_HASH',
        'Administradora',
        1,
        0,
        1
    );
    PRINT 'Usuario administrador creado.';
END
ELSE
BEGIN
    PRINT 'Usuario administrador ya existe, omitiendo.';
END
