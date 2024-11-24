// <copyright file="EmailSender.cs" company="ITANEO">
// Copyright (c) ITANEO (https://www.itaneo.com). All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Hexalith.DaprIdentityStore.UI.Services;

using System.Threading.Tasks;

using Hexalith.Application.Emails;
using Hexalith.DaprIdentityStore.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

/// <summary>
/// Service for sending emails related to user identity actions such as confirmation links and password resets.
/// Implements the <see cref="IEmailSender{CustomUser}"/> interface.
/// </summary>
public partial class EmailSender(IEmailService emailService, ILogger<EmailSender> logger) : IEmailSender<CustomUser>
{
    private readonly IEmailService _emailService = emailService;

    /// <inheritdoc/>
    public Task SendConfirmationLinkAsync(CustomUser user, string email, string confirmationLink)
    {
        string content = $"""
            <h1>Confirm your email</h1>
            <p>Hello {user?.UserName},</p>
            <p>Click the link below to confirm your email address:</p>
            <p><strong><a href='{confirmationLink}'>Confirm your email</a></strong></p>
            """;
        LogSendingConfirmationEmail(email, confirmationLink);
        return _emailService.SendAsync(email, "Confirm your email", null, content, CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task SendPasswordResetCodeAsync(CustomUser user, string email, string resetCode)
    {
        string content = $"""
            <h1>Reset your password</h1>
            <p>Hello {user?.UserName},</p>
            <p>Use the following code to reset your password:</p>
            <p><strong>{resetCode}</strong></p>
        """;
        LogSendingPasswordResetEmailWithCode(email, resetCode);
        return _emailService.SendAsync(email, "Reset your password", null, content, CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task SendPasswordResetLinkAsync(CustomUser user, string email, string resetLink)
    {
        string content = $"""
            <h1>Reset your password</h1>
            <p>Hello {user?.UserName},</p>
            <p>Click the link below to reset your password:</p>
            <p><strong><a href='{resetLink}'>Reset your password</a></strong></p>
        """;
        LogSendingPasswordResetEmailWithLink(email, resetLink);
        return _emailService.SendAsync(email, "Reset your password", null, content, CancellationToken.None);
    }

    /// <inheritdoc/>
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Sending confirmation email to {Email} with link {ConfirmationLink}")]
    private partial void LogSendingConfirmationEmail(string email, string confirmationLink);

    /// <inheritdoc/>
    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "Sending password reset email to {Email} with code {ResetCode}")]
    private partial void LogSendingPasswordResetEmailWithCode(string email, string resetCode);

    /// <inheritdoc/>
    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "Sending password reset email to {Email} with link {ResetLink}")]
    private partial void LogSendingPasswordResetEmailWithLink(string email, string resetLink);
}