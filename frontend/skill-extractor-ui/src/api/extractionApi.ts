/**
 * Extraction API - CV skill extraction
 */

import { fetchJson } from './client';
import type { ExtractRequestDto, ExtractResponseDto } from './types';

export const extractionApi = {
  /**
   * Extract skills from raw CV text
   */
  extract: async (dto: ExtractRequestDto): Promise<ExtractResponseDto> => {
    return fetchJson<ExtractResponseDto>('/api/extraction/extract', {
      method: 'POST',
      body: JSON.stringify(dto),
    });
  },
};
