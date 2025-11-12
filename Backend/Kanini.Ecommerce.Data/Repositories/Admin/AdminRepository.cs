using System.Data;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.DatabaseContext;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DomainVendor = Kanini.Ecommerce.Domain.Entities.Vendor;

namespace Kanini.Ecommerce.Data.Repositories.Admin;

public class AdminRepository : IAdminRepository
{
    private readonly EcommerceDatabaseContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminRepository> _logger;
    private readonly string _connectionString;

    public AdminRepository(
        EcommerceDatabaseContext context,
        IConfiguration configuration,
        ILogger<AdminRepository> logger
    )
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
        _connectionString = _configuration.GetConnectionString("DatabaseConnectionString")!;
    }

    public async Task<Result<List<DomainVendor>>> GetPendingVendorsAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetPendingVendorsStarted);

            var vendors = new List<DomainVendor>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(MagicStrings.StoredProcedures.GetPendingVendors, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var vendor = new DomainVendor
                {
                    VendorId = reader.GetInt32("VendorId"),
                    BusinessName = reader.GetString("BusinessName"),
                    OwnerName = reader.GetString("OwnerName"),
                    BusinessLicenseNumber = reader.GetString("BusinessLicenseNumber"),
                    BusinessAddress = reader.GetString("BusinessAddress"),
                    City = reader.IsDBNull("City") ? null : reader.GetString("City"),
                    State = reader.IsDBNull("State") ? null : reader.GetString("State"),
                    PinCode = reader.IsDBNull("PinCode") ? null : reader.GetString("PinCode"),
                    TaxRegistrationNumber = reader.IsDBNull("TaxRegistrationNumber") ? null : reader.GetString("TaxRegistrationNumber"),
                    DocumentPath = reader.IsDBNull("DocumentPath") ? "/temp/pending" : reader.GetString("DocumentPath"),
                    Status = (VendorStatus)reader.GetInt32("Status"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    User = new User
                    {
                        Email = reader.GetString("Email"),
                        Phone = reader.GetString("Phone")
                    }
                };
                vendors.Add(vendor);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.GetPendingVendorsCompleted,
                vendors.Count
            );
            return vendors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<List<DomainVendor>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<DomainVendor>> GetVendorForApprovalAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Getting vendor for approval: {VendorId}", vendorId);

            var vendor = await _context.Vendors
                .Include(v => v.User)
                .FirstOrDefaultAsync(v => v.VendorId == vendorId && !v.IsDeleted);

            if (vendor == null)
            {
                return Result.Failure<DomainVendor>(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.VendorNotFound,
                        MagicStrings.ErrorMessages.VendorNotFound
                    )
                );
            }

            return vendor;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<DomainVendor>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }



    public async Task<Result> ApproveVendorAsync(
        int vendorId,
        string approvedBy,
        string approvalReason
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.VendorApprovalStarted, vendorId);

            var vendor = await _context.Vendors.FirstOrDefaultAsync(v =>
                v.VendorId == vendorId && !v.IsDeleted
            );

            if (vendor == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.VendorNotFound,
                        MagicStrings.ErrorMessages.VendorNotFound
                    )
                );
            }

            if (vendor.Status == VendorStatus.Active)
            {
                return Result.Failure(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.VendorAlreadyApproved,
                        MagicStrings.ErrorMessages.VendorAlreadyApproved
                    )
                );
            }

            if (vendor.Status == VendorStatus.Rejected)
            {
                return Result.Failure(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.VendorAlreadyRejected,
                        MagicStrings.ErrorMessages.VendorAlreadyRejected
                    )
                );
            }

            vendor.Status = VendorStatus.Active;
            vendor.IsActive = true;
            vendor.DocumentStatus = DocumentStatus.Verified;
            vendor.VerifiedBy = approvedBy;
            vendor.VerifiedOn = DateTime.UtcNow;
            vendor.UpdatedBy = approvedBy;
            vendor.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.VendorApprovalCompleted, vendorId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorApprovalFailed, ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result> RejectVendorAsync(
        int vendorId,
        string rejectedBy,
        string rejectionReason
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.VendorRejectionStarted, vendorId);

            var vendor = await _context.Vendors.FirstOrDefaultAsync(v =>
                v.VendorId == vendorId && !v.IsDeleted
            );

            if (vendor == null)
            {
                return Result.Failure(
                    Error.NotFound(
                        MagicStrings.ErrorCodes.VendorNotFound,
                        MagicStrings.ErrorMessages.VendorNotFound
                    )
                );
            }

            if (vendor.Status == VendorStatus.Active)
            {
                return Result.Failure(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.VendorAlreadyApproved,
                        MagicStrings.ErrorMessages.VendorAlreadyApproved
                    )
                );
            }

            if (vendor.Status == VendorStatus.Rejected)
            {
                return Result.Failure(
                    Error.Conflict(
                        MagicStrings.ErrorCodes.VendorAlreadyRejected,
                        MagicStrings.ErrorMessages.VendorAlreadyRejected
                    )
                );
            }

            vendor.Status = VendorStatus.Rejected;
            vendor.IsActive = false;
            vendor.DocumentStatus = DocumentStatus.Rejected;
            vendor.RejectionReason = rejectionReason;
            vendor.VerifiedBy = rejectedBy;
            vendor.VerifiedOn = DateTime.UtcNow;
            vendor.UpdatedBy = rejectedBy;
            vendor.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation(MagicStrings.LogMessages.VendorRejectionCompleted, vendorId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorRejectionFailed, ex.Message);
            return Result.Failure(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }
}
