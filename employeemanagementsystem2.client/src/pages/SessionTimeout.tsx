import { Link } from 'react-router-dom';
import '../styles/SessionTimeout.css';

const SessionTimeout = () => {
  return (
    <div className="session-timeout-page">
      <div className="session-timeout-card">
        <h1>Session Timed Out</h1>
        <p>Your session is timed out. Please login again.</p>
        <Link to="/login" className="session-timeout-login-button">
          Login
        </Link>
      </div>
    </div>
  );
};

export default SessionTimeout;
