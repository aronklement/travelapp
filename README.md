Egyetemi beadandó, a feladatleírás a travelapp.pdf fájlban található
# Rétegzett konzolos alkalmazás – Utazáskezelő rendszer

Ez a projekt a **Haladó fejlesztési technikák** tantárgy féléves feladataként készült  
(**.NET 8**, C#).

A feladat célja egy **adatbázis-alapú, rétegzett alkalmazás** megvalósítása volt  
**SOLID elvek**, **Dependency Injection** és **Entity Framework Core** használatával.

---

## Rövid leírás

Az alkalmazás JSON fájlokból beolvasott utazási adatokat kezel, adatbázisban tárolja őket,
és konzolos felületen keresztül lehetőséget ad légitársaságok és utazási ajánlatok
kezelésére, keresésére és riportok készítésére.

---

## Architektúra

A megoldás **rétegzett felépítést** követ:

- **Console** – felhasználói interakció, IoC konténer
- **Application** – üzleti logika
- **Model** – domain entitások
- **Persistence.MsSql** – adatbázis-kezelés (EF Core)
- **Test** – unit tesztek

A rétegek közötti kommunikáció **interfészeken keresztül** történik.

---

## Fő funkciók

- JSON fájl beolvasása és mentése adatbázisba
- Légitársaságok és utazások hozzáadása, módosítása, törlése
- Esemény kiváltása új legolcsóbb utazás esetén
- Riportok generálása (átlagár, minimum, maximum, népszerű város)
- Keresés város, ár és távolság alapján
- Konzolos felhasználói felület

---
