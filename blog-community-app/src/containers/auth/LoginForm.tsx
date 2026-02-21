import  { useState, type SyntheticEvent, type ChangeEvent } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import type { UserLoginRequest } from "../../types/Types.ts";

export const LoginForm = () => {
    const { login, isAuthenticated } = useAuth();
    const [loginForm, setLoginForm] = useState<UserLoginRequest>({
        usernameOrEmail: '',
        password: ''
    });
    const [loginError, setLoginError] = useState<string | null>(null);
    const [isLoginSubmitting, setIsLoginSubmitting] = useState(false);
    
    const handleLoginChange = (event: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setLoginForm(currentLoginForm => ({
            ...currentLoginForm,
            [name]: value
        }));
    };
    
    const handleLoginSubmit = async (event: SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault();
        setLoginError(null);
        setIsLoginSubmitting(true);
        
        login(loginForm)
            .catch(error => setLoginError(String(error)))
            .finally(() => setIsLoginSubmitting(false));
    };
    
    if (isAuthenticated) {
        return <p>You are already logged in.</p>;
    }
    
    return (
        <form onSubmit={handleLoginSubmit}>
            {loginError && <p style={{ color: "red" }}>{loginError}</p>}
            
            <div>
                <label>
                    Username or Email:
                    <input
                        type="text"
                        name="usernameOrEmail"
                        value={loginForm.usernameOrEmail}
                        onChange={handleLoginChange}
                        autoComplete="username"
                    />
                </label>
            </div>
            
            <div>
                <label>
                    Password:
                    <input 
                        type="password"
                        name="password"
                        value={loginForm.password}
                        onChange={handleLoginChange}
                        autoComplete="current-password"
                    />
                </label>
            </div>

            <button type="submit" disabled={isLoginSubmitting}>
                {isLoginSubmitting ? 'Logging in...' : 'Login'}
            </button>
        </form>
    );
}