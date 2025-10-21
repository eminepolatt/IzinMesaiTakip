ALTER TABLE FazlaMesai
ALTER COLUMN Durum bit NULL;

-- Bekliyor → 0, Onaylandı → 1 yapalım
UPDATE FazlaMesai
SET Durum = 
    CASE 
        WHEN Durum = 'Bekliyor' THEN 0
        WHEN Durum = 'Onaylandı' THEN 1
    END;

    SELECT DISTINCT Durum FROM FazlaMesai;
    ALTER TABLE FazlaMesai
ALTER COLUMN Durum bit NULL;


