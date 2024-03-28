// Create a resolver service to fetch the files from the server dispatching an action to get the files from the store. Execute only when the files are not already loaded in the store and user is logged in.

import { Injectable, inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  Resolve,
  ResolveFn,
  RouterStateSnapshot,
} from '@angular/router';
import { Store } from '@ngrx/store';
import { AppStateType } from '../store/app.reducer';
import { getFilesStart } from './store/files.actions';
import { map, take } from 'rxjs/operators';
import { FileModel } from './models/file.model';
import { Observable } from 'rxjs';

export const filesResolver: ResolveFn<Observable<void>> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
) => {
  const store = inject(Store<AppStateType>);
  console.log('im in resolver');
  return store.select('auth').pipe(
    take(1),
    map((authState) => {
      if (authState.user) {
        store.dispatch(getFilesStart());
      }
    })
  );
};
