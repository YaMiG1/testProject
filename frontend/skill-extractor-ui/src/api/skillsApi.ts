/**
 * Skills API - CRUD operations for skills
 */

import { fetchJson } from './client';
import type { SkillDto, CreateSkillDto, UpdateSkillDto } from './types';

export const skillsApi = {
  /**
   * Get all skills
   */
  list: async (): Promise<SkillDto[]> => {
    return fetchJson<SkillDto[]>('/api/skills');
  },

  /**
   * Create a new skill
   */
  create: async (dto: CreateSkillDto): Promise<SkillDto> => {
    return fetchJson<SkillDto>('/api/skills', {
      method: 'POST',
      body: JSON.stringify(dto),
    });
  },

  /**
   * Update an existing skill
   */
  update: async (id: number, dto: UpdateSkillDto): Promise<SkillDto> => {
    return fetchJson<SkillDto>(`/api/skills/${id}`, {
      method: 'PUT',
      body: JSON.stringify(dto),
    });
  },

  /**
   * Delete a skill
   */
  remove: async (id: number): Promise<void> => {
    await fetchJson<void>(`/api/skills/${id}`, {
      method: 'DELETE',
    });
  },
};
