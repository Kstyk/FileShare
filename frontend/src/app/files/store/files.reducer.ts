import { createReducer, on } from '@ngrx/store';
import { uploadComplete, uploadFail, uploadStart } from './files.actions';

export type filesStateType = {
  files: File[];
  loading: boolean;
  error: string;
};

const initialState: filesStateType = {
  files: [],
  loading: false,
  error: null,
};

export const filesReducer = createReducer(
  initialState,
  on(uploadStart, (state) => {
    return { ...state, loading: true, error: null };
  }),
  on(uploadFail, (state, action) => {
    return { ...state, loading: false, error: action.payload.error };
  }),
  on(uploadComplete, (state) => {
    return { ...state, loading: false, error: null };
  })
);
