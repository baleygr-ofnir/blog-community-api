import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import { UsersService } from '../services/UsersService';
import type {UserLoginRequest, UserLoginResponse, UserRegisterRequest } from '../types/Types';
        
interface AuthContextType {
    userId?: string;
    token?: string;
    isAuthenticated: boolean;
    login: (data: UserLoginRequest) => Promise<void>;
    register: (data: UserRegisterRequest) => Promise<void>;
    logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const useAuth = (): AuthContextType => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within useAuth');
    
    return context;
}

export const AuthProvider = ({ children }: { children: ReactNode }) => {
    const [userId, setUserId] = useState<string>();
    const [token, setToken] = useState<string>();
    const isAuthenticated: boolean = Boolean(userId && token);
    
    useEffect(() => {
        const storedUserId: string | null = localStorage.getItem('userId');
        const storedToken: string | null = localStorage.getItem('token');
        if (storedUserId && storedToken) {
            setUserId(storedUserId);
            setToken(storedToken);
        }
    }, []);

    const login = async (data: UserLoginRequest) => {
        const response: UserLoginResponse = await UsersService.login(data);
        
        localStorage.setItem('userId', response.userId);
        localStorage.setItem('token', response.token);
        
        setUserId(response.userId);
        setToken(response.token);
    }

    const register = async (data: UserRegisterRequest) => {
        const response: UserLoginResponse = await UsersService.register(data);

        localStorage.setItem('userId', response.userId);
        localStorage.setItem('token', response.token);
        
        setUserId(response.userId);
        setToken(response.token);
    }
    
    const logout = () => {
        localStorage.removeItem('userId');
        localStorage.removeItem('token');
        
        setUserId(undefined);
        setToken(undefined);
    }
    
    const value: AuthContextType = {
        userId,
        token,
        isAuthenticated,
        login,
        register,
        logout,
    }
    
    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}

