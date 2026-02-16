import React from 'react';
import { Link } from 'react-router-dom';
import './Navbar.css';

const Navbar: React.FC = () => {
  return (
    <nav className="navbar">
      <div className="navbar-content">
        <Link to="/" className="navbar-logo">
          Skill Extractor
        </Link>
        <ul className="navbar-menu">
          <li className="navbar-item">
            <Link to="/" className="navbar-link">Home</Link>
          </li>
          <li className="navbar-item">
            <Link to="/new" className="navbar-link">New CV</Link>
          </li>
          <li className="navbar-item">
            <Link to="/employees" className="navbar-link">Employees</Link>
          </li>
          <li className="navbar-item">
            <Link to="/admin/skills" className="navbar-link">Admin</Link>
          </li>
        </ul>
      </div>
    </nav>
  );
};

export default Navbar;
