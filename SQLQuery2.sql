use IzinMesaiTakip;
go

INSERT INTO Rol (RolAdi) VALUES ('Admin');
INSERT INTO Rol (RolAdi) VALUES ('Yönetici');
INSERT INTO Rol (RolAdi) VALUES ('Çalışan');

INSERT INTO Departman (DepartmanAdi) VALUES ('İnsan Kaynakları');
INSERT INTO Departman (DepartmanAdi) VALUES ('Bilgi İşlem');
INSERT INTO Departman (DepartmanAdi) VALUES ('Muhasebe');

-- Çalışan
INSERT INTO Kullanici (Ad, Soyad, Eposta, Sifre, RolID, DepartmanID)
VALUES ('Zeynep', 'Kaya', 'zeynep.kaya@firma.com', '1234', 3, 2);

-- Yönetici
INSERT INTO Kullanici (Ad, Soyad, Eposta, Sifre, RolID, DepartmanID)
VALUES ('Ayşe', 'Yılmaz', 'ayse.yilmaz@firma.com', 'yonetici456', 2, 2);

-- Admin
INSERT INTO Kullanici (Ad, Soyad, Eposta, Sifre, RolID, DepartmanID)
VALUES ('Ahmet', 'Demir', 'ahmet.demir@firma.com', 'admin123', 1, 1);


INSERT INTO IzinTur (TurAdi) VALUES ('Yıllık İzin');
INSERT INTO IzinTur (TurAdi) VALUES ('Mazeret İzni');
INSERT INTO IzinTur (TurAdi) VALUES ('Ücretsiz İzin');

-- Zeynep’in yıllık izin başvurusu (ID: 1)
INSERT INTO Izin (KullaniciID, IzinTurID, BaslangicTarih, BitisTarih, Aciklama, Durum, OlusturmaTarihi)
VALUES (1, 1, '2025-08-01', '2025-08-05', 'Yıllık izin kullanmak istiyorum.', 'Bekliyor', GETDATE());

-- Zeynep’in mazeret izni başvurusu
INSERT INTO Izin (KullaniciID, IzinTurID, BaslangicTarih, BitisTarih, Aciklama, Durum, OlusturmaTarihi)
VALUES (1, 2, '2025-08-10', '2025-08-11', 'Acil ailevi durum.', 'Onaylandı', GETDATE());

-- Zeynep’in fazla mesaisi
INSERT INTO FazlaMesai (KullaniciID, Tarih, Saat, Aciklama, Durum, OlusturmaTarihi)
VALUES (1, '2025-08-12', 3.5, 'Proje yetiştirmek için çalışıldı.', 'Bekliyor', GETDATE());

-- Başka bir fazla mesai (önceden onaylanmış)
INSERT INTO FazlaMesai (KullaniciID, Tarih, Saat, Aciklama, Durum, OlusturmaTarihi)
VALUES (1, '2025-07-28', 2, 'Sunum hazırlığı.', 'Onaylandı', GETDATE());

SELECT * FROM Rol;
SELECT * FROM Departman;
SELECT * FROM Kullanici;
SELECT * FROM IzinTur;
SELECT * FROM Izin;
SELECT * FROM FazlaMesai;

