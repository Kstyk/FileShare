import { filesStateType } from './files.reducer';

export const selectFiles = (state: { files: filesStateType }) => state.files;
