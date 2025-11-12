using AutoMapper;
using Kanini.Ecommerce.Application.DTOs;
using Kanini.Ecommerce.Common;
using Kanini.Ecommerce.Data.Repositories.Customer;
using Kanini.Ecommerce.Domain.Entities;
using Kanini.Ecommerce.Domain.Enums;
using Microsoft.Extensions.Logging;
using DomainCustomer = Kanini.Ecommerce.Domain.Entities.Customer;

namespace Kanini.Ecommerce.Application.Services.Customer;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(
        ICustomerRepository customerRepository,
        IMapper mapper,
        ILogger<CustomerService> logger
    )
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
        _logger = logger;
    }



    public async Task<Result<CustomerProfileDto>> GetCustomerProfileAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(
                "Getting customer profile started for CustomerId: {CustomerId}",
                customerId
            );

            var customerResult = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customerResult.IsFailure)
            {
                _logger.LogError(
                    "Customer profile retrieval failed: {Error}",
                    customerResult.Error.Description
                );
                return Result.Failure<CustomerProfileDto>(customerResult.Error);
            }

            _logger.LogInformation(
                "Customer profile retrieved successfully for CustomerId: {CustomerId}",
                customerId
            );

            var customerProfileDto = _mapper.Map<CustomerProfileDto>(customerResult.Value);
            return customerProfileDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<CustomerProfileDto>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<CustomerProfileDto>> GetCustomerProfileByUserIdAsync(int userId)
    {
        try
        {
            var customerResult = await _customerRepository.GetCustomerByUserIdAsync(userId);
            if (customerResult.IsFailure)
                return Result.Failure<CustomerProfileDto>(customerResult.Error);

            var customerProfileDto = _mapper.Map<CustomerProfileDto>(customerResult.Value);
            return customerProfileDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<CustomerProfileDto>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<List<CustomerProfileDto>>> GetAllCustomersAsync()
    {
        try
        {
            _logger.LogInformation("Getting all customers started");

            var customersResult = await _customerRepository.GetAllCustomersAsync();
            if (customersResult.IsFailure)
                return Result.Failure<List<CustomerProfileDto>>(customersResult.Error);

            _logger.LogInformation(
                "Customers retrieved successfully. Found {Count} customers",
                customersResult.Value.Count
            );

            var customerDtos = _mapper.Map<List<CustomerProfileDto>>(customersResult.Value);
            return customerDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.ErrorMessages.DatabaseError);
            return Result.Failure<List<CustomerProfileDto>>(
                Error.Database(
                    MagicStrings.ErrorCodes.DatabaseError,
                    MagicStrings.ErrorMessages.DatabaseError
                )
            );
        }
    }

    public async Task<Result<CustomerProfileDto>> UpdateCustomerProfileAsync(
        int customerId,
        CustomerProfileUpdateDto request
    )
    {
        try
        {
            _logger.LogInformation(
                "Customer profile update started for CustomerId: {CustomerId}",
                customerId
            );

            var customerResult = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (customerResult.IsFailure)
            {
                _logger.LogError(
                    "Customer profile update failed: {Error}",
                    MagicStrings.ErrorMessages.CustomerNotFound
                );
                return Result.Failure<CustomerProfileDto>(customerResult.Error);
            }

            // Update customer profile
            var customer = customerResult.Value;
            customer.FirstName = request.FirstName;
            customer.MiddleName = request.MiddleName;
            customer.LastName = request.LastName;
            customer.DateOfBirth = request.DateOfBirth;
            customer.Gender = request.Gender.HasValue ? (Gender)request.Gender.Value : null;
            customer.Address = request.Address;
            customer.City = request.City;
            customer.State = request.State;
            customer.PinCode = request.PinCode;
            customer.UpdatedBy = "System";
            customer.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _customerRepository.UpdateCustomerAsync(customer);
            if (updateResult.IsFailure)
                return Result.Failure<CustomerProfileDto>(updateResult.Error);

            _logger.LogInformation(
                "Customer profile updated successfully for CustomerId: {CustomerId}",
                customerId
            );

            var updatedCustomerProfileDto = _mapper.Map<CustomerProfileDto>(customer);
            return updatedCustomerProfileDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer profile update failed: {Error}", ex.Message);
            return Result.Failure<CustomerProfileDto>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result> DeleteCustomerAsync(int customerId)
    {
        try
        {
            _logger.LogInformation(
                "Customer deletion started for CustomerId: {CustomerId}",
                customerId
            );

            var deleteResult = await _customerRepository.DeleteCustomerAsync(customerId);
            if (deleteResult.IsFailure)
                return Result.Failure(deleteResult.Error);

            _logger.LogInformation(
                "Customer deleted successfully for CustomerId: {CustomerId}",
                customerId
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Customer deletion failed: {Error}", ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<int>> GetCustomerIdByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Looking up customer for UserId: {UserId}", userId);
            
            var customerResult = await _customerRepository.GetCustomerByUserIdAsync(userId);
            if (customerResult.IsFailure)
            {
                _logger.LogWarning("No customer found for UserId: {UserId}", userId);
                return Result.Failure<int>(customerResult.Error);
            }

            _logger.LogInformation("Found CustomerId: {CustomerId} for UserId: {UserId}", customerResult.Value.CustomerId, userId);
            return customerResult.Value.CustomerId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get customer ID for UserId: {UserId}", userId);
            return Result.Failure<int>(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.UnexpectedError,
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

}
