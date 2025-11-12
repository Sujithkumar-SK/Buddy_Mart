# Drop and recreate with correct password
docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Sujith@!123" -C -d Vendor_Ecommerce -Q "DROP USER ecommerce_user;"

docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Sujith@!123" -C -Q "DROP LOGIN ecommerce_user; CREATE LOGIN ecommerce_user WITH PASSWORD = 'Sujith@!23';"

docker exec sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Sujith@!123" -C -d Vendor_Ecommerce -Q "CREATE USER ecommerce_user FOR LOGIN ecommerce_user; ALTER ROLE db_owner ADD MEMBER ecommerce_user;"

# Verify

