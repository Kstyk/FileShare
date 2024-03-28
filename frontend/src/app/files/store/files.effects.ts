import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import {
  actionFail,
  getFilesComplete,
  getFilesStart,
  uploadComplete,
  uploadStart,
} from './files.actions';
import { catchError, map, of, pipe, switchMap, tap } from 'rxjs';
import { FileModel } from '../models/file.model';

const handleError = (errorRes: any) => {
  let errorMessage = 'An unknown error occurred!';

  if (!errorRes.error && !errorRes.error.error) {
    return of(
      actionFail({
        payload: { error: errorMessage },
      })
    );
  }
  switch (errorRes.status) {
    case 401:
      errorMessage = errorRes.error;
      break;
    case 400:
      break;
    default:
      break;
  }

  return of(
    actionFail({
      payload: { error: errorMessage },
    })
  );
};

@Injectable()
export class FilesEffects {
  constructor(
    private http: HttpClient,
    private actions$: Actions,
    private router: Router
  ) {}

  uploadStart = createEffect(() =>
    this.actions$.pipe(
      ofType(uploadStart),
      switchMap((uploadData) => {
        var fd = new FormData();

        uploadData.payload.files.forEach((file: File) => {
          fd.append('files', file);
        });

        return this.http
          .post('https://localhost:7041/api/files/upload-multiple', fd, {
            responseType: 'json',
          })
          .pipe(
            map((resData: FileModel[]) => {
              return uploadComplete({
                payload: { files: resData },
              });
            }),
            catchError((errorRes) => {
              return handleError(errorRes);
            })
          );
      })
    )
  );

  getFilesStart = createEffect(() =>
    this.actions$.pipe(
      ofType(getFilesStart),
      switchMap(() => {
        return this.http.get('https://localhost:7041/api/files').pipe(
          map((resData: FileModel[]) => {
            return getFilesComplete({
              payload: { files: resData },
            });
          }),
          catchError((errorRes) => {
            return handleError(errorRes);
          })
        );
      })
    )
  );

  successRedirect = createEffect(
    () =>
      this.actions$.pipe(
        ofType(uploadComplete),
        tap(() => {
          console.log('here');
          this.router.navigate(['/files']);
        })
      ),
    { dispatch: false }
  );
}
