import React from 'react';
import type { SkillDto } from '../api/types';

interface Props {
  skills: SkillDto[];
}

const chipStyle: React.CSSProperties = {
  display: 'inline-block',
  padding: '4px 10px',
  margin: '4px 6px 4px 0',
  borderRadius: 16,
  background: '#eef2ff',
  color: '#1f2937',
  fontSize: 12,
};

const containerStyle: React.CSSProperties = {
  display: 'flex',
  flexWrap: 'wrap',
  gap: 4,
  alignItems: 'center',
};

const SkillChips: React.FC<Props> = ({ skills }) => {
  if (!skills || skills.length === 0) return <div>No skills extracted yet</div>;

  return (
    <div style={containerStyle}>
      {skills.map((s) => (
        <div key={s.id} style={chipStyle} aria-label={`skill-${s.name}`}>
          {s.name}
        </div>
      ))}
    </div>
  );
};

export default SkillChips;
