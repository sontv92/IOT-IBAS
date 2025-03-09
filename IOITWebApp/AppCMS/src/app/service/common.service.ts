import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CommonService {
	public domainImage: string = "https://localhost:44304/";

  constructor() { }

  ConvertUrl(str) {
    str = str.toLowerCase();
    str = str.replace(/á|à|ả|ã|ạ|â|ấ|ầ|ẩ|ẫ|ậ|ă|ắ|ằ|ẳ|ẵ|ặ"/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    str = str.replace(/[^a-zA-Z0-9 ]/g, "");
    str = str.replace(/ + /g, " ");
    str = str.trim();
    str = str.replace(/ /g, "-");

    return str;
  }

  CheckAccessKey(Str: string, Code: string) {
    let Arr = [];
    Arr = Str.split('-');
    for (let i = 0; i < Arr.length; i++) {
      let ConvertArr = Arr[i].split(':');
      if (Code == ConvertArr[0]) {
        let check = ConvertArr[1].substr(0, 1);
        if (check == "1") {
          return true;
        }
        else {
          return false;
        }
      }
    }

    return false;
  }

  ConvertDateTime(obj: Date) {
    return obj.getFullYear() + "-" + (obj.getMonth() + 1) + "-" + obj.getDate() + " " + obj.getHours() + ":" + obj.getMinutes() + ":" + obj.getSeconds();
  }

  //Hàm đọc dữ liệu từ dạng cha con html => json truyền vào id của noda cha
  ConvertHtmlToJson(Arr, CurrentNode, Selector, ParentId, lct) {
    let slt = Selector + " > ol";
    let ol = CurrentNode.getElementsByTagName("ol");
    if (ol.length > 0) {
      slt = slt + " > li";
      let li = ol[0].querySelectorAll(slt);
      if (li.length > 0) {
        for (let i = 0; i < li.length; i++) {
          let Id = li[i].getAttribute("data-id");
          let Name = li[i].getAttribute("data-name");
          let PrtId = ParentId;

          Arr.push({ CategoryId: Id, Name: Name, CategoryParentId: PrtId, Location: lct });
          lct++;
          this.ConvertHtmlToJson(Arr, li[i], slt, Id, lct);
        }
      }
    }
  }
}
