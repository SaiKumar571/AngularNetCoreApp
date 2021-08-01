import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs/operators';
import { member, Photo } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member:any={};
  uploader:FileUploader=new FileUploader({});
  hasBaseDropzoneOver=false;
  baseUrl=environment.apiUrl;
  user:any={};
  constructor(private accountService:AccountService,private memberService:MembersService) { 
    this.accountService.currentUser$.subscribe((response)=>{ this.user = response});
    console.log(this.user.token)  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e:any){
    this.hasBaseDropzoneOver=e;
  }

  setMainPhoto(photo:any){
    this.memberService.SetMainPhoto(photo.id).subscribe(()=>{
      this.user.photoUrl=photo.url;
      this.accountService.setCurrentUser(this.user);
      this.member.photoUrl=photo.url;
      this.member.photos.forEach(function (p:any) {
          if (p.isMain) p.isMain=false;
            if(p.id===photo.id) p.isMain=true;
        });
    })
  }

  DeletePhoto(photo:any){
    this.memberService.DeletePhoto(photo.id).subscribe(()=>{
      this.member.photos=this.member.photos.filter(function (x:any) {
          return x.id != photo.id;
        })
    })
  }

  initializeUploader(){
    this.uploader=new FileUploader({
      url:this.baseUrl+'users/add-photo',
      authToken:'Bearer '+this.user.token,
      isHTML5:true,
      allowedFileType:['image'],
      removeAfterUpload:true,
      autoUpload:false,
      maxFileSize:10*1024
    });   

    
  this.uploader.onAfterAddingFile=(file)=>{
    file.withCredentials=false;
  }

  this.uploader.onSuccessItem=(item,response,status,headers)=>{
    if(response){
      const photo:Photo=JSON.parse(response);
      this.member.photos.push(photo);
      if(photo.isMain)
      {
        this.user.photoUrl=photo.url;
        this.member.photoUrl=photo.url;
        this.accountService.setCurrentUser(this.user);
      }
    }

  }
    
  }

}