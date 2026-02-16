/**
 * API Types - TypeScript interfaces matching backend DTOs
 */

export interface SkillDto {
  id: number;
  name: string;
  aliases?: string | null;
}

export interface CreateSkillDto {
  name: string;
  aliases?: string | null;
}

export interface UpdateSkillDto {
  name: string;
  aliases?: string | null;
}

export interface EmployeeListItemDto {
  id: number;
  fullName: string;
  email?: string | null;
  createdAt: string;
  skillsCount: number;
}

export interface CvDocumentDto {
  id: number;
  createdAt: string;
  preview: string;
}

export interface EmployeeDetailsDto {
  id: number;
  fullName: string;
  email?: string | null;
  createdAt: string;
  skills: SkillDto[];
  cvDocuments: CvDocumentDto[];
}

export interface ExtractRequestDto {
  fullName: string;
  email?: string | null;
  rawText: string;
}

export interface ExtractResponseDto {
  employeeId: number;
  extractedSkills: SkillDto[];
}
