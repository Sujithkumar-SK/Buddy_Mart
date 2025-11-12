export interface CheckoutSummary {
  customerId: number;
  items: CheckoutItem[];
  totalItems: number;
  subTotal: number;
  shippingCharges: number;
  totalDiscount: number;
  taxAmount: number;
  grandTotal: number;
  availablePaymentMethods: string[];
}

export interface CheckoutItem {
  cartId: number;
  productId: number;
  productName: string;
  productSKU: string;
  price: number;
  discountPrice?: number;
  quantity: number;
  totalPrice: number;
  productImage?: string;
  vendorName: string;
  addedOn: string;
}

export interface CreateOrderRequest {
  fullName: string;
  phone: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  pinCode: string;
  landmark?: string;
  paymentMethod: string;
  orderNotes?: string;
}

export interface OrderDetails {
  orderId: number;
  customerId: number;
  customerName: string;
  customerPhone: string;
  customerEmail: string;
  orderDate: string;
  totalAmount: number;
  shippingAddress: string;
  paymentMethod: string;
  status: string;
  createdOn: string;
  items: OrderItem[];
}

export interface OrderSummary {
  orderId: number;
  customerId: number;
  customerName: string;
  orderDate: string;
  totalAmount: number;
  shippingAddress: string;
  paymentMethod: string;
  status: string;
  itemCount: number;
  createdOn: string;
  trackingSteps?: TrackingStep[];
}

export interface OrderItem {
  orderItemId: number;
  productId: number;
  productName: string;
  productSKU: string;
  productImage?: string;
  vendorName: string;
  quantity: number;
  unitPrice: number; // Original price
  totalPrice: number; // Amount paid (discounted price * quantity)
}

export interface TrackingStep {
  step: string;
  status: 'Completed' | 'In Progress' | 'Pending';
  date?: string;
}

export interface OrderTracking {
  orderId: number;
  status: string;
  orderDate: string;
  trackingSteps: TrackingStep[];
}