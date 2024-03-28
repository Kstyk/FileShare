import { createReducer, on } from '@ngrx/store';
import {
  actionFail,
  getFilesComplete,
  getFilesStart,
  uploadComplete,
  uploadStart,
} from './files.actions';
import { FileModel } from '../models/file.model';

export type filesStateType = {
  files: FileModel[];
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
  on(actionFail, (state, action) => {
    return { ...state, loading: false, error: action.payload.error };
  }),
  on(uploadComplete, (state, action) => {
    return {
      ...state,
      loading: false,
      error: null,
      files: action.payload.files,
    };
  }),
  on(getFilesStart, (state) => {
    return { ...state, loading: true, error: null };
  }),

  on(getFilesComplete, (state, action) => {
    return {
      ...state,
      loading: false,
      error: null,
      files: action.payload.files,
    };
  })
);
