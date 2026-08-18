FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

RUN mkdir -p /docker-init
COPY SIBI.sql /docker-init/SIBI.sql
COPY migrate_categoria_nvarchar.sql /docker-init/migrate.sql
COPY seed.sql /docker-init/seed.sql
COPY init.sh /docker-init/init.sh
RUN chmod +x /docker-init/init.sh

USER mssql

EXPOSE 1433
ENTRYPOINT ["/docker-init/init.sh"]
