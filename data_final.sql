
	USE master;
	GO
	-- Xóa nếu đang tồn tại để tạo mới sạch sẽ
	IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SpotFoodApp')
	BEGIN
		ALTER DATABASE SpotFoodApp SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
		DROP DATABASE SpotFoodApp;
	END
	GO

	CREATE DATABASE SpotFoodApp;
	GO
	USE SpotFoodApp;
	GO

	select * from POI;
	select * from POI_CATEGORY;
	select * from POI_CONTENT;

	select * from POI_TRANSLATION;

	SELECT * FROM POI_TRANSLATION ORDER BY poi_id, language_code;

	-- 1. Bảng Danh mục
	CREATE TABLE POI_CATEGORY (
		category_id   INT PRIMARY KEY IDENTITY(1,1),
		category_name NVARCHAR(255) NOT NULL,
		created_at    DATETIME DEFAULT GETDATE()
	);

	-- 2. Bảng File âm thanh
	CREATE TABLE AUDIO_FILE (
		audio_id    INT PRIMARY KEY IDENTITY(1,1),
		file_path   NVARCHAR(MAX) NOT NULL,
		duration    INT NULL,                    -- giây
		file_size   BIGINT NULL,                  -- byte, dùng BIGINT tốt hơn FLOAT
		created_at  DATETIME DEFAULT GETDATE()
	);

	-- 3. Bảng Điểm quan tâm (POI)
	CREATE TABLE POI (
		poi_id       INT PRIMARY KEY IDENTITY(1,1),
		name         NVARCHAR(255) NOT NULL,       -- Tên gốc (tiếng Việt)
		latitude     FLOAT NOT NULL,
		longitude    FLOAT NOT NULL,
		image_url    NVARCHAR(MAX) NULL,
		category_id  INT NULL,
		address      NVARCHAR(500) NULL,
		map_link     NVARCHAR(MAX) NULL,
		created_at   DATETIME DEFAULT GETDATE(),
    
		CONSTRAINT FK_POI_Category FOREIGN KEY (category_id) 
			REFERENCES POI_CATEGORY(category_id) ON DELETE SET NULL
	);

	-- Index cho tìm kiếm gần
	CREATE NONCLUSTERED INDEX IX_POI_Location ON POI(latitude, longitude);
	CREATE NONCLUSTERED INDEX IX_POI_Category ON POI(category_id);

	-- 4. Bảng Nội dung chi tiết (giữ audio)
	CREATE TABLE POI_CONTENT (
		content_id   INT PRIMARY KEY IDENTITY(1,1),
		poi_id       INT NOT NULL,
		title        NVARCHAR(255) NULL,
		description  NVARCHAR(MAX) NULL,
		audio_id     INT NULL,
    
		CONSTRAINT FK_Content_POI FOREIGN KEY (poi_id) 
			REFERENCES POI(poi_id) ON DELETE CASCADE,
    
		CONSTRAINT FK_Content_Audio FOREIGN KEY (audio_id) 
			REFERENCES AUDIO_FILE(audio_id) ON DELETE SET NULL
	);

	-- 5. Bảng Dịch (ĐÃ TỐI ƯU)
	CREATE TABLE POI_TRANSLATION (
		translation_id   INT PRIMARY KEY IDENTITY(1,1),
		poi_id           INT NOT NULL,
		language_code    VARCHAR(10) NOT NULL,        -- vi, en, ja, fr, ko, zh...
		name             NVARCHAR(255) NULL,
		description      NVARCHAR(MAX) NULL,
		address          NVARCHAR(500) NULL,
		created_at       DATETIME DEFAULT GETDATE(),
		updated_at       DATETIME NULL,

		CONSTRAINT FK_Translation_POI FOREIGN KEY (poi_id) 
			REFERENCES POI(poi_id) ON DELETE CASCADE,

		-- Ngăn chặn trùng ngôn ngữ cho cùng POI
		CONSTRAINT UQ_POI_Language UNIQUE (poi_id, language_code)
	);

	-- Index cho query nhanh theo ngôn ngữ
	CREATE NONCLUSTERED INDEX IX_POI_TRANSLATION_Language ON POI_TRANSLATION(language_code);
	CREATE NONCLUSTERED INDEX IX_POI_TRANSLATION_POI ON POI_TRANSLATION(poi_id);


	INSERT INTO POI_CATEGORY (category_name) VALUES
	(N'Nhà Hàng'),
	(N'Quán ăn đường phố'),
	(N'Quán Cafe'),
	(N'Khách sạn'),
	(N'Chùa');

	INSERT INTO POI (name, latitude, longitude, image_url, category_id, address, map_link)
	VALUES 
	(N'Ốc Oanh', 10.760888032143251, 106.70327573645312, '/img/poi_Pics/oc_oanh.jpg', 2, N'534 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/awpmKSUsMFP4nRXw5'),
	(N'Sườn Muối Ớt Quận 4', 10.76099475346122, 106.70362955179596, '/img/poi_Pics/suon_muoi_ot.png', 2, N'712 Đ. Vĩnh Khánh, Phường 10, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/3k5ZopvpaX7eTMjL7'),
	(N'Lẩu Nướng Thuận Việt', 10.76095446729409, 106.70304693280792, '/img/poi_Pics/lau_nuong.jpg', 1, N'424 Đ. Vĩnh Khánh, phường 8, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/cpA4YydQJJBW3L2Y7'),
	(N'Bún Thịt Nướng Cô Nga', 10.760832222731903, 106.70682350202136, '/img/poi_Pics/bun_thit_nuong_co_nga.jpg', 2, N'14 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/daUVL8A7HYkjvC84A'),
	(N'Link Coffee & Tea', 10.760910989104278, 106.70492286328614, '/img/poi_Pics/link_coffee_tea.png', 3, N'522 Đ. Vĩnh Khánh, Khánh Hội, Hồ Chí Minh 700000, Việt Nam', 'https://maps.app.goo.gl/kU8rnqYiPhBpz5ko6'),
	(N'Hotel M&H cinema', 10.761174466305755, 106.70583059093926, '/img/poi_Pics/Hotel_M&H_cinema.jpg', 4, N'779 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, Hồ Chí Minh 70000, Việt Nam', 'https://maps.app.goo.gl/W9ECogkHFp3LsqjA7'),
	(N'Phật Bửu Tự', 10.762353293083697, 106.70226673645318, '/img/poi_Pics/phat_buu_tu.jpg', 5, N'239 Hoàng Diệu, phường 8, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/WNcTThKnGQzNdhgG6'),
	(N'Thèm Nướng YAKINIKU', 10.7607991612774, 106.70473007496572, '/img/poi_Pics/nuong_yakiniku.png', 1, N'122 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, Hồ Chí Minh 70000, Việt Nam', 'https://maps.app.goo.gl/CuGDm44NxXe7wbZx5'),
	(N'Nem Nướng Đặc Sản Quê Nhà', 10.760930751918627, 106.704976111642, '/img/poi_Pics/nem_nuong.jpg', 2, N'122/45 Đ. Vĩnh Khánh, Phường 10, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/DZzbY1X3e1Q98jMh9'),
	(N'Quán Nước SINZIEN', 10.761758483294047, 106.70226741581305, '/img/poi_Pics/quan_nuoc.jpg', 3, N'375 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, Hồ Chí Minh, Việt Nam', 'https://maps.app.goo.gl/V2uMbdESkT2c1JSB6'),
	(N'Lãng Quán', 10.7611408964303, 106.70542035250088, '/img/poi_Pics/lang_quan.jpg', 1, N'531 Đ. Vĩnh Khánh, Phường 10, Khánh Hội, Hồ Chí Minh 700000, Việt Nam', 'https://maps.app.goo.gl/1ZEt5DjGdENcJJpW7');


	-- 1. Nội dung cho Ốc Oanh
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Ốc Oanh Vĩnh Khánh', 
	N'Ốc Oanh là một trong những quán ăn biểu tượng và nhộn nhịp nhất trên con phố ẩm thực Vĩnh Khánh. Quán nổi tiếng với các món hải sản tươi sống được chế biến đậm đà, đặc biệt là món ốc hương rang muối ớt và càng cúm núm. Không gian quán rộng rãi, mở lộ thiên giúp thực khách vừa thưởng thức món ăn vừa tận hưởng không khí sôi động của phố phường. Đây là địa điểm lý tưởng cho các buổi tụ tập bạn bè vào buổi tối cuối tuần. Phục vụ tại đây khá nhanh nhẹn dù lượng khách đổ về mỗi đêm cực kỳ đông đúc.'
	FROM POI WHERE name = N'Ốc Oanh';

	-- 2. Nội dung cho Sườn Muối Ớt Quận 4
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Sườn Muối Ớt Quận 4', 
	N'Quán chuyên phục vụ các món nướng tại bàn với hương vị tẩm ướp đặc trưng khó quên. Món "đinh" của quán chính là sườn non nướng muối ớt với vị cay nồng quyện cùng thịt sườn mềm ngọt. Thực khách có thể tự tay nướng thịt trên bếp than hồng, tạo cảm giác gần gũi và ấm cúng. Giá cả tại đây rất bình dân, phù hợp với túi tiền của đa số học sinh, sinh viên và người lao động. Quán cũng có nhiều món ăn kèm phong phú như salad và kim chi để chống ngán.'
	FROM POI WHERE name = N'Sườn Muối Ớt Quận 4';


	-- 4. Nội dung cho Bún Thịt Nướng Cô Nga
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Bún Thịt Nướng Cô Nga', 
	N'Bún thịt nướng Cô Nga là điểm dừng chân ăn sáng và ăn trưa quen thuộc của người dân khu vực Khánh Hội. Thịt nướng tại quán được nướng liên tục trên bếp than, tỏa mùi thơm nức cả một góc đường. Một tô bún đầy đặn bao gồm thịt nướng, chả giò giòn tan, nem nướng và rất nhiều rau sống tươi sạch. Điểm đặc biệt nhất chính là phần nước mắm pha chua ngọt rất vừa miệng, làm tôn lên vị ngon của các thành phần khác. Không gian quán giản dị, sạch sẽ mang đậm phong cách ẩm thực bình dân của Sài Gòn.'
	FROM POI WHERE name = N'Bún Thịt Nướng Cô Nga';

	-- 5. Nội dung cho Link Coffee & Tea
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Link Coffee & Tea', 
	N'Link Coffee & Tea là một không gian hiện đại và yên tĩnh hiếm hoi giữa lòng phố ẩm thực Vĩnh Khánh ồn ào. Quán được trang trí theo phong cách tối giản với ánh sáng ấm áp, rất thích hợp để làm việc hoặc học tập. Menu của quán đa dạng từ các dòng cà phê máy đến trà trái cây tươi mát giải nhiệt hiệu quả. Đội ngũ nhân viên trẻ trung, nhiệt tình luôn sẵn sàng tư vấn món uống phù hợp với khẩu vị của bạn. Đây cũng là nơi lý tưởng để bạn nghỉ chân sau một vòng dạo chơi và thưởng thức đồ ăn đường phố.'
	FROM POI WHERE name = N'Link Coffee & Tea';

	-- 6. Nội dung cho Hotel M&H cinema
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Hotel M&H Cinema', 
	N'M&H Cinema là mô hình khách sạn kết hợp giải trí độc đáo tọa lạc ngay trên trục đường Vĩnh Khánh sầm uất. Các phòng nghỉ tại đây được trang bị màn hình chiếu lớn và hệ thống âm thanh chất lượng cao để khách hàng thư giãn xem phim. Không gian phòng được thiết kế tinh tế, sạch sẽ và đảm bảo sự riêng tư tuyệt đối cho khách lưu trú. Vị trí của khách sạn cực kỳ thuận tiện cho những ai muốn khám phá ẩm thực Quận 4 về đêm mà không cần di chuyển xa. Đây là sự lựa chọn mới mẻ cho các cặp đôi hoặc nhóm bạn muốn có không gian giải trí riêng biệt.'
	FROM POI WHERE name = N'Hotel M&H cinema';

	-- 7. Nội dung cho Phật Bửu Tự
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Phật Bửu Tự', 
	N'Phật Bửu Tự là ngôi chùa cổ kính nằm trên đường Hoàng Diệu, mang lại cảm giác thanh tịnh ngay khi bước qua cổng tam quan. Kiến trúc của chùa mang đậm nét truyền thống với những đường chạm khắc tinh xảo và không gian thờ tự trang nghiêm. Người dân địa phương thường xuyên đến đây để thắp hương cầu an và tìm kiếm sự bình yên trong tâm hồn. Khuôn viên chùa có nhiều cây xanh, tạo bóng mát râm ran và không khí trong lành giữa lòng đô thị. Vào các ngày rằm hoặc lễ lớn, chùa tổ chức nhiều hoạt động tâm linh ý nghĩa thu hút đông đảo phật tử.'
	FROM POI WHERE name = N'Phật Bửu Tự';

	-- 8. Nội dung cho THÈM NƯỚNG YAKINIKU
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Thèm Nướng Yakiniku', 
	N'Thèm Nướng Yakiniku mang phong cách nướng Nhật Bản hiện đại đến với khu phố ẩm thực Quận 4. Quán sử dụng hệ thống bếp nướng không khói giúp thực khách thoải mái thưởng thức đồ nướng mà không lo ám mùi lên quần áo. Các loại thịt bò và hải sản tại đây đều được tuyển chọn kỹ lưỡng, thái lát chuẩn phong cách Yakiniku và tẩm sốt đặc chế. Không gian quán sang trọng nhưng vẫn giữ được sự ấm cúng, phù hợp cho cả tiệc gia đình lẫn gặp gỡ đối tác. Những set combo đa dạng giúp bạn có thể trải nghiệm nhiều loại thịt thượng hạng với mức chi phí hợp lý.'
	FROM POI WHERE name = N'THÈM NƯỚNG YAKINIKU';

	-- 1. Nội dung cho Lẩu Nướng Thuận Việt
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Lẩu Nướng Thuận Việt', 
	N'Lẩu Nướng Thuận Việt là điểm đến lý tưởng cho những ai yêu thích sự kết hợp giữa hương vị lẩu truyền thống và đồ nướng đậm đà. Quán sở hữu không gian mở thoáng đãng ngay mặt tiền đường Vĩnh Khánh, tạo cảm giác thoải mái cho thực khách thưởng thức bữa ăn giữa nhịp sống phố thị. Thực đơn tại đây rất phong phú với các loại thịt, hải sản tươi sống được tẩm ướp theo công thức riêng biệt của quán. Nước lẩu có vị ngọt thanh tự nhiên, quyện cùng đồ nhúng đa dạng tạo nên một bữa tiệc vị giác đầy hấp dẫn. Đây là lựa chọn hàng đầu cho các buổi liên hoan gia đình hay tụ họp bạn bè đông đúc.'
	FROM POI WHERE name = N'Lẩu Nướng Thuận Việt';

	-- 2. Nội dung cho Nem Nướng Đặc Sản Quê Nhà
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Nem Nướng Đặc Sản Quê Nhà', 
	N'Nếu bạn đang tìm kiếm hương vị quê hương dân dã giữa lòng Quận 4, Nem Nướng Đặc Sản Quê Nhà chính là câu trả lời hoàn hảo. Từng cây nem được nướng vàng ruộm trên bếp than hoa, tỏa ra mùi thơm đặc trưng khó cưỡng lan tỏa khắp con hẻm. Một phần nem nướng đầy đủ đi kèm với bánh tráng, ram giòn, các loại rau rừng tươi xanh và dưa chua giải ngấy. Điểm nhấn làm nên thương hiệu của quán chính là bát nước chấm sền sệt, đậm đà được pha chế theo bí quyết gia truyền. Quán mang đến cảm giác gần gũi, mộc mạc như chính tên gọi của mình, khiến bất kỳ thực khách nào cũng muốn quay lại.'
	FROM POI WHERE name = N'Nem Nướng Đặc Sản Quê Nhà';

	-- 3. Nội dung cho Quán Nước SINZIEN
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Quán Nước SINZIEN', 
	N'Quán Nước SINZIEN là một góc nhỏ đầy màu sắc và sự trẻ trung tọa lạc tại khu vực sầm uất của phố ẩm thực Vĩnh Khánh. Quán chuyên phục vụ các loại trà trái cây nhiệt đới, sinh tố và cà phê với phong cách pha chế hiện đại, bắt mắt. Không gian quán tuy đơn giản nhưng rất sạch sẽ, là điểm dừng chân nghỉ ngơi lý tưởng sau khi dạo quanh các hàng quán đồ ăn mặn. Đặc biệt, trà đào và trà dâu tại đây được rất nhiều bạn trẻ yêu thích nhờ vị trà đậm đà phối hợp cùng trái cây tươi mát lạnh. Nhân viên phục vụ tại SINZIEN rất nhanh nhẹn và luôn giữ thái độ niềm nở, nhiệt tình với khách hàng.'
	FROM POI WHERE name = N'Quán Nước SINZIEN';

	-- 4. Nội dung cho Lãng Quán
	INSERT INTO POI_CONTENT (poi_id, title, description)
	SELECT poi_id, N'Lãng Quán', 
	N'Lãng Quán nổi tiếng với phong cách ẩm thực nhậu bình dân nhưng chất lượng món ăn lại được đầu tư vô cùng kỹ lưỡng. Quán có vị trí đắc địa trên đường Vĩnh Khánh, thu hút đông đảo thực khách bởi các món hải sản rang muối và lẩu hải sản đầy ắp. Không gian tại đây luôn tràn đầy năng lượng với âm thanh nhộn nhịp, rất phù hợp cho những ai thích cảm giác vừa ăn uống vừa ngắm nhìn phố xá đông vui. Đội ngũ đầu bếp tại Lãng Quán có tay nghề cao, đảm bảo các món ăn mang ra luôn nóng hổi và giữ được hương vị đặc trưng. Với giá cả hợp lý và phong cách phục vụ tận tâm, quán đã trở thành địa chỉ "ruột" của nhiều tín đồ ẩm thực Sài Gòn.'
	FROM POI WHERE name = N'Lãng Quán';


	-- ================= POI_TRANSLATION - DỊCH RÚT GỌN (2-3 câu) =================

	INSERT INTO POI_TRANSLATION (poi_id, language_code, name, description, address) VALUES

	-- 1. Ốc Oanh
	((SELECT poi_id FROM POI WHERE name = N'Ốc Oanh'), 'vi', N'Ốc Oanh', 
	N'Ốc Oanh là quán ăn biểu tượng nhộn nhịp nhất phố Vĩnh Khánh. Nổi tiếng với hải sản tươi sống, đặc biệt ốc hương rang muối ớt và càng cúm núm. Không gian mở, lý tưởng cho tụ tập bạn bè buổi tối.', 
	N'534 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, TP.HCM'),

	((SELECT poi_id FROM POI WHERE name = N'Ốc Oanh'), 'en', N'Oanh Snail Restaurant', 
	N'Oanh Snail is the most iconic and bustling restaurant on Vinh Khanh Food Street. Famous for fresh seafood, especially salted chili snails and crab claws. Open-air space perfect for group gatherings at night.', 
	N'534 Vinh Khanh Street, Ward 8, Khanh Hoi, Ho Chi Minh City'),

	((SELECT poi_id FROM POI WHERE name = N'Ốc Oanh'), 'ja', N'オアン カタツムリ', 
	N'ヴィンカイン美食街で最も有名で賑やかな店です。新鮮なシーフード、特に塩唐辛子炒めのカタツムリとカニ爪が人気。夜の友人との集まりに最適なオープンエアの空間です。', 
	N'534 ヴィンカイン通り、8区、カイン・ホイ、ホーチミン市'),

	((SELECT poi_id FROM POI WHERE name = N'Ốc Oanh'), 'zh', N'蚝王', 
	N'蚝王是永庆美食街最热闹的标志性餐厅。以新鲜海鲜闻名，尤其是盐辣螺和蟹钳。露天环境适合晚上朋友聚会。', 
	N'永庆街534号，8坊，庆会，胡志明市'),


	-- 2. Sườn Muối Ớt Quận 4
	((SELECT poi_id FROM POI WHERE name = N'Sườn Muối Ớt Quận 4'), 'vi', N'Sườn Muối Ớt Quận 4', 
	N'Quán chuyên món nướng tại bàn với hương vị muối ớt đặc trưng. Món signature là sườn non nướng muối ớt cay nồng hấp dẫn. Giá cả bình dân, phù hợp nhóm bạn và sinh viên.', 
	N'712 Đ. Vĩnh Khánh, Phường 10, Khánh Hội, TP.HCM'),

	((SELECT poi_id FROM POI WHERE name = N'Sườn Muối Ớt Quận 4'), 'en', N'Salted Chili Ribs District 4', 
	N'Specializes in table-side BBQ with signature salted chili marinade. The highlight is tender pork ribs grilled with spicy-sweet flavor. Affordable prices, ideal for groups and students.', 
	N'712 Vinh Khanh Street, Ward 10, Khanh Hoi, Ho Chi Minh City'),

	((SELECT poi_id FROM POI WHERE name = N'Sườn Muối Ớt Quận 4'), 'ja', N'塩唐辛子スペアリブ 4区', 
	N'テーブルで焼く焼肉専門店。看板メニューは塩唐辛子スペアリブ。手頃な価格でグループや学生に人気です。', 
	N'ヴィンカイン通り712番地、10区、カイン・ホイ、ホーチミン市'),

	((SELECT poi_id FROM POI WHERE name = N'Sườn Muối Ớt Quận 4'), 'zh', N'盐辣排骨 第4郡', 
	N'主打桌边烧烤，特色盐辣排骨鲜嫩入味。价格亲民，适合朋友聚餐和学生。', 
	N'永庆街712号，10坊，庆会，胡志明市'),


	-- 3. Lẩu Nướng Thuận Việt
	((SELECT poi_id FROM POI WHERE name = N'Lẩu Nướng Thuận Việt'), 'vi', N'Lẩu Nướng Thuận Việt', 
	N'Kết hợp hoàn hảo giữa lẩu và nướng tại bàn. Không gian thoáng đãng, thực đơn phong phú với hải sản và thịt tươi. Phù hợp cho bữa ăn gia đình và tụ họp bạn bè.', 
	N'424 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, TP.HCM'),

	((SELECT poi_id FROM POI WHERE name = N'Lẩu Nướng Thuận Việt'), 'en', N'Thuan Viet Hotpot & Grill', 
	N'Perfect combination of hotpot and table BBQ. Spacious open space with rich menu of fresh seafood and meat. Ideal for family meals and friend gatherings.', 
	N'424 Vinh Khanh Street, Ward 8, Khanh Hoi, Ho Chi Minh City'),

	((SELECT poi_id FROM POI WHERE name = N'Lẩu Nướng Thuận Việt'), 'ja', N'トゥアン・ベト 鍋と焼肉', 
	N'鍋と焼肉の理想的な組み合わせ。開放的な空間に新鮮なシーフードと肉が豊富。家族や友人との食事に最適です。', 
	N'424 ヴィンカイン通り、8区、カイン・ホイ、ホーチミン市'),

	((SELECT poi_id FROM POI WHERE name = N'Lẩu Nướng Thuận Việt'), 'zh', N'顺越火锅烧烤', 
	N'火锅与烧烤完美结合。空间宽敞，菜单丰富新鲜海鲜和肉类。适合家庭聚餐和朋友聚会。', 
	N'永庆街424号，8坊，庆会，胡志明市'),


	-- 4. Bún Thịt Nướng Cô Nga
	((SELECT poi_id FROM POI WHERE name = N'Bún Thịt Nướng Cô Nga'), 'vi', N'Bún Thịt Nướng Cô Nga', 
	N'Quán ăn quen thuộc với bún thịt nướng thơm ngon tại Khánh Hội. Thịt nướng than liên tục, nước mắm chua ngọt vừa miệng. Không gian giản dị, sạch sẽ.', 
	N'14 Đ. Vĩnh Khánh, Phường 8, Khánh Hội, TP.HCM'),

	((SELECT poi_id FROM POI WHERE name = N'Bún Thịt Nướng Cô Nga'), 'en', N'Co Nga Grilled Meat Vermicelli', 
	N'Popular spot for delicious grilled meat vermicelli in Khanh Hoi. Freshly grilled meat and perfectly balanced sweet-sour fish sauce. Simple and clean space.', 
	N'14 Vinh Khanh Street, Ward 8, Khanh Hoi, Ho Chi Minh City'),

	((SELECT poi_id FROM POI WHERE name = N'Bún Thịt Nướng Cô Nga'), 'ja', N'コ・ガ 焼肉ビーフン', 
	N'カイン・ホイ地区で人気の焼肉ビーフン専門店。炭火で焼いた肉と絶妙な甘酸っぱいヌクマムが魅力。シンプルで清潔な店内です。', 
	N'14 ヴィンカイン通り、8区、カイン・ホイ、ホーチミン市'),

	((SELECT poi_id FROM POI WHERE name = N'Bún Thịt Nướng Cô Nga'), 'zh', N'吴姐烤肉米粉', 
	N'庆会区著名烤肉米粉店。炭火烤肉搭配酸甜鱼露。环境简单干净。', 
	N'永庆街14号，8坊，庆会，胡志明市'),


	-- 5. Link Coffee & Tea
	((SELECT poi_id FROM POI WHERE name = N'Link Coffee & Tea'), 'vi', N'Link Coffee & Tea', 
	N'Không gian hiện đại và yên tĩnh giữa phố ẩm thực sôi động. Phù hợp làm việc, học tập và nghỉ ngơi. Menu đa dạng cà phê và trà trái cây tươi.', 
	N'522 Đ. Vĩnh Khánh, Khánh Hội, TP.HCM'),

	((SELECT poi_id FROM POI WHERE name = N'Link Coffee & Tea'), 'en', N'Link Coffee & Tea', 
	N'Modern and quiet space in the middle of bustling Vinh Khanh Food Street. Great for working, studying and relaxing. Wide selection of coffee and fresh fruit tea.', 
	N'522 Vinh Khanh Street, Khanh Hoi, Ho Chi Minh City'),

	((SELECT poi_id FROM POI WHERE name = N'Link Coffee & Tea'), 'ja', N'リンク コーヒー＆ティー', 
	N'賑やかな美食街の中にあるモダンで静かな空間。仕事や勉強、休憩に最適。コーヒーと新鮮なフルーツティーが豊富です。', 
	N'522 ヴィンカイン通り、カイン・ホイ、ホーチミン市'),

	((SELECT poi_id FROM POI WHERE name = N'Link Coffee & Tea'), 'zh', N'Link咖啡茶饮', 
	N'热闹美食街中现代而安静的空间。适合工作、学习和休息。提供多种咖啡和新鲜果茶。', 
	N'永庆街522号，庆会，胡志明市')


