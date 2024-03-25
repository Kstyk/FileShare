import { createAction, props } from '@ngrx/store';

export const UPLOAD_START = '[Files] Upload Start';
export const UPLOAD_FAIL = '[Files] Upload Fail';
export const UPLOAD_COMPLETE = '[Files] Upload Complete';

export const uploadStart = createAction(
  UPLOAD_START,
  props<{ payload: { file: File } }>()
);

export const uploadFail = createAction(
  UPLOAD_FAIL,
  props<{ payload: { error: string } }>()
);
export const uploadComplete = createAction(UPLOAD_COMPLETE);
