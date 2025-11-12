using System.Net;
using System.Net.Mail;
using Kanini.Ecommerce.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kanini.Ecommerce.Application.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result> SendRegistrationOtpAsync(string email, string otp, string firstName)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = "Welcome to Kanini E-commerce - Verify Your Email";
            var body = GetRegistrationOtpTemplate(otp, firstName);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendForgotPasswordOtpAsync(string email, string otp)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = "Password Reset Request - Kanini E-commerce";
            var body = GetForgotPasswordOtpTemplate(otp);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendPasswordResetConfirmationAsync(string email, string firstName)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = "Password Reset Successful - Kanini E-commerce";
            var body = GetPasswordResetConfirmationTemplate(firstName);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendVendorApprovalEmailAsync(
        string email,
        string businessName,
        string approvalReason
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = "🎉 Vendor Application Approved - Kanini E-commerce";
            var body = GetVendorApprovalTemplate(businessName, approvalReason);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendVendorRejectionEmailAsync(
        string email,
        string businessName,
        string rejectionReason
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = "Vendor Application Update - Kanini E-commerce";
            var body = GetVendorRejectionTemplate(businessName, rejectionReason);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendOrderConfirmationEmailAsync(
        string email,
        string customerName,
        string orderNumber,
        decimal totalAmount,
        DateTime orderDate
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = $"Order Confirmation #{orderNumber} - Kanini E-commerce";
            var body = GetOrderConfirmationTemplate(
                customerName,
                orderNumber,
                totalAmount,
                orderDate
            );

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendPaymentSuccessEmailAsync(
        string email,
        string customerName,
        string orderNumber,
        decimal amount,
        string paymentMethod
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = $"Payment Confirmed for Order #{orderNumber} - Kanini E-commerce";
            var body = GetPaymentSuccessTemplate(customerName, orderNumber, amount, paymentMethod);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendOrderShippedEmailAsync(
        string email,
        string customerName,
        string orderNumber,
        string trackingNumber,
        string courierService,
        DateTime? estimatedDelivery
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = $"Order #{orderNumber} Shipped - Track Your Package";
            var body = GetOrderShippedTemplate(
                customerName,
                orderNumber,
                trackingNumber,
                courierService,
                estimatedDelivery
            );

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendOrderDeliveredEmailAsync(
        string email,
        string customerName,
        string orderNumber,
        DateTime deliveryDate
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = $"Order #{orderNumber} Delivered Successfully!";
            var body = GetOrderDeliveredTemplate(customerName, orderNumber, deliveryDate);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    public async Task<Result> SendOrderCancelledEmailAsync(
        string email,
        string customerName,
        string orderNumber,
        string reason
    )
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = $"Order #{orderNumber} Cancelled - Kanini E-commerce";
            var body = GetOrderCancelledTemplate(customerName, orderNumber, reason);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    private async Task<Result> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration[MagicStrings.ConfigKeys.SmtpHost];
            var smtpPort = int.Parse(_configuration[MagicStrings.ConfigKeys.SmtpPort] ?? "587");
            var smtpUsername = _configuration[MagicStrings.ConfigKeys.SmtpUsername];
            var smtpPassword = _configuration[MagicStrings.ConfigKeys.SmtpPassword];
            var fromEmail = _configuration[MagicStrings.ConfigKeys.FromEmail];
            var fromName = _configuration[MagicStrings.ConfigKeys.FromName];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP error while sending email to: {Email}", toEmail);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    private static string GetRegistrationOtpTemplate(string otp, string firstName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Welcome to Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>🎉 Welcome to Kanini E-commerce!</h1>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {firstName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Thank you for joining Kanini E-commerce! To complete your registration and secure your account, 
            please verify your email address using the verification code below:
        </p>
        
        <div style='background: white; border: 2px dashed #667eea; border-radius: 8px; padding: 25px; text-align: center; margin: 25px 0;'>
            <p style='margin: 0; font-size: 14px; color: #6c757d; margin-bottom: 10px;'>Your Verification Code</p>
            <h1 style='font-size: 36px; font-weight: bold; color: #667eea; margin: 0; letter-spacing: 5px; font-family: monospace;'>{otp}</h1>
            <p style='margin: 10px 0 0 0; font-size: 12px; color: #dc3545;'>⏰ Expires in 5 minutes</p>
        </div>
        
        <div style='background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 5px; padding: 15px; margin: 20px 0;'>
            <p style='margin: 0; font-size: 14px; color: #856404;'>
                <strong>🔒 Security Note:</strong> Never share this code with anyone. Our team will never ask for your verification code.
            </p>
        </div>
        
        <p style='font-size: 14px; color: #6c757d; margin-top: 30px;'>
            If you didn't create an account with us, please ignore this email or contact our support team.
        </p>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
            <p>Need help? Contact us at <a href='mailto:support@kaninieccommerce.com' style='color: #667eea;'>support@kaninieccommerce.com</a></p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetForgotPasswordOtpTemplate(string otp)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Password Reset - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>🔒 Password Reset Request</h1>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Security Verification Required</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            We received a request to reset your password for your Kanini E-commerce account. 
            To proceed with the password reset, please use the verification code below:
        </p>
        
        <div style='background: white; border: 2px dashed #ff6b6b; border-radius: 8px; padding: 25px; text-align: center; margin: 25px 0;'>
            <p style='margin: 0; font-size: 14px; color: #6c757d; margin-bottom: 10px;'>Your Security Code</p>
            <h1 style='font-size: 36px; font-weight: bold; color: #ff6b6b; margin: 0; letter-spacing: 5px; font-family: monospace;'>{otp}</h1>
            <p style='margin: 10px 0 0 0; font-size: 12px; color: #dc3545;'>⏰ Expires in 5 minutes</p>
        </div>
        
        <div style='background: #f8d7da; border: 1px solid #f5c6cb; border-radius: 5px; padding: 15px; margin: 20px 0;'>
            <p style='margin: 0; font-size: 14px; color: #721c24;'>
                <strong>⚠️ Important:</strong> If you didn't request a password reset, please ignore this email and consider changing your password for security.
            </p>
        </div>
        
        <div style='background: #d1ecf1; border: 1px solid #bee5eb; border-radius: 5px; padding: 15px; margin: 20px 0;'>
            <p style='margin: 0; font-size: 14px; color: #0c5460;'>
                <strong>💡 Security Tips:</strong>
            </p>
            <ul style='margin: 10px 0 0 0; font-size: 14px; color: #0c5460;'>
                <li>Use a strong, unique password</li>
                <li>Don't share your password with anyone</li>
                <li>Enable two-factor authentication when available</li>
            </ul>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
            <p>Need help? Contact us at <a href='mailto:support@kaninieccommerce.com' style='color: #ff6b6b;'>support@kaninieccommerce.com</a></p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetPasswordResetConfirmationTemplate(string firstName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Password Reset Successful - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #00b894 0%, #00a085 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>✅ Password Reset Successful</h1>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {firstName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Your password has been successfully reset for your Kanini E-commerce account. 
            You can now log in with your new password.
        </p>
        
        <div style='background: #d4edda; border: 1px solid #c3e6cb; border-radius: 5px; padding: 20px; margin: 25px 0; text-align: center;'>
            <h3 style='color: #155724; margin: 0 0 10px 0;'>🎉 All Set!</h3>
            <p style='margin: 0; font-size: 14px; color: #155724;'>
                Your account is secure and ready to use.
            </p>
        </div>
        
        <div style='background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 5px; padding: 15px; margin: 20px 0;'>
            <p style='margin: 0; font-size: 14px; color: #856404;'>
                <strong>🔒 Security Reminder:</strong> If you didn't reset your password, please contact our support team immediately.
            </p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #00b894; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                Login to Your Account
            </a>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
            <p>Need help? Contact us at <a href='mailto:support@kaninieccommerce.com' style='color: #00b894;'>support@kaninieccommerce.com</a></p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetVendorApprovalTemplate(string businessName, string approvalReason)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Vendor Application Approved - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #00b894 0%, #00a085 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>🎉 Congratulations!</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Your vendor application has been approved</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Welcome to Kanini E-commerce, {businessName}!</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            We are excited to inform you that your vendor application has been <strong style='color: #00b894;'>approved</strong>! 
            You can now start selling your products on our platform.
        </p>
        
        <div style='background: #d4edda; border: 1px solid #c3e6cb; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #155724; margin: 0 0 15px 0; font-size: 18px;'>✅ Approval Details</h3>
            <p style='margin: 0; font-size: 14px; color: #155724;'>
                <strong>Reason:</strong> {approvalReason}
            </p>
        </div>
        
        <div style='background: white; border: 1px solid #dee2e6; border-radius: 8px; padding: 25px; margin: 25px 0;'>
            <h3 style='color: #495057; margin: 0 0 15px 0;'>🚀 Next Steps</h3>
            <ol style='margin: 0; padding-left: 20px; color: #495057;'>
                <li style='margin-bottom: 10px;'>Log in to your vendor dashboard</li>
                <li style='margin-bottom: 10px;'>Complete your store profile setup</li>
                <li style='margin-bottom: 10px;'>Add your first products</li>
                <li style='margin-bottom: 10px;'>Start receiving orders!</li>
            </ol>
        </div>
        
        <div style='background: #e7f3ff; border: 1px solid #b8daff; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #004085; margin: 0 0 15px 0;'>📚 Resources for Success</h3>
            <ul style='margin: 0; padding-left: 20px; color: #004085; font-size: 14px;'>
                <li style='margin-bottom: 8px;'>Vendor Guidelines & Best Practices</li>
                <li style='margin-bottom: 8px;'>Product Photography Tips</li>
                <li style='margin-bottom: 8px;'>Order Management Tutorial</li>
                <li style='margin-bottom: 8px;'>Marketing & Promotion Tools</li>
            </ul>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #00b894; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-right: 10px;'>
                Access Vendor Dashboard
            </a>
            <a href='#' style='background: #6c757d; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                View Guidelines
            </a>
        </div>
        
        <div style='background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 5px; padding: 15px; margin: 20px 0;'>
            <p style='margin: 0; font-size: 14px; color: #856404;'>
                <strong>📞 Need Help?</strong> Our vendor support team is here to assist you. 
                Contact us at <a href='mailto:vendor-support@kaninieccommerce.com' style='color: #856404;'>vendor-support@kaninieccommerce.com</a>
            </p>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
            <p>This email was sent to you as a registered vendor applicant.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetVendorRejectionTemplate(string businessName, string rejectionReason)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Vendor Application Update - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #ff7675 0%, #e17055 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>Application Update</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Regarding your vendor application</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Dear {businessName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Thank you for your interest in becoming a vendor on Kanini E-commerce. 
            After careful review of your application, we regret to inform you that we cannot approve your vendor account at this time.
        </p>
        
        <div style='background: #f8d7da; border: 1px solid #f5c6cb; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #721c24; margin: 0 0 15px 0; font-size: 18px;'>📝 Reason for Decision</h3>
            <p style='margin: 0; font-size: 14px; color: #721c24;'>
                {rejectionReason}
            </p>
        </div>
        
        <div style='background: #e7f3ff; border: 1px solid #b8daff; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #004085; margin: 0 0 15px 0;'>🔄 What's Next?</h3>
            <p style='margin: 0 0 15px 0; font-size: 14px; color: #004085;'>
                Don't worry! You can reapply once you've addressed the concerns mentioned above. Here's what you can do:
            </p>
            <ul style='margin: 0; padding-left: 20px; color: #004085; font-size: 14px;'>
                <li style='margin-bottom: 8px;'>Review and update your business documentation</li>
                <li style='margin-bottom: 8px;'>Ensure all required information is complete and accurate</li>
                <li style='margin-bottom: 8px;'>Submit a new application when ready</li>
            </ul>
        </div>
        
        <div style='background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #856404; margin: 0 0 15px 0;'>📞 Need Clarification?</h3>
            <p style='margin: 0; font-size: 14px; color: #856404;'>
                If you have questions about this decision or need guidance on improving your application, 
                please don't hesitate to contact our vendor support team.
            </p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #6c757d; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-right: 10px;'>
                Contact Support
            </a>
            <a href='#' style='background: #007bff; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                Apply Again
            </a>
        </div>
        
        <div style='background: #d1ecf1; border: 1px solid #bee5eb; border-radius: 5px; padding: 15px; margin: 20px 0;'>
            <p style='margin: 0; font-size: 14px; color: #0c5460;'>
                <strong>💡 Tip:</strong> Make sure to read our vendor guidelines carefully before reapplying. 
                This will help ensure your application meets all our requirements.
            </p>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
            <p>Contact us: <a href='mailto:vendor-support@kaninieccommerce.com' style='color: #ff7675;'>vendor-support@kaninieccommerce.com</a></p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetOrderConfirmationTemplate(
        string customerName,
        string orderNumber,
        decimal totalAmount,
        DateTime orderDate
    )
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Order Confirmation - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>🎉 Order Confirmed!</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Thank you for your purchase</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {customerName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Great news! We've received your order and it's being processed. Here are your order details:
        </p>
        
        <div style='background: white; border: 1px solid #dee2e6; border-radius: 8px; padding: 25px; margin: 25px 0;'>
            <h3 style='color: #495057; margin: 0 0 15px 0;'>📦 Order Details</h3>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #6c757d;'>Order Number:</td>
                    <td style='padding: 8px 0; color: #495057;'>{orderNumber}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #6c757d;'>Order Date:</td>
                    <td style='padding: 8px 0; color: #495057;'>{orderDate:MMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; font-weight: bold; color: #6c757d;'>Total Amount:</td>
                    <td style='padding: 8px 0; color: #495057; font-weight: bold; font-size: 18px;'>₹{totalAmount:N2}</td>
                </tr>
            </table>
        </div>
        
        <div style='background: #e7f3ff; border: 1px solid #b8daff; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #004085; margin: 0 0 15px 0;'>🚀 What's Next?</h3>
            <ol style='margin: 0; padding-left: 20px; color: #004085;'>
                <li style='margin-bottom: 8px;'>We'll process your order within 24 hours</li>
                <li style='margin-bottom: 8px;'>You'll receive a shipping confirmation with tracking details</li>
                <li style='margin-bottom: 8px;'>Your order will be delivered within 3-5 business days</li>
            </ol>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #667eea; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                Track Your Order
            </a>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
            <p>Need help? Contact us at <a href='mailto:support@kaninieccommerce.com' style='color: #667eea;'>support@kaninieccommerce.com</a></p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetPaymentSuccessTemplate(
        string customerName,
        string orderNumber,
        decimal amount,
        string paymentMethod
    )
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Payment Confirmed - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #00b894 0%, #00a085 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>✅ Payment Confirmed</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Your payment was successful</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {customerName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Your payment has been successfully processed! Your order is now confirmed and will be prepared for shipping.
        </p>
        
        <div style='background: #d4edda; border: 1px solid #c3e6cb; border-radius: 8px; padding: 20px; margin: 25px 0; text-align: center;'>
            <h3 style='color: #155724; margin: 0 0 10px 0;'>💳 Payment Details</h3>
            <p style='margin: 5px 0; color: #155724;'><strong>Order:</strong> {orderNumber}</p>
            <p style='margin: 5px 0; color: #155724;'><strong>Amount:</strong> ₹{amount:N2}</p>
            <p style='margin: 5px 0; color: #155724;'><strong>Method:</strong> {paymentMethod}</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #00b894; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                View Order Details
            </a>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetOrderShippedTemplate(
        string customerName,
        string orderNumber,
        string trackingNumber,
        string courierService,
        DateTime? estimatedDelivery
    )
    {
        var deliveryText = estimatedDelivery.HasValue
            ? estimatedDelivery.Value.ToString("MMM dd, yyyy")
            : "3-5 business days";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Order Shipped - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #fd79a8 0%, #e84393 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>🚚 Order Shipped!</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Your package is on its way</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {customerName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Great news! Your order has been shipped and is on its way to you. Here are your tracking details:
        </p>
        
        <div style='background: white; border: 2px dashed #fd79a8; border-radius: 8px; padding: 25px; text-align: center; margin: 25px 0;'>
            <p style='margin: 0; font-size: 14px; color: #6c757d; margin-bottom: 10px;'>Tracking Number</p>
            <h2 style='font-size: 24px; font-weight: bold; color: #fd79a8; margin: 0; letter-spacing: 2px; font-family: monospace;'>{trackingNumber}</h2>
            <p style='margin: 10px 0 0 0; font-size: 14px; color: #6c757d;'>Courier: {courierService}</p>
        </div>
        
        <div style='background: #e7f3ff; border: 1px solid #b8daff; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #004085; margin: 0 0 15px 0;'>📅 Delivery Information</h3>
            <p style='margin: 0; color: #004085;'><strong>Order Number:</strong> {orderNumber}</p>
            <p style='margin: 5px 0 0 0; color: #004085;'><strong>Expected Delivery:</strong> {deliveryText}</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #fd79a8; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                Track Package
            </a>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetOrderDeliveredTemplate(
        string customerName,
        string orderNumber,
        DateTime deliveryDate
    )
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Order Delivered - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #00b894 0%, #00a085 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>🎉 Delivered!</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Your order has been delivered</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {customerName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            Wonderful! Your order has been successfully delivered. We hope you love your purchase!
        </p>
        
        <div style='background: #d4edda; border: 1px solid #c3e6cb; border-radius: 8px; padding: 20px; margin: 25px 0; text-align: center;'>
            <h3 style='color: #155724; margin: 0 0 10px 0;'>📦 Delivery Confirmed</h3>
            <p style='margin: 5px 0; color: #155724;'><strong>Order:</strong> {orderNumber}</p>
            <p style='margin: 5px 0; color: #155724;'><strong>Delivered on:</strong> {deliveryDate:MMM dd, yyyy}</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #00b894; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; margin-right: 10px;'>
                Rate & Review
            </a>
            <a href='#' style='background: #6c757d; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                Shop Again
            </a>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetOrderCancelledTemplate(
        string customerName,
        string orderNumber,
        string reason
    )
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>Order Cancelled - Kanini E-commerce</title>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: linear-gradient(135deg, #ff7675 0%, #e17055 100%); padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0; font-size: 28px;'>Order Cancelled</h1>
        <p style='color: white; margin: 10px 0 0 0; font-size: 16px;'>Your order has been cancelled</p>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2 style='color: #495057; margin-top: 0;'>Hello {customerName},</h2>
        
        <p style='font-size: 16px; margin-bottom: 25px;'>
            We're writing to inform you that your order has been cancelled. Here are the details:
        </p>
        
        <div style='background: #f8d7da; border: 1px solid #f5c6cb; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #721c24; margin: 0 0 15px 0;'>📋 Cancellation Details</h3>
            <p style='margin: 0; color: #721c24;'><strong>Order Number:</strong> {orderNumber}</p>
            <p style='margin: 10px 0 0 0; color: #721c24;'><strong>Reason:</strong> {reason}</p>
        </div>
        
        <div style='background: #e7f3ff; border: 1px solid #b8daff; border-radius: 8px; padding: 20px; margin: 25px 0;'>
            <h3 style='color: #004085; margin: 0 0 15px 0;'>💰 Refund Information</h3>
            <p style='margin: 0; color: #004085;'>If you made a payment, the refund will be processed within 3-5 business days to your original payment method.</p>
        </div>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='#' style='background: #007bff; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                Continue Shopping
            </a>
        </div>
        
        <hr style='border: none; border-top: 1px solid #e9ecef; margin: 30px 0;'>
        
        <div style='text-align: center; color: #6c757d; font-size: 12px;'>
            <p>© 2024 Kanini E-commerce. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
    }

    public async Task<Result> SendVendorProfilePendingApprovalAsync(string email, string businessName, string ownerName)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.EmailSendingStarted, email);

            var subject = "Vendor Profile Created - Pending Approval";
            var body = GetVendorProfilePendingApprovalTemplate(businessName, ownerName);

            var result = await SendEmailAsync(email, subject, body);

            if (result.IsSuccess)
            {
                _logger.LogInformation(MagicStrings.LogMessages.EmailSentSuccessfully, email);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, MagicStrings.LogMessages.EmailSendingFailed, email, ex.Message);
            return Result.Failure(
                Error.Unexpected(
                    MagicStrings.ErrorCodes.EmailSendingFailed,
                    MagicStrings.ErrorMessages.EmailSendingFailed
                )
            );
        }
    }

    private static string GetVendorProfilePendingApprovalTemplate(string businessName, string ownerName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Vendor Profile Created</title>
</head>
<body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background: #667eea; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;'>
        <h1 style='color: white; margin: 0;'>Profile Created!</h1>
    </div>
    
    <div style='background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; border: 1px solid #e9ecef;'>
        <h2>Hello {ownerName},</h2>
        
        <p>Thank you for creating your vendor profile for <strong>{businessName}</strong>! Your application is now under review.</p>
        
        <div style='background: #fff3cd; border: 1px solid #ffeaa7; border-radius: 8px; padding: 20px; margin: 20px 0;'>
            <h3>What's Next?</h3>
            <p>Our admin team will review your profile. You will receive an email notification once approved.</p>
        </div>
        
        
        <p style='text-align: center; color: #6c757d; font-size: 12px;'>© 2024 Kanini E-commerce. All rights reserved.</p>
    </div>
</body>
</html>";
    }
}
