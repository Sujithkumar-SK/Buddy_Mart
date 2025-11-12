export interface VendorProfile {
  vendorId: number;
  businessName: string;
  ownerName: string;
  businessLicenseNumber: string;
  businessAddress: string;
  city?: string;
  state?: string;
  pinCode?: string;
  taxRegistrationNumber?: string;
  documentPath?: string;
  status: string;
  currentPlan: string;
}

export interface VendorProfileUpdate {
  businessName: string;
  ownerName: string;
  businessLicenseNumber: string;
  businessAddress: string;
  city?: string;
  state?: string;
  pinCode?: string;
  taxRegistrationNumber?: string;
}

export interface VendorDashboard {
  analytics: VendorAnalytics;
  subscription: VendorSubscription;
}

export interface VendorAnalytics {
  totalProducts: number;
  activeProducts: number;
  lowStockProducts: number;
  totalOrders: number;
  pendingOrders: number;
  processingOrders: number;
  completedOrders: number;
  totalShipments: number;
  pendingShipments: number;
  shippedOrders: number;
  deliveredOrders: number;
  totalRevenue: number;
  monthlyRevenue: number;
  averageOrderValue: number;
}

export interface VendorSubscription {
  planName: string;
  maxProducts: number;
  usedProducts: number;
  remainingProducts: number;
  expiryDate?: string;
  daysRemaining: number;
  isActive: boolean;
}