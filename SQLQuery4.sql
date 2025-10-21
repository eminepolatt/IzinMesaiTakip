ALTER TABLE FazlaMesai
ALTER COLUMN Durum bit NULL

ALTER TABLE FazlaMesai
ALTER COLUMN Durum bit NULL

ALTER TABLE Izin
ALTER COLUMN Durum bit NULL;

UPDATE Izin SET Durum = 0 WHERE Durum = 'Bekliyor';
UPDATE Izin SET Durum = 1 WHERE Durum = 'Onaylandı';
UPDATE Izin SET Durum = 0 WHERE Durum = 'Bekliyor';
UPDATE Izin SET Durum = 1 WHERE Durum = 'Onaylandı';

-- 1. önce yeni bir geçici sütun ekle
ALTER TABLE Izin ADD DurumTemp bit;

-- 2. metinsel değerleri uygun bit değerine yaz
UPDATE Izin SET DurumTemp = 0 WHERE Durum = 'Bekliyor';
UPDATE Izin SET DurumTemp = 1 WHERE Durum = 'Onaylandı';

ALTER TABLE Izin ALTER COLUMN Durum nvarchar(20);
ALTER TABLE Izin ADD DurumBit bit NULL;

UPDATE Izin SET DurumBit = 0 WHERE Durum = 'Bekliyor';
UPDATE Izin SET DurumBit = 1 WHERE Durum = 'Onaylandı';
SELECT DISTINCT Durum FROM Izin;



-- 3. eski metin sütununu sil
ALTER TABLE Izin DROP COLUMN Durum;

-- 4. geçici sütunu asıl sütun ismine çevir
EXEC sp_rename 'Izin.DurumTemp', 'Durum', 'COLUMN';

ALTER TABLE FazlaMesai
ALTER COLUMN Durum bit;
ALTER TABLE FazlaMesai
ALTER COLUMN Durum BIT NULL;
SELECT COLUMN_NAME, DATA_TYPE  
FROM INFORMATION_SCHEMA.COLUMNS  
WHERE TABLE_NAME = 'Izin' AND COLUMN_NAME = 'Durum';
ALTER TABLE Izin
ALTER COLUMN Durum bit;
