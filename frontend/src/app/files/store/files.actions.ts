import { createAction, props } from '@ngrx/store';
import { FileModel } from '../models/file.model';

export const UPLOAD_START = '[Files] Upload Start';
export const ACTION_FAIL = '[Files] Action Fail';
export const UPLOAD_COMPLETE = '[Files] Upload Complete';

export const GET_FILES_START = '[Files] Get Files Start';
export const GET_FILES_COMPLETE = '[Files] Get Files Complete';

export const uploadStart = createAction(
  UPLOAD_START,
  props<{ payload: { files: File[] } }>()
);

export const actionFail = createAction(
  ACTION_FAIL,
  props<{ payload: { error: string } }>()
);
export const uploadComplete = createAction(
  UPLOAD_COMPLETE,
  props<{ payload: { files: FileModel[] } }>()
);

export const getFilesStart = createAction(GET_FILES_START);

export const getFilesComplete = createAction(
  GET_FILES_COMPLETE,
  props<{ payload: { files: FileModel[] } }>()
);
