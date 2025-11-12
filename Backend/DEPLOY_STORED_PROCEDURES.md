# Deploy Stored Procedures to SQL Server Container

## Quick Deployment Commands

```bash
# 1. Copy the SQL file to container
docker cp ALL_STORED_PROCEDURES.sql sqlserver:/tmp/

# 2. Execute the script
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Sujith@!23" -i /tmp/ALL_STORED_PROCEDURES.sql

# 3. Verify procedures were created
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Sujith@!23" -Q "USE Vendor_Ecommerce; SELECT name FROM sys.procedures WHERE name LIKE 'sp_%';"
```

## Alternative: Execute from Host

```bash
# Execute SQL script directly from host
docker exec -i sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Sujith@!23" < ALL_STORED_PROCEDURES.sql
```

## Manual Execution

```bash
# Connect to SQL Server container
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Sujith@!23"

# Then run:
USE Vendor_Ecommerce;
GO

# Copy and paste the stored procedure definitions...
```

## Verify Deployment

```bash
# Check if procedures exist
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Sujith@!23" -Q "USE Vendor_Ecommerce; SELECT COUNT(*) as ProcedureCount FROM sys.procedures WHERE name LIKE 'sp_%';"
```