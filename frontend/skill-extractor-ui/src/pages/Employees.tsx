import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import './Employees.css';
import { employeesApi } from '../api';

interface Employee {
  id: number;
  fullName: string;
  email?: string;
  createdAt: string;
  skillsCount: number;
}

const Employees: React.FC = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchEmployees = async () => {
      try {
        const data = await employeesApi.list();
        const transformedData = data.map(emp => ({
          ...emp,
          email: emp.email || undefined
        }));
        setEmployees(transformedData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchEmployees();
  }, []);

  if (loading) return <div className="employees-page"><p>Loading...</p></div>;
  if (error) return <div className="employees-page error-message">{error}</div>;

  return (
    <div className="employees-page">
      <h1>Employees</h1>
      {employees.length === 0 ? (
        <p>No employees found.</p>
      ) : (
        <table className="employees-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Skills</th>
              <th>Added</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {employees.map((emp) => (
              <tr key={emp.id}>
                <td>{emp.fullName}</td>
                <td>{emp.email || '-'}</td>
                <td>{emp.skillsCount}</td>
                <td>{new Date(emp.createdAt).toLocaleDateString()}</td>
                <td>
                  <Link to={`/employees/${emp.id}`} className="action-link">
                    View
                  </Link>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default Employees;
