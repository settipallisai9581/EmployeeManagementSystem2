import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { employeeApi } from '../services/api';
import type { Employee } from '../types';
import '../styles/Employee.css';

const EmployeeDetail = () => {
  const { id } = useParams<{ id: string }>();
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const loadEmployee = async () => {
      if (!id) return;

      try {
        setLoading(true);
        const data = await employeeApi.getById(parseInt(id));
        setEmployee(data);
      } catch {
        setError('Failed to load employee details');
      } finally {
        setLoading(false);
      }
    };

    loadEmployee();
  }, [id]);

  if (loading) {
    return <div className="loading">Loading employee details...</div>;
  }

  if (error || !employee) {
    return (
      <div className="error-container">
        <div className="error-message">{error || 'Employee not found'}</div>
        <Link to="/employees" className="btn-primary">
          Back to List
        </Link>
      </div>
    );
  }

  return (
    <div className="employee-detail-container">
      <div className="detail-header">
        <h1>Employee Details</h1>
        <div className="detail-actions">
          <Link to={`/employees/${employee.id}/edit`} className="btn-edit">
            Edit
          </Link>
          <Link to="/employees" className="btn-secondary">
            Back to List
          </Link>
        </div>
      </div>

      <div className="detail-card">
        {employee.photoPath && (
          <div className="employee-photo">
            <img src={employee.photoPath} alt={`${employee.firstName} ${employee.lastName}`} />
          </div>
        )}

        <div className="detail-section">
          <h2>Personal Information</h2>
          <div className="detail-grid">
            <div className="detail-item">
              <label>First Name:</label>
              <span>{employee.firstName}</span>
            </div>
            <div className="detail-item">
              <label>Last Name:</label>
              <span>{employee.lastName}</span>
            </div>
            <div className="detail-item">
              <label>Email:</label>
              <span>{employee.email}</span>
            </div>
            <div className="detail-item">
              <label>Phone:</label>
              <span>{employee.phone || '-'}</span>
            </div>
            <div className="detail-item">
              <label>Date of Birth:</label>
              <span>
                {employee.dateOfBirth
                  ? new Date(employee.dateOfBirth).toLocaleDateString()
                  : '-'}
              </span>
            </div>
            <div className="detail-item">
              <label>Status:</label>
              <span className={`status ${employee.isActive ? 'active' : 'inactive'}`}>
                {employee.isActive ? 'Active' : 'Inactive'}
              </span>
            </div>
          </div>
        </div>

        <div className="detail-section">
          <h2>Address</h2>
          <div className="detail-grid">
            <div className="detail-item full-width">
              <label>Address:</label>
              <span>{employee.address || '-'}</span>
            </div>
            <div className="detail-item">
              <label>City:</label>
              <span>{employee.city || '-'}</span>
            </div>
            <div className="detail-item">
              <label>State:</label>
              <span>{employee.state || '-'}</span>
            </div>
            <div className="detail-item">
              <label>Zip Code:</label>
              <span>{employee.zipCode || '-'}</span>
            </div>
          </div>
        </div>

        <div className="detail-section">
          <h2>Employment Information</h2>
          <div className="detail-grid">
            <div className="detail-item">
              <label>Department:</label>
              <span>{employee.departmentName}</span>
            </div>
            <div className="detail-item">
              <label>Role:</label>
              <span>{employee.roleName}</span>
            </div>
            <div className="detail-item">
              <label>Hire Date:</label>
              <span>{new Date(employee.hireDate).toLocaleDateString()}</span>
            </div>
            <div className="detail-item">
              <label>Salary:</label>
              <span>
                {employee.salary
                  ? `$${employee.salary.toLocaleString()}`
                  : '-'}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EmployeeDetail;
