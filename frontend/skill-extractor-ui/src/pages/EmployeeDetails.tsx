import React, { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import './EmployeeDetails.css';
import { employeesApi } from '../api';
import type { EmployeeDetailsDto } from '../api/types';
import SkillChips from '../components/SkillChips';

const EmployeeDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [employee, setEmployee] = useState<EmployeeDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchEmployee = async () => {
      if (!id) {
        setError('Missing employee id');
        setLoading(false);
        return;
      }

      const numericId = Number(id);
      if (Number.isNaN(numericId)) {
        setError('Invalid employee id');
        setLoading(false);
        return;
      }

      try {
        const data = await employeesApi.getDetails(numericId);
        setEmployee(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchEmployee();
  }, [id]);

  const handleDelete = async () => {
    if (!employee) return;
    if (!window.confirm('Delete this employee?')) return;

    try {
      await employeesApi.remove(employee.id);
      navigate('/employees');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to delete employee');
    }
  };

  if (loading) return <div className="employee-details"><p>Loading...</p></div>;
  if (error) return <div className="employee-details error-message">{error}</div>;
  if (!employee) return <div className="employee-details"><p>Employee not found</p></div>;

  return (
    <div className="employee-details">
      <div className="page-actions">
        <Link to="/employees" className="back-link">← Back to Employees</Link>
        <button className="btn btn-danger" onClick={handleDelete}>Delete employee</button>
      </div>

      <div className="details-card">
        <h1>{employee.fullName}</h1>
        {employee.email && <p><strong>Email:</strong> {employee.email}</p>}
        <p><strong>Added:</strong> {new Date(employee.createdAt).toLocaleString()}</p>
      </div>

      <div className="details-section">
        <h2>Skills ({employee.skills.length})</h2>
        <SkillChips skills={employee.skills} />
      </div>

      <div className="details-section">
        <h2>CV Documents ({employee.cvDocuments.length})</h2>
        {employee.cvDocuments.length > 0 ? (
          <div className="cv-list">
            {employee.cvDocuments.map((doc) => (
              <div key={doc.id} className="cv-item">
                <h4>Document #{doc.id}</h4>
                <p><strong>Added:</strong> {new Date(doc.createdAt).toLocaleString()}</p>
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
