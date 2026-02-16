import React from 'react';
import { Link } from 'react-router-dom';
import './Home.css';

const Home: React.FC = () => {
  return (
    <div className="home-page">
      <h1>Welcome to Skill Extractor</h1>
      <p>Extract and manage skills from CV documents.</p>
      <div className="home-actions">
        <Link to="/new" className="action-btn primary">
          Extract Skills from CV
        </Link>
        <Link to="/employees" className="action-btn secondary">
          View All Employees
        </Link>
      </div>
    </div>
  );
};

export default Home;
