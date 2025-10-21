

-- Örnek: 'Onaylandı' olanları true (1) yapalım
UPDATE Izin
SET Durum = 1
WHERE Durum = 'Onaylandı';

-- Örnek: 'Bekliyor' olanları false (0) yapalım
UPDATE Izin
SET Durum = 0
WHERE Durum = 'Bekliyor';


ALTER TABLE Izin
ALTER COLUMN Durum BIT NULL;