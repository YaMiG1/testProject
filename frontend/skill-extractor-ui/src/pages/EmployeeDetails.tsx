import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import './EmployeeDetails.css';

interface Skill {
  id: number;
  name: string;
}

interface CVDocument {
  id: number;
  createdAt: string;
  preview: string;
}

interface EmployeeDetail {
  id: number;
  fullName: string;
  email?: string;
  createdAt: string;
  skills: Skill[];
  cvDocuments: CVDocument[];
}

const EmployeeDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [employee, setEmployee] = useState<EmployeeDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchEmployee = async () => {
      try {
        const data = await (await import('../api')).employeesApi.getDetails(Number(id));
        setEmployee(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchEmployee();
  }, [id]);

  if (loading) return <div className="employee-details"><p>Loading...</p></div>;
  if (error) return <div className="employee-details error-message">{error}</div>;
  if (!employee) return <div className="employee-details"><p>Employee not found</p></div>;

  return (
    <div className="employee-details">
      <Link to="/employees" className="back-link">← Back to Employees</Link>
      
      <div className="details-card">
        <h1>{employee.fullName}</h1>
        {employee.email && <p><strong>Email:</strong> {employee.email}</p>}
        <p><strong>Added:</strong> {new Date(employee.createdAt).toLocaleDateString()}</p>
      </div>

      <div className="details-section">
        <h2>Skills ({employee.skills.length})</h2>
        {employee.skills.length > 0 ? (
          <ul className="skills-list">
            {employee.skills.map((skill) => (
              <li key={skill.id}>{skill.name}</li>
            ))}
          </ul>
        ) : (
          <p>No skills recorded.</p>
        )}
      </div>

      <div className="details-section">
        <h2>CV Documents ({employee.cvDocuments.length})</h2>
        {employee.cvDocuments.length > 0 ? (
          <div className="cv-list">
            {employee.cvDocuments.map((doc) => (
              <div key={doc.id} className="cv-item">
                <h4>Document #{doc.id}</h4>
                <p><strong>Added:</strong> {new Date(doc.createdAt).toLocaleDateString()}</p>
                <p className="preview">{doc.preview}</p>
              </div>
            ))}
          </div>
        ) : (
          <p>No CV documents uploaded.</p>
        )}
      </div>
    </div>
  );
};

export default EmployeeDetails;
