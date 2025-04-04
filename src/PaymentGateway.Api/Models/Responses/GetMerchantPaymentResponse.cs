﻿using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Responses;

public class GetMerchantPaymentResponse
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public int CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; } = default!;
    public int Amount { get; set; }
}