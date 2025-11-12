import axios from 'axios';

export interface PendingVendor {
  vendorId: number;
  businessName: string;
  ownerName: string;
  businessLicenseNumber: string;
  businessAddress: string;
  city: string;
  state: string;
  pinCode: string;
  taxRegistrationNumber?: string;
  documentPath?: string;
  email: string;
  phone: string;
  createdOn: string;
  status: string;
}

export interface VendorApprovalDetails {
  vendorId: number;
  userId: number;
  email: string;
  phone: string;
  registrationDate: string;
  isEmailVerified: boolean;
  businessName: string;
  ownerName: string;
  businessLicenseNumber: string;
  businessAddress: string;
  city?: string;
  state?: string;
  pinCode?: string;
  taxRegistrationNumber?: string;
  documentPath: string;
  documentStatus: string;
  verifiedOn?: string;
  verifiedBy?: string;
  rejectionReason?: string;
  status: string;
  currentPlan: string;
  isActive: boolean;
  createdOn: string;
  updatedOn?: string;
  createdBy: string;
  updatedBy?: string;
}

export interface DashboardAnalytics {
  totalRevenueToday: number;
  totalRevenueThisMonth: number;
  totalRevenueThisYear: number;
  revenueGrowthRate: number;
  totalOrdersToday: number;
  totalOrdersThisMonth: number;
  pendingOrders: number;
  processingOrders: number;
  completedOrders: number;
  totalCustomersToday: number;
  totalCustomersThisMonth: number;
  activeCustomers: number;
  totalVendors: number;
  activeVendors: number;
  pendingVendorApplications: number;
  totalProducts: number;
  lowStockProductsCount: number;
  outOfStockProductsCount: number;
  averageOrderValue: number;
  conversionRate: number;
  recentActivities: RecentActivity[];
  topSellingProducts: TopSellingProduct[];
}

export interface RecentActivity {
  activityType: string;
  description: string;
  createdOn: string;
  userName: string;
}

export interface TopSellingProduct {
  productId: number;
  productName: string;
  totalSold: number;
  revenue: number;
}

export interface VendorApprovalRequest {
  vendorId: number;
  approvalReason: string;
}

export interface VendorRejectionRequest {
  vendorId: number;
  rejectionReason: string;
}

export const adminService = {
  getPendingVendors: async (): Promise<PendingVendor[]> => {
    const response = await axios.get('/admin/vendors/pending');
    return response.data;
  },

  getVendorForApproval: async (vendorId: number): Promise<VendorApprovalDetails> => {
    const response = await axios.get(`/admin/vendors/${vendorId}/approval`);
    return response.data;
  },

  approveVendor: async (request: VendorApprovalRequest) => {
    const response = await axios.post('/admin/vendors/approve', request);
    return response.data;
  },

  rejectVendor: async (request: VendorRejectionRequest) => {
    const response = await axios.post('/admin/vendors/reject', request);
    return response.data;
  },

  getDashboardAnalytics: async (startDate: string, endDate: string): Promise<DashboardAnalytics> => {
    const response = await axios.get('/admin/analytics/dashboard', {
      params: { startDate, endDate }
    });
    return response.data;
  }
};