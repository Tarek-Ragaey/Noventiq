import { create } from 'zustand';
import { apiService } from '../services/api.service';
import { AuthResponse, LoginModel } from '../types/api.types';

interface AuthState {
    isAuthenticated: boolean;
    user: {
        id?: string;
        email?: string;
        roles?: string[];
    } | null;
    isLoading: boolean;
    error: string | null;
    login: (data: LoginModel) => Promise<void>;
    logout: () => Promise<void>;
    checkAuth: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
    isAuthenticated: false,
    user: null,
    isLoading: false,
    error: null,

    login: async (data: LoginModel) => {
        set({ isLoading: true, error: null });
        try {
            const response = await apiService.login(data);
            if (response.isSuccess) {
                localStorage.setItem('token', response.token!);
                localStorage.setItem('refreshToken', response.refreshToken!);
                set({
                    isAuthenticated: true,
                    user: {
                        id: response.userId,
                        email: response.email,
                        roles: response.roles
                    },
                    isLoading: false
                });
            } else {
                set({ error: response.message, isLoading: false });
            }
        } catch (error) {
            set({ error: 'Failed to login', isLoading: false });
        }
    },

    logout: async () => {
        set({ isLoading: true });
        try {
            await apiService.logout();
        } finally {
            set({
                isAuthenticated: false,
                user: null,
                isLoading: false
            });
        }
    },

    checkAuth: async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            set({ isAuthenticated: false, user: null });
            return;
        }

        try {
            const response = await apiService.refreshToken(localStorage.getItem('refreshToken')!);
            if (response.isSuccess) {
                localStorage.setItem('token', response.token!);
                localStorage.setItem('refreshToken', response.refreshToken!);
                set({
                    isAuthenticated: true,
                    user: {
                        id: response.userId,
                        email: response.email,
                        roles: response.roles
                    }
                });
            } else {
                set({ isAuthenticated: false, user: null });
            }
        } catch {
            set({ isAuthenticated: false, user: null });
        }
    }
})); 