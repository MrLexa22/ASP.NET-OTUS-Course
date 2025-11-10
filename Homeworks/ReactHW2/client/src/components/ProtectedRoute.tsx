import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthProvider';

export const ProtectedRoute: React.FC<{ children: React.ReactElement }> = ({ children }) => {
   const { user } = useAuth();
   if (!user) return <Navigate to="/login" replace />;
   return children;
};