export interface UserLoginResponse {
    userId: string;
    token: string;
}

export interface CategoryResponse {
    id: string;
    name: string;
}

export interface BlogPostResponse {
    id: string;
    categoryId: string;
    userId: string;
    title: string;
    content: string;
    categoryName: string;
    authorUsername: string;
    createdAt: string;
    updatedAt?: string | null;
}

export interface BlogPostCreateRequest {
    categoryId: string;
    title: string;
    content: string;
}

export interface BlogPostUpdateRequest extends BlogPostCreateRequest {}

export interface CommentResponse {
    id: string;
    blogPostId: string;
    userId: string;
    content: string;
    authorUsername: string;
    createdAt: string;
    updatedAt?: string | null;
}

export interface CommentCreateRequest {
    content: string;
}