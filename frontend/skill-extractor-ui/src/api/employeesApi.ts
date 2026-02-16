/**
 * Employees API - Operations for employees
 */

import { fetchJson } from './client';
import type { EmployeeListItemDto, EmployeeDetailsDto } from './types';

export const employeesApi = {
  /**
   * Get all employees
   */
  list: async (): Promise<EmployeeListItemDto[]> => {
    return fetchJson<EmployeeListItemDto[]>('/api/employees');
  },

  /**
   * Get employee details by ID
   */
  getDetails: async (id: number): Promise<EmployeeDetailsDto> => {
    return fetchJson<EmployeeDetailsDto>(`/api/employees/${id}`);
  },

  /**
   * Delete an employee
   */
  remove: async (id: number): Promise<void> => {
    await fetchJson<void>(`/api/employees/${id}`, {
      method: 'DELETE',
    });
  },
};
