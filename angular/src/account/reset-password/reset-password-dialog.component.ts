import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { AppComponentBase } from '@shared/app-component-base';
import { AccountServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppTenantAvailabilityState } from '@shared/AppEnums';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';

@Component({
    templateUrl: './reset-password-dialog.component.html',
})
export class ResetPasswordDialogComponent extends AppComponentBase {
    saving = false;
    userNameOrEmailAddress = '';
    modalRef: BsModalRef;
    constructor(
        injector: Injector,
        private _accountService: AccountServiceProxy,
        private modalService: BsModalService
        //private _dialogRef: MatDialogRef<ResetPasswordDialogComponent>
    ) {
        super(injector);
    }

    save(): void {
        this.saving = true;
        this._accountService.sendPasswordResetEmail(this.userNameOrEmailAddress).subscribe((result) => {
            this.notify.info(
                this.l('PasswordResetEmailSentSucessfullyMessage'),
                this.l('PasswordResetEmailSentSucessfullyTitle'));
            this.saving = false;
            this.close();
        });
    }

    close(result?: any): void {
        this.modalService.hide(1);
        //this.modalRef.close(result);
    }
}