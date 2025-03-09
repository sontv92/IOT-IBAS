import { Injectable } from '@angular/core';
import { Observable ,  Subject } from 'rxjs';



@Injectable({
	providedIn: 'root'
})
export class CallCategoryFunctionService {

	private subject = new Subject<any>();

	//Gửi thông tin gọi đến hàm với TypeAction 1 = thêm mới, 2 = sửa , 3 = xóa, 4 = lưu thông tin sắp xếp
	sendAction(CategoryId: number, TypeAction: Number) {
		this.subject.next({ CategoryId: CategoryId, TypeAction: TypeAction });
	}

	//Nhận thông tin là hàm đã đc gọi
	getAction(): Observable<any> {
		return this.subject.asObservable();
	}

}
