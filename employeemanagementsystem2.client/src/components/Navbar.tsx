import { Link, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import '../styles/Navbar.css';

const Navbar = () => {
  const { isAuthenticated, logout, user } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const isEmployeesPage = location.pathname.startsWith('/employees');
  const isDashboardOverviewVisible =
    isEmployeesPage && new URLSearchParams(location.search).get('overview') === '1';

  const handleDashboardClick = (e: React.MouseEvent) => {
    e.preventDefault();

    if (isDashboardOverviewVisible) {
      navigate('/employees');
      return;
    }

    navigate('/employees?overview=1');
  };

  return (
    <nav className="navbar">
      <div className="nav-container">
        {isAuthenticated && (
          <>
            <div className="nav-left">
              <button 
                onClick={handleDashboardClick} 
                className={`nav-link dashboard-btn ${isDashboardOverviewVisible ? 'active' : ''}`}
              >
                <span className="nav-icon" aria-hidden="true">
                  <svg viewBox="0 0 24 24" focusable="false">
                    <path d="M4 13h4v7H4v-7Zm6-9h4v16h-4V4Zm6 5h4v11h-4V9Z" fill="currentColor" />
                  </svg>
                </span>
                Dashboard
              </button>
              <Link 
                to="/employees" 
                className={`nav-link employees-btn ${isEmployeesPage && !isDashboardOverviewVisible ? 'active' : ''}`}
              >
                <span className="nav-icon" aria-hidden="true">
                  <svg viewBox="0 0 24 24" focusable="false">
                    <path d="M16 11a3 3 0 1 0-2.999-3A3 3 0 0 0 16 11Zm-8 0A3 3 0 1 0 5 8a3 3 0 0 0 3 3Zm0 2c-2.67 0-8 1.34-8 4v3h10v-3c0-1.2.57-2.24 1.53-3.06A13.14 13.14 0 0 0 8 13Zm8 0c-.29 0-.62.02-.97.05 1.19.86 1.97 2.01 1.97 3.45v3H24v-3c0-2.66-5.33-4-8-4Z" fill="currentColor" />
                  </svg>
                </span>
                Employees
              </Link>
              <Link to="/employees/new" className="nav-link add-employee">
                <span className="nav-icon" aria-hidden="true">
                  <svg viewBox="0 0 24 24" focusable="false">
                    <path d="M19 11h-6V5h-2v6H5v2h6v6h2v-6h6v-2Z" fill="currentColor" />
                  </svg>
                </span>
                Add Employee
              </Link>
            </div>
            <div className="nav-right">
              <span className="welcome-text">Welcome, {user?.username}</span>
              <button onClick={logout} className="btn-logout">
                <span className="nav-icon" aria-hidden="true">
                  <svg viewBox="0 0 24 24" focusable="false">
                    <path d="M16 17v-3h-6v-4h6V7l5 5-5 5ZM3 5h9v2H5v10h7v2H3V5Z" fill="currentColor" />
                  </svg>
                </span>
                Logout
              </button>
            </div>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
