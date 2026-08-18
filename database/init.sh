#!/bin/bash
set -e

SQLCMD="/opt/mssql-tools18/bin/sqlcmd"

# Iniciar SQL Server en segundo plano
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo "[SIBI-DB] Esperando que SQL Server esté listo..."
RETRIES=30
until $SQLCMD -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" &>/dev/null 2>&1; do
    RETRIES=$((RETRIES - 1))
    if [ $RETRIES -le 0 ]; then
        echo "[SIBI-DB] ERROR: SQL Server no respondió en el tiempo límite."
        exit 1
    fi
    echo "[SIBI-DB] No está listo aún, reintentando... ($RETRIES intentos restantes)"
    sleep 2
done

echo "[SIBI-DB] SQL Server listo. Verificando si la base de datos ya existe..."

DB_EXISTS=$($SQLCMD -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C \
    -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.databases WHERE name = 'SIBI'" \
    -h -1 2>/dev/null | tr -d '[:space:]')

if [ "$DB_EXISTS" = "0" ]; then
    echo "[SIBI-DB] Base de datos no encontrada. Ejecutando esquema inicial..."
    $SQLCMD -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /docker-init/SIBI.sql
    echo "[SIBI-DB] Ejecutando migraciones..."
    $SQLCMD -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d SIBI -i /docker-init/migrate.sql
    echo "[SIBI-DB] Insertando usuario administrador inicial..."
    $SQLCMD -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -d SIBI -i /docker-init/seed.sql
    echo "[SIBI-DB] Base de datos inicializada correctamente."
else
    echo "[SIBI-DB] Base de datos SIBI ya existe. Omitiendo inicialización."
fi

# Mantener SQL Server en primer plano
wait $SQL_PID
