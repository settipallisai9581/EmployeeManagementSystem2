import { useState, useEffect, useMemo } from 'react';
import { useLocation, useNavigate, useSearchParams } from 'react-router-dom';
import { employeeApi } from '../services/api';
import type { Employee } from '../types';
import '../styles/Employee.css';

const EmployeeList = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const [searchParams] = useSearchParams();
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const showDashboardOverview = searchParams.get('overview') === '1';

  const overview = useMemo(() => {
    const totalEmployees = employees.length;
    const activeEmployees = employees.filter((emp) => emp.isActive).length;
    const inactiveEmployees = totalEmployees - activeEmployees;
    const departments = new Set(employees.map((emp) => emp.departmentName).filter(Boolean));
    const roles = new Set(employees.map((emp) => emp.roleName).filter(Boolean));

    return {
      totalEmployees,
      activeEmployees,
      inactiveEmployees,
      departments: departments.size,
      roles: roles.size,
    };
  }, [employees]);

  const loadEmployees = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await employeeApi.getAll();
      setEmployees(data);
    } catch {
      setError('Failed to load employees');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadEmployees();
  }, []);

  useEffect(() => {
    const state = location.state as { successMessage?: string } | null;
    if (!state?.successMessage) {
      return;
    }

    setSuccessMessage(state.successMessage);
    navigate(location.pathname + location.search, { replace: true, state: null });
  }, [location.pathname, location.search, location.state, navigate]);

  useEffect(() => {
    if (successMessage) {
      const timer = setTimeout(() => setSuccessMessage(''), 3000);
      return () => clearTimeout(timer);
    }
  }, [successMessage]);

  const handleDelete = async (id: number, name: string) => {
    if (!window.confirm(`Are you sure you want to delete ${name}?`)) {
      return;
    }

    try {
      await employeeApi.delete(id);
      setEmployees(employees.filter((emp) => emp.id !== id));
      setSuccessMessage('Employee deleted successfully');
    } catch {
      setError('Failed to delete employee');
    }
  };

  if (loading) {
    return <div className="loading">Loading employees...</div>;
  }

  return (
    <div className="employee-list-container">
      {error && <div className="error-message">{error}</div>}
      {successMessage && <div className="success-message">{successMessage}</div>}

      {showDashboardOverview && (
        <section id="dashboard-overview-panel" className="dashboard-overview-panel">
          <div className="overview-card">
            <span className="overview-label">Total Employees</span>
            <strong className="overview-value">{overview.totalEmployees}</strong>
          </div>
          <div className="overview-card">
            <span className="overview-label">Active</span>
            <strong className="overview-value">{overview.activeEmployees}</strong>
          </div>
          <div className="overview-card">
            <span className="overview-label">Inactive</span>
            <strong className="overview-value">{overview.inactiveEmployees}</strong>
          </div>
          <div className="overview-card">
            <span className="overview-label">Departments</span>
            <strong className="overview-value">{overview.departments}</strong>
          </div>
          <div className="overview-card">
            <span className="overview-label">Roles</span>
            <strong className="overview-value">{overview.roles}</strong>
          </div>
        </section>
      )}

      <div className="employee-grid">
        {employees.length === 0 ? (
          <div className="empty-state">
            <strong>No employees found</strong>
            <span>No employee records available</span>
          </div>
        ) : (
          employees.map((employee) => (
            <div key={employee.id} className="employee-card">
              <div className="card-avatar-large">
                {employee.firstName.charAt(0)}{employee.lastName.charAt(0)}
              </div>
              <h3>{employee.firstName} {employee.lastName}</h3>
              <p className="card-email">{employee.email}</p>
              <div className="card-details">
                <span className="badge badge-department">{employee.departmentName}</span>
                <span className="badge badge-role">{employee.roleName}</span>
              </div>
              <div className="card-info">
                <span>📞 {employee.phone || 'N/A'}</span>
                <span>📅 {new Date(employee.hireDate).toLocaleDateString()}</span>
              </div>
              <span className={`status ${employee.isActive ? 'active' : 'inactive'}`}>
                {employee.isActive ? '✓ Active' : '✗ Inactive'}
              </span>
              <div className="card-actions">
                <button onClick={() => navigate(`/employees/${employee.id}`)} className="btn-view">
                  View
                </button>
                <button onClick={() => navigate(`/employees/${employee.id}/edit`)} className="btn-edit">
                  Edit
                </button>
                <button onClick={() => handleDelete(employee.id, `${employee.firstName} ${employee.lastName}`)} className="btn-delete">
                  Delete
                </button>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default EmployeeList;
