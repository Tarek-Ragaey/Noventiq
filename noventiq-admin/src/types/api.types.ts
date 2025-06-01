export interface LoginModel {
    email: string;
    password: string;
    rememberMe?: boolean;
}

export interface AuthResponse {
    isSuccess: boolean;
    message: string;
    token?: string;
    refreshToken?: string;
    userId?: string;
    email?: string;
    roles?: string[];
    errors?: Record<string, string>;
}

export interface UserListDto {
    id: string;
    userName: string;
    email: string;
    roles: string[];
}

export interface RoleWithTranslationDto {
    id: string;
    name: string;
    translatedName: string;
}

export interface RoleModel {
    name: string;
    translations: Record<string, string>;
}

export interface RoleUpdateModel {
    newName: string;
    translations: Record<string, string>;
}

export interface UserRoleModel {
    userId: string;
    roleName: string;
}

export interface PaginationHeader {
    currentPage: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export interface CreateUserModel {
    userName: string;
    email: string;
    password: string;
    roles?: string[];
} 