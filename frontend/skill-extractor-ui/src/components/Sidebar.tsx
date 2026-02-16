import React from 'react';
import { Link } from 'react-router-dom';
import './Sidebar.css';

const Sidebar: React.FC = () => {
  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <h2>Admin Panel</h2>
      </div>
      <ul className="sidebar-menu">
        <li className="sidebar-item">
          <Link to="/" className="sidebar-link">Back to Home</Link>
        </li>
        <li className="sidebar-item">
          <Link to="/admin/skills" className="sidebar-link">Manage Skills</Link>
        </li>
        <li className="sidebar-item">
          <Link to="/employees" className="sidebar-link">View Employees</Link>
        </li>
      </ul>
    </aside>
  );
};

export default Sidebar;
