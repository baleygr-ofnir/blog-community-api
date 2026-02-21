import { api } from '../config/api.ts';
import type { CategoryResponse, CategoryRequest } from "../types/Types.ts";

export const CategoriesService = {
    create: (data: CategoryRequest) =>
        api.post<CategoryResponse>('/api/categories', data),
    getAll: () =>
        api.get<CategoryResponse[]>('/api/categories')
            .then(response => response.data),
    getById: (id: string) =>
        api.get<CategoryResponse>(`/api/categories/${id}`)
            .then(response => response.data),
    update: (id: string, data: CategoryRequest) =>
        api.put(`/api/categories/${id}`, data),
    delete: (id: string) =>
        api.delete(`/api/categories/${id}`)
};