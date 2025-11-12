using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Application.Services.Email;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Admin;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Admin;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IAnalyticsService _analyticsService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IAdminRepository adminRepository,
        IAnalyticsService analyticsService,
        IEmailService emailService,
        IMapper mapper,
        ILogger<AdminService> logger
    )
    {
        _adminRepository = adminRepository;
        _analyticsService = analyticsService;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<PendingVendorDto>>> GetPendingVendorsAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetPendingVendorsStarted);

            var result = await _adminRepository.GetPendingVendorsAsync();

            if (result.IsFailure)
            {
                _logger.LogError(
                    "Failed to get pending vendors: {Error}",
                    result.Error.Description
                );
                return Result.Failure<List<PendingVendorDto>>(result.Error);
            }

            var pendingVendorDtos = _mapper.Map<List<PendingVendorDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.GetPendingVendorsCompleted,
                pendingVendorDtos.Count
            );
            return pendingVendorDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.UnexpectedError);
            return Result.Failure<List<PendingVendorDto>>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<VendorApprovalDetailsDto>> GetVendorForApprovalAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.GetVendorForApprovalStarted, vendorId);

            var result = await _adminRepository.GetVendorForApprovalAsync(vendorId);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.GetVendorForApprovalFailed,
                    vendorId,
                    result.Error.Description
                );
                return Result.Failure<VendorApprovalDetailsDto>(result.Error);
            }

            var vendorDetailsDto = _mapper.Map<VendorApprovalDetailsDto>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.GetVendorForApprovalCompleted,
                vendorId
            );
            return vendorDetailsDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.GetVendorForApprovalFailed,
                vendorId,
                ex.Message
            );
            return Result.Failure<VendorApprovalDetailsDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> ApproveVendorAsync(
        VendorApprovalRequestDto request,
        string approvedBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.VendorApprovalStarted,
                request.VendorId
            );

            // Validate vendor exists and is in correct status
            var vendorResult = await _adminRepository.GetVendorForApprovalAsync(request.VendorId);
            if (vendorResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorApprovalFailed,
                    vendorResult.Error.Description
                );
                return Result.Failure(vendorResult.Error);
            }

            var vendor = vendorResult.Value;
            if (vendor.Status != VendorStatus.PendingApproval)
            {
                _logger.LogWarning("Invalid vendor status for approval: {Status}", vendor.Status);
                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InvalidVendorStatus,
                        MagicStrings.ErrorMessages.InvalidVendorStatus
                    )
                );
            }

            var result = await _adminRepository.ApproveVendorAsync(
                request.VendorId,
                approvedBy,
                request.ApprovalReason
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorApprovalFailed,
                    result.Error.Description
                );
                return Result.Failure(result.Error);
            }

            // Send approval email
            var emailResult = await _emailService.SendVendorApprovalEmailAsync(
                vendor.User.Email,
                vendor.BusinessName,
                request.ApprovalReason
            );

            if (emailResult.IsFailure)
            {
                _logger.LogWarning(
                    "Failed to send approval email to vendor {VendorId}: {Error}",
                    request.VendorId,
                    emailResult.Error.Description
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.VendorApprovalCompleted,
                request.VendorId
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorApprovalFailed, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> RejectVendorAsync(
        VendorRejectionRequestDto request,
        string rejectedBy
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.VendorRejectionStarted,
                request.VendorId
            );

            // Validate vendor exists and is in correct status
            var vendorResult = await _adminRepository.GetVendorForApprovalAsync(request.VendorId);
            if (vendorResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorRejectionFailed,
                    vendorResult.Error.Description
                );
                return Result.Failure(vendorResult.Error);
            }

            var vendor = vendorResult.Value;
            if (vendor.Status != VendorStatus.PendingApproval)
            {
                _logger.LogWarning("Invalid vendor status for rejection: {Status}", vendor.Status);
                return Result.Failure(
                    Error.Validation(
                        MagicStrings.ErrorCodes.InvalidVendorStatus,
                        MagicStrings.ErrorMessages.InvalidVendorStatus
                    )
                );
            }

            var result = await _adminRepository.RejectVendorAsync(
                request.VendorId,
                rejectedBy,
                request.RejectionReason
            );

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.VendorRejectionFailed,
                    result.Error.Description
                );
                return Result.Failure(result.Error);
            }

            // Send rejection email
            var emailResult = await _emailService.SendVendorRejectionEmailAsync(
                vendor.User.Email,
                vendor.BusinessName,
                request.RejectionReason
            );

            if (emailResult.IsFailure)
            {
                _logger.LogWarning(
                    "Failed to send rejection email to vendor {VendorId}: {Error}",
                    request.VendorId,
                    emailResult.Error.Description
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.VendorRejectionCompleted,
                request.VendorId
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.VendorRejectionFailed, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<DashboardAnalyticsDto>> GetDashboardAnalyticsAsync(
        AnalyticsFilterDto filter
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.AnalyticsRetrievalStarted, "Dashboard");

            var result = await _analyticsService.GetDashboardAnalyticsAsync(filter);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                    "Dashboard",
                    result.Error.Description
                );
                return Result.Failure<DashboardAnalyticsDto>(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalCompleted,
                "Dashboard"
            );
            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                "Dashboard",
                ex.Message
            );
            return Result.Failure<DashboardAnalyticsDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<SalesAnalyticsDto>> GetSalesAnalyticsAsync(AnalyticsFilterDto filter)
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalStarted,
                "SalesAnalytics"
            );

            var result = await _analyticsService.GetSalesAnalyticsAsync(filter);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                    "SalesAnalytics",
                    result.Error.Description
                );
                return Result.Failure<SalesAnalyticsDto>(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalCompleted,
                "SalesAnalytics"
            );
            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                "SalesAnalytics",
                ex.Message
            );
            return Result.Failure<SalesAnalyticsDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<CustomerAnalyticsDto>> GetCustomerAnalyticsAsync(
        AnalyticsFilterDto filter
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalStarted,
                "CustomerAnalytics"
            );

            var result = await _analyticsService.GetCustomerAnalyticsAsync(filter);

            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                    "CustomerAnalytics",
                    result.Error.Description
                );
                return Result.Failure<CustomerAnalyticsDto>(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.AnalyticsRetrievalCompleted,
                "CustomerAnalytics"
            );
            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.AnalyticsRetrievalFailed,
                "CustomerAnalytics",
                ex.Message
            );
            return Result.Failure<CustomerAnalyticsDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}
