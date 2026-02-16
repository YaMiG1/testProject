import React, { useState } from 'react';
import './NewCV.css';
import { extractionApi } from '../api';

interface ExtractResult {
  employeeId: number;
  skills: Array<{ id: number; name: string }>;
}

const NewCV: React.FC = () => {
  const [fullName, setFullName] = useState('');
  const [email, setEmail] = useState('');
  const [cvText, setCvText] = useState('');
  const [loading, setLoading] = useState(false);
  const [result, setResult] = useState<ExtractResult | null>(null);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setResult(null);

    try {
      const data = await extractionApi.extract({ fullName, email: email || null, rawText: cvText });
      // map to existing local shape
      setResult({ employeeId: data.employeeId, skills: data.extractedSkills.map(s => ({ id: s.id, name: s.name })) });
      setFullName('');
      setEmail('');
      setCvText('');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="new-cv-page">
      <h1>Extract Skills from CV</h1>
      <form className="cv-form" onSubmit={handleSubmit}>
        <div className="form-group">
          <label htmlFor="fullName">Full Name *</label>
          <input
            id="fullName"
            type="text"
            placeholder="John Doe"
            value={fullName}
            onChange={(e) => setFullName(e.target.value)}
            required
          />
        </div>

        <div className="form-group">
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            placeholder="john@example.com"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </div>

        <div className="form-group">
          <label htmlFor="cvText">CV Text *</label>
          <textarea
            id="cvText"
            placeholder="Paste your CV text here..."
            rows={10}
            value={cvText}
            onChange={(e) => setCvText(e.target.value)}
            required
          />
        </div>

        <button type="submit" className="submit-btn" disabled={loading}>
          {loading ? 'Extracting...' : 'Extract Skills'}
        </button>
      </form>

      {error && <div className="error-message">{error}</div>}

      {result && (
        <div className="result-section">
          <h2>Extraction Result</h2>
          <p>Employee ID: <strong>{result.employeeId}</strong></p>
          <h3>Extracted Skills:</h3>
          {result.skills.length > 0 ? (
            <ul className="skills-list">
              {result.skills.map((skill) => (
                <li key={skill.id}>{skill.name}</li>
              ))}
            </ul>
          ) : (
            <p>No skills matched.</p>
          )}
        </div>
      )}
    </div>
  );
};

export default NewCV;
