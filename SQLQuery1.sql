CREATE TABLE Rol (
    RolID INT PRIMARY KEY IDENTITY(1,1),
    RolAdi NVARCHAR(50)
);

CREATE TABLE Departman (
    DepartmanID INT PRIMARY KEY IDENTITY(1,1),
    DepartmanAdi NVARCHAR(100)
);

CREATE TABLE Kullanici (
    KullaniciID INT PRIMARY KEY IDENTITY(1,1),
    Ad NVARCHAR(100),
    Soyad NVARCHAR(100),
    Eposta NVARCHAR(100),
    Sifre NVARCHAR(100),
    RolID INT FOREIGN KEY REFERENCES Rol(RolID),
    DepartmanID INT FOREIGN KEY REFERENCES Departman(DepartmanID)
);

CREATE TABLE IzinTur (
    IzinTurID INT PRIMARY KEY IDENTITY(1,1),
    TurAdi NVARCHAR(100)
);

CREATE TABLE Izin (
    IzinID INT PRIMARY KEY IDENTITY(1,1),
    KullaniciID INT FOREIGN KEY REFERENCES Kullanici(KullaniciID),
    IzinTurID INT FOREIGN KEY REFERENCES IzinTur(IzinTurID),
    BaslangicTarih DATE,
    BitisTarih DATE,
    Aciklama NVARCHAR(500),
    Durum NVARCHAR(50),
    OlusturmaTarihi DATETIME DEFAULT GETDATE()
);

CREATE TABLE FazlaMesai (
    MesaiID INT PRIMARY KEY IDENTITY(1,1),
    KullaniciID INT FOREIGN KEY REFERENCES Kullanici(KullaniciID),
    Tarih DATE,
    Saat DECIMAL(5,2),
    Aciklama NVARCHAR(500),
    Durum NVARCHAR(50),
    OlusturmaTarihi DATETIME DEFAULT GETDATE()
);