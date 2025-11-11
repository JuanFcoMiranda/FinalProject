#!/bin/bash

# Script de entrada para SQL Server que ejecuta la inicialización de la base de datos
# Este script inicia SQL Server y luego ejecuta el script de inicialización

# Iniciar SQL Server en segundo plano
/opt/mssql/bin/sqlservr &

# Esperar a que SQL Server esté listo
echo "Esperando a que SQL Server inicie..."
sleep 30s

# Ejecutar el script de inicialización si existe
if [ -f /docker-entrypoint-initdb.d/init-db.sql ]; then
    echo "Ejecutando script de inicialización de base de datos..."
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -C -i /docker-entrypoint-initdb.d/init-db.sql
    
    if [ $? -eq 0 ]; then
        echo "Base de datos inicializada correctamente"
    else
        echo "Error al inicializar la base de datos"
    fi
fi

# Mantener el contenedor en ejecución
wait
