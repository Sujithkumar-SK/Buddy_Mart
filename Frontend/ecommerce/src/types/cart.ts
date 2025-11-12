export interface CartItem {
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
  stockQuantity: number;
  isActive: boolean;
  addedOn: string;
}

export interface CartSummary {
  customerId: number;
  items: CartItem[];
  totalItems: number;
  subTotal: number;
  totalDiscount: number;
  grandTotal: number;
  lastUpdated: string;
}

export interface AddToCartRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}