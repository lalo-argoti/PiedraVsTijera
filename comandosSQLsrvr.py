docker_SQLserver_crea="""sudo docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=PruebDeTalento!" \
 -p 1433:1433 --name sqlserver-container -d \
   mcr.microsoft.com/mssql/server:2022-latest"""   
   
docker_SQLserver_start="""sudo docker exec -it sqlserver-container /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U SA -P 'PruebDeTalento!'   """
server_cx="""ssh ubuntu@51.91.252.190"""
SQLserver_cx="""sqlcmd -S 51.91.252.190,1433 -U SA -P 'PruebDeTalento!'"""
comandos=[["USE miBaseDatos","GO"],["",""]]

SQLserver_tabla_campos="""SELECT 
    t.name AS Tabla,
    c.name AS Columna,
    ty.name AS TipoDato,
    c.max_length,
    c.is_nullable
FROM 
    sys.tables t
JOIN 
    sys.columns c ON t.object_id = c.object_id
JOIN 
    sys.types ty ON c.user_type_id = ty.user_type_id
WHERE 
    t.name = 'pdt_users_groups' """
    
SQLserver_tabla_campos=SQLserver_tabla_campos.replace("\n"," ")
#print(SQLserver_tabla_campos)
