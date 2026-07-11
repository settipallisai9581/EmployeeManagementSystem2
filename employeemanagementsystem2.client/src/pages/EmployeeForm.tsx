import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { AxiosError } from 'axios';
import { employeeApi, departmentApi, roleApi } from '../services/api';
import type { Department, Role, CreateEmployeeRequest, UpdateEmployeeRequest, Employee } from '../types';
import '../styles/Employee.css';

const EmployeeForm = () => {
  const { id } = useParams<{ id: string }>();
  const isEdit = !!id;
  const navigate = useNavigate();

  const [formData, setFormData] = useState<CreateEmployeeRequest & { isActive?: boolean }>({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    dateOfBirth: '',
    hireDate: new Date().toISOString().split('T')[0],
    address: '',
    city: '',
    state: '',
    zipCode: '',
    salary: undefined,
    departmentId: 0,
    roleId: 0,
    isActive: true,
  });

  const [departments, setDepartments] = useState<Department[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [photoFile, setPhotoFile] = useState<File | null>(null);

  const getApiErrorMessage = (error: AxiosError<{
    message?: string;
    title?: string;
    errors?: Record<string, string[]>;
  }>) => {
    const data = error.response?.data;
    if (!data) {
      return null;
    }

    if (data.message) {
      return data.message;
    }

    if (data.errors) {
      const firstFieldErrors = Object.values(data.errors).find(
        (messages) => Array.isArray(messages) && messages.length > 0
      );
      if (firstFieldErrors) {
        return firstFieldErrors[0];
      }
    }

    return data.title || null;
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [deptData, roleData] = await Promise.all([
          departmentApi.getAll(),
          roleApi.getAll(),
        ]);
        setDepartments(deptData);
        setRoles(roleData);

        if (isEdit && id) {
          const employeeData = await employeeApi.getById(parseInt(id));
          setFormData({
            firstName: employeeData.firstName,
            lastName: employeeData.lastName,
            email: employeeData.email,
            phone: employeeData.phone || '',
            dateOfBirth: employeeData.dateOfBirth?.split('T')[0] || '',
            hireDate: employeeData.hireDate.split('T')[0],
            address: employeeData.address || '',
            city: employeeData.city || '',
            state: employeeData.state || '',
            zipCode: employeeData.zipCode || '',
            salary: employeeData.salary,
            departmentId: employeeData.departmentId,
            roleId: employeeData.roleId,
            isActive: employeeData.isActive,
          });
        }
      } catch {
        setError('Failed to load form data');
      }
    };

    fetchData();
  }, [id, isEdit]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;

    if (type === 'checkbox') {
      const checked = (e.target as HTMLInputElement).checked;
      setFormData({ ...formData, [name]: checked });
    } else if (name === 'departmentId' || name === 'roleId') {
      setFormData({ ...formData, [name]: parseInt(value) });
    } else if (name === 'salary') {
      setFormData({ ...formData, [name]: value ? parseFloat(value) : undefined });
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handlePhotoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setPhotoFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');

    if (!formData.departmentId || !formData.roleId) {
      setError('Please select Department and Role.');
      return;
    }

    if (formData.salary !== undefined && formData.salary < 1) {
      setError('Salary must be greater than or equal to 1.');
      return;
    }

    setLoading(true);

    try {
      let employee: Employee;

      if (isEdit && id) {
        const updateData: UpdateEmployeeRequest = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          phone: formData.phone || undefined,
          dateOfBirth: formData.dateOfBirth || undefined,
          hireDate: formData.hireDate,
          address: formData.address || undefined,
          city: formData.city || undefined,
          state: formData.state || undefined,
          zipCode: formData.zipCode || undefined,
          salary: formData.salary,
          departmentId: formData.departmentId,
          roleId: formData.roleId,
          isActive: formData.isActive ?? true,
        };
        employee = await employeeApi.update(parseInt(id), updateData);
      } else {
        const createData: CreateEmployeeRequest = {
          firstName: formData.firstName,
          lastName: formData.lastName,
          email: formData.email,
          phone: formData.phone || undefined,
          dateOfBirth: formData.dateOfBirth || undefined,
          hireDate: formData.hireDate,
          address: formData.address || undefined,
          city: formData.city || undefined,
          state: formData.state || undefined,
          zipCode: formData.zipCode || undefined,
          salary: formData.salary,
          departmentId: formData.departmentId,
          roleId: formData.roleId,
        };
        employee = await employeeApi.create(createData);
      }

      if (photoFile && employee.id) {
        await employeeApi.uploadPhoto(employee.id, photoFile);
      }

      navigate('/employees', {
        state: {
          successMessage: isEdit
            ? 'Employee updated successfully'
            : 'Employee created successfully',
        },
      });
    } catch (err) {
      const axiosErr = err as AxiosError<{ message?: string }>;
      const serverMessage = getApiErrorMessage(axiosErr as AxiosError<{
        message?: string;
        title?: string;
        errors?: Record<string, string[]>;
      }>);
      setError(serverMessage || (isEdit ? 'Failed to update employee' : 'Failed to create employee'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="employee-form-container">
      <div className="form-header">
        <h1>{isEdit ? 'Edit Employee' : 'Add New Employee'}</h1>
        <Link to="/employees" className="btn-secondary">
          Cancel
        </Link>
      </div>

      {error && <div className="error-message">{error}</div>}

      <form onSubmit={handleSubmit} className="employee-form">
        <div className="form-section">
          <h2>Personal Information</h2>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="firstName">First Name *</label>
              <input
                id="firstName"
                name="firstName"
                type="text"
                value={formData.firstName}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="lastName">Last Name *</label>
              <input
                id="lastName"
                name="lastName"
                type="text"
                value={formData.lastName}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="email">Email *</label>
              <input
                id="email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="phone">Phone</label>
              <input
                id="phone"
                name="phone"
                type="tel"
                value={formData.phone}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="dateOfBirth">Date of Birth</label>
              <input
                id="dateOfBirth"
                name="dateOfBirth"
                type="date"
                value={formData.dateOfBirth}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="photo">Photo</label>
              <input
                id="photo"
                name="photo"
                type="file"
                accept="image/*"
                onChange={handlePhotoChange}
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Address</h2>
          <div className="form-group">
            <label htmlFor="address">Street Address</label>
            <input
              id="address"
              name="address"
              type="text"
              value={formData.address}
              onChange={handleChange}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="city">City</label>
              <input
                id="city"
                name="city"
                type="text"
                value={formData.city}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="state">State</label>
              <input
                id="state"
                name="state"
                type="text"
                value={formData.state}
                onChange={handleChange}
              />
            </div>

            <div className="form-group">
              <label htmlFor="zipCode">Zip Code</label>
              <input
                id="zipCode"
                name="zipCode"
                type="text"
                value={formData.zipCode}
                onChange={handleChange}
              />
            </div>
          </div>
        </div>

        <div className="form-section">
          <h2>Employment Information</h2>
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="departmentId">Department *</label>
              <select
                id="departmentId"
                name="departmentId"
                value={formData.departmentId}
                onChange={handleChange}
                required
              >
                <option value="">Select Department</option>
                {departments.map((dept) => (
                  <option key={dept.id} value={dept.id}>
                    {dept.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label htmlFor="roleId">Role *</label>
              <select
                id="roleId"
                name="roleId"
                value={formData.roleId}
                onChange={handleChange}
                required
              >
                <option value="">Select Role</option>
                {roles.map((role) => (
                  <option key={role.id} value={role.id}>
                    {role.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="hireDate">Hire Date *</label>
              <input
                id="hireDate"
                name="hireDate"
                type="date"
                value={formData.hireDate}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="salary">Salary</label>
              <input
                id="salary"
                name="salary"
                type="number"
                min="1"
                step="0.01"
                value={formData.salary || ''}
                onChange={handleChange}
              />
            </div>
          </div>

          {isEdit && (
            <div className="form-group">
              <label className="checkbox-label">
                <input
                  type="checkbox"
                  name="isActive"
                  checked={formData.isActive}
                  onChange={handleChange}
                />
                Active Employee
              </label>
            </div>
          )}
        </div>

        <div className="form-actions">
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Saving...' : isEdit ? 'Update Employee' : 'Create Employee'}
          </button>
          <Link to="/employees" className="btn-secondary">
            Cancel
          </Link>
        </div>
      </form>
    </div>
  );
};

export default EmployeeForm;
