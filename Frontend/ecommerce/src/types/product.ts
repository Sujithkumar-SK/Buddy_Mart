export interface ProductImage {
  imageId: number;
  imagePath: string;
  displayOrder: number;
  isPrimary: boolean;
}

export interface Product {
  productId: number;
  name: string;
  description?: string;
  sku: string;
  price: number;
  discountPrice?: number;
  stockQuantity: number;
  minStockLevel?: number;
  brand?: string;
  weight?: string;
  dimensions?: string;
  status: string;
  isActive: boolean;
  vendorId: number;
  vendorName: string;
  categoryId: number;
  categoryName: string;
  createdOn: string;
  updatedOn?: string;
  imagePaths: string[];
  images?: ProductImage[];
}

export interface ProductCreateRequest {
  name: string;
  description?: string;
  sku: string;
  price: number;
  discountPrice?: number;
  stockQuantity: number;
  minStockLevel?: number;
  brand?: string;
  weight?: string;
  dimensions?: string;
  vendorId: number;
  categoryId: number;
}

export interface Category {
  categoryId: number;
  name: string;
  description?: string;
  imagePath?: string;
  isActive: boolean;
  parentCategoryId?: number;
  parentCategoryName?: string;
  createdOn: string;
  subCategories: Category[];
}
