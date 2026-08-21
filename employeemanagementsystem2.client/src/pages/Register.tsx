import { useState, useEffect } from 'react';
import type { FormEvent } from 'react';
import axios from 'axios';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/useAuth';
import { departmentApi, roleApi } from '../services/api';
import type { Department, Role } from '../types';
import '../styles/Auth.css';

const DEFAULT_DEPARTMENTS: Department[] = [
  { id: 1, name: 'Human Resources' },
  { id: 2, name: 'Information Technology' },
  { id: 3, name: 'Finance' },
  { id: 4, name: 'Operations' },
];

const DEFAULT_ROLES: Role[] = [
  { id: 1, name: 'Manager' },
  { id: 2, name: 'Team Lead' },
  { id: 3, name: 'Specialist' },
  { id: 4, name: 'Coordinator' },
];

const Register = () => {
  const [formData, setFormData] = useState({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    departmentId: '',
    roleId: '',
  });
  const [departments, setDepartments] = useState<Department[]>([]);
  const [roles, setRoles] = useState<Role[]>([]);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [deptData, roleData] = await Promise.all([
          departmentApi.getAll(),
          roleApi.getAll(),
        ]);
        setDepartments(deptData.length > 0 ? deptData : DEFAULT_DEPARTMENTS);
        setRoles(roleData.length > 0 ? roleData : DEFAULT_ROLES);
      } catch {
        setDepartments(DEFAULT_DEPARTMENTS);
        setRoles(DEFAULT_ROLES);
      }
    };
    fetchData();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError('');

    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    setLoading(true);

    try {
      await register({
        username: formData.username,
        email: formData.email,
        password: formData.password,
        firstName: formData.firstName,
        lastName: formData.lastName,
        departmentId: parseInt(formData.departmentId),
        roleId: parseInt(formData.roleId),
      });
      navigate('/employees');
    } catch (err) {
      if (axios.isAxiosError(err)) {
        const responseData = err.response?.data;
        const messageFromObject =
          responseData && typeof responseData === 'object' && 'message' in responseData
            ? (responseData as { message?: unknown }).message
            : undefined;
        const messageFromString = typeof responseData === 'string' ? responseData : undefined;

        const resolvedMessage =
          (typeof messageFromObject === 'string' && messageFromObject.trim().length > 0
            ? messageFromObject
            : undefined) ??
          (typeof messageFromString === 'string' && messageFromString.trim().length > 0
            ? messageFromString
            : undefined);

        setError(resolvedMessage ?? 'Registration failed. Please try again.');
      } else {
        setError('Registration failed. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-container">
      {/* Left Side - Branding */}
      <div className="auth-branding">
        <div className="branding-content">
          <h1 className="branding-title">
            Employee<br />Management
          </h1>
          <p className="branding-subtitle">Streamline your workforce operations</p>
        </div>
      </div>

      {/* Right Side - Register Form */}
      <div className="auth-form-container">
        <div className="auth-card">
          <div className="auth-icon">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" strokeLinecap="round" strokeLinejoin="round"/>
              <circle cx="9" cy="7" r="4" strokeLinecap="round" strokeLinejoin="round"/>
              <line x1="19" y1="8" x2="19" y2="14" strokeLinecap="round" strokeLinejoin="round"/>
              <line x1="22" y1="11" x2="16" y2="11" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>

          <h2>Create Account</h2>

          <form onSubmit={handleSubmit}>
            {error && <div className="error-message">{error}</div>}

            <div className="form-group">
              <label htmlFor="username">Username</label>
              <input
                id="username"
                name="username"
                type="text"
                value={formData.username}
                onChange={handleChange}
                required
                minLength={3}
                placeholder="Choose a username"
                autoComplete="username"
              />
            </div>

            <div className="form-group">
              <label htmlFor="email">Email address</label>
              <input
                id="email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleChange}
                required
                placeholder="your.email@company.com"
                autoComplete="email"
              />
            </div>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="firstName">First name</label>
                <input
                  id="firstName"
                  name="firstName"
                  type="text"
                  value={formData.firstName}
                  onChange={handleChange}
                  required
                  placeholder="John"
                  autoComplete="given-name"
                />
              </div>

              <div className="form-group">
                <label htmlFor="lastName">Last name</label>
                <input
                  id="lastName"
                  name="lastName"
                  type="text"
                  value={formData.lastName}
                  onChange={handleChange}
                  required
                  placeholder="Doe"
                  autoComplete="family-name"
                />
              </div>
            </div>

            <div className="form-row">
              <div className="form-group">
                <label htmlFor="departmentId">Department</label>
                <select
                  id="departmentId"
                  name="departmentId"
                  value={formData.departmentId}
                  onChange={handleChange}
                  required
                >
                  <option value="">Select department</option>
                  {departments.map((dept) => (
                    <option key={dept.id} value={dept.id}>
                      {dept.name}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label htmlFor="roleId">Role</label>
                <select
                  id="roleId"
                  name="roleId"
                  value={formData.roleId}
                  onChange={handleChange}
                  required
                >
                  <option value="">Select role</option>
                  {roles.map((role) => (
                    <option key={role.id} value={role.id}>
                      {role.name}
                    </option>
                  ))}
                </select>
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="password">Password</label>
              <input
                id="password"
                name="password"
                type="password"
                value={formData.password}
                onChange={handleChange}
                required
                minLength={6}
                placeholder="Create a password (min. 6 characters)"
                autoComplete="new-password"
              />
            </div>

            <div className="form-group">
              <label htmlFor="confirmPassword">Confirm password</label>
              <input
                id="confirmPassword"
                name="confirmPassword"
                type="password"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
                minLength={6}
                placeholder="Confirm your password"
                autoComplete="new-password"
              />
            </div>

            <button type="submit" className="btn-primary" disabled={loading}>
              {loading ? 'Creating account...' : 'Create Account'}
            </button>
          </form>

          <p className="auth-link">
            Already have an account? <Link to="/login">Sign In</Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
