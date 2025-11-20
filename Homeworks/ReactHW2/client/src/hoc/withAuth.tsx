import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../auth/AuthProvider';

export function withAuth<P extends object>(WrappedComponent: React.ComponentType<P>) {
   const ComponentWithAuth: React.FC<P> = (props) => {
      const { user } = useAuth();
      if (!user) return <Navigate to="/login" replace />;
      return <WrappedComponent {...props} />;
   };
   ComponentWithAuth.displayName = `withAuth(${WrappedComponent.displayName || WrappedComponent.name || 'Component'})`;
   return ComponentWithAuth;
}

export function withoutAuth<P extends object>(WrappedComponent: React.ComponentType<P>) {
   const ComponentWithoutAuth: React.FC<P> = (props) => {
      const { user } = useAuth();
      if (user) return <Navigate to="/home" replace />;
      return <WrappedComponent {...props} />;
   };
   ComponentWithoutAuth.displayName = `withoutAuth(${WrappedComponent.displayName || WrappedComponent.name || 'Component'})`;
   return ComponentWithoutAuth;
}