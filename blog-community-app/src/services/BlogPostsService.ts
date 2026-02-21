import { api } from '../config/api';
import type { BlogPostResponse, BlogPostCreateRequest, BlogPostUpdateRequest } from "../types/Types";

export const BlogPostsService = {
    create: (data: BlogPostCreateRequest) =>
        api.post<BlogPostResponse>('/api/blogposts/', data)
            .then(response => response.data),
    getAll: (categoryId?: string, title?: string)=> {
        const params = new URLSearchParams();
        if (categoryId) params.append('categoryId', categoryId);
        if (title?.trim()) params.append('title', title.trim());
      
        const query = params.toString() ? `?${params}` : '';
      
        return api.get<BlogPostResponse[]>(`/api/blogposts${query}`)
            .then(response => response.data);
    },
    getById: (id: string) =>
        api.get<BlogPostResponse>(`/api/blogposts/${id}`)
            .then(response => response.data),
    update: (id: string, data: BlogPostUpdateRequest) =>
        api.put(`/api/blogposts/${id}`, data)
}