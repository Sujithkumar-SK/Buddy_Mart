# Order Tracking Fix

## Issue
The order tracking steps were not reflecting the actual shipping status. For example, Order 22 had shipping status "Shipped" but the tracking still showed "Processing" as "In Progress".

## Root Cause
The `GetTrackingSteps` method in `OrderService.cs` was using hardcoded logic that only considered the order status, not the actual shipping status from the shipping table.

## Solution
Updated the tracking logic to:

1. **Include Shipping Information**: Modified `GetOrderTrackingAsync` to fetch shipping details from the shipping repository.

2. **Dynamic Status Calculation**: Updated tracking step logic to consider both order status and shipping status:
   - **Order Placed**: Always completed when order exists
   - **Order Confirmed**: Based on order status (Pending = Pending, others = Completed)
   - **Processing**: Completed when shipping is created and shipped/delivered
   - **Shipped**: Uses actual shipping status and shipped date
   - **Delivered**: Uses actual shipping status and delivery date

3. **Real Dates**: Uses actual dates from shipping records instead of hardcoded relative dates.

## Key Changes

### OrderService.cs
- Added `IShippingRepository` dependency injection
- Updated `GetOrderTrackingAsync` to fetch shipping information
- Replaced hardcoded `GetTrackingSteps` with dynamic logic that considers shipping status
- Added separate methods for each tracking step status calculation

### OrderDetailsResponseDto.cs
- Added shipping-related fields (OrderNumber, ShippingCity, ShippingState, etc.)
- Added Shipping property to include full shipping details

### OrderRepository.cs
- Updated to return additional shipping address fields from database

## Test Case
For Order 22 with shipping status "Shipped":
- **Before**: Processing showed "In Progress" even though item was shipped
- **After**: Processing shows "Completed", Shipped shows "Completed" with actual shipped date

## Verification
The tracking endpoint `/api/order/{id}/tracking` now properly reflects:
- Actual shipping status from the shipping table
- Real dates from shipping records
- Proper step progression based on shipping status