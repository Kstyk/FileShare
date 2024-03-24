import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-upload-file',
  templateUrl: './upload-file.component.html',
  styleUrl: './upload-file.component.scss',
})
export class UploadFileComponent implements OnInit {
  uploadFileForm: FormGroup;

  constructor() {}

  ngOnInit(): void {
    this.initForm();
  }

  onSubmit() {
    console.log(this.uploadFileForm.value);
  }

  private initForm() {
    this.uploadFileForm = new FormGroup({
      file: new FormControl(null, [Validators.required]),
    });
  }
}
