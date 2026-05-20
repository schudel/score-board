import {Component, Inject, OnInit} from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface DialogData {
  title: string;
  message: string;
  okButtonText: string;
  cancelButtonText: string;
}

@Component({
  selector: 'app-modal-dialog',
  templateUrl: './modal-dialog.component.html',
  styleUrls: ['./modal-dialog.component.scss']
})
export class ModalDialogComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<ModalDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {
    // set default values
    if (!data.okButtonText) {
      data.okButtonText = $localize`:@@okButtonText:Ok`;
    }
    if (!data.cancelButtonText) {
      data.cancelButtonText = $localize`:@@cancelButtonText:Cancel`;
    }
  }

  ngOnInit() {
  }

  onOkClick() {
    this.dialogRef.close('ok');
  }

  onCancelClick(): void {
    this.dialogRef.close('cancel');
  }
}
