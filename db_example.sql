-- LANGUAGE
INSERT INTO LANGUAGE (language_code, language_name) VALUES
('vi', N'Tiếng Việt'),
('en', N'English'),
('jp', N'日本語');

-- POI_CATEGORY
INSERT INTO POI_CATEGORY (category_name) VALUES
(N'Nhà hàng'),
(N'Quán cà phê'),
(N'Địa điểm du lịch'),
(N'Quán ăn đường phố');

-- POI
INSERT INTO POI (name, latitude, longitude, radius, priority, image_url, map_link, category_id)
VALUES
(N'Phở Hòa Pasteur', 10.7812, 106.6992, 50, 1, 'phohoa.jpg', 'https://maps.google.com/?q=10.7812,106.6992', 1),
(N'The Coffee House', 10.7765, 106.7000, 50, 2, 'coffeehouse.jpg', 'https://maps.google.com/?q=10.7765,106.7000', 2),
(N'Nhà thờ Đức Bà', 10.7798, 106.6990, 100, 5, 'ducba.jpg', 'https://maps.google.com/?q=10.7798,106.6990', 3),
(N'Bánh mì Huỳnh Hoa', 10.7705, 106.6930, 50, 3, 'banhmi.jpg', 'https://maps.google.com/?q=10.7705,106.6930', 4);

-- AUDIO_FILE
INSERT INTO AUDIO_FILE (file_path, duration, file_size)
VALUES
('audio/phohoa_vi.mp3', 120, 2048),
('audio/phohoa_en.mp3', 110, 1980),
('audio/ducba_vi.mp3', 180, 3050);

-- POI_CONTENT
INSERT INTO POI_CONTENT (poi_id, language_code, title, description, audio_id)
VALUES
(2, 'vi', N'Phở Hòa Pasteur', N'Quán phở nổi tiếng tại Sài Gòn.', 1),
(2, 'en', N'Pho Hoa Pasteur', N'Famous pho restaurant in Saigon.', 2),
(4, 'vi', N'Nhà thờ Đức Bà', N'Công trình kiến trúc cổ nổi tiếng.', 3);

-- APP_USER
INSERT INTO APP_USER (username, email, password_hash, avatar_url)
VALUES
('user1', 'user1@gmail.com', 'hash123', 'avatar1.png'),
('user2', 'user2@gmail.com', 'hash456', 'avatar2.png');

-- USER_LANGUAGE
INSERT INTO USER_LANGUAGE (user_id, language_code)
VALUES
(1, 'vi'),
(1, 'en'),
(2, 'en');

-- USER SETTING
INSERT INTO USER_SETTING (user_id, auto_play, voice_type, gps_accuracy, notification_enabled, offline_mode)
VALUES
(1, 1, 'female', 10, 1, 0),
(2, 0, 'male', 15, 1, 1);

-- DEVICE
INSERT INTO DEVICE (user_id, os, app_version, last_login)
VALUES
(1, 'Android', '1.0.0', GETDATE()),
(2, 'iOS', '1.0.1', GETDATE());

-- PLAY_LOG
INSERT INTO PLAY_LOG (user_id, poi_id, listen_duration)
VALUES
(1, 1, 100),
(1, 3, 150),
(2, 2, 80);

-- USER HISTORY
INSERT INTO USER_HISTORY (user_id, poi_id)
VALUES
(1, 1),
(1, 3),
(2, 4);

-- USER FAVORIATE
INSERT INTO USER_FAVORITE (user_id, poi_id)
VALUES
(1, 1),
(1, 3),
(2, 2);

SELECT * FROM POI
SELECT * FROM POI_CONTENT
SELECT * FROM APP_USER
SELECT * FROM AUDIO_FILE