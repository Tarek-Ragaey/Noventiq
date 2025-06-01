import axios, { AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { 
    AuthResponse, 
    LoginModel, 
    UserListDto, 
    RoleWithTranslationDto,
    CreateUserModel,
    RoleModel,
    RoleUpdateModel,
    UserRoleModel
} from '../types/api.types';

class ApiService {
    private api: AxiosInstance;
    private language: string = 'en';

    constructor() {
        const apiUrl = process.env.REACT_APP_API_URL || 'https://localhost:7213/api';
        console.log('API URL:', apiUrl); // Debug log

        this.api = axios.create({
            baseURL: apiUrl,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        // Add request interceptor for auth token
        this.api.interceptors.request.use(
            (config: InternalAxiosRequestConfig) => {
                const token = localStorage.getItem('token');
                if (token) {
                    config.headers.Authorization = `Bearer ${token}`;
                }
                console.log('Request URL:', `${config.baseURL}${config.url}`); // Debug log
                return config;
            },
            (error) => Promise.reject(error)
        );

        // Add response interceptor for token refresh
        this.api.interceptors.response.use(
            (response) => response,
            async (error) => {
                const originalRequest = error.config;
                if (error.response?.status === 401 && !originalRequest._retry) {
                    originalRequest._retry = true;
                    try {
                        const refreshToken = localStorage.getItem('refreshToken');
                        const response = await this.refreshToken(refreshToken!);
                        if (response.isSuccess) {
                            localStorage.setItem('token', response.token!);
                            localStorage.setItem('refreshToken', response.refreshToken!);
                            return this.api(originalRequest);
                        }
                    } catch {
                        localStorage.removeItem('token');
                        localStorage.removeItem('refreshToken');
                        window.location.href = '/login';
                    }
                }
                return Promise.reject(error);
            }
        );
    }

    setLanguage(lang: string) {
        this.language = lang;
    }

    // Auth endpoints
    async login(data: LoginModel): Promise<AuthResponse> {
        const response = await this.api.post<AuthResponse>('/Authentication/login', data);
        return response.data;
    }

    async refreshToken(refreshToken: string): Promise<AuthResponse> {
        const response = await this.api.post<AuthResponse>('/Authentication/refresh-token', {
            refreshToken
        });
        return response.data;
    }

    async logout(): Promise<void> {
        await this.api.post('/Authentication/logout');
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
    }

    // User endpoints
    async getUsers(page: number = 1, pageSize: number = 10): Promise<{ users: UserListDto[], totalCount: number }> {
        const response = await this.api.get<UserListDto[]>(`/User?pageNumber=${page}&pageSize=${pageSize}&languageKey=${this.language}`);
        const totalCount = parseInt(response.headers['x-pagination-total-count'] || '0');
        return { users: response.data, totalCount };
    }

    async createUser(data: CreateUserModel): Promise<AuthResponse> {
        const response = await this.api.post<AuthResponse>('/User', data);
        return response.data;
    }

    async deleteUser(id: string): Promise<AuthResponse> {
        const response = await this.api.delete<AuthResponse>(`/User/${id}`);
        return response.data;
    }

    // Role endpoints
    async getRoles(page: number = 1, pageSize: number = 10): Promise<{ roles: RoleWithTranslationDto[], totalCount: number }> {
        const response = await this.api.get<RoleWithTranslationDto[]>(
            `/Role?pageNumber=${page}&pageSize=${pageSize}&languageKey=${this.language}`
        );
        const totalCount = parseInt(response.headers['x-pagination-total-count'] || '0');
        return { roles: response.data, totalCount };
    }

    async createRole(data: RoleModel): Promise<AuthResponse> {
        const response = await this.api.post<AuthResponse>('/Role', data);
        return response.data;
    }

    async updateRole(currentName: string, data: RoleUpdateModel): Promise<AuthResponse> {
        const response = await this.api.put<AuthResponse>(`/Role/${currentName}`, data);
        return response.data;
    }

    async deleteRole(name: string): Promise<AuthResponse> {
        const response = await this.api.delete<AuthResponse>(`/Role/${name}`);
        return response.data;
    }

    async assignRole(data: UserRoleModel): Promise<AuthResponse> {
        const response = await this.api.post<AuthResponse>('/Role/assign', data);
        return response.data;
    }

    async removeRole(data: UserRoleModel): Promise<AuthResponse> {
        const response = await this.api.post<AuthResponse>('/Role/remove', data);
        return response.data;
    }
}

export const apiService = new ApiService(); 