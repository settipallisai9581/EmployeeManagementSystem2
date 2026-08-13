import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { useEffect } from 'react';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import EmployeeList from './pages/EmployeeList';
import EmployeeDetail from './pages/EmployeeDetail';
import EmployeeForm from './pages/EmployeeForm';
import SessionTimeout from './pages/SessionTimeout';
import { clientLogger } from './services/logger';
import './App.css';

function AppLayout() {
    const location = useLocation();
    const isAuthPage =
        location.pathname === '/login' ||
        location.pathname === '/register' ||
        location.pathname === '/session-timeout';

    useEffect(() => {
        clientLogger.info('Route changed', { path: location.pathname });
    }, [location.pathname]);

    return (
        <div className="app">
            {!isAuthPage && <Navbar />}
            <main className="main-content">
                <Routes>
                    <Route path="/" element={<Navigate to="/login" replace />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/session-timeout" element={<SessionTimeout />} />
                    <Route
                        path="/dashboard"
                        element={
                            <ProtectedRoute>
                                <Dashboard />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/employees"
                        element={
                            <ProtectedRoute>
                                <EmployeeList />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/employees/new"
                        element={
                            <ProtectedRoute>
                                <EmployeeForm />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/employees/:id"
                        element={
                            <ProtectedRoute>
                                <EmployeeDetail />
                            </ProtectedRoute>
                        }
                    />
                    <Route
                        path="/employees/:id/edit"
                        element={
                            <ProtectedRoute>
                                <EmployeeForm />
                            </ProtectedRoute>
                        }
                    />
                </Routes>
            </main>
        </div>
    );
}

function App() {
    return (
        <Router>
            <AuthProvider>
                <AppLayout />
            </AuthProvider>
        </Router>
    );
}

export default App;
