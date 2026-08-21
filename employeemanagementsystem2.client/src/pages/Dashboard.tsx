import { useState, useEffect, useCallback } from 'react';
import { Link } from 'react-router-dom';
import { employeeApi, departmentApi, roleApi } from '../services/api';
import type { Employee, Department } from '../types';
import '../styles/Dashboard.css';

interface DashboardStats {
  totalEmployees: number;
  activeEmployees: number;
  inactiveEmployees: number;
  totalDepartments: number;
  totalRoles: number;
  recentHires: number;
  avgSalary: number;
}

const Dashboard = () => {
  const [stats, setStats] = useState<DashboardStats>({
    totalEmployees: 0,
    activeEmployees: 0,
    inactiveEmployees: 0,
    totalDepartments: 0,
    totalRoles: 0,
    recentHires: 0,
    avgSalary: 0,
  });
  const [recentEmployees, setRecentEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const loadDashboardData = useCallback(async () => {
    try {
      setLoading(true);
      const [employees, depts, roles] = await Promise.all([
        employeeApi.getAll(),
        departmentApi.getAll(),
        roleApi.getAll(),
      ]);

      const activeCount = employees.filter((emp) => emp.isActive).length;
      const thirtyDaysAgo = Date.now() - (30 * 24 * 60 * 60 * 1000);
      const recentHiresCount = employees.filter((emp) => {
        const hireDate = new Date(emp.hireDate).getTime();
        return hireDate >= thirtyDaysAgo;
      }).length;

      const salaries = employees.filter((emp) => emp.salary).map((emp) => emp.salary || 0);
      const avgSalary = salaries.length > 0
        ? salaries.reduce((sum, salary) => sum + salary, 0) / salaries.length
        : 0;

      setStats({
        totalEmployees: employees.length,
        activeEmployees: activeCount,
        inactiveEmployees: employees.length - activeCount,
        totalDepartments: depts.length,
        totalRoles: roles.length,
        recentHires: recentHiresCount,
        avgSalary,
      });

      const sortedEmployees = [...employees]
        .sort((a, b) => new Date(b.hireDate).getTime() - new Date(a.hireDate).getTime())
        .slice(0, 5);
      setRecentEmployees(sortedEmployees);
      setDepartments(depts);
    } catch {
      setError('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadDashboardData();
  }, [loadDashboardData]);

  if (loading) {
    return <div className="loading">Loading dashboard...</div>;
  }

  if (error) {
    return <div className="error-message">{error}</div>;
  }

  return (
    <div className="dashboard-container">
      <div className="dashboard-header">
        <div>
          <h1>Dashboard</h1>
          <p className="header-subtitle">Welcome back! Here's your workforce overview.</p>
        </div>
        <Link to="/employees/new" className="btn-primary">
          Add New Employee
        </Link>
      </div>

      <div className="stats-grid">
        <div className="stat-card primary">
          <div className="stat-icon">??</div>
          <div className="stat-content">
            <span className="stat-label">Total Employees</span>
            <strong className="stat-value">{stats.totalEmployees}</strong>
          </div>
        </div>

        <div className="stat-card success">
          <div className="stat-icon">?</div>
          <div className="stat-content">
            <span className="stat-label">Active</span>
            <strong className="stat-value">{stats.activeEmployees}</strong>
          </div>
        </div>

        <div className="stat-card warning">
          <div className="stat-icon">?</div>
          <div className="stat-content">
            <span className="stat-label">Inactive</span>
            <strong className="stat-value">{stats.inactiveEmployees}</strong>
          </div>
        </div>

        <div className="stat-card info">
          <div className="stat-icon">??</div>
          <div className="stat-content">
            <span className="stat-label">Departments</span>
            <strong className="stat-value">{stats.totalDepartments}</strong>
          </div>
        </div>

        <div className="stat-card purple">
          <div className="stat-icon">??</div>
          <div className="stat-content">
            <span className="stat-label">Roles</span>
            <strong className="stat-value">{stats.totalRoles}</strong>
          </div>
        </div>

        <div className="stat-card accent">
          <div className="stat-icon">??</div>
          <div className="stat-content">
            <span className="stat-label">Recent Hires (30d)</span>
            <strong className="stat-value">{stats.recentHires}</strong>
          </div>
        </div>
      </div>

      <div className="dashboard-grid">
        <div className="dashboard-card recent-employees">
          <div className="card-header">
            <h2>Recent Hires</h2>
            <Link to="/employees" className="view-all-link">View All ?</Link>
          </div>
          <div className="employees-list">
            {recentEmployees.length === 0 ? (
              <div className="empty-state">
                <p>No employees found</p>
              </div>
            ) : (
              recentEmployees.map((employee) => (
                <Link to={`/employees/${employee.id}`} key={employee.id} className="employee-item">
                  <div className="employee-avatar">
                    {employee.firstName.charAt(0)}{employee.lastName.charAt(0)}
                  </div>
                  <div className="employee-info">
                    <strong>{employee.firstName} {employee.lastName}</strong>
                    <span>{employee.departmentName} | {employee.roleName}</span>
                  </div>
                  <div className="employee-date">
                    {new Date(employee.hireDate).toLocaleDateString()}
                  </div>
                </Link>
              ))
            )}
          </div>
        </div>

        <div className="dashboard-card departments-overview">
          <div className="card-header">
            <h2>Departments</h2>
          </div>
          <div className="departments-list">
            {departments.length === 0 ? (
              <div className="empty-state">
                <p>No departments found</p>
              </div>
            ) : (
              departments.map((dept) => (
                <div key={dept.id} className="department-item">
                  <div className="department-icon">??</div>
                  <div className="department-info">
                    <strong>{dept.name}</strong>
                    <span>{dept.description || 'No description'}</span>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      <div className="dashboard-card quick-actions">
        <div className="card-header">
          <h2>Quick Actions</h2>
        </div>
        <div className="actions-grid">
          <Link to="/employees/new" className="action-card">
            <div className="action-icon">?</div>
            <strong>Add Employee</strong>
            <span>Register a new team member</span>
          </Link>
          <Link to="/employees" className="action-card">
            <div className="action-icon">??</div>
            <strong>View All Employees</strong>
            <span>Browse complete workforce</span>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;

