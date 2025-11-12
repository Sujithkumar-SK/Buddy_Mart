import axios from 'axios';

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

export interface CategoryCreateRequest {
  name: string;
  description?: string;
  parentCategoryId?: number;
  imagePath?: string;
}

export interface CategoryUpdateRequest {
  name: string;
  description?: string;
  parentCategoryId?: number;
  imagePath?: string;
  isActive: boolean;
}

export const categoryService = {
  getAllCategories: async (): Promise<Category[]> => {
    const response = await axios.get('/category');
    return response.data;
  },

  getCategoryById: async (id: number): Promise<Category> => {
    const response = await axios.get(`/category/${id}`);
    return response.data;
  },

  createCategory: async (data: CategoryCreateRequest): Promise<Category> => {
    const response = await axios.post('/category', data);
    return response.data;
  },

  updateCategory: async (id: number, data: CategoryUpdateRequest): Promise<Category> => {
    const response = await axios.put(`/category/${id}`, data);
    return response.data;
  },

  deleteCategory: async (id: number): Promise<void> => {
    await axios.delete(`/category/${id}`);
  }
};