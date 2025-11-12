export interface ShippingDetails {
  shippingId: number;
  orderId: number;
  trackingNumber?: string;
  courierService?: string;
  status: string;
  shippedDate?: string;
  estimatedDeliveryDate?: string;
  actualDeliveryDate?: string;
  deliveryNotes?: string;
  createdOn: string;
}

export interface UpdateTrackingDetails {
  shippingId: number;
  trackingNumber: string;
  courierService: string;
  estimatedDeliveryDate?: string;
  notes?: string;
}

export interface UpdateShippingStatus {
  status: number;
  notes?: string;
}

export interface VendorOrder {
  orderId: number;
  orderNumber: string;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  orderDate: string;
  totalAmount: number;
  shippingAddress: string;
  shippingCity?: string;
  shippingState?: string;
  shippingPinCode?: string;
  shippingPhone?: string;
  status: string;
  itemCount: number;
  items: VendorOrderItem[];
  shipping?: ShippingDetails;
}

export interface VendorOrderItem {
  orderItemId: number;
  productId: number;
  productName: string;
  productSKU: string;
  productImage?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}