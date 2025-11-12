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