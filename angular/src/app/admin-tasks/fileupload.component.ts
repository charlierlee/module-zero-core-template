import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FileServiceProxy, FileParameter } from '@shared/service-proxies/service-proxies';
import { Observable, throwError as _observableThrow, of as _observableOf } from 'rxjs';
@Component({
    selector: 'app-fileupload',
    templateUrl: './fileupload.component.html',
    styleUrls: ['./fileupload.component.css']
})
export class FileUploadComponent extends AppComponentBase {
    input:FileInfo = new FileInfo();
    constructor(
        injector: Injector,
        private _fileappService: FileServiceProxy,
    ) {
        super(injector);
    }
    
    upload(event): Promise<boolean> {
        return new Promise<boolean>((resolve, reject) => {
            this.input.fileName = "AbpCompanyName.AbpProjectName.db";
            this.input.data = event.files[0]
            this._fileappService.upload(this.input).subscribe(() => {
                resolve(true);
            });
        });
    }
}
export class FileInfo implements FileParameter {
    fileName: string;
    data:any
}