export interface FileModel {
  id: number;
  name: string;
  path: string;
  uploadedAt: Date;
  downloads: number;
  isPublic: boolean;
}
