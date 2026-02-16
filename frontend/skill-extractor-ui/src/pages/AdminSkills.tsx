import React, { useState, useEffect } from 'react';
import './AdminSkills.css';
import { skillsApi, ApiError } from '../api';

interface Skill {
  id: number;
  name: string;
  aliases?: string | null;
}

const AdminSkills: React.FC = () => {
  const [skills, setSkills] = useState<Skill[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [name, setName] = useState('');
  const [aliases, setAliases] = useState('');
  const [editingId, setEditingId] = useState<number | null>(null);

  useEffect(() => {
    fetchSkills();
  }, []);

  const fetchSkills = async () => {
    try {
      const data = await skillsApi.list();
      setSkills(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      try {
        if (editingId) {
          await skillsApi.update(editingId, { name, aliases: aliases || null });
        } else {
          await skillsApi.create({ name, aliases: aliases || null });
        }

        await fetchSkills();
      } catch (err) {
        if (err instanceof ApiError && err.status === 409) {
          throw new Error('Skill name already exists');
        }
        throw err;
      }
      setName('');
      setAliases('');
      setEditingId(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleEdit = (skill: Skill) => {
    setEditingId(skill.id);
    setName(skill.name);
    setAliases(skill.aliases || '');
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Delete this skill?')) return;

    try {
      await skillsApi.remove(id);
      await fetchSkills();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    }
  };

  const handleCancel = () => {
    setEditingId(null);
    setName('');
    setAliases('');
  };

  if (loading) return <div className="admin-skills"><p>Loading...</p></div>;

  return (
    <div className="admin-skills">
      <h1>Manage Skills</h1>

      {error && <div className="error-message">{error}</div>}

      <form className="skill-form" onSubmit={handleSubmit}>
        <h2>{editingId ? 'Edit Skill' : 'Add New Skill'}</h2>

        <div className="form-group">
          <label htmlFor="name">Name *</label>
          <input
            id="name"
            type="text"
            placeholder="e.g., C#, React"
            value={name}
            onChange={(e) => setName(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="aliases">Aliases (comma-separated)</label>
          <input
            id="aliases"
            type="text"
            placeholder="e.g., C Sharp, csharp"
            value={aliases}
            onChange={(e) => setAliases(e.target.value)}
          />
        </div>

        <div className="form-actions">
          <button type="submit" className="btn btn-primary">
            {editingId ? 'Update Skill' : 'Add Skill'}
          </button>
          {editingId && (
            <button type="button" className="btn btn-secondary" onClick={handleCancel}>
              Cancel
            </button>
          )}
        </div>
      </form>

      <div className="skills-grid">
        <h2>Skills List</h2>
        {skills.length === 0 ? (
          <p>No skills yet.</p>
        ) : (
          <table className="skills-table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Aliases</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {skills.map((skill) => (
                <tr key={skill.id}>
                  <td>{skill.name}</td>
                  <td>{skill.aliases || '-'}</td>
                  <td>
                    <button
                      className="btn-small btn-edit"
                      onClick={() => handleEdit(skill)}
                    >
                      Edit
                    </button>
                    <button
                      className="btn-small btn-delete"
                      onClick={() => handleDelete(skill.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default AdminSkills;
