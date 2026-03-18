Create Database SpotFoodApp
Use SpotFoodApp

-------- 
---------
-- Nhóm địa điểm
CREATE TABLE POI_CATEGORY (
    category_id INT PRIMARY KEY IDENTITY,
    category_name NVARCHAR(100) NOT NULL
);

CREATE TABLE POI (
    poi_id INT PRIMARY KEY IDENTITY,
    name NVARCHAR(255) NOT NULL,
    latitude FLOAT NOT NULL,
    longitude FLOAT NOT NULL,
    radius FLOAT DEFAULT 50,
    priority INT DEFAULT 0,
    image_url NVARCHAR(500),
    map_link NVARCHAR(500),

    category_id INT,
    created_at DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (category_id) REFERENCES POI_CATEGORY(category_id)
);

CREATE INDEX idx_poi_location ON POI(latitude, longitude);

-- Nhóm nội dung thuyết minh 
CREATE TABLE LANGUAGE (
    language_code NVARCHAR(10) PRIMARY KEY,
    language_name NVARCHAR(50)
);

CREATE TABLE AUDIO_FILE (
    audio_id INT PRIMARY KEY IDENTITY,
    file_path NVARCHAR(500),
    duration INT,
    file_size INT,
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE POI_CONTENT (
    content_id INT PRIMARY KEY IDENTITY,
    poi_id INT NOT NULL,
    language_code NVARCHAR(10),
    title NVARCHAR(255),
    description NVARCHAR(MAX),
    audio_id INT,

    FOREIGN KEY (poi_id) REFERENCES POI(poi_id),
    FOREIGN KEY (language_code) REFERENCES LANGUAGE(language_code),
    FOREIGN KEY (audio_id) REFERENCES AUDIO_FILE(audio_id),

    CONSTRAINT UQ_POI_LANG UNIQUE (poi_id, language_code)
);

-- Nhóm User
CREATE TABLE APP_USER (
    user_id INT PRIMARY KEY IDENTITY,
    username NVARCHAR(100),
    email NVARCHAR(150) UNIQUE,
    password_hash NVARCHAR(255),
    avatar_url NVARCHAR(500),
    created_at DATETIME DEFAULT GETDATE()
);

CREATE TABLE USER_LANGUAGE (
    id INT PRIMARY KEY IDENTITY,
    user_id INT,
    language_code NVARCHAR(10),

    FOREIGN KEY (user_id) REFERENCES APP_USER(user_id),
    FOREIGN KEY (language_code) REFERENCES LANGUAGE(language_code)
);

CREATE TABLE USER_SETTING (
    setting_id INT PRIMARY KEY IDENTITY,
    user_id INT UNIQUE,

    auto_play BIT DEFAULT 1,
    voice_type NVARCHAR(50),
    gps_accuracy FLOAT DEFAULT 10,
    notification_enabled BIT DEFAULT 1,
    offline_mode BIT DEFAULT 0,

    FOREIGN KEY (user_id) REFERENCES APP_USER(user_id)
);

CREATE TABLE DEVICE (
    device_id INT PRIMARY KEY IDENTITY,
    user_id INT,
    os NVARCHAR(50),
    app_version NVARCHAR(50),
    last_login DATETIME,

    FOREIGN KEY (user_id) REFERENCES APP_USER(user_id)
);

-- nhóm hệ thống hoạt động
CREATE TABLE PLAY_LOG (
    log_id INT PRIMARY KEY IDENTITY,
    user_id INT,
    poi_id INT,
    played_time DATETIME DEFAULT GETDATE(),
    listen_duration INT,

    FOREIGN KEY (user_id) REFERENCES APP_USER(user_id),
    FOREIGN KEY (poi_id) REFERENCES POI(poi_id)
);

CREATE TABLE USER_HISTORY (
    history_id INT PRIMARY KEY IDENTITY,
    user_id INT,
    poi_id INT,
    visit_time DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (user_id) REFERENCES APP_USER(user_id),
    FOREIGN KEY (poi_id) REFERENCES POI(poi_id)
);

CREATE TABLE USER_FAVORITE (
    favorite_id INT PRIMARY KEY IDENTITY,
    user_id INT,
    poi_id INT,
    saved_time DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (user_id) REFERENCES APP_USER(user_id),
    FOREIGN KEY (poi_id) REFERENCES POI(poi_id),

    CONSTRAINT UQ_USER_POI UNIQUE (user_id, poi_id)
);

-- nhóm offline 
CREATE TABLE OFFLINE_PACKAGE (
    package_id INT PRIMARY KEY IDENTITY,
    name NVARCHAR(255),
    version NVARCHAR(50),
    size INT,
    download_url NVARCHAR(500)
);

CREATE TABLE OFFLINE_POI (
    id INT PRIMARY KEY IDENTITY,
    package_id INT,
    poi_id INT,

    FOREIGN KEY (package_id) REFERENCES OFFLINE_PACKAGE(package_id),
    FOREIGN KEY (poi_id) REFERENCES POI(poi_id),

    CONSTRAINT UQ_PACKAGE_POI UNIQUE (package_id, poi_id)
);

