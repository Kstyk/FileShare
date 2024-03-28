import { Component, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { AppStateType } from '../../store/app.reducer';
import { uploadStart } from '../store/files.actions';

@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrl: './upload-file.component.scss',
})
export class UploadFileComponent implements OnInit {
  uploadFileForm: FormGroup;
  filesArray: File[];

  constructor(private store: Store<AppStateType>) {}

  ngOnInit(): void {
    this.initForm();
  }

  onSubmit() {
    if (
      this.uploadFileForm.valid &&
      this.filesArray &&
      this.filesArray.length > 0
    ) {
      // Perform any additional validation or processing if needed

      // Dispatch the uploadStart action with the file payload
      const copyArray = Object.assign([], this.filesArray);
      this.store.dispatch(uploadStart({ payload: { files: copyArray } }));
    }
  }

  onChangeFiles(event: any) {
    this.filesArray = event.target.files;
  }

  private initForm() {
    this.uploadFileForm = new FormGroup({
      file: new FormControl([], [Validators.required]),
    });
  }
}
