1. NỘI DUNG: Tích hợp thanh toán qua VNPAY trên POS
	- Thanh toán bằng thẻ Card.
	- Thanh toán bằng quét QRCode.
	- Tra cứu kết quả giao dịch.

2. CHÚ Ý: 
	- Nếu Addon dùng trên POS cần add thêm reference CXSRetailPOS.exe trong folder C:\Program Files (x86)\CitiXsys\iVend Retail\PointOfSale
	- Khi đóng gói sản phẩm cẩn zip 03 file: AddonVNPAY.dll, CXSRetailPOS.exe, RestSharp.dll
	- Trong màn hình MC Addon Management, chọn Store sẽ áp dụng Addon này.
	- Coppy folder cấu hình "Models" để vào trong đường dẫn C:\Program Files (x86)\CitiXsys\iVend Retail\PointOfSale
