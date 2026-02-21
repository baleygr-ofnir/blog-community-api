import { api } from '../config/api';
import type { CommentResponse, CommentCreateRequest } from '../types/Types';

export const CommentsService = {
    create: (blogPostId: string, data: CommentCreateRequest) =>
        api.post<CommentResponse>(`/api/blogPosts/${blogPostId}/comments`, data)
            .then(response => response.data),
    getAll: (blogPostId: string) =>
        api.get<CommentResponse[]>(`/api/blogPosts/${blogPostId}/comments`)
            .then(response => response.data),
};