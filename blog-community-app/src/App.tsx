import { JSX } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { LoginForm } from "./containers/auth/LoginForm";
import './App.css'
// TODO: other containers once created
// import { BlogPostList } from './containers/blogposts/BlostPostList';

const ProtectedRoute = ({ children }: { children: JSX.Element }) => {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  
  return children;
};

function App() {

  return (
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/" element={<div>Home (posts lists later)</div>} />
            <Route path="/login" element={<LoginForm />} />
              {/* <Route path="/register" element={<RegisterForm />} /> */}

              {/*
              <Route
                path="/posts/new"
                element={
                    <ProtectedRoute>
                        <BlogPostForm />
                    </ProtectedRoute>
                }
              />
                */}
              <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
  )
}

export default App
