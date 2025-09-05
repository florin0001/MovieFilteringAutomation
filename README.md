# MovieFilteringAutomation

Proiect de automatizare pentru testarea functionalitatii de filtrare a filmelor pe TheMovieDB.org folosind Selenium si API-uri REST.

## Ce face proiectul

Am creat un framework de testare care verifica daca filtrele de pe site functioneaza corect, atat prin interfata web cat si prin API-uri. Testez filtrarea dupa data lansarii, genuri si scorul utilizatorilor.

## Structura proiectului

```
├── Framework/          # Componentele de baza ale framework-ului
├── Tests.UI/          # Teste pentru interfata web cu Selenium
├── Tests.API/         # Teste pentru API-urile TMDB
```

## Cum sa rulezi testele

### 1. Ce iti trebuie

- .NET 8.0
- Chrome browser
- API key de la TMDB (il iei de pe https://www.themoviedb.org/settings/api)

### 2. Configurare

Seteaza API key-ul:
```bash
cd Tests.API
dotnet user-secrets set TMDB_API_KEY "cheia_ta_api"
```

### 3. Ruleaza testele

```bash
# Toate testele
dotnet test

# Doar testele UI
dotnet test Tests.UI

# Doar testele API
dotnet test Tests.API
```

## Ce testez

**Task 1 - Filtrarea UI:**
- Sortare dupa data lansarii (crescator)
- Selectie genuri multiple (Action + Adventure)  
- Filtrare dupa perioada 1990-2005

**Task 2 - Verificare:**
- Controlez ca filtrarea s-a aplicat corect
- Verific sortarea si intervalele de date

**Task 3 - Comparatie API vs UI:**
- Iau lista de filme prin API
- Aplic aceleasi filtre prin API
- Compar rezultatele din UI cu cele din API

## Tehnologii folosite

- **Selenium WebDriver** pentru testele UI
- **HttpClient** pentru testele API
- **NUnit** ca framework de testare
- **Page Object Model** pentru organizarea codului
- **JSON.NET** pentru procesarea raspunsurilor API

## Probleme intalnite

- Site-ul TMDB se schimba des, uneori trebuie actualizate selectorii CSS
- API-ul TMDB are limite de rate (40 cereri per 10 secunde)
- Elementele web pot fi incarcate cu intarziere

## Cum sa rulezi

1. Cloneaza repository-ul
2. Seteaza API key-ul cum e explicat mai sus
3. Ruleaza `dotnet restore` pentru dependinte
4. Ruleaza `dotnet test` pentru toate testele
