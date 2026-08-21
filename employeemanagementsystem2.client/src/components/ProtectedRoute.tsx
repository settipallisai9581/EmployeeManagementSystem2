import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/useAuth';

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated, sessionTimedOut } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to={sessionTimedOut ? '/session-timeout' : '/login'} replace />;
  }

  return <>{children}</>;
};

export default ProtectedRoute;
